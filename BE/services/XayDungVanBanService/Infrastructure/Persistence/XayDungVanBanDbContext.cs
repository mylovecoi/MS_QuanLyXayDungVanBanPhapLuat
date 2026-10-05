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
    public DbSet<HoSoXayDungVanBanSoanThao> HoSoXayDungVanBanSoanThaos => Set<HoSoXayDungVanBanSoanThao>();
    public DbSet<HoSoXayDungVanBanTrinhThamDinh> HoSoXayDungVanBanTrinhThamDinhs => Set<HoSoXayDungVanBanTrinhThamDinh>();
    public DbSet<HoSoXayDungVanBanThamDinh> HoSoXayDungVanBanThamDinhs => Set<HoSoXayDungVanBanThamDinh>();
    public DbSet<HoSoXayDungVanBanTrinhPheDuyet> HoSoXayDungVanBanTrinhPheDuyets => Set<HoSoXayDungVanBanTrinhPheDuyet>();
    public DbSet<HoSoXayDungVanBanYKienUbnd> HoSoXayDungVanBanYKienUbnds => Set<HoSoXayDungVanBanYKienUbnd>();
    public DbSet<HoSoXayDungVanBanThamTraHdnd> HoSoXayDungVanBanThamTraHdnds => Set<HoSoXayDungVanBanThamTraHdnd>();
    public DbSet<HoSoXayDungVanBanKetQuaBanHanh> HoSoXayDungVanBanKetQuaBanHanhs => Set<HoSoXayDungVanBanKetQuaBanHanh>();

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
