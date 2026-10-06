using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Infrastructure.Persistence;

public sealed class XayDungVanBanDbContext(DbContextOptions<XayDungVanBanDbContext> options) : DbContext(options)
{
    public DbSet<HoSoXayDungVanBan> HoSoXayDungVanBans => Set<HoSoXayDungVanBan>();
    public DbSet<BoHoSoNghiepVu> BoHoSoNghiepVus => Set<BoHoSoNghiepVu>();
    public DbSet<HoSoXayDungVanBanFile> HoSoXayDungVanBanFiles => Set<HoSoXayDungVanBanFile>();
    public DbSet<BoHoSoNghiepVuTaiLieu> BoHoSoNghiepVuTaiLieus => Set<BoHoSoNghiepVuTaiLieu>();
    public DbSet<HoSoXayDungVanBanLichSuXuLy> HoSoXayDungVanBanLichSuXuLys => Set<HoSoXayDungVanBanLichSuXuLy>();
    public DbSet<HoSoXayDungVanBanYKienDonVi> HoSoXayDungVanBanYKienDonVis => Set<HoSoXayDungVanBanYKienDonVi>();
    public DbSet<HoSoXayDungVanBanSoSanhDuThao> HoSoXayDungVanBanSoSanhDuThaos => Set<HoSoXayDungVanBanSoSanhDuThao>();
    public DbSet<HoSoXayDungVanBanSoanThao> HoSoXayDungVanBanSoanThaos => Set<HoSoXayDungVanBanSoanThao>();
    public DbSet<HoSoXayDungVanBanTrinhThamDinh> HoSoXayDungVanBanTrinhThamDinhs => Set<HoSoXayDungVanBanTrinhThamDinh>();
    public DbSet<HoSoXayDungVanBanThamDinh> HoSoXayDungVanBanThamDinhs => Set<HoSoXayDungVanBanThamDinh>();
    public DbSet<HoSoXayDungVanBanTrinhPheDuyet> HoSoXayDungVanBanTrinhPheDuyets => Set<HoSoXayDungVanBanTrinhPheDuyet>();
    public DbSet<HoSoXayDungVanBanYKienUbnd> HoSoXayDungVanBanYKienUbnds => Set<HoSoXayDungVanBanYKienUbnd>();
    public DbSet<HoSoXayDungVanBanThamTraHdnd> HoSoXayDungVanBanThamTraHdnds => Set<HoSoXayDungVanBanThamTraHdnd>();
    public DbSet<HoSoXayDungVanBanKetQuaBanHanh> HoSoXayDungVanBanKetQuaBanHanhs => Set<HoSoXayDungVanBanKetQuaBanHanh>();
    public DbSet<HoSoXayDungVanBanChamDiem> HoSoXayDungVanBanChamDiems => Set<HoSoXayDungVanBanChamDiem>();
    public DbSet<HoSoXayDungVanBanChamDiemChiTiet> HoSoXayDungVanBanChamDiemChiTiets => Set<HoSoXayDungVanBanChamDiemChiTiet>();
    public DbSet<HoSoXayDungVanBanChamDiemLichSu> HoSoXayDungVanBanChamDiemLichSus => Set<HoSoXayDungVanBanChamDiemLichSu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HoSoXayDungVanBan>(entity =>
        {
            entity.HasIndex(x => x.MaHoSo).IsUnique();
            entity.HasIndex(x => new { x.DanhMucVanBanId, x.NamXayDung });
            entity.HasIndex(x => new { x.QuyTrinhSoanThaoId, x.BuocHienTaiId, x.TrangThaiHoSoId });
            entity.HasIndex(x => new { x.DonViChuTriSoanThaoId, x.TrangThaiHoSoId, x.CreatedAt });
            entity.HasIndex(x => new { x.NguoiPhuTrachId, x.CreatedAt });
            entity.HasIndex(x => new { x.CreatedBy, x.CreatedAt });
            entity.HasIndex(x => x.HoSoDangKyXayDungVanBanId);
            entity.HasMany(x => x.BoHoSos).WithOne(x => x.HoSoXayDungVanBan).HasForeignKey(x => x.HoSoXayDungVanBanId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Files).WithOne(x => x.HoSoXayDungVanBan).HasForeignKey(x => x.HoSoXayDungVanBanId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.LichSuXuLys).WithOne(x => x.HoSoXayDungVanBan).HasForeignKey(x => x.HoSoXayDungVanBanId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.YKienDonVis).WithOne(x => x.HoSoXayDungVanBan).HasForeignKey(x => x.HoSoXayDungVanBanId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BoHoSoNghiepVu>(entity =>
        {
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.BuocQuyTrinhId, x.LanXuLy }).IsUnique();
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.LoaiBoHoSo, x.TrangThai });
            entity.HasOne(x => x.BoHoSoNguon).WithMany().HasForeignKey(x => x.BoHoSoNguonId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<HoSoXayDungVanBanFile>(entity =>
        {
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.LoaiTaiLieuId, x.PhienBan }).IsUnique();
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.IsCurrent });
        });

        modelBuilder.Entity<BoHoSoNghiepVuTaiLieu>(entity =>
        {
            entity.HasIndex(x => new { x.BoHoSoNghiepVuId, x.HoSoXayDungVanBanFileId }).IsUnique();
            entity.HasOne(x => x.BoHoSoNghiepVu).WithMany(x => x.TaiLieus).HasForeignKey(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.HoSoXayDungVanBanFile).WithMany().HasForeignKey(x => x.HoSoXayDungVanBanFileId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.BoHoSoTaiLieuNguon).WithMany().HasForeignKey(x => x.BoHoSoTaiLieuNguonId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<HoSoXayDungVanBanLichSuXuLy>(entity =>
        {
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.ThoiGianXuLy });
            entity.HasIndex(x => new { x.BoHoSoNghiepVuId, x.ThoiGianXuLy });
            entity.HasOne(x => x.BoHoSoNghiepVu).WithMany(x => x.LichSuXuLys).HasForeignKey(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<HoSoXayDungVanBanYKienDonVi>(entity =>
        {
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.DonViGopYId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.NgayNhan });
        });

        modelBuilder.Entity<HoSoXayDungVanBanSoSanhDuThao>(entity =>
        {
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.CreatedAt });
            entity.HasIndex(x => new { x.FileGocId, x.FileSoSanhId });
        });

        modelBuilder.Entity<HoSoXayDungVanBanChamDiem>(entity =>
        {
            entity.Property(x => x.TongDiemTuDong).HasPrecision(18, 2);
            entity.Property(x => x.TongDiemDieuChinh).HasPrecision(18, 2);
            entity.Property(x => x.TongDiemChinhThuc).HasPrecision(18, 2);
            entity.Property(x => x.TyLeThoiGianThucTe).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.LanCham }).IsUnique();
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.TrangThaiId });
            entity.HasOne<HoSoXayDungVanBan>().WithMany().HasForeignKey(x => x.HoSoXayDungVanBanId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HoSoXayDungVanBanChamDiemChiTiet>(entity =>
        {
            entity.Property(x => x.GiaTriDauVao).HasPrecision(18, 2);
            entity.Property(x => x.DiemToiDa).HasPrecision(18, 2);
            entity.Property(x => x.DiemTuDong).HasPrecision(18, 2);
            entity.Property(x => x.DiemDieuChinh).HasPrecision(18, 2);
            entity.Property(x => x.DiemChinhThuc).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.HoSoXayDungVanBanChamDiemId, x.DanhMucTieuChiDiemId }).IsUnique();
            entity.HasOne(x => x.ChamDiem).WithMany(x => x.ChiTiets).HasForeignKey(x => x.HoSoXayDungVanBanChamDiemId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HoSoXayDungVanBanChamDiemLichSu>(entity =>
        {
            entity.HasIndex(x => new { x.HoSoXayDungVanBanChamDiemId, x.ThoiGianThucHien });
            entity.HasOne(x => x.ChamDiem).WithMany(x => x.LichSus).HasForeignKey(x => x.HoSoXayDungVanBanChamDiemId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HoSoXayDungVanBanSoanThao>().HasKey(x => x.BoHoSoNghiepVuId);
        modelBuilder.Entity<HoSoXayDungVanBanTrinhThamDinh>().HasKey(x => x.BoHoSoNghiepVuId);
        modelBuilder.Entity<HoSoXayDungVanBanThamDinh>().HasKey(x => x.BoHoSoNghiepVuId);
        modelBuilder.Entity<HoSoXayDungVanBanTrinhPheDuyet>().HasKey(x => x.BoHoSoNghiepVuId);
        modelBuilder.Entity<HoSoXayDungVanBanYKienUbnd>().HasKey(x => x.BoHoSoNghiepVuId);
        modelBuilder.Entity<HoSoXayDungVanBanThamTraHdnd>().HasKey(x => x.BoHoSoNghiepVuId);
        modelBuilder.Entity<HoSoXayDungVanBanKetQuaBanHanh>().HasKey(x => x.BoHoSoNghiepVuId);

        modelBuilder.Entity<BoHoSoNghiepVu>().HasOne(x => x.SoanThao).WithOne(x => x.BoHoSoNghiepVu).HasForeignKey<HoSoXayDungVanBanSoanThao>(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BoHoSoNghiepVu>().HasOne(x => x.TrinhThamDinh).WithOne(x => x.BoHoSoNghiepVu).HasForeignKey<HoSoXayDungVanBanTrinhThamDinh>(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BoHoSoNghiepVu>().HasOne(x => x.ThamDinh).WithOne(x => x.BoHoSoNghiepVu).HasForeignKey<HoSoXayDungVanBanThamDinh>(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BoHoSoNghiepVu>().HasOne(x => x.TrinhPheDuyet).WithOne(x => x.BoHoSoNghiepVu).HasForeignKey<HoSoXayDungVanBanTrinhPheDuyet>(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BoHoSoNghiepVu>().HasOne(x => x.YKienUbnd).WithOne(x => x.BoHoSoNghiepVu).HasForeignKey<HoSoXayDungVanBanYKienUbnd>(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BoHoSoNghiepVu>().HasOne(x => x.ThamTraHdnd).WithOne(x => x.BoHoSoNghiepVu).HasForeignKey<HoSoXayDungVanBanThamTraHdnd>(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BoHoSoNghiepVu>().HasOne(x => x.KetQuaBanHanh).WithOne(x => x.BoHoSoNghiepVu).HasForeignKey<HoSoXayDungVanBanKetQuaBanHanh>(x => x.BoHoSoNghiepVuId).OnDelete(DeleteBehavior.Cascade);
    }
}
