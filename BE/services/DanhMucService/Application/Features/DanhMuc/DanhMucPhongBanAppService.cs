using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucPhongBanAppService(IDanhMucPhongBanRepository repository) : IDanhMucPhongBanAppService
{
    private readonly IDanhMucPhongBanRepository _repository = repository;

    public async Task<PagedResult<DanhMucPhongBanDto>> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        int? loaiPhongBan,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, donViId, loaiPhongBan, cancellationToken);
        return new PagedResult<DanhMucPhongBanDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<IReadOnlyList<DanhMucPhongBanDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<DanhMucPhongBanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<(bool IsSuccess, string Message, DanhMucPhongBanDto? Data)> CreateAsync(UpsertDanhMucPhongBanRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(request, null, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        var entity = request.ToEntity();
        await _repository.AddAsync(entity, cancellationToken);
        var created = await _repository.GetByIdAsync(entity.Id, cancellationToken);

        return (true, "Tạo mới phòng ban thành công.", created?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucPhongBanDto? Data)> UpdateAsync(Guid id, UpsertDanhMucPhongBanRequest request, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy phòng ban cần cập nhật.", null);
        }

        var validation = await ValidateAsync(request, id, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        current.TenPhongBan = request.TenPhongBan.Trim();
        current.MaPhongBan = request.MaPhongBan.Trim();
        current.LoaiPhongBan = request.LoaiPhongBan;
        current.DanhMucDonViId = request.DanhMucDonViId;

        await _repository.UpdateAsync(current, cancellationToken);
        var updated = await _repository.GetByIdAsync(id, cancellationToken);

        return (true, "Cập nhật phòng ban thành công.", updated?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy phòng ban cần xóa.");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return (true, "Xóa phòng ban thành công.");
    }

    private async Task<(bool IsSuccess, string Message)> ValidateAsync(UpsertDanhMucPhongBanRequest request, Guid? ignoreId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenPhongBan))
        {
            return (false, "Tên phòng ban là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(request.MaPhongBan))
        {
            return (false, "Mã phòng ban là bắt buộc.");
        }

        if (request.DanhMucDonViId == Guid.Empty)
        {
            return (false, "Đơn vị là bắt buộc.");
        }

        var existed = await _repository.ExistsByCodeAsync(request.MaPhongBan.Trim(), ignoreId, cancellationToken);
        if (existed)
        {
            return (false, "Mã phòng ban đã tồn tại.");
        }

        return (true, "Dữ liệu hợp lệ.");
    }
}

