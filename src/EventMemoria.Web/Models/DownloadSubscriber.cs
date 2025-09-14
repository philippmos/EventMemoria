using Azure;
using Azure.Data.Tables;

namespace EventMemoria.Web.Models;

public class DownloadSubscriber : ITableEntity
{
    public string PartitionKey { get; set; } = "DownloadSubscriber";
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public DateTime SubscribedAt { get; set; }
    
    public DownloadSubscriber()
    { }
    
    public DownloadSubscriber(string email)
    {
        Email = email;
        RowKey = Guid.NewGuid().ToString();
        SubscribedAt = DateTime.UtcNow;
    }
}
