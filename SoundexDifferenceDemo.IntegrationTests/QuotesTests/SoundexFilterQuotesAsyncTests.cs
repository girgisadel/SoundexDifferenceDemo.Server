using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;
using System.Net;
using System.Net.Http.Json;

namespace SoundexDifferenceDemo.IntegrationTests.QuotesTests;

public class SoundexFilterQuotesAsyncTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
{
    private readonly HttpClient client = factory.CreateClient();
    private int? count;

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();

        var quoteFaker = new Faker<Quote>()
            .RuleFor(q => q.Author, f => f.Name.FullName())
            .RuleFor(q => q.NormalizedAuthor, (f, q) => q.Author!.Normalize().ToUpperInvariant())
            .RuleFor(q => q.Text, f => f.Lorem.Sentence())
            .RuleFor(q => q.CreatedAt, f => f.Date.Between(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow));

        var quotes = quoteFaker.Generate(2000).DistinctBy(q => q.Text);
        dbContext.Quotes.AddRange(quotes);
        count = await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task SoundexFilterQuotesAsync_WithValidAuthor_ReturnsOk()
    {
        // Assert
        count.Should().NotBeNull();

        // Arrange
        var author = await dbContext.Quotes.AsNoTracking().Select(q => q.Author).FirstAsync();

        // Act
        var response = await client.GetAsync($"/api/quotes/soundex-filter?searchTerm={author}&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SoundexFilterQuotesAsync_WithNonExistingAuthor_ReturnsEmptyList()
    {
        // Act
        var response = await client.GetAsync("/api/quotes/soundex-filter?searchTerm=xxxxxx&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task SoundexFilterQuotesAsync_WithDateRange_ReturnsFilteredQuotes()
    {
        // Arrange
        var from = DateTime.UtcNow.AddYears(-1);
        var to = DateTime.UtcNow;

        // Act
        var response = await client.GetAsync($"/api/quotes/soundex-filter?createdAtFrom={from:O}&createdAtTo={to:O}&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Items.Should().AllSatisfy(q => q.CreatedAt.Should().BeOnOrAfter(from).And.BeOnOrBefore(to));
    }

    [Fact]
    public async Task SoundexFilterQuotesAsync_WithSorting_ReturnsOrderedResults()
    {
        // Act
        var response = await client.GetAsync("/api/quotes/soundex-filter?orderBy=Author&isDescending=true&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Items.Should().BeInDescendingOrder(q => q.Author);
    }

    [Fact]
    public async Task SoundexFilterQuotesAsync_WithPagination_ReturnsCorrectPage()
    {
        // Act
        var response = await client.GetAsync("/api/quotes/soundex-filter?page=2&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Page.Should().Be(2);
        result.Items.Count.Should().BeLessThanOrEqualTo(10);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
