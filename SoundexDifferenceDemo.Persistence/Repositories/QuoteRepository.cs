using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Abstractions.Repositories;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Repositories;

public class QuoteRepository<TQuote>(DatabaseContext context) : IQuoteRepository<TQuote>
    where TQuote : Quote
{
    private bool _disposed;
    private DbSet<TQuote> QuotesSet => Context.Set<TQuote>();
    public virtual IQueryable<TQuote> Quotes => QuotesSet;

    public virtual DatabaseContext Context { get; init; } = context;

    public virtual bool AutoSaveChanges { get; set; } = true;

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return AutoSaveChanges
            ? Context.SaveChangesAsync(cancellationToken)
            : Task.CompletedTask;
    }

    public virtual async Task<AppResult> CreateAsync(TQuote quote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        Context.Add(quote);
        await SaveChangesAsync(cancellationToken);
        return AppResult.Success();
    }

    public virtual async Task<AppResult> DeleteAsync(TQuote quote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);

        Context.Remove(quote);
        try
        {
            await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return AppResult.Failure(new AppError("ConcurrencyFailure",
                "A concurrency conflict occurred during delete."));
        }
        return AppResult.Success();
    }

    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public virtual Task<TQuote?> FindByIdAsync(string quoteId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return QuotesSet.FindAsync([quoteId], cancellationToken).AsTask();
    }

    public virtual Task<TQuote?> FindByTextAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return Quotes.FirstOrDefaultAsync(q => q.Text == text, cancellationToken);
    }

    public virtual Task<string?> GetAuthorAsync(TQuote quote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        return Task.FromResult(quote.Author);
    }

    public virtual Task<DateTime> GetCreatedAtAsync(TQuote quote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        return Task.FromResult(quote.CreatedAt);
    }

    public virtual Task<string> GetIdAsync(TQuote quote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        return Task.FromResult(quote.Id);
    }

    public virtual async Task<IReadOnlyList<TQuote>> JustGetQuotesBySpecificationAsync(IPaginationSpecification<TQuote> specification,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(specification);
        return await PaginationSpecificationEvaluator<TQuote>.GetQuery(Quotes.AsNoTracking(), specification)
            .ToListAsync(cancellationToken);
    }

    public virtual IQueryable<TQuote> JustDeferredGetQuotesByPaginationSpecification(IPaginationSpecification<TQuote> specification,
      CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(specification);
        return PaginationSpecificationEvaluator<TQuote>.GetQuery(Quotes.AsNoTracking(), specification);
    }

    public virtual IQueryable<TQuote> JustDeferredGetQuotesBySpecification(ISpecification<TQuote> specification, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(specification);
        return SpecificationEvaluator<TQuote>.GetQuery(Quotes.AsNoTracking(), specification);
    }

    public virtual async Task<long> JustCountQuotesBySpecificationAsync(ISpecification<TQuote> specification, 
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator<TQuote>.GetQuery(Quotes.AsNoTracking(), specification)
            .LongCountAsync(cancellationToken);
    }

    public virtual Task<string?> GetTextAsync(TQuote quote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        return Task.FromResult(quote.Text);
    }

    public virtual Task SetAuthorAsync(TQuote quote, string? author, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        quote.Author = author;
        return Task.CompletedTask;
    }

    public virtual Task SetNormalizedAuthorAsync(TQuote quote, string? normalizedAuthor, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        quote.NormalizedAuthor = normalizedAuthor;
        return Task.CompletedTask;
    }

    public virtual Task SetTextAsync(TQuote quote, string? text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        quote.Text = text;
        return Task.CompletedTask;
    }

    public virtual async Task<AppResult> UpdateAsync(TQuote quote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);

        Context.Attach(quote);
        quote.ConcurrencyStamp = Guid.NewGuid().ToString();
        Context.Update(quote);
        try
        {
            await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return AppResult.Failure(new AppError("ConcurrencyFailure",
                "A concurrency conflict occurred during update."));
        }
        return AppResult.Success();
    }

    public virtual Task<TQuote?> JustFindByIdAsync(string quoteId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return QuotesSet.AsNoTracking().FirstOrDefaultAsync(q => q.Id == quoteId, cancellationToken = default);
    }

    public virtual Task<TQuote?> JustFindByTextAsync(string text, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return Quotes.AsNoTracking().FirstOrDefaultAsync(q => q.Text == text, cancellationToken);
    }

    public Task<string?> GetNormalizedAuthorAsync(TQuote quote, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        return Task.FromResult(quote.NormalizedAuthor);
    }
}
