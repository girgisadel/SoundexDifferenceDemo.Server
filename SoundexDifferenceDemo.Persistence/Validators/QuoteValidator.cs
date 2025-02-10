using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Validators;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Validators;

public class QuoteValidator<TQuote> : IQuoteValidator<TQuote> where TQuote : Quote
{
    public async Task<AppResult> ValidateAsync(IQuoteManager<TQuote> manager, TQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        var validateTextErrors = await ValidateTextAsync(manager, quote).ConfigureAwait(false);
        var validateAuthorErrors = await ValidateAuthorAsync(manager, quote).ConfigureAwait(false);

        List<AppError> errors = [];
        if (validateTextErrors != null && validateTextErrors.Count != 0)
        {
            errors.AddRange(validateTextErrors);
        }

        if (validateAuthorErrors != null && validateAuthorErrors.Count != 0)
        {
            errors.AddRange(validateAuthorErrors);
        }

        return errors?.Count > 0 ? AppResult.Failure([.. errors]) : AppResult.Success();
    }

    private async Task<List<AppError>?> ValidateTextAsync(IQuoteManager<TQuote> manager, TQuote quot)
    {
        List<AppError>? errors = null;
        var text = await manager.GetTextAsync(quot).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(text))
        {
            errors ??= [];
            errors.Add(new AppError("QuoteRequired", "Quote is required."));
        }
        else
        {
            var owner = await manager.JustFindByTextAsync(text).ConfigureAwait(false);
            if (owner != null &&
                !string.Equals(await manager.GetIdAsync(owner).ConfigureAwait(false),
                await manager.GetIdAsync(quot).ConfigureAwait(false)))
            {
                errors ??= [];
                errors.Add(new AppError("QuoteInUse", "This quote already exists."));
            }
        }
        return errors;
    }

    private async Task<List<AppError>?> ValidateAuthorAsync(IQuoteManager<TQuote> manager, TQuote quot)
    {
        List<AppError>? errors = null;
        var author = await manager.GetAuthorAsync(quot).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(author))
        {
            errors ??= [];
            errors.Add(new AppError("AuthorRequired", "Author is required."));
        }
        return errors;
    }
}
