using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Specifications;

public class GetAuthorsSoundexSpecification<TQuote> : Specification<TQuote> where TQuote : Quote
{
    public GetAuthorsSoundexSpecification(string normalizedSearchTerm)
    {
        AddCriterion(x => !string.IsNullOrEmpty(x.NormalizedAuthor) &&
        InternalDbFunctionsExtensions.Soundex(x.NormalizedAuthor) == InternalDbFunctionsExtensions.Soundex(normalizedSearchTerm));
    }
}