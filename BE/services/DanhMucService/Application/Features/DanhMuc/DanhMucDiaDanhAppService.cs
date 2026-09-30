using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucDiaDanhAppService(IDanhMucDiaDanhRepository repository) : IDanhMucDiaDanhAppService
{
    private readonly IDanhMucDiaDanhRepository _repository = repository;

    public async Task<PagedResult<DanhMucDiaDanhDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<DanhMucDiaDanhDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<IReadOnlyList<DanhMucDiaDanhDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<IReadOnlyList<DanhMucDiaDanhDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetChildrenAsync(parentId, cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<DanhMucDiaDanhDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public Task<int> GetNextSortOrderAsync(Guid parentId, CancellationToken cancellationToken = default)
        => _repository.GetNextSortOrderAsync(parentId, cancellationToken);

    public async Task<(bool IsSuccess, string Message, DanhMucDiaDanhDto? Data)> CreateAsync(UpsertDanhMucDiaDanhRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TenDiaDanh))
        {
            return (false, "Tên địa danh không được để trống.", null);
        }

        var created = await _repository.AddAsync(request.ToEntity(), cancellationToken);
        return (true, "Tạo mới địa danh thành công.", created.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucDiaDanhDto? Data)> UpdateAsync(Guid id, UpsertDanhMucDiaDanhRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TenDiaDanh))
        {
            return (false, "Tên địa danh không được để trống.", null);
        }

        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy địa danh.", null);
        }

        current.TenDiaDanh = request.TenDiaDanh.Trim();
        current.Level = request.Level;
        current.STTSapXep = request.STTSapXep;
        current.DiaDanhCapTrenId = request.DiaDanhCapTrenId;

        var updated = await _repository.UpdateAsync(current, cancellationToken);
        return (true, "Cập nhật địa danh thành công.", updated.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteCascadeAsync(id, cancellationToken);
        return deleted
            ? (true, "Xóa địa danh thành công.")
            : (false, "Không tìm thấy địa danh.");
    }
}

