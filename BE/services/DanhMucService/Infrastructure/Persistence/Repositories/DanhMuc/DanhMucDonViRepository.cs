using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucDonViRepository(DanhMucDbContext dbContext) : IDanhMucDonViRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucDonViEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbContext.DanhMucDonVis.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseQuery = baseQuery.Where(x => x.TenDonVi.Contains(search) || (x.DiaChi != null && x.DiaChi.Contains(search)) || (x.PhanLoaiDonVi != null && x.PhanLoaiDonVi.Contains(search)));
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var parentNames = await _dbContext.DanhMucDonVis.AsNoTracking().Select(x => new { x.Id, x.TenDonVi }).ToDictionaryAsync(x => x.Id, x => x.TenDonVi, cancellationToken);
        var items = await baseQuery.OrderBy(x => x.Level).ThenBy(x => x.STTSapXep).ThenBy(x => x.TenDonVi)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DanhMucDonViEntity
            {
                Id = x.Id,
                TenDonVi = x.TenDonVi,
                Level = x.Level,
                STTSapXep = x.STTSapXep,
                DonViChuQuanId = x.DonViChuQuanId,
                DiaChi = x.DiaChi,
                MaQHNS = x.MaQHNS,
                SoDienThoai = x.SoDienThoai,
                ChucDanhQuanLy = x.ChucDanhQuanLy,
                HoVaTenNguoiQuanLy = x.HoVaTenNguoiQuanLy,
                PhanLoaiDonVi = x.PhanLoaiDonVi,
                TinhNangThanhToan = x.TinhNangThanhToan
            }).ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.TenDonViChuQuan = parentNames.GetValueOrDefault(item.DonViChuQuanId);
        }

        return (items, totalCount);
    }

    public async Task<DanhMucDonViEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var parentNames = await _dbContext.DanhMucDonVis.AsNoTracking().Select(x => new { x.Id, x.TenDonVi }).ToDictionaryAsync(x => x.Id, x => x.TenDonVi, cancellationToken);
        var item = await _dbContext.DanhMucDonVis.AsNoTracking().Where(x => x.Id == id).Select(x => new DanhMucDonViEntity
        {
            Id = x.Id,
            TenDonVi = x.TenDonVi,
            Level = x.Level,
            STTSapXep = x.STTSapXep,
            DonViChuQuanId = x.DonViChuQuanId,
            DiaChi = x.DiaChi,
            MaQHNS = x.MaQHNS,
            SoDienThoai = x.SoDienThoai,
            ChucDanhQuanLy = x.ChucDanhQuanLy,
            HoVaTenNguoiQuanLy = x.HoVaTenNguoiQuanLy,
            PhanLoaiDonVi = x.PhanLoaiDonVi,
            TinhNangThanhToan = x.TinhNangThanhToan
        }).FirstOrDefaultAsync(cancellationToken);

        if (item != null) item.TenDonViChuQuan = parentNames.GetValueOrDefault(item.DonViChuQuanId);
        return item;
    }

    public async Task<IReadOnlyList<DanhMucDonViEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var parentNames = await _dbContext.DanhMucDonVis.AsNoTracking().Select(x => new { x.Id, x.TenDonVi }).ToDictionaryAsync(x => x.Id, x => x.TenDonVi, cancellationToken);
        var items = await _dbContext.DanhMucDonVis.AsNoTracking().OrderBy(x => x.Level).ThenBy(x => x.STTSapXep).ThenBy(x => x.TenDonVi)
            .Select(x => new DanhMucDonViEntity
            {
                Id = x.Id,
                TenDonVi = x.TenDonVi,
                Level = x.Level,
                STTSapXep = x.STTSapXep,
                DonViChuQuanId = x.DonViChuQuanId,
                DiaChi = x.DiaChi,
                MaQHNS = x.MaQHNS,
                SoDienThoai = x.SoDienThoai,
                ChucDanhQuanLy = x.ChucDanhQuanLy,
                HoVaTenNguoiQuanLy = x.HoVaTenNguoiQuanLy,
                PhanLoaiDonVi = x.PhanLoaiDonVi,
                TinhNangThanhToan = x.TinhNangThanhToan
            }).ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.TenDonViChuQuan = parentNames.GetValueOrDefault(item.DonViChuQuanId);
        }

        return items;
    }

    public Task<bool> ExistsByNameAsync(string tenDonVi, Guid? excludeId = null, CancellationToken cancellationToken = default)
        => _dbContext.DanhMucDonVis.AnyAsync(x => x.TenDonVi == tenDonVi && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public async Task<int> GetNextSortOrderAsync(Guid donViChuQuanId, CancellationToken cancellationToken = default)
    {
        var maxValue = await _dbContext.DanhMucDonVis.AsNoTracking()
            .Where(x => x.DonViChuQuanId == donViChuQuanId)
            .Select(x => (int?)x.STTSapXep)
            .MaxAsync(cancellationToken);
        return (maxValue ?? 0) + 1;
    }

    public async Task<DanhMucDonViEntity> AddAsync(DanhMucDonViEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = new DanhMucDonVi
        {
            TenDonVi = entity.TenDonVi,
            Level = entity.Level,
            STTSapXep = entity.STTSapXep,
            DonViChuQuanId = entity.DonViChuQuanId,
            DiaChi = entity.DiaChi,
            MaQHNS = entity.MaQHNS,
            SoDienThoai = entity.SoDienThoai,
            ChucDanhQuanLy = entity.ChucDanhQuanLy,
            HoVaTenNguoiQuanLy = entity.HoVaTenNguoiQuanLy,
            PhanLoaiDonVi = entity.PhanLoaiDonVi,
            TinhNangThanhToan = entity.TinhNangThanhToan
        };
        _dbContext.DanhMucDonVis.Add(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = dbEntity.Id;
        return entity;
    }

    public async Task<DanhMucDonViEntity> UpdateAsync(DanhMucDonViEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.DanhMucDonVis.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dbEntity.TenDonVi = entity.TenDonVi;
        dbEntity.Level = entity.Level;
        dbEntity.STTSapXep = entity.STTSapXep;
        dbEntity.DonViChuQuanId = entity.DonViChuQuanId;
        dbEntity.DiaChi = entity.DiaChi;
        dbEntity.MaQHNS = entity.MaQHNS;
        dbEntity.SoDienThoai = entity.SoDienThoai;
        dbEntity.ChucDanhQuanLy = entity.ChucDanhQuanLy;
        dbEntity.HoVaTenNguoiQuanLy = entity.HoVaTenNguoiQuanLy;
        dbEntity.PhanLoaiDonVi = entity.PhanLoaiDonVi;
        dbEntity.TinhNangThanhToan = entity.TinhNangThanhToan;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.DanhMucDonVis.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (dbEntity == null) return false;
        _dbContext.DanhMucDonVis.Remove(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

