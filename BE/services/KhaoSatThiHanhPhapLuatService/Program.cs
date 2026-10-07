using BuildingBlocks.Security;
using BuildingBlocks.Abstractions;
using KhaoSatThiHanhPhapLuatService.Application;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Authorization;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Identity;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<KhaoSatThiHanhPhapLuatDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers(options => options.Filters.Add<KhaoSatPermissionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = [] });
});
builder.Services.AddApplicationJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
builder.Services.AddScoped<KhaoSatPermissionFilter>();
builder.Services.AddHttpClient<IQuanTriHeThongPermissionClient, QuanTriHeThongPermissionClient>((_, client) => client.BaseAddress = new Uri(builder.Configuration["QuanTriHeThongService:BaseUrl"] ?? throw new InvalidOperationException("Thiếu QuanTriHeThongService:BaseUrl.")));
builder.Services.AddScoped<IWordSurveyParser, WordSurveyParser>();
builder.Services.AddScoped<IWordDocumentConverter, WordDocumentConverter>();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
