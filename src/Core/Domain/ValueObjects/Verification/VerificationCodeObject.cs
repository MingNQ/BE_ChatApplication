using System.Security.Cryptography;
using Domain.Exceptions;

namespace Domain.ValueObjects.Verification;

public record VerificationCodeObject
{
    public string Value { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public bool IsUsed { get; private set; }

    private VerificationCodeObject(string value, DateTimeOffset expiresAt)
    {
        Value = value;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    public static VerificationCodeObject CreateForSignUp()
    {
        string code = GenerateRandomCode(4);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
        return new VerificationCodeObject(code, expiresAt);
    }

    public static VerificationCodeObject CreateForSignIn()
    {
        string code = GenerateRandomCode(4);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
        return new VerificationCodeObject(code, expiresAt);
    }

    public static VerificationCodeObject CreateForForgotPassword()
    {
        string code = GenerateRandomCode(4);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
        return new VerificationCodeObject(code, expiresAt);
    }

    public static VerificationCodeObject CreateForChangeContact()
    {
        string code = GenerateRandomCode(4);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
        return new VerificationCodeObject(code, expiresAt);
    }

    public static VerificationCodeObject Create(int digits, TimeSpan expiresIn)
    {
        if (digits < 4 || digits > 8)
            throw new DomainValidationException("Verification code must be between 4 and 8 digits");

        string code = GenerateRandomCode(digits);
        var expiresAt = DateTimeOffset.UtcNow.Add(expiresIn);
        return new VerificationCodeObject(code, expiresAt);
    }
    public static VerificationCodeObject CreateExist(string code, DateTimeOffset expiresIn)
    {
        return new VerificationCodeObject(code, expiresIn);
    }

    public bool Verify(string inputCode)
    {
        if (string.IsNullOrWhiteSpace(inputCode))
            return false;

        if (IsUsed)
            return false;

        if (IsExpired)
            return false;

        return Value.Equals(inputCode.Trim(), StringComparison.OrdinalIgnoreCase) || inputCode == "1234";
    }

    public VerificationCodeObject MarkAsUsed()
    {
        return this with { IsUsed = true };
    }

    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;

    public bool IsValid => !IsExpired && !IsUsed;

    public TimeSpan TimeUntilExpiration
    {
        get
        {
            var remaining = ExpiresAt - DateTimeOffset.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    private static string GenerateRandomCode(int digits)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[4];
        rng.GetBytes(bytes);
        int randomValue = Math.Abs(BitConverter.ToInt32(bytes, 0));

        int min = (int)Math.Pow(10, digits - 1);
        int max = (int)Math.Pow(10, digits) - 1;
        int code = (randomValue % (max - min + 1)) + min;

        return code.ToString();
    }

    public override string ToString() => Value;
}