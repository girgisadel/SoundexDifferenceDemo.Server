using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Filter.NormalText;

internal class NormalFilterQuotesCommandHandler(IQuoteManager<Quote> manager) : ICommandHandler<NormalFilterQuotesCommand, PaginatedResult<QuoteItem>>
{
    public async Task<AppResult<PaginatedResult<QuoteItem>>> Handle(NormalFilterQuotesCommand request, CancellationToken cancellationToken)
    {
        var items = await manager.DeferredNormalFilterQuotesByText(request.SearchTerm?.Trim(),
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

        var count = await manager.CountNormalTextFilteredQuotesByTextAsync(request.SearchTerm?.Trim(), request.CreatedAtFrom, request.CreatedAtTo);

        return new PaginatedResult<QuoteItem>(items, items.Count, count, request.Page, request.PageSize);
    }
}