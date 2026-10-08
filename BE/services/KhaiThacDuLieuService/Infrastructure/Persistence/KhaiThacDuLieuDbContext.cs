using KhaiThacDuLieuService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhaiThacDuLieuService.Infrastructure.Persistence;

public sealed class KhaiThacDuLieuDbContext(DbContextOptions<KhaiThacDuLieuDbContext> options) : DbContext(options)
{
    public DbSet<CauHinhCanhBaoKhaiThacDuLieu> CauHinhCanhBaoKhaiThacDuLieus => Set<CauHinhCanhBaoKhaiThacDuLieu>();
    public DbSet<CanhBaoKhaiThacDuLieu> CanhBaoKhaiThacDuLieus => Set<CanhBaoKhaiThacDuLieu>();
    public DbSet<DongBoKhaiThacDuLieuLog> DongBoKhaiThacDuLieuLogs => Set<DongBoKhaiThacDuLieuLog>();
    public DbSet<DangKyXayDungVanBanTraCuu> DangKyXayDungVanBans => Set<DangKyXayDungVanBanTraCuu>();
    public DbSet<DangKyTrangThaiHoSoTraCuu> DangKyTrangThaiHoSos => Set<DangKyTrangThaiHoSoTraCuu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CauHinhCanhBaoKhaiThacDuLieu>(entity =>
        {
            entity.HasIndex(x => x.MaCanhBao).IsUnique();
            entity.HasIndex(x => new { x.NhomCanhBao, x.TrangThai });
        });

        modelBuilder.Entity<CanhBaoKhaiThacDuLieu>(entity =>
        {
            entity.HasIndex(x => new { x.NhomCanhBao, x.TrangThaiXuLy, x.NgayPhatSinh });
            entity.HasIndex(x => new { x.DonViNhanId, x.TrangThaiXuLy, x.NgayPhatSinh });
            entity.HasIndex(x => new { x.NguoiNhanId, x.TrangThaiXuLy, x.NgayPhatSinh });
            entity.HasIndex(x => new { x.DoiTuongNguon, x.DoiTuongNguonId, x.MaCanhBao });
        });

        modelBuilder.Entity<DongBoKhaiThacDuLieuLog>(entity =>
        {
            entity.HasIndex(x => new { x.NguonDuLieu, x.LoaiDongBo, x.BatDauLuc });
        });

        modelBuilder.Entity<DangKyXayDungVanBanTraCuu>(entity =>
        {
            entity.ToTable("DangKyXayDungVanBans");
            entity.HasIndex(x => x.MaHoSo);
            entity.HasIndex(x => x.TrangThaiHoSoId);
            entity.HasIndex(x => x.LoaiVanBanId);
            entity.HasIndex(x => x.NamDangKy);
            entity.HasIndex(x => new { x.DonViSoanThaoId, x.TrangThaiHoSoId, x.CreatedAt });
            entity.HasIndex(x => new { x.DonViPheDuyetId, x.TrangThaiHoSoId, x.CreatedAt });
        });

        modelBuilder.Entity<DangKyTrangThaiHoSoTraCuu>(entity =>
        {
            entity.ToTable("DangKyTrangThaiHoSos");
            entity.HasIndex(x => x.MaTrangThai);
        });
    }
}
