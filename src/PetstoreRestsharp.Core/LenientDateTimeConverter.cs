using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PetstoreRestsharp.Core;

/// <summary>
/// The petstore demo serializes dates as "yyyy-MM-dd'T'HH:mm:ss.fff+0000",
/// which System.Text.Json's strict ISO-8601 parser does not accept.
/// This converter falls back to a lenient parser (KISS).
/// </summary>
public sealed class LenientDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dt))
                return dt;
        }
        else if (reader.TokenType is JsonTokenType.Number)
        {
            return reader.GetDateTime();
        }

        throw new JsonException($"Unable to parse date time value from token type {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToString("O", CultureInfo.InvariantCulture));
    }
}