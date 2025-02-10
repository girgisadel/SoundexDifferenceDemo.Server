using FluentValidation;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.Application.Quotes.Filter.FreeTextSearch;

internal class FreeTextSearchFilterQuotesCommandValidator : AbstractValidator<FreeTextSearchFilterQuotesCommand>
{
    public FreeTextSearchFilterQuotesCommandValidator()
    {
        Include(new FilterQuotesCommandValidator());
    }
}