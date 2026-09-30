using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucLinhVucAppService(IDanhMucLinhVucRepository repository) : IDanhMucLinhVucAppService
{
    private readonly IDanhMucLinhVucRepository _repository = repository;

    public async Task<PagedResult<DanhMucLinhVucDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<DanhMucLinhVucDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<IReadOnlyList<DanhMucLinhVucDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<DanhMucLinhVucDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
        => _repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<(bool IsSuccess, string Message, DanhMucLinhVucDto? Data)> CreateAsync(UpsertDanhMucLinhVucRequest request, CancellationToken cancellationToken = default)
    {
        var validation = Validate(request);
        if (validation != null)
        {
            return (false, validation, null);
        }

        if (await _repository.ExistsByCodeAsync(request.MaLinhVuc.Trim(), null, cancellationToken))
        {
            return (false, "Mã lĩnh vực đã tồn tại.", null);
        }

        var created = await _repository.AddAsync(request.ToEntity(), cancellationToken);
        return (true, "Tạo mới lĩnh vực thành công.", created.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucLinhVucDto? Data)> UpdateAsync(Guid id, UpsertDanhMucLinhVucRequest request, CancellationToken cancellationToken = default)
    {
        var validation = Validate(request);
        if (validation != null)
        {
            return (false, validation, null);
        }

        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy lĩnh vực.", null);
        }

        if (await _repository.ExistsByCodeAsync(request.MaLinhVuc.Trim(), id, cancellationToken))
        {
            return (false, "Mã lĩnh vực đã tồn tại.", null);
        }

        current.MaLinhVuc = request.MaLinhVuc.Trim();
        current.TenLinhVuc = request.TenLinhVuc.Trim();
        current.ThuTuSapXep = request.ThuTuSapXep;
        current.TrangThai = request.TrangThai;
        current.MoTa = request.MoTa?.Trim();
        current.GhiChu = request.GhiChu?.Trim();

        var updated = await _repository.UpdateAsync(current, cancellationToken);
        return (true, "Cập nhật lĩnh vực thành công.", updated.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted
            ? (true, "Xóa lĩnh vực thành công.")
            : (false, "Không tìm thấy lĩnh vực.");
    }

    private static string? Validate(UpsertDanhMucLinhVucRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MaLinhVuc))
        {
            return "Mã lĩnh vực không được để trống.";
        }

        if (string.IsNullOrWhiteSpace(request.TenLinhVuc))
        {
            return "Tên lĩnh vực không được để trống.";
        }

        return null;
    }
}

