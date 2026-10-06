using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;

public sealed class KhaoSatThiHanhPhapLuatDbContext(DbContextOptions<KhaoSatThiHanhPhapLuatDbContext> options) : DbContext(options)
{
    public DbSet<CuocKhaoSat> CuocKhaoSats => Set<CuocKhaoSat>();
    public DbSet<NhomDoiTuongKhaoSat> NhomDoiTuongKhaoSats => Set<NhomDoiTuongKhaoSat>();
    public DbSet<MauPhieuKhaoSat> MauPhieuKhaoSats => Set<MauPhieuKhaoSat>();
    public DbSet<CauHoiThongKe> CauHoiThongKes => Set<CauHoiThongKe>();
    public DbSet<CauHoiMauPhieu> CauHoiMauPhieus => Set<CauHoiMauPhieu>();
    public DbSet<LuaChonTraLoi> LuaChonTraLois => Set<LuaChonTraLoi>();
    public DbSet<DoiTuongKhaoSat> DoiTuongKhaoSats => Set<DoiTuongKhaoSat>();
    public DbSet<PhieuNopKhaoSat> PhieuNopKhaoSats => Set<PhieuNopKhaoSat>();
    public DbSet<CauTraLoiKhaoSat> CauTraLoiKhaoSats => Set<CauTraLoiKhaoSat>();
    public DbSet<LoiImportKhaoSat> LoiImportKhaoSats => Set<LoiImportKhaoSat>();
    public DbSet<LichSuXuLyKhaoSat> LichSuXuLyKhaoSats => Set<LichSuXuLyKhaoSat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("kspl");
        modelBuilder.Entity<CuocKhaoSat>(x => { x.HasIndex(e => e.MaCuocKhaoSat).IsUnique(); x.HasIndex(e => new { e.DonViChuTriId, e.TrangThaiId, e.CreatedAt }); x.Property(e => e.MaCuocKhaoSat).HasMaxLength(50); x.Property(e => e.TenCuocKhaoSat).HasMaxLength(500); });
        modelBuilder.Entity<NhomDoiTuongKhaoSat>(x => x.HasIndex(e => new { e.CuocKhaoSatId, e.MaNhom }).IsUnique());
        modelBuilder.Entity<MauPhieuKhaoSat>(x => { x.HasIndex(e => new { e.NhomDoiTuongKhaoSatId, e.PhienBan }).IsUnique(); x.Property(e => e.MaHash).HasMaxLength(64); });
        modelBuilder.Entity<CauHoiThongKe>(x => x.HasIndex(e => new { e.CuocKhaoSatId, e.MaCauHoiThongKe }).IsUnique());
        modelBuilder.Entity<CauHoiMauPhieu>(x => x.HasIndex(e => new { e.MauPhieuKhaoSatId, e.MaCauHoi }).IsUnique());
        modelBuilder.Entity<LuaChonTraLoi>(x => x.HasIndex(e => new { e.CauHoiMauPhieuId, e.MaLuaChon }).IsUnique());
        modelBuilder.Entity<DoiTuongKhaoSat>(x => x.HasIndex(e => new { e.CuocKhaoSatId, e.DonViId }).IsUnique());
        modelBuilder.Entity<PhieuNopKhaoSat>(x => x.HasIndex(e => new { e.DoiTuongKhaoSatId, e.CreatedAt }));
        modelBuilder.Entity<CauTraLoiKhaoSat>(x => { x.Property(e => e.GiaTriSo).HasPrecision(18, 2); x.Property(e => e.SoLuong).HasPrecision(18, 2); x.Property(e => e.TongSoTraLoi).HasPrecision(18, 2); });
        modelBuilder.Entity<LichSuXuLyKhaoSat>(x => x.HasIndex(e => new { e.CuocKhaoSatId, e.ThoiGianXuLy }));
    }
}
