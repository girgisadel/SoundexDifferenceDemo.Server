using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Abstractions.Repositories;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Update;

internal class UpdateQuoteCommandHandler(IQuoteManager<Quote> manager, IQuoteRepository<Quote> repository) : ICommandHandler<UpdateQuoteCommand, QuoteIdItem>
{
    public async Task<AppResult<QuoteIdItem>> Handle(UpdateQuoteCommand request, CancellationToken cancellationToken)
    {
        var quote = await manager.FindByIdAsync(request.Id);
        if (quote is null)
        {
            return AppResult<QuoteIdItem>.Failure(new AppError("QuoteNotFound", 
                "Quote not found."));
        }

        var quoteText = await manager.GetTextAsync(quote).ConfigureAwait(false);
        var quoteAuthor = await manager.GetAuthorAsync(quote).ConfigureAwait(false);

        var updateFlag = false;
        if (!string.IsNullOrEmpty(request.Text) && !request.Text.Equals(quoteText))
        {
            updateFlag = true;
            await repository.SetTextAsync(quote, request.Text, cancellationToken).ConfigureAwait(false);
        }
        if (!string.IsNullOrEmpty(request.Author) && !request.Author.Equals(quoteAuthor))
        {
            updateFlag = true;
            await repository.SetAuthorAsync(quote, request.Author, cancellationToken).ConfigureAwait(false);
        }

        var quoteId = await manager.GetIdAsync(quote).ConfigureAwait(false);

        if (!updateFlag)
        {
            return AppResult<QuoteIdItem>
                .Failure(new AppError("NoChangesDetected", "No updates were made."));
        }

        quote.Raise(new UpdateQuoteDomainEvent(quoteId));
        var result = await manager.UpdateAsync(quote);
        return result.Succeeded
            ? AppResult<QuoteIdItem>.Success(new QuoteIdItem { Id = quoteId })
            : AppResult<QuoteIdItem>.Failure([.. result.Errors]);
    }
}