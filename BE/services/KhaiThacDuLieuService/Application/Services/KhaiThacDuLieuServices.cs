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

        var entity = new CanhBaoKhaiThacDuLieu
        {
            MaCanhBao = NormalizeCode(request.MaCanhBao),
            NhomCanhBao = NormalizeCode(request.NhomCanhBao),
            DoiTuongNguon = NormalizeCode(request.DoiTuongNguon),
            DoiTuongNguonId = request.DoiTuongNguonId,
            TieuDe = request.TieuDe.Trim(),
            NoiDung = request.NoiDung.Trim(),
            MucDo = NormalizeCode(request.MucDo, "TRUNG_BINH"),
            TrangThaiXuLy = "MOI",
            HanXuLy = request.HanXuLy,
            NgayPhatSinh = DateTime.UtcNow,
            NguoiNhanId = request.NguoiNhanId,
            DonViNhanId = request.DonViNhanId,
            CreatedBy = actor.UserId.ToString()
        };

        dbContext.CanhBaoKhaiThacDuLieus.Add(entity);
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
        entity.TrangThaiXuLy = entity.TrangThaiXuLy == "MOI" ? "DANG_XU_LY" : entity.TrangThaiXuLy;
        entity.NgayXem ??= DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
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
        entity.TrangThaiXuLy = "DA_XU_LY";
        entity.NguoiXuLyId = actor.UserId;
        entity.NgayXuLy = DateTime.UtcNow;
        entity.GhiChuXuLy = request.GhiChuXuLy;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
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

        return new CurrentActor(userId);
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

    private sealed record CurrentActor(Guid UserId);
}

public sealed class TraCuuKhaiThacDuLieuService : ITraCuuKhaiThacDuLieuService
{
    public Task<PagedResultDto<TraCuuTongHopItemDto>> SearchAsync(string nguonDuLieu, TraCuuRequest request, CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        return Task.FromResult(new PagedResultDto<TraCuuTongHopItemDto>([], 0, pageSize, pageCurrent));
    }
}

public sealed class BaoCaoKhaiThacDuLieuService : IBaoCaoKhaiThacDuLieuService
{
    public Task<BaoCaoTongHopDto> GetBaoCaoAsync(string loaiBaoCao, BaoCaoRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BaoCaoTongHopDto(loaiBaoCao, DateTime.UtcNow, []));
    }
}
