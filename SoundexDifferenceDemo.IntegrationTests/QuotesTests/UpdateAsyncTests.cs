using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Application.Quotes.Update;
using SoundexDifferenceDemo.Domain.Quotes;
using System.Net;
using System.Net.Http.Json;

namespace SoundexDifferenceDemo.IntegrationTests.QuotesTests;

public class UpdateAsyncTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
{
    private readonly HttpClient client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ReturnsOk()
    {
        // Arrange
        var quote = new Quote
        {
            Author = "Update Author 1",
            Text = "Update Text 1",
            NormalizedAuthor = "Update Author 1".Normalize().ToLowerInvariant()
        };
        await dbContext.Quotes.AddAsync(quote);
        await dbContext.SaveChangesAsync();

        var updateAuthorCommand = new UpdateQuoteCommand
        {
            Id = quote.Id,
            Author = "Update Author 1 New"
        };

        // Act
        var updateAuthorResponse = await client.PutAsJsonAsync("/api/quotes", updateAuthorCommand);
        updateAuthorResponse.EnsureSuccessStatusCode();

        // Assert
        updateAuthorResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updateAuthorResult = await updateAuthorResponse.Content.ReadFromJsonAsync<QuoteIdItem>();
        updateAuthorResult.Should().NotBeNull();
        updateAuthorResult.Id.Should().NotBeNullOrEmpty();

        var quoteFromDbAfterUpdateAuthor = await dbContext.Quotes.AsNoTracking().FirstOrDefaultAsync(q => q.Id == updateAuthorResult.Id);
        quoteFromDbAfterUpdateAuthor.Should().NotBeNull();
        quoteFromDbAfterUpdateAuthor.Author.Should().Be(updateAuthorCommand.Author);

        // Arrange
        var updateTextCommand = new UpdateQuoteCommand
        {
            Id = quote.Id,
            Text = "Update Text 1 New",
        };

        // Act
        var updateTextResponse = await client.PutAsJsonAsync("/api/quotes", updateTextCommand);
        updateTextResponse.EnsureSuccessStatusCode();

        // Assert
        updateTextResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updateTextResult = await updateTextResponse.Content.ReadFromJsonAsync<QuoteIdItem>();
        updateTextResult.Should().NotBeNull();
        updateTextResult.Id.Should().NotBeNullOrEmpty();

        var quoteFromDbAfterUpdateText = await dbContext.Quotes.AsNoTracking().FirstOrDefaultAsync(q => q.Id == updateTextResult.Id);
        quoteFromDbAfterUpdateText.Should().NotBeNull();
        quoteFromDbAfterUpdateText.Text.Should().Be(updateTextCommand.Text);
    }

    [Fact]
    public async Task UpdateAsync_WithEmptyAuthorAndText_ReturnsBadRequest()
    {
        // Arrange
        var quote = new Quote
        {
            Author = "Update Author 2",
            Text = "Update Text 2",
            NormalizedAuthor = "Update Author 2".Normalize().ToLowerInvariant()
        };
        await dbContext.Quotes.AddAsync(quote);
        await dbContext.SaveChangesAsync();

        // Arrange
        var command = new UpdateQuoteCommand
        {
            Id = quote.Id,
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/quotes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result.Should().NotBeNull();
        result.Title.Should().Contain("Validation Failed");
        var errors = result.Extensions["errors"];
        errors.Should().NotBeNull();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Contains("UpdateQuoteCommand").Should().BeTrue();
        responseString.Contains("TextOrAuthorRequired").Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var command = new UpdateQuoteCommand
        {
            Id = "invalid id",
            Author = "Doesn't matter", // bc it didn't pass the validation layer.
            Text = "Doesn't matter." // bc it didn't pass the validation layer.
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/quotes", command);

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

    [Fact]
    public async Task UpdateAsync_WithValidDataButNoUpdatesWereMade_ReturnsBadRequest()
    {
        // Arrange
        var quote = new Quote
        {
            Author = "Update Author 3",
            Text = "Update Text 3",
            NormalizedAuthor = "Update Author 3".Normalize().ToLowerInvariant()
        };
        await dbContext.Quotes.AddAsync(quote);
        await dbContext.SaveChangesAsync();

        var updateCommand = new UpdateQuoteCommand
        {
            Id = quote.Id,
            Author = quote.Author,
            Text = quote.Text
        };

        // Act
        var updateResponse = await client.PutAsJsonAsync("/api/quotes", updateCommand);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        updateResult.Should().NotBeNull();
        updateResult.Title.Should().Contain("Operation Failed");
        var errors = updateResult.Extensions["errors"];
        errors.Should().NotBeNull();
        var updateResultString = await updateResponse.Content.ReadAsStringAsync();
        updateResultString.Contains("NoChangesDetected").Should().BeTrue();
        updateResultString.Contains("No updates were made.").Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_WithNotFoundId_ReturnsBadRequest()
    {
        // Arrange
        var command = new UpdateQuoteCommand
        {
            Id = Guid.NewGuid().ToString(), // not found id
            Author = "Doesn't matter",
            Text = "Doesn't matter."
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/quotes", command);

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

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
