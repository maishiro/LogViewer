using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogViewer.Models
{
    public class LogEntry
    {
        [JsonPropertyName( "timestamp" )]
        public string Timestamp { get; set; }
        [JsonPropertyName( "message" )]
        public string Message { get; set; } = string.Empty;

        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalData { get; set; }
    }
}
