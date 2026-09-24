using Microsoft.AspNetCore.Mvc.Testing;
using ONIONARCH.Infrastructure.Versioning;
using ONIONARCH.Presentation.API;
using System.Text.Json;

namespace ONIONARCH.Tests.InfrastructureTests;

/// <summary>
/// Boots the real ONIONARCH.Presentation.API host in-memory and verifies the application version is
/// surfaced where callers can see it: the <c>/health</c> response and the OpenAPI document.
/// Neither endpoint touches the database.
/// </summary>
[TestFixture]
public class ApplicationVersionIntegrationTests
{
    /// <summary>
    /// Verifies that the <c>/health</c> response body reports the semantic and informational versions.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HealthEndpoint_Should_ReportApplicationVersion()
    {
        using var factory = new WebApplicationFactory<ApiAssemblyMarker>();
        var client = factory.CreateClient();

        // The sample health check reports a random status, so the HTTP status code varies;
        // the body (including the version) is written either way.
        var response = await client.GetAsync("/health");
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(document.RootElement.GetProperty("version").GetString(),
                Is.EqualTo(ApplicationVersion.Current.SemanticVersion));
            Assert.That(document.RootElement.GetProperty("informationalVersion").GetString(),
                Is.EqualTo(ApplicationVersion.Current.InformationalVersion));
        }
    }

    /// <summary>
    /// Verifies that the OpenAPI document's <c>info.version</c> is the application's semantic version.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task OpenApiDocument_Should_ReportApplicationVersion()
    {
        using var factory = new WebApplicationFactory<ApiAssemblyMarker>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var documentVersion = document.RootElement.GetProperty("info").GetProperty("version").GetString();
        Assert.That(documentVersion, Is.EqualTo(ApplicationVersion.Current.SemanticVersion));
    }
}
