using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Specifications;

public class GetAuthorsByAuthorSoundexSpecification<TQuote> : Specification<TQuote> where TQuote : Quote
{
    public GetAuthorsByAuthorSoundexSpecification(string normalizedSearchTerm)
    {
        AddCriterion(x => !string.IsNullOrEmpty(x.NormalizedAuthor) &&
        InternalDbFunctionsExtensions.Soundex(x.NormalizedAuthor) == InternalDbFunctionsExtensions.Soundex(normalizedSearchTerm));
    }
}