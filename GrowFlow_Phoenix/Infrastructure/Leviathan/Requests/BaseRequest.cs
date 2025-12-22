using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.Infrastructure.Leviathan.Requests
{
    public class BaseRequest
    {
        [JsonIgnore]
        public KeyValuePair<string,string> User { get; }

        [JsonIgnore]
        public KeyValuePair<string,string> Key { get; }

        public BaseRequest()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            User = new KeyValuePair<string, string>(configuration["LeviathanApi:Credentials:User:Param"], configuration["LeviathanApi:Credentials:User:Value"]);
            Key = new KeyValuePair<string, string>(configuration["LeviathanApi:Credentials:Key:Param"], configuration["LeviathanApi:Credentials:Key:Value"]);
        }
    }
}
