using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Persistence.Specifications;

public class SoundexFilterQuotesByAuthorPaginatedSpecification<TQuote> : PaginationSpecification<TQuote> where TQuote : Quote
{
    public SoundexFilterQuotesByAuthorPaginatedSpecification(string? normalizedSearchTerm,
        int page, int pageSize,
        DateTime? from, DateTime? to,
        string OrderBy,
        bool IsDescending)
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

        ApplyPagination((page - 1) * pageSize, pageSize);

        switch (OrderBy)
        {
            case Constants.Quotes.OrderByText:
                if (IsDescending)
                {
                    AddOrderByDescending(x => x.Text);
                    break;
                }
                AddOrderBy(x => x.Text);
                break;

            case Constants.Quotes.OrderByAuthor:
                if (IsDescending)
                {
                    AddOrderByDescending(x => x.Author);
                    break;
                }
                AddOrderBy(x => x.Author);
                break;

            default:
                if (IsDescending)
                {
                    AddOrderByDescending(x => x.CreatedAt);
                    break;
                }
                AddOrderBy(x => x.CreatedAt);
                break;
        }
    }
}