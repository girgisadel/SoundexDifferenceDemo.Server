﻿using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Filter.FreeTextSearch;

internal class FreeTextSearchFilterQuotesCommandHandler(IQuoteManager<Quote> manager) : ICommandHandler<FreeTextSearchFilterQuotesCommand, PaginatedResult<QuoteItem>>
{
    public async Task<AppResult<PaginatedResult<QuoteItem>>> Handle(FreeTextSearchFilterQuotesCommand request, CancellationToken cancellationToken)
    {
        var items = await manager.DeferredFreeTextSearchFilterQuotesByText(request.SearchTerm?.Trim(),
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

        var count = await manager.CountFreeTextSearchFilteredQuotesByTextAsync(request.SearchTerm?.Trim(), request.CreatedAtFrom, request.CreatedAtTo);

        return new PaginatedResult<QuoteItem>(items, items.Count, count, request.Page, request.PageSize);
    }
}