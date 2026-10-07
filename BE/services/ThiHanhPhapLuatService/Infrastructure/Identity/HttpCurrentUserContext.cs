using System.Security.Claims;
using BuildingBlocks.Abstractions;

namespace ThiHanhPhapLuatService.Infrastructure.Identity;

public sealed class HttpCurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    private HttpContext? Context => accessor.HttpContext;
    public Guid? UserId => Guid.TryParse(Read("UserId"), out var value) ? value : null;
    public string? Username => Context?.User.FindFirstValue(ClaimTypes.Name);
    public Guid? DonViId => Guid.TryParse(Read("DonViId"), out var value) ? value : null;
    public Guid? GroupPermissionId => Guid.TryParse(Read("GroupPermissionId"), out var value) ? value : null;
    public bool IsSSA => bool.TryParse(Read("IsSSA"), out var value) && value;
    public bool IsAuthenticated => Context?.User.Identity?.IsAuthenticated == true || UserId.HasValue;
    private string? Read(string claim) => Context?.User.FindFirstValue(claim);
}
