using System.Text.RegularExpressions;
using Domain.Common.Contracts;
using Domain.Exceptions;

namespace Domain.ValueObjects.Identity;

public class Password : ValueObject
{
    private const int MinLength = 8;
    private const string UppercasePattern = @"[A-Z]";
    private const string LowercasePattern = @"[a-z]";
    private const string DigitPattern = @"[0-9]";
    private const string SpecialCharPattern = @"[!@#$%^&*]";

    public string Value { get; private set; }

    private Password(string value)
    {
        Value = value;
    }

    public static Password Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainValidationException("Password is required");
        }

        ValidatePasswordRules(password);

        return new Password(password);
    }

    private static void ValidatePasswordRules(string password)
    {
        var errors = new List<string>();

        if (password.Length < MinLength)
        {
            errors.Add($"Password must be at least {MinLength} characters long");
        }

        if (!Regex.IsMatch(password, UppercasePattern))
        {
            errors.Add("Password must contain at least one uppercase letter (A-Z)");
        }

        if (!Regex.IsMatch(password, LowercasePattern))
        {
            errors.Add("Password must contain at least one lowercase letter (a-z)");
        }

        if (!Regex.IsMatch(password, DigitPattern))
        {
            errors.Add("Password must contain at least one number (0-9)");
        }

        if (!Regex.IsMatch(password, SpecialCharPattern))
        {
            errors.Add("Password must contain at least one special character (!@#$%^&*)");
        }

        if (password.Contains(' '))
        {
            errors.Add("Password must not contain spaces");
        }

        if (errors.Any())
        {
            throw new DomainValidationException($"Password validation failed: {string.Join(", ", errors)}");
        }
    }

    public static string PasswordRequirements =>
        "Password must contain: 8+ characters, uppercase letter, lowercase letter, number, and special character (!@#$%^&*).";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return "***HIDDEN***";
    }
}