namespace EventMemoria.Web.Helpers;

public static class PhotoHelper
{
    public static string GetFileNameWithoutExtension(string fileName)
        => Path.GetFileNameWithoutExtension(fileName).ToLowerInvariant();

    public static string GetFileExtension(string fileName)
        => Path.GetExtension(fileName).ToLowerInvariant();

    public static string FormatFileSize(long sizeInBytes)
    {
        return sizeInBytes switch
        {
            < 1024 => $"{sizeInBytes} B",
            < 1024 * 1024 => $"{sizeInBytes / 1024:F1} KB",
            _ => $"{sizeInBytes / (1024 * 1024):F1} MB"
        };
    }
}
