using PetstoreRestsharp.Core.Clients;
using PetstoreRestsharp.Core.Models;
using RestSharp;

namespace PetstoreRestsharp.Core.Api;

/// <summary>
/// Encapsulates all /user endpoints. Single Responsibility (SOLID).
/// </summary>
public sealed class UserApi : BaseApiHelper
{
    private const string Resource = "/user";

    public UserApi(IApiClient apiClient) : base(apiClient) { }

    public Task<RestResponse> CreateAsync(User user, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest(Resource, Method.Post).AddJsonBody(user), ct);

    public Task<RestResponse> CreateWithListAsync(IEnumerable<User> users, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/createWithList", Method.Post).AddJsonBody(users), ct);

    public Task<RestResponse> CreateWithArrayAsync(IEnumerable<User> users, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/createWithArray", Method.Post).AddJsonBody(users), ct);

    public Task<RestResponse> LoginAsync(string username, string password, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/login", Method.Get)
            .AddQueryParameter("username", username)
            .AddQueryParameter("password", password), ct);

    public Task<RestResponse> LogoutAsync(CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/logout", Method.Get), ct);

    public Task<RestResponse> GetByUsernameAsync(string username, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/{username}", Method.Get), ct);

    public Task<RestResponse> UpdateAsync(string username, User user, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/{username}", Method.Put).AddJsonBody(user), ct);

    public Task<RestResponse> DeleteAsync(string username, CancellationToken ct = default)
        => ExecuteAsync(CreateRequest($"{Resource}/{username}", Method.Delete), ct);
}