using DanhMucService.Application.Abstractions;
using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Enums;
using DanhMucService.Domain.Interfaces.Repositories;

namespace DanhMucService.Application.Features.DanhMuc;

public class DanhMucCanBoAppService(IDanhMucCanBoRepository repository) : IDanhMucCanBoAppService
{
    private readonly IDanhMucCanBoRepository _repository = repository;

    public async Task<PagedResult<DanhMucCanBoDto>> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        Guid? phongBanId,
        LoaiLaoDongType? loaiLaoDong,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, donViId, phongBanId, loaiLaoDong, cancellationToken);
        return new PagedResult<DanhMucCanBoDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<IReadOnlyList<DanhMucCanBoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<DanhMucCanBoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<(bool IsSuccess, string Message, DanhMucCanBoDto? Data)> CreateAsync(UpsertDanhMucCanBoRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(request, null, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        var entity = request.ToEntity();
        await _repository.AddAsync(entity, cancellationToken);
        var created = await _repository.GetByIdAsync(entity.Id, cancellationToken);
        return (true, "Tạo mới cán bộ thành công.", created?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, DanhMucCanBoDto? Data)> UpdateAsync(Guid id, UpsertDanhMucCanBoRequest request, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy cán bộ cần cập nhật.", null);
        }

        var validation = await ValidateAsync(request, id, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        current.DonViQuanLyId = request.DonViQuanLyId;
        current.TenCanBo = request.TenCanBo.Trim();
        current.NgaySinh = request.NgaySinh;
        current.UserId = request.UserId;
        current.PhongBanId = request.PhongBanId;
        current.GioiTinh = request.GioiTinh;
        current.TrinhDoChuyenMon = request.TrinhDoChuyenMon.Trim();
        current.LoaiLaoDong = request.LoaiLaoDong;
        current.SoTienBHXH = request.SoTienBHXH;
        current.SoTienBHYT = request.SoTienBHYT;
        current.SoQuyetDinhDung = request.SoQuyetDinhDung;
        current.NgayQuyetDinhDung = request.NgayQuyetDinhDung;
        current.GhiChu = request.GhiChu;
        current.SoQuyetDinhBoNhiem = request.SoQuyetDinhBoNhiem;
        current.NgayQuyetDinhBoNhiem = request.NgayQuyetDinhBoNhiem;
        current.SoQuyetDinhCapThe = request.SoQuyetDinhCapThe;
        current.NgayQuyetDinhCapThe = request.NgayQuyetDinhCapThe;
        current.SoTheCongChungVien = request.SoTheCongChungVien;
        current.ChucVu = request.ChucVu;
        current.MucPhiBaoHiemTrachNhiem = request.MucPhiBaoHiemTrachNhiem;
        current.ViTriViecLam = request.ViTriViecLam;
        current.NgayTuyenDung = request.NgayTuyenDung;
        current.SoHopDongLaoDong = request.SoHopDongLaoDong;
        current.NgayKyHopDongLaoDong = request.NgayKyHopDongLaoDong;

        await _repository.UpdateAsync(current, cancellationToken);
        var updated = await _repository.GetByIdAsync(id, cancellationToken);
        return (true, "Cập nhật cán bộ thành công.", updated?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy cán bộ cần xóa.");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return (true, "Xóa cán bộ thành công.");
    }

    private async Task<(bool IsSuccess, string Message)> ValidateAsync(UpsertDanhMucCanBoRequest request, Guid? ignoreId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenCanBo))
        {
            return (false, "Tên cán bộ là bắt buộc.");
        }

        if (request.DonViQuanLyId == Guid.Empty)
        {
            return (false, "Đơn vị quản lý là bắt buộc.");
        }

        if (request.PhongBanId == Guid.Empty)
        {
            return (false, "Phòng ban là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(request.TrinhDoChuyenMon))
        {
            return (false, "Trình độ chuyên môn là bắt buộc.");
        }

        var isValidPhongBan = await _repository.IsPhongBanThuocDonViAsync(request.PhongBanId, request.DonViQuanLyId, cancellationToken);
        if (!isValidPhongBan)
        {
            return (false, "Phòng ban không thuộc đơn vị đã chọn.");
        }

        return (true, "Dữ liệu hợp lệ.");
    }
}

