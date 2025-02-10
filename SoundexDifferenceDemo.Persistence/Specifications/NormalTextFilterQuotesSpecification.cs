using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Specifications;

public class NormalTextFilterQuotesSpecification<TQuote> : Specification<TQuote> where TQuote : Quote
{
    public NormalTextFilterQuotesSpecification(string? searchTerm, DateTime? from, DateTime? to)
    {
        if (!string.IsNullOrEmpty(searchTerm))
        {
            AddCriterion(x => !string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(x.Text) && x.Text.Contains(searchTerm));
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
