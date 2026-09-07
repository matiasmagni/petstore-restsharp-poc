using System.Text.Json;
using PetstoreRestsharp.Core;
using RestSharp;

namespace PetstoreRestsharp.Core.Extensions;

/// <summary>
/// Helpers to safely read content from a RestResponse (DRY).
/// </summary>
public static class RestResponseExtensions
{
    public static T DeserializeOrThrow<T>(this RestResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException($"Cannot deserialize empty response content. Status: {response.StatusCode} {response.StatusDescription}");

        return JsonSerializer.Deserialize<T>(response.Content, Json.Options)
               ?? throw new InvalidOperationException($"Failed to deserialize content to {typeof(T).Name}");
    }

    public static Uri Resource(this RestResponse response)
        => response.ResponseUri ?? new Uri("unknown");
}