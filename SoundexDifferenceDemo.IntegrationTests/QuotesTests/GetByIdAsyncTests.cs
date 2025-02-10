
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using System.Net;
using System.Net.Http.Json;

namespace SoundexDifferenceDemo.IntegrationTests.QuotesTests;

public class GetByIdAsyncTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
{
    private readonly HttpClient client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOk()
    {
        // Arrange
        var quote = new Quote
        {
            Author = "GetById Author 1",
            Text = "GetById Text 1",
            NormalizedAuthor = "GetById Author 1".Normalize().ToLowerInvariant()
        };
        await dbContext.Quotes.AddAsync(quote);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/quotes/get-by-id/{quote.Id}");
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<QuoteItem>();
        result.Should().NotBeNull();
        result.Id.Should().Be(quote.Id);
        result.Text.Should().Be(quote.Text);
        result.Author.Should().Be(quote.Author);
        result.CreatedAt.Should().Be(quote.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsBadRequest()
    {
        // Act
        var response = await client.GetAsync($"/api/quotes/get-by-id/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result.Should().NotBeNull();
        result.Title.Should().Contain("Operation Failed");
        var errors = result.Extensions["errors"];
        errors.Should().NotBeNull();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Contains("QuoteNotFound").Should().BeTrue();
        responseString.Contains("Quote not found.").Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Act
        var response = await client.GetAsync("/api/quotes/Get-By-Id/invalid id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result.Should().NotBeNull();
        result.Title.Should().Contain("Validation Failed");
        var errors = result.Extensions["errors"];
        errors.Should().NotBeNull();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Contains("IdInvalid").Should().BeTrue();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
