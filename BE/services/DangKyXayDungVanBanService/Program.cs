using BuildingBlocks.Abstractions;
using DangKyXayDungVanBanService.Application.Abstractions;
using DangKyXayDungVanBanService.Application.Services;
using DangKyXayDungVanBanService.Infrastructure.Identity;
using DangKyXayDungVanBanService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DangKyXayDungVanBanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
builder.Services.AddScoped<IDangKyXayDungVanBanAppService, DangKyXayDungVanBanAppService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultCors");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
