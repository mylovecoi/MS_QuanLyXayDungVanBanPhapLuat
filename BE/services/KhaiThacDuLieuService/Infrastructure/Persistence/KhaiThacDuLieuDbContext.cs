using KhaiThacDuLieuService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhaiThacDuLieuService.Infrastructure.Persistence;

public sealed class KhaiThacDuLieuDbContext(DbContextOptions<KhaiThacDuLieuDbContext> options) : DbContext(options)
{
    public DbSet<CauHinhCanhBaoKhaiThacDuLieu> CauHinhCanhBaoKhaiThacDuLieus => Set<CauHinhCanhBaoKhaiThacDuLieu>();
    public DbSet<CanhBaoKhaiThacDuLieu> CanhBaoKhaiThacDuLieus => Set<CanhBaoKhaiThacDuLieu>();
    public DbSet<DongBoKhaiThacDuLieuLog> DongBoKhaiThacDuLieuLogs => Set<DongBoKhaiThacDuLieuLog>();

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
    }
}
