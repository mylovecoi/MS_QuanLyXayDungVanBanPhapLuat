using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucTieuChiDiemRepository(DanhMucDbContext dbContext) : IDanhMucTieuChiDiemRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucTieuChiDiemEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.DanhMucTieuChiDiems.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.MaTieuChi, $"%{search}%") ||
                EF.Functions.Like(x.TenTieuChi, $"%{search}%") ||
                EF.Functions.Like(x.LoaiTieuChi, $"%{search}%") ||
                EF.Functions.Like(x.KieuGiaTri, $"%{search}%") ||
                EF.Functions.Like(x.DonViGiaTri, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.ThuTuSapXep)
            .ThenBy(x => x.TenTieuChi)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DanhMucTieuChiDiemEntity
            {
                Id = x.Id,
                MaTieuChi = x.MaTieuChi,
                TenTieuChi = x.TenTieuChi,
                LoaiTieuChi = x.LoaiTieuChi,
                KieuGiaTri = x.KieuGiaTri,
                DonViGiaTri = x.DonViGiaTri,
                ThuTuSapXep = x.ThuTuSapXep,
                DiemToiDa = x.DiemToiDa,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        if (items.Count > 0)
        {
            var ids = items.Select(x => x.Id).ToList();
            var mucCounts = await _dbContext.DanhMucTieuChiDiemMucs.AsNoTracking()
                .Where(x => ids.Contains(x.DanhMucTieuChiDiemId))
                .GroupBy(x => x.DanhMucTieuChiDiemId)
                .Select(x => new { DanhMucTieuChiDiemId = x.Key, Count = x.Count() })
                .ToDictionaryAsync(x => x.DanhMucTieuChiDiemId, x => x.Count, cancellationToken);

            foreach (var item in items)
            {
                if (mucCounts.TryGetValue(item.Id, out var count) && count > 0)
                {
                    item.Mucs = Enumerable.Range(0, count)
                        .Select(_ => new DanhMucTieuChiDiemMucEntity())
                        .ToList();
                }
            }
        }

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<DanhMucTieuChiDiemEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucTieuChiDiems.AsNoTracking()
            .OrderBy(x => x.ThuTuSapXep)
            .ThenBy(x => x.TenTieuChi)
            .Select(x => new DanhMucTieuChiDiemEntity
            {
                Id = x.Id,
                MaTieuChi = x.MaTieuChi,
                TenTieuChi = x.TenTieuChi,
                LoaiTieuChi = x.LoaiTieuChi,
                KieuGiaTri = x.KieuGiaTri,
                DonViGiaTri = x.DonViGiaTri,
                ThuTuSapXep = x.ThuTuSapXep,
                DiemToiDa = x.DiemToiDa,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<DanhMucTieuChiDiemEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.DanhMucTieuChiDiems.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DanhMucTieuChiDiemEntity
            {
                Id = x.Id,
                MaTieuChi = x.MaTieuChi,
                TenTieuChi = x.TenTieuChi,
                LoaiTieuChi = x.LoaiTieuChi,
                KieuGiaTri = x.KieuGiaTri,
                DonViGiaTri = x.DonViGiaTri,
                ThuTuSapXep = x.ThuTuSapXep,
                DiemToiDa = x.DiemToiDa,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (item == null)
        {
            return null;
        }

        item.Mucs = await _dbContext.DanhMucTieuChiDiemMucs.AsNoTracking()
            .Where(x => x.DanhMucTieuChiDiemId == id)
            .OrderBy(x => x.ThuTuSapXep)
            .ThenBy(x => x.TuGiaTri)
            .Select(x => new DanhMucTieuChiDiemMucEntity
            {
                Id = x.Id,
                DanhMucTieuChiDiemId = x.DanhMucTieuChiDiemId,
                TuGiaTri = x.TuGiaTri,
                DenGiaTri = x.DenGiaTri,
                BaoGomTuGiaTri = x.BaoGomTuGiaTri,
                BaoGomDenGiaTri = x.BaoGomDenGiaTri,
                Diem = x.Diem,
                NhanHienThi = x.NhanHienThi,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        return item;
    }

    public Task<bool> ExistsByCodeAsync(string maTieuChi, Guid? ignoreId = null, CancellationToken cancellationToken = default)
        => _dbContext.DanhMucTieuChiDiems.AnyAsync(
            x => x.MaTieuChi == maTieuChi && (!ignoreId.HasValue || x.Id != ignoreId.Value),
            cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var maxValue = await _dbContext.DanhMucTieuChiDiems.AsNoTracking()
            .Select(x => (int?)x.ThuTuSapXep)
            .MaxAsync(cancellationToken);
        return (maxValue ?? 0) + 1;
    }

    public async Task AddAsync(DanhMucTieuChiDiemEntity entity, CancellationToken cancellationToken = default)
    {
        var parentId = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
        var dataEntity = new DanhMucTieuChiDiem
        {
            Id = parentId,
            MaTieuChi = entity.MaTieuChi,
            TenTieuChi = entity.TenTieuChi,
            LoaiTieuChi = entity.LoaiTieuChi,
            KieuGiaTri = entity.KieuGiaTri,
            DonViGiaTri = entity.DonViGiaTri,
            ThuTuSapXep = entity.ThuTuSapXep,
            DiemToiDa = entity.DiemToiDa,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };

        _dbContext.DanhMucTieuChiDiems.Add(dataEntity);
        _dbContext.DanhMucTieuChiDiemMucs.AddRange(entity.Mucs.Select(x => new DanhMucTieuChiDiemMuc
        {
            Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
            DanhMucTieuChiDiemId = parentId,
            TuGiaTri = x.TuGiaTri,
            DenGiaTri = x.DenGiaTri,
            BaoGomTuGiaTri = x.BaoGomTuGiaTri,
            BaoGomDenGiaTri = x.BaoGomDenGiaTri,
            Diem = x.Diem,
            NhanHienThi = x.NhanHienThi,
            ThuTuSapXep = x.ThuTuSapXep,
            TrangThai = x.TrangThai,
            GhiChu = x.GhiChu
        }));

        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = dataEntity.Id;
    }

    public async Task UpdateAsync(DanhMucTieuChiDiemEntity entity, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _dbContext.DanhMucTieuChiDiems.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dataEntity.MaTieuChi = entity.MaTieuChi;
        dataEntity.TenTieuChi = entity.TenTieuChi;
        dataEntity.LoaiTieuChi = entity.LoaiTieuChi;
        dataEntity.KieuGiaTri = entity.KieuGiaTri;
        dataEntity.DonViGiaTri = entity.DonViGiaTri;
        dataEntity.ThuTuSapXep = entity.ThuTuSapXep;
        dataEntity.DiemToiDa = entity.DiemToiDa;
        dataEntity.TrangThai = entity.TrangThai;
        dataEntity.MoTa = entity.MoTa;
        dataEntity.GhiChu = entity.GhiChu;

        var currentChildren = await _dbContext.DanhMucTieuChiDiemMucs
            .Where(x => x.DanhMucTieuChiDiemId == entity.Id)
            .ToListAsync(cancellationToken);
        _dbContext.DanhMucTieuChiDiemMucs.RemoveRange(currentChildren);

        _dbContext.DanhMucTieuChiDiemMucs.AddRange(entity.Mucs.Select(x => new DanhMucTieuChiDiemMuc
        {
            Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
            DanhMucTieuChiDiemId = entity.Id,
            TuGiaTri = x.TuGiaTri,
            DenGiaTri = x.DenGiaTri,
            BaoGomTuGiaTri = x.BaoGomTuGiaTri,
            BaoGomDenGiaTri = x.BaoGomDenGiaTri,
            Diem = x.Diem,
            NhanHienThi = x.NhanHienThi,
            ThuTuSapXep = x.ThuTuSapXep,
            TrangThai = x.TrangThai,
            GhiChu = x.GhiChu
        }));

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _dbContext.DanhMucTieuChiDiems.FirstAsync(x => x.Id == id, cancellationToken);
        _dbContext.DanhMucTieuChiDiems.Remove(dataEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

