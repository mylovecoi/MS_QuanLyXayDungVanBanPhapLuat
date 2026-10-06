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
    public async Task<XayDungVanBanTrinhThamDinhDto> CreateAsync(TaoHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        if (request.HoSoId == Guid.Empty || request.BuocQuyTrinhTiepTheoId == Guid.Empty || request.DonViNhanThamDinhId == Guid.Empty)
            throw new InvalidOperationException("Thiếu hồ sơ, bước tiếp theo hoặc đơn vị nhận thẩm định.");

        var source = await GetSourceDraftAsync(request.HoSoId, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy bộ hồ sơ soạn thảo đang xử lý.");
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
        return data is null ? null : ToDto(data.HoSoId, data.BoHoSo, data.ChiTiet);
    }

    public async Task<XayDungVanBanTrinhThamDinhDto?> UpdateAsync(Guid hoSoId, CapNhatHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        if (request.DonViNhanThamDinhId == Guid.Empty) throw new InvalidOperationException("Chưa chọn đơn vị nhận thẩm định.");
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
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
            select ToTaiLieuDto(file)).ToListAsync(cancellationToken);
    }

    public async Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid hoSoId, TaiTaiLieuTrinhThamDinhRequest request, CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        if (request.LoaiTaiLieuId == Guid.Empty || request.NoiDung.Length == 0 || string.IsNullOrWhiteSpace(request.TenTaiLieu))
            throw new InvalidOperationException("Thiếu loại, tên tài liệu hoặc file tải lên.");
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        EnsureNhap(data.BoHoSo);
        var safeFileName = Path.GetFileName(request.TenFile);
        if (string.IsNullOrWhiteSpace(safeFileName)) throw new InvalidOperationException("Tên file không hợp lệ.");

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
            dbContext.BoHoSoNghiepVuTaiLieus.Add(new BoHoSoNghiepVuTaiLieu { BoHoSoNghiepVuId = data.BoHoSo.Id, HoSoXayDungVanBanFileId = file.Id, LoaiTaiLieuId = file.LoaiTaiLieuId, HinhThucThem = "TaoMoi", ThuTu = file.PhienBan, CreatedBy = actor.UserId.ToString() });
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
        var actor = RequireActor();
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return false;
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
        var actor = RequireActor();
        if (request.BuocQuyTrinhTiepTheoId == Guid.Empty || request.TrangThaiHoSoTiepTheoId == Guid.Empty) throw new InvalidOperationException("Thiếu bước hoặc trạng thái tiếp theo.");
        var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        EnsureNhap(data.BoHoSo);
        if (data.BoHoSo.BuocQuyTrinhId != request.BuocQuyTrinhTiepTheoId)
            throw new InvalidOperationException("Bước gửi thẩm định không khớp với bộ hồ sơ đã tạo.");
        var conditions = await GetConditionsAsync(data, cancellationToken);
        if (conditions.Count > 0) throw new InvalidOperationException(string.Join(" ", conditions));

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var source = await dbContext.BoHoSoNghiepVus.FirstAsync(x => x.Id == data.BoHoSo.BoHoSoNguonId, cancellationToken);
        var now = DateTime.UtcNow;
        source.TrangThai = TrangThaiBoHoSo.DaGui; source.NgayGui = now; source.UpdatedAt = now; source.UpdatedBy = actor.UserId.ToString();
        data.BoHoSo.TrangThai = TrangThaiBoHoSo.DaGui; data.BoHoSo.NgayGui = now; data.BoHoSo.UpdatedAt = now; data.BoHoSo.UpdatedBy = actor.UserId.ToString();
        data.ChiTiet.NgayGuiThamDinh = now;
        var hoSo = await dbContext.HoSoXayDungVanBans.FirstAsync(x => x.Id == hoSoId, cancellationToken);
        var previousBuoc = hoSo.BuocHienTaiId; var previousTrangThai = hoSo.TrangThaiHoSoId;
        hoSo.BuocHienTaiId = request.BuocQuyTrinhTiepTheoId; hoSo.TrangThaiHoSoId = request.TrangThaiHoSoTiepTheoId; hoSo.UpdatedAt = now; hoSo.UpdatedBy = actor.UserId.ToString();
        AddTimeline(hoSoId, data.BoHoSo.Id, "GUI_THAM_DINH", "Gửi hồ sơ thẩm định", actor, null, previousBuoc, request.BuocQuyTrinhTiepTheoId, previousTrangThai, request.TrangThaiHoSoTiepTheoId);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ToDto(hoSoId, data.BoHoSo, data.ChiTiet);
    }

    public async Task<bool> HuyAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var actor = RequireActor(); var data = await GetDataAsync(hoSoId, true, cancellationToken);
        if (data is null) return false;
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
        return conditions;
    }

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

    private CurrentActor RequireActor()
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid userId || currentUserContext.DonViId is not Guid donViId || donViId == Guid.Empty)
            throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý.");
        return new CurrentActor(userId, donViId);
    }

    private void AddTimeline(Guid hoSoId, Guid boHoSoId, string hanhDong, string noiDung, CurrentActor actor, Guid? fileId = null, Guid? buocTruoc = null, Guid? buocSau = null, Guid? trangThaiTruoc = null, Guid? trangThaiSau = null) =>
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(new HoSoXayDungVanBanLichSuXuLy { HoSoXayDungVanBanId = hoSoId, BoHoSoNghiepVuId = boHoSoId, HanhDong = hanhDong, NoiDung = noiDung, NguoiXuLyId = actor.UserId, DonViXuLyId = actor.DonViId, HoSoXayDungVanBanFileId = fileId, BuocQuyTrinhTruocId = buocTruoc, BuocQuyTrinhSauId = buocSau, TrangThaiTruocId = trangThaiTruoc, TrangThaiSauId = trangThaiSau, CreatedBy = actor.UserId.ToString() });

    private static XayDungVanBanTrinhThamDinhDto ToDto(Guid hoSoId, BoHoSoNghiepVu boHoSo, HoSoXayDungVanBanTrinhThamDinh detail) => new(hoSoId, boHoSo.Id, boHoSo.BoHoSoNguonId ?? Guid.Empty, boHoSo.TrangThai.ToString(), detail.SoToTrinh, detail.NgayToTrinh, detail.NgayGuiThamDinh, detail.DonViNhanThamDinhId, detail.NoiDungDeNghiThamDinh, detail.HanDeNghiTraKetQua, boHoSo.NoiDungGhiChu);
    private static XayDungVanBanTaiLieuDto ToTaiLieuDto(HoSoXayDungVanBanFile file) => new(file.Id, file.LoaiTaiLieuId, file.TenTaiLieu, file.PhienBan, file.TenFile, file.DuongDanFile, file.MimeType, file.DungLuong, file.IsCurrent, file.NgayTaiLen);
    private sealed record Data(Guid HoSoId, BoHoSoNghiepVu BoHoSo, HoSoXayDungVanBanTrinhThamDinh ChiTiet);
    private sealed record CurrentActor(Guid UserId, Guid DonViId);
}
