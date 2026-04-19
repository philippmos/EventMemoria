namespace EventMemoria.Web.Services.Interfaces;

public interface IAnalyticsService
{
    Task<bool> LogEventAsync(string eventName, string? itemName = null);
}
