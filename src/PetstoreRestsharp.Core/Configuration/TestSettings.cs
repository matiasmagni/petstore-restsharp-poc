namespace PetstoreRestsharp.Core.Configuration;

/// <summary>
/// Immutable test settings. Sourced from environment variables first, with sensible defaults (KISS/DRY).
/// Non-trivial external dependencies (IConfiguration) are intentionally avoided to keep the framework simple.
/// </summary>
public sealed record TestSettings
{
    public static TestSettings Current { get; } = Create();

    public string BaseUrl { get; }
    public int TimeoutSeconds { get; }

    private TestSettings(string baseUrl, int timeoutSeconds)
    {
        BaseUrl = baseUrl;
        TimeoutSeconds = timeoutSeconds;
    }

    private static TestSettings Create()
    {
        var baseUrl = GetEnvironmentVariable("PETSTORE_BASE_URL", "https://petstore.swagger.io/v2");
        var timeout = int.Parse(GetEnvironmentVariable("PETSTORE_TIMEOUT_SECONDS", "30"));
        return new TestSettings(baseUrl, timeout);
    }

    private static string GetEnvironmentVariable(string name, string defaultValue)
        => Environment.GetEnvironmentVariable(name) ?? defaultValue;
}
