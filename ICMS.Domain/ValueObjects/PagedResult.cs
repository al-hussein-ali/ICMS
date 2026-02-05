namespace ICMS.Domain.ValueObjects;

public class PagedResult<T> where T : class 
{
    public IReadOnlyList<T> Items { get; }
   
    public int TotalCount { get;}

    public int PageNumber { get; }
    public int PageSize { get; }

    public int TotalPages => (int)Math.Ceiling((decimal)(TotalCount / PageSize));


    public PagedResult(IReadOnlyList<T> items,int totalCount,int PageNumber,int PageSize)
    {
        this.Items = items;
        this.TotalCount = totalCount;
        this.PageNumber = PageNumber;
        this.PageSize = PageSize;
    }
}
