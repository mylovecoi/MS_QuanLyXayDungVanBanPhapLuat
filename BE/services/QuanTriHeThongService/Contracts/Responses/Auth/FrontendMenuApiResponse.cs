namespace QuanTriHeThongService.Contracts.Responses.Auth;

public class FrontendMenuApiResponse
{
    public List<FrontendMenuItemApiResponse> Items { get; set; } = new();
}

public class FrontendMenuItemApiResponse
{
    public Guid RoleActionId { get; set; }
    public Guid? ParentRoleActionId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? PhanLoai { get; set; }
    public int Level { get; set; }
    public int STTSapXep { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public string? Parameter { get; set; }
    public string? Icon { get; set; }
    public string? ClientApp { get; set; }
    public List<FrontendMenuItemApiResponse> Children { get; set; } = new();
}

