using Domain.Common.Contracts;
using Domain.ValueObjects.Verification;

namespace Domain.Events.Verification;

/// <summary>
/// Domain event fired when a verification is created
/// </summary>
public class VerificationCreatedEvent(
    long? userId,
    ContactInfo contactInfo,
    VerificationMode mode,
    VerificationCodeObject code)
    : DomainEvent
{
    public long? UserId { get; } = userId;
    public ContactInfo ContactInfo { get; } = contactInfo;
    public VerificationMode Mode { get; } = mode;
    public VerificationCodeObject Code { get; } = code;
}

/// <summary>
/// Domain event fired when a verification code is sent
/// </summary>
public class VerificationCodeSentEvent : DomainEvent
{
    public VerificationCodeSentEvent(
        long? userId,
        ContactInfo contactInfo,
        VerificationMode mode,
        string code)
    {
        UserId = userId;
        ContactInfo = contactInfo;
        Mode = mode;
        Code = code;
    }

    public long? UserId { get; }
    public ContactInfo ContactInfo { get; }
    public VerificationMode Mode { get; }
    public string Code { get; }
}

/// <summary>
/// Domain event fired when a verification succeeds
/// </summary>
public class VerificationSucceededEvent : DomainEvent
{
    public VerificationSucceededEvent(
        long? userId,
        ContactInfo contactInfo,
        VerificationMode mode)
    {
        UserId = userId;
        ContactInfo = contactInfo;
        Mode = mode;
    }

    public long? UserId { get; }
    public ContactInfo ContactInfo { get; }
    public VerificationMode Mode { get; }
}

/// <summary>
/// Domain event fired when a verification fails
/// </summary>
public class VerificationFailedEvent : DomainEvent
{
    public VerificationFailedEvent(
        long? userId,
        ContactInfo contactInfo,
        VerificationMode mode,
        string reason)
    {
        UserId = userId;
        ContactInfo = contactInfo;
        Mode = mode;
        Reason = reason;
    }

    public long? UserId { get; }
    public ContactInfo ContactInfo { get; }
    public VerificationMode Mode { get; }
    public string Reason { get; }
}

/// <summary>
/// Domain event fired when a verification expires
/// </summary>
public class VerificationExpiredEvent : DomainEvent
{
    public VerificationExpiredEvent(
        long? userId,
        ContactInfo contactInfo,
        VerificationMode mode)
    {
        UserId = userId;
        ContactInfo = contactInfo;
        Mode = mode;
    }

    public long? UserId { get; }
    public ContactInfo ContactInfo { get; }
    public VerificationMode Mode { get; }
}

public enum VerificationMode
{
    None = 0,
    SignUp = 1,
    SignIn = 2,
    ForgotPassword = 3,
    EmailVerification = 4,
    PhoneVerification = 5
}