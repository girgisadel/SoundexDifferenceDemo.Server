using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Specifications;

public class AuthorSoundexFilterQuotesSpecification<TQuote> : Specification<TQuote> where TQuote : Quote
{
    public AuthorSoundexFilterQuotesSpecification(string? normalizedSearchTerm, DateTime? from, DateTime? to)
    {
        if (!string.IsNullOrEmpty(normalizedSearchTerm))
        {
            AddCriterion(x =>
            !string.IsNullOrEmpty(x.NormalizedAuthor) &&
            InternalDbFunctionsExtensions.Soundex(x.NormalizedAuthor) == InternalDbFunctionsExtensions.Soundex(normalizedSearchTerm));
        }

        if (from.HasValue)
        {
            AddCriterion(x => x.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            AddCriterion(x => x.CreatedAt <= to.Value);
        }
    }
}