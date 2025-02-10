using Bogus;
using FluentAssertions;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;
using System.Net.Http.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.IntegrationTests.QuotesTests;

public class NormalFilterQuotesAsyncTests(TestWebAppFactory factory) : BaseTest(factory), IAsyncLifetime
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
        count =  await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task NormalFilterQuotesAsync_WithValidAuthor_ReturnsOk()
    {
        // Assert
        count.Should().NotBeNull();

        // Arrange
        var searchTerm = await dbContext.Quotes.AsNoTracking().Select(q => q.Author).FirstAsync();
        var normalizedSearchTerm = searchTerm!.Normalize().ToLowerInvariant();

        // Act
        var response = await client.GetAsync($"/api/quotes/normal-filter?searchTerm={searchTerm}&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Items.Count.Should().BeInRange(1, 10);
    }

    [Fact]
    public async Task SearchQuotesByAuthorAsync_WithNonExistingAuthor_ReturnsEmptyList()
    {
        // Act
        var response = await client.GetAsync($"/api/quotes/normal-filter?searchTerm=xxxxxx?page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task NormalFilterQuotesAsync_WithDateRange_ReturnsFilteredQuotes()
    {
        // Assert
        count.Should().NotBeNull();

        // Arrange
        var from = DateTime.UtcNow.AddYears(-1);
        var to = DateTime.UtcNow;
        
        // Act
        var response = await client.GetAsync($"/api/quotes/normal-filter?createdAtFrom={from:O}&createdAtTo={to:O}&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Items.Should().AllSatisfy(q => q.CreatedAt.Should().BeOnOrAfter(from).And.BeOnOrBefore(to));
    }

    [Fact]
    public async Task NormalFilterQuotesAsync_WithSorting_ReturnsOrderedResults()
    {
        // Assert
        count.Should().NotBeNull();

        // Act
        var response = await client.GetAsync("/api/quotes/normal-filter?orderBy=Author&isDescending=true&page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Items.Should().BeInDescendingOrder(q => q.Author);
    }

    [Fact]
    public async Task NormalFilterQuotesAsync_WithPagination_ReturnsCorrectPage()
    {
        // Assert
        count.Should().NotBeNull();

        // Act
        var response = await client.GetAsync("/api/quotes/normal-filter?page=2&pageSize=10");
        response.EnsureSuccessStatusCode();

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<QuoteItem>>();
        result.Should().NotBeNull();
        result.Page.Should().Be(2);
        result.Items.Count.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public async Task NormalFilterQuotesAsync_WithInvalidDateRange_ReturnsBadRequest()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow.AddYears(-1);
        
        // Act
        var response = await client.GetAsync($"/api/quotes/normal-filter?createdAtFrom={from:O}&createdAtTo={to:O}&page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
