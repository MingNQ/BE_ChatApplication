using Application.Common.Interfaces;

namespace Application.Identity.Tokens;

public interface ITokenService : ITransientService
{
    Task<TokenResponse> GetTokenAsync(long userId, bool rememberme, string ipAddress, CancellationToken cancellationToken);

    Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest, string ipAddress, CancellationToken cancellationToken);

    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
}