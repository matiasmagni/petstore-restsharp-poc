using RestSharp;

namespace PetstoreRestsharp.Core.Clients;

/// <summary>
/// Abstraction over the HTTP client. Used by tests and helpers. Allows swapping in a
/// fake client in unit tests (SOLID - DIP / ISP).
/// </summary>
public interface IApiClient
{
    Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default);
}