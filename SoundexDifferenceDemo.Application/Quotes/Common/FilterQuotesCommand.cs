using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Common;

public abstract class FilterQuotesCommand : ICommand<PaginatedResult<QuoteItem>>
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = Constants.DefaultPage;
    public int PageSize { get; set; } = Constants.DefaultPageSize;
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string OrderBy { get; set; } = Constants.Quotes.OrderByCreatedAt;
    public bool IsDescending { get; set; }
}
