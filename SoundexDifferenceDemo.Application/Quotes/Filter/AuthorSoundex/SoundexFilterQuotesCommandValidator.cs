using FluentValidation;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.Application.Quotes.Filter.AuthorSoundex;

internal class SoundexFilterQuotesCommandValidator : AbstractValidator<SoundexFilterQuotesCommand>
{
    public SoundexFilterQuotesCommandValidator()
    {
        Include(new FilterQuotesCommandValidator());
    }
}
