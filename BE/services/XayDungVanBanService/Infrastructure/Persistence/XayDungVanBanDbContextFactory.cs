using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace XayDungVanBanService.Infrastructure.Persistence;

public sealed class XayDungVanBanDbContextFactory : IDesignTimeDbContextFactory<XayDungVanBanDbContext>
{
    public XayDungVanBanDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not configured.");

        return new XayDungVanBanDbContext(new DbContextOptionsBuilder<XayDungVanBanDbContext>()
            .UseSqlServer(connectionString)
            .Options);
    }
}
