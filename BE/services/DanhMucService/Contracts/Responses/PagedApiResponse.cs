namespace DanhMucService.Contracts.Responses;

public class PagedApiResponse<T> : ApiResponse<IReadOnlyList<T>>
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageCurrent { get; set; }
}

