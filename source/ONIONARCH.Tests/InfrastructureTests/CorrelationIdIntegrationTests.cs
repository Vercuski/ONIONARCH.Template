using Microsoft.AspNetCore.Mvc.Testing;
using ONIONARCH.Infrastructure.Correlation;
using ONIONARCH.Presentation.API;

namespace ONIONARCH.Tests.InfrastructureTests;

/// <summary>
/// Boots the real ONIONARCH.Presentation.API host in-memory (via WebApplicationFactory) and
/// exercises it through an actual HttpClient, so this verifies the real Program.cs wiring —
/// including middleware order — rather than just the CorrelationIdMiddleware class in isolation.
/// Uses the /health endpoint specifically because it never touches the database, so this test
/// has no dependency on a real SQL Server/PostgreSQL/MySQL instance being reachable.
/// </summary>
[TestFixture]
public class CorrelationIdIntegrationTests
{
    /// <summary>Per-test setup (currently empty; each test creates its own factory).</summary>
    [SetUp]
    public void SetUp()
    {
        
    }

    /// <summary>Per-test teardown (currently empty; factories are disposed by each test).</summary>
    [TearDown]
    public void TearDown()
    {
    }

    /// <summary>
    /// Verifies that a request without an incoming correlation ID receives a newly generated GUID
    /// in the <see cref="CorrelationIdMiddleware.HeaderName"/> response header.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HealthEndpoint_Should_ReturnCorrelationIdResponseHeader()
    {
        using var factory = new WebApplicationFactory<ApiAssemblyMarker>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.That(response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values), Is.True);
        var headerValue = values!.Single();
        Assert.That(Guid.TryParse(headerValue, out _), Is.True);
    }

    /// <summary>
    /// Verifies that successive requests without an incoming correlation ID each receive a
    /// distinct ID.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Requests_WithoutIncomingHeader_Should_GetADifferentCorrelationIdEachTime()
    {
        using var factory = new WebApplicationFactory<ApiAssemblyMarker>();
        var client = factory.CreateClient();

        var first = await client.GetAsync("/health");
        var second = await client.GetAsync("/health");

        var firstId = first.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();
        var secondId = second.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();

        Assert.That(firstId, Is.Not.EqualTo(secondId));
    }
}
