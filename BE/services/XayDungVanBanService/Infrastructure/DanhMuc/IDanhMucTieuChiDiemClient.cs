namespace XayDungVanBanService.Infrastructure.DanhMuc;

public interface IDanhMucTieuChiDiemClient
{
    Task<IReadOnlyList<DanhMucTieuChiDiemItem>> GetActiveAsync(CancellationToken cancellationToken = default);
}

public sealed record DanhMucTieuChiDiemItem(Guid Id, string MaTieuChi, string TenTieuChi, decimal DiemToiDa, bool TrangThai, IReadOnlyList<DanhMucTieuChiDiemMucItem> Mucs);
public sealed record DanhMucTieuChiDiemMucItem(Guid Id, decimal? TuGiaTri, decimal? DenGiaTri, bool BaoGomTuGiaTri, bool BaoGomDenGiaTri, decimal Diem, string? NhanHienThi, bool TrangThai);
