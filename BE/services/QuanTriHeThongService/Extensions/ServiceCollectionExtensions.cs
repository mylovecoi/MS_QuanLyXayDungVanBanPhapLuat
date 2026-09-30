using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.Common.Interfaces;
using BuildingBlocks.Configuration;
using BuildingBlocks.Abstractions;
using QuanTriHeThongService.Application.Features.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;
using QuanTriHeThongService.Infrastructure.Identity;
using QuanTriHeThongService.Infrastructure.Persistence;
using QuanTriHeThongService.Infrastructure.Persistence.Repositories.Systems;
using Microsoft.EntityFrameworkCore;

namespace QuanTriHeThongService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuanTriHeThongModules(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = RuntimeConnectionStringResolver.GetRequiredConnectionString(configuration, "QuanTriHeThongService");
        services.AddDbContext<QuanTriHeThongDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
        services.AddScoped<IPermissionChecker, DatabasePermissionChecker>();

        services.AddScoped<IRoleActionRepository, RoleActionRepository>();
        services.AddScoped<IRoleActionAppService, RoleActionAppService>();
        services.AddScoped<IGroupPermissionRepository, GroupPermissionRepository>();
        services.AddScoped<IGroupPermissionAppService, GroupPermissionAppService>();
        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IUserAccountAppService, UserAccountAppService>();
        services.AddScoped<ISystemInfoRepository, SystemInfoRepository>();
        services.AddScoped<ISystemInfoAppService, SystemInfoAppService>();
        services.AddScoped<ILogEntryRepository, LogEntryRepository>();
        services.AddScoped<ILogEntryAppService, LogEntryAppService>();

        return services;
    }
}

