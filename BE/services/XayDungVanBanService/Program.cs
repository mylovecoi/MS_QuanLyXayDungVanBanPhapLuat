using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Identity;
using BuildingBlocks.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<XayDungVanBanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();

var app = builder.Build();
app.MapControllers();
app.Run();
