using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Quotes.Common;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Get.ById;

internal class GetQuoteByIdCommandHandler(IQuoteManager<Quote> manager) : ICommandHandler<GetQuoteByIdCommand, QuoteItem>
{
    public async Task<AppResult<QuoteItem>> Handle(GetQuoteByIdCommand request, CancellationToken cancellationToken)
    {
        var quote = await manager.GetByIdAsync(request.Id);
        if (quote is null)
        {
            return AppResult<QuoteItem>.Failure(new AppError("QuoteNotFound",
                "Quote not found."));
        }

        var id = await manager.GetIdAsync(quote);
        var text = await manager.GetTextAsync(quote);
        var author = await manager.GetAuthorAsync(quote);
        var createdAt = await manager.GetCreatedAtAsync(quote);

        return AppResult<QuoteItem>
            .Success(new QuoteItem
            {
                CreatedAt = createdAt,
                Id = id,
                Text = text,
                Author = author
            });
    }
}