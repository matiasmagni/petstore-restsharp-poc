using PetstoreRestsharp.Core.Configuration;
using RestSharp;

namespace PetstoreRestsharp.Core.Clients;

/// <summary>
/// A single RestSharp client instance that is cheap enough to create per-test.
/// Centralizes the base URL and timeout so the whole suite shares the same defaults (DRY).
/// </summary>
public sealed class ApiClient : IApiClient
{
    public RestClient Client { get; }

    public ApiClient(TestSettings settings)
    {
        Client = new RestClient(new RestClientOptions(settings.BaseUrl)
        {
            Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds)
        });
    }

    public Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
        => Client.ExecuteAsync(request, cancellationToken);
}