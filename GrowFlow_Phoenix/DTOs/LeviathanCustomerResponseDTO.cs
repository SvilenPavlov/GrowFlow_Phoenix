using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.DTOs
{
    public class LeviathanCustomerResponseDTO
    {
        public Guid LeviathanEmployeeId { get; set; }

        [JsonPropertyName("id")]
        public Guid LeviathanId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; } = null!;

        [JsonPropertyName("Address")]
        public string? Address { get; set; } = null!;
    }
}
