using System.Security.Claims;
using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace QuanTriHeThongService.Infrastructure.Identity;

public sealed class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId => ReadGuidClaim("UserId") ?? ReadGuidSession("Id");

    public string? Username =>
        ReadClaim(ClaimTypes.Name)
        ?? ReadSessionValue("Username")
        ?? ReadSessionValue("username");

    public Guid? DonViId => ReadGuidClaim("DonViId") ?? ReadGuidSession("DanhMucDonViId");

    public Guid? GroupPermissionId => ReadGuidClaim("GroupPermissionId") ?? ReadGuidSession("GroupPermissionId");

    public bool IsSSA => ReadBoolClaim("IsSSA") ?? ReadBoolSession("SSA") ?? false;

    public bool IsAuthenticated =>
        (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
        || UserId.HasValue;

    private string? ReadClaim(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }

    private Guid? ReadGuidClaim(string claimType)
    {
        return Guid.TryParse(ReadClaim(claimType), out var value) ? value : null;
    }

    private bool? ReadBoolClaim(string claimType)
    {
        return bool.TryParse(ReadClaim(claimType), out var value) ? value : null;
    }

    private string? ReadSessionValue(string key)
    {
        var session = _httpContextAccessor.HttpContext?.Features.Get<ISessionFeature>()?.Session;
        var payload = session?.GetString("SsAdmin");
        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        try
        {
            using var json = System.Text.Json.JsonDocument.Parse(payload);
            if (json.RootElement.TryGetProperty(key, out var property))
            {
                return property.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.String => property.GetString(),
                    System.Text.Json.JsonValueKind.Number => property.GetRawText(),
                    System.Text.Json.JsonValueKind.True => bool.TrueString,
                    System.Text.Json.JsonValueKind.False => bool.FalseString,
                    _ => property.GetRawText()
                };
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    private Guid? ReadGuidSession(string key)
    {
        return Guid.TryParse(ReadSessionValue(key), out var value) ? value : null;
    }

    private bool? ReadBoolSession(string key)
    {
        return bool.TryParse(ReadSessionValue(key), out var value) ? value : null;
    }

}

