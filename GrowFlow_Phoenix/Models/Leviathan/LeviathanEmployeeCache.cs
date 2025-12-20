using GrowFlow_Phoenix.Models.Utility.IUtility;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.Models.Leviathan
{
    public class LeviathanEmployeeCache : ISyncable
    {
        public Guid Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("id")]
        public Guid LeviathanId { get; set; }
        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("role")]
        public string? Role { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        public DateTime LastSyncedAt { get; set; }
    }
}
