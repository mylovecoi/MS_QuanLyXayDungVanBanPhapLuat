using System.Security.Claims;
using BuildingBlocks.Abstractions;
namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Identity;
public sealed class HttpCurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    private string? Read(string name) => accessor.HttpContext?.User.FindFirstValue(name);
    public Guid? UserId => Guid.TryParse(Read("UserId"), out var id) ? id : null;
    public string? Username => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
    public Guid? DonViId => Guid.TryParse(Read("DonViId"), out var id) ? id : null;
    public Guid? GroupPermissionId => Guid.TryParse(Read("GroupPermissionId"), out var id) ? id : null;
    public bool IsSSA => bool.TryParse(Read("IsSSA"), out var value) && value;
    public bool IsAuthenticated => accessor.HttpContext?.User.Identity?.IsAuthenticated == true;
}
