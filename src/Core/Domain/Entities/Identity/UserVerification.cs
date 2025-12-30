using Domain.Common.Contracts;
using Domain.Events.Verification;
using Domain.ValueObjects.Verification;
using Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Identity;

public class UserVerification : AuditableEntity<long>, IAggregateRoot
{
    [MaxLength(256)]
    public string Mode { get; private set; } = string.Empty;

    [MaxLength(256)]
    public string Token { get; private set; } = string.Empty;

    public DateTimeOffset? TokenExpirationDate { get; private set; }

    [MaxLength(15)]
    public string VerificationCode { get; private set; } = string.Empty;

    public DateTimeOffset? CodeExpirationDate { get; private set; }
    public DateTimeOffset CreatedDate { get; private set; }

    public int Status { get; private set; }

    [MaxLength(500)]
    public string Link { get; private set; } = string.Empty;

    public long? UserId { get; private set; }

    [MaxLength(256)]
    public string Email { get; private set; } = string.Empty;

    [MaxLength(15)]
    public string Phone { get; private set; } = string.Empty;

    public DateTimeOffset DeletedDate { get; private set; }

    public static UserVerification Create(string code, long? userId, string phoneNumber, string email)
    {
        return new UserVerification
        {
            Token = string.Empty,
            TokenExpirationDate = null,
            VerificationCode = code,
            CreatedDate = DateTime.UtcNow,
            Link = string.Empty,
            UserId = userId,
            Status = UserVerificationStatus.Active,
            Phone = phoneNumber,
            Email = email
        };
    }

    public static UserVerification CreateForSignUp(ContactInfo contactInfo, long? userId = null)
    {
        var verification = new UserVerification();
        var code = VerificationCodeObject.CreateForSignUp();

        verification.InitializeVerification(
            contactInfo,
            code,
            VerificationMode.SignUp,
            userId);

        return verification;
    }

    public static UserVerification CreateForSignIn(ContactInfo contactInfo, long userId)
    {
        var verification = new UserVerification();
        var code = VerificationCodeObject.CreateForSignIn();

        verification.InitializeVerification(
            contactInfo,
            code,
            VerificationMode.SignIn,
            userId);

        return verification;
    }

    public static UserVerification CreateForForgotPassword(ContactInfo contactInfo, long userId)
    {
        var verification = new UserVerification();
        var code = VerificationCodeObject.CreateForForgotPassword();
        var token = VerificationTokenObject.CreateForPasswordReset();

        verification.InitializeVerification(
            contactInfo,
            code,
            VerificationMode.ForgotPassword,
            userId,
            token);

        return verification;
    }

    private void InitializeVerification(
        ContactInfo contactInfo,
        VerificationCodeObject code,
        VerificationMode mode,
        long? userId,
        VerificationTokenObject? token = null)
    {
        Mode = mode.ToString();
        UserId = userId;
        VerificationCode = code.Value;
        CodeExpirationDate = code.ExpiresAt;
        CreatedDate = DateTimeOffset.UtcNow;
        Status = (int)VerificationStatus.Active;

        if (contactInfo.IsEmail)
        {
            Email = contactInfo.Value;
            Phone = string.Empty;
        }
        else
        {
            Phone = contactInfo.Value;
            Email = string.Empty;
        }

        if (token != null)
        {
            Token = token.Value;
            TokenExpirationDate = token.ExpiresAt;
        }

        // Set deletion date based on mode
        DeletedDate = mode switch
        {
            VerificationMode.SignUp => DateTimeOffset.UtcNow.AddDays(1),
            VerificationMode.SignIn => DateTimeOffset.UtcNow.AddDays(1),
            VerificationMode.ForgotPassword => DateTimeOffset.UtcNow.AddDays(1),
            VerificationMode.EmailVerification => DateTimeOffset.UtcNow.AddDays(30),
            VerificationMode.PhoneVerification => DateTimeOffset.UtcNow.AddDays(30),
            _ => DateTimeOffset.UtcNow.AddDays(1)
        };
    }

    /// <summary>
    /// Verifies the provided code
    /// </summary>
    public bool VerifyCode(string inputCode)
    {
        if (Status != (int)VerificationStatus.Active)
        {
            return false;
        }

        var codeVo = GetVerificationCode();
        bool isValid = codeVo.Verify(inputCode);

        if (isValid)
        {
            MarkAsUsed();
        }
        else
        {
            string reason = codeVo.IsExpired ? "Code expired" : "Invalid code";
        }

        return isValid;
    }

    /// <summary>
    /// Verifies the provided token
    /// </summary>
    public bool VerifyToken(string inputToken)
    {
        if (Status != (int)VerificationStatus.Active)
        {
            return false;
        }

        var tokenVo = GetVerificationToken();
        if (tokenVo == null)
        {
            return false;
        }

        bool isValid = tokenVo.Verify(inputToken);

        if (isValid)
        {
            MarkAsUsed();
        }
        else
        {
            string reason = tokenVo.IsExpired ? "Token expired" : "Invalid token";
        }

        return isValid;
    }

    /// <summary>
    /// Marks verification as used
    /// </summary>
    public void MarkAsUsed()
    {
        Status = (int)VerificationStatus.Used;
    }

    /// <summary>
    /// Marks verification as expired
    /// </summary>
    public void MarkAsExpired()
    {
        Status = (int)VerificationStatus.Expired;
    }

    /// <summary>
    /// Resends the verification with new code
    /// </summary>
    public void Resend()
    {
        if (Status != (int)VerificationStatus.Active)
        {
            throw new Exception("Cannot resend inactive verification");
        }

        var mode = Enum.Parse<VerificationMode>(Mode);
        var newCode = mode switch
        {
            VerificationMode.SignUp => VerificationCodeObject.CreateForSignUp(),
            VerificationMode.SignIn => VerificationCodeObject.CreateForSignIn(),
            VerificationMode.ForgotPassword => VerificationCodeObject.CreateForForgotPassword(),
            _ => VerificationCodeObject.CreateForSignUp()
        };

        VerificationCode = newCode.Value;
        CodeExpirationDate = newCode.ExpiresAt;
    }

    // Helper methods to reconstruct value objects
    public ContactInfo GetContactInfo()
    {
        if (!string.IsNullOrEmpty(Email))
        {
            return ContactInfo.CreateEmail(Email);
        }

        if (!string.IsNullOrEmpty(Phone))
        {
            return ContactInfo.CreatePhone(Phone);
        }

        throw new Exception("No valid contact information found");
    }

    public VerificationCodeObject GetVerificationCode()
    {
        if (string.IsNullOrEmpty(VerificationCode) || !CodeExpirationDate.HasValue)
        {
            throw new Exception("No valid verification code found");
        }

        return VerificationCodeObject.CreateExist(VerificationCode, CodeExpirationDate.Value);
    }

    public VerificationTokenObject? GetVerificationToken()
    {
        if (string.IsNullOrEmpty(Token) || !TokenExpirationDate.HasValue)
        {
            return null;
        }

        var expiresIn = TokenExpirationDate.Value - DateTimeOffset.UtcNow;
        return VerificationTokenObject.Create(
            expiresIn > TimeSpan.Zero ? expiresIn : TimeSpan.Zero);
    }

    // Properties for business logic
    public bool IsExpired =>
        (CodeExpirationDate.HasValue && CodeExpirationDate.Value < DateTimeOffset.UtcNow) ||
        (TokenExpirationDate.HasValue && TokenExpirationDate.Value < DateTimeOffset.UtcNow);

    public bool IsActive => Status == (int)VerificationStatus.Active;
    public bool IsUsed => Status == (int)VerificationStatus.Used;
    public bool IsValid => IsActive && !IsExpired;
}

public enum VerificationStatus
{
    None = 0,
    Active = 1,
    Used = 2,
    Expired = 3,
    Cancelled = 4
}