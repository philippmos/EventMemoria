namespace EventMemoria.Web.Common.Settings;

public record PhotoOptions
{
    public string ContainerName { get; init; } = string.Empty;
    public IEnumerable<string> AllowedFileTypes { get; init; } = [];
    public int DefaultPhotosPerRow { get; init; }
}
