namespace Domain.ValueObjects.Verification;

/// <summary>
/// Value object representing a verification token for email links
/// </summary>
public record VerificationTokenObject
{
    public string Value { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsUsed { get; private set; }

    private VerificationTokenObject(string value, DateTime expiresAt)
    {
        Value = value;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    /// <summary>
    /// Creates a new verification token for email verification (expires in 24 hours)
    /// </summary>
    public static VerificationTokenObject CreateForEmailVerification()
    {
        string token = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.AddHours(24);
        return new VerificationTokenObject(token, expiresAt);
    }

    /// <summary>
    /// Creates a new verification token for password reset (expires in 1 hour)
    /// </summary>
    public static VerificationTokenObject CreateForPasswordReset()
    {
        string token = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.AddHours(1);
        return new VerificationTokenObject(token, expiresAt);
    }

    /// <summary>
    /// Creates a verification token with custom expiration
    /// </summary>
    public static VerificationTokenObject Create(TimeSpan expiresIn)
    {
        string token = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.Add(expiresIn);
        return new VerificationTokenObject(token, expiresAt);
    }

    /// <summary>
    /// Verifies if the provided token matches this verification token
    /// </summary>
    public bool Verify(string inputToken)
    {
        if (string.IsNullOrWhiteSpace(inputToken))
            return false;

        if (IsUsed)
            return false;

        if (IsExpired)
            return false;

        return Value.Equals(inputToken.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Marks this verification token as used
    /// </summary>
    public VerificationTokenObject MarkAsUsed()
    {
        return this with { IsUsed = true };
    }

    /// <summary>
    /// Checks if the verification token is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Checks if the verification token is valid (not expired and not used)
    /// </summary>
    public bool IsValid => !IsExpired && !IsUsed;

    /// <summary>
    /// Gets remaining time until expiration
    /// </summary>
    public TimeSpan TimeUntilExpiration
    {
        get
        {
            var remaining = ExpiresAt - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    private static string GenerateSecureToken()
    {
        return Guid.NewGuid().ToString("N")[..32]; // 32 character token
    }

    public override string ToString() => Value;
}