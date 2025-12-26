using System.Text.RegularExpressions;

namespace Shared.Helpers;

public static class MaskHelper
{
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        string[] parts = email.Split('@');
        if (parts.Length != 2)
            return email;

        string localPart = parts[0];
        string domain = parts[1];

        if (string.IsNullOrEmpty(localPart) || string.IsNullOrEmpty(domain))
            return email;

        if (localPart.Length <= 2)
        {
            // Keep the first character, mask the rest
            return localPart[0] + new string('*', localPart.Length - 1) + "@" + domain;
        }

        string visible = localPart.Substring(0, 2);
        int maskedCount = localPart.Length - 2;
        return $"{visible}{new string('*', maskedCount)}@{domain}";
    }

    public static string MaskPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        string clean = Regex.Replace(phone, @"\s+", "");
        if (clean.Length < 5)
            return clean;

        var prefixMatch = Regex.Match(clean, @"^\+\d{2}");
        string prefix = prefixMatch.Success ? prefixMatch.Value : string.Empty;
        string rest = prefix != string.Empty ? clean.Substring(prefix.Length) : clean;

        string lastTwo = rest.Substring(rest.Length - 2);
        string maskedCore = new('*', rest.Length - 2);

        // Add space every 3 characters for readability
        maskedCore = Regex.Replace(maskedCore, @"(.{3})(?=.)", "$1 ");

        return $"{prefix} {maskedCore}{lastTwo}".Trim();
    }
}