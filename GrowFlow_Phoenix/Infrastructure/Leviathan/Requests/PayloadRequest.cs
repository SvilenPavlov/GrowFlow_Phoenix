using System.Reflection;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.Infrastructure.Leviathan.Requests
{
    public class PayloadRequest<T> : BaseRequest
    {
        [JsonExtensionData]
        public Dictionary<string, object?> Fields { get; } = new();

        public PayloadRequest(T payload)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var jsonName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name;
                Fields[jsonName] = prop.GetValue(payload);
            }

            Fields[base.User.Key] = base.User.Value;
            Fields[base.Key.Key] = base.Key.Value;
        }

        // This request could also include query params, endpoint and other metadata if needed or they could be split up in different interfaces
    }
}

