using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucVanBanAppService(IDanhMucVanBanRepository repository) : IDanhMucVanBanAppService
{
    private readonly IDanhMucVanBanRepository _repository = repository;

    public async Task<PagedResult<DanhMucVanBanDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<DanhMucVanBanDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToList(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<DanhMucVanBanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
        => _repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<(bool IsSuccess, string Message, DanhMucVanBanDto? Data)> CreateAsync(UpsertDanhMucVanBanRequest request, CancellationToken cancellationToken = default)
    {
        var validationMessage = Validate(request);
        if (validationMessage != null)
        {
            return (false, validationMessage, null);
        }

        if (await _repository.ExistsByNameAsync(request.TenLoaiVanBan.Trim(), null, cancellationToken))
        {
            return (false, "Tên loại văn bản đã tồn tại.", null);
        }

        var created = await _repository.AddAsync(request.ToEntity(), cancellationToken);
        return (true, "Tạo mới loại văn bản thành công.", created.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucVanBanDto? Data)> UpdateAsync(Guid id, UpsertDanhMucVanBanRequest request, CancellationToken cancellationToken = default)
    {
        var validationMessage = Validate(request);
        if (validationMessage != null)
        {
            return (false, validationMessage, null);
        }

        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy loại văn bản.", null);
        }

        if (await _repository.ExistsByNameAsync(request.TenLoaiVanBan.Trim(), id, cancellationToken))
        {
            return (false, "Tên loại văn bản đã tồn tại.", null);
        }

        current.TenLoaiVanBan = request.TenLoaiVanBan.Trim();
        current.CapChinhQuyen = request.CapChinhQuyen.Trim();
        current.ChuTheBanHanh = request.ChuTheBanHanh.Trim();
        current.KyHieuMau = request.KyHieuMau?.Trim();
        current.ThuTuSapXep = request.ThuTuSapXep;
        current.TrangThai = request.TrangThai;
        current.MoTa = request.MoTa?.Trim();
        current.GhiChu = request.GhiChu?.Trim();

        var updated = await _repository.UpdateAsync(current, cancellationToken);
        return (true, "Cập nhật loại văn bản thành công.", updated.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted
            ? (true, "Xóa loại văn bản thành công.")
            : (false, "Không tìm thấy loại văn bản.");
    }

    private static string? Validate(UpsertDanhMucVanBanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenLoaiVanBan))
        {
            return "Tên loại văn bản không được để trống.";
        }

        if (string.IsNullOrWhiteSpace(request.CapChinhQuyen))
        {
            return "Cấp chính quyền không được để trống.";
        }

        if (string.IsNullOrWhiteSpace(request.ChuTheBanHanh))
        {
            return "Chủ thể ban hành không được để trống.";
        }

        return null;
    }
}

