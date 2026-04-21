using EventMemoria.Web.Services.Interfaces;
using Azure.Data.Tables;
using EventMemoria.Web.Models;

namespace EventMemoria.Web.Services;

public class AnalyticsService(
    TableServiceClient tableServiceClient,
    ILogger<AnalyticsService> logger) : IAnalyticsService
{
    private const string TableName = "Analytics";
    private readonly Lazy<Task<TableClient>> _tableClient = new(() => CreateTableClientAsync(tableServiceClient));

    public async Task<bool> LogPageViewAsync(string pageName)
    {
        if (string.IsNullOrWhiteSpace(pageName))
        {
            logger.LogWarning("Cannot log analytics page view with a null, empty, or whitespace page name");
            return false;
        }

        return await LogAnalyticsAsync(
            new AnalyticsLog("PageView", pageName));
    }

    public async Task<bool> LogEventAsync(string eventName, string? itemName = null)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            logger.LogWarning("Cannot log analytics event with a null, empty, or whitespace event name");
            return false;
        }

        return await LogAnalyticsAsync(
            new AnalyticsLog(eventName, itemName));
    }

    private async Task<bool> LogAnalyticsAsync(AnalyticsLog logEvent)
    {
        try
        {
            var tableClient = await _tableClient.Value;
            await tableClient.AddEntityAsync(logEvent);

            logger.LogDebug("Successfully logged analytics {Name}", logEvent.EventName);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging analytics {Name}", logEvent.EventName);
            return false;
        }
    }

    private static async Task<TableClient> CreateTableClientAsync(TableServiceClient tableServiceClient)
    {
        var tableClient = tableServiceClient.GetTableClient(TableName);
        return tableClient;
    }
}
