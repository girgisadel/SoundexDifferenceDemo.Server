using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Delete;

internal class DeleteQuoteByIdCommandHandler(IQuoteManager<Quote> manager) : ICommandHandler<DeleteQuoteByIdCommand>
{
    public async Task<AppResult> Handle(DeleteQuoteByIdCommand request, CancellationToken cancellationToken)
    {
        var quote = await manager.FindByIdAsync(request.Id);
        if (quote is null)
        {
            return AppResult.Success();
        }
        var quoteId = await manager.GetIdAsync(quote).ConfigureAwait(false);
        quote.Raise(new DeleteQuoteDomainEvent(quoteId));
        return await manager.DeleteAsync(quote);
    }
}