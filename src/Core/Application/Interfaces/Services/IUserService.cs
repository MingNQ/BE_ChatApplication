using Application.Cqrs.Users.Commands;
using Application.Dto.Authorization.Accounts;
using Application.Dto.Authorization.Verification;
using Application.Dto.Persistence.Catalog.User;

namespace Application.Interfaces.Services;

public interface IUserService
{
    /// <summary>
    /// Authenticates user and returns user data
    /// </summary>
    Task<UserDto> GetLoginResultAsync(string username, string password);

    /// <summary>
    /// Changes user password using current password verification
    /// </summary>
    //Task<bool> ChangePassword(UpdatePasswordCommand request);

    /// <summary>
    /// Checks if email exists in the system
    /// </summary>
    Task<CheckingItemExistModel> CheckEmailExisted(string email);

    /// <summary>
    /// Gets user by email address
    /// </summary>
    Task<UserDto> GetUserEmailExisted(string email);

    /// <summary>
    /// Gets user by ID with roles and avatar
    /// </summary>
    Task<UserDto> GetUserByIdAsync(int userId);

    /// <summary>
    /// Registers a new user account
    /// </summary>
    Task<UserDto> Register(RegisterAccountInput input);

    /// <summary>
    /// Changes user password by user ID (admin function)
    /// </summary>
    Task ChangePasswordAsync(long userId, string password);

    /// <summary>
    /// Changes user password
    /// </summary>
    Task<bool> ChangePassword(UpdatePasswordCommand request);

    /// <summary>
    /// Initiates password reset process
    /// </summary>
    Task<SendVerificationEmailOutputModel> ForgotPassword(SendPasswordResetCodeInput input);

    /// <summary>
    /// Validates password reset code/token
    /// </summary>
    Task<ResetPasswordOutput> ValidResetPasswordCode(ValidateResetPasswordCodeInput input);

    /// <summary>
    /// Resets password using reset token
    /// </summary>
    Task<string> ResetPassword(ResetPasswordInput input);

    /// <summary>
    /// Validates email verification
    /// </summary>
    Task ValidateVerifyEmail(VerifyEmailInput input);

    /// <summary>
    /// Sets email as verified
    /// </summary>
    Task SetVerificationEmail(string email);

    /// <summary>
    /// Resends email verification
    /// </summary>
    Task ResendVerificationEmail(int userId);

    /// <summary>
    /// Gets user details by ID
    /// </summary>
    Task<UserDto> GetUserDetailById(long userId);
}