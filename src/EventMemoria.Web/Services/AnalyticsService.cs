using EventMemoria.Web.Services.Interfaces;
using Azure.Data.Tables;
using EventMemoria.Web.Models;

namespace EventMemoria.Web.Services;

public class AnalyticsService(
    TableServiceClient tableServiceClient,
    ILogger<AnalyticsService> logger) : IAnalyticsService
{
    private const string TableName = "Analytics";

    public async Task<bool> LogEventAsync(string eventName)
    {
        try
        {
            var tableClient = await GetTableClientAsync();

            var subscriber = new AnalyticsLog(eventName);
            await tableClient.AddEntityAsync(subscriber);
            
            logger.LogInformation("Successfully logged event");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging event {EventName}", eventName);
            return false;
        }
    }

    private async Task<TableClient> GetTableClientAsync()
    {
        var tableClient = tableServiceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }
}
