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
