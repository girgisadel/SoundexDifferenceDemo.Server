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
    Task<TQuote?> GetByIdAsync(string quoteId);
    Task<TQuote?> GetByTextAsync(string text);
    Task<IReadOnlyList<TQuote>> NormalFilterQuotesByTextAsync(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    IQueryable<TQuote> DeferredNormalFilterQuotesByText(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    IQueryable<TQuote> DeferredFreeTextSearchFilterQuotesByText(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    Task<long> CountNormalTextFilteredQuotesByTextAsync(string? searchTerm, DateTime? from, DateTime? to);
    Task<long> CountFreeTextSearchFilteredQuotesByTextAsync(string? searchTerm, DateTime? from, DateTime? to);
    Task<IReadOnlyList<TQuote>> SoundexFilterQuotesByAuthorAsync(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    IQueryable<TQuote> DeferredSoundexFilterQuotesByAuthor(string? searchTerm, int page, int pageSize, DateTime? from, DateTime? to, string orderBy, bool isDescending);
    Task<long> CountSoundexFilteredQuotesByAuthorAsync(string? searchTerm, DateTime? from, DateTime? to);
    IQueryable<string> DeferredGetAuthorsByAuthorSoundexAsync(string searchTerm);
}
