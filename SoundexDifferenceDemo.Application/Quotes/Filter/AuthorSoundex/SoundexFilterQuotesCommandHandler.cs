using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Filter.AuthorSoundex;

internal class SoundexFilterQuotesCommandHandler(IQuoteManager<Quote> manager) : ICommandHandler<SoundexFilterQuotesCommand, PaginatedResult<QuoteItem>>
{
    public async Task<AppResult<PaginatedResult<QuoteItem>>> Handle(SoundexFilterQuotesCommand request, CancellationToken cancellationToken)
    {
        var items = await manager.DeferredSoundexFilterQuotesByAuthor(request.SearchTerm?.Trim(),
            request.Page,
            request.PageSize,
            request.CreatedAtFrom,
            request.CreatedAtTo,
            request.OrderBy,
            request.IsDescending).Select(q => new QuoteItem
            {
                Author = q.Author,
                CreatedAt = q.CreatedAt,
                Id = q.Id,
                Text = q.Text,
            })
            .ToListAsync(cancellationToken);

        var count = await manager.CountSoundexFilteredQuotesByAuthorAsync(request.SearchTerm?.Trim(), request.CreatedAtFrom, request.CreatedAtTo);

        return new PaginatedResult<QuoteItem>(items, items.Count, count, request.Page, request.PageSize);
    }
}