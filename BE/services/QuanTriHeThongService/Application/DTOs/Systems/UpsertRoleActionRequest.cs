namespace QuanTriHeThongService.Application.DTOs.Systems;

public class UpsertRoleActionRequest
{
    public Guid? Id { get; set; }
    public int STTSapXep { get; set; }
    public string PhanLoai { get; set; } = "Group";
    public string Role { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? Title { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public string? Parameter { get; set; }
    public string? Table { get; set; }
    public string Status { get; set; } = "Kích hoạt";
    public string? UseGroup { get; set; }
    public string? FrontendPath { get; set; }
    public bool IsVisibleInMenu { get; set; } = true;
    public string? ClientApp { get; set; }
    public string? MenuTitle { get; set; }
    public string? MenuIcon { get; set; }
    public string? Icon { get; set; }
}

