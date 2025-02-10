namespace SoundexDifferenceDemo.SharedKernel;

public class PaginatedResult<T>(IReadOnlyList<T> items, long pageTotalCount, long totalCount, int page, int pageSize)
{
    public IReadOnlyList<T> Items { get; } = items;
    public long TotalCount { get; } = totalCount;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public long PageTotalCount { get; } = pageTotalCount;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
