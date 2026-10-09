using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;

public sealed class KhaoSatThiHanhPhapLuatDbContext(DbContextOptions<KhaoSatThiHanhPhapLuatDbContext> options) : DbContext(options)
{
    public DbSet<CuocKhaoSat> CuocKhaoSats => Set<CuocKhaoSat>();
    public DbSet<NhomDoiTuongKhaoSat> NhomDoiTuongKhaoSats => Set<NhomDoiTuongKhaoSat>();
    public DbSet<MauPhieuKhaoSat> MauPhieuKhaoSats => Set<MauPhieuKhaoSat>();
    public DbSet<CauHoiThongKe> CauHoiThongKes => Set<CauHoiThongKe>();
    public DbSet<PhienDocMauPhieuKhaoSat> PhienDocMauPhieuKhaoSats => Set<PhienDocMauPhieuKhaoSat>();
    public DbSet<CauHoiNhapMauPhieuKhaoSat> CauHoiNhapMauPhieuKhaoSats => Set<CauHoiNhapMauPhieuKhaoSat>();
    public DbSet<CauHoiMauPhieu> CauHoiMauPhieus => Set<CauHoiMauPhieu>();
    public DbSet<LuaChonTraLoi> LuaChonTraLois => Set<LuaChonTraLoi>();
    public DbSet<DoiTuongKhaoSat> DoiTuongKhaoSats => Set<DoiTuongKhaoSat>();
    public DbSet<PhieuNopKhaoSat> PhieuNopKhaoSats => Set<PhieuNopKhaoSat>();
    public DbSet<CauTraLoiKhaoSat> CauTraLoiKhaoSats => Set<CauTraLoiKhaoSat>();
    public DbSet<LoiImportKhaoSat> LoiImportKhaoSats => Set<LoiImportKhaoSat>();
    public DbSet<BaoCaoKhaoSat> BaoCaoKhaoSats => Set<BaoCaoKhaoSat>();
    public DbSet<ChiTietBaoCaoKhaoSat> ChiTietBaoCaoKhaoSats => Set<ChiTietBaoCaoKhaoSat>();
    public DbSet<LichSuXuLyKhaoSat> LichSuXuLyKhaoSats => Set<LichSuXuLyKhaoSat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("kspl");
        modelBuilder.Entity<CuocKhaoSat>(x => { x.HasIndex(e => e.MaCuocKhaoSat).IsUnique(); x.HasIndex(e => new { e.DonViChuTriId, e.TrangThaiId, e.CreatedAt }); x.Property(e => e.MaCuocKhaoSat).HasMaxLength(50); x.Property(e => e.TenCuocKhaoSat).HasMaxLength(500); });
        modelBuilder.Entity<NhomDoiTuongKhaoSat>(x => x.HasIndex(e => new { e.CuocKhaoSatId, e.MaNhom }).IsUnique());
        modelBuilder.Entity<MauPhieuKhaoSat>(x => { x.HasIndex(e => new { e.NhomDoiTuongKhaoSatId, e.PhienBan }).IsUnique(); x.HasIndex(e => e.NhomDoiTuongKhaoSatId).HasFilter("[TrangThaiMauPhieu] = N'DANG_SU_DUNG'").IsUnique(); x.Property(e => e.MaHash).HasMaxLength(64); x.Property(e => e.TrangThaiMauPhieu).HasMaxLength(30); });
        modelBuilder.Entity<CauHoiThongKe>(x => { x.HasIndex(e => new { e.CuocKhaoSatId, e.MaCauHoiThongKe }).IsUnique(); x.Property(e => e.MauSoTyLe).HasMaxLength(30).HasDefaultValue("PHIEU_HOP_LE"); });
        modelBuilder.Entity<CauHoiMauPhieu>(x => { x.HasIndex(e => new { e.MauPhieuKhaoSatId, e.MaCauHoi }).IsUnique(); x.Property(e => e.MauSoTyLe).HasMaxLength(30).HasDefaultValue("PHIEU_HOP_LE"); });
        modelBuilder.Entity<LuaChonTraLoi>(x => x.HasIndex(e => new { e.CauHoiMauPhieuId, e.MaLuaChon }).IsUnique());
        modelBuilder.Entity<DoiTuongKhaoSat>(x => x.HasIndex(e => new { e.CuocKhaoSatId, e.DonViId }).IsUnique());
        modelBuilder.Entity<PhieuNopKhaoSat>(x =>
        {
            x.HasIndex(e => new { e.DoiTuongKhaoSatId, e.CreatedAt });
            x.Property(e => e.TenPhanMem).HasMaxLength(250);
            x.Property(e => e.PhienBanPhanMem).HasMaxLength(100);
            x.Property(e => e.DuongDanHeThongNguon).HasMaxLength(1000);
        });
        modelBuilder.Entity<CauTraLoiKhaoSat>(x => { x.Property(e => e.GiaTriSo).HasPrecision(18, 2); x.Property(e => e.SoLuong).HasPrecision(18, 2); x.Property(e => e.TongSoTraLoi).HasPrecision(18, 2); });
        modelBuilder.Entity<BaoCaoKhaoSat>(x => { x.HasIndex(e => new { e.CuocKhaoSatId, e.TrangThai }); x.Property(e => e.TenBaoCao).HasMaxLength(500); x.Property(e => e.SoKyHieu).HasMaxLength(100); x.Property(e => e.TrangThai).HasMaxLength(20); });
        modelBuilder.Entity<ChiTietBaoCaoKhaoSat>(x => { x.HasIndex(e => new { e.BaoCaoKhaoSatId, e.CauHoiThongKeId, e.MaLuaChon }); x.Property(e => e.SoLuong).HasPrecision(18, 2); x.Property(e => e.MauSoTyLe).HasPrecision(18, 2); x.Property(e => e.TyLe).HasPrecision(18, 2); });
        modelBuilder.Entity<LichSuXuLyKhaoSat>(x => x.HasIndex(e => new { e.CuocKhaoSatId, e.ThoiGianXuLy }));
    }
}
