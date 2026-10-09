using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Identity;
using XayDungVanBanService.Infrastructure.Authorization;
using XayDungVanBanService.Infrastructure.DanhMuc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.Services;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Security;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<XayDungVanBanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập access token nhận được sau khi đăng nhập."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }] = []
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
builder.Services.AddScoped<IXayDungVanBanHoSoQueryService, XayDungVanBanHoSoQueryService>();
builder.Services.AddScoped<IXayDungVanBanSoanThaoService, XayDungVanBanSoanThaoService>();
builder.Services.AddScoped<IXayDungVanBanTrinhThamDinhService, XayDungVanBanTrinhThamDinhService>();
builder.Services.AddScoped<IXayDungVanBanThamDinhService, XayDungVanBanThamDinhService>();
builder.Services.AddScoped<IXayDungVanBanTrinhPheDuyetService, XayDungVanBanTrinhPheDuyetService>();
builder.Services.AddScoped<IXayDungVanBanYKienUbndService, XayDungVanBanYKienUbndService>();
builder.Services.AddScoped<IXayDungVanBanThamTraHdndService, XayDungVanBanThamTraHdndService>();
builder.Services.AddScoped<IXayDungVanBanBanHanhService, XayDungVanBanBanHanhService>();
builder.Services.AddScoped<IXayDungVanBanChamDiemService, XayDungVanBanChamDiemService>();
builder.Services.AddScoped<IXayDungVanBanTienDoService, XayDungVanBanTienDoService>();
builder.Services.Configure<QuanTriHeThongPermissionOptions>(
    builder.Configuration.GetSection(QuanTriHeThongPermissionOptions.SectionName));
builder.Services.AddHttpClient<IQuanTriHeThongPermissionClient, QuanTriHeThongPermissionClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<QuanTriHeThongPermissionOptions>>().Value;
    if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
    {
        throw new InvalidOperationException("Thiếu cấu hình QuanTriHeThongService:BaseUrl hợp lệ.");
    }

    client.BaseAddress = baseUri;
});
builder.Services.AddHttpClient<IDanhMucTrangThaiClient, DanhMucTrangThaiClient>((_, client) =>
{
    var baseUrl = builder.Configuration["DanhMucService:BaseUrl"];
    if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
    {
        throw new InvalidOperationException("Thiếu cấu hình DanhMucService:BaseUrl hợp lệ.");
    }

    client.BaseAddress = baseUri;
});
builder.Services.AddHttpClient<IDanhMucTieuChiDiemClient, DanhMucTieuChiDiemClient>((_, client) =>
{
    var baseUrl = builder.Configuration["DanhMucService:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
