using FluentAssertions;
using SoundexDifferenceDemo.WebApi.DTOs.Responses;
using System.Net;
using System.Net.Http.Json;

namespace SoundexDifferenceDemo.IntegrationTests.LookupsTests;

public class GetPageSizeValuesTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
{
    private readonly HttpClient client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetPageSizeValues_ReturnsSuccessAndExpectedValues()
    {
        // Act
        var response = await client.GetAsync("/api/lookups/Page-Size-Values");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var values = await response.Content.ReadFromJsonAsync<List<LookupResponse<int>>>();
        values.Should().NotBeNull();
        values.Should().HaveCountGreaterThan(0);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
