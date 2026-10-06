using System.Net.Http.Json;
using System.Text.Json;
using BuildingBlocks.Abstractions;
using Microsoft.Net.Http.Headers;

namespace ThiHanhPhapLuatService.Infrastructure.Authorization;

public sealed class QuanTriHeThongPermissionClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor,
    ICurrentUserContext currentUserContext) : IQuanTriHeThongPermissionClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<bool> HasPermissionAsync(string controller, string action, string permissionType, CancellationToken cancellationToken = default)
    {
        if (currentUserContext.UserId is null || currentUserContext.UserId == Guid.Empty)
        {
            return false;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/internal/permissions/check")
        {
            Content = JsonContent.Create(new PermissionCheckRequest(controller, action, permissionType))
        };
        var authorization = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization)) return false;
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, authorization);

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode) return false;
            var payload = await response.Content.ReadFromJsonAsync<PermissionCheckEnvelope>(JsonOptions, cancellationToken);
            return payload?.IsSuccess == true && payload.Data?.Allowed == true;
        }
        catch (HttpRequestException) { return false; }
        catch (JsonException) { return false; }
    }

    private sealed record PermissionCheckRequest(string Controller, string Action, string PermissionType);
    private sealed class PermissionCheckEnvelope { public bool IsSuccess { get; init; } public PermissionCheckResult? Data { get; init; } }
    private sealed class PermissionCheckResult { public bool Allowed { get; init; } }
}
