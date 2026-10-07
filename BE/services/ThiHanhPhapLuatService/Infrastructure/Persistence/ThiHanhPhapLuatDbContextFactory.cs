using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ThiHanhPhapLuatService.Infrastructure.Persistence;

public sealed class ThiHanhPhapLuatDbContextFactory : IDesignTimeDbContextFactory<ThiHanhPhapLuatDbContext>
{
    public ThiHanhPhapLuatDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: true).AddEnvironmentVariables().Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=ThiHanhPhapLuat_DesignTime;Trusted_Connection=True;TrustServerCertificate=True;";
        }
        return new ThiHanhPhapLuatDbContext(new DbContextOptionsBuilder<ThiHanhPhapLuatDbContext>().UseSqlServer(connectionString).Options);
    }
}
