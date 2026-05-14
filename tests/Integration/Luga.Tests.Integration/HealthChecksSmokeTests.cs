using System.Net;

using Luga.Tests.Integration.Fixtures;

namespace Luga.Tests.Integration;

/// <summary>
/// Smoke test that proves the host boots end-to-end against the throwaway SQL
/// container. Hits the liveness endpoint (no DB dependencies) so a broken host
/// surfaces immediately even before migrations exist.
/// </summary>
public sealed class HealthChecksSmokeTests(SqlServerFixture sqlServer) : IntegrationTestBase(sqlServer)
{
    [Fact]
    public async Task Liveness_ReturnsOk()
    {
        HttpResponseMessage response = await Client.GetAsync(new Uri("/health/live", UriKind.Relative));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
