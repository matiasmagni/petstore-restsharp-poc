using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using PetstoreRestsharp.Core.Api;
using PetstoreRestsharp.Core.Clients;
using PetstoreRestsharp.Core.Configuration;

namespace PetstoreRestsharp.Tests;

/// <summary>
/// Shared setup for all test suites. Each NUnit test gets a fresh instance,
/// so a fresh HTTP client is created per test. Keeps tests independent (KISS).
/// Also manages the ExtentReports lifecycle.
/// </summary>
public abstract class TestFixtureBase
{
    private readonly ApiClient _apiClient;

    protected static ExtentReports Extent;
    protected static ExtentTest Test;

    protected PetApi Pets { get; }
    protected StoreApi Store { get; }
    protected UserApi Users { get; }

    static TestFixtureBase()
    {
        var reportPath = Path.Combine(AppContext.BaseDirectory, "TestReport.html");
        var sparkReporter = new ExtentSparkReporter(reportPath);
        sparkReporter.Config.Theme = Theme.Standard;
        sparkReporter.Config.DocumentTitle = "Petstore API Test Report";
        sparkReporter.Config.ReportName = "Petstore API Test Execution Report";

        Extent = new ExtentReports();
        Extent.AttachReporter(sparkReporter);
    }

    protected TestFixtureBase()
    {
        _apiClient = new ApiClient(TestSettings.Current);
        Pets = new PetApi(_apiClient);
        Store = new StoreApi(_apiClient);
        Users = new UserApi(_apiClient);
    }

    [SetUp]
    public void SetupTest()
    {
        Test = Extent.CreateTest(TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void TearDownTest()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var message = TestContext.CurrentContext.Result.Message;

        switch (status)
        {
            case NUnit.Framework.Interfaces.TestStatus.Passed:
                Test.Log(Status.Pass, "Test Passed");
                break;
            case NUnit.Framework.Interfaces.TestStatus.Failed:
                Test.Log(Status.Fail, $"Test Failed: {message}");
                break;
            case NUnit.Framework.Interfaces.TestStatus.Skipped:
                Test.Log(Status.Skip, "Test Skipped");
                break;
            default:
                Test.Log(Status.Warning, "Test Inconclusive");
                break;
        }

        Extent.Flush();
    }
}