namespace EventMemoria.Web.Models;

public class Photo
{
    public required string Id { get; init; }
    public required string FileName { get; init; }
    public required string Url { get; init; }
    public required DateTime UploadDate { get; init; }
    public required long FileSize { get; init; }
    public string? Alt { get; init; }
    public string Author { get; set; } = string.Empty;
    public MediaType MediaType { get; init; } = MediaType.Image;
    public string? ThumbnailUrl { get; init; }
}

public enum MediaType
{
    Image,
    Video
}
