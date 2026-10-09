using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanTrinhThamDinhService(
    XayDungVanBanDbContext dbContext,
    ICurrentUserContext currentUserContext,
    IWebHostEnvironment environment) : IXayDungVanBanTrinhThamDinhService
{
    public async Task<IReadOnlyList<HoSoTrinhThamDinhListItemDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await (from bo in dbContext.BoHoSoNghiepVus.AsNoTracking()
                           join detail in dbContext.HoSoXayDungVanBanTrinhThamDinhs.AsNoTracking() on bo.Id equals detail.BoHoSoNghiepVuId
                           join hoSo in dbContext.HoSoXayDungVanBans.AsNoTracking() on bo.HoSoXayDungVanBanId equals hoSo.Id
                           where bo.LoaiBoHoSo == LoaiBoHoSo.TrinhThamDinh && !bo.IsDeleted && !hoSo.IsDeleted
                           orderby bo.CreatedAt descending
                           select new
                           {
                               HoSoId = hoSo.Id,
                               BoHoSoId = bo.Id,
                               hoSo.MaHoSo,
                               hoSo.TenHoSo,
                               hoSo.TenDuThaoVanBan,
                               hoSo.NamXayDung,
                               TrangThai = bo.TrangThai == TrangThaiBoHoSo.Nhap && bo.LyDoTraLai != null && bo.LyDoTraLai != "" ? "BiTraLai" : bo.TrangThai.ToString(),
                               detail.DonViNhanThamDinhId,
                               bo.NgayTao,
                               detail.NgayGuiThamDinh,
                               bo.LyDoTraLai
                           }).ToListAsync(cancellationToken);

        var hoSoIds = items.Select(item => item.HoSoId).Distinct().ToList();
        var returnCounts = await dbContext.HoSoXayDungVanBanLichSuXuLys.AsNoTracking()
            .Where(x => hoSoIds.Contains(x.HoSoXayDungVanBanId) && x.HanhDong == "TRA_LAI_TRINH_THAM_DINH" && !x.IsDeleted)
            .GroupBy(x => x.HoSoXayDungVanBanId)
            .Select(x => new { HoSoId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.HoSoId, x => x.Count, cancellationToken);

        return items.Select(item =>
        {
            var soLanTraLai = Math.Max(ExtractSoLanTraLai(item.LyDoTraLai), returnCounts.GetValueOrDefault(item.HoSoId));
            return new HoSoTrinhThamDinhListItemDto(
            item.HoSoId,
            item.BoHoSoId,
            item.MaHoSo,
            item.TenHoSo,
            item.TenDuThaoVanBan,
            item.NamXayDung,
            item.TrangThai,
            item.DonViNhanThamDinhId,
            item.NgayTao,
            item.NgayGuiThamDinh,
            soLanTraLai,
            item.LyDoTraLai);
        }).ToList();
    }

    public async Task<IReadOnlyList<HoSoNguonTrinhThamDinhDto>> GetNguonKeThuaAsync(CancellationToken cancellationToken = default) =>
        await (from hoSo in dbContext.HoSoXayDungVanBans.AsNoTracking()
               where !hoSo.IsDeleted
                     && dbContext.BoHoSoNghiepVus.Any(bo => bo.HoSoXayDungVanBanId == hoSo.Id && bo.LoaiBoHoSo == LoaiBoHoSo.SoanThao && bo.TrangThai == TrangThaiBoHoSo.Nhap && !bo.IsDeleted)
                     && !dbContext.BoHoSoNghiepVus.Any(bo => bo.HoSoXayDungVanBanId == hoSo.Id && bo.LoaiBoHoSo == LoaiBoHoSo.TrinhThamDinh && !bo.IsDeleted)
               orderby hoSo.UpdatedAt descending, hoSo.CreatedAt descending
               select new HoSoNguonTrinhThamDinhDto(hoSo.Id, hoSo.MaHoSo, hoSo.TenHoSo, hoSo.TenDuThaoVanBan, hoSo.NamXayDung, hoSo.QuyTrinhSoanThaoId, hoSo.BuocHienTaiId, hoSo.TrangThaiHoSoId))
            .ToListAsync(cancellationToken);

    public async Task<XayDungVanBanTrinhThamDinhDto> CreateAsync(TaoHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken = default)
    {
        if (request.HoSoId == Guid.Empty || request.BuocQuyTrinhTiepTheoId == Guid.Empty || request.DonViNhanThamDinhId == Guid.Empty)
            throw new InvalidOperationException("Thiếu hồ sơ, bước tiếp theo hoặc đơn vị nhận thẩm định.");

        var source = await GetSourceDraftAsync(request.HoSoId, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy bộ hồ sơ soạn thảo đang xử lý.");
        var actor = RequireActor(source.DonViLapId);
        if (await GetDataAsync(request.HoSoId, true, cancellationToken) is not null)
            throw new InvalidOperationException("Hồ sơ đã có bộ trình thẩm định chưa hủy.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var boHoSo = new BoHoSoNghiepVu
        {
            HoSoXayDungVanBanId = request.HoSoId,
            BuocQuyTrinhId = request.BuocQuyTrinhTiepTheoId,
            LoaiBoHoSo = LoaiBoHoSo.TrinhThamDinh,
            TrangThai = TrangThaiBoHoSo.Nhap,
            LanXuLy = await NextLanXuLyAsync(request.HoSoId, request.BuocQuyTrinhTiepTheoId, cancellationToken),
            BoHoSoNguonId = source.Id,
            NguoiLapId = actor.UserId,
            DonViLapId = actor.DonViId,
            NoiDungGhiChu = request.NoiDungGhiChu,
            CreatedBy = actor.UserId.ToString()
        };
        dbContext.BoHoSoNghiepVus.Add(boHoSo);
        dbContext.HoSoXayDungVanBanTrinhThamDinhs.Add(new HoSoXayDungVanBanTrinhThamDinh
        {
            BoHoSoNghiepVuId = boHoSo.Id,
            DonViNhanThamDinhId = request.DonViNhanThamDinhId
        });

        var sourceDocuments = await (
            from link in dbContext.BoHoSoNghiepVuTaiLieus
            join file in dbContext.HoSoXayDungVanBanFiles on link.HoSoXayDungVanBanFileId equals file.Id
            where link.BoHoSoNghiepVuId == source.Id && !link.IsDeleted && !file.IsDeleted && file.IsCurrent
            select link).ToListAsync(cancellationToken);
        foreach (var sourceDocument in sourceDocuments)
        {
            dbContext.BoHoSoNghiepVuTaiLieus.Add(new BoHoSoNghiepVuTaiLieu
            {
                BoHoSoNghiepVuId = boHoSo.Id,
                HoSoXayDungVanBanFileId = sourceDocument.HoSoXayDungVanBanFileId,
                LoaiTaiLieuId = sourceDocument.LoaiTaiLieuId,
                HinhThucThem = "KeThua",
                BoHoSoTaiLieuNguonId = sourceDocument.Id,
                BatBuoc = sourceDocument.BatBuoc,
                ThuTu = sourceDocument.ThuTu,
                CreatedBy = actor.UserId.ToString()
            });
        }
        AddTimeline(request.HoSoId, boHoSo.Id, "TAO_HO_SO_TRINH_THAM_DINH", "Tạo hồ sơ trình thẩm định", actor);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ToDto(request.HoSoId, boHoSo, dbContext.HoSoXayDungVanBanTrinhThamDinhs.Local.Single(x => x.BoHoSoNghiepVuId == boHoSo.Id));
    }

    public async Task<XayDungVanBanTrinhThamDinhDto?> GetByHoSoIdAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await GetDataAsync(hoSoId, false, cancellationToken);
        return data is null ? null : await ToDtoAsync(data.HoSoId, data.BoHoSo, data.ChiTiet, cancellationToken);
    }

    public async Task<XayDungVanBanTrinhThamDinhDto?> UpdateAsync(Guid hoSoId, CapNhatHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken = default)
    {
        if (request.DonViNhanThamDinhId == Guid.Empty) throw new InvalidOperationException("Chưa chọn đơn vị nhận thẩm định.");
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        var actor = RequireActor(data.BoHoSo.DonViLapId);
        EnsureNhap(data.BoHoSo);
        data.ChiTiet.SoToTrinh = request.SoToTrinh?.Trim();
        data.ChiTiet.NgayToTrinh = request.NgayToTrinh;
        data.ChiTiet.DonViNhanThamDinhId = request.DonViNhanThamDinhId;
        data.ChiTiet.NoiDungDeNghiThamDinh = request.NoiDungDeNghiThamDinh;
        data.ChiTiet.HanDeNghiTraKetQua = request.HanDeNghiTraKetQua;
        data.BoHoSo.NoiDungGhiChu = request.NoiDungGhiChu;
        data.BoHoSo.UpdatedAt = DateTime.UtcNow;
        data.BoHoSo.UpdatedBy = actor.UserId.ToString();
        AddTimeline(hoSoId, data.BoHoSo.Id, "CAP_NHAT_HO_SO_TRINH_THAM_DINH", "Cập nhật hồ sơ trình thẩm định", actor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(hoSoId, data.BoHoSo, data.ChiTiet);
    }

    public async Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await GetDataAsync(hoSoId, false, cancellationToken);
        if (data is null) return null;
        return await (
            from link in dbContext.BoHoSoNghiepVuTaiLieus.AsNoTracking()
            join file in dbContext.HoSoXayDungVanBanFiles.AsNoTracking() on link.HoSoXayDungVanBanFileId equals file.Id
            where link.BoHoSoNghiepVuId == data.BoHoSo.Id && !link.IsDeleted && !file.IsDeleted
            orderby link.ThuTu, file.TenTaiLieu, file.PhienBan descending
            select new XayDungVanBanTaiLieuDto(
                file.Id, file.LoaiTaiLieuId, file.TenTaiLieu, file.PhienBan, file.TenFile, file.DuongDanFile,
                file.MimeType, file.DungLuong, file.IsCurrent, file.NgayTaiLen, link.Id, link.HinhThucThem,
                file.Id == data.ChiTiet.FileDuThaoId ? "DU_THAO" : (link.HinhThucThem == "KeThua" ? "KE_THUA" : (link.GhiChu == "DU_THAO" ? "DU_THAO" : "TO_TRINH"))))
            .ToListAsync(cancellationToken);
    }

    public async Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid hoSoId, TaiTaiLieuTrinhThamDinhRequest request, CancellationToken cancellationToken = default)
    {
        if (request.LoaiTaiLieuId == Guid.Empty || request.NoiDung.Length == 0 || string.IsNullOrWhiteSpace(request.TenTaiLieu))
            throw new InvalidOperationException("Thiếu loại, tên tài liệu hoặc file tải lên.");
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        var actor = RequireActor(data.BoHoSo.DonViLapId);
        EnsureNhap(data.BoHoSo);
        var safeFileName = Path.GetFileName(request.TenFile);
        if (string.IsNullOrWhiteSpace(safeFileName)) throw new InvalidOperationException("Tên file không hợp lệ.");
        var loaiDinhKem = request.LoaiDinhKem == "DU_THAO" ? "DU_THAO" : "TO_TRINH";
        if (loaiDinhKem == "DU_THAO" && !safeFileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("File dự thảo phải có định dạng .docx.");

        var relativeDirectory = Path.Combine("uploads", "xay-dung-van-ban", hoSoId.ToString("N"));
        var physicalDirectory = Path.Combine(environment.ContentRootPath, relativeDirectory);
        Directory.CreateDirectory(physicalDirectory);
        var storageFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{safeFileName}";
        var physicalPath = Path.Combine(physicalDirectory, storageFileName);
        try
        {
            await using (var output = File.Create(physicalPath)) await request.NoiDung.CopyToAsync(output, cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var existing = await dbContext.HoSoXayDungVanBanFiles.Where(x => x.HoSoXayDungVanBanId == hoSoId && x.LoaiTaiLieuId == request.LoaiTaiLieuId && !x.IsDeleted).ToListAsync(cancellationToken);
            foreach (var item in existing.Where(x => x.IsCurrent)) item.IsCurrent = false;
            var file = new HoSoXayDungVanBanFile
            {
                HoSoXayDungVanBanId = hoSoId, LoaiTaiLieuId = request.LoaiTaiLieuId, TenTaiLieu = request.TenTaiLieu.Trim(),
                PhienBan = existing.Count == 0 ? 1 : existing.Max(x => x.PhienBan) + 1, TenFile = safeFileName,
                DuongDanFile = Path.Combine(relativeDirectory, storageFileName).Replace('\\', '/'), MimeType = request.MimeType,
                DungLuong = request.NoiDung.Length, NguoiTaiLenId = actor.UserId, CreatedBy = actor.UserId.ToString()
            };
            dbContext.HoSoXayDungVanBanFiles.Add(file);
            dbContext.BoHoSoNghiepVuTaiLieus.Add(new BoHoSoNghiepVuTaiLieu { BoHoSoNghiepVuId = data.BoHoSo.Id, HoSoXayDungVanBanFileId = file.Id, LoaiTaiLieuId = file.LoaiTaiLieuId, HinhThucThem = "TaoMoi", GhiChu = loaiDinhKem, ThuTu = file.PhienBan, CreatedBy = actor.UserId.ToString() });
            AddTimeline(hoSoId, data.BoHoSo.Id, "TAI_TAI_LIEU_TRINH_THAM_DINH", $"Tải tài liệu {file.TenTaiLieu}", actor, file.Id);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return ToTaiLieuDto(file);
        }
        catch
        {
            if (File.Exists(physicalPath)) File.Delete(physicalPath);
            throw;
        }
    }

    public async Task<bool> DeleteTaiLieuAsync(Guid hoSoId, Guid boHoSoTaiLieuId, CancellationToken cancellationToken = default)
    {
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return false;
        var actor = RequireActor(data.BoHoSo.DonViLapId);
        EnsureNhap(data.BoHoSo);
        var link = await dbContext.BoHoSoNghiepVuTaiLieus.FirstOrDefaultAsync(x => x.Id == boHoSoTaiLieuId && x.BoHoSoNghiepVuId == data.BoHoSo.Id && !x.IsDeleted, cancellationToken);
        if (link is null) return false;
        if (link.HinhThucThem == "KeThua") throw new InvalidOperationException("Không thể gỡ tài liệu kế thừa từ hồ sơ soạn thảo.");
        link.IsDeleted = true; link.UpdatedAt = DateTime.UtcNow; link.UpdatedBy = actor.UserId.ToString();
        AddTimeline(hoSoId, data.BoHoSo.Id, "GO_TAI_LIEU_TRINH_THAM_DINH", "Gỡ tài liệu bổ sung", actor, link.HoSoXayDungVanBanFileId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DieuKienGuiThamDinhDto?> KiemTraTruocGuiAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await GetDataAsync(hoSoId, false, cancellationToken);
        if (data is null) return null;
        var conditions = await GetConditionsAsync(data, cancellationToken);
        return new DieuKienGuiThamDinhDto(conditions.Count == 0, conditions);
    }

    public async Task<XayDungVanBanTrinhThamDinhDto?> GuiAsync(Guid hoSoId, GuiThamDinhRequest request, CancellationToken cancellationToken = default)
    {
        if (request.BuocQuyTrinhTiepTheoId == Guid.Empty || request.TrangThaiHoSoTiepTheoId == Guid.Empty || request.FileDuThaoId == Guid.Empty) throw new InvalidOperationException("Thiếu bước, trạng thái hoặc file dự thảo.");
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        var actor = RequireActor(data.BoHoSo.DonViLapId);
        EnsureNhap(data.BoHoSo);
        if (data.BoHoSo.BuocQuyTrinhId != request.BuocQuyTrinhTiepTheoId)
            throw new InvalidOperationException("Bước gửi thẩm định không khớp với bộ hồ sơ đã tạo.");
        var conditions = await GetConditionsAsync(data, cancellationToken);
        if (conditions.Count > 0) throw new InvalidOperationException(string.Join(" ", conditions));
        var duThaoDocx = await (from link in dbContext.BoHoSoNghiepVuTaiLieus
                                 join file in dbContext.HoSoXayDungVanBanFiles on link.HoSoXayDungVanBanFileId equals file.Id
                                 where link.BoHoSoNghiepVuId == data.BoHoSo.Id && link.HoSoXayDungVanBanFileId == request.FileDuThaoId && !link.IsDeleted && !file.IsDeleted && file.TenFile.EndsWith(".docx")
                                 select file).FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("File dự thảo phải là tài liệu .docx thuộc hồ sơ trình thẩm định.");
        var latestReturn = await GetLatestReturnAsync(hoSoId, cancellationToken);
        if (latestReturn is not null && duThaoDocx.NgayTaiLen <= latestReturn.CreatedAt)
        {
            var soLanTraLai = await GetSoLanTraLaiAsync(hoSoId, cancellationToken);
            throw new InvalidOperationException($"Hồ sơ đã bị trả lại lần {soLanTraLai}. Vui lòng đính kèm file dự thảo lần {soLanTraLai + 1} trước khi gửi lại.");
        }
        var daGuiFileNay = await (from submitted in dbContext.HoSoXayDungVanBanTrinhThamDinhs
                                   join submittedBo in dbContext.BoHoSoNghiepVus on submitted.BoHoSoNghiepVuId equals submittedBo.Id
                                   where submittedBo.HoSoXayDungVanBanId == hoSoId && submittedBo.LoaiBoHoSo == LoaiBoHoSo.TrinhThamDinh && submittedBo.TrangThai == TrangThaiBoHoSo.DaGui && submitted.FileDuThaoId == request.FileDuThaoId && !submittedBo.IsDeleted
                                   select submitted.BoHoSoNghiepVuId).AnyAsync(cancellationToken);
        if (daGuiFileNay) throw new InvalidOperationException("File dự thảo này đã được chốt ở lần gửi thẩm định trước. Vui lòng chọn phiên bản .docx mới.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var source = await dbContext.BoHoSoNghiepVus.FirstAsync(x => x.Id == data.BoHoSo.BoHoSoNguonId, cancellationToken);
        var now = DateTime.UtcNow;
        source.TrangThai = TrangThaiBoHoSo.DaGui; source.NgayGui = now; source.UpdatedAt = now; source.UpdatedBy = actor.UserId.ToString();
        data.BoHoSo.TrangThai = TrangThaiBoHoSo.DaGui; data.BoHoSo.NgayGui = now; data.BoHoSo.LyDoTraLai = null; data.BoHoSo.UpdatedAt = now; data.BoHoSo.UpdatedBy = actor.UserId.ToString();
        data.ChiTiet.NgayGuiThamDinh = now;
        data.ChiTiet.FileDuThaoId = duThaoDocx.Id;
        var hoSo = await dbContext.HoSoXayDungVanBans.FirstAsync(x => x.Id == hoSoId, cancellationToken);
        var previousBuoc = hoSo.BuocHienTaiId; var previousTrangThai = hoSo.TrangThaiHoSoId;
        hoSo.BuocHienTaiId = request.BuocQuyTrinhTiepTheoId; hoSo.TrangThaiHoSoId = request.TrangThaiHoSoTiepTheoId; hoSo.UpdatedAt = now; hoSo.UpdatedBy = actor.UserId.ToString();
        AddTimeline(hoSoId, data.BoHoSo.Id, "GUI_THAM_DINH", $"Gửi hồ sơ thẩm định lần {data.BoHoSo.LanXuLy}; file dự thảo: {duThaoDocx.TenFile}", actor, duThaoDocx.Id, previousBuoc, request.BuocQuyTrinhTiepTheoId, previousTrangThai, request.TrangThaiHoSoTiepTheoId);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ToDto(hoSoId, data.BoHoSo, data.ChiTiet);
    }

    public async Task<bool> HuyAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return false;
        var actor = RequireActor(data.BoHoSo.DonViLapId);
        EnsureNhap(data.BoHoSo);
        data.BoHoSo.IsDeleted = true; data.BoHoSo.UpdatedAt = DateTime.UtcNow; data.BoHoSo.UpdatedBy = actor.UserId.ToString();
        AddTimeline(hoSoId, data.BoHoSo.Id, "HUY_TRINH_THAM_DINH", "Hủy hồ sơ trình thẩm định", actor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<List<string>> GetConditionsAsync(Data data, CancellationToken cancellationToken)
    {
        var conditions = new List<string>();
        if (string.IsNullOrWhiteSpace(data.ChiTiet.NoiDungDeNghiThamDinh)) conditions.Add("Chưa cập nhật nội dung đề nghị thẩm định.");
        if (data.ChiTiet.DonViNhanThamDinhId == Guid.Empty) conditions.Add("Chưa chọn đơn vị nhận thẩm định.");
        var hasDocuments = await dbContext.BoHoSoNghiepVuTaiLieus.AnyAsync(x => x.BoHoSoNghiepVuId == data.BoHoSo.Id && !x.IsDeleted, cancellationToken);
        if (!hasDocuments) conditions.Add("Chưa có tài liệu trình thẩm định.");
        if (await GetDuThaoDocxAsync(data, cancellationToken) is null) conditions.Add("Bắt buộc phải có file dự thảo định dạng .docx trước khi gửi thẩm định.");
        var latestReturn = await GetLatestReturnAsync(data.HoSoId, cancellationToken);
        if (latestReturn is not null && !await HasDraftAfterAsync(data.BoHoSo.Id, latestReturn.CreatedAt, cancellationToken))
        {
            var soLanTraLai = await GetSoLanTraLaiAsync(data.HoSoId, cancellationToken);
            conditions.Add($"Hồ sơ đã bị trả lại lần {soLanTraLai}. Cần đính kèm file dự thảo lần {soLanTraLai + 1} trước khi gửi lại.");
        }
        return conditions;
    }

    private async Task<HoSoXayDungVanBanLichSuXuLy?> GetLatestReturnAsync(Guid hoSoId, CancellationToken cancellationToken) =>
        await dbContext.HoSoXayDungVanBanLichSuXuLys.AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && x.HanhDong == "TRA_LAI_TRINH_THAM_DINH" && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    private async Task<int> GetSoLanTraLaiAsync(Guid hoSoId, CancellationToken cancellationToken) =>
        await dbContext.HoSoXayDungVanBanLichSuXuLys.AsNoTracking()
            .CountAsync(x => x.HoSoXayDungVanBanId == hoSoId && x.HanhDong == "TRA_LAI_TRINH_THAM_DINH" && !x.IsDeleted, cancellationToken);

    private async Task<bool> HasDraftAfterAsync(Guid boHoSoId, DateTime after, CancellationToken cancellationToken) =>
        await (from link in dbContext.BoHoSoNghiepVuTaiLieus.AsNoTracking()
               join file in dbContext.HoSoXayDungVanBanFiles.AsNoTracking() on link.HoSoXayDungVanBanFileId equals file.Id
               where link.BoHoSoNghiepVuId == boHoSoId && !link.IsDeleted && !file.IsDeleted && file.TenFile.EndsWith(".docx") && file.NgayTaiLen > after
               select file.Id).AnyAsync(cancellationToken);

    private async Task<HoSoXayDungVanBanFile?> GetDuThaoDocxAsync(Data data, CancellationToken cancellationToken) => await (
        from link in dbContext.BoHoSoNghiepVuTaiLieus.AsNoTracking()
        join file in dbContext.HoSoXayDungVanBanFiles.AsNoTracking() on link.HoSoXayDungVanBanFileId equals file.Id
        where link.BoHoSoNghiepVuId == data.BoHoSo.Id && !link.IsDeleted && !file.IsDeleted && file.TenFile.EndsWith(".docx")
        orderby file.NgayTaiLen descending
        select file).FirstOrDefaultAsync(cancellationToken);

    private async Task<BoHoSoNghiepVu?> GetSourceDraftAsync(Guid hoSoId, CancellationToken cancellationToken) => await dbContext.BoHoSoNghiepVus
        .OrderByDescending(x => x.LanXuLy).FirstOrDefaultAsync(x => x.HoSoXayDungVanBanId == hoSoId && x.LoaiBoHoSo == LoaiBoHoSo.SoanThao && x.TrangThai == TrangThaiBoHoSo.Nhap && !x.IsDeleted, cancellationToken);

    private async Task<Data?> GetDataAsync(Guid hoSoId, bool tracking, CancellationToken cancellationToken)
    {
        var bo = tracking ? dbContext.BoHoSoNghiepVus : dbContext.BoHoSoNghiepVus.AsNoTracking();
        var detail = tracking ? dbContext.HoSoXayDungVanBanTrinhThamDinhs : dbContext.HoSoXayDungVanBanTrinhThamDinhs.AsNoTracking();
        return await (from b in bo join d in detail on b.Id equals d.BoHoSoNghiepVuId
            where b.HoSoXayDungVanBanId == hoSoId && b.LoaiBoHoSo == LoaiBoHoSo.TrinhThamDinh && !b.IsDeleted
            orderby b.CreatedAt descending select new Data(hoSoId, b, d)).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<int> NextLanXuLyAsync(Guid hoSoId, Guid buocId, CancellationToken cancellationToken) =>
        (await dbContext.BoHoSoNghiepVus.Where(x => x.HoSoXayDungVanBanId == hoSoId && x.BuocQuyTrinhId == buocId).Select(x => (int?)x.LanXuLy).MaxAsync(cancellationToken) ?? 0) + 1;

    private static void EnsureNhap(BoHoSoNghiepVu boHoSo)
    {
        if (boHoSo.TrangThai != TrangThaiBoHoSo.Nhap) throw new InvalidOperationException("Bộ hồ sơ không còn ở trạng thái nhập.");
    }

    private CurrentActor RequireActor(Guid? fallbackDonViId = null)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid userId)
            throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý.");
        var donViId = currentUserContext.DonViId is { } currentDonViId && currentDonViId != Guid.Empty ? currentDonViId : fallbackDonViId;
        if (donViId is null || donViId == Guid.Empty) throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý.");
        return new CurrentActor(userId, donViId.Value);
    }

    private void AddTimeline(Guid hoSoId, Guid boHoSoId, string hanhDong, string noiDung, CurrentActor actor, Guid? fileId = null, Guid? buocTruoc = null, Guid? buocSau = null, Guid? trangThaiTruoc = null, Guid? trangThaiSau = null) =>
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(new HoSoXayDungVanBanLichSuXuLy { HoSoXayDungVanBanId = hoSoId, BoHoSoNghiepVuId = boHoSoId, HanhDong = hanhDong, NoiDung = noiDung, NguoiXuLyId = actor.UserId, DonViXuLyId = actor.DonViId, HoSoXayDungVanBanFileId = fileId, BuocQuyTrinhTruocId = buocTruoc, BuocQuyTrinhSauId = buocSau, TrangThaiTruocId = trangThaiTruoc, TrangThaiSauId = trangThaiSau, CreatedBy = actor.UserId.ToString() });

    private static string ToTrangThaiHienThi(BoHoSoNghiepVu boHoSo) =>
        boHoSo.TrangThai == TrangThaiBoHoSo.Nhap && !string.IsNullOrWhiteSpace(boHoSo.LyDoTraLai)
            ? "BiTraLai"
            : boHoSo.TrangThai.ToString();

    private static int ExtractSoLanTraLai(string? lyDoTraLai)
    {
        if (string.IsNullOrWhiteSpace(lyDoTraLai) || !lyDoTraLai.StartsWith("Trả lại lần ", StringComparison.OrdinalIgnoreCase)) return 0;
        var start = "Trả lại lần ".Length;
        var end = lyDoTraLai.IndexOf(':', start);
        return end > start && int.TryParse(lyDoTraLai[start..end].Trim(), out var value) ? value : 0;
    }

    private async Task<XayDungVanBanTrinhThamDinhDto> ToDtoAsync(Guid hoSoId, BoHoSoNghiepVu boHoSo, HoSoXayDungVanBanTrinhThamDinh detail, CancellationToken cancellationToken)
    {
        var soLanTraLai = Math.Max(ExtractSoLanTraLai(boHoSo.LyDoTraLai), await GetSoLanTraLaiAsync(hoSoId, cancellationToken));
        return new(hoSoId, boHoSo.Id, boHoSo.BoHoSoNguonId ?? Guid.Empty, detail.FileDuThaoId, boHoSo.BuocQuyTrinhId, ToTrangThaiHienThi(boHoSo), detail.SoToTrinh, detail.NgayToTrinh, detail.NgayGuiThamDinh, detail.DonViNhanThamDinhId, detail.NoiDungDeNghiThamDinh, detail.HanDeNghiTraKetQua, boHoSo.NoiDungGhiChu, soLanTraLai, boHoSo.LyDoTraLai);
    }

    private static XayDungVanBanTrinhThamDinhDto ToDto(Guid hoSoId, BoHoSoNghiepVu boHoSo, HoSoXayDungVanBanTrinhThamDinh detail) => new(hoSoId, boHoSo.Id, boHoSo.BoHoSoNguonId ?? Guid.Empty, detail.FileDuThaoId, boHoSo.BuocQuyTrinhId, ToTrangThaiHienThi(boHoSo), detail.SoToTrinh, detail.NgayToTrinh, detail.NgayGuiThamDinh, detail.DonViNhanThamDinhId, detail.NoiDungDeNghiThamDinh, detail.HanDeNghiTraKetQua, boHoSo.NoiDungGhiChu, ExtractSoLanTraLai(boHoSo.LyDoTraLai), boHoSo.LyDoTraLai);
    private static XayDungVanBanTaiLieuDto ToTaiLieuDto(HoSoXayDungVanBanFile file) => new(file.Id, file.LoaiTaiLieuId, file.TenTaiLieu, file.PhienBan, file.TenFile, file.DuongDanFile, file.MimeType, file.DungLuong, file.IsCurrent, file.NgayTaiLen);
    private sealed record Data(Guid HoSoId, BoHoSoNghiepVu BoHoSo, HoSoXayDungVanBanTrinhThamDinh ChiTiet);
    private sealed record CurrentActor(Guid UserId, Guid DonViId);
}
