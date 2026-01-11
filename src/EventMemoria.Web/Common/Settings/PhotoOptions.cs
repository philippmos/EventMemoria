namespace EventMemoria.Web.Common.Settings;

public record PhotoOptions
{
    public StorageContainer StorageContainer { get; init; } = null!;
    public int DefaultPhotosPerRow { get; init; }
    public string AllPhotosDownloadArchive { get; init; } = string.Empty;
}

public record StorageContainer
{
    public string FullSize { get; init; } = string.Empty;
    public string Thumbnails { get; init; } = string.Empty;
    public string Videos { get; init; } = string.Empty;
    public string GalleryThumbnails { get; init; } = string.Empty;
    public string Gallery { get; init; } = string.Empty;
}
