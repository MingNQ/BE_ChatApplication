using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Cqrs.Identity.Users.Commands;
using Application.Dto.Authorization.Accounts;
using Application.Dto.Authorization.Verification;
using Application.Dto.Persistence.Catalog.User;
using Application.Interfaces.Services;
using Application.Utility;
using Domain.Entities.Identity;
using Domain.Events.Verification;
using Domain.ValueObjects.Verification;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Infrastructure.Services.Identity;

/// <summary>
/// Service for managing user operations including authentication, registration, and password management
/// </summary>

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVerificationService _verificationService;
    private readonly IWriteRepository<User> _userRepository;
    private readonly IWriteRepository<Role> _roleRepository;

    public UserService(IUnitOfWork unitOfWork, IVerificationService verificationService)
    {
        _unitOfWork = unitOfWork;
        _verificationService = verificationService;
        _userRepository = unitOfWork.GetRepository<User>();
        _roleRepository = unitOfWork.GetRepository<Role>();
    }

    public async Task<bool> ChangePassword(UpdatePasswordCommand request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            throw new ArgumentException("New password and confirmation password do not match");
        }

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.UserId,
            disableTracking: false);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var currentPasswordHash = Utils.ComputeHash(request.CurrentPassword);
        if (user.PasswordHash != currentPasswordHash)
        {
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        var newPasswordHash = Utils.ComputeHash(request.NewPassword);
        user.SetPassword(newPasswordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task ChangePasswordAsync(int userId, string password)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == userId,
            disableTracking: false);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var passwordHash = Utils.ComputeHash(password);
        user.SetPassword(passwordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<CheckingItemExistModel> CheckEmailExisted(string email)
    {
        var normalizedEmail = Utils.NormalizeEmail(email);
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedEmail == normalizedEmail,
            disableTracking: true);

        if (user == null)
        {
            return new CheckingItemExistModel(email);
        }

        return new CheckingItemExistModel(
            existed: true,
            activated: user.IsVerifiedEmail ?? false,
            value: email)
        {
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash)
        };
    }

    public async Task<SendVerificationEmailOutputModel> ForgotPassword(SendPasswordResetCodeInput input)
    {
        var normalizedEmail = Utils.NormalizeEmail(input.EmailAddress);
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedEmail == normalizedEmail,
            disableTracking: true);

        if (user == null)
        {
            return new SendVerificationEmailOutputModel
            {
                Email = input.EmailAddress,
                Sent = false,
                Message = "User not found with this email address"
            };
        }

        var contactInfo = ContactInfo.CreateEmail(input.EmailAddress);

        // Check if there's already an active verification
        var existingVerification = await _verificationService.FindActiveVerification(
            contactInfo, VerificationMode.ForgotPassword, user.Id);

        if (existingVerification != null)
        {
            return new SendVerificationEmailOutputModel
            {
                Email = input.EmailAddress,
                UserId = (int)user.Id,
                Code = existingVerification.VerificationCode,
                Sent = true,
                Message = $"Password reset code already sent to {contactInfo.ValueMask()}"
            };
        }

        // Create new verification for password reset
        var verification = UserVerification.CreateForForgotPassword(contactInfo, user.Id);
        await _verificationService.InsertVerificationAsync(verification, CancellationToken.None);

        return new SendVerificationEmailOutputModel
        {
            Email = input.EmailAddress,
            UserId = (int)user.Id,
            Code = verification.VerificationCode,
            Sent = true,
            Message = $"Password reset code sent to {contactInfo.ValueMask()}"
        };
    }

    public async Task<UserDto> GetLoginResultAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username and password are required");
        }

        var normalizedUsername = Utils.NormalizeUserName(username);
        var passwordHash = Utils.ComputeHash(password);

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedUserName == normalizedUsername || x.NormalizedEmail == normalizedUsername,
            include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Avatar!),
            disableTracking: false);

        if (user == null || user.PasswordHash != passwordHash)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        return user.Adapt<UserDto>();
    }

    public async Task<UserDto> GetUserByIdAsync(long userId)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == userId,
            include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Avatar!),
            disableTracking: true);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        return user.Adapt<UserDto>();
    }

    public async Task<UserDto> GetUserDetailById(long userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Invalid user ID");
        }

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == userId,
            include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Avatar!),
            disableTracking: true);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        return user.Adapt<UserDto>();
    }

    public async Task<UserDto> GetUserEmailExisted(string email)
    {
        if (!Utils.CheckEmailIsValid(email))
        {
            throw new ArgumentException("Email is required");
        }

        var normalizedEmail = Utils.NormalizeEmail(email);
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedEmail == normalizedEmail,
            include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Avatar!),
            disableTracking: true);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        return user.Adapt<UserDto>();
    }

    public async Task<UserDto> Register(RegisterAccountInput input)
    {
        var normalizedUsername = Utils.NormalizeUserName(input.Username);
        var normalizedEmail = Utils.NormalizeEmail(input.Email);

        // Check if username or email already exists
        var existingUser = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedUserName == normalizedUsername || x.NormalizedEmail == normalizedEmail,
            disableTracking: true);

        if (existingUser != null)
        {
            throw new ArgumentException("Username or email already exists");
        }

        // Create new user
        var user = User.Create(input.Username, input.Email, input.FirstName, input.LastName);
        var passwordHash = Utils.ComputeHash(input.Password);
        user.SetPassword(passwordHash);

        // Assign default user role
        var defaultRole = await _roleRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedName == AppConsts.NormalUserRoleName.ToUpperInvariant(),
            disableTracking: true);

        if (defaultRole != null)
        {
            user.AddRole(defaultRole.Id);
        }

        await _userRepository.InsertAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Get the created user with roles and avatar
        var createdUser = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == user.Id,
            include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Avatar!),
            disableTracking: true);

        return createdUser!.Adapt<UserDto>();
    }

    public async Task ResendVerificationEmail(int userId)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == userId,
            disableTracking: true);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("User email is not set");
        }

        var contactInfo = ContactInfo.CreateEmail(user.Email);

        // Check if there's already an active verification
        var existingVerification = await _verificationService.FindActiveVerification(
            contactInfo, VerificationMode.EmailVerification, user.Id);

        if (existingVerification != null)
        {
            // Resend existing verification
            existingVerification.Resend();
            await _verificationService.UpdateVerificationAsync(existingVerification);
        }
        else
        {
            // Create new verification for email verification using the basic Create method
            var code = VerificationCodeObject.CreateForSignUp();
            var verification = UserVerification.Create(code.Value, user.Id, string.Empty, user.Email);

            // Set the mode to EmailVerification using reflection since it's private
            var verificationType = typeof(UserVerification);
            var modeField = verificationType.GetField("Mode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            modeField?.SetValue(verification, VerificationMode.EmailVerification.ToString());

            await _verificationService.InsertVerificationAsync(verification, CancellationToken.None);
        }
    }

    public async Task<string> ResetPassword(ResetPasswordInput input)
    {
        var contactInfo = ContactInfo.CreateEmail(input.Email);
        var verification = await _verificationService.FindActiveVerification(
            contactInfo, VerificationMode.ForgotPassword);

        if (verification == null)
        {
            throw new ArgumentException("No active password reset verification found");
        }

        if (!verification.VerifyToken(input.ResetToken))
        {
            throw new ArgumentException("Invalid or expired reset token");
        }

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedEmail == Utils.NormalizeEmail(input.Email),
            disableTracking: false);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var newPasswordHash = Utils.ComputeHash(input.NewPassword);
        user.SetPassword(newPasswordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return "Password reset successfully";
    }

    public async Task SetVerificationEmail(string email)
    {
        if (!Utils.CheckEmailIsValid(email))
        {
            throw new ArgumentException("Email is required");
        }

        var normalizedEmail = Utils.NormalizeEmail(email);
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedEmail == normalizedEmail,
            disableTracking: false);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        user.VerifyEmail();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ValidateVerifyEmail(VerifyEmailInput input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (!Utils.CheckEmailIsValid(input.Email) || string.IsNullOrWhiteSpace(input.VerifyCode))
        {
            throw new ArgumentException("Email and verification code are required");
        }

        var contactInfo = ContactInfo.CreateEmail(input.Email);
        var verification = await _verificationService.FindActiveVerification(
            contactInfo, VerificationMode.EmailVerification);

        if (verification == null)
        {
            throw new ArgumentException("No active email verification found");
        }

        if (!verification.VerifyCode(input.VerifyCode))
        {
            throw new ArgumentException("Invalid or expired verification code");
        }

        // Mark email as verified
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.NormalizedEmail == Utils.NormalizeEmail(input.Email),
            disableTracking: false);

        if (user != null)
        {
            user.VerifyEmail();
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<ResetPasswordOutput> ValidResetPasswordCode(ValidateResetPasswordCodeInput input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (!Utils.CheckEmailIsValid(input.Email) || string.IsNullOrWhiteSpace(input.ResetCode))
        {
            throw new ArgumentException("Email and reset code are required");
        }

        var contactInfo = ContactInfo.CreateEmail(input.Email);
        var verification = await _verificationService.FindActiveVerification(
            contactInfo, VerificationMode.ForgotPassword);

        if (verification == null)
        {
            throw new ArgumentException("No active password reset verification found");
        }

        if (!verification.VerifyCode(input.ResetCode))
        {
            throw new ArgumentException("Invalid or expired reset code");
        }

        return new ResetPasswordOutput
        {
            Email = input.Email,
            Token = verification.Token
        };
    }
}