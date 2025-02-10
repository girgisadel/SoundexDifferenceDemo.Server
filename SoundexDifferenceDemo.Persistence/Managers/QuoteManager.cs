using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Repositories;
using SoundexDifferenceDemo.Application.Abstractions.Validators;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Persistence.Specifications;
using SoundexDifferenceDemo.SharedKernel;
using System.Diagnostics.CodeAnalysis;

namespace SoundexDifferenceDemo.Persistence.Managers;

public class QuoteManager<TQuote> : IQuoteManager<TQuote>
    where TQuote : Quote
{
    private bool _disposed;

    public QuoteManager(IQuoteRepository<TQuote> repository,
        IEnumerable<IQuoteValidator<TQuote>> quoteValidator,
        ILogger<QuoteManager<TQuote>> logger)
    {
        Repository = repository;
        Logger = logger;
        if (quoteValidator != null)
        {
            foreach (var v in quoteValidator)
            {
                Validators.Add(v);
            }
        }
    }

    public virtual ILogger Logger { get; init; }

    public virtual IQuoteRepository<TQuote> Repository { get; init; }

    public virtual IList<IQuoteValidator<TQuote>> Validators { get; init; } = [];

    public virtual CancellationToken CancellationToken => CancellationToken.None;

    public virtual async Task<AppResult> CreateAsync(TQuote quote)
    {
        ThrowIfDisposed();
        var result = await ValidateQuoteAsync(quote).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        await UpdateNormalizedAuthorAsync(quote).ConfigureAwait(false);
        return await Repository.CreateAsync(quote, CancellationToken).ConfigureAwait(false);
    }

    public virtual Task<AppResult> DeleteAsync(TQuote quote)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        return Repository.DeleteAsync(quote, CancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            Repository.Dispose();
            _disposed = true;
        }
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public virtual Task<TQuote?> FindByIdAsync(string quoteId)
    {
        ThrowIfDisposed();
        return Repository.FindByIdAsync(quoteId, CancellationToken);
    }

    public virtual Task<TQuote?> FindByTextAsync(string text)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(text);
        return Repository.FindByTextAsync(text, CancellationToken);
    }

    public virtual async Task<string?> GetAuthorAsync(TQuote quote)
    {
        ThrowIfDisposed();
        return await Repository.GetAuthorAsync(quote, CancellationToken)
            .ConfigureAwait(false);
    }

    public virtual async Task<DateTime> GetCreatedAtAsync(TQuote quote)
    {
        ThrowIfDisposed();
        return await Repository.GetCreatedAtAsync(quote, CancellationToken)
            .ConfigureAwait(false);
    }

    public virtual async Task<string> GetIdAsync(TQuote quote)
    {
        ThrowIfDisposed();
        return await Repository.GetIdAsync(quote, CancellationToken)
            .ConfigureAwait(false);
    }

    public virtual async Task<IReadOnlyList<TQuote>> NormalFilterQuotesByTextAsync(string? searchTerm, int page, int pageSize,
        DateTime? from, DateTime? to, string orderBy, bool isDescending)
        => await DeferredNormalFilterQuotesByText(searchTerm, page, pageSize, from, to, orderBy, isDescending).ToListAsync(CancellationToken);

    public virtual IQueryable<TQuote> DeferredNormalFilterQuotesByText(string? searchTerm, int page, int pageSize, 
        DateTime? from, DateTime? to, string orderBy, bool isDescending)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(orderBy);
        page = page <= 0 ? Constants.DefaultPage : page;
        pageSize = pageSize <= 0 ? Constants.DefaultPageSize : pageSize;
        var itemsSpecification = new NormalFilterQuotesByTextPaginatedSpecification<TQuote>(searchTerm, page, pageSize, from, to, orderBy, isDescending);
        return Repository.DeferredGetQuotesByPaginationSpecification(itemsSpecification, CancellationToken);
    }

    public virtual IQueryable<TQuote> DeferredFreeTextSearchFilterQuotesByText(string? searchTerm, int page, int pageSize,
        DateTime? from, DateTime? to, string orderBy, bool isDescending)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(orderBy);
        page = page <= 0 ? Constants.DefaultPage : page;
        pageSize = pageSize <= 0 ? Constants.DefaultPageSize : pageSize;
        var itemsSpecification = new FreeTextSearchFilterQuotesByTextPaginatedSpecification<TQuote>(searchTerm, page, pageSize, from, to, orderBy, isDescending);
        return Repository.DeferredGetQuotesByPaginationSpecification(itemsSpecification, CancellationToken);
    }

    public virtual async Task<long> CountNormalTextFilteredQuotesByTextAsync(string? searchTerm, DateTime? from, DateTime? to)
    {
        ThrowIfDisposed();
        var countSpecification = new NormalFilterQuotesByTextSpecification<TQuote>(searchTerm, from, to);
        return await Repository.CountQuotesBySpecificationAsync(countSpecification, CancellationToken);
    }

    public virtual async Task<long> CountFreeTextSearchFilteredQuotesByTextAsync(string? searchTerm, DateTime? from, DateTime? to)
    {
        ThrowIfDisposed();
        var countSpecification = new FreeTextSearchFilterQuotesByTextSpecification<TQuote>(searchTerm, from, to);
        return await Repository.CountQuotesBySpecificationAsync(countSpecification, CancellationToken);
    }

    public virtual async Task<IReadOnlyList<TQuote>> SoundexFilterQuotesByAuthorAsync(string? searchTerm, int page, int pageSize, 
        DateTime? from, DateTime? to, string orderBy, bool isDescending)
        => await DeferredSoundexFilterQuotesByAuthor(searchTerm, page, pageSize, from, to, orderBy, isDescending).ToListAsync(CancellationToken);

    public virtual IQueryable<string> DeferredGetAuthorsByAuthorSoundexAsync(string searchTerm)
    {
        ArgumentNullException.ThrowIfNull(searchTerm);
        var itemsSpecification = new GetAuthorsByAuthorSoundexSpecification<TQuote>(NormalizeAuthor(searchTerm));
        return Repository.DeferredGetQuotesBySpecification(itemsSpecification, CancellationToken)
            .Select(q => q.Author!);
    }

    public virtual IQueryable<TQuote> DeferredSoundexFilterQuotesByAuthor(string? searchTerm, int page, int pageSize,
        DateTime? from, DateTime? to, string orderBy, bool isDescending)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(orderBy);
        page = page <= 0 ? Constants.DefaultPage : page;
        pageSize = pageSize <= 0 ? Constants.DefaultPageSize : pageSize;
        var itemsSpecification = new SoundexFilterQuotesByAuthorPaginatedSpecification<TQuote>(NormalizeAuthor(searchTerm), page, pageSize, from, to, orderBy, isDescending);
        return Repository.DeferredGetQuotesByPaginationSpecification(itemsSpecification, CancellationToken);
    }

    public virtual async Task<long> CountSoundexFilteredQuotesByAuthorAsync(string? searchTerm, DateTime? from, DateTime? to)
    {
        ThrowIfDisposed();
        var countSpecification = new SoundexFilterQuotesByAuthorSpecification<TQuote>(NormalizeAuthor(searchTerm), from, to);
        return await Repository.CountQuotesBySpecificationAsync(countSpecification, CancellationToken);
    }

    public virtual Task<string?> GetTextAsync(TQuote quote)
    {
        ThrowIfDisposed();
        return Repository.GetTextAsync(quote, CancellationToken);
    }

    public virtual async Task<AppResult> SetAuthorAsync(TQuote quote, string? author)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        await Repository.SetAuthorAsync(quote, author, CancellationToken)
            .ConfigureAwait(false);
        return await UpdateAsync(quote).ConfigureAwait(false);
    }

    public virtual async Task<AppResult> SetTextAsync(TQuote quote, string? text)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        await Repository.SetTextAsync(quote, text, CancellationToken)
            .ConfigureAwait(false);
        return await UpdateAsync(quote).ConfigureAwait(false);
    }

    public virtual async Task<AppResult> UpdateAsync(TQuote quote)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(quote);
        var result = await ValidateQuoteAsync(quote).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await Repository.UpdateAsync(quote, CancellationToken).ConfigureAwait(false);
    }

    protected async Task<AppResult> ValidateQuoteAsync(TQuote quote)
    {
        List<AppError>? errors = null;
        foreach (var v in Validators)
        {
            var result = await v.ValidateAsync(this, quote).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= [];
                errors.AddRange(result.Errors);
            }
        }
        if (errors?.Count > 0)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("User validation failed: {errors}.", string.Join(";", errors.Select(e => e.Code)));
            }
            return AppResult.Failure([.. errors]);
        }
        return AppResult.Success();
    }

    public virtual Task<TQuote?> GetByIdAsync(string quoteId)
    {
        ThrowIfDisposed();
        return Repository.GetByIdAsync(quoteId, CancellationToken);
    }

    public virtual Task<TQuote?> GetByTextAsync(string text)
    {
        ThrowIfDisposed();
        return Repository.GetByTextAsync(text, CancellationToken);
    }

    public virtual async Task UpdateNormalizedAuthorAsync(TQuote quote)
    {
        var normalizedAuthor = NormalizeAuthor(await GetAuthorAsync(quote).ConfigureAwait(false));
        await Repository.SetNormalizedAuthorAsync(quote, normalizedAuthor, CancellationToken).ConfigureAwait(false);
    }

    [return: NotNullIfNotNull(nameof(author))]
    public virtual string? NormalizeAuthor(string? author)
    {
        if (author == null)
        {
            return null;
        }

        return author.Normalize().ToLowerInvariant();
    }
}
