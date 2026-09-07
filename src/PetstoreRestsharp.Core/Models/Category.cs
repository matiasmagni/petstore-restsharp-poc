using System.Text.Json.Serialization;

namespace PetstoreRestsharp.Core.Models;

public class Category
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}
