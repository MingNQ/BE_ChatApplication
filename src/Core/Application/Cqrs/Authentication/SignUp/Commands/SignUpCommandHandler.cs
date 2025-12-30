using Application.Common.Events;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Authentication;
using Application.Identity.Tokens;
using Application.Interfaces.Services;
using Application.Utility;
using Domain.Entities.Identity;
using Domain.Events.Verification;
using Domain.ValueObjects.Verification;
using Mapster;
using MediatR;
using Shared.Constants;
using System.Text.RegularExpressions;

namespace Application.Cqrs.Authentication.SignUp.Commands;

public class InitiateSignUpCommandHandler(
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IVerificationService verificationService)
    : IRequestHandler<InitiateSignUpCommand, InitiateSignUpResponse>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{7,14}$",
        RegexOptions.Compiled);

    private readonly IWriteRepository<User> _userRepository = unitOfWork.GetRepository<User>();
    private readonly IWriteRepository<Role> _roleRepository = unitOfWork.GetRepository<Role>();

    public async Task<InitiateSignUpResponse> Handle(InitiateSignUpCommand request, CancellationToken cancellationToken)
    {
        BeValidContact(request.Contact);
        var contact = ContactInfo.Create(request.Contact);

        await CheckContactExists(contact);

        if (request.Password != request.ConfirmPassword)
        {
            throw new Exception("Password and Confirm Password do not match.");
        }

        var userRole = await _roleRepository.GetFirstOrDefaultAsync(
            predicate: r => r.NormalizedName == AppConsts.NormalUserRoleName.ToUpperInvariant(),
            disableTracking: true);

        if (userRole == null)
        {
            throw new Exception("User role not found. Please contact support.");
        }

        string username = string.Empty;
        string email = string.Empty;

        if (contact.IsEmail)
        {
            email = contact.Value.ToLowerInvariant();
            username = contact.Value.Split('@')[0];
        }
        else
        {
            username = contact.Value;
        }

        var user = User.Create(
            username,
            email,
            request.FirstName,
            request.LastName);

        user.SetPassword(Utils.ComputeHash(request.Password));
        user.AddRole(userRole.Id);

        await _userRepository.InsertAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync();

        var existVerification = await verificationService.FindActiveVerification(contact, VerificationMode.SignUp);
        if (existVerification is not { IsValid: true })
        {
            var verification = UserVerification.CreateForSignUp(contact, user.Id);

            await verificationService.InsertVerificationAsync(verification, cancellationToken);

            var verificationEvent = new VerificationCreatedEvent(
                verification.UserId,
                contact,
                VerificationMode.SignUp,
                verification.GetVerificationCode());

            await eventPublisher.PublishAsync(verificationEvent);

            return new InitiateSignUpResponse
            {
                VerificationId = verification.Id,
                IsContactExists = false,
                Contact = contact.Value,
                Message = "Verification code sent successfully.",
                User = new UserProfileDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                }
            };
        }

        return new InitiateSignUpResponse
        {
            VerificationId = existVerification.Id,
            IsContactExists = false,
            Contact = contact.Value,
            Message = "A valid verification code has already been sent to this contact. Please check your messages.",
            User = new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }
        };
    }

    private void BeValidContact(string contact)
    {
        if (string.IsNullOrWhiteSpace(contact))
            throw new Exception("This field is required.");

        string trimmed = contact.Trim();

        // Check if it's email
        if (trimmed.Contains('@'))
        {
            bool isValidEmail = EmailRegex.IsMatch(trimmed);

            if (!isValidEmail)
            {
                throw new Exception("Please enter a valid email address.");
            }
            return;
        }

        // Check if it's phone
        string normalizedPhone = trimmed.Replace(" ", "").Replace("-", "");
        bool isValidPhone = PhoneRegex.IsMatch(normalizedPhone);

        if (!isValidPhone)
        {
            throw new Exception("Please enter a valid phone number.");
        }
    }

    private async Task CheckContactExists(ContactInfo contactInfo)
    {
        string normalizedValue = contactInfo.IsEmail
            ? contactInfo.Value.ToUpperInvariant()
            : contactInfo.Value;

        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: u => contactInfo.IsEmail
                ? u.NormalizedEmail == normalizedValue
                : u.PhoneNumber == normalizedValue,
            disableTracking: true);

        if (user != null)
        {
            string message = contactInfo.IsEmail
                ? "This email is already registered. Please log in or use a different email address."
                : "This mobile number is already registered. Please log in or use a different mobile number.";

            throw new Exception(message);
        }
    }
}

public class VerifySignUpContactCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IVerificationService verificationService)
    : IRequestHandler<VerifySignUpContactCommand, VerifySignUpContactResponse>
{
    private readonly IWriteRepository<User> _userRepository = unitOfWork.GetRepository<User>();
    private readonly IWriteRepository<UserVerification> _verificationRepository = unitOfWork.GetRepository<UserVerification>();

    public async Task<VerifySignUpContactResponse> Handle(VerifySignUpContactCommand request, CancellationToken cancellationToken)
    {
        var contact = ContactInfo.Create(request.Contact);

        var verification = await verificationService.FindActiveVerification(contact, VerificationMode.SignUp);
        if (verification == null || !verification.IsValid)
        {
            throw new Exception("No valid verification found for the provided contact.");
        }

        bool isValid = verification.VerifyCode(request.VerificationCode);
        if (!isValid)
        {
            throw new Exception("Invalid or expired verification code.");
        }

        verification.MarkAsUsed();
        _verificationRepository.Update(verification);

        var user = await _userRepository.GetFirstOrDefaultAsync(
                predicate: u => contact.IsEmail
                    ? u.NormalizedEmail == contact.Value.ToUpperInvariant()
                    : u.PhoneNumber == contact.Value,
                disableTracking: true)
                ?? throw new Exception("User not found for the provided contact.");

        if (contact.IsPhone)
        {
            user.SetPhoneNumber(contact.Value);
            user.VerifyPhone(); // First contact is already verified
        }
        else
        {
            user.VerifyEmail(); // First contact is already verified
        }

        if (contact.IsPhone)
        {
            user.SetPhoneNumber(contact.Value);
        }
        await unitOfWork.SaveChangesAsync();

        var userDto = user.Adapt<UserProfileDto>();
        userDto.IsEmailVerified = contact.IsEmail;
        userDto.IsPhoneVerified = contact.IsPhone;

        _verificationRepository.Delete(verification);
        await unitOfWork.SaveChangesAsync();

        var responseToken = await tokenService.GetTokenAsync(user.Id, false, request.IpAddress ?? "N/A", cancellationToken);

        return new VerifySignUpContactResponse
        {
            IsVerified = true,
            AccessToken = responseToken.Token,
            RefreshToken = responseToken.RefreshToken,
            UserId = user.Id,
            Message = "Contact verified and user signed up successfully.",
            User = userDto
        };
    }
}

public class ResendSignUpOtpCommandHandler(
    IEventPublisher eventPublisher,
    IVerificationService verificationService)
    : IRequestHandler<ResendSignUpOtpCommand, ResendSignUpOtpResponse>
{
    public async Task<ResendSignUpOtpResponse> Handle(ResendSignUpOtpCommand request, CancellationToken cancellationToken)
    {
        var contactInfo = ContactInfo.Create(request.Contact);

        var verification = await verificationService.FindVerificationById(contactInfo, request.VerificationId);

        if (verification == null || verification.Mode != nameof(VerificationMode.SignUp))
        {
            throw new Exception("No verification found for the provided contact and verification ID.");
        }

        if (contactInfo.IsEmail && verification.Email != contactInfo.Value || contactInfo.IsPhone && verification.Phone != contactInfo.Value)
        {
            throw new Exception("The contact information does not match the verification record.");
        }

        verification.Resend();
        await verificationService.UpdateVerificationAsync(verification);

        var verificationEvent = new VerificationCreatedEvent(
            verification.UserId,
            contactInfo,
            VerificationMode.SignUp,
            verification.GetVerificationCode());

        await eventPublisher.PublishAsync(verificationEvent);

        return new ResendSignUpOtpResponse
        {
            IsSent = true,
            VerificationId = verification.Id,
            Message = $"New verification code sent to {contactInfo.ValueMask()}"
        };
    }
}