using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucDonViAppService(IDanhMucDonViRepository repository) : IDanhMucDonViAppService
{
    private readonly IDanhMucDonViRepository _repository = repository;

    public async Task<PagedResult<DanhMucDonViDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;
        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<DanhMucDonViDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToList(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<DanhMucDonViDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => (await _repository.GetByIdAsync(id, cancellationToken))?.ToDto();

    public async Task<IReadOnlyList<DanhMucDonViDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await _repository.GetAllAsync(cancellationToken)).Select(x => x.ToDto()).ToList();

    public Task<int> GetNextSortOrderAsync(Guid donViChuQuanId, CancellationToken cancellationToken = default)
        => _repository.GetNextSortOrderAsync(donViChuQuanId, cancellationToken);

    public async Task<(bool IsSuccess, string Message, DanhMucDonViDto? Data)> CreateAsync(UpsertDanhMucDonViRequest request, CancellationToken cancellationToken = default)
    {
        var validationMessage = Validate(request);
        if (validationMessage != null) return (false, validationMessage, null);
        if (await _repository.ExistsByNameAsync(request.TenDonVi.Trim(), null, cancellationToken))
            return (false, "Tên đơn vị đã tồn tại.", null);

        var created = await _repository.AddAsync(request.ToEntity(), cancellationToken);
        return (true, "Tạo mới đơn vị thành công.", created.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucDonViDto? Data)> UpdateAsync(Guid id, UpsertDanhMucDonViRequest request, CancellationToken cancellationToken = default)
    {
        var validationMessage = Validate(request);
        if (validationMessage != null) return (false, validationMessage, null);

        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null) return (false, "Không tìm thấy đơn vị.", null);
        if (await _repository.ExistsByNameAsync(request.TenDonVi.Trim(), id, cancellationToken))
            return (false, "Tên đơn vị đã tồn tại.", null);

        current.TenDonVi = request.TenDonVi.Trim();
        current.Level = request.Level;
        current.STTSapXep = request.STTSapXep;
        current.DonViChuQuanId = request.DonViChuQuanId;
        current.DiaChi = request.DiaChi?.Trim();
        current.MaQHNS = request.MaQHNS?.Trim();
        current.SoDienThoai = request.SoDienThoai?.Trim();
        current.ChucDanhQuanLy = request.ChucDanhQuanLy?.Trim();
        current.HoVaTenNguoiQuanLy = request.HoVaTenNguoiQuanLy?.Trim();
        current.PhanLoaiDonVi = request.PhanLoaiDonVi?.Trim();
        current.TinhNangThanhToan = request.TinhNangThanhToan;

        var updated = await _repository.UpdateAsync(current, cancellationToken);
        return (true, "Cập nhật đơn vị thành công.", updated.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted ? (true, "Xóa đơn vị thành công.") : (false, "Không tìm thấy đơn vị.");
    }

    private static string? Validate(UpsertDanhMucDonViRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenDonVi)) return "Tên đơn vị không được để trống.";
        if (request.STTSapXep < 0) return "Thứ tự sắp xếp không hợp lệ.";
        if (request.Level < 0) return "Level không hợp lệ.";
        return null;
    }
}

