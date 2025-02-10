using FluentAssertions;
using SoundexDifferenceDemo.WebApi.DTOs.Responses;
using System.Net;
using System.Net.Http.Json;

namespace SoundexDifferenceDemo.IntegrationTests.LookupsTests;

public class GetOrderByValuesTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
{
    private readonly HttpClient client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetOrderByValues_ReturnsSuccessAndExpectedValues()
    {
        // Act
        var response = await client.GetAsync("/api/lookups/Order-By-Values");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var values = await response.Content.ReadFromJsonAsync<List<LookupResponse<string>>>();
        values.Should().NotBeNull();
        values.Should().HaveCountGreaterThan(0);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}