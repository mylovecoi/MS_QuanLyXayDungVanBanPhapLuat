using BuildingBlocks.Abstractions;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence;

public class DanhMucDbContext : DbContext
{
    private readonly ICurrentUserContext? _currentUserContext;

    public DanhMucDbContext(DbContextOptions<DanhMucDbContext> options, ICurrentUserContext? currentUserContext = null)
        : base(options)
    {
        _currentUserContext = currentUserContext;
    }

    public DbSet<DanhMucDonVi> DanhMucDonVis { get; set; }
    public DbSet<DanhMucDiaDanh> DanhMucDiaDanhs { get; set; }
    public DbSet<DanhMucQuyTrinhSoanThao> DanhMucQuyTrinhSoanThaos { get; set; }
    public DbSet<DanhMucBuocQuyTrinh> DanhMucBuocQuyTrinhs { get; set; }
    public DbSet<DanhMucChuyenBuocQuyTrinh> DanhMucChuyenBuocQuyTrinhs { get; set; }
    public DbSet<DanhMucTrangThai> DanhMucTrangThais { get; set; }
    public DbSet<DanhMucTieuChiDiem> DanhMucTieuChiDiems { get; set; }
    public DbSet<DanhMucTieuChiDiemMuc> DanhMucTieuChiDiemMucs { get; set; }
    public DbSet<DanhMucVanBan> DanhMucVanBans { get; set; }
    public DbSet<DanhMucLinhVuc> DanhMucLinhVucs { get; set; }
    public DbSet<DanhMucPhongBan> DanhMucPhongBans { get; set; }
    public DbSet<DanhMucCanBo> DanhMucCanBos { get; set; }
    public DbSet<HoSoVanBanUsage> HoSoVanBanUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("dm");

        modelBuilder.Entity<DanhMucBuocQuyTrinh>()
            .HasOne<DanhMucQuyTrinhSoanThao>()
            .WithMany()
            .HasForeignKey(x => x.QuyTrinhSoanThaoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DanhMucBuocQuyTrinh>()
            .HasOne<DanhMucDonVi>()
            .WithMany()
            .HasForeignKey(x => x.DonViTiepNhanMacDinhId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
            .HasOne<DanhMucQuyTrinhSoanThao>()
            .WithMany()
            .HasForeignKey(x => x.QuyTrinhSoanThaoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
            .HasOne<DanhMucBuocQuyTrinh>()
            .WithMany()
            .HasForeignKey(x => x.TuBuocId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
            .HasOne<DanhMucBuocQuyTrinh>()
            .WithMany()
            .HasForeignKey(x => x.DenBuocId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DanhMucQuyTrinhSoanThao>()
            .HasOne<DanhMucVanBan>()
            .WithMany()
            .HasForeignKey(x => x.DanhMucVanBanId)
            .OnDelete(DeleteBehavior.NoAction);

        // Transitional read guard: this should move behind a VanBan service API/read model
        // when that bounded context exposes an API or read model owned by the VanBan service.
        modelBuilder.Entity<HoSoVanBanUsage>().ToTable("HoSoVanBans", "dbo", tableBuilder => tableBuilder.ExcludeFromMigrations());
        modelBuilder.Entity<DanhMucTieuChiDiem>().Property(x => x.DiemToiDa).HasPrecision(18, 2);
        modelBuilder.Entity<DanhMucTieuChiDiemMuc>().Property(x => x.TuGiaTri).HasPrecision(18, 2);
        modelBuilder.Entity<DanhMucTieuChiDiemMuc>().Property(x => x.DenGiaTri).HasPrecision(18, 2);
        modelBuilder.Entity<DanhMucTieuChiDiemMuc>().Property(x => x.Diem).HasPrecision(18, 2);

        SeedQuyTrinhDangKyXayDungQppl(modelBuilder);
    }

    private static void SeedQuyTrinhDangKyXayDungQppl(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Local);
        var seedUser = Guid.Empty;

        modelBuilder.Entity<DanhMucQuyTrinhSoanThao>().HasData(new DanhMucQuyTrinhSoanThao
        {
            Id = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
            MaQuyTrinh = "DXDM_DKXD_QPPL",
            TenQuyTrinh = "Đề xuất danh mục / Đăng ký xây dựng QPPL",
            LoaiQuyTrinh = "DangKyXayDungQPPL",
            CapApDung = "CapTinh",
            PhienBan = 1,
            TrangThai = true,
            MoTa = "Quy trình đăng ký xây dựng văn bản QPPL cấp tỉnh",
            CreatedBy = seedUser,
            CreatedDate = seedDate,
            UpdatedBy = seedUser,
            UpdatedDate = seedDate
        });

        modelBuilder.Entity<DanhMucBuocQuyTrinh>().HasData(
            new DanhMucBuocQuyTrinh
            {
                Id = DanhMucSeedIds.BuocDangKyXayDungQppl.DeXuatDangKy,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                MaBuoc = "DE_XUAT_DANG_KY",
                TenBuoc = "Đề xuất danh mục/Đăng ký xây dựng văn bản QPPL",
                ThuTuSapXep = 1,
                LoaiBuoc = "NhapLieu",
                BatBuoc = true,
                MoTa = "Cơ quan chủ trì lập hồ sơ, trình hồ sơ đến cơ quan có thẩm quyền và hệ thống lưu trữ hồ sơ đề xuất/đăng ký.",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucBuocQuyTrinh
            {
                Id = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyetCapNhatKetQua,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                MaBuoc = "PHE_DUYET_CAP_NHAT_KET_QUA",
                TenBuoc = "Phê duyệt và cập nhật kết quả",
                ThuTuSapXep = 2,
                LoaiBuoc = "XuLy",
                BatBuoc = true,
                YeuCauFileDinhKem = true,
                MoTa = "Việc phê duyệt diễn ra ngoài hệ thống; cơ quan chủ trì cập nhật kết quả phê duyệt lên phần mềm.",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            });

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>().HasData(
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.DeXuatDangKyToPheDuyetCapNhatKetQua,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.DeXuatDangKy,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyetCapNhatKetQua,
                DieuKienKetQua = "TRINH_PHE_DUYET",
                LoaiChuyenBuoc = "Forward",
                LaNhanhMacDinh = true,
                MoTa = "Trình hồ sơ đề xuất/đăng ký sang bước phê duyệt và cập nhật kết quả.",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.PheDuyetCapNhatKetQuaToDeXuatDangKy,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyetCapNhatKetQua,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.DeXuatDangKy,
                DieuKienKetQua = "TRA_LAI",
                LoaiChuyenBuoc = "Return",
                YeuCauNhapLyDo = true,
                MoTa = "Trả hồ sơ về bước đề xuất/đăng ký để chỉnh sửa.",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.PheDuyetCapNhatKetQuaToHoanThanh,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyetCapNhatKetQua,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyetCapNhatKetQua,
                DieuKienKetQua = "HOAN_THANH",
                LoaiChuyenBuoc = "Complete",
                IsKetThuc = true,
                MoTa = "Kết thúc quy trình đăng ký sau khi cập nhật kết quả.",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            });
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var userId = _currentUserContext?.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return;
        }

        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedBy = userId.Value;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = userId.Value;
                continue;
            }

            entry.Property(nameof(BaseEntity.CreatedDate)).IsModified = false;
            entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
        }
    }
}
