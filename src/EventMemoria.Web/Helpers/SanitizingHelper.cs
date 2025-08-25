using System.Text.RegularExpressions;

namespace EventMemoria.Web.Helpers;

public static class SanitizingHelper
{
    public static string SanitizeValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "Unknown";
        }

        var sanitized = Regex.Replace(value, @"[^a-zA-Z0-9\s\+\-\.\:=_]", "_");

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
