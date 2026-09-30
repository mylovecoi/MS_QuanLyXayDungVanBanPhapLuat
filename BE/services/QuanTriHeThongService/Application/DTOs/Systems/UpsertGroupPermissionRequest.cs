namespace QuanTriHeThongService.Application.DTOs.Systems;

public class UpsertGroupPermissionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Kích hoạt";
    public string? TemplateGroup { get; set; }
}

