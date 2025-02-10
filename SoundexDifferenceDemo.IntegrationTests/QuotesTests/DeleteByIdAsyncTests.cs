using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Domain.Quotes;
using System.Net;
using System.Net.Http.Json;

namespace SoundexDifferenceDemo.IntegrationTests.QuotesTests;

public class DeleteByIdAsyncTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
{
    private readonly HttpClient client = factory.CreateClient();
    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public async Task DeleteByIdAsync_WithValidData_ReturnsOk()
    {
        // Arrange
        var quote = new Quote
        {
            Author = "Delete Author 1",
            Text = "Delete Text 1",
            NormalizedAuthor = "Delete Author 1".Normalize().ToLowerInvariant()
        };
        await dbContext.Quotes.AddAsync(quote);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await client.DeleteAsync($"/api/quotes/{quote.Id}");
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quoteFromDbAfterDeletion = await dbContext.Quotes.AsNoTracking().FirstOrDefaultAsync(q => q.Id == quote.Id);
        quoteFromDbAfterDeletion.Should().BeNull();
    }

    [Fact]
    public async Task DeleteByIdAsync_WithEmptyId_ReturnsMethodNotAllowed()
    {
        // Act
        var response = await client.DeleteAsync($"/api/quotes/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task DeleteByIdAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Act
        var response = await client.DeleteAsync($"/api/quotes/invalid id");

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
