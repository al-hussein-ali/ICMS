
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.DTOs.Pagination;

public static class QueryableExtentions
{
    public static PagedResult<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize) where T : class
    {

        var totalCount = query.Count();

        var items = query.Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();


        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}
