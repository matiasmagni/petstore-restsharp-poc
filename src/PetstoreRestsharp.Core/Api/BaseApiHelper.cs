using PetstoreRestsharp.Core.Clients;
using RestSharp;

namespace PetstoreRestsharp.Core.Api;

/// <summary>
/// Base class for all API resource helpers. Provides common request
/// plumbing so each resource keeps only its endpoint-specific code (DRY).
/// Uses the IApiClient abstraction (SOLID - DIP).
/// </summary>
public abstract class BaseApiHelper
{
    protected readonly IApiClient ApiClient;

    protected BaseApiHelper(IApiClient apiClient)
    {
        ApiClient = apiClient;
    }

    protected Task<RestResponse> ExecuteAsync(Method method, string resource, CancellationToken ct = default)
        => ExecuteAsync(new RestRequest(resource, method), ct);

    protected async Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken ct = default)
        => await ApiClient.ExecuteAsync(request, ct);

    protected static RestRequest CreateRequest(string resource, Method method)
        => new(resource, method);
}