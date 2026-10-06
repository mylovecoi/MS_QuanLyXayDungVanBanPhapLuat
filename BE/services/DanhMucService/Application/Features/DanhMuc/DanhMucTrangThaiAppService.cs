using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucTrangThaiAppService(IDanhMucTrangThaiRepository repository) : IDanhMucTrangThaiAppService
{
    private readonly IDanhMucTrangThaiRepository _repository = repository;

    public async Task<PagedResult<DanhMucTrangThaiDto>> GetPagedAsync(
        string? search,
        string? nhomTrangThai,
        int pageSize,
        int pageCurrent,
        CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

        var result = await _repository.GetPagedAsync(search, nhomTrangThai, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<DanhMucTrangThaiDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToList(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<DanhMucTrangThaiDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
        => _repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<(bool IsSuccess, string Message, DanhMucTrangThaiDto? Data)> CreateAsync(
        UpsertDanhMucTrangThaiRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationMessage = Validate(request);
        if (validationMessage != null)
        {
            return (false, validationMessage, null);
        }

        if (await _repository.ExistsByCodeAsync(request.NhomTrangThai.Trim(), request.MaTrangThai.Trim(), null, cancellationToken))
        {
            return (false, "Mã trạng thái đã tồn tại.", null);
        }

        var created = await _repository.AddAsync(request.ToEntity(), cancellationToken);
        return (true, "Tạo mới trạng thái thành công.", created.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucTrangThaiDto? Data)> UpdateAsync(
        Guid id,
        UpsertDanhMucTrangThaiRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationMessage = Validate(request);
        if (validationMessage != null)
        {
            return (false, validationMessage, null);
        }

        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy trạng thái.", null);
        }

        if (await _repository.ExistsByCodeAsync(request.NhomTrangThai.Trim(), request.MaTrangThai.Trim(), id, cancellationToken))
        {
            return (false, "Mã trạng thái đã tồn tại.", null);
        }

        current.NhomTrangThai = request.NhomTrangThai.Trim();
        current.MaTrangThai = request.MaTrangThai.Trim();
        current.TenTrangThai = request.TenTrangThai.Trim();
        current.MaMauHex = request.MaMauHex.Trim();
        current.ThuTuSapXep = request.ThuTuSapXep;
        current.TrangThai = request.TrangThai;
        current.MoTa = request.MoTa?.Trim();
        current.GhiChu = request.GhiChu?.Trim();

        var updated = await _repository.UpdateAsync(current, cancellationToken);
        return (true, "Cập nhật trạng thái thành công.", updated.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted
            ? (true, "Xóa trạng thái thành công.")
            : (false, "Không tìm thấy trạng thái.");
    }

    private static string? Validate(UpsertDanhMucTrangThaiRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NhomTrangThai))
        {
            return "Nhóm trạng thái không được để trống.";
        }

        if (string.IsNullOrWhiteSpace(request.MaTrangThai))
        {
            return "Mã trạng thái không được để trống.";
        }

        if (string.IsNullOrWhiteSpace(request.TenTrangThai))
        {
            return "Tên trạng thái không được để trống.";
        }

        if (string.IsNullOrWhiteSpace(request.MaMauHex))
        {
            return "Mã màu hiển thị không được để trống.";
        }

        return null;
    }
}

