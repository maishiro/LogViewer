using System.Text.Json.Serialization;

namespace LogViewer.Models
{
    public class LogEntry
    {
        [JsonPropertyName( "timestamp" )]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName( "message" )]
        public string Message { get; set; } = string.Empty;
    }
}
