using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace ThiHanhPhapLuatService.Infrastructure.Persistence;

public sealed class ThiHanhPhapLuatDbContext(DbContextOptions<ThiHanhPhapLuatDbContext> options) : DbContext(options)
{
    public DbSet<KeHoachThiHanhPhapLuat> KeHoachThiHanhPhapLuats => Set<KeHoachThiHanhPhapLuat>();
    public DbSet<KeHoachCanCuPhapLy> KeHoachCanCuPhapLys => Set<KeHoachCanCuPhapLy>();
    public DbSet<NoiDungKeHoach> NoiDungKeHoachs => Set<NoiDungKeHoach>();
    public DbSet<PhanCongThiHanh> PhanCongThiHanhs => Set<PhanCongThiHanh>();
    public DbSet<BaoCaoTienDoThiHanh> BaoCaoTienDoThiHanhs => Set<BaoCaoTienDoThiHanh>();
    public DbSet<TepDinhKemThiHanh> TepDinhKemThiHanhs => Set<TepDinhKemThiHanh>();
    public DbSet<DanhGiaThiHanh> DanhGiaThiHanhs => Set<DanhGiaThiHanh>();
    public DbSet<YeuCauBoSungThiHanh> YeuCauBoSungThiHanhs => Set<YeuCauBoSungThiHanh>();
    public DbSet<BaoCaoTongHopThiHanh> BaoCaoTongHopThiHanhs => Set<BaoCaoTongHopThiHanh>();
    public DbSet<BaoCaoTongHopChiTiet> BaoCaoTongHopChiTiets => Set<BaoCaoTongHopChiTiet>();
    public DbSet<LichSuXuLyThiHanh> LichSuXuLyThiHanhs => Set<LichSuXuLyThiHanh>();
    public DbSet<LichSuNhacViecThiHanh> LichSuNhacViecThiHanhs => Set<LichSuNhacViecThiHanh>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("thpl");
        modelBuilder.Entity<KeHoachThiHanhPhapLuat>(entity =>
        {
            entity.HasIndex(x => x.MaKeHoach).IsUnique();
            entity.HasIndex(x => new { x.DonViChuTriId, x.TrangThaiId, x.Nam });
            entity.Property(x => x.MaKeHoach).HasMaxLength(50);
            entity.Property(x => x.TenKeHoach).HasMaxLength(500);
            entity.HasMany(x => x.NoiDungs).WithOne(x => x.KeHoach).HasForeignKey(x => x.KeHoachId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<NoiDungKeHoach>(entity =>
        {
            entity.HasIndex(x => new { x.KeHoachId, x.MaNoiDung }).IsUnique();
            entity.HasIndex(x => new { x.KeHoachId, x.TrangThaiId, x.HanHoanThanh });
            entity.Property(x => x.TyLeHoanThanh).HasPrecision(5, 2);
            entity.Property(x => x.ChiTieu).HasPrecision(18, 2);
        });
        modelBuilder.Entity<BaoCaoTienDoThiHanh>(entity =>
        {
            entity.HasIndex(x => new { x.NoiDungKeHoachId, x.KyBaoCao, x.CreatedAt });
            entity.Property(x => x.TyLeHoanThanh).HasPrecision(5, 2);
        });
        modelBuilder.Entity<TepDinhKemThiHanh>(entity =>
        {
            entity.HasIndex(x => new { x.KeHoachId, x.LoaiTaiLieu, x.PhienBan });
            entity.HasIndex(x => new { x.NoiDungKeHoachId, x.BaoCaoTienDoThiHanhId, x.IsCurrent });
            entity.Property(x => x.LoaiTaiLieu).HasMaxLength(100);
            entity.Property(x => x.TenTep).HasMaxLength(500);
            entity.Property(x => x.DuongDan).HasMaxLength(2000);
        });
        modelBuilder.Entity<BaoCaoTongHopThiHanh>(entity =>
        {
            entity.HasIndex(x => x.MaBaoCao).IsUnique();
            entity.HasIndex(x => new { x.KeHoachId, x.KyBaoCao }).IsUnique();
        });
        modelBuilder.Entity<BaoCaoTongHopChiTiet>(entity =>
        {
            entity.HasIndex(x => new { x.BaoCaoTongHopThiHanhId, x.NoiDungKeHoachId, x.LanChot }).IsUnique();
            entity.Property(x => x.TyLeHoanThanh).HasPrecision(5, 2);
        });
        modelBuilder.Entity<LichSuXuLyThiHanh>().HasIndex(x => new { x.KeHoachId, x.ThoiGianXuLy });
        modelBuilder.Entity<LichSuNhacViecThiHanh>().HasIndex(x => new { x.NoiDungKeHoachId, x.LoaiNhacViec, x.ThoiGianGui });
    }
}
