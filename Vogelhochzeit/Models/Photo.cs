namespace Vogelhochzeit.Models;

public class Photo
{
    public required string Id { get; init; }
    public required string FileName { get; init; }
    public required string Url { get; init; }
    public required DateTime UploadDate { get; init; }
    public required long FileSize { get; init; }
    public string? Description { get; init; }
    public string? Alt { get; init; }
    public bool IsLoaded { get; set; } = true;
    public string Author { get; set; } = string.Empty;

    public string FormattedFileSize => FormatFileSize(FileSize);

    private static string FormatFileSize(long sizeInBytes)
    {
        return sizeInBytes switch
        {
            < 1024 => $"{sizeInBytes} B",
            < 1024 * 1024 => $"{sizeInBytes / 1024:F1} KB",
            _ => $"{sizeInBytes / (1024 * 1024):F1} MB"
        };
    }
}
