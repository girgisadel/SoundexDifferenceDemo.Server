using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.WebApi.DTOs.Requests;

public class PaginationRequest
{
    public int Page { get; set; } = Constants.DefaultPage;
    public int PageSize { get; set; } = Constants.DefaultPageSize;
}
