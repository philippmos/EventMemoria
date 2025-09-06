using System.Text.RegularExpressions;

namespace EventMemoria.Web.Helpers;

public static partial class SanitizingHelper
{
    [GeneratedRegex(@"[^a-zA-Z0-9\s\+\-\.\:=_]")]
    private static partial Regex SanitizeRegEx();
    
    public static string SanitizeValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "Unknown";
        }

        var sanitized = SanitizeRegEx().Replace(value, "_");

        sanitized = sanitized.Trim();

        if (string.IsNullOrEmpty(sanitized))
        {
            sanitized = "Unknown";
        }

        if (sanitized.Length > 256)
        {
            sanitized = sanitized[..256];
        }

        sanitized = sanitized.TrimEnd();

        return sanitized;
    }
}
