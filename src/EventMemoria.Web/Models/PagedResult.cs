namespace EventMemoria.Web.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    private int PageSize { get; set; }
    private int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public PagedResult() { }

    public PagedResult(List<T> items, int currentPage, int pageSize, int totalCount)
    {
        Items = items;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
