namespace DanhMucService.Contracts.Responses;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

