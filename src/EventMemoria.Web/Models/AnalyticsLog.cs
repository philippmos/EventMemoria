using Azure;
using Azure.Data.Tables;

namespace EventMemoria.Web.Models;

public class AnalyticsLog : ITableEntity
{
    public string PartitionKey { get; set; } = "AnalyticsLog";
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string EventName { get; set; } = string.Empty;
    
    public AnalyticsLog()
    { }
    
    public AnalyticsLog(string eventName)
    {
        EventName = eventName;
        RowKey = Guid.NewGuid().ToString();
    }
}
