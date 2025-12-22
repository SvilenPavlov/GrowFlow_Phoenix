using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs.Leviathan.Employee
{
    public class EmployeeCreateDTO
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = null!;
        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;
        //Leviathan accepts non-email values
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
