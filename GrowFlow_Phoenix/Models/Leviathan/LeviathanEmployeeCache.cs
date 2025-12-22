using GrowFlow_Phoenix.Models.Utility.IUtility;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.Models.Leviathan
{
    public class LeviathanEmployeeCache : ISyncable, IEmployeeComparable
    {
        public Guid Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = null!;

        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = null!;
        [JsonPropertyName("role")]
        public string? Role { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
        [JsonPropertyName("id")]
        public Guid LeviathanId { get; set; }
        public DateTime LastSyncedAt { get; set; }
    }
}
