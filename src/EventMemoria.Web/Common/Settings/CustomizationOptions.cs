namespace EventMemoria.Web.Common.Settings;

public record CustomizationOptions
{
    public string PageTitle { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string AvatarPath { get; init; } = string.Empty;
    public string Names { get; init; } = string.Empty;
    public string WelcomeMessage { get; init; } = string.Empty;
    public string QrCodeTargetUrl { get; init; } = string.Empty;
    public NameModalOptions NameModal { get; init; } = null!;
}

public record NameModalOptions
{
    public string Title { get; init; } = string.Empty;
    public string SubTitle { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
