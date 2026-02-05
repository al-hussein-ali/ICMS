namespace ICMS.Application.DTOs.Pagination;

public class PaginationParams
{
    private const int _maxPageSize = 50;

    private int _pageSize = 10;

    public int PageNumber { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
    }

}
