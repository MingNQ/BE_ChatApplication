namespace Shared.Extensions;

public static class StringExtensions
{
    public static string ToSeparatedWords(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        string result = System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        return result.ToLower();
    }
}