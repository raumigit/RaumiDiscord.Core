using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Tests;

[Collection("Sequential")]
public class UrlDataModelsControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UrlDataModelsControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndList()
    {
        // Act
        var response = await _client.GetAsync("/api/UrlDataModels");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<UrlDataModel>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Post_CreatesUrlDataModel()
    {
        // Arrange
        var newModel = new UrlDataModel
        {
            Url = "https://genshin.hoyoverse.com/ja/gift?code=TEST123",
            UrlType = "gi",
            Ttl = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/UrlDataModels", newModel);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<UrlDataModel>();
        Assert.NotNull(created);
        Assert.Equal(newModel.Url, created.Url);
        Assert.Equal(newModel.UrlType, created.UrlType);
        Assert.True(created.Id > 0, "ID should be auto-generated");
    }

    [Fact]
    public async Task GetById_ReturnsUrlDataModel_WhenExists()
    {
        // Arrange - データを作成
        var model = new UrlDataModel
        {
            Url = "https://hsr.hoyoverse.com/gift?code=TESTCODE",
            UrlType = "hsr",
            Ttl = DateTime.UtcNow.AddDays(3)
        };

        var postResponse = await _client.PostAsJsonAsync("/api/UrlDataModels", model);
        var created = await postResponse.Content.ReadFromJsonAsync<UrlDataModel>();
        Assert.NotNull(created);

        // Act
        var getResponse = await _client.GetAsync($"/api/UrlDataModels/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<UrlDataModel>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal(created.Url, fetched.Url);
        Assert.Equal(created.UrlType, fetched.UrlType);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/UrlDataModels/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesUrlDataModel()
    {
        // Arrange - データを作成
        var original = new UrlDataModel
        {
            Url = "https://zenless.hoyoverse.com/redemption?code=ORIGINAL",
            UrlType = "zzz",
            Ttl = DateTime.UtcNow.AddDays(1)
        };

        var postResponse = await _client.PostAsJsonAsync("/api/UrlDataModels", original);
        var created = await postResponse.Content.ReadFromJsonAsync<UrlDataModel>();
        Assert.NotNull(created);

        // 更新内容を準備
        created.Url = "https://zenless.hoyoverse.com/redemption?code=UPDATED";
        created.UrlType = "zzz";
        created.Ttl = DateTime.UtcNow.AddDays(5);

        // Act
        var putResponse = await _client.PutAsJsonAsync($"/api/UrlDataModels/{created.Id}", created);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        // 更新されたことを確認
        var getResponse = await _client.GetAsync($"/api/UrlDataModels/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<UrlDataModel>();

        Assert.NotNull(updated);
        Assert.Equal("https://zenless.hoyoverse.com/redemption?code=UPDATED", updated.Url);
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var model = new UrlDataModel
        {
            Id = 999,
            Url = "https://example.com",
            UrlType = "url",
            Ttl = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/UrlDataModels/1", model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesUrlDataModel()
    {
        // Arrange - データを作成
        var model = new UrlDataModel
        {
            Url = "https://play.google.com/store/apps/details?id=com.YostarJP.BlueArchive",
            UrlType = "blac",
            Ttl = DateTime.UtcNow.AddDays(2)
        };

        var postResponse = await _client.PostAsJsonAsync("/api/UrlDataModels", model);
        var created = await postResponse.Content.ReadFromJsonAsync<UrlDataModel>();
        Assert.NotNull(created);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/UrlDataModels/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 削除されたことを確認
        var getResponse = await _client.GetAsync($"/api/UrlDataModels/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithDifferentUrlTypes()
    {
        // Arrange - 異なるゲームタイプのURLをテスト
        var testData = new[]
        {
            new UrlDataModel { Url = "https://example.com", UrlType = "url", Ttl = DateTime.UtcNow },
            new UrlDataModel { Url = "https://genshin.hoyoverse.com/ja/gift?code=ABC", UrlType = "gi", Ttl = DateTime.UtcNow },
            new UrlDataModel { Url = "https://hsr.hoyoverse.com/gift?code=XYZ", UrlType = "hsr", Ttl = DateTime.UtcNow },
            new UrlDataModel { Url = "https://zenless.hoyoverse.com/redemption?code=123", UrlType = "zzz", Ttl = DateTime.UtcNow },
            new UrlDataModel { Url = "https://play.google.com/store/apps/details?id=com.kurogame.wutheringwaves.global", UrlType = "wuwa", Ttl = DateTime.UtcNow }
        };

        // Act & Assert
        foreach (var data in testData)
        {
            var response = await _client.PostAsJsonAsync("/api/UrlDataModels", data);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<UrlDataModel>();
            Assert.NotNull(created);
            Assert.Equal(data.UrlType, created.UrlType);
        }
    }
}