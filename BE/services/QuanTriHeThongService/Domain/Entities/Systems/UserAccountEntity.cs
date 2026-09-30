using QuanTriHeThongService.Domain.Common;

namespace QuanTriHeThongService.Domain.Entities.Systems;

public class UserAccountEntity : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Kích hoạt";
    public string? Content { get; set; } = "Fixted";
    public Guid GroupPermissionId { get; set; }
    public string? GroupPermissionName { get; set; }
    public bool FirstLogin { get; set; }
    public string? Level { get; set; } = "Nhà nước";
}

