using System.Text.Json.Serialization;

namespace BewerbungMasterApp.Models
{
    public class JobApplication
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("URL")]
        public string URL { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;

        [JsonPropertyName("company")]
        public string Company { get; set; } = string.Empty;

    }
}