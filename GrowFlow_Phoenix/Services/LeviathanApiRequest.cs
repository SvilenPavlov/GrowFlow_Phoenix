using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.Services
{
    //public class LeviathanApiRequest<T>
    //{
    //    public LeviathanApiRequest(IEnumerable<KeyValuePair<string, object>> payload, string apiUser, string apiKey)
    //    {
    //        Payload = payload;
    //        ApiUser = apiUser;
    //        ApiKey = apiKey;
    //    }
    //    public string ApiUser { get; set; }
    //    public string ApiKey { get; set; }
    //    public IEnumerable<KeyValuePair<string, object>> Payload { get; set; } 

    //}

    public class LeviathanApiRequest<T>
    {
        [JsonIgnore]
        private readonly string _apiUser;

        [JsonIgnore]
        private readonly string _apiKey;

        [JsonExtensionData]
        public Dictionary<string, object?> Fields { get; } = new();

        public LeviathanApiRequest(T payload, string apiUser, string apiKey)
        {
            _apiUser = apiUser;
            _apiKey = apiKey;

            // Add payload properties to the dictionary
            foreach (var prop in typeof(T).GetProperties())
            {
                var jsonName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name;
                Fields[jsonName] = prop.GetValue(payload);
            }

            // Add credentials at the same level
            Fields["ApiUser"] = _apiUser;
            Fields["ApiKey"] = _apiKey;
        }
    }
}

