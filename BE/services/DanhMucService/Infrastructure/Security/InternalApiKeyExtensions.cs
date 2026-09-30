using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DanhMucService.Infrastructure.Security;

public static class InternalApiKeyExtensions
{
    public static IServiceCollection AddInternalApiKeyProtection(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InternalApiOptions>(options =>
        {
            options.ApiKey = configuration[$"{InternalApiOptions.SectionName}:ApiKey"] ?? string.Empty;
        });

        return services;
    }

    public static IApplicationBuilder UseInternalApiKeyProtection(this IApplicationBuilder app)
    {
        return app.UseMiddleware<InternalApiKeyMiddleware>();
    }
}

