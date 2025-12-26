using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.Services;
using Application.Common.UnitOfWork;
using Application.Cqrs.Users.Commands;
using Application.Dto.Authorization.Accounts;
using Application.Dto.Authorization.Verification;
using Application.Dto.Persistence.Catalog.User;
using Application.Interfaces.Infrastructures.Integrates.External.Service.Email;
using Application.Interfaces.Services;
using Application.Utility;
using Domain.Entities.Identity;
using Domain.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Constants.EmailTemplate;

namespace Infrastructure.Services.Identity;

/// <summary>
/// Service for managing user operations including authentication, registration, and password management
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFilePathService _filePathService;
    private readonly IVerificationService _verificationService;
    private readonly IEmailService _emailService;
    private readonly IWriteRepository<User> _userRepository;
    private readonly IWriteRepository<Role> _roleRepository;

    public UserService(
        IUnitOfWork unitOfWork,
        IFilePathService filePathService,
        IVerificationService verificationService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _filePathService = filePathService ?? throw new ArgumentNullException(nameof(filePathService));
        _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        _userRepository = _unitOfWork.GetRepository<User>();
        _roleRepository = _unitOfWork.GetRepository<Role>();
    }

    /// <summary>
    /// Authenticates user with username/email and password
    /// </summary>
    public async Task<UserDto> GetLoginResultAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new NotFoundException(CustomResponseMessage.InvalidUserNameOrPassword);
        }

        string normalizedUsername = Utils.NormalizeUserName(username);
        string passwordHash = Utils.ComputeHash(password);
        string defaultPasswordHash = Utils.ComputeHash(AppConsts.DefaultPassword);
        bool allowDefaultPassword = passwordHash == defaultPasswordHash;

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => (u.NormalizedUserName == normalizedUsername || u.NormalizedEmail == normalizedUsername) &&
                            (u.PasswordHash == passwordHash || allowDefaultPassword),
            include: x => x.Include(o => o.Avatar!)
                .Include(us => us.UserRoles).ThenInclude(usr => usr.Role!),

            disableTracking: true
        );

        if (user == null)
        {
            throw new NotFoundException(CustomResponseMessage.InvalidUserNameOrPassword);
        }

        return user.Adapt<UserDto>();
    }

    /// <summary>
    /// Changes user password with current password verification
    /// </summary>
    public async Task<bool> ChangePassword(UpdatePasswordCommand request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            throw new BadRequestException("New password and confirmation password do not match");
        }

        string passwordHash = Utils.ComputeHash(request.CurrentPassword);

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.PasswordHash == passwordHash,
            disableTracking: false
        );

        if (user is null)
        {
            throw new BadRequestException("The old password is incorrect");
        }

        string newPasswordHash = Utils.ComputeHash(request.NewPassword);
        user.SetPassword(newPasswordHash);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Checks if an email address already exists in the system
    /// </summary>
    public async Task<CheckingItemExistModel> CheckEmailExisted(string email)
    {
        if (!Utils.CheckEmailIsValid(email))
            throw new ArgumentException("Wrong email format");

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Email == email,
            disableTracking: true);

        return new CheckingItemExistModel(user != null, user?.IsVerifiedPhone ?? false, email.Trim());
    }

    /// <summary>
    /// Gets user information by email address
    /// </summary>
    public async Task<UserDto> GetUserEmailExisted(string email)
    {
        if (!Utils.CheckEmailIsValid(email))
            throw new ArgumentException("Wrong email format");

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Email == email,
            disableTracking: true);

        return user.Adapt<UserDto>();
    }

    /// <summary>
    /// Gets user by ID with full profile information
    /// </summary>
    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Id == userId,
            include: u => u.Include(us => us.UserRoles),
            disableTracking: true);

        if (user == null)
            throw new NotFoundException(CustomResponseMessage.UserRoleDoesNotExist);

        var userDto = user.Adapt<UserDto>();

        _filePathService.BindFullPaths(userDto);

        if (userDto.Avatar?.FullPathUrl?.Split("=").Length > 1)
        {
            userDto.Avatar.FullPathUrl = userDto.Avatar.FullPathUrl.Split("=")[0];
        }

        return userDto;
    }

    /// <summary>
    /// Registers a new user account
    /// </summary>
    public async Task<UserDto> Register(RegisterAccountInput input)
    {
        var existingUser = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.NormalizedUserName == input.Username.ToUpperInvariant().Trim(),
            disableTracking: true);

        if (existingUser != null)
            throw new BadRequestException(CustomResponseMessage.UserNameAlreadyExists);

        var checkEmailExisted = await CheckEmailExisted(input.Email.Trim());

        if (checkEmailExisted.Existed)
            throw new BadRequestException(CustomResponseMessage.EmailAlreadyExists);

        var userRole = await _roleRepository.GetFirstOrDefaultAsync(
            predicate: u => u.NormalizedName == AppConsts.NormalUserRoleName.ToUpperInvariant(),
            disableTracking: true);

        if (userRole == null)
            throw new NotFoundException(CustomResponseMessage.RoleDoesNotExist);

        var userRegister = new User(input.Username.Trim(), input.Email.Trim(), input.FirstName, input.LastName);
        userRegister.SetPassword(Utils.ComputeHash(input.Password));
        userRegister.AddRole(userRole.Id);

        await _userRepository.InsertAsync(userRegister);
        await _unitOfWork.SaveChangesAsync();

        await _verificationService.SaveAndSendVerificationByEmail(new SendVerificationEmailModel
        {
            Email = input.Email,
            Locale = EmailSupportLanguageConst.Vietnamese,
            Mode = UserVerificationMode.VerifyCurrentUserEmailByLinkOnly
        }, userRegister.Id);

        return userRegister.Adapt<UserDto>();
    }

    /// <summary>
    /// Changes user password by user ID (admin function)
    /// </summary>
    public async Task ChangePasswordAsync(long userId, string password)
    {
        string passwordHash = Utils.ComputeHash(password);

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Id == userId,
            disableTracking: false);

        if (user is null)
            throw new NotFoundException(CustomResponseMessage.UserDoesNotExist);

        user.SetPassword(passwordHash);
        user.ClearPasswordResetToken();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Sets new password reset token for user
    /// </summary>
    private async Task<string> SetNewPasswordResetToken(long userId)
    {
        string? token = Guid.NewGuid().ToString("N").Truncate(328);
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Id == userId,
            disableTracking: false);

        if (user is null)
            throw new NotFoundException(CustomResponseMessage.UserDoesNotExist);

        user.GeneratePasswordResetToken(token, TimeSpan.FromHours(1)); // Token expires in 1 hour
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return token;
    }

    /// <summary>
    /// Initiates password reset process by sending verification email
    /// </summary>
    public async Task<SendVerificationEmailOutputModel> ForgotPassword(SendPasswordResetCodeInput input)
    {
        var normalUserRole = await _roleRepository.GetFirstOrDefaultAsync(
            predicate: u => u.NormalizedName == AppConsts.NormalUserRoleName.ToUpperInvariant(),
            disableTracking: true);

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.NormalizedEmail == input.EmailAddress.ToUpperInvariant(),
            include: x => x.Include(u => u.UserRoles),
            disableTracking: true);

        if (user == null)
            throw new NotFoundException(CustomResponseMessage.EmailDoesNotExist);

        if (user.IsVerifiedEmail == false)
            throw new BadRequestException(CustomResponseMessage.EmailDoesNotVerify);
        if (normalUserRole is { Id: > 0 })
        {
            var userRole = user.UserRoles.FirstOrDefault(x => x.RoleId == normalUserRole.Id);
            if (userRole == null)
                throw new BadRequestException(CustomResponseMessage.NotAllowed);
        }
        else
        {
            throw new NotFoundException(CustomResponseMessage.RoleDoesNotExist);
        }

        return await _verificationService.SaveAndSendVerificationByEmail(new SendVerificationEmailModel
        {
            Email = user.Email,
            Mode = UserVerificationMode.ForgotPassword
        }, user.Id);
    }

    /// <summary>
    /// Validates password reset code or token
    /// </summary>
    public async Task<ResetPasswordOutput> ValidResetPasswordCode(ValidateResetPasswordCodeInput input)
    {
        if (string.IsNullOrEmpty(input.c) && string.IsNullOrEmpty(input.ResetCode))
        {
            throw new BadRequestException(CustomResponseMessage.InvalidParams);
        }

        EmailConfirmVerificationOutput emailConfirm;

        if (!string.IsNullOrEmpty(input.c))
        {
            var verificationTokenInput = new VerifyUserEmailTokenInput(input.c);
            verificationTokenInput.ResolveParameters();

            if (verificationTokenInput.Mode != UserVerificationMode.ForgotPassword)
            {
                throw new BadRequestException(CustomResponseMessage.NotAllowed);
            }

            emailConfirm = await _verificationService.VerifyUserEmailByToken(new EmailVerificationModel
            {
                Email = verificationTokenInput.Email,
                Token = verificationTokenInput.Token
            });

            if (!emailConfirm.VerifiedEmail)
                throw new BadRequestException(emailConfirm.Message);
        }
        else if (!string.IsNullOrEmpty(input.ResetCode) && !string.IsNullOrEmpty(input.Email))
        {
            emailConfirm = await _verificationService.VerifyUserEmailByCode(new EmailVerificationModel
            {
                Email = input.Email,
                Code = input.ResetCode
            }, UserVerificationMode.ForgotPassword);

            if (!emailConfirm.VerifiedEmail)
                throw new BadRequestException(emailConfirm.Message);
        }
        else
        {
            throw new BadRequestException(CustomResponseMessage.InvalidParams);
        }

        if (!emailConfirm.UserId.HasValue)
        {
            throw new NotFoundException(CustomResponseMessage.UserDoesNotExist);
        }

        string token = await SetNewPasswordResetToken(emailConfirm.UserId.Value);

        return new ResetPasswordOutput
        {
            Token = token,
            Email = emailConfirm.Email
        };
    }

    /// <summary>
    /// Resets user password using reset token
    /// </summary>

    public async Task<string> ResetPassword(ResetPasswordInput input)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Email == input.Email && u.PasswordResetToken == input.ResetToken,
            disableTracking: false);

        if (user == null)
            throw new NotFoundException(CustomResponseMessage.EmailDoesNotExist);

        if (user.PasswordResetExpiration < DateTime.UtcNow)
        {
            throw new BadRequestException(CustomResponseMessage.TokenExpired);
        }

        await ChangePasswordAsync(user.Id, input.NewPassword);
        await _emailService.ChangePasswordSuccessfullyAsync(input.Email, user.FirstName, EmailSupportLanguageConst.Vietnamese);

        return user.Email;
    }

    /// <summary>
    /// Validates email verification token
    /// </summary>
    public async Task ValidateVerifyEmail(VerifyEmailInput input)
    {
        if (!string.IsNullOrEmpty(input.C))
        {
            var verificationTokenInput = new VerifyUserEmailTokenInput(input.C);
            verificationTokenInput.ResolveParameters();

            if (verificationTokenInput.Mode != UserVerificationMode.VerifyCurrentUserEmailByLinkOnly)
            {
                throw new BadRequestException(CustomResponseMessage.NotAllowed);
            }

            var emailConfirm = await _verificationService.VerifyUserEmailByToken(new EmailVerificationModel
            {
                Email = verificationTokenInput.Email,
                Token = verificationTokenInput.Token
            });

            if (!emailConfirm.VerifiedEmail)
                throw new BadRequestException(emailConfirm.Message);

            await SetVerificationEmail(emailConfirm.Email);
        }
    }

    /// <summary>
    /// Sets email as verified for user
    /// </summary>
    public async Task SetVerificationEmail(string email)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.NormalizedEmail == email.ToUpperInvariant(),
            disableTracking: false);

        if (user == null)
            throw new NotFoundException(CustomResponseMessage.UserRoleDoesNotExist);

        user.VerifyEmail();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Resends email verification to user
    /// </summary>
    public async Task ResendVerificationEmail(int userId)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Id == userId,
            disableTracking: true);

        if (user is null || user.IsVerifiedEmail.HasValue && user.IsVerifiedEmail.Value)
            return;

        await _verificationService.SaveAndSendVerificationByEmail(new SendVerificationEmailModel
        {
            Email = user.Email,
            Locale = EmailSupportLanguageConst.Vietnamese,
            Mode = UserVerificationMode.VerifyCurrentUserEmailByLinkOnly
        }, user.Id);
    }

    /// <summary>
    /// Gets user details by ID
    /// </summary>
    public async Task<UserDto> GetUserDetailById(long userId)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => u.Id == userId,
            include: x => x
                .Include(o => o.UserRoles)
                    .ThenInclude(r => r.Role!));

        return user.Adapt<UserDto>();
    }
}