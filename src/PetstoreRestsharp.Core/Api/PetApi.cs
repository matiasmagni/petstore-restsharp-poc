using PetstoreRestsharp.Core.Clients;
using PetstoreRestsharp.Core.Models;
using RestSharp;

namespace PetstoreRestsharp.Core.Api;

/// <summary>
/// Encapsulates all /pet endpoints. Single Responsibility (SOLID).
/// </summary>
public sealed class PetApi : BaseApiHelper
{
    private const string Resource = "/pet";

    public PetApi(IApiClient apiClient) : base(apiClient) { }

    public Task<RestResponse> AddPetAsync(Pet pet, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest(Resource, Method.Post).AddJsonBody(pet), ct);

    public Task<RestResponse> UpdatePetAsync(Pet pet, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest(Resource, Method.Put).AddJsonBody(pet), ct);

    public Task<RestResponse> UpdateWithFormAsync(long petId, string name, string status, CancellationToken ct = default)
    {
        var request = CreateRequest($"{Resource}/{petId}", Method.Post);
        request.AddParameter("name", name);
        request.AddParameter("status", status);
        return ExecuteAsync(request, ct);
    }

    public Task<RestResponse> UploadImageAsync(long petId, byte[] file, string fileName, string? additionalMetadata = null, CancellationToken ct = default)
    {
        var request = CreateRequest($"{Resource}/{petId}/uploadImage", Method.Post);
        request.AlwaysMultipartFormData = true;
        if (!string.IsNullOrWhiteSpace(additionalMetadata))
            request.AddParameter("additionalMetadata", additionalMetadata);
        request.AddFile("file", file, fileName);
        return ExecuteAsync(request, ct);
    }

    public Task<RestResponse> GetByStatusAsync(string status, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/findByStatus", Method.Get).AddQueryParameter("status", status), ct);

    public Task<RestResponse> GetByTagsAsync(params string[] tags)
    {
        var request = CreateRequest($"{Resource}/findByTags", Method.Get);
        foreach (var tag in tags)
            request.AddQueryParameter("tags", tag);
        return ExecuteAsync(request);
    }

    public Task<RestResponse> GetByIdAsync(long petId, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/{petId}", Method.Get), ct);

    public Task<RestResponse> DeleteAsync(long petId, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/{petId}", Method.Delete), ct);
}