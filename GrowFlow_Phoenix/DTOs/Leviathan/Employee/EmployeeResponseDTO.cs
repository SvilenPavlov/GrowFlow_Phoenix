using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Leviathan.Employee
{
    public class EmployeeResponseDTO
    {
        [Required]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [Required]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [Required]
        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("role")]
        public string? Role { get; set; }
        //Leviathan accepts non-email values
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("id")]
        public Guid LeviathanId { get; set; }
    }
}
