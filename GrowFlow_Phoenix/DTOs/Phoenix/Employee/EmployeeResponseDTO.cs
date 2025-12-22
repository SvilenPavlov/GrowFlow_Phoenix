using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Phoenix.Employee
{
    public class EmployeeResponseDTO
    {
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("LastName")]
        public string LastName { get; set; } = null!;
        [JsonPropertyName("Telephone")]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("Role")]
        public string? Role { get; set; }
        [EmailAddress]
        [JsonPropertyName("Email")]
        public string? Email { get; set; }

        public Dictionary<string,bool> ExternallySyncedEntries { get; set; }
    }
}
