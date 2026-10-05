using DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence;

public class DangKyXayDungVanBanDbContext : DbContext
{
    public DangKyXayDungVanBanDbContext(DbContextOptions<DangKyXayDungVanBanDbContext> options)
        : base(options)
    {
    }

    public DbSet<DangKyXayDungVanBan> DangKyXayDungVanBans => Set<DangKyXayDungVanBan>();
    public DbSet<DangKyXayDungVanBanFile> DangKyXayDungVanBanFiles => Set<DangKyXayDungVanBanFile>();
    public DbSet<DangKyXayDungVanBanLichSuXuLy> DangKyXayDungVanBanLichSuXuLys => Set<DangKyXayDungVanBanLichSuXuLy>();
    public DbSet<DangKyXayDungVanBanKetQuaPheDuyet> DangKyXayDungVanBanKetQuaPheDuyets => Set<DangKyXayDungVanBanKetQuaPheDuyet>();
    public DbSet<DangKyXayDungVanBanLienKetQuyTrinh> DangKyXayDungVanBanLienKetQuyTrinhs => Set<DangKyXayDungVanBanLienKetQuyTrinh>();
    public DbSet<DangKyTrangThaiHoSo> DangKyTrangThaiHoSos => Set<DangKyTrangThaiHoSo>();
    public DbSet<DangKyHanhDongXuLy> DangKyHanhDongXuLys => Set<DangKyHanhDongXuLy>();
    public DbSet<DangKyCauHinhChuyenTrangThai> DangKyCauHinhChuyenTrangThais => Set<DangKyCauHinhChuyenTrangThai>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DangKyXayDungVanBan>(entity =>
        {
            entity.HasIndex(x => x.MaHoSo).IsUnique();
            entity.HasIndex(x => x.TrangThaiHoSoId);
            entity.HasIndex(x => x.BuocHienTaiId);
            entity.HasIndex(x => x.DonViSoanThaoId);
            entity.HasIndex(x => x.DonViPheDuyetId);
            entity.HasIndex(x => x.LoaiVanBanId);
            entity.HasIndex(x => x.NamDangKy);
            entity.HasIndex(x => x.CreatedAt);
            entity.HasIndex(x => new { x.DonViSoanThaoId, x.TrangThaiHoSoId, x.CreatedAt });
            entity.HasIndex(x => new { x.DonViPheDuyetId, x.TrangThaiHoSoId, x.CreatedAt });
            entity.HasIndex(x => new { x.CreatedBy, x.CreatedAt });

            entity.HasMany(x => x.Files)
                .WithOne(x => x.DangKyXayDungVanBan)
                .HasForeignKey(x => x.DangKyXayDungVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.LichSuXuLys)
                .WithOne(x => x.DangKyXayDungVanBan)
                .HasForeignKey(x => x.DangKyXayDungVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.KetQuaPheDuyets)
                .WithOne(x => x.DangKyXayDungVanBan)
                .HasForeignKey(x => x.DangKyXayDungVanBanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DangKyXayDungVanBanFile>()
            .HasIndex(x => x.DangKyXayDungVanBanId);

        modelBuilder.Entity<DangKyXayDungVanBanLichSuXuLy>(entity =>
        {
            entity.HasIndex(x => x.DangKyXayDungVanBanId);
            entity.HasIndex(x => x.NgayXuLy);
        });

        modelBuilder.Entity<DangKyXayDungVanBanKetQuaPheDuyet>()
            .HasIndex(x => x.DangKyXayDungVanBanId);

        modelBuilder.Entity<DangKyXayDungVanBanLienKetQuyTrinh>()
            .HasIndex(x => x.DangKyXayDungVanBanId);

        modelBuilder.Entity<DangKyTrangThaiHoSo>(entity =>
        {
            entity.HasIndex(x => x.MaTrangThai).IsUnique();
        });

        modelBuilder.Entity<DangKyHanhDongXuLy>(entity =>
        {
            entity.HasIndex(x => x.MaHanhDong).IsUnique();
        });

        modelBuilder.Entity<DangKyCauHinhChuyenTrangThai>(entity =>
        {
            entity.HasIndex(x => new
            {
                x.QuyTrinhSoanThaoId,
                x.BuocHienTaiId,
                x.TrangThaiHienTaiId,
                x.HanhDongId
            }).IsUnique();
        });

        SeedDanhMucDong(modelBuilder);
    }

    private static void SeedDanhMucDong(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<DangKyTrangThaiHoSo>().HasData(
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.MoiTao,
                MaTrangThai = "MOI_TAO",
                TenTrangThai = "Mới tạo",
                MauHienThi = "gray",
                ThuTuSapXep = 1,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.DangSoanThao,
                MaTrangThai = "DANG_SOAN_THAO",
                TenTrangThai = "Đang soạn thảo",
                MauHienThi = "blue",
                ThuTuSapXep = 2,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.DaTrinhPheDuyet,
                MaTrangThai = "DA_TRINH_PHE_DUYET",
                TenTrangThai = "Đã trình phê duyệt",
                MauHienThi = "orange",
                ThuTuSapXep = 3,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.DaPheDuyet,
                MaTrangThai = "DA_PHE_DUYET",
                TenTrangThai = "Đã phê duyệt",
                MauHienThi = "green",
                ThuTuSapXep = 4,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.BiTraLai,
                MaTrangThai = "BI_TRA_LAI",
                TenTrangThai = "Bị trả lại",
                MauHienThi = "red",
                ThuTuSapXep = 5,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.KhongPheDuyet,
                MaTrangThai = "KHONG_PHE_DUYET",
                TenTrangThai = "Không phê duyệt",
                MauHienThi = "red",
                ThuTuSapXep = 6,
                LaTrangThaiKetThuc = true,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.DaCapNhatKetQua,
                MaTrangThai = "DA_CAP_NHAT_KET_QUA",
                TenTrangThai = "Đã cập nhật kết quả",
                MauHienThi = "cyan",
                ThuTuSapXep = 7,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.HoanThanh,
                MaTrangThai = "HOAN_THANH",
                TenTrangThai = "Hoàn thành",
                MauHienThi = "green",
                ThuTuSapXep = 8,
                LaTrangThaiKetThuc = true,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyTrangThaiHoSo
            {
                Id = DangKySeedIds.TrangThai.DaChuyenQuyTrinhXayDung,
                MaTrangThai = "DA_CHUYEN_QUY_TRINH_XAY_DUNG",
                TenTrangThai = "Đã chuyển quy trình xây dựng",
                MauHienThi = "purple",
                ThuTuSapXep = 9,
                LaTrangThaiKetThuc = true,
                TrangThai = true,
                CreatedAt = seedDate
            });

        modelBuilder.Entity<DangKyHanhDongXuLy>().HasData(
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.TaoMoi,
                MaHanhDong = "TAO_MOI",
                TenHanhDong = "Tạo mới",
                LoaiHanhDong = "Create",
                ThuTuSapXep = 1,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.CapNhatHoSo,
                MaHanhDong = "CAP_NHAT_HO_SO",
                TenHanhDong = "Cập nhật hồ sơ",
                LoaiHanhDong = "Update",
                ThuTuSapXep = 2,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.TrinhPheDuyet,
                MaHanhDong = "TRINH_PHE_DUYET",
                TenHanhDong = "Trình phê duyệt",
                LoaiHanhDong = "Forward",
                ThuTuSapXep = 3,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.PheDuyet,
                MaHanhDong = "PHE_DUYET",
                TenHanhDong = "Phê duyệt",
                LoaiHanhDong = "Approve",
                ThuTuSapXep = 4,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.TraLai,
                MaHanhDong = "TRA_LAI",
                TenHanhDong = "Trả lại",
                LoaiHanhDong = "Return",
                YeuCauLyDo = true,
                ThuTuSapXep = 5,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.KhongPheDuyet,
                MaHanhDong = "KHONG_PHE_DUYET",
                TenHanhDong = "Không phê duyệt",
                LoaiHanhDong = "Reject",
                YeuCauLyDo = true,
                ThuTuSapXep = 6,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.CapNhatKetQua,
                MaHanhDong = "CAP_NHAT_KET_QUA",
                TenHanhDong = "Cập nhật kết quả",
                LoaiHanhDong = "Update",
                YeuCauFileDinhKem = true,
                ThuTuSapXep = 7,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.HoanThanh,
                MaHanhDong = "HOAN_THANH",
                TenHanhDong = "Hoàn thành",
                LoaiHanhDong = "Complete",
                ThuTuSapXep = 8,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyHanhDongXuLy
            {
                Id = DangKySeedIds.HanhDong.KhoiTaoQuyTrinhXayDung,
                MaHanhDong = "KHOI_TAO_QUY_TRINH_XAY_DUNG",
                TenHanhDong = "Khởi tạo quy trình xây dựng",
                LoaiHanhDong = "StartNextWorkflow",
                ThuTuSapXep = 9,
                TrangThai = true,
                CreatedAt = seedDate
            });

        modelBuilder.Entity<DangKyCauHinhChuyenTrangThai>().HasData(
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.CapNhatHoSoDangSoanThao,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.DangSoanThao,
                HanhDongId = DangKySeedIds.HanhDong.CapNhatHoSo,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.DangSoanThao,
                NhomNhapLieu = "DonViSoanThao",
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.CapNhatHoSoBiTraLai,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.BiTraLai,
                HanhDongId = DangKySeedIds.HanhDong.CapNhatHoSo,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.DangSoanThao,
                NhomNhapLieu = "DonViSoanThao",
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.TrinhPheDuyetDangSoanThao,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.DangSoanThao,
                HanhDongId = DangKySeedIds.HanhDong.TrinhPheDuyet,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.TrinhHoSo,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.DaTrinhPheDuyet,
                ChuyenBuocId = DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.LapHoSoToTrinhHoSo,
                NhomNhapLieu = "DonViSoanThao",
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.TrinhPheDuyetBiTraLai,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.BiTraLai,
                HanhDongId = DangKySeedIds.HanhDong.TrinhPheDuyet,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.TrinhHoSo,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.DaTrinhPheDuyet,
                ChuyenBuocId = DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.LapHoSoToTrinhHoSo,
                NhomNhapLieu = "DonViSoanThao",
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.PheDuyet,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.TrinhHoSo,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.DaTrinhPheDuyet,
                HanhDongId = DangKySeedIds.HanhDong.PheDuyet,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.PheDuyet,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.DaPheDuyet,
                ChuyenBuocId = DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.TrinhHoSoToPheDuyet,
                NhomNhapLieu = "DonViPheDuyet",
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.TraLai,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.TrinhHoSo,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.DaTrinhPheDuyet,
                HanhDongId = DangKySeedIds.HanhDong.TraLai,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.BiTraLai,
                ChuyenBuocId = DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.TrinhHoSoToLapHoSo,
                NhomNhapLieu = "DonViPheDuyet",
                YeuCauLyDo = true,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.KhongPheDuyet,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.TrinhHoSo,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.DaTrinhPheDuyet,
                HanhDongId = DangKySeedIds.HanhDong.KhongPheDuyet,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.HoanThanh,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.KhongPheDuyet,
                ChuyenBuocId = DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.TrinhHoSoToHoanThanh,
                NhomNhapLieu = "DonViPheDuyet",
                YeuCauLyDo = true,
                LaKetThuc = true,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.CapNhatKetQua,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.PheDuyet,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.DaPheDuyet,
                HanhDongId = DangKySeedIds.HanhDong.CapNhatKetQua,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.CapNhatKetQua,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.DaCapNhatKetQua,
                ChuyenBuocId = DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.PheDuyetToCapNhatKetQua,
                NhomNhapLieu = "DonViSoanThao",
                YeuCauFileDinhKem = true,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.HoanThanh,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.CapNhatKetQua,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.DaCapNhatKetQua,
                HanhDongId = DangKySeedIds.HanhDong.HoanThanh,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.HoanThanh,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.HoanThanh,
                ChuyenBuocId = DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.CapNhatKetQuaToHoanThanh,
                NhomNhapLieu = "DonViSoanThao",
                LaKetThuc = true,
                TrangThai = true,
                CreatedAt = seedDate
            },
            new DangKyCauHinhChuyenTrangThai
            {
                Id = DangKySeedIds.CauHinhChuyenTrangThai.KhoiTaoQuyTrinhXayDung,
                QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
                BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.HoanThanh,
                TrangThaiHienTaiId = DangKySeedIds.TrangThai.HoanThanh,
                HanhDongId = DangKySeedIds.HanhDong.KhoiTaoQuyTrinhXayDung,
                BuocTiepTheoId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.HoanThanh,
                TrangThaiTiepTheoId = DangKySeedIds.TrangThai.DaChuyenQuyTrinhXayDung,
                NhomNhapLieu = "DonViSoanThao",
                LaKetThuc = true,
                TrangThai = true,
                CreatedAt = seedDate
            });
    }
}
