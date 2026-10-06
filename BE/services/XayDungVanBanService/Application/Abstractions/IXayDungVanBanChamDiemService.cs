using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanChamDiemService
{
    Task<PagedResultDto<ChamDiemListItemDto>> GetListAsync(ChamDiemListRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChamDiemDto>> GetByHoSoAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<ChamDiemDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChamDiemDto> CreateAsync(Guid hoSoId, TaoChamDiemRequest request, CancellationToken cancellationToken = default);
    Task<ChamDiemDto?> TinhLaiAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChamDiemDto?> DieuChinhAsync(Guid id, Guid chiTietId, DieuChinhChamDiemRequest request, CancellationToken cancellationToken = default);
    Task<ChamDiemDto?> ChuyenTrangThaiAsync(Guid id, ChuyenTrangThaiChamDiemRequest request, string expectedCode, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChamDiemLichSuDto>?> GetLichSuAsync(Guid id, CancellationToken cancellationToken = default);
}
