using System.Security.Claims;
using BuildingBlocks.Abstractions;

namespace XayDungVanBanService.Infrastructure.Identity;

public sealed class HttpCurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    private HttpContext? Context => accessor.HttpContext;
    public Guid? UserId => Guid.TryParse(Read("UserId", "X-User-Id"), out var value) ? value : null;
    public string? Username => Context?.User.FindFirstValue(ClaimTypes.Name) ?? Context?.Request.Headers["X-Username"].ToString();
    public Guid? DonViId => Guid.TryParse(Read("DonViId", "X-Don-Vi-Id"), out var value) ? value : null;
    public Guid? GroupPermissionId => Guid.TryParse(Read("GroupPermissionId", "X-Group-Permission-Id"), out var value) ? value : null;
    public bool IsSSA => bool.TryParse(Read("IsSSA", "X-Is-SSA"), out var value) && value;
    public bool IsAuthenticated => Context?.User.Identity?.IsAuthenticated == true || UserId.HasValue;
    private string? Read(string claim, string header) => Context?.User.FindFirstValue(claim) ?? Context?.Request.Headers[header].ToString();
}
