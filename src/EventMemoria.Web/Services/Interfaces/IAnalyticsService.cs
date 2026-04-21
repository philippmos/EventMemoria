namespace EventMemoria.Web.Services.Interfaces;

public interface IAnalyticsService
{
    Task<bool> LogPageViewAsync(string pageName);
    Task<bool> LogEventAsync(string eventName, string? itemName = null);
}
