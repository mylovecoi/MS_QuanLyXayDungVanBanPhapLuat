using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XayDungVanBanService.Infrastructure.Persistence;

public sealed class XayDungVanBanDbContextFactory : IDesignTimeDbContextFactory<XayDungVanBanDbContext>
{
    public XayDungVanBanDbContext CreateDbContext(string[] args)
    {
        const string connectionString = "Server=.;Database=CSDLXayDungVanBan_MS;Trusted_Connection=True;TrustServerCertificate=True";
        return new XayDungVanBanDbContext(new DbContextOptionsBuilder<XayDungVanBanDbContext>()
            .UseSqlServer(connectionString)
            .Options);
    }
}
