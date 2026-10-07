namespace ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

public sealed class KeHoachThiHanhPhapLuat : BaseEntity
{
    public string MaKeHoach { get; set; } = null!;
    public string TenKeHoach { get; set; } = null!;
    public int Nam { get; set; }
    public DateOnly TuNgay { get; set; }
    public DateOnly DenNgay { get; set; }
    public Guid DonViChuTriId { get; set; }
    public Guid? NguoiPhuTrachId { get; set; }
    public Guid TrangThaiId { get; set; }
    public string? PhamVi { get; set; }
    public string? MucTieu { get; set; }
    public ICollection<NoiDungKeHoach> NoiDungs { get; set; } = [];
}

public sealed class KeHoachCanCuPhapLy : BaseEntity
{
    public Guid KeHoachId { get; set; }
    public Guid? VanBanId { get; set; }
    public string? SoKyHieu { get; set; }
    public string TrichYeu { get; set; } = null!;
    public int ThuTu { get; set; }
}

public sealed class NoiDungKeHoach : BaseEntity
{
    public Guid KeHoachId { get; set; }
    public string MaNoiDung { get; set; } = null!;
    public string TenNoiDung { get; set; } = null!;
    public string? NoiDung { get; set; }
    public decimal? ChiTieu { get; set; }
    public string? DonViTinh { get; set; }
    public DateOnly HanHoanThanh { get; set; }
    public int ThuTu { get; set; }
    public Guid TrangThaiId { get; set; }
    public decimal TyLeHoanThanh { get; set; }
    public KeHoachThiHanhPhapLuat KeHoach { get; set; } = null!;
}

public sealed class PhanCongThiHanh : BaseEntity
{
    public Guid NoiDungKeHoachId { get; set; }
    public Guid DonViDuocGiaoId { get; set; }
    public Guid? CanBoDuocGiaoId { get; set; }
    public string VaiTro { get; set; } = null!;
    public DateOnly HanThucHien { get; set; }
    public int MucDoUuTien { get; set; }
}

public sealed class BaoCaoTienDoThiHanh : BaseEntity
{
    public Guid NoiDungKeHoachId { get; set; }
    public Guid? PhanCongThiHanhId { get; set; }
    public string KyBaoCao { get; set; } = null!;
    public decimal TyLeHoanThanh { get; set; }
    public string? KetQua { get; set; }
    public string? KhoKhan { get; set; }
    public string? KienNghi { get; set; }
    public Guid TrangThaiId { get; set; }
    public DateTime NgayBaoCao { get; set; }
}

public sealed class TepDinhKemThiHanh : BaseEntity
{
    public Guid? KeHoachId { get; set; }
    public Guid? NoiDungKeHoachId { get; set; }
    public Guid? BaoCaoTienDoThiHanhId { get; set; }
    public string LoaiTaiLieu { get; set; } = null!;
    public string TenTep { get; set; } = null!;
    public string DuongDan { get; set; } = null!;
    public int PhienBan { get; set; } = 1;
    public bool IsCurrent { get; set; } = true;
}

public sealed class DanhGiaThiHanh : BaseEntity
{
    public Guid NoiDungKeHoachId { get; set; }
    public Guid? BaoCaoTienDoThiHanhId { get; set; }
    public string KetQuaDanhGia { get; set; } = null!;
    public string? NhanXet { get; set; }
    public Guid TrangThaiSauId { get; set; }
    public Guid NguoiDanhGiaId { get; set; }
    public DateTime NgayDanhGia { get; set; }
}

public sealed class YeuCauBoSungThiHanh : BaseEntity
{
    public Guid NoiDungKeHoachId { get; set; }
    public Guid? BaoCaoTienDoThiHanhId { get; set; }
    public string NoiDungYeuCau { get; set; } = null!;
    public DateOnly HanBoSung { get; set; }
    public string TrangThai { get; set; } = null!;
    public DateTime? NgayHoanThanh { get; set; }
}

public sealed class BaoCaoTongHopThiHanh : BaseEntity
{
    public string MaBaoCao { get; set; } = null!;
    public Guid KeHoachId { get; set; }
    public string KyBaoCao { get; set; } = null!;
    public DateOnly TuNgay { get; set; }
    public DateOnly DenNgay { get; set; }
    public Guid TrangThaiId { get; set; }
    public string? SoLieuTongHopJson { get; set; }
    public DateTime? NgayChot { get; set; }
    public Guid? NguoiChotId { get; set; }
}

public sealed class BaoCaoTongHopChiTiet : BaseEntity
{
    public Guid BaoCaoTongHopThiHanhId { get; set; }
    public Guid NoiDungKeHoachId { get; set; }
    public int LanChot { get; set; }
    public Guid TrangThaiId { get; set; }
    public decimal TyLeHoanThanh { get; set; }
    public string? KetQua { get; set; }
    public string? KhoKhan { get; set; }
    public string? KienNghi { get; set; }
}

public sealed class LichSuXuLyThiHanh
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KeHoachId { get; set; }
    public Guid? NoiDungKeHoachId { get; set; }
    public string HanhDong { get; set; } = null!;
    public Guid? TrangThaiTruocId { get; set; }
    public Guid? TrangThaiSauId { get; set; }
    public string? NoiDung { get; set; }
    public DateTime ThoiGianXuLy { get; set; } = DateTime.UtcNow;
    public Guid NguoiXuLyId { get; set; }
    public Guid? DonViXuLyId { get; set; }
}

public sealed class LichSuNhacViecThiHanh
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KeHoachId { get; set; }
    public Guid? NoiDungKeHoachId { get; set; }
    public string LoaiNhacViec { get; set; } = null!;
    public Guid? NguoiNhanId { get; set; }
    public Guid? DonViNhanId { get; set; }
    public DateTime ThoiGianGui { get; set; } = DateTime.UtcNow;
    public string KenhGui { get; set; } = null!;
    public string KetQua { get; set; } = null!;
}
