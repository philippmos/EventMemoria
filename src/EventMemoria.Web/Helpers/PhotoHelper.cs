namespace EventMemoria.Web.Helpers;

public static class PhotoHelper
{
    public static string GetFileNameWithoutExtension(string fileName)
        => Path.GetFileNameWithoutExtension(fileName).ToLowerInvariant();

    public static string GetFileExtension(string fileName)
        => Path.GetExtension(fileName).ToLowerInvariant();
}
