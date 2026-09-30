using QuanTriHeThongService.Domain.Common;

namespace QuanTriHeThongService.Domain.Entities.Systems;

public class GroupPermissionEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Kích hoạt";
}

