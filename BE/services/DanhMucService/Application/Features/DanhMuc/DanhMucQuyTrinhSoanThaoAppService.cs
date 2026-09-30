using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucQuyTrinhSoanThaoAppService(IDanhMucQuyTrinhSoanThaoRepository repository) : IDanhMucQuyTrinhSoanThaoAppService
{
    private readonly IDanhMucQuyTrinhSoanThaoRepository _repository = repository;

    public async Task<PagedResult<DanhMucQuyTrinhSoanThaoDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<DanhMucQuyTrinhSoanThaoDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<DanhMucQuyTrinhSoanThaoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IReadOnlyList<DanhMucLookupDto>> GetDanhMucVanBanOptionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetDanhMucVanBanOptionsAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<IReadOnlyList<DanhMucLookupDto>> GetDanhMucDonViOptionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetDanhMucDonViOptionsAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<(bool IsSuccess, string Message, DanhMucQuyTrinhSoanThaoDto? Data)> CreateAsync(UpsertDanhMucQuyTrinhSoanThaoRequest request, CancellationToken cancellationToken = default)
    {
        NormalizeRequest(request);
        var validation = await ValidateAsync(request, null, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        var entity = request.ToEntity();
        await _repository.AddAsync(entity, cancellationToken);
        var created = await _repository.GetByIdAsync(entity.Id, cancellationToken);
        return (true, "Tạo mới quy trình soạn thảo thành công.", created?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucQuyTrinhSoanThaoDto? Data)> UpdateAsync(Guid id, UpsertDanhMucQuyTrinhSoanThaoRequest request, CancellationToken cancellationToken = default)
    {
        request.Id = id;
        NormalizeRequest(request);
        var validation = await ValidateAsync(request, id, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return (false, "Không tìm thấy quy trình soạn thảo.", null);
        }

        var deletedStepIds = existing.BuocQuyTrinhs
            .Where(x => request.BuocQuyTrinhs.All(y => y.Id == Guid.Empty || y.Id != x.Id))
            .Select(x => x.Id)
            .ToArray();

        if (deletedStepIds.Length > 0 && await _repository.IsAnyStepInUseAsync(deletedStepIds, cancellationToken))
        {
            return (false, "Không thể xóa bước đã được sử dụng trong hồ sơ văn bản.", null);
        }

        var entity = request.ToEntity();
        await _repository.UpdateAsync(entity, cancellationToken);
        var updated = await _repository.GetByIdAsync(id, cancellationToken);
        return (true, "Cập nhật quy trình soạn thảo thành công.", updated?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return (false, "Không tìm thấy quy trình soạn thảo.");
        }

        if (await _repository.IsWorkflowUsedAsync(id, cancellationToken))
        {
            return (false, "Quy trình đã phát sinh hồ sơ văn bản, không thể xóa.");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return (true, "Xóa quy trình soạn thảo thành công.");
    }

    private async Task<(bool IsSuccess, string Message)> ValidateAsync(UpsertDanhMucQuyTrinhSoanThaoRequest request, Guid? ignoreId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MaQuyTrinh))
        {
            return (false, "Mã quy trình không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(request.TenQuyTrinh))
        {
            return (false, "Tên quy trình không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(request.LoaiQuyTrinh))
        {
            return (false, "Loại quy trình không được để trống.");
        }

        if (await _repository.ExistsByCodeAsync(request.MaQuyTrinh, ignoreId, cancellationToken))
        {
            return (false, "Mã quy trình đã tồn tại.");
        }

        if (request.BuocQuyTrinhs.Count == 0)
        {
            return (false, "Phải có ít nhất 1 bước quy trình.");
        }

        if (request.BuocQuyTrinhs.Any(x => string.IsNullOrWhiteSpace(x.MaBuoc) || string.IsNullOrWhiteSpace(x.TenBuoc)))
        {
            return (false, "Tất cả các bước phải có mã bước và tên bước.");
        }

        var duplicateStepCodes = request.BuocQuyTrinhs
            .GroupBy(x => x.MaBuoc, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (duplicateStepCodes.Length > 0)
        {
            return (false, $"Mã bước bị trùng: {string.Join(", ", duplicateStepCodes)}");
        }

        var stepCodes = request.BuocQuyTrinhs
            .Select(x => x.MaBuoc)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var transition in request.ChuyenBuocs)
        {
            if (!stepCodes.Contains(transition.TuBuocMa) || !stepCodes.Contains(transition.DenBuocMa))
            {
                return (false, $"Nhánh chuyển bước '{transition.TuBuocMa} -> {transition.DenBuocMa}' không hợp lệ vì mã bước không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(transition.DieuKienKetQua))
            {
                return (false, "Nhánh chuyển bước phải có điều kiện kết quả.");
            }
        }

        return (true, string.Empty);
    }

    private static void NormalizeRequest(UpsertDanhMucQuyTrinhSoanThaoRequest request)
    {
        request.MaQuyTrinh = request.MaQuyTrinh?.Trim() ?? string.Empty;
        request.TenQuyTrinh = request.TenQuyTrinh?.Trim() ?? string.Empty;
        request.LoaiQuyTrinh = NormalizeLoaiQuyTrinhValue(request.LoaiQuyTrinh);
        request.CapApDungs = request.CapApDungs
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(NormalizeCapApDungValue)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (request.CapApDungs.Count == 0)
        {
            request.CapApDungs.Add("Tinh");
        }

        request.CapApDung = string.Join(",", request.CapApDungs);
        request.DanhMucVanBanIds = request.DanhMucVanBanIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();
        request.DanhMucVanBanId = request.DanhMucVanBanIds.FirstOrDefault();
        request.BuocQuyTrinhs = request.BuocQuyTrinhs
            .Where(x => !string.IsNullOrWhiteSpace(x.MaBuoc) || !string.IsNullOrWhiteSpace(x.TenBuoc) || !string.IsNullOrWhiteSpace(x.LoaiBuoc))
            .Select((x, index) =>
            {
                x.MaBuoc = x.MaBuoc?.Trim() ?? string.Empty;
                x.TenBuoc = x.TenBuoc?.Trim() ?? string.Empty;
                x.LoaiBuoc = string.IsNullOrWhiteSpace(x.LoaiBuoc) ? "XuLy" : x.LoaiBuoc.Trim();
                x.CachHoanThanh = x.CachHoanThanh?.Trim();
                x.SoNgayXuLyTieuChuan = x.SoNgayXuLyTieuChuan.HasValue && x.SoNgayXuLyTieuChuan.Value <= 0 ? null : x.SoNgayXuLyTieuChuan;
                x.SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan.HasValue && x.SoNgayCanhBaoSapHan.Value < 0 ? 0 : x.SoNgayCanhBaoSapHan;
                x.DonViTiepNhanMacDinhId = x.DonViTiepNhanMacDinhId == Guid.Empty ? null : x.DonViTiepNhanMacDinhId;
                x.MoTa = x.MoTa?.Trim();
                x.GhiChu = x.GhiChu?.Trim();
                x.ThuTuSapXep = x.ThuTuSapXep <= 0 ? index + 1 : x.ThuTuSapXep;
                return x;
            })
            .OrderBy(x => x.ThuTuSapXep)
            .ThenBy(x => x.MaBuoc)
            .ToList();

        request.ChuyenBuocs = request.ChuyenBuocs
            .Where(x => !string.IsNullOrWhiteSpace(x.TuBuocMa) || !string.IsNullOrWhiteSpace(x.DenBuocMa) || !string.IsNullOrWhiteSpace(x.DieuKienKetQua))
            .Select(x =>
            {
                x.TuBuocMa = x.TuBuocMa?.Trim() ?? string.Empty;
                x.DenBuocMa = x.DenBuocMa?.Trim() ?? string.Empty;
                x.DieuKienKetQua = x.DieuKienKetQua?.Trim() ?? string.Empty;
                x.MoTa = x.MoTa?.Trim();
                x.GhiChu = x.GhiChu?.Trim();
                return x;
            })
            .ToList();
    }

    private static string NormalizeCapApDungValue(string? value)
    {
        return (value ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            "TINH" => "Tinh",
            "XA" => "Xa",
            _ => (value ?? string.Empty).Trim()
        };
    }

    private static string NormalizeLoaiQuyTrinhValue(string? value)
    {
        return (value ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            "DANGKY" => "DangKy",
            "DANG_KY" => "DangKy",
            "XAYDUNG" => "XayDung",
            "XAY_DUNG" => "XayDung",
            _ => string.IsNullOrWhiteSpace(value) ? "XayDung" : value.Trim()
        };
    }
}

