namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

/// <summary>
/// Bản chỉ-đọc của danh mục trạng thái nghiệp vụ do DanhMucService quản lý.
/// Hồ sơ lưu Id của bảng này, vì vậy cần tra cứu trực tiếp để trả tên và màu hiển thị.
/// </summary>
public sealed class DanhMucTrangThaiNghiepVuQuyTrinh
{
    public Guid Id { get; set; }
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid BuocQuyTrinhId { get; set; }
    public string MaTrangThai { get; set; } = string.Empty;
    public string TenTrangThai { get; set; } = string.Empty;
    public string MaMauHex { get; set; } = string.Empty;
    public bool TrangThai { get; set; }
}
