using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Abstractions.Managers;

public interface IQuoteManager<TQuote> : IDisposable where TQuote : Quote
{
    string? NormalizeAuthor(string? author);
    Task<string> GetIdAsync(TQuote quote);
    Task<string?> GetTextAsync(TQuote quote);
    Task<AppResult> SetTextAsync(TQuote quote, string? text);
    Task<string?> GetAuthorAsync(TQuote quote);
    Task<DateTime> GetCreatedAtAsync(TQuote quote);
    Task<AppResult> SetAuthorAsync(TQuote quote, string? author);
    Task<AppResult> CreateAsync(TQuote quote);
    Task<AppResult> UpdateAsync(TQuote quote);
    Task<AppResult> DeleteAsync(TQuote quote);
    Task<TQuote?> FindByIdAsync(string quoteId);
    Task<TQuote?> FindByTextAsync(string text);
    Task<TQuote?> JustFindByIdAsync(string quoteId);
    Task<TQuote?> JustFindByTextAsync(string text);
    Task<IReadOnlyList<TQuote>> JustNormalTextFilterQuotesAsync(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    IQueryable<TQuote> JustDeferredNormalTextFilterQuotes(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    IQueryable<TQuote> JustDeferredFreeTextSearchFilterQuotes(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    Task<long> JustCountNormalTextFilteredQuotesAsync(string? searchTerm, DateTime? from, DateTime? to);
    Task<long> JustCountFreeTextSearchFilteredQuotesAsync(string? searchTerm, DateTime? from, DateTime? to);
    IQueryable<TQuote> JustDeferredGetAuthorsSoundexAsync(string searchTerm);
    Task<IReadOnlyList<TQuote>> JustAuthorSoundexFilterQuotesAsync(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    IQueryable<TQuote> JustDeferredAuthorSoundexFilterQuotes(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    Task<long> JustCountAuthorSoundexFilteredQuotesAsync(string? searchTerm, DateTime? from, DateTime? to);
}
