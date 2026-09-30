using QuanTriHeThongService.Domain.Common;

namespace QuanTriHeThongService.Domain.Entities.Systems;

public class PermissionEntity : BaseEntity
{
    public Guid GroupPermissionId { get; set; }
    public Guid RoleActionId { get; set; }
    public string? Status { get; set; }
    public bool Index { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Approve { get; set; }
    public bool Public { get; set; }
    public string? PhanLoai { get; set; }
    public Guid? RoleActionGroupId { get; set; }
    public int Level { get; set; }
    public int STTSapXep { get; set; }
    public string? Title { get; set; }
    public string? Role { get; set; }
    public string? Controller { get; set; }
    public string? ActionName { get; set; }
    public string? Table { get; set; }
    public string? Icon { get; set; }
}

