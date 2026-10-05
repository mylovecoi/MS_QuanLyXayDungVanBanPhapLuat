using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DanhMucService.Infrastructure.Security;

public sealed class InternalApiKeyMiddleware(
    RequestDelegate next,
    IOptions<InternalApiOptions> options)
{
    private const string HeaderName = "X-Internal-Api-Key";
    private readonly RequestDelegate _next = next;
    private readonly InternalApiOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldBypass(context.Request.Path))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { isSuccess = false, message = "Missing InternalApi:ApiKey configuration." }));
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var apiKeyHeader) || string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { isSuccess = false, message = "Missing internal API key." }));
            return;
        }

        if (!string.Equals(apiKeyHeader.ToString(), _options.ApiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { isSuccess = false, message = "Invalid internal API key." }));
            return;
        }

        await _next(context);
    }

    private static bool ShouldBypass(PathString path)
    {
        if (path.StartsWithSegments("/api/health", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (path.StartsWithSegments("/api/auth/login", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}

