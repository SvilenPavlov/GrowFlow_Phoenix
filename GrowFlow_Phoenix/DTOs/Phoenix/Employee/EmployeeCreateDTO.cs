using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Phoenix.Employee
{
    public class EmployeeCreateDTO
    {
        [Required]
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; } = null!;
        [Required]
        [JsonPropertyName("LastName")]
        public string LastName { get; set; } = null!;
        [Required]
        [JsonPropertyName("Telephone")]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("Role")]
        public string? Role { get; set; } // Leviathan does not accept entries with no value for Role
        [EmailAddress]
        [JsonPropertyName("Email")]
        public string? Email { get; set; }
    }
}
