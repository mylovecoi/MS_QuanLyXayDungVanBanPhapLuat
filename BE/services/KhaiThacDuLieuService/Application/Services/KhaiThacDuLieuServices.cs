using BuildingBlocks.Abstractions;
using KhaiThacDuLieuService.Application.Abstractions;
using KhaiThacDuLieuService.Application.DTOs;
using KhaiThacDuLieuService.Infrastructure.Persistence;
using KhaiThacDuLieuService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhaiThacDuLieuService.Application.Services;

public sealed class DashboardKhaiThacDuLieuService(KhaiThacDuLieuDbContext dbContext) : IDashboardKhaiThacDuLieuService
{
    public async Task<DashboardTongQuanDto> GetTongQuanAsync(CancellationToken cancellationToken = default)
    {
        var query = dbContext.CanhBaoKhaiThacDuLieus.AsNoTracking().Where(x => !x.IsDeleted);
        var items = await query
            .GroupBy(x => 1)
            .Select(g => new
            {
                Tong = g.Count(),
                Moi = g.Count(x => x.TrangThaiXuLy == "MOI"),
                DangXuLy = g.Count(x => x.TrangThaiXuLy == "DANG_XU_LY"),
                DaXuLy = g.Count(x => x.TrangThaiXuLy == "DA_XU_LY")
            })
            .FirstOrDefaultAsync(cancellationToken);

        var theoNhom = await query
            .GroupBy(x => x.NhomCanhBao)
            .Select(x => new ThongKeTheoNhomDto(x.Key, x.Count()))
            .ToListAsync(cancellationToken);

        var theoMucDo = await query
            .GroupBy(x => x.MucDo)
            .Select(x => new ThongKeTheoNhomDto(x.Key, x.Count()))
            .ToListAsync(cancellationToken);

        return new DashboardTongQuanDto(
            items?.Tong ?? 0,
            items?.Moi ?? 0,
            items?.DangXuLy ?? 0,
            items?.DaXuLy ?? 0,
            theoNhom,
            theoMucDo);
    }
}

public sealed class CanhBaoKhaiThacDuLieuService(
    KhaiThacDuLieuDbContext dbContext,
    ICurrentUserContext currentUser) : ICanhBaoKhaiThacDuLieuService
{
    public async Task<PagedResultDto<CanhBaoDto>> GetListAsync(CanhBaoListRequest request, CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        var query = ApplyFilters(ApplyDataScope(dbContext.CanhBaoKhaiThacDuLieus.AsNoTracking().Where(x => !x.IsDeleted)), request);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.NgayPhatSinh)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<CanhBaoDto>(items, total, pageSize, pageCurrent);
    }

    public async Task<CanhBaoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await ApplyDataScope(dbContext.CanhBaoKhaiThacDuLieus.AsNoTracking())
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<CanhBaoDto> CreateAsync(TaoCanhBaoRequest request, CancellationToken cancellationToken = default)
    {
        var actor = Actor();
        if (string.IsNullOrWhiteSpace(request.TieuDe) || string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new InvalidOperationException("Thiếu tiêu đề hoặc nội dung cảnh báo.");
        }

        var maCanhBao = NormalizeCode(request.MaCanhBao);
        var doiTuongNguon = NormalizeCode(request.DoiTuongNguon);
        var entity = await dbContext.CanhBaoKhaiThacDuLieus
            .FirstOrDefaultAsync(x => !x.IsDeleted
                && x.MaCanhBao == maCanhBao
                && x.DoiTuongNguon == doiTuongNguon
                && x.DoiTuongNguonId == request.DoiTuongNguonId
                && (x.TrangThaiXuLy == "MOI" || x.TrangThaiXuLy == "DANG_XU_LY"),
                cancellationToken);

        var isNew = entity is null;
        var oldStatus = entity?.TrangThaiXuLy;
        if (entity is null)
        {
            entity = new CanhBaoKhaiThacDuLieu
            {
                MaCanhBao = maCanhBao,
                DoiTuongNguon = doiTuongNguon,
                DoiTuongNguonId = request.DoiTuongNguonId,
                TrangThaiXuLy = "MOI",
                NgayPhatSinh = DateTime.UtcNow,
                CreatedBy = actor.UserId.ToString()
            };
            dbContext.CanhBaoKhaiThacDuLieus.Add(entity);
        }

        entity.NhomCanhBao = NormalizeCode(request.NhomCanhBao);
        entity.TieuDe = request.TieuDe.Trim();
        entity.NoiDung = request.NoiDung.Trim();
        entity.MucDo = NormalizeCode(request.MucDo, "TRUNG_BINH");
        entity.HanXuLy = request.HanXuLy;
        entity.NguoiNhanId = request.NguoiNhanId;
        entity.DonViNhanId = request.DonViNhanId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
        AddHistory(entity.Id, null, isNew ? "TAO_CANH_BAO" : "CAP_NHAT_CANH_BAO", entity.NoiDung, actor, oldStatus, entity.TrangThaiXuLy);

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task<CanhBaoDto?> DanhDauDaXemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await ApplyDataScope(dbContext.CanhBaoKhaiThacDuLieus)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var actor = Actor();
        var oldStatus = entity.TrangThaiXuLy;
        entity.TrangThaiXuLy = entity.TrangThaiXuLy == "MOI" ? "DANG_XU_LY" : entity.TrangThaiXuLy;
        entity.NgayXem ??= DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
        AddHistory(entity.Id, null, "XEM_CANH_BAO", "Đã xem cảnh báo.", actor, oldStatus, entity.TrangThaiXuLy);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task<CanhBaoDto?> XacNhanXuLyAsync(Guid id, XacNhanXuLyCanhBaoRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await ApplyDataScope(dbContext.CanhBaoKhaiThacDuLieus)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var actor = Actor();
        var oldStatus = entity.TrangThaiXuLy;
        entity.TrangThaiXuLy = "DA_XU_LY";
        entity.NguoiXuLyId = actor.UserId;
        entity.NgayXuLy = DateTime.UtcNow;
        entity.GhiChuXuLy = request.GhiChuXuLy;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
        AddHistory(entity.Id, null, "XU_LY_CANH_BAO", request.GhiChuXuLy, actor, oldStatus, entity.TrangThaiXuLy);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task<IReadOnlyList<CauHinhCanhBaoDto>> GetCauHinhAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.CauHinhCanhBaoKhaiThacDuLieus
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.NhomCanhBao)
            .ThenBy(x => x.MaCanhBao)
            .Select(x => ToDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<CauHinhCanhBaoDto?> UpdateCauHinhAsync(Guid id, CapNhatCauHinhCanhBaoRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.CauHinhCanhBaoKhaiThacDuLieus.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var actor = Actor();
        entity.TenCanhBao = request.TenCanhBao.Trim();
        entity.NhomCanhBao = NormalizeCode(request.NhomCanhBao);
        entity.SoNgayCanhBaoTruocHan = Math.Max(0, request.SoNgayCanhBaoTruocHan);
        entity.MucDoMacDinh = NormalizeCode(request.MucDoMacDinh);
        entity.KenhThongBao = request.KenhThongBao;
        entity.TrangThai = request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task<IReadOnlyList<CanhBaoLichSuXuLyDto>?> GetLichSuAsync(Guid canhBaoId, CancellationToken cancellationToken = default)
    {
        if (!await CanAccessCanhBaoAsync(canhBaoId, cancellationToken))
        {
            return null;
        }

        var items = await dbContext.CanhBaoLichSuXuLys
            .AsNoTracking()
            .Where(x => x.CanhBaoId == canhBaoId && !x.IsDeleted)
            .OrderByDescending(x => x.ThoiGian)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return items.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<CanhBaoNhacViecDto>?> GetNhacViecAsync(Guid canhBaoId, CancellationToken cancellationToken = default)
    {
        if (!await CanAccessCanhBaoAsync(canhBaoId, cancellationToken))
        {
            return null;
        }

        var items = await dbContext.CanhBaoNhacViecs
            .AsNoTracking()
            .Where(x => x.CanhBaoId == canhBaoId && !x.IsDeleted)
            .OrderBy(x => x.TrangThai == "DA_HOAN_THANH" || x.TrangThai == "HUY")
            .ThenBy(x => x.HanXuLy == null)
            .ThenBy(x => x.HanXuLy)
            .ThenByDescending(x => x.NgayGui)
            .ToListAsync(cancellationToken);

        return items.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<CanhBaoNhacViecDto>> GetNhacViecCuaToiAsync(CancellationToken cancellationToken = default)
    {
        var query = ApplyNhacViecDataScope(dbContext.CanhBaoNhacViecs.AsNoTracking().Where(x => !x.IsDeleted));
        var items = await query
            .Where(x => x.TrangThai != "DA_HOAN_THANH" && x.TrangThai != "HUY")
            .OrderBy(x => x.HanXuLy == null)
            .ThenBy(x => x.HanXuLy)
            .ThenByDescending(x => x.NgayGui)
            .Take(10)
            .ToListAsync(cancellationToken);

        return items.Select(ToDto).ToList();
    }

    public async Task<CanhBaoNhacViecDto?> TaoNhacViecAsync(Guid canhBaoId, TaoCanhBaoNhacViecRequest request, CancellationToken cancellationToken = default)
    {
        var actor = Actor();
        if (string.IsNullOrWhiteSpace(request.TieuDe) || string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new InvalidOperationException("Thiếu tiêu đề hoặc nội dung nhắc việc.");
        }

        var canhBao = await ApplyDataScope(dbContext.CanhBaoKhaiThacDuLieus)
            .FirstOrDefaultAsync(x => x.Id == canhBaoId && !x.IsDeleted, cancellationToken);
        if (canhBao is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var entity = new CanhBaoNhacViec
        {
            CanhBaoId = canhBaoId,
            TieuDe = request.TieuDe.Trim(),
            NoiDung = request.NoiDung.Trim(),
            NguoiGiaoId = actor.UserId,
            DonViGiaoId = actor.DonViId,
            NguoiNhanId = request.NguoiNhanId,
            DonViNhanId = request.DonViNhanId ?? canhBao.DonViNhanId,
            HanXuLy = request.HanXuLy,
            ThoiGianNhac = request.ThoiGianNhac,
            MucDoUuTien = NormalizeCode(request.MucDoUuTien, canhBao.MucDo),
            TrangThai = "DA_GUI",
            NgayGui = now,
            CreatedBy = actor.UserId.ToString()
        };

        dbContext.CanhBaoNhacViecs.Add(entity);
        AddHistory(canhBaoId, entity.Id, "TAO_NHAC_VIEC", entity.NoiDung, actor, null, entity.TrangThai);
        if (canhBao.TrangThaiXuLy == "MOI")
        {
            var oldAlertStatus = canhBao.TrangThaiXuLy;
            canhBao.TrangThaiXuLy = "DANG_XU_LY";
            canhBao.NgayXem ??= now;
            AddHistory(canhBaoId, null, "CAP_NHAT_TRANG_THAI_CANH_BAO", "Tự chuyển sang đang xử lý khi tạo nhắc việc.", actor, oldAlertStatus, canhBao.TrangThaiXuLy);
        }

        canhBao.UpdatedAt = now;
        canhBao.UpdatedBy = actor.UserId.ToString();
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public Task<CanhBaoNhacViecDto?> DanhDauDaXemNhacViecAsync(Guid canhBaoId, Guid nhacViecId, CancellationToken cancellationToken = default)
    {
        return UpdateNhacViecAsync(canhBaoId, nhacViecId, "XEM_NHAC_VIEC", "Đã xem nhắc việc.", (entity, actor, now) =>
        {
            entity.NgayXem ??= now;
            if (entity.TrangThai == "DA_GUI" || entity.TrangThai == "MOI")
            {
                entity.TrangThai = "DANG_XU_LY";
            }
        }, cancellationToken);
    }

    public Task<CanhBaoNhacViecDto?> HoanThanhNhacViecAsync(Guid canhBaoId, Guid nhacViecId, HoanThanhCanhBaoNhacViecRequest request, CancellationToken cancellationToken = default)
    {
        return UpdateNhacViecAsync(canhBaoId, nhacViecId, "HOAN_THANH_NHAC_VIEC", request.GhiChuHoanThanh, (entity, actor, now) =>
        {
            entity.TrangThai = "DA_HOAN_THANH";
            entity.NgayHoanThanh = now;
            entity.GhiChuHoanThanh = request.GhiChuHoanThanh;
        }, cancellationToken);
    }

    public Task<CanhBaoNhacViecDto?> HuyNhacViecAsync(Guid canhBaoId, Guid nhacViecId, CancellationToken cancellationToken = default)
    {
        return UpdateNhacViecAsync(canhBaoId, nhacViecId, "HUY_NHAC_VIEC", "Đã hủy nhắc việc.", (entity, actor, now) =>
        {
            entity.TrangThai = "HUY";
            entity.NgayHoanThanh = now;
        }, cancellationToken);
    }

    private async Task<CanhBaoNhacViecDto?> UpdateNhacViecAsync(
        Guid canhBaoId,
        Guid nhacViecId,
        string action,
        string? content,
        Action<CanhBaoNhacViec, CurrentActor, DateTime> update,
        CancellationToken cancellationToken)
    {
        var actor = Actor();
        var entity = await ApplyNhacViecDataScope(dbContext.CanhBaoNhacViecs)
            .FirstOrDefaultAsync(x => x.Id == nhacViecId && x.CanhBaoId == canhBaoId && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var oldStatus = entity.TrangThai;
        update(entity, actor, now);
        entity.UpdatedAt = now;
        entity.UpdatedBy = actor.UserId.ToString();
        AddHistory(canhBaoId, nhacViecId, action, content, actor, oldStatus, entity.TrangThai);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    private IQueryable<CanhBaoKhaiThacDuLieu> ApplyDataScope(IQueryable<CanhBaoKhaiThacDuLieu> query)
    {
        if (currentUser.IsSSA)
        {
            return query;
        }

        var userId = currentUser.UserId;
        var donViId = currentUser.DonViId;
        return query.Where(x =>
            (userId.HasValue && (x.NguoiNhanId == userId.Value || x.NguoiXuLyId == userId.Value))
            || (donViId.HasValue && x.DonViNhanId == donViId.Value));
    }

    private IQueryable<CanhBaoNhacViec> ApplyNhacViecDataScope(IQueryable<CanhBaoNhacViec> query)
    {
        if (currentUser.IsSSA)
        {
            return query;
        }

        var userId = currentUser.UserId;
        var donViId = currentUser.DonViId;
        return query.Where(x =>
            (userId.HasValue && (x.NguoiGiaoId == userId.Value || x.NguoiNhanId == userId.Value))
            || (donViId.HasValue && (x.DonViGiaoId == donViId.Value || x.DonViNhanId == donViId.Value)));
    }

    private async Task<bool> CanAccessCanhBaoAsync(Guid canhBaoId, CancellationToken cancellationToken)
    {
        return await ApplyDataScope(dbContext.CanhBaoKhaiThacDuLieus.AsNoTracking())
            .AnyAsync(x => x.Id == canhBaoId && !x.IsDeleted, cancellationToken);
    }

    private static IQueryable<CanhBaoKhaiThacDuLieu> ApplyFilters(IQueryable<CanhBaoKhaiThacDuLieu> query, CanhBaoListRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.TieuDe.Contains(search) || x.NoiDung.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.NhomCanhBao)) query = query.Where(x => x.NhomCanhBao == request.NhomCanhBao.Trim());
        if (!string.IsNullOrWhiteSpace(request.TrangThaiXuLy)) query = query.Where(x => x.TrangThaiXuLy == request.TrangThaiXuLy.Trim());
        if (!string.IsNullOrWhiteSpace(request.MucDo)) query = query.Where(x => x.MucDo == request.MucDo.Trim());
        if (request.DonViNhanId is Guid donViNhanId) query = query.Where(x => x.DonViNhanId == donViNhanId);
        if (request.NguoiNhanId is Guid nguoiNhanId) query = query.Where(x => x.NguoiNhanId == nguoiNhanId);
        if (request.TuNgay is DateTime tuNgay) query = query.Where(x => x.NgayPhatSinh >= tuNgay);
        if (request.DenNgay is DateTime denNgay) query = query.Where(x => x.NgayPhatSinh <= denNgay);
        return query;
    }

    private CurrentActor Actor()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not Guid userId)
        {
            throw new UnauthorizedAccessException();
        }

        return new CurrentActor(userId, currentUser.DonViId);
    }

    private static string NormalizeCode(string? value, string fallback = "")
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim().ToUpperInvariant();
    }

    private static CanhBaoDto ToDto(CanhBaoKhaiThacDuLieu x) => new(
        x.Id, x.MaCanhBao, x.NhomCanhBao, x.DoiTuongNguon, x.DoiTuongNguonId, x.TieuDe, x.NoiDung,
        x.MucDo, x.TrangThaiXuLy, x.HanXuLy, x.NgayPhatSinh, x.NguoiNhanId, x.DonViNhanId,
        x.NguoiXuLyId, x.NgayXem, x.NgayXuLy, x.GhiChuXuLy);

    private static CauHinhCanhBaoDto ToDto(CauHinhCanhBaoKhaiThacDuLieu x) => new(
        x.Id, x.MaCanhBao, x.TenCanhBao, x.NhomCanhBao, x.SoNgayCanhBaoTruocHan,
        x.MucDoMacDinh, x.KenhThongBao, x.TrangThai);

    private static CanhBaoNhacViecDto ToDto(CanhBaoNhacViec x) => new(
        x.Id, x.CanhBaoId, x.TieuDe, x.NoiDung, x.NguoiGiaoId, x.DonViGiaoId,
        x.NguoiNhanId, x.DonViNhanId, x.HanXuLy, x.ThoiGianNhac, x.MucDoUuTien,
        ResolveReminderStatus(x), x.NgayGui, x.NgayXem, x.NgayHoanThanh, x.GhiChuHoanThanh);

    private static string ResolveReminderStatus(CanhBaoNhacViec x)
    {
        if (x.TrangThai is "DA_HOAN_THANH" or "HUY")
        {
            return x.TrangThai;
        }

        return x.HanXuLy.HasValue && x.HanXuLy.Value.Date < DateTime.UtcNow.Date ? "QUA_HAN" : x.TrangThai;
    }

    private void AddHistory(Guid canhBaoId, Guid? nhacViecId, string action, string? content, CurrentActor actor, string? oldStatus, string? newStatus)
    {
        dbContext.CanhBaoLichSuXuLys.Add(new CanhBaoLichSuXuLy
        {
            CanhBaoId = canhBaoId,
            NhacViecId = nhacViecId,
            HanhDong = action,
            NoiDung = content,
            NguoiThucHienId = actor.UserId,
            DonViThucHienId = actor.DonViId,
            TrangThaiTruoc = oldStatus,
            TrangThaiSau = newStatus,
            ThoiGian = DateTime.UtcNow,
            CreatedBy = actor.UserId.ToString()
        });
    }

    private static CanhBaoLichSuXuLyDto ToDto(CanhBaoLichSuXuLy x) => new(
        x.Id, x.CanhBaoId, x.NhacViecId, x.HanhDong, x.NoiDung, x.NguoiThucHienId,
        x.DonViThucHienId, x.TrangThaiTruoc, x.TrangThaiSau, x.ThoiGian);

    private sealed record CurrentActor(Guid UserId, Guid? DonViId);
}

public sealed class CanhBaoThongMinhGeneratorService(KhaiThacDuLieuDbContext dbContext) : ICanhBaoThongMinhGeneratorService
{
    private const string SystemActor = "CANH_BAO_THONG_MINH";
    private const string NguonDangKyXayDungVanBan = "DANG_KY_XAY_DUNG_VAN_BAN";
    private const string MaSapDenHan = "DANG_KY_SAP_DEN_HAN";
    private const string MaQuaHan = "DANG_KY_QUA_HAN";
    private const string NhomTienDo = "TIEN_DO";

    public async Task<SinhCanhBaoResultDto> SinhCanhBaoTuDongAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDefaultConfigurationsAsync(cancellationToken);

        var configs = await dbContext.CauHinhCanhBaoKhaiThacDuLieus
            .Where(x => !x.IsDeleted && x.TrangThai && (x.MaCanhBao == MaSapDenHan || x.MaCanhBao == MaQuaHan))
            .ToDictionaryAsync(x => x.MaCanhBao, cancellationToken);

        var today = DateTime.UtcNow.Date;
        var maxWarningDays = configs.TryGetValue(MaSapDenHan, out var dueSoonConfig)
            ? Math.Max(0, dueSoonConfig.SoNgayCanhBaoTruocHan)
            : 0;
        var warningLimit = today.AddDays(maxWarningDays);

        var hoSos = await (
            from hoSo in dbContext.DangKyXayDungVanBans.Where(x => !x.IsDeleted && x.DuKienThoiGianTrinh.HasValue)
            join trangThai in dbContext.DangKyTrangThaiHoSos.Where(x => !x.IsDeleted)
                on hoSo.TrangThaiHoSoId equals trangThai.Id into trangThaiJoin
            from trangThai in trangThaiJoin.DefaultIfEmpty()
            where trangThai == null || !trangThai.LaTrangThaiKetThuc
            where hoSo.DuKienThoiGianTrinh!.Value.Date < today
                || hoSo.DuKienThoiGianTrinh.Value.Date <= warningLimit
            select new DangKyCanhBaoProjection(
                hoSo.Id,
                hoSo.MaHoSo,
                hoSo.TenHoSo,
                hoSo.TenVanBanDuKien,
                hoSo.DonViSoanThaoId,
                hoSo.DuKienThoiGianTrinh!.Value))
            .ToListAsync(cancellationToken);

        var result = new GenerationCounter(configs.Count, hoSos.Count);
        foreach (var hoSo in hoSos)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var deadline = hoSo.DuKienThoiGianTrinh.Date;
            if (deadline < today && configs.TryGetValue(MaQuaHan, out var overdueConfig))
            {
                result.Closed += await CloseOpenAlertAsync(MaSapDenHan, hoSo.Id, "Tự đóng cảnh báo sắp hạn vì hồ sơ đã quá hạn.", cancellationToken);
                await UpsertAlertAsync(overdueConfig, hoSo, today, cancellationToken, result);
            }
            else if (deadline >= today && configs.TryGetValue(MaSapDenHan, out var soonConfig)
                && deadline <= today.AddDays(Math.Max(0, soonConfig.SoNgayCanhBaoTruocHan)))
            {
                await UpsertAlertAsync(soonConfig, hoSo, today, cancellationToken, result);
            }
        }

        result.OverdueReminders = await MarkOverdueRemindersAsync(today, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SinhCanhBaoResultDto(result.ActiveConfigurations, result.CheckedObjects, result.Created, result.Updated, result.Closed, result.OverdueReminders);
    }

    private async Task EnsureDefaultConfigurationsAsync(CancellationToken cancellationToken)
    {
        await EnsureConfigurationAsync(MaSapDenHan, "Hồ sơ đăng ký sắp đến hạn trình", 5, "TRUNG_BINH", cancellationToken);
        await EnsureConfigurationAsync(MaQuaHan, "Hồ sơ đăng ký đã quá hạn trình", 0, "CAO", cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureConfigurationAsync(string maCanhBao, string tenCanhBao, int soNgayCanhBao, string mucDo, CancellationToken cancellationToken)
    {
        var exists = await dbContext.CauHinhCanhBaoKhaiThacDuLieus
            .AnyAsync(x => !x.IsDeleted && x.MaCanhBao == maCanhBao, cancellationToken);
        if (exists)
        {
            return;
        }

        dbContext.CauHinhCanhBaoKhaiThacDuLieus.Add(new CauHinhCanhBaoKhaiThacDuLieu
        {
            MaCanhBao = maCanhBao,
            TenCanhBao = tenCanhBao,
            NhomCanhBao = NhomTienDo,
            SoNgayCanhBaoTruocHan = soNgayCanhBao,
            MucDoMacDinh = mucDo,
            KenhThongBao = "IN_APP",
            TrangThai = true,
            CreatedBy = SystemActor
        });
    }

    private async Task UpsertAlertAsync(
        CauHinhCanhBaoKhaiThacDuLieu config,
        DangKyCanhBaoProjection hoSo,
        DateTime today,
        CancellationToken cancellationToken,
        GenerationCounter counter)
    {
        var existing = await dbContext.CanhBaoKhaiThacDuLieus
            .FirstOrDefaultAsync(x => !x.IsDeleted
                && x.MaCanhBao == config.MaCanhBao
                && x.DoiTuongNguon == NguonDangKyXayDungVanBan
                && x.DoiTuongNguonId == hoSo.Id
                && (x.TrangThaiXuLy == "MOI" || x.TrangThaiXuLy == "DANG_XU_LY"),
                cancellationToken);

        if (existing is null)
        {
            existing = new CanhBaoKhaiThacDuLieu
            {
                MaCanhBao = config.MaCanhBao,
                DoiTuongNguon = NguonDangKyXayDungVanBan,
                DoiTuongNguonId = hoSo.Id,
                TrangThaiXuLy = "MOI",
                NgayPhatSinh = DateTime.UtcNow,
                CreatedBy = SystemActor
            };
            dbContext.CanhBaoKhaiThacDuLieus.Add(existing);
            counter.Created++;
            AddSystemHistory(existing.Id, null, "TU_DONG_TAO_CANH_BAO", "Hệ thống tự động tạo cảnh báo.", null, existing.TrangThaiXuLy);
        }
        else
        {
            counter.Updated++;
            AddSystemHistory(existing.Id, null, "TU_DONG_CAP_NHAT_CANH_BAO", "Hệ thống tự động cập nhật cảnh báo.", existing.TrangThaiXuLy, existing.TrangThaiXuLy);
        }

        var soNgayConLai = (hoSo.DuKienThoiGianTrinh.Date - today).Days;
        var isOverdue = soNgayConLai < 0;
        existing.NhomCanhBao = config.NhomCanhBao;
        existing.TieuDe = isOverdue
            ? $"Hồ sơ {hoSo.MaHoSo} đã quá hạn trình"
            : $"Hồ sơ {hoSo.MaHoSo} sắp đến hạn trình";
        existing.NoiDung = BuildContent(hoSo, soNgayConLai);
        existing.MucDo = ResolveSeverity(config.MucDoMacDinh, soNgayConLai);
        existing.HanXuLy = hoSo.DuKienThoiGianTrinh;
        existing.DonViNhanId = hoSo.DonViSoanThaoId;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = SystemActor;
    }

    private async Task<int> CloseOpenAlertAsync(string maCanhBao, Guid doiTuongNguonId, string ghiChu, CancellationToken cancellationToken)
    {
        var alerts = await dbContext.CanhBaoKhaiThacDuLieus
            .Where(x => !x.IsDeleted
                && x.MaCanhBao == maCanhBao
                && x.DoiTuongNguon == NguonDangKyXayDungVanBan
                && x.DoiTuongNguonId == doiTuongNguonId
                && (x.TrangThaiXuLy == "MOI" || x.TrangThaiXuLy == "DANG_XU_LY"))
            .ToListAsync(cancellationToken);

        foreach (var alert in alerts)
        {
            var oldStatus = alert.TrangThaiXuLy;
            alert.TrangThaiXuLy = "DA_XU_LY";
            alert.NgayXuLy = DateTime.UtcNow;
            alert.GhiChuXuLy = ghiChu;
            alert.UpdatedAt = DateTime.UtcNow;
            alert.UpdatedBy = SystemActor;
            AddSystemHistory(alert.Id, null, "TU_DONG_DONG_CANH_BAO", ghiChu, oldStatus, alert.TrangThaiXuLy);
        }

        return alerts.Count;
    }

    private async Task<int> MarkOverdueRemindersAsync(DateTime today, CancellationToken cancellationToken)
    {
        var reminders = await dbContext.CanhBaoNhacViecs
            .Where(x => !x.IsDeleted
                && x.HanXuLy.HasValue
                && x.HanXuLy.Value.Date < today
                && x.TrangThai != "DA_HOAN_THANH"
                && x.TrangThai != "HUY"
                && x.TrangThai != "QUA_HAN")
            .ToListAsync(cancellationToken);

        foreach (var reminder in reminders)
        {
            var oldStatus = reminder.TrangThai;
            reminder.TrangThai = "QUA_HAN";
            reminder.UpdatedAt = DateTime.UtcNow;
            reminder.UpdatedBy = SystemActor;
            AddSystemHistory(reminder.CanhBaoId, reminder.Id, "TU_DONG_CHUYEN_NHAC_VIEC_QUA_HAN", "Nhắc việc đã quá hạn xử lý.", oldStatus, reminder.TrangThai);
        }

        return reminders.Count;
    }

    private static string BuildContent(DangKyCanhBaoProjection hoSo, int soNgayConLai)
    {
        var deadline = hoSo.DuKienThoiGianTrinh.ToString("dd/MM/yyyy");
        var prefix = soNgayConLai < 0
            ? $"Hồ sơ đã quá hạn {Math.Abs(soNgayConLai)} ngày."
            : $"Hồ sơ còn {soNgayConLai} ngày đến hạn.";
        return $"{prefix} Mã hồ sơ: {hoSo.MaHoSo}. Tên hồ sơ: {hoSo.TenHoSo}. Văn bản dự kiến: {hoSo.TenVanBanDuKien}. Hạn trình: {deadline}.";
    }

    private static string ResolveSeverity(string configuredSeverity, int soNgayConLai)
    {
        if (soNgayConLai < -7) return "KHAN_CAP";
        if (soNgayConLai < 0) return "CAO";
        if (soNgayConLai <= 2) return "TRUNG_BINH";
        return string.IsNullOrWhiteSpace(configuredSeverity) ? "THAP" : configuredSeverity.Trim().ToUpperInvariant();
    }

    private void AddSystemHistory(Guid canhBaoId, Guid? nhacViecId, string action, string? content, string? oldStatus, string? newStatus)
    {
        dbContext.CanhBaoLichSuXuLys.Add(new CanhBaoLichSuXuLy
        {
            CanhBaoId = canhBaoId,
            NhacViecId = nhacViecId,
            HanhDong = action,
            NoiDung = content,
            TrangThaiTruoc = oldStatus,
            TrangThaiSau = newStatus,
            ThoiGian = DateTime.UtcNow,
            CreatedBy = SystemActor
        });
    }

    private sealed record DangKyCanhBaoProjection(
        Guid Id,
        string MaHoSo,
        string TenHoSo,
        string TenVanBanDuKien,
        Guid DonViSoanThaoId,
        DateTime DuKienThoiGianTrinh);

    private sealed class GenerationCounter(int activeConfigurations, int checkedObjects)
    {
        public int ActiveConfigurations { get; } = activeConfigurations;
        public int CheckedObjects { get; } = checkedObjects;
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Closed { get; set; }
        public int OverdueReminders { get; set; }
    }
}

public sealed class TraCuuKhaiThacDuLieuService(
    KhaiThacDuLieuDbContext dbContext,
    ICurrentUserContext currentUser) : ITraCuuKhaiThacDuLieuService
{
    private const string NguonDangKyXayDungVanBan = "DANG_KY_XAY_DUNG_VAN_BAN";
    private const string NguonTongHop = "TONG_HOP";

    public async Task<PagedResultDto<TraCuuTongHopItemDto>> SearchAsync(string nguonDuLieu, TraCuuRequest request, CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);

        if (nguonDuLieu is not NguonDangKyXayDungVanBan and not NguonTongHop)
        {
            return new PagedResultDto<TraCuuTongHopItemDto>([], 0, pageSize, pageCurrent);
        }

        var query =
            from hoSo in ApplyDangKyDataScope(dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted))
            join trangThai in dbContext.DangKyTrangThaiHoSos.AsNoTracking()
                on hoSo.TrangThaiHoSoId equals trangThai.Id into trangThaiJoin
            from trangThai in trangThaiJoin.DefaultIfEmpty()
            select new
            {
                hoSo.Id,
                hoSo.MaHoSo,
                hoSo.TenHoSo,
                hoSo.TenVanBanDuKien,
                hoSo.LoaiVanBanId,
                hoSo.TrangThaiHoSoId,
                hoSo.DonViSoanThaoId,
                hoSo.DonViPheDuyetId,
                hoSo.NamDangKy,
                hoSo.CreatedAt,
                hoSo.DuKienThoiGianTrinh,
                TenTrangThai = trangThai != null ? trangThai.TenTrangThai : null,
                MaTrangThai = trangThai != null ? trangThai.MaTrangThai : null
            };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var keyword = request.Search.Trim();
            query = query.Where(x =>
                x.MaHoSo.Contains(keyword)
                || x.TenHoSo.Contains(keyword)
                || x.TenVanBanDuKien.Contains(keyword));
        }

        if (request.DonViId is Guid donViId)
        {
            query = query.Where(x => x.DonViSoanThaoId == donViId || x.DonViPheDuyetId == donViId);
        }

        if (request.LoaiVanBanId is Guid loaiVanBanId)
        {
            query = query.Where(x => x.LoaiVanBanId == loaiVanBanId);
        }

        if (request.TrangThaiId is Guid trangThaiId)
        {
            query = query.Where(x => x.TrangThaiHoSoId == trangThaiId);
        }

        if (request.Nam is int nam)
        {
            query = query.Where(x => x.NamDangKy == nam);
        }

        if (request.TuNgay is DateTime tuNgay)
        {
            query = query.Where(x => x.CreatedAt >= tuNgay);
        }

        if (request.DenNgay is DateTime denNgay)
        {
            var denNgayExclusive = denNgay.Date.AddDays(1);
            query = query.Where(x => x.CreatedAt < denNgayExclusive);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TraCuuTongHopItemDto(
                NguonDangKyXayDungVanBan,
                x.Id,
                x.MaHoSo,
                x.TenHoSo,
                x.DonViSoanThaoId,
                x.TrangThaiHoSoId,
                x.CreatedAt,
                x.DuKienThoiGianTrinh,
                x.TenTrangThai ?? x.MaTrangThai))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<TraCuuTongHopItemDto>(items, totalCount, pageSize, pageCurrent);
    }

    private IQueryable<DangKyXayDungVanBanTraCuu> ApplyDangKyDataScope(IQueryable<DangKyXayDungVanBanTraCuu> query)
    {
        if (currentUser.IsSSA)
        {
            return query;
        }

        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            return query.Where(_ => false);
        }

        var userIdText = currentUser.UserId.Value.ToString();
        if (currentUser.DonViId is not Guid donViId)
        {
            return query.Where(x => x.CreatedBy == userIdText);
        }

        return query.Where(x =>
            x.DonViSoanThaoId == donViId
            || x.DonViPheDuyetId == donViId
            || x.CreatedBy == userIdText);
    }
}

public sealed class BaoCaoKhaiThacDuLieuService : IBaoCaoKhaiThacDuLieuService
{
    public Task<BaoCaoTongHopDto> GetBaoCaoAsync(string loaiBaoCao, BaoCaoRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BaoCaoTongHopDto(loaiBaoCao, DateTime.UtcNow, []));
    }
}
