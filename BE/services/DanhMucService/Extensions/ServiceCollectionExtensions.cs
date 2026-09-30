using DanhMucService.Application.Abstractions;
using BuildingBlocks.Configuration;
using DanhMucService.Application.Features.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDanhMucModules(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = RuntimeConnectionStringResolver.GetRequiredConnectionString(configuration, "DanhMucService");
        services.AddDbContext<DanhMucDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IDanhMucTrangThaiRepository, DanhMucTrangThaiRepository>();
        services.AddScoped<IDanhMucTrangThaiAppService, DanhMucTrangThaiAppService>();
        services.AddScoped<IDanhMucVanBanRepository, DanhMucVanBanRepository>();
        services.AddScoped<IDanhMucVanBanAppService, DanhMucVanBanAppService>();
        services.AddScoped<IDanhMucDonViRepository, DanhMucDonViRepository>();
        services.AddScoped<IDanhMucDonViAppService, DanhMucDonViAppService>();
        services.AddScoped<IDanhMucPhongBanRepository, DanhMucPhongBanRepository>();
        services.AddScoped<IDanhMucPhongBanAppService, DanhMucPhongBanAppService>();
        services.AddScoped<IDanhMucDiaDanhRepository, DanhMucDiaDanhRepository>();
        services.AddScoped<IDanhMucDiaDanhAppService, DanhMucDiaDanhAppService>();
        services.AddScoped<IDanhMucLinhVucRepository, DanhMucLinhVucRepository>();
        services.AddScoped<IDanhMucLinhVucAppService, DanhMucLinhVucAppService>();
        services.AddScoped<IDanhMucCanBoRepository, DanhMucCanBoRepository>();
        services.AddScoped<IDanhMucCanBoAppService, DanhMucCanBoAppService>();
        services.AddScoped<IDanhMucTieuChiDiemRepository, DanhMucTieuChiDiemRepository>();
        services.AddScoped<IDanhMucTieuChiDiemAppService, DanhMucTieuChiDiemAppService>();
        services.AddScoped<IDanhMucQuyTrinhSoanThaoRepository, DanhMucQuyTrinhSoanThaoRepository>();
        services.AddScoped<IDanhMucQuyTrinhSoanThaoAppService, DanhMucQuyTrinhSoanThaoAppService>();

        return services;
    }
}

