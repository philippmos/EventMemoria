namespace Vogelhochzeit.Common.Settings;

public record ImageOptions
{
    public string ContainerName { get; init; } = string.Empty;
    public IEnumerable<string> AllowedFileTypes { get; init; } = [];
    public int DefaultPhotosPerRow { get; init; }
}
