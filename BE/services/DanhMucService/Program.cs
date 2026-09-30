using DanhMucService.Infrastructure.Security;
using DanhMucService.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
}

builder.Services.AddControllers();
builder.Services.AddInternalApiKeyProtection(builder.Configuration);
builder.Services.AddDanhMucModules(builder.Configuration);

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

