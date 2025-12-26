using Domain.Exceptions;
using Shared.Constants;
using Shared.Helpers;

namespace Domain.ValueObjects.Verification;

/// <summary>
/// Value object representing contact information (email or phone) for verification
/// </summary>
public record ContactInfo
{
    public string Value { get; init; }
    public ContactType Type { get; init; }

    private ContactInfo(string value, ContactType type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Creates a ContactInfo for email
    /// </summary>
    public static ContactInfo CreateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainValidationException("Email cannot be empty");

        string normalizedEmail = email.Trim().ToLowerInvariant();

        if (!ValidatorConst.EmailRegex.IsMatch(normalizedEmail))
            throw new DomainValidationException("Invalid email format");

        return new ContactInfo(normalizedEmail, ContactType.Email);
    }

    /// <summary>
    /// Creates a ContactInfo for phone number
    /// </summary>
    public static ContactInfo CreatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainValidationException("Phone number cannot be empty");

        string normalizedPhone = phone.Trim()
            .Replace(" ", "")
            .Replace("-", "");

        if (!normalizedPhone.StartsWith("+"))
        {
            normalizedPhone = "+" + normalizedPhone;
        }

        if (!ValidatorConst.PhoneRegex.IsMatch(normalizedPhone))
            throw new DomainValidationException("Invalid phone number format");

        return new ContactInfo(normalizedPhone, ContactType.Phone);
    }

    /// <summary>
    /// Auto-detects the contact type and creates appropriate ContactInfo
    /// </summary>
    public static ContactInfo Create(string contact)
    {
        if (string.IsNullOrWhiteSpace(contact))
            throw new DomainValidationException("Contact information cannot be empty");

        string trimmedContact = contact.Trim();

        // Try email first
        if (trimmedContact.Contains('@'))
        {
            return CreateEmail(trimmedContact);
        }

        // Otherwise treat as phone
        return CreatePhone(trimmedContact);
    }

    public bool IsEmail => Type == ContactType.Email;
    public bool IsPhone => Type == ContactType.Phone;

    public override string ToString() => Value;

    public string ValueMask()
    {
        if (IsEmail)
        {
            return MaskHelper.MaskEmail(Value);
        }

        return IsPhone ? MaskHelper.MaskPhone(Value) : Value;
    }
}

public enum ContactType
{
    None = 0,
    Email = 1,
    Phone = 2
}