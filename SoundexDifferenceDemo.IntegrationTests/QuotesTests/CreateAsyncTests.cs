using FluentAssertions;
using System.Net.Http.Json;
using System.Net;
using SoundexDifferenceDemo.Application.Quotes.Create;
using Microsoft.AspNetCore.Mvc;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.IntegrationTests.QuotesTests;

public class CreateAsyncTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
{
    private readonly HttpClient client = factory.CreateClient();
    private const string longText = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
        "aaaaaaaaaaaaaaaaaaaaa";

    private const string longAuthor = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsOk()
    {
        // Arrange
        var command = new CreateQuoteCommand
        {
            Text = "Create - Text 1",
            Author = "Create - Author 1"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/quotes", command);
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<QuoteIdItem>();
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateAsync_WithDuplicatedText_ReturnsBadRequest()
    {
        // Arrange
        var quote = new Quote
        {
            Author = "Create Author 1",
            Text = "Create Text 1",
            NormalizedAuthor = "Create Author 1".Normalize().ToLowerInvariant()
        };
        await dbContext.Quotes.AddAsync(quote);
        await dbContext.SaveChangesAsync();

        var recreateCommand = new CreateQuoteCommand
        {
            // Duplicated text
            Text = quote.Text!,
            Author = "Recreate Author 1"
        };

        // Act
        var recreateResponse = await client.PostAsJsonAsync("/api/quotes", recreateCommand);

        // Assert
        recreateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var recreateResult = await recreateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        recreateResult.Should().NotBeNull();
        recreateResult.Title.Should().Contain("Operation Failed");
        var errors = recreateResult.Extensions["errors"];
        errors.Should().NotBeNull();
        var recreateResultString = await recreateResponse.Content.ReadAsStringAsync();
        recreateResultString.Contains("QuoteInUse").Should().BeTrue();
        recreateResultString.Contains("This quote already exists.").Should().BeTrue();
    }

    [Theory]
    [InlineData("a", "Create Author 1", "Validation Failed", new string[] { "TextTooShort" })]
    [InlineData(longText, "Create Author 1", "Validation Failed", new string[] { "TextTooLong" })]
    [InlineData("Create Text 1 - Short", "a", "Validation Failed", new string[] { "AuthorTooShort" })]
    [InlineData("Create Text 1 - Long", longAuthor, "Validation Failed", new string[] { "AuthorTooLong" })]
    [InlineData("", "Create Author 2", "Validation Failed", new string[] { "TextEmptyField", "TextTooShort" })]
    [InlineData("Create Text 2", "", "Validation Failed", new string[] { "AuthorEmptyField", "AuthorTooShort" })]
    public async Task CreateAsync_WithInvalidData_ReturnsBadRequest(string text, string author, string title, string[] expectations)
    {
        // Arrange
        var commandTooShort = new CreateQuoteCommand
        {
            Text = text,
            Author = author
        };

        // Act
        var responseTooShort = await client.PostAsJsonAsync("/api/quotes", commandTooShort);

        // Assert
        responseTooShort.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var resultTooShort = await responseTooShort.Content.ReadFromJsonAsync<ProblemDetails>();
        resultTooShort.Should().NotBeNull();
        resultTooShort.Title.Should().Contain(title);
        var errorsTooShort = resultTooShort.Extensions["errors"];
        errorsTooShort.Should().NotBeNull();
        var resultTooShortString = await responseTooShort.Content.ReadAsStringAsync();
        foreach (var expectation in expectations)
        {
            resultTooShortString.Contains(expectation).Should().BeTrue();
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
