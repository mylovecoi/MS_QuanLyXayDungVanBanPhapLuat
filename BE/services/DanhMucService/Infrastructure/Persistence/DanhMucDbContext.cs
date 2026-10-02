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
                Id = DanhMucSeedIds.BuocDangKyXayDungQppl.LapHoSo,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                MaBuoc = "LAP_HO_SO",
                TenBuoc = "Lập hồ sơ đề nghị/đăng ký",
                ThuTuSapXep = 1,
                LoaiBuoc = "NhapLieu",
                BatBuoc = true,
                MoTa = "Đơn vị soạn thảo lập hồ sơ đăng ký trên phần mềm",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucBuocQuyTrinh
            {
                Id = DanhMucSeedIds.BuocDangKyXayDungQppl.TrinhHoSo,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                MaBuoc = "TRINH_HO_SO",
                TenBuoc = "Trình hồ sơ/Chờ phê duyệt",
                ThuTuSapXep = 2,
                LoaiBuoc = "XuLy",
                BatBuoc = true,
                MoTa = "Hồ sơ đã trình sang đơn vị phê duyệt",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucBuocQuyTrinh
            {
                Id = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyet,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                MaBuoc = "PHE_DUYET",
                TenBuoc = "Phê duyệt hồ sơ",
                ThuTuSapXep = 3,
                LoaiBuoc = "PheDuyet",
                BatBuoc = true,
                MoTa = "Đơn vị phê duyệt nhập kết quả phê duyệt hoặc trả lại",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucBuocQuyTrinh
            {
                Id = DanhMucSeedIds.BuocDangKyXayDungQppl.CapNhatKetQua,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                MaBuoc = "CAP_NHAT_KET_QUA",
                TenBuoc = "Cập nhật kết quả phê duyệt",
                ThuTuSapXep = 4,
                LoaiBuoc = "NhapLieu",
                BatBuoc = true,
                YeuCauFileDinhKem = true,
                MoTa = "Đơn vị soạn thảo cập nhật số/ngày văn bản và file kết quả",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucBuocQuyTrinh
            {
                Id = DanhMucSeedIds.BuocDangKyXayDungQppl.HoanThanh,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                MaBuoc = "HOAN_THANH",
                TenBuoc = "Hoàn thành",
                ThuTuSapXep = 5,
                LoaiBuoc = "KetThuc",
                BatBuoc = true,
                MoTa = "Hoàn thành quy trình đăng ký xây dựng văn bản",
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            });

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>().HasData(
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.LapHoSoToTrinhHoSo,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.LapHoSo,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.TrinhHoSo,
                DieuKienKetQua = "TRINH_PHE_DUYET",
                LoaiChuyenBuoc = "Forward",
                LaNhanhMacDinh = true,
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.TrinhHoSoToPheDuyet,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.TrinhHoSo,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyet,
                DieuKienKetQua = "TIEP_NHAN_PHE_DUYET",
                LoaiChuyenBuoc = "Forward",
                LaNhanhMacDinh = true,
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.PheDuyetToCapNhatKetQua,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.PheDuyet,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.CapNhatKetQua,
                DieuKienKetQua = "PHE_DUYET",
                LoaiChuyenBuoc = "Forward",
                LaNhanhMacDinh = true,
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.PheDuyetToLapHoSo,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.TrinhHoSo,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.LapHoSo,
                DieuKienKetQua = "TRA_LAI",
                LoaiChuyenBuoc = "Return",
                YeuCauNhapLyDo = true,
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.PheDuyetToHoanThanh,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.TrinhHoSo,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.HoanThanh,
                DieuKienKetQua = "KHONG_PHE_DUYET",
                LoaiChuyenBuoc = "Reject",
                YeuCauNhapLyDo = true,
                IsKetThuc = true,
                CreatedBy = seedUser,
                CreatedDate = seedDate,
                UpdatedBy = seedUser,
                UpdatedDate = seedDate
            },
            new DanhMucChuyenBuocQuyTrinh
            {
                Id = DanhMucSeedIds.ChuyenBuocDangKyXayDungQppl.CapNhatKetQuaToHoanThanh,
                QuyTrinhSoanThaoId = DanhMucSeedIds.QuyTrinh.DeXuatDangKyXayDungQppl,
                TuBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.CapNhatKetQua,
                DenBuocId = DanhMucSeedIds.BuocDangKyXayDungQppl.HoanThanh,
                DieuKienKetQua = "HOAN_THANH",
                LoaiChuyenBuoc = "Forward",
                IsKetThuc = true,
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
