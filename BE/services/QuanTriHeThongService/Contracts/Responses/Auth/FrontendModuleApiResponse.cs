namespace QuanTriHeThongService.Contracts.Responses.Auth;

public class FrontendModuleApiResponse
{
    public List<FrontendModuleItemApiResponse> Items { get; set; } = [];
}

public class FrontendModuleItemApiResponse
{
    public string Key { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public bool IsVisibleInMenu { get; set; }
    public string? ClientApp { get; set; }
    public bool CanAccess { get; set; }
    public bool Index { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Approve { get; set; }
    public bool Public { get; set; }
}

