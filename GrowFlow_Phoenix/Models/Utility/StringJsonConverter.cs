using System.Text.Json;
using System.Text.Json.Serialization;

namespace GrowFlow_Phoenix.Models.Utility
{
    // Needed to ensure Leviathon values not adhering to required type does not break desrializing to DTOs
    public class JsonStringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString()!;
                case JsonTokenType.Number:
                    return reader.GetDouble().ToString(); // covers int, long, double
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return reader.GetBoolean().ToString();
                case JsonTokenType.Null:
                    return string.Empty;
                case JsonTokenType.StartObject:
                    {
                        using var doc = JsonDocument.ParseValue(ref reader);
                        return doc.RootElement.GetRawText();
                    }

                case JsonTokenType.StartArray:
                    {
                        using var doc = JsonDocument.ParseValue(ref reader);
                        return doc.RootElement.GetRawText();
                    }
                default:
                    return string.Empty;
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }
}
