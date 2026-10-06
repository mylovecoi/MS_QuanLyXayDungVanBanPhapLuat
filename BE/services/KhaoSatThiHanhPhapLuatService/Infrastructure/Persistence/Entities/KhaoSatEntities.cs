namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

public sealed class CuocKhaoSat : BaseEntity
{
    public string MaCuocKhaoSat { get; set; } = null!;
    public string TenCuocKhaoSat { get; set; } = null!;
    public string? MucDich { get; set; }
    public string? PhamVi { get; set; }
    public Guid? LinhVucId { get; set; }
    public Guid? VanBanId { get; set; }
    public Guid DonViChuTriId { get; set; }
    public DateOnly TuNgay { get; set; }
    public DateOnly DenNgay { get; set; }
    public Guid TrangThaiId { get; set; }
    public Guid? KeHoachThiHanhPhapLuatId { get; set; }
    public Guid? NoiDungKeHoachId { get; set; }
}

public sealed class NhomDoiTuongKhaoSat : BaseEntity
{
    public Guid CuocKhaoSatId { get; set; }
    public string MaNhom { get; set; } = null!;
    public string TenNhom { get; set; } = null!;
    public int ThuTu { get; set; }
}

public sealed class MauPhieuKhaoSat : BaseEntity
{
    public Guid CuocKhaoSatId { get; set; }
    public Guid NhomDoiTuongKhaoSatId { get; set; }
    public int PhienBan { get; set; }
    public string TenFile { get; set; } = null!;
    public string DuongDanFile { get; set; } = null!;
    public string MaHash { get; set; } = null!;
    public bool DaPhatHanh { get; set; }
}

public sealed class CauHoiThongKe : BaseEntity
{
    public Guid CuocKhaoSatId { get; set; }
    public string MaCauHoiThongKe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public string LoaiCauHoi { get; set; } = null!;
}

public sealed class CauHoiMauPhieu : BaseEntity
{
    public Guid MauPhieuKhaoSatId { get; set; }
    public Guid CauHoiThongKeId { get; set; }
    public string MaCauHoi { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public string LoaiCauHoi { get; set; } = null!;
    public int ThuTu { get; set; }
}

public sealed class LuaChonTraLoi : BaseEntity
{
    public Guid CauHoiMauPhieuId { get; set; }
    public string MaLuaChon { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public int ThuTu { get; set; }
}

public sealed class DoiTuongKhaoSat : BaseEntity
{
    public Guid CuocKhaoSatId { get; set; }
    public Guid NhomDoiTuongKhaoSatId { get; set; }
    public Guid MauPhieuKhaoSatId { get; set; }
    public Guid DonViId { get; set; }
    public Guid? CanBoId { get; set; }
    public DateOnly HanNop { get; set; }
    public Guid TrangThaiId { get; set; }
}

public sealed class PhieuNopKhaoSat : BaseEntity
{
    public Guid DoiTuongKhaoSatId { get; set; }
    public Guid MauPhieuKhaoSatId { get; set; }
    public string TenFile { get; set; } = null!;
    public string DuongDanFile { get; set; } = null!;
    public string MaHash { get; set; } = null!;
    public Guid TrangThaiId { get; set; }
    public DateTime? NgayImport { get; set; }
}

public sealed class CauTraLoiKhaoSat : BaseEntity
{
    public Guid PhieuNopKhaoSatId { get; set; }
    public Guid CauHoiThongKeId { get; set; }
    public Guid CauHoiMauPhieuId { get; set; }
    public Guid? LuaChonTraLoiId { get; set; }
    public string? GiaTriText { get; set; }
    public decimal? GiaTriSo { get; set; }
    public decimal? SoLuong { get; set; }
    public decimal? TongSoTraLoi { get; set; }
}

public sealed class LoiImportKhaoSat : BaseEntity
{
    public Guid PhieuNopKhaoSatId { get; set; }
    public string ViTri { get; set; } = null!;
    public string MaCauHoi { get; set; } = null!;
    public string NoiDungLoi { get; set; } = null!;
}

public sealed class LichSuXuLyKhaoSat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CuocKhaoSatId { get; set; }
    public Guid? DoiTuongKhaoSatId { get; set; }
    public Guid? PhieuNopKhaoSatId { get; set; }
    public string HanhDong { get; set; } = null!;
    public string? NoiDung { get; set; }
    public DateTime ThoiGianXuLy { get; set; } = DateTime.UtcNow;
    public Guid NguoiXuLyId { get; set; }
    public Guid? DonViXuLyId { get; set; }
}
