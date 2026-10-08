using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucQuyTrinhSoanThaoRepository(DanhMucDbContext dbContext) : IDanhMucQuyTrinhSoanThaoRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucQuyTrinhSoanThaoEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var vanBanMap = await _dbContext.DanhMucVanBans.AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x.TenLoaiVanBan, cancellationToken);

        var query = _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.MaQuyTrinh, $"%{search}%") ||
                EF.Functions.Like(x.TenQuyTrinh, $"%{search}%") ||
                EF.Functions.Like(x.LoaiQuyTrinh, $"%{search}%") ||
                EF.Functions.Like(x.CapApDung ?? string.Empty, $"%{search}%") ||
                EF.Functions.Like(x.DanhMucVanBanIds ?? string.Empty, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.CapApDung)
            .ThenBy(x => x.MaQuyTrinh)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DanhMucQuyTrinhSoanThaoEntity
            {
                Id = x.Id,
                MaQuyTrinh = x.MaQuyTrinh,
                TenQuyTrinh = x.TenQuyTrinh,
                LoaiQuyTrinh = x.LoaiQuyTrinh,
                DanhMucVanBanId = x.DanhMucVanBanId,
                CapApDung = x.CapApDung,
                PhienBan = x.PhienBan,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        if (items.Count == 0)
        {
            return (items, totalCount);
        }

        var ids = items.Select(x => x.Id).ToArray();
        var stepCounts = await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking()
            .Where(x => ids.Contains(x.QuyTrinhSoanThaoId))
            .GroupBy(x => x.QuyTrinhSoanThaoId)
            .Select(x => new { QuyTrinhSoanThaoId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.QuyTrinhSoanThaoId, x => x.Count, cancellationToken);
        var transitionCounts = await _dbContext.DanhMucChuyenBuocQuyTrinhs.AsNoTracking()
            .Where(x => ids.Contains(x.QuyTrinhSoanThaoId))
            .GroupBy(x => x.QuyTrinhSoanThaoId)
            .Select(x => new { QuyTrinhSoanThaoId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.QuyTrinhSoanThaoId, x => x.Count, cancellationToken);
        var workflowCsvMap = await _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new { x.Id, x.DanhMucVanBanIds })
            .ToDictionaryAsync(x => x.Id, x => x.DanhMucVanBanIds, cancellationToken);

        foreach (var item in items)
        {
            item.TenLoaiQuyTrinh = FormatLoaiQuyTrinhDisplay(item.LoaiQuyTrinh);
            item.CapApDungs = ParseCapApDungList(item.CapApDung);
            item.CapApDung = FormatCapApDungDisplay(item.CapApDung);
            item.DanhMucVanBanIds = ParseGuidList(workflowCsvMap.GetValueOrDefault(item.Id), item.DanhMucVanBanId);
            item.TenLoaiVanBan = ResolveDanhMucVanBanNames(item.DanhMucVanBanIds, vanBanMap);
            item.SoBuoc = stepCounts.GetValueOrDefault(item.Id);
            item.SoNhanhChuyen = transitionCounts.GetValueOrDefault(item.Id);
        }

        return (items, totalCount);
    }

    public async Task<DanhMucQuyTrinhSoanThaoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workflow = await _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (workflow == null)
        {
            return null;
        }

        var donViMap = await _dbContext.DanhMucDonVis.AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x.TenDonVi, cancellationToken);
        var vanBanMap = await _dbContext.DanhMucVanBans.AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x.TenLoaiVanBan, cancellationToken);

        var steps = await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking()
            .Where(x => x.QuyTrinhSoanThaoId == id)
            .OrderBy(x => x.ThuTuSapXep)
            .ThenBy(x => x.MaBuoc)
            .Select(x => new DanhMucBuocQuyTrinhEntity
            {
                Id = x.Id,
                QuyTrinhSoanThaoId = x.QuyTrinhSoanThaoId,
                MaBuoc = x.MaBuoc,
                TenBuoc = x.TenBuoc,
                ThuTuSapXep = x.ThuTuSapXep,
                LoaiBuoc = x.LoaiBuoc,
                BatBuoc = x.BatBuoc,
                ChoPhepBoQua = x.ChoPhepBoQua,
                ChoPhepQuayLui = x.ChoPhepQuayLui,
                CachHoanThanh = x.CachHoanThanh,
                SoLuongPhanHoiToiThieu = x.SoLuongPhanHoiToiThieu,
                YeuCauFileDinhKem = x.YeuCauFileDinhKem,
                SoLanTraLaiToiDa = x.SoLanTraLaiToiDa,
                SoNgayXuLyTieuChuan = x.SoNgayXuLyTieuChuan,
                SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan,
                DonViTiepNhanMacDinhId = x.DonViTiepNhanMacDinhId,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        foreach (var step in steps)
        {
            if (step.DonViTiepNhanMacDinhId.HasValue && donViMap.TryGetValue(step.DonViTiepNhanMacDinhId.Value, out var tenDonVi))
            {
                step.TenDonViTiepNhanMacDinh = tenDonVi;
            }
        }

        var stepMap = steps.ToDictionary(x => x.Id, x => x.MaBuoc);
        var transitions = await _dbContext.DanhMucChuyenBuocQuyTrinhs.AsNoTracking()
            .Where(x => x.QuyTrinhSoanThaoId == id)
            .OrderBy(x => x.CreatedDate)
            .Select(x => new DanhMucChuyenBuocQuyTrinhEntity
            {
                Id = x.Id,
                QuyTrinhSoanThaoId = x.QuyTrinhSoanThaoId,
                TuBuocId = x.TuBuocId,
                DenBuocId = x.DenBuocId,
                DieuKienKetQua = x.DieuKienKetQua,
                LoaiChuyenBuoc = x.LoaiChuyenBuoc,
                LaNhanhMacDinh = x.LaNhanhMacDinh,
                YeuCauNhapLyDo = x.YeuCauNhapLyDo,
                IsKetThuc = x.IsKetThuc,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        foreach (var transition in transitions)
        {
            transition.TuBuocMa = stepMap.GetValueOrDefault(transition.TuBuocId, string.Empty);
            transition.DenBuocMa = stepMap.GetValueOrDefault(transition.DenBuocId, string.Empty);
        }

        var entity = new DanhMucQuyTrinhSoanThaoEntity
        {
            Id = workflow.Id,
            MaQuyTrinh = workflow.MaQuyTrinh,
            TenQuyTrinh = workflow.TenQuyTrinh,
            LoaiQuyTrinh = workflow.LoaiQuyTrinh,
            TenLoaiQuyTrinh = FormatLoaiQuyTrinhDisplay(workflow.LoaiQuyTrinh),
            DanhMucVanBanId = workflow.DanhMucVanBanId,
            DanhMucVanBanIds = ParseGuidList(workflow.DanhMucVanBanIds, workflow.DanhMucVanBanId),
            CapApDung = FormatCapApDungDisplay(workflow.CapApDung),
            CapApDungs = ParseCapApDungList(workflow.CapApDung),
            PhienBan = workflow.PhienBan,
            TrangThai = workflow.TrangThai,
            MoTa = workflow.MoTa,
            GhiChu = workflow.GhiChu,
            TenLoaiVanBan = ResolveDanhMucVanBanNames(ParseGuidList(workflow.DanhMucVanBanIds, workflow.DanhMucVanBanId), vanBanMap),
            SoBuoc = steps.Count,
            SoNhanhChuyen = transitions.Count,
            BuocQuyTrinhs = steps,
            ChuyenBuocs = transitions
        };

        return entity;
    }

    public Task<bool> ExistsByCodeAsync(string maQuyTrinh, Guid? ignoreId = null, CancellationToken cancellationToken = default)
        => _dbContext.DanhMucQuyTrinhSoanThaos.AnyAsync(
            x => x.MaQuyTrinh == maQuyTrinh && (!ignoreId.HasValue || x.Id != ignoreId.Value),
            cancellationToken);

    public async Task<IReadOnlyList<DanhMucLookupEntity>> GetDanhMucVanBanOptionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucVanBans.AsNoTracking()
            .OrderBy(x => x.ThuTuSapXep)
            .ThenBy(x => x.TenLoaiVanBan)
            .Select(x => new DanhMucLookupEntity
            {
                Id = x.Id,
                Ma = x.KyHieuMau ?? string.Empty,
                Ten = x.TenLoaiVanBan
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DanhMucLookupEntity>> GetDanhMucDonViOptionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucDonVis.AsNoTracking()
            .OrderBy(x => x.STTSapXep)
            .ThenBy(x => x.TenDonVi)
            .Select(x => new DanhMucLookupEntity
            {
                Id = x.Id,
                Ma = x.MaQHNS ?? string.Empty,
                Ten = x.TenDonVi
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsWorkflowUsedAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.HoSoVanBanUsages.AnyAsync(x => x.QuyTrinhSoanThaoId == id, cancellationToken);

    public Task<bool> IsAnyStepInUseAsync(IReadOnlyCollection<Guid> stepIds, CancellationToken cancellationToken = default)
    {
        if (stepIds.Count == 0)
        {
            return Task.FromResult(false);
        }

        return _dbContext.HoSoVanBanUsages.AnyAsync(x => x.BuocHienTaiId.HasValue && stepIds.Contains(x.BuocHienTaiId.Value), cancellationToken);
    }

    public async Task AddAsync(DanhMucQuyTrinhSoanThaoEntity entity, CancellationToken cancellationToken = default)
    {
        var workflowId = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
        var dataEntity = new DanhMucQuyTrinhSoanThao
        {
            Id = workflowId,
            MaQuyTrinh = entity.MaQuyTrinh,
            TenQuyTrinh = entity.TenQuyTrinh,
            LoaiQuyTrinh = entity.LoaiQuyTrinh,
            DanhMucVanBanId = entity.DanhMucVanBanIds.Count > 0 ? entity.DanhMucVanBanIds[0] : null,
            DanhMucVanBanIds = ConvertGuidListToString(entity.DanhMucVanBanIds),
            CapApDung = string.Join(",", entity.CapApDungs),
            PhienBan = entity.PhienBan > 0 ? entity.PhienBan : 1,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };

        _dbContext.DanhMucQuyTrinhSoanThaos.Add(dataEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await UpsertStepsAndTransitionsAsync(workflowId, entity, cancellationToken);
        entity.Id = workflowId;
    }

    public async Task UpdateAsync(DanhMucQuyTrinhSoanThaoEntity entity, CancellationToken cancellationToken = default)
    {
        var workflow = await _dbContext.DanhMucQuyTrinhSoanThaos.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        workflow.MaQuyTrinh = entity.MaQuyTrinh;
        workflow.TenQuyTrinh = entity.TenQuyTrinh;
        workflow.LoaiQuyTrinh = entity.LoaiQuyTrinh;
        workflow.DanhMucVanBanId = entity.DanhMucVanBanIds.Count > 0 ? entity.DanhMucVanBanIds[0] : null;
        workflow.DanhMucVanBanIds = ConvertGuidListToString(entity.DanhMucVanBanIds);
        workflow.CapApDung = string.Join(",", entity.CapApDungs);
        workflow.PhienBan = entity.PhienBan > 0 ? entity.PhienBan : 1;
        workflow.TrangThai = entity.TrangThai;
        workflow.MoTa = entity.MoTa;
        workflow.GhiChu = entity.GhiChu;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var currentTransitions = await _dbContext.DanhMucChuyenBuocQuyTrinhs
            .Where(x => x.QuyTrinhSoanThaoId == entity.Id)
            .ToListAsync(cancellationToken);
        if (currentTransitions.Count > 0)
        {
            _dbContext.DanhMucChuyenBuocQuyTrinhs.RemoveRange(currentTransitions);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var currentSteps = await _dbContext.DanhMucBuocQuyTrinhs
            .Where(x => x.QuyTrinhSoanThaoId == entity.Id)
            .ToListAsync(cancellationToken);
        var requestedStepIds = entity.BuocQuyTrinhs.Where(x => x.Id != Guid.Empty).Select(x => x.Id).ToHashSet();
        var deletedSteps = currentSteps.Where(x => !requestedStepIds.Contains(x.Id)).ToList();
        if (deletedSteps.Count > 0)
        {
            _dbContext.DanhMucBuocQuyTrinhs.RemoveRange(deletedSteps);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await UpsertStepsAndTransitionsAsync(entity.Id, entity, cancellationToken, currentSteps.Except(deletedSteps).ToList());
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transitions = await _dbContext.DanhMucChuyenBuocQuyTrinhs.Where(x => x.QuyTrinhSoanThaoId == id).ToListAsync(cancellationToken);
        if (transitions.Count > 0)
        {
            _dbContext.DanhMucChuyenBuocQuyTrinhs.RemoveRange(transitions);
        }

        var steps = await _dbContext.DanhMucBuocQuyTrinhs.Where(x => x.QuyTrinhSoanThaoId == id).ToListAsync(cancellationToken);
        if (steps.Count > 0)
        {
            _dbContext.DanhMucBuocQuyTrinhs.RemoveRange(steps);
        }

        var workflow = await _dbContext.DanhMucQuyTrinhSoanThaos.FirstAsync(x => x.Id == id, cancellationToken);
        _dbContext.DanhMucQuyTrinhSoanThaos.Remove(workflow);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task UpsertStepsAndTransitionsAsync(
        Guid workflowId,
        DanhMucQuyTrinhSoanThaoEntity entity,
        CancellationToken cancellationToken,
        List<DanhMucBuocQuyTrinh>? existingSteps = null)
    {
        existingSteps ??= new List<DanhMucBuocQuyTrinh>();
        var existingMap = existingSteps.ToDictionary(x => x.Id, x => x);
        var codeMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var step in entity.BuocQuyTrinhs)
        {
            DanhMucBuocQuyTrinh dataStep;
            if (step.Id != Guid.Empty && existingMap.TryGetValue(step.Id, out var current))
            {
                dataStep = current;
            }
            else
            {
                dataStep = new DanhMucBuocQuyTrinh
                {
                    Id = step.Id == Guid.Empty ? Guid.NewGuid() : step.Id,
                    QuyTrinhSoanThaoId = workflowId
                };
                _dbContext.DanhMucBuocQuyTrinhs.Add(dataStep);
            }

            dataStep.QuyTrinhSoanThaoId = workflowId;
            dataStep.MaBuoc = step.MaBuoc;
            dataStep.TenBuoc = step.TenBuoc;
            dataStep.ThuTuSapXep = step.ThuTuSapXep <= 0 ? 1 : step.ThuTuSapXep;
            dataStep.LoaiBuoc = step.LoaiBuoc;
            dataStep.BatBuoc = step.BatBuoc;
            dataStep.ChoPhepBoQua = step.ChoPhepBoQua;
            dataStep.ChoPhepQuayLui = step.ChoPhepQuayLui;
            dataStep.CachHoanThanh = step.CachHoanThanh;
            dataStep.SoLuongPhanHoiToiThieu = step.SoLuongPhanHoiToiThieu;
            dataStep.YeuCauFileDinhKem = step.YeuCauFileDinhKem;
            dataStep.SoLanTraLaiToiDa = step.SoLanTraLaiToiDa < 0 ? 0 : step.SoLanTraLaiToiDa;
            dataStep.SoNgayXuLyTieuChuan = step.SoNgayXuLyTieuChuan.HasValue && step.SoNgayXuLyTieuChuan.Value > 0 ? step.SoNgayXuLyTieuChuan : null;
            dataStep.SoNgayCanhBaoSapHan = step.SoNgayCanhBaoSapHan.HasValue && step.SoNgayCanhBaoSapHan.Value >= 0 ? step.SoNgayCanhBaoSapHan : null;
            dataStep.DonViTiepNhanMacDinhId = step.DonViTiepNhanMacDinhId;
            dataStep.MoTa = step.MoTa;
            dataStep.GhiChu = step.GhiChu;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var refreshedSteps = await _dbContext.DanhMucBuocQuyTrinhs
            .Where(x => x.QuyTrinhSoanThaoId == workflowId)
            .ToListAsync(cancellationToken);
        foreach (var step in refreshedSteps)
        {
            codeMap[step.MaBuoc] = step.Id;
        }

        var newTransitions = entity.ChuyenBuocs.Select(x => new DanhMucChuyenBuocQuyTrinh
        {
            Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
            QuyTrinhSoanThaoId = workflowId,
            TuBuocId = codeMap[x.TuBuocMa],
            DenBuocId = codeMap[x.DenBuocMa],
            DieuKienKetQua = x.DieuKienKetQua,
            LoaiChuyenBuoc = string.IsNullOrWhiteSpace(x.LoaiChuyenBuoc) ? "Forward" : x.LoaiChuyenBuoc.Trim(),
            LaNhanhMacDinh = x.LaNhanhMacDinh,
            YeuCauNhapLyDo = x.YeuCauNhapLyDo,
            IsKetThuc = x.IsKetThuc,
            MoTa = x.MoTa,
            GhiChu = x.GhiChu
        }).ToList();

        if (newTransitions.Count > 0)
        {
            _dbContext.DanhMucChuyenBuocQuyTrinhs.AddRange(newTransitions);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static List<Guid> ParseGuidList(string? value, Guid? fallbackValue)
    {
        var result = (value ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(item => Guid.TryParse(item, out var parsed) ? parsed : Guid.Empty)
            .Where(item => item != Guid.Empty)
            .Distinct()
            .ToList();

        if (result.Count == 0 && fallbackValue.HasValue && fallbackValue.Value != Guid.Empty)
        {
            result.Add(fallbackValue.Value);
        }

        return result;
    }

    private static string ConvertGuidListToString(IEnumerable<Guid> values)
        => string.Join(",", values.Where(x => x != Guid.Empty).Distinct());

    private static List<string> ParseCapApDungList(string? value)
    {
        var result = (value ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizeCapApDungValue)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        return result.Count > 0 ? result : new List<string> { "Tinh" };
    }

    private static string NormalizeCapApDungValue(string? value)
        => (value ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            "TINH" => "Tinh",
            "XA" => "Xa",
            _ => (value ?? string.Empty).Trim()
        };

    private static string FormatLoaiQuyTrinhDisplay(string? loaiQuyTrinh)
        => (loaiQuyTrinh ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            "DANGKY" => "Đăng ký",
            "DANG_KY" => "Đăng ký",
            _ => "Xây dựng"
        };

    private static string? ResolveDanhMucVanBanNames(IEnumerable<Guid> ids, IReadOnlyDictionary<Guid, string> vanBanMap)
    {
        var names = ids
            .Where(vanBanMap.ContainsKey)
            .Select(id => vanBanMap[id])
            .Distinct()
            .ToList();
        return names.Count == 0 ? null : string.Join(", ", names);
    }

    private static string? FormatCapApDungDisplay(string? capApDung)
    {
        if (string.IsNullOrWhiteSpace(capApDung))
        {
            return capApDung;
        }

        return string.Join(", ",
            capApDung
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(item => item.Equals("Tinh", StringComparison.OrdinalIgnoreCase) ? "Tỉnh" :
                                item.Equals("Xa", StringComparison.OrdinalIgnoreCase) ? "Xã" :
                                item)
                .Distinct(StringComparer.OrdinalIgnoreCase));
    }
}

