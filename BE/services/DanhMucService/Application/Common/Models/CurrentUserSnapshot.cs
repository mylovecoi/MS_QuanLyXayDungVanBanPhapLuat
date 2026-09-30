namespace DanhMucService.Application.Common.Models;

public sealed class CurrentUserSnapshot
{
    public Guid? UserId { get; init; }
    public string? Username { get; init; }
    public Guid? DonViId { get; init; }
    public Guid? GroupPermissionId { get; init; }
    public bool IsSSA { get; init; }
    public bool IsAuthenticated { get; init; }
}

