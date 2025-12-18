using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Phoenix.Employee
{
    public class EmployeeCreateDTO
    {
        [Required]
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; }
        [Required]
        [JsonPropertyName("LastName")]
        public string LastName { get; set; }
        [Required]
        [JsonPropertyName("Telephone")]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("Role")]
        public string? Role { get; set; }
        [EmailAddress]
        [JsonPropertyName("Email")]
        public string? Email { get; set; }
    }
}
