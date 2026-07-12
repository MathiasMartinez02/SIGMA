namespace SIGMA.Application.Common.Pagination;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Data { get; init; } = [];
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);

    public static PaginatedResult<T> Create(IReadOnlyList<T> data, int total, int page, int pageSize) =>
        new() { Data = data, Total = total, Page = page, PageSize = pageSize };
}
