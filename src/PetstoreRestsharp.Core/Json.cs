using System.Text.Json;
using System.Text.Json.Serialization;

namespace PetstoreRestsharp.Core;

/// <summary>
/// Central place for all JSON serialization settings (DRY).
/// </summary>
public static class Json
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new LenientDateTimeConverter()
        }
    };

    public static JsonSerializerOptions RestSharpOptions() => Options;
}
