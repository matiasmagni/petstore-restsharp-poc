using PetstoreRestsharp.Core.Api;
using PetstoreRestsharp.Core.Clients;
using PetstoreRestsharp.Core.Configuration;

namespace PetstoreRestsharp.Tests;

/// <summary>
/// Shared setup for all test suites. Each NUnit test gets a fresh instance,
/// so a fresh HTTP client is created per test. Keeps tests independent (KISS).
/// </summary>
public abstract class TestFixtureBase
{
    private readonly ApiClient _apiClient;

    protected PetApi Pets { get; }
    protected StoreApi Store { get; }
    protected UserApi Users { get; }

    protected TestFixtureBase()
    {
        _apiClient = new ApiClient(TestSettings.Current);
        Pets = new PetApi(_apiClient);
        Store = new StoreApi(_apiClient);
        Users = new UserApi(_apiClient);
    }
}