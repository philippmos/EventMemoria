namespace Vogelhochzeit.Common.Settings;

public record CustomizationOptions
{
    public string PageTitle { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string AvatarPath { get; init; } = string.Empty;
    public string Names { get; init; } = string.Empty;
    public string WelcomeMessage { get; init; } = string.Empty;
    public NameModalOptions NameModal { get; init; } = new();
}

public record NameModalOptions
{
    public string Title { get; init; } = string.Empty;
    public string SubTitle { get; init; } = string.Empty;
}
