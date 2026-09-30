using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucTieuChiDiemAppService(IDanhMucTieuChiDiemRepository repository) : IDanhMucTieuChiDiemAppService
{
    private readonly IDanhMucTieuChiDiemRepository _repository = repository;

    public async Task<PagedResult<DanhMucTieuChiDiemDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<DanhMucTieuChiDiemDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<IReadOnlyList<DanhMucTieuChiDiemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<DanhMucTieuChiDiemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
        => _repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<(bool IsSuccess, string Message, DanhMucTieuChiDiemDto? Data)> CreateAsync(UpsertDanhMucTieuChiDiemRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedRequest = NormalizeRequest(request);
        var validation = await ValidateAsync(normalizedRequest, null, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        var entity = normalizedRequest.ToEntity();
        await _repository.AddAsync(entity, cancellationToken);
        var created = await _repository.GetByIdAsync(entity.Id, cancellationToken);
        return (true, "Tạo mới tiêu chí điểm thành công.", created?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucTieuChiDiemDto? Data)> UpdateAsync(Guid id, UpsertDanhMucTieuChiDiemRequest request, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy tiêu chí điểm cần cập nhật.", null);
        }

        var normalizedRequest = NormalizeRequest(request);
        var validation = await ValidateAsync(normalizedRequest, id, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        current.MaTieuChi = normalizedRequest.MaTieuChi.Trim().ToUpperInvariant();
        current.TenTieuChi = normalizedRequest.TenTieuChi.Trim();
        current.LoaiTieuChi = normalizedRequest.LoaiTieuChi.Trim().ToUpperInvariant();
        current.KieuGiaTri = normalizedRequest.KieuGiaTri.Trim().ToUpperInvariant();
        current.DonViGiaTri = normalizedRequest.DonViGiaTri.Trim().ToUpperInvariant();
        current.ThuTuSapXep = normalizedRequest.ThuTuSapXep;
        current.DiemToiDa = normalizedRequest.DiemToiDa;
        current.TrangThai = normalizedRequest.TrangThai;
        current.MoTa = string.IsNullOrWhiteSpace(normalizedRequest.MoTa) ? null : normalizedRequest.MoTa.Trim();
        current.GhiChu = string.IsNullOrWhiteSpace(normalizedRequest.GhiChu) ? null : normalizedRequest.GhiChu.Trim();
        current.Mucs = normalizedRequest.Mucs.Select(x => x.ToEntity()).ToList();

        await _repository.UpdateAsync(current, cancellationToken);
        var updated = await _repository.GetByIdAsync(id, cancellationToken);
        return (true, "Cập nhật tiêu chí điểm thành công.", updated?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy tiêu chí điểm cần xóa.");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return (true, "Xóa tiêu chí điểm thành công.");
    }

    private async Task<(bool IsSuccess, string Message)> ValidateAsync(UpsertDanhMucTieuChiDiemRequest request, Guid? ignoreId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MaTieuChi))
        {
            return (false, "Mã tiêu chí là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(request.TenTieuChi))
        {
            return (false, "Tên tiêu chí là bắt buộc.");
        }

        if (request.DiemToiDa < 0)
        {
            return (false, "Điểm tối đa phải lớn hơn hoặc bằng 0.");
        }

        if (request.Mucs.Count == 0)
        {
            return (false, "Phải khai báo ít nhất 1 mức điểm.");
        }

        var existed = await _repository.ExistsByCodeAsync(request.MaTieuChi.Trim().ToUpperInvariant(), ignoreId, cancellationToken);
        if (existed)
        {
            return (false, "Mã tiêu chí đã tồn tại.");
        }

        var activeRows = request.Mucs.Where(x => x.TrangThai).OrderBy(x => x.TuGiaTri).ToList();
        if (activeRows.Count == 0)
        {
            return (false, "Phải có ít nhất 1 mức điểm đang kích hoạt.");
        }

        for (var i = 0; i < activeRows.Count; i++)
        {
            var item = activeRows[i];
            if (item.Diem < 0)
            {
                return (false, $"Điểm tại dòng mức {i + 1} phải lớn hơn hoặc bằng 0.");
            }

            if (item.Diem > request.DiemToiDa)
            {
                return (false, $"Điểm tại dòng mức {i + 1} không được vượt quá điểm tối đa.");
            }

            if (item.TuGiaTri.HasValue && item.DenGiaTri.HasValue && item.TuGiaTri > item.DenGiaTri)
            {
                return (false, $"Khoảng giá trị tại dòng mức {i + 1} không hợp lệ.");
            }

            for (var j = i + 1; j < activeRows.Count; j++)
            {
                if (IsOverlap(activeRows[i], activeRows[j]))
                {
                    return (false, $"Các mức điểm dòng {i + 1} và {j + 1} đang bị chồng lấn khoảng giá trị.");
                }
            }
        }

        return (true, "Dữ liệu hợp lệ.");
    }

    private static UpsertDanhMucTieuChiDiemRequest NormalizeRequest(UpsertDanhMucTieuChiDiemRequest request)
    {
        request.Mucs ??= new List<UpsertDanhMucTieuChiDiemMucRequest>();
        request.Mucs = request.Mucs
            .Where(x => x != null)
            .Select((x, index) =>
            {
                x.ThuTuSapXep = x.ThuTuSapXep <= 0 ? index + 1 : x.ThuTuSapXep;
                x.NhanHienThi = string.IsNullOrWhiteSpace(x.NhanHienThi) ? null : x.NhanHienThi.Trim();
                x.GhiChu = string.IsNullOrWhiteSpace(x.GhiChu) ? null : x.GhiChu.Trim();
                return x;
            })
            .OrderBy(x => x.ThuTuSapXep)
            .ToList();

        return request;
    }

    private static bool IsOverlap(UpsertDanhMucTieuChiDiemMucRequest first, UpsertDanhMucTieuChiDiemMucRequest second)
    {
        var firstStart = first.TuGiaTri ?? decimal.MinValue;
        var firstEnd = first.DenGiaTri ?? decimal.MaxValue;
        var secondStart = second.TuGiaTri ?? decimal.MinValue;
        var secondEnd = second.DenGiaTri ?? decimal.MaxValue;

        if (firstEnd < secondStart || secondEnd < firstStart)
        {
            return false;
        }

        if (firstEnd == secondStart)
        {
            return first.BaoGomDenGiaTri && second.BaoGomTuGiaTri;
        }

        if (secondEnd == firstStart)
        {
            return second.BaoGomDenGiaTri && first.BaoGomTuGiaTri;
        }

        return true;
    }
}

