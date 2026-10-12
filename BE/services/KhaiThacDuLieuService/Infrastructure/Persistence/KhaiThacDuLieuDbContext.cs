using KhaiThacDuLieuService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhaiThacDuLieuService.Infrastructure.Persistence;

public sealed class KhaiThacDuLieuDbContext(DbContextOptions<KhaiThacDuLieuDbContext> options) : DbContext(options)
{
    public DbSet<CauHinhCanhBaoKhaiThacDuLieu> CauHinhCanhBaoKhaiThacDuLieus => Set<CauHinhCanhBaoKhaiThacDuLieu>();
    public DbSet<CanhBaoKhaiThacDuLieu> CanhBaoKhaiThacDuLieus => Set<CanhBaoKhaiThacDuLieu>();
    public DbSet<CanhBaoNhacViec> CanhBaoNhacViecs => Set<CanhBaoNhacViec>();
    public DbSet<CanhBaoLichSuXuLy> CanhBaoLichSuXuLys => Set<CanhBaoLichSuXuLy>();
    public DbSet<DongBoKhaiThacDuLieuLog> DongBoKhaiThacDuLieuLogs => Set<DongBoKhaiThacDuLieuLog>();
    public DbSet<DashboardWidget> DashboardWidgets => Set<DashboardWidget>();
    public DbSet<DashboardWidgetCauHinh> DashboardWidgetCauHinhs => Set<DashboardWidgetCauHinh>();
    public DbSet<DangKyXayDungVanBanTraCuu> DangKyXayDungVanBans => Set<DangKyXayDungVanBanTraCuu>();
    public DbSet<DangKyTrangThaiHoSoTraCuu> DangKyTrangThaiHoSos => Set<DangKyTrangThaiHoSoTraCuu>();
    public DbSet<XayDungVanBanHoSoTraCuu> XayDungVanBanHoSos => Set<XayDungVanBanHoSoTraCuu>();
    public DbSet<XayDungVanBanBoHoSoTraCuu> XayDungVanBanBoHoSos => Set<XayDungVanBanBoHoSoTraCuu>();
    public DbSet<DanhMucDonViTraCuu> DanhMucDonVis => Set<DanhMucDonViTraCuu>();
    public DbSet<ThiHanhPhapLuatKeHoachTraCuu> ThiHanhPhapLuatKeHoachs => Set<ThiHanhPhapLuatKeHoachTraCuu>();
    public DbSet<ThiHanhPhapLuatNoiDungTraCuu> ThiHanhPhapLuatNoiDungs => Set<ThiHanhPhapLuatNoiDungTraCuu>();
    public DbSet<ThiHanhPhapLuatPhanCongTraCuu> ThiHanhPhapLuatPhanCongs => Set<ThiHanhPhapLuatPhanCongTraCuu>();

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

        modelBuilder.Entity<CanhBaoNhacViec>(entity =>
        {
            entity.HasOne(x => x.CanhBao)
                .WithMany(x => x.NhacViecs)
                .HasForeignKey(x => x.CanhBaoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.CanhBaoId, x.TrangThai, x.HanXuLy });
            entity.HasIndex(x => new { x.NguoiNhanId, x.TrangThai, x.HanXuLy });
            entity.HasIndex(x => new { x.DonViNhanId, x.TrangThai, x.HanXuLy });
        });

        modelBuilder.Entity<CanhBaoLichSuXuLy>(entity =>
        {
            entity.HasOne(x => x.CanhBao)
                .WithMany(x => x.LichSuXuLys)
                .HasForeignKey(x => x.CanhBaoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.NhacViec)
                .WithMany(x => x.LichSuXuLys)
                .HasForeignKey(x => x.NhacViecId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasIndex(x => new { x.CanhBaoId, x.ThoiGian });
            entity.HasIndex(x => new { x.NhacViecId, x.ThoiGian });
        });

        modelBuilder.Entity<DongBoKhaiThacDuLieuLog>(entity =>
        {
            entity.HasIndex(x => new { x.NguonDuLieu, x.LoaiDongBo, x.BatDauLuc });
        });

        modelBuilder.Entity<DashboardWidget>(entity =>
        {
            entity.HasIndex(x => x.MaWidget).IsUnique();
            entity.HasIndex(x => new { x.NhomWidget, x.TrangThai });
        });

        modelBuilder.Entity<DashboardWidgetCauHinh>(entity =>
        {
            entity.HasOne(x => x.DashboardWidget)
                .WithMany(x => x.CauHinhs)
                .HasForeignKey(x => x.DashboardWidgetId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.NguoiDungId, x.ThuTu });
            entity.HasIndex(x => new { x.NhomQuyenId, x.ThuTu });
            entity.HasIndex(x => new { x.DonViId, x.ThuTu });
            entity.HasIndex(x => new { x.DashboardWidgetId, x.NguoiDungId, x.NhomQuyenId, x.DonViId });
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

        modelBuilder.Entity<XayDungVanBanHoSoTraCuu>(entity =>
        {
            entity.ToTable("HoSoXayDungVanBans", table => table.ExcludeFromMigrations());
            entity.HasIndex(x => x.MaHoSo);
            entity.HasIndex(x => x.NamXayDung);
            entity.HasIndex(x => new { x.DonViChuTriSoanThaoId, x.TrangThaiHoSoId, x.CreatedAt });
            entity.HasIndex(x => new { x.QuyTrinhSoanThaoId, x.BuocHienTaiId, x.TrangThaiHoSoId });
        });

        modelBuilder.Entity<XayDungVanBanBoHoSoTraCuu>(entity =>
        {
            entity.ToTable("BoHoSoNghiepVus", table => table.ExcludeFromMigrations());
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.LoaiBoHoSo, x.TrangThai });
            entity.HasIndex(x => new { x.HoSoXayDungVanBanId, x.BuocQuyTrinhId, x.LanXuLy });
        });

        modelBuilder.Entity<DanhMucDonViTraCuu>(entity =>
        {
            entity.ToTable("DanhMucDonVis", "dm", table => table.ExcludeFromMigrations());
            // Bảng danh mục dùng chung hiện chưa có cột soft-delete.
            // Đây là entity chỉ đọc nên không ánh xạ cột IsDeleted kế thừa.
            entity.Ignore(x => x.IsDeleted);
            entity.HasIndex(x => x.TenDonVi);
        });

        modelBuilder.Entity<ThiHanhPhapLuatKeHoachTraCuu>(entity =>
        {
            entity.ToTable("KeHoachThiHanhPhapLuats", "thpl", table => table.ExcludeFromMigrations());
            entity.HasIndex(x => x.MaKeHoach);
            entity.HasIndex(x => new { x.DonViChuTriId, x.TrangThaiId, x.Nam });
        });

        modelBuilder.Entity<ThiHanhPhapLuatNoiDungTraCuu>(entity =>
        {
            entity.ToTable("NoiDungKeHoachs", "thpl", table => table.ExcludeFromMigrations());
            entity.Property(x => x.TyLeHoanThanh).HasPrecision(5, 2);
            entity.HasIndex(x => new { x.KeHoachId, x.TrangThaiId, x.HanHoanThanh });
        });

        modelBuilder.Entity<ThiHanhPhapLuatPhanCongTraCuu>(entity =>
        {
            entity.ToTable("PhanCongThiHanhs", "thpl", table => table.ExcludeFromMigrations());
            entity.HasIndex(x => new { x.NoiDungKeHoachId, x.DonViDuocGiaoId });
            entity.HasIndex(x => new { x.DonViDuocGiaoId, x.HanThucHien });
        });
    }
}
