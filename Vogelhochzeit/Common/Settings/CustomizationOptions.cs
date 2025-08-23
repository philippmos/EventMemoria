namespace Vogelhochzeit.Common.Settings;

public record CustomizationOptions
{
    public string PageTitle { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string AvatarImagePath { get; init; } = string.Empty;
    public string Names { get; init; } = string.Empty;
    public string WelcomeMessage { get; init; } = string.Empty;
}
