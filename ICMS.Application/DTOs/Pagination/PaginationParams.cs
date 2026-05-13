namespace ICMS.Application.DTOs.Pagination;

public class PaginationParams
{
    private const int _maxPageSize = 200;
    private int _pageSize = 50;

    public int PageNumber { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = (value <= 0) ? 50 : (value > _maxPageSize ? _maxPageSize : value);
    }
    public string? Search { get; init; }

}
