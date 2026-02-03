using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Xunit;


namespace RaumiDiscord.Core.Tests;


public class DeltaraumiControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DeltaraumiControllerTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Ping_ReturnsPingConnection()
    {
        var response = await _client.GetAsync("/api/deltaraumi/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("pingConnection",body);
    }
}
