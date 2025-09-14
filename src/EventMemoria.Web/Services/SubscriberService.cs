using Azure.Data.Tables;
using EventMemoria.Web.Models;
using EventMemoria.Web.Services.Interfaces;

namespace EventMemoria.Web.Services;

public class SubscriberService(
    TableServiceClient tableServiceClient,
    ILogger<SubscriberService> logger) : ISubscriberService
{
    private const string TableName = "DownloadSubscribers";

    public async Task<bool> AddSubscriberAsync(string email)
    {
        try
        {
            var tableClient = await GetTableClientAsync();
            
            if (await IsEmailAlreadySubscribedAsync(email))
            {
                logger.LogInformation("Email is already subscribed");
                return true;
            }

            var subscriber = new DownloadSubscriber(email);
            await tableClient.AddEntityAsync(subscriber);
            
            logger.LogInformation("Successfully added subscriber");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding subscriber with email {Email}", email);
            return false;
        }
    }

    private async Task<bool> IsEmailAlreadySubscribedAsync(string email)
    {
        try
        {
            var tableClient = await GetTableClientAsync();
            
            var queryResults = tableClient.QueryAsync<DownloadSubscriber>(
                filter: $"PartitionKey eq 'DownloadSubscriber' and Email eq '{email}'");

            await foreach (var entity in queryResults)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if email is already subscribed");
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
