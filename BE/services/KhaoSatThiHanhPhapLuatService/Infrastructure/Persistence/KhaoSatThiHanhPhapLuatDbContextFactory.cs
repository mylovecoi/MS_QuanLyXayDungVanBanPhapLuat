using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;

public sealed class KhaoSatThiHanhPhapLuatDbContextFactory : IDesignTimeDbContextFactory<KhaoSatThiHanhPhapLuatDbContext>
{
    public KhaoSatThiHanhPhapLuatDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        return new(new DbContextOptionsBuilder<KhaoSatThiHanhPhapLuatDbContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options);
    }
}
