using BuildingBlocks.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BuildingBlocks.Abstractions;
using Microsoft.Extensions.Options;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.Identity;
using ThiHanhPhapLuatService.Infrastructure.DanhMuc;
using ThiHanhPhapLuatService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ThiHanhPhapLuatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = []
    });
});
builder.Services.AddApplicationJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
builder.Services.Configure<QuanTriHeThongPermissionOptions>(builder.Configuration.GetSection(QuanTriHeThongPermissionOptions.SectionName));
builder.Services.AddHttpClient<IQuanTriHeThongPermissionClient, QuanTriHeThongPermissionClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<QuanTriHeThongPermissionOptions>>().Value;
    if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
    {
        throw new InvalidOperationException("Thiếu cấu hình QuanTriHeThongService:BaseUrl hợp lệ.");
    }
    client.BaseAddress = baseUri;
});
builder.Services.AddHttpClient<IDanhMucTrangThaiClient, DanhMucTrangThaiClient>((_, client) =>
{
    var baseUrl = builder.Configuration["DanhMucService:BaseUrl"];
    if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri)) throw new InvalidOperationException("Thiếu cấu hình DanhMucService:BaseUrl hợp lệ.");
    client.BaseAddress = baseUri;
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
