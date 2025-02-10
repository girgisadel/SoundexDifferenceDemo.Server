using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Abstractions.Repositories;

public interface IQuoteRepository<TQuote> : IDisposable where TQuote : Quote
{
    Task<string> GetIdAsync(TQuote quote, CancellationToken cancellationToken);

    Task<string?> GetTextAsync(TQuote quote, CancellationToken cancellationToken);
    Task SetTextAsync(TQuote quote, string? text, CancellationToken cancellationToken);


    Task<string?> GetAuthorAsync(TQuote quote, CancellationToken cancellationToken);
    Task SetAuthorAsync(TQuote quote, string? author, CancellationToken cancellationToken);


    Task<DateTime> GetCreatedAtAsync(TQuote quote, CancellationToken cancellationToken);


    Task<string?> GetNormalizedAuthorAsync(TQuote quote, CancellationToken cancellationToken);
    Task SetNormalizedAuthorAsync(TQuote quote, string? normalizedAuthor, CancellationToken cancellationToken);

    
    Task<AppResult> CreateAsync(TQuote quote, CancellationToken cancellationToken);

    Task<AppResult> UpdateAsync(TQuote quote, CancellationToken cancellationToken);

    Task<AppResult> DeleteAsync(TQuote quote, CancellationToken cancellationToken);

    Task<TQuote?> FindByIdAsync(string quoteId, CancellationToken cancellationToken);

    Task<TQuote?> FindByTextAsync(string text, CancellationToken cancellationToken);
    
    Task<TQuote?> GetByIdAsync(string quoteId, CancellationToken cancellationToken);

    Task<TQuote?> GetByTextAsync(string text, CancellationToken cancellationToken);

    Task<IReadOnlyList<TQuote>> GetQuotesByPaginationSpecificationAsync(IPaginationSpecification<TQuote> specification, CancellationToken cancellationToken);

    IQueryable<TQuote> DeferredGetQuotesByPaginationSpecification(IPaginationSpecification<TQuote> specification, CancellationToken cancellationToken);

    IQueryable<TQuote> DeferredGetQuotesBySpecification(ISpecification<TQuote> specification, CancellationToken cancellationToken);

    Task<long> CountQuotesBySpecificationAsync(ISpecification<TQuote> specification, CancellationToken cancellationToken);
}
