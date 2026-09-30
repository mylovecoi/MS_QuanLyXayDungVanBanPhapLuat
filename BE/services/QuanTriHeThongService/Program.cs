using QuanTriHeThongService.Infrastructure.Security;
using QuanTriHeThongService.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
}

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInternalApiKeyProtection(builder.Configuration);
builder.Services.AddQuanTriHeThongModules(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseInternalApiKeyProtection();
app.UseAuthorization();
app.MapControllers();

app.Run();

