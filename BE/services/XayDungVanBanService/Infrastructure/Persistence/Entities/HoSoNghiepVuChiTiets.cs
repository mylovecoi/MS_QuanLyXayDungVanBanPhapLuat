namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class HoSoXayDungVanBanSoanThao
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public string? CanCuXayDung { get; set; }
    public string? PhamViDieuChinh { get; set; }
    public string? NoiDungChinhSach { get; set; }
    public string? HinhThucLayYKien { get; set; }
    public DateTime? TuNgayLayYKien { get; set; }
    public DateTime? DenNgayLayYKien { get; set; }
    public int? TongSoYKien { get; set; }
    public string? NoiDungTongHopTiepThuGiaiTrinh { get; set; }
    public Guid? LoaiTaiLieuTongHopYKienId { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
}

public sealed class HoSoXayDungVanBanTrinhThamDinh
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public Guid? FileDuThaoId { get; set; }
    public string? SoToTrinh { get; set; }
    public DateTime? NgayToTrinh { get; set; }
    public DateTime? NgayGuiThamDinh { get; set; }
    public Guid DonViNhanThamDinhId { get; set; }
    public string? NoiDungDeNghiThamDinh { get; set; }
    public DateTime? HanDeNghiTraKetQua { get; set; }
    public string? LyDoTrinhLai { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
}

public sealed class HoSoXayDungVanBanThamDinh
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public DateTime? NgayTiepNhan { get; set; }
    public string? HinhThucThamDinh { get; set; }
    public DateTime? NgayThamDinh { get; set; }
    public string? KetQuaThamDinh { get; set; }
    public string? NoiDungKetLuan { get; set; }
    public string? NoiDungYeuCauBoSung { get; set; }
    public DateTime? HanBoSung { get; set; }
    public DateTime? NgayGuiKetQua { get; set; }
    public Guid? NguoiKetLuanId { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
}

public sealed class HoSoXayDungVanBanTrinhPheDuyet
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public string CapTrinh { get; set; } = string.Empty;
    public string MucDichTrinh { get; set; } = string.Empty;
    public string? SoToTrinh { get; set; }
    public DateTime? NgayToTrinh { get; set; }
    public DateTime? NgayTrinh { get; set; }
    public string? NoiDungTrinh { get; set; }
    public Guid? DonViDongGuiId { get; set; }
    public string? TinhTrangXuLyBenNgoaiHeThong { get; set; }
    public DateTime? NgayCapNhatTinhTrang { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
}

public sealed class HoSoXayDungVanBanYKienUbnd
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public DateTime? NgayNhanYKien { get; set; }
    public int? TongSoThanhVienDuocLayYKien { get; set; }
    public int? SoDongY { get; set; }
    public int? SoKhongDongY { get; set; }
    public int? SoYKienKhac { get; set; }
    public string? KetLuanTongHop { get; set; }
    public string? NoiDungTongHop { get; set; }
    public string? NoiDungGiaiTrinh { get; set; }
    public bool CanBoSungHoSo { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
}

public sealed class HoSoXayDungVanBanThamTraHdnd
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public Guid? BanHdndThamTraId { get; set; }
    public DateTime? NgayTrinhThamTra { get; set; }
    public DateTime? NgayNhanKetQuaThamTra { get; set; }
    public string? KetQuaThamTra { get; set; }
    public string? NoiDungKienNghi { get; set; }
    public string? NoiDungTiepThuGiaiTrinh { get; set; }
    public DateTime? NgayNhanYKienThaoLuan { get; set; }
    public string? NoiDungTongHopYKienThaoLuan { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
}

public sealed class HoSoXayDungVanBanKetQuaBanHanh
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public string KetQua { get; set; } = string.Empty;
    public string? SoVanBan { get; set; }
    public DateTime? NgayBanHanh { get; set; }
    public Guid? CoQuanBanHanhId { get; set; }
    public Guid? NguoiKyId { get; set; }
    public string? ChucVuNguoiKy { get; set; }
    public DateTime? NgayCoHieuLuc { get; set; }
    public string? NoiDungKetQua { get; set; }
    public string? LyDoKhongThongQua { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
}
