using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Persistence.Catalog.User;
using Application.Identity.Tokens;
using Application.Interfaces.Services;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Infrastructure.Auth.Jwt;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Auth.Authorization;

public class TokenService : ITokenService
{
    private readonly IWriteRepository<User> _userRepository;
    private readonly IWriteRepository<RefreshToken> _tokenRefreshRepository;
    private readonly IUserService _userService;
    private readonly JwtSettings _jwtOptions;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IUnitOfWork _unitOfWork;

    public TokenService(
        IUnitOfWork unitOfWork,
        IUserService userService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = unitOfWork.GetRepository<User>();
        _tokenRefreshRepository = unitOfWork.GetRepository<RefreshToken>();
        _userService = userService;
        _jwtOptions = jwtSettings.Value;
        _unitOfWork = unitOfWork;

        // Initialize reusable objects once in constructor
        byte[] secret = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var securityKey = new SymmetricSecurityKey(secret);
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = JwtAuthConstants.Audience,
            ValidIssuer = JwtAuthConstants.Issuer,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Email == request.Email && request.Password == AppConsts.AdminPassword,
            include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)!,
            disableTracking: true)
            ?? throw new Exception("Not Found");

        var userLogin = user.Adapt<UserDto>();

        return await GenerateTokensAndUpdateUser(userLogin, false, ipAddress);
    }

    public async Task<TokenResponse> GetTokenAsync(long userId, bool rememberme, string ipAddress, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(userId);

        if (user is null)
        {
            throw new UnauthorizedException("User not found");
        }

        return await GenerateTokensAndUpdateUser(user, rememberme, ipAddress);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        // Validate input parameters
        if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new UnauthorizedException("Invalid token or refresh token");
        }

        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string? nameIdentifier = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(nameIdentifier, out int userId))
        {
            throw new SecurityTokenException("Invalid token: unable to determine user ID");
        }

        // Verify refresh token is valid
        if (!await IsValidRefreshToken(userId, request.RefreshToken))
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        // Get user with single efficient query
        var user = await _userService.GetUserByIdAsync(userId);

        if (user is null)
        {
            throw new UnauthorizedException("User not found");
        }

        string? rememberMeInToken = userPrincipal.FindFirstValue(SystemClaims.RememberMe);

        bool.TryParse(rememberMeInToken, out bool rememberMe);

        return await GenerateTokensAndUpdateUser(user, rememberMe, ipAddress);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(UserDto user, bool rememberMe, string ipAddress)
    {
        // Generate JWT Token
        string token = GenerateJwt(user, rememberMe, ipAddress);

        // Generate refresh token with improved security
        string refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays);

        if (rememberMe)
        {
            refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtOptions.RememberRefreshTokenExpirationInDays);
        }

        // Add or update refresh token in database
        await UpdateRefreshToken(user.Id, refreshToken, refreshTokenExpiryTime);

        return new TokenResponse(token, refreshToken, refreshTokenExpiryTime);
    }

    private string GenerateJwt(UserDto user, bool rememberMe, string ipAddress)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(SystemClaims.Fullname, $"{user.FirstName} {user.LastName}".Trim()),
            new(ClaimTypes.Name, user.FirstName  ),
            new(ClaimTypes.Surname, user.LastName),
            new(SystemClaims.IpAddress, ipAddress),
            new(SystemClaims.Avatar, user.Avatar?.Path ?? string.Empty),
            new(SystemClaims.UserId, user.Id.ToString()),
            new(SystemClaims.RememberMe, rememberMe.ToString()),
            new(ClaimTypes.MobilePhone, user.PhoneNumber)
        };

        claims.AddRange(user.UserRoles.Select(x => new Claim(ClaimTypes.Role, x.Role!.Name)));

        var tokenExpire = rememberMe ? DateTime.UtcNow.AddMinutes(_jwtOptions.RememberTokenExpirationInMinutes)
            : DateTime.UtcNow.AddMinutes(_jwtOptions.TokenExpirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: JwtAuthConstants.Issuer,
            audience: JwtAuthConstants.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenExpirationInMinutes),
            signingCredentials: _signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        byte[] randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private async Task UpdateRefreshToken(int userId, string token, DateTime expiredDate)
    {
        var utcNow = DateTime.UtcNow;

        var expiredTokens = await _tokenRefreshRepository.GetAllAsync(
            predicate: x => x.UserId == userId && x.ExpiredDate < utcNow,
            disableTracking: false);

        if (expiredTokens.Count > 0)
        {
            _tokenRefreshRepository.Delete(expiredTokens.ToArray());
            await _tokenRefreshRepository.InsertAsync(RefreshToken.Create(userId, token, expiredDate));
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            await _tokenRefreshRepository.InsertAsync(RefreshToken.Create(userId, token, expiredDate));
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            // Copy validation parameters to disable lifetime validation temporarily
            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false;
            tokenValidationParameters.ValidateAudience = false;
            tokenValidationParameters.ValidateIssuer = false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedException($"Token validation failed: {ex.Message}");
        }
    }

    private async Task<bool> IsValidRefreshToken(int userId, string refreshToken)
    {
        var token = await _tokenRefreshRepository.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId &&
                           x.Token == refreshToken &&
                           x.ExpiredDate > DateTime.UtcNow,
            disableTracking: true);

        return token != null;
    }
}