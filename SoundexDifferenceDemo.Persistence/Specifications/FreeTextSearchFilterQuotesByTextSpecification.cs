using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Specifications;

public class FreeTextSearchFilterQuotesByTextSpecification<TQuote> : Specification<TQuote> where TQuote : Quote
{
    public FreeTextSearchFilterQuotesByTextSpecification(string? searchTerm, DateTime? from, DateTime? to)
    {
        if (!string.IsNullOrEmpty(searchTerm))
        {
            AddCriterion(x => !string.IsNullOrEmpty(x.Text) && EF.Functions.FreeText(x.Text, searchTerm));
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