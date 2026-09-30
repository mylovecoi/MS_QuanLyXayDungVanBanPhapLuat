namespace QuanTriHeThongService.Application.DTOs.Systems;

public class GroupPermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Kích hoạt";
}

