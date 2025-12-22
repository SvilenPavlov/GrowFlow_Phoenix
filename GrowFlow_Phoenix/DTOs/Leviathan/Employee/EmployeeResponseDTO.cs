using GrowFlow_Phoenix.Models.Utility;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Leviathan.Employee
{
    public class EmployeeResponseDTO
    {
        [JsonPropertyName("firstName")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("lastName")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string LastName { get; set; } = null!;
        [JsonPropertyName("telephone")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("role")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string Role { get; set; } = null!;
        
        [JsonPropertyName("email")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string? Email { get; set; }
        [JsonPropertyName("id")]
        public Guid LeviathanId { get; set; }
    }
}
