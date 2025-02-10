using FluentValidation;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.Application.Quotes.Filter.NormalText;

internal class NormalFilterQuotesCommandValidator : AbstractValidator<NormalFilterQuotesCommand>
{
    public NormalFilterQuotesCommandValidator()
    {
        Include(new FilterQuotesCommandValidator());
    }
}