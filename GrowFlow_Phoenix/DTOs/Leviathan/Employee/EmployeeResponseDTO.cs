using GrowFlow_Phoenix.Models.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Leviathan.Employee
{
    public class EmployeeResponseDTO
    {
        [Required]
        [JsonPropertyName("firstName")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string FirstName { get; set; }
        [Required]
        [JsonPropertyName("lastName")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string LastName { get; set; }
        [Required]
        [JsonPropertyName("telephone")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("role")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string? Role { get; set; }
        //Leviathan accepts non-email values
        [JsonPropertyName("email")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string? Email { get; set; }
        [JsonPropertyName("id")]
        public Guid LeviathanId { get; set; }
    }
}
