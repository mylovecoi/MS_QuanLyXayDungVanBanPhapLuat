namespace ThiHanhPhapLuatService.Infrastructure.DanhMuc;

public interface IDanhMucTrangThaiClient
{
    Task<DanhMucTrangThaiItem?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed record DanhMucTrangThaiItem(Guid Id, string NhomTrangThai, string MaTrangThai, bool TrangThai);
