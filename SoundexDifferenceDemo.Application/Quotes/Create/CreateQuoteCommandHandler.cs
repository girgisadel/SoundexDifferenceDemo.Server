using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Abstractions.Repositories;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Create;

internal class CreateQuoteCommandHandler(IQuoteManager<Quote> manager, IQuoteRepository<Quote> repository) : ICommandHandler<CreateQuoteCommand, QuoteIdItem>
{
    public async Task<AppResult<QuoteIdItem>> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
    {
        var quote = new Quote();
        await repository.SetTextAsync(quote, request.Text, cancellationToken).ConfigureAwait(false);
        await repository.SetAuthorAsync(quote, request.Author, cancellationToken).ConfigureAwait(false);
        var quoteId = await manager.GetIdAsync(quote).ConfigureAwait(false);
        quote.Raise(new CreateQuoteDomainEvent(quoteId));
        var result = await manager.CreateAsync(quote);
        return result.Succeeded
            ? AppResult<QuoteIdItem>.Success(new QuoteIdItem { Id = quoteId })
            : AppResult<QuoteIdItem>.Failure([.. result.Errors]);
    }
}
