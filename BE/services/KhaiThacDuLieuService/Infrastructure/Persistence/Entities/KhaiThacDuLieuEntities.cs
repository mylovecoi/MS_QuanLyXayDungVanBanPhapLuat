using System.ComponentModel.DataAnnotations;

namespace KhaiThacDuLieuService.Infrastructure.Persistence.Entities;

public sealed class CauHinhCanhBaoKhaiThacDuLieu : BaseEntity
{
    [Required, MaxLength(100)]
    public string MaCanhBao { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string TenCanhBao { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string NhomCanhBao { get; set; } = string.Empty;

    public int SoNgayCanhBaoTruocHan { get; set; } = 3;

    [Required, MaxLength(50)]
    public string MucDoMacDinh { get; set; } = "TRUNG_BINH";

    [MaxLength(500)]
    public string? KenhThongBao { get; set; }

    public bool TrangThai { get; set; } = true;
}

public sealed class CanhBaoKhaiThacDuLieu : BaseEntity
{
    [Required, MaxLength(100)]
    public string MaCanhBao { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string NhomCanhBao { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string DoiTuongNguon { get; set; } = string.Empty;

    public Guid DoiTuongNguonId { get; set; }

    [Required, MaxLength(500)]
    public string TieuDe { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string NoiDung { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string MucDo { get; set; } = "TRUNG_BINH";

    [Required, MaxLength(50)]
    public string TrangThaiXuLy { get; set; } = "MOI";

    public DateTime? HanXuLy { get; set; }
    public DateTime NgayPhatSinh { get; set; } = DateTime.UtcNow;
    public Guid? NguoiNhanId { get; set; }
    public Guid? DonViNhanId { get; set; }
    public Guid? NguoiXuLyId { get; set; }
    public DateTime? NgayXem { get; set; }
    public DateTime? NgayXuLy { get; set; }

    [MaxLength(2000)]
    public string? GhiChuXuLy { get; set; }

    public ICollection<CanhBaoNhacViec> NhacViecs { get; set; } = new List<CanhBaoNhacViec>();
    public ICollection<CanhBaoLichSuXuLy> LichSuXuLys { get; set; } = new List<CanhBaoLichSuXuLy>();
}

public sealed class CanhBaoNhacViec : BaseEntity
{
    public Guid CanhBaoId { get; set; }
    public CanhBaoKhaiThacDuLieu? CanhBao { get; set; }

    [Required, MaxLength(500)]
    public string TieuDe { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string NoiDung { get; set; } = string.Empty;

    public Guid NguoiGiaoId { get; set; }
    public Guid? DonViGiaoId { get; set; }
    public Guid? NguoiNhanId { get; set; }
    public Guid? DonViNhanId { get; set; }
    public DateTime? HanXuLy { get; set; }
    public DateTime? ThoiGianNhac { get; set; }

    [Required, MaxLength(50)]
    public string MucDoUuTien { get; set; } = "TRUNG_BINH";

    [Required, MaxLength(50)]
    public string TrangThai { get; set; } = "DA_GUI";

    public DateTime NgayGui { get; set; } = DateTime.UtcNow;
    public DateTime? NgayXem { get; set; }
    public DateTime? NgayHoanThanh { get; set; }

    [MaxLength(2000)]
    public string? GhiChuHoanThanh { get; set; }

    public ICollection<CanhBaoLichSuXuLy> LichSuXuLys { get; set; } = new List<CanhBaoLichSuXuLy>();
}

public sealed class CanhBaoLichSuXuLy : BaseEntity
{
    public Guid CanhBaoId { get; set; }
    public CanhBaoKhaiThacDuLieu? CanhBao { get; set; }
    public Guid? NhacViecId { get; set; }
    public CanhBaoNhacViec? NhacViec { get; set; }

    [Required, MaxLength(100)]
    public string HanhDong { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? NoiDung { get; set; }

    public Guid? NguoiThucHienId { get; set; }
    public Guid? DonViThucHienId { get; set; }

    [MaxLength(50)]
    public string? TrangThaiTruoc { get; set; }

    [MaxLength(50)]
    public string? TrangThaiSau { get; set; }

    public DateTime ThoiGian { get; set; } = DateTime.UtcNow;
}

public sealed class DongBoKhaiThacDuLieuLog : BaseEntity
{
    [Required, MaxLength(100)]
    public string NguonDuLieu { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LoaiDongBo { get; set; } = string.Empty;

    public DateTime BatDauLuc { get; set; } = DateTime.UtcNow;
    public DateTime? KetThucLuc { get; set; }

    [Required, MaxLength(50)]
    public string TrangThai { get; set; } = "DANG_CHAY";

    public int SoBanGhi { get; set; }
    public string? Loi { get; set; }
}

public sealed class DashboardWidget : BaseEntity
{
    [Required, MaxLength(100)]
    public string MaWidget { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string TenWidget { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string NhomWidget { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LoaiWidget { get; set; } = "metric";

    [MaxLength(1000)]
    public string? MoTa { get; set; }

    public bool TrangThai { get; set; } = true;

    [MaxLength(4000)]
    public string? CauHinhMacDinhJson { get; set; }

    public ICollection<DashboardWidgetCauHinh> CauHinhs { get; set; } = new List<DashboardWidgetCauHinh>();
}

public sealed class DashboardWidgetCauHinh : BaseEntity
{
    public Guid DashboardWidgetId { get; set; }
    public DashboardWidget? DashboardWidget { get; set; }

    public Guid? NguoiDungId { get; set; }
    public Guid? NhomQuyenId { get; set; }
    public Guid? DonViId { get; set; }

    public int ThuTu { get; set; }
    public bool HienThi { get; set; } = true;

    [Required, MaxLength(50)]
    public string DangHienThi { get; set; } = "metric";

    [Required, MaxLength(50)]
    public string KichThuoc { get; set; } = "medium";

    [MaxLength(4000)]
    public string? CauHinhBoLocJson { get; set; }

    public int? TuDongLamMoiPhut { get; set; }
}

public sealed class DangKyXayDungVanBanTraCuu : BaseEntity
{
    [Required, MaxLength(50)]
    public string MaHoSo { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TenHoSo { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TenVanBanDuKien { get; set; } = string.Empty;

    public Guid LoaiVanBanId { get; set; }
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid BuocHienTaiId { get; set; }
    public Guid TrangThaiHoSoId { get; set; }
    public Guid DonViSoanThaoId { get; set; }
    public Guid DonViPheDuyetId { get; set; }
    public int NamDangKy { get; set; }
    public DateTime? DuKienThoiGianTrinh { get; set; }
    public bool DaKhoiTaoQuyTrinhXayDung { get; set; }
    public Guid? HoSoXayDungVanBanId { get; set; }
}

public sealed class DangKyTrangThaiHoSoTraCuu : BaseEntity
{
    [Required, MaxLength(100)]
    public string MaTrangThai { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string TenTrangThai { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MauHienThi { get; set; }

    public int ThuTuSapXep { get; set; }
    public bool LaTrangThaiKetThuc { get; set; }
    public bool TrangThai { get; set; } = true;
}

public sealed class XayDungVanBanHoSoTraCuu : BaseEntity
{
    [Required, MaxLength(50)]
    public string MaHoSo { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TenHoSo { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TenDuThaoVanBan { get; set; } = string.Empty;

    public Guid DanhMucVanBanId { get; set; }
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid BuocHienTaiId { get; set; }
    public Guid TrangThaiHoSoId { get; set; }
    public Guid DonViChuTriSoanThaoId { get; set; }
    public Guid? NguoiPhuTrachId { get; set; }
    public int NamXayDung { get; set; }
    public DateTime? ThoiGianDuKienBatDau { get; set; }
    public DateTime? ThoiGianDuKienHoanThanh { get; set; }
    public string? MoTa { get; set; }
    public Guid? HoSoDangKyXayDungVanBanId { get; set; }
}

public sealed class XayDungVanBanBoHoSoTraCuu : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }
    public Guid BuocQuyTrinhId { get; set; }
    public int LoaiBoHoSo { get; set; }
    public int TrangThai { get; set; }
    public int LanXuLy { get; set; }
    public Guid? BoHoSoNguonId { get; set; }
    public DateTime NgayTao { get; set; }
    public DateTime? NgayGui { get; set; }
    public DateTime? NgayHoanThanh { get; set; }
    public Guid NguoiLapId { get; set; }
    public Guid DonViLapId { get; set; }
    public string? LyDoTraLai { get; set; }
    public string? NoiDungGhiChu { get; set; }
}

public sealed class DanhMucDonViTraCuu : BaseEntity
{
    [Required, MaxLength(500)]
    public string TenDonVi { get; set; } = string.Empty;
}

public sealed class ThiHanhPhapLuatKeHoachTraCuu : BaseEntity
{
    [Required, MaxLength(50)]
    public string MaKeHoach { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TenKeHoach { get; set; } = string.Empty;

    public int Nam { get; set; }
    public Guid DonViChuTriId { get; set; }
    public Guid? NguoiPhuTrachId { get; set; }
    public Guid TrangThaiId { get; set; }
}

public sealed class ThiHanhPhapLuatNoiDungTraCuu : BaseEntity
{
    public Guid KeHoachId { get; set; }

    [Required, MaxLength(100)]
    public string MaNoiDung { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TenNoiDung { get; set; } = string.Empty;

    public DateOnly HanHoanThanh { get; set; }
    public Guid TrangThaiId { get; set; }
    public decimal TyLeHoanThanh { get; set; }
}

public sealed class ThiHanhPhapLuatPhanCongTraCuu : BaseEntity
{
    public Guid NoiDungKeHoachId { get; set; }
    public Guid DonViDuocGiaoId { get; set; }
    public Guid? CanBoDuocGiaoId { get; set; }

    [Required, MaxLength(100)]
    public string VaiTro { get; set; } = string.Empty;

    public DateOnly HanThucHien { get; set; }
    public int MucDoUuTien { get; set; }
}
