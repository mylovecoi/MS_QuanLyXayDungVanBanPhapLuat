namespace DanhMucService.Application.Common.Models;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageCurrent { get; set; }
}

