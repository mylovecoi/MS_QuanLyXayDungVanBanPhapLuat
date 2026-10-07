using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;

public sealed class KhaoSatThiHanhPhapLuatDbContextFactory : IDesignTimeDbContextFactory<KhaoSatThiHanhPhapLuatDbContext>
{
    public KhaoSatThiHanhPhapLuatDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<KhaoSatThiHanhPhapLuatDbContext>().UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=KhaoSatThiHanhPhapLuat_DesignTime;Trusted_Connection=True;TrustServerCertificate=True;").Options);
}
