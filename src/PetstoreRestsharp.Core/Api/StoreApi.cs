using PetstoreRestsharp.Core.Clients;
using PetstoreRestsharp.Core.Models;
using RestSharp;

namespace PetstoreRestsharp.Core.Api;

/// <summary>
/// Encapsulates all /store endpoints. Single Responsibility (SOLID).
/// </summary>
public sealed class StoreApi : BaseApiHelper
{
    private const string Resource = "/store";

    public StoreApi(IApiClient apiClient) : base(apiClient) { }

    public Task<RestResponse> GetInventoryAsync(CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/inventory", Method.Get), ct);

    public Task<RestResponse> PlaceOrderAsync(Order order, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/order", Method.Post).AddJsonBody(order), ct);

    public Task<RestResponse> GetOrderByIdAsync(long orderId, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/order/{orderId}", Method.Get), ct);

    public Task<RestResponse> DeleteOrderAsync(long orderId, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/order/{orderId}", Method.Delete), ct);
}