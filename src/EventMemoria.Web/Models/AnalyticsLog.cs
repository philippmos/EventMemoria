using Azure;
using Azure.Data.Tables;

namespace EventMemoria.Web.Models;

public class AnalyticsLog : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string EventName { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    
    public AnalyticsLog()
    { }
    
    public AnalyticsLog(string eventName, string? itemName = null)
    {
        PartitionKey = eventName;
        EventName = eventName;
        ItemName = itemName ?? string.Empty;
        RowKey = Guid.NewGuid().ToString();
    }
}
