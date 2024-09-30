using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BewerbungMasterApp.Models
{
    public class JobApplication
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("URL")]
        [Url(ErrorMessage = "Bitte geben Sie eine gültige URL ein.")]
        public string URL { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        [Required(ErrorMessage = "Position ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Die Position darf nicht länger als 100 Zeichen sein.")]
        public string Position { get; set; } = string.Empty;

        [JsonPropertyName("company")]
        [Required(ErrorMessage = "Unternehmen ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Der Unternehmensname darf nicht länger als 100 Zeichen sein.")]
        public string Company { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "Deweloper";

        [JsonPropertyName("isInitiative")]
        public bool IsInitiative => Type == "Initiativbewerbung";
    }
}