using BuildingBlocks.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KhaiThacDuLieuService.Infrastructure.Persistence;

public sealed class KhaiThacDuLieuDbContextFactory : IDesignTimeDbContextFactory<KhaiThacDuLieuDbContext>
{
    public KhaiThacDuLieuDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<KhaiThacDuLieuDbContext>()
            .UseSqlServer(RuntimeConnectionStringResolver.GetRequiredConnectionString(configuration, "KhaiThacDuLieuService"))
            .Options;

        return new KhaiThacDuLieuDbContext(options);
    }
}
