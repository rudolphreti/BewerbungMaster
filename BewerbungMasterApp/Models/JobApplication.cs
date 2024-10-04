using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BewerbungMasterApp.Models
{
    public class JobApplication
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("URL")]
        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? URL { get; set; }

        [JsonPropertyName("position")]
        [Required(ErrorMessage = "Position is required.")]
        [StringLength(100, ErrorMessage = "Position cannot be longer than 100 characters.")]
        public string Position { get; set; } = string.Empty;

        [JsonPropertyName("company")]
        [Required(ErrorMessage = "Company is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot be longer than 100 characters.")]
        public string Company { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "Deweloper"; //it schould be on English
    }
}