using SoundexDifferenceDemo.Application.Quotes.Filter.AuthorSoundex;
using SoundexDifferenceDemo.Application.Quotes.Filter.FreeTextSearch;
using SoundexDifferenceDemo.Application.Quotes.Filter.NormalText;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.WebApi.DTOs.Requests;

public class FilterRequest : PaginationRequest
{
    public string? SearchTerm { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string OrderBy { get; set; } = Constants.Quotes.OrderByCreatedAt;
    public bool IsDescending { get; set; }

    public NormalFilterQuotesCommand AsNormalFilterQuotesCommand()
    {
        return new NormalFilterQuotesCommand
        {
            CreatedAtFrom = CreatedAtFrom,
            CreatedAtTo = CreatedAtTo,
            IsDescending = IsDescending,
            OrderBy = OrderBy,
            Page = Page,
            PageSize = PageSize,
            SearchTerm = SearchTerm
        };
    }

    public SoundexFilterQuotesCommand AsSoundexFilterQuotesCommand()
    {
        return new SoundexFilterQuotesCommand
        {
            CreatedAtFrom = CreatedAtFrom,
            CreatedAtTo = CreatedAtTo,
            IsDescending = IsDescending,
            OrderBy = OrderBy,
            Page = Page,
            PageSize = PageSize,
            SearchTerm = SearchTerm
        };
    }

    public FreeTextSearchFilterQuotesCommand AsFreeTextSearchFilterQuotesCommand()
    {
        return new FreeTextSearchFilterQuotesCommand
        {
            CreatedAtFrom = CreatedAtFrom,
            CreatedAtTo = CreatedAtTo,
            IsDescending = IsDescending,
            OrderBy = OrderBy,
            Page = Page,
            PageSize = PageSize,
            SearchTerm = SearchTerm
        };
    }
}