using System.Net.Http.Json;
using System.Text.Json;
using BuildingBlocks.Abstractions;

namespace XayDungVanBanService.Infrastructure.Authorization;

public sealed class QuanTriHeThongPermissionClient(
    HttpClient httpClient,
    ICurrentUserContext currentUserContext) : IQuanTriHeThongPermissionClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<bool> HasPermissionAsync(
        string controller,
        string action,
        string permissionType,
        CancellationToken cancellationToken = default)
    {
        if (currentUserContext.UserId is null || currentUserContext.UserId == Guid.Empty)
        {
            return false;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/internal/permissions/check")
        {
            Content = JsonContent.Create(new PermissionCheckRequest(controller, action, permissionType))
        };

        // QTH resolves group membership and SSA from its database; only user identity is propagated.
        request.Headers.Add("X-User-Id", currentUserContext.UserId.Value.ToString());

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var payload = await response.Content.ReadFromJsonAsync<PermissionCheckEnvelope>(JsonOptions, cancellationToken);
            return payload?.IsSuccess == true && payload.Data?.Allowed == true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private sealed record PermissionCheckRequest(string Controller, string Action, string PermissionType);

    private sealed class PermissionCheckEnvelope
    {
        public bool IsSuccess { get; init; }
        public PermissionCheckResult? Data { get; init; }
    }

    private sealed class PermissionCheckResult
    {
        public bool Allowed { get; init; }
    }
}
