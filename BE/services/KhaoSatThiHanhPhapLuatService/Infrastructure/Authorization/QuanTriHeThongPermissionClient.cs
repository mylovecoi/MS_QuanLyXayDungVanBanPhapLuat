using System.Net.Http.Json;
using System.Text.Json;
using BuildingBlocks.Abstractions;
using Microsoft.Net.Http.Headers;

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Authorization;

public interface IQuanTriHeThongPermissionClient { Task<bool> HasPermissionAsync(string controller, string action, string permissionType, CancellationToken ct); }
public sealed class QuanTriHeThongPermissionClient(HttpClient client, IHttpContextAccessor accessor, ICurrentUserContext user) : IQuanTriHeThongPermissionClient
{
    public async Task<bool> HasPermissionAsync(string controller, string action, string permissionType, CancellationToken ct)
    {
        if (user.IsSSA) return true;
        if (user.UserId is null) return false;
        var token = accessor.HttpContext?.Request.Headers.Authorization.ToString(); if (string.IsNullOrWhiteSpace(token)) return false;
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/internal/permissions/check") { Content = JsonContent.Create(new { Controller = controller, Action = action, PermissionType = permissionType }) };
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, token);
        try { using var response = await client.SendAsync(request, ct); var payload = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ct); return response.IsSuccessStatusCode && payload?.Data?.Allowed == true; }
        catch (HttpRequestException) { return false; } catch (JsonException) { return false; }
    }
    private sealed class Result { public Data? Data { get; init; } } private sealed class Data { public bool Allowed { get; init; } }
}
