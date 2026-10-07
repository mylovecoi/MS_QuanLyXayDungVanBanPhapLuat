using BuildingBlocks.Security;
using KhaoSatThiHanhPhapLuatService.Application;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<KhaoSatThiHanhPhapLuatDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IWordSurveyParser, WordSurveyParser>();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
