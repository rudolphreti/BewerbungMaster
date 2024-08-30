using System.Text.Json.Serialization;

namespace BewerbungMasterApp.Models
{
    public class JobApplication
    {
        [JsonPropertyName("URL")]
        public string URL { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }
    }
}