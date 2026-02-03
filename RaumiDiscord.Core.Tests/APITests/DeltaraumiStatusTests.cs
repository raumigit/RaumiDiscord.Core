using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace RaumiDiscord.Core.Tests;
public class DeltaraumiStatusTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DeltaraumiStatusTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Status_ReturnsExpectedStructure()
    {
        var response = await _client.GetAsync("/api/deltaraumi/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // 固定値は正確に比較
        Assert.Equal("0.1.3.18", root.GetProperty("serverVersion").GetString());
        Assert.Equal("1.0", root.GetProperty("apiVersion").GetString());

        // processCount
        Assert.Equal(JsonValueKind.Number, root.GetProperty("processCount").ValueKind);

        // availableMemoryMB
        Assert.Equal(JsonValueKind.Number, root.GetProperty("availableMemoryMB").ValueKind);

        // uptime
        Assert.Equal(JsonValueKind.String, root.GetProperty("uptime").ValueKind);

        // cpuUsagePercent
        Assert.Equal(JsonValueKind.Number, root.GetProperty("cpuUsagePercent").ValueKind);

        // requestDurationMs
        Assert.Equal(JsonValueKind.Number, root.GetProperty("requestDurationMs").ValueKind);

        // network
        var network = root.GetProperty("network");
        Assert.Equal(JsonValueKind.Number, network.GetProperty("bytesSent").ValueKind);
        Assert.Equal(JsonValueKind.Number, network.GetProperty("bytesReceived").ValueKind);
    }
}
