using System.Text.Json.Serialization;

namespace BewerbungMasterApp.Models
{
    public class JobApplication
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("URL")]
        public string URL { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

    }
}