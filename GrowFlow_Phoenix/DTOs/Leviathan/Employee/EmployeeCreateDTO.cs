using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Leviathan.Employee
{
    public class EmployeeCreateDTO
    {
        [Required]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = null!;
        [Required]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = null!;
        [Required]
        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = null!;
        [Required]
        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;
        //Leviathan accepts non-email values
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
