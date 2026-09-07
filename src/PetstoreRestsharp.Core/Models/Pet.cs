using System.Text.Json.Serialization;

namespace PetstoreRestsharp.Core.Models;

public enum PetStatus
{
    [JsonPropertyName("available")] Available,
    [JsonPropertyName("pending")] Pending,
    [JsonPropertyName("sold")] Sold
}

public class Pet
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("category")] public Category? Category { get; set; }
    [JsonPropertyName("photoUrls")] public List<string> PhotoUrls { get; set; } = new();
    [JsonPropertyName("tags")] public List<Tag>? Tags { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
}
