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

    public async Task<bool> LogEventAsync(string eventName, string? itemName = null)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            logger.LogWarning("Cannot log analytics event with a null, empty, or whitespace event name");
            return false;
        }

        try
        {
            var tableClient = await _tableClient.Value;

            var logEvent = new AnalyticsLog(eventName, itemName);
            await tableClient.AddEntityAsync(logEvent);
            
            logger.LogDebug("Successfully logged event {EventName}", eventName);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging event {EventName}", eventName);
            return false;
        }
    }

    private static async Task<TableClient> CreateTableClientAsync(TableServiceClient tableServiceClient)
    {
        var tableClient = tableServiceClient.GetTableClient(TableName);
        return tableClient;
    }
}
