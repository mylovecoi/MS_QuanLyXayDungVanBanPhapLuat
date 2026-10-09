using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanYKienUbndService(XayDungVanBanDbContext db, ICurrentUserContext user, IWebHostEnvironment environment) : IXayDungVanBanYKienUbndService
{
    public async Task<IReadOnlyList<HoSoYKienUbndListItemDto>> GetListAsync(CancellationToken ct = default)
    {
        var sources = await (
            from b in db.BoHoSoNghiepVus.AsNoTracking()
            join d in db.HoSoXayDungVanBanTrinhPheDuyets.AsNoTracking() on b.Id equals d.BoHoSoNghiepVuId
            join h in db.HoSoXayDungVanBans.AsNoTracking() on b.HoSoXayDungVanBanId equals h.Id
            where b.LoaiBoHoSo == LoaiBoHoSo.TrinhPheDuyet && b.TrangThai == TrangThaiBoHoSo.DaGui && !b.IsDeleted && !h.IsDeleted
            select new { h.Id, h.MaHoSo, h.TenHoSo, h.TenDuThaoVanBan, h.NamXayDung, d.NgayTrinh, b.CreatedAt })
            .ToListAsync(ct);
        var ids = sources.Select(x => x.Id).ToList();
        var yKiens = await (
            from b in db.BoHoSoNghiepVus.AsNoTracking()
            join d in db.HoSoXayDungVanBanYKienUbnds.AsNoTracking() on b.Id equals d.BoHoSoNghiepVuId
            where ids.Contains(b.HoSoXayDungVanBanId) && b.LoaiBoHoSo == LoaiBoHoSo.YKienThanhVienUbnd && !b.IsDeleted
            select new { b.HoSoXayDungVanBanId, b.Id, b.TrangThai, d.NgayNhanYKien, d.KetLuanTongHop, b.CreatedAt })
            .ToListAsync(ct);
        var latest = yKiens.GroupBy(x => x.HoSoXayDungVanBanId).ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.CreatedAt).First());
        return sources.OrderByDescending(x => x.CreatedAt).Select(x => latest.TryGetValue(x.Id, out var y)
            ? new HoSoYKienUbndListItemDto(x.Id, y.Id, x.MaHoSo, x.TenHoSo, x.TenDuThaoVanBan, x.NamXayDung, y.TrangThai.ToString(), x.NgayTrinh, y.NgayNhanYKien, y.KetLuanTongHop)
            : new HoSoYKienUbndListItemDto(x.Id, null, x.MaHoSo, x.TenHoSo, x.TenDuThaoVanBan, x.NamXayDung, "ChoLapHoSo", x.NgayTrinh, null, null)).ToList();
    }

    public async Task<XayDungVanBanYKienUbndDto> CreateAsync(TaoHoSoYKienUbndRequest r, CancellationToken ct = default)
    {
        var source = await db.BoHoSoNghiepVus.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(x => x.HoSoXayDungVanBanId == r.HoSoId && x.LoaiBoHoSo == LoaiBoHoSo.TrinhPheDuyet && x.TrangThai == TrangThaiBoHoSo.DaGui && !x.IsDeleted, ct) ?? throw new InvalidOperationException("Chưa có hồ sơ trình phê duyệt đã gửi.");
        var hoSo = await db.HoSoXayDungVanBans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == r.HoSoId && !x.IsDeleted, ct) ?? throw new InvalidOperationException("Không tìm thấy hồ sơ.");
        var actor = GetActor(hoSo.DonViChuTriSoanThaoId);
        if (await LoadAsync(r.HoSoId, true, ct) is not null) throw new InvalidOperationException("Đã có hồ sơ ý kiến UBND.");
        var buocQuyTrinhId = hoSo.BuocHienTaiId;
        var lanXuLy = (await db.BoHoSoNghiepVus.Where(x => x.HoSoXayDungVanBanId == r.HoSoId && x.BuocQuyTrinhId == buocQuyTrinhId && !x.IsDeleted).Select(x => (int?)x.LanXuLy).MaxAsync(ct) ?? 0) + 1;
        var b = new BoHoSoNghiepVu { HoSoXayDungVanBanId = r.HoSoId, BuocQuyTrinhId = buocQuyTrinhId, LoaiBoHoSo = LoaiBoHoSo.YKienThanhVienUbnd, TrangThai = TrangThaiBoHoSo.Nhap, LanXuLy = lanXuLy, BoHoSoNguonId = source.Id, NguoiLapId = actor.UserId, DonViLapId = actor.DonViId, CreatedBy = actor.UserId.ToString() };
        var d = new HoSoXayDungVanBanYKienUbnd { BoHoSoNghiepVuId = b.Id }; db.BoHoSoNghiepVus.Add(b); db.HoSoXayDungVanBanYKienUbnds.Add(d);
        var links = await db.BoHoSoNghiepVuTaiLieus.Where(x => x.BoHoSoNghiepVuId == source.Id && !x.IsDeleted).ToListAsync(ct);
        foreach (var link in links) db.BoHoSoNghiepVuTaiLieus.Add(new() { BoHoSoNghiepVuId = b.Id, HoSoXayDungVanBanFileId = link.HoSoXayDungVanBanFileId, LoaiTaiLieuId = link.LoaiTaiLieuId, HinhThucThem = "KeThua", BoHoSoTaiLieuNguonId = link.Id, CreatedBy = actor.UserId.ToString() });
        await db.SaveChangesAsync(ct); return ToDto(r.HoSoId, b, d);
    }
    public async Task<XayDungVanBanYKienUbndDto?> GetAsync(Guid id, CancellationToken ct = default) { var x = await LoadAsync(id, false, ct); return x is null ? null : ToDto(id, x.B, x.D); }
    public async Task<XayDungVanBanYKienUbndDto?> UpdateAsync(Guid id, CapNhatYKienUbndRequest r, CancellationToken ct = default) { var x = await LoadAsync(id, true, ct); if (x is null) return null; EnsureNhap(x.B); var total = (r.SoDongY ?? 0) + (r.SoKhongDongY ?? 0) + (r.SoYKienKhac ?? 0); if (r.TongSoThanhVienDuocLayYKien is int t && total > t) throw new InvalidOperationException("Tổng ý kiến vượt số thành viên."); var d = x.D; d.NgayNhanYKien = r.NgayNhanYKien; d.TongSoThanhVienDuocLayYKien = r.TongSoThanhVienDuocLayYKien; d.SoDongY = r.SoDongY; d.SoKhongDongY = r.SoKhongDongY; d.SoYKienKhac = r.SoYKienKhac; d.KetLuanTongHop = r.KetLuanTongHop; d.NoiDungTongHop = r.NoiDungTongHop; d.NoiDungGiaiTrinh = r.NoiDungGiaiTrinh; await db.SaveChangesAsync(ct); return ToDto(id, x.B, d); }
    public async Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid id, CancellationToken ct = default) { var x = await LoadAsync(id, false, ct); if (x is null) return null; return await (from l in db.BoHoSoNghiepVuTaiLieus join f in db.HoSoXayDungVanBanFiles on l.HoSoXayDungVanBanFileId equals f.Id where l.BoHoSoNghiepVuId == x.B.Id && !l.IsDeleted && !f.IsDeleted select FileDto(f)).ToListAsync(ct); }
    public async Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid id, TaiTaiLieuYKienUbndRequest r, CancellationToken ct = default) { var x = await LoadAsync(id, true, ct); if (x is null) return null; var actor = GetActor(x.B.DonViLapId); EnsureNhap(x.B); if (r.LoaiTaiLieuId == Guid.Empty || r.NoiDung.Length == 0) throw new InvalidOperationException("Thiếu tài liệu."); var name = Path.GetFileName(r.TenFile); var rel = Path.Combine("uploads", "xay-dung-van-ban", id.ToString("N")); var dir = Path.Combine(environment.ContentRootPath, rel); Directory.CreateDirectory(dir); var stored = $"{Guid.NewGuid():N}_{name}"; var path = Path.Combine(dir, stored); await using (var output = File.Create(path)) await r.NoiDung.CopyToAsync(output, ct); var nextVersion = (await db.HoSoXayDungVanBanFiles.Where(f => f.HoSoXayDungVanBanId == id && f.LoaiTaiLieuId == r.LoaiTaiLieuId).Select(f => (int?)f.PhienBan).MaxAsync(ct) ?? 0) + 1; var f = new HoSoXayDungVanBanFile { HoSoXayDungVanBanId = id, LoaiTaiLieuId = r.LoaiTaiLieuId, TenTaiLieu = r.TenTaiLieu, TenFile = name, DuongDanFile = Path.Combine(rel, stored).Replace('\\', '/'), MimeType = r.MimeType, DungLuong = r.NoiDung.Length, PhienBan = nextVersion, NguoiTaiLenId = actor.UserId, CreatedBy = actor.UserId.ToString() }; db.HoSoXayDungVanBanFiles.Add(f); db.BoHoSoNghiepVuTaiLieus.Add(new() { BoHoSoNghiepVuId = x.B.Id, HoSoXayDungVanBanFileId = f.Id, LoaiTaiLieuId = f.LoaiTaiLieuId, HinhThucThem = "TaoMoi", CreatedBy = actor.UserId.ToString() }); await db.SaveChangesAsync(ct); return FileDto(f); }
    public async Task<bool> DeleteTaiLieuAsync(Guid id, Guid linkId, CancellationToken ct = default) { var x = await LoadAsync(id, true, ct); if (x is null) return false; EnsureNhap(x.B); var link = await db.BoHoSoNghiepVuTaiLieus.FirstOrDefaultAsync(z => z.Id == linkId && z.BoHoSoNghiepVuId == x.B.Id && !z.IsDeleted, ct); if (link is null) return false; if (link.HinhThucThem == "KeThua") throw new InvalidOperationException("Không thể gỡ tài liệu kế thừa."); link.IsDeleted = true; await db.SaveChangesAsync(ct); return true; }
    public async Task<DieuKienGuiYKienUbndDto?> KiemTraAsync(Guid id, CancellationToken ct = default) { var x = await LoadAsync(id, false, ct); if (x is null) return null; var c = await ConditionsAsync(x, ct); return new(c.Count == 0, c); }
    public async Task<XayDungVanBanYKienUbndDto?> GuiAsync(Guid id, GuiYKienUbndRequest r, CancellationToken ct = default)
    {
        var x = await LoadAsync(id, true, ct);
        if (x is null) return null;

        var actor = GetActor(x.B.DonViLapId);
        EnsureNhap(x.B);

        var conditions = await ConditionsAsync(x, ct);
        if (conditions.Count > 0)
            throw new InvalidOperationException(string.Join(" ", conditions));

        if (r.BuocQuyTrinhTiepTheoId == Guid.Empty || r.TrangThaiHoSoTiepTheoId == Guid.Empty)
            throw new InvalidOperationException("Thiếu bước hoặc trạng thái tiếp theo.");

        var isReturn = string.Equals(r.LoaiChuyenBuoc, "Return", StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.D.KetLuanTongHop, "KHONG_DONG_Y", StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.D.KetLuanTongHop, "CON_Y_KIEN_KHAC_NHAU", StringComparison.OrdinalIgnoreCase);

        if (isReturn && string.IsNullOrWhiteSpace(r.LyDoTraLai))
            throw new InvalidOperationException("Vui lòng nhập lý do trả lại hồ sơ.");

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            x.B.TrangThai = isReturn ? TrangThaiBoHoSo.YeuCauBoSung : TrangThaiBoHoSo.DaHoanThanh;
            x.B.NgayGui = DateTime.UtcNow;
            x.B.NgayHoanThanh = DateTime.UtcNow;
            x.B.LyDoTraLai = isReturn ? r.LyDoTraLai?.Trim() : null;

            var hoSo = await db.HoSoXayDungVanBans.FirstAsync(z => z.Id == id, ct);
            var previousBuoc = hoSo.BuocHienTaiId;
            var previousTrangThai = hoSo.TrangThaiHoSoId;

            hoSo.BuocHienTaiId = r.BuocQuyTrinhTiepTheoId;
            hoSo.TrangThaiHoSoId = r.TrangThaiHoSoTiepTheoId;
            if (r.HanXuLy.HasValue)
                hoSo.ThoiGianDuKienHoanThanh = r.HanXuLy.Value;

            if (isReturn)
                await TraLaiHoSoTrinhYKienAsync(x.B, r.LyDoTraLai?.Trim(), ct);
            else
            {
                await TaoHoSoBanHanhNeuChuaCoAsync(id, r.BuocQuyTrinhTiepTheoId, x.B, actor, ct);
                ThemNhacTienDoNeuCo(id, r, actor);
            }

            db.HoSoXayDungVanBanLichSuXuLys.Add(new()
            {
                HoSoXayDungVanBanId = id,
                BoHoSoNghiepVuId = x.B.Id,
                HanhDong = isReturn ? "TRA_LAI_TRINH_Y_KIEN_UBND" : "GUI_KET_QUA_Y_KIEN_UBND",
                NoiDung = isReturn
                    ? $"Không đồng ý, trả lại hồ sơ trình lấy ý kiến UBND. Lý do: {r.LyDoTraLai?.Trim()}"
                    : "Đồng ý, chuyển hồ sơ sang bước ban hành/tiếp theo.",
                NguoiXuLyId = actor.UserId,
                DonViXuLyId = actor.DonViId,
                BuocQuyTrinhTruocId = previousBuoc,
                BuocQuyTrinhSauId = r.BuocQuyTrinhTiepTheoId,
                TrangThaiTruocId = previousTrangThai,
                TrangThaiSauId = r.TrangThaiHoSoTiepTheoId,
                CreatedBy = actor.UserId.ToString()
            });

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return ToDto(id, x.B, x.D);
        }
        catch (DbUpdateException ex)
        {
            await tx.RollbackAsync(ct);
            throw new InvalidOperationException($"Không lưu được kết quả ý kiến UBND: {GetDatabaseError(ex)}", ex);
        }
    }
    public async Task<bool> HuyAsync(Guid id, CancellationToken ct = default) { var x = await LoadAsync(id, true, ct); if (x is null) return false; EnsureNhap(x.B); x.B.IsDeleted = true; await db.SaveChangesAsync(ct); return true; }
    private async Task TraLaiHoSoTrinhYKienAsync(BoHoSoNghiepVu hoSoYKienUbnd, string? lyDoTraLai, CancellationToken ct)
    {
        if (hoSoYKienUbnd.BoHoSoNguonId is not Guid sourceId)
            return;

        var source = await db.BoHoSoNghiepVus.FirstOrDefaultAsync(z => z.Id == sourceId && !z.IsDeleted, ct);
        if (source is null)
            return;

        source.TrangThai = TrangThaiBoHoSo.Nhap;
        source.NgayGui = null;
        source.LyDoTraLai = lyDoTraLai;

        var sourceDetail = await db.HoSoXayDungVanBanTrinhPheDuyets.FirstOrDefaultAsync(z => z.BoHoSoNghiepVuId == source.Id, ct);
        if (sourceDetail is not null)
            sourceDetail.NgayTrinh = null;
    }

    private async Task TaoHoSoBanHanhNeuChuaCoAsync(Guid hoSoId, Guid buocQuyTrinhId, BoHoSoNghiepVu source, CurrentActor actor, CancellationToken ct)
    {
        var existing = await db.BoHoSoNghiepVus
            .AnyAsync(x => x.HoSoXayDungVanBanId == hoSoId
                && x.BuocQuyTrinhId == buocQuyTrinhId
                && x.LoaiBoHoSo == LoaiBoHoSo.BanHanh
                && !x.IsDeleted, ct);
        if (existing)
            return;

        var lanXuLy = (await db.BoHoSoNghiepVus
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && x.BuocQuyTrinhId == buocQuyTrinhId && !x.IsDeleted)
            .Select(x => (int?)x.LanXuLy)
            .MaxAsync(ct) ?? 0) + 1;

        var boHoSoBanHanh = new BoHoSoNghiepVu
        {
            HoSoXayDungVanBanId = hoSoId,
            BuocQuyTrinhId = buocQuyTrinhId,
            LoaiBoHoSo = LoaiBoHoSo.BanHanh,
            TrangThai = TrangThaiBoHoSo.Nhap,
            LanXuLy = lanXuLy,
            BoHoSoNguonId = source.Id,
            NguoiLapId = actor.UserId,
            DonViLapId = actor.DonViId,
            CreatedBy = actor.UserId.ToString()
        };
        var ketQuaBanHanh = new HoSoXayDungVanBanKetQuaBanHanh { BoHoSoNghiepVuId = boHoSoBanHanh.Id };

        db.BoHoSoNghiepVus.Add(boHoSoBanHanh);
        db.HoSoXayDungVanBanKetQuaBanHanhs.Add(ketQuaBanHanh);

        var links = await db.BoHoSoNghiepVuTaiLieus
            .Where(x => x.BoHoSoNghiepVuId == source.Id && !x.IsDeleted)
            .ToListAsync(ct);
        foreach (var link in links)
            db.BoHoSoNghiepVuTaiLieus.Add(new()
            {
                BoHoSoNghiepVuId = boHoSoBanHanh.Id,
                HoSoXayDungVanBanFileId = link.HoSoXayDungVanBanFileId,
                LoaiTaiLieuId = link.LoaiTaiLieuId,
                HinhThucThem = "KeThua",
                BoHoSoTaiLieuNguonId = link.Id,
                ThuTu = link.ThuTu,
                CreatedBy = actor.UserId.ToString()
            });
    }

    private void ThemNhacTienDoNeuCo(Guid hoSoId, GuiYKienUbndRequest r, CurrentActor actor)
    {
        if (!r.ThoiGianCanhBao.HasValue)
            return;

        db.HoSoXayDungVanBanNhacTienDos.Add(new()
        {
            HoSoXayDungVanBanId = hoSoId,
            LoaiNhacNho = "CANH_BAO_SAP_HAN",
            TrangThaiXuLy = "DA_GUI",
            NoiDungNhacNho = $"Cảnh báo sắp đến hạn xử lý bước tiếp theo. Hạn xử lý: {r.HanXuLy:dd/MM/yyyy}.",
            NguoiGuiId = actor.UserId,
            DonViGuiId = actor.DonViId == Guid.Empty ? null : actor.DonViId,
            NgayGui = r.ThoiGianCanhBao.Value,
            CreatedBy = actor.UserId.ToString()
        });
    }

    private static string GetDatabaseError(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;
    private async Task<List<string>> ConditionsAsync(Data x, CancellationToken ct) { var c = new List<string>(); if (string.IsNullOrWhiteSpace(x.D.KetLuanTongHop) && string.IsNullOrWhiteSpace(x.D.NoiDungTongHop)) c.Add("Chưa có nội dung tổng hợp hoặc kết luận."); if (!await db.BoHoSoNghiepVuTaiLieus.AnyAsync(z => z.BoHoSoNghiepVuId == x.B.Id && !z.IsDeleted, ct)) c.Add("Chưa có file tổng hợp hoặc biên bản."); return c; }
    private async Task<Data?> LoadAsync(Guid id, bool tracking, CancellationToken ct) { var b = tracking ? db.BoHoSoNghiepVus : db.BoHoSoNghiepVus.AsNoTracking(); var d = tracking ? db.HoSoXayDungVanBanYKienUbnds : db.HoSoXayDungVanBanYKienUbnds.AsNoTracking(); return await (from bo in b join detail in d on bo.Id equals detail.BoHoSoNghiepVuId where bo.HoSoXayDungVanBanId == id && bo.LoaiBoHoSo == LoaiBoHoSo.YKienThanhVienUbnd && !bo.IsDeleted select new Data(bo, detail)).FirstOrDefaultAsync(ct); }
    private static void EnsureNhap(BoHoSoNghiepVu b) { if (b.TrangThai != TrangThaiBoHoSo.Nhap) throw new InvalidOperationException("Hồ sơ không ở trạng thái nhập."); }
    private CurrentActor GetActor(Guid? fallbackDonViId = null) { if (!user.IsAuthenticated || user.UserId is not Guid u || u == Guid.Empty) throw new UnauthorizedAccessException("Người dùng chưa đăng nhập."); var d = user.DonViId is { } current && current != Guid.Empty ? current : fallbackDonViId.GetValueOrDefault(); if (d == Guid.Empty) throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý."); return new(u, d); }
    private static XayDungVanBanYKienUbndDto ToDto(Guid h, BoHoSoNghiepVu b, HoSoXayDungVanBanYKienUbnd d) => new(h, b.Id, b.TrangThai.ToString(), d.NgayNhanYKien, d.TongSoThanhVienDuocLayYKien, d.SoDongY, d.SoKhongDongY, d.SoYKienKhac, d.KetLuanTongHop, d.NoiDungTongHop, d.NoiDungGiaiTrinh);
    private static XayDungVanBanTaiLieuDto FileDto(HoSoXayDungVanBanFile f) => new(f.Id, f.LoaiTaiLieuId, f.TenTaiLieu, f.PhienBan, f.TenFile, f.DuongDanFile, f.MimeType, f.DungLuong, f.IsCurrent, f.NgayTaiLen);
    private sealed record Data(BoHoSoNghiepVu B, HoSoXayDungVanBanYKienUbnd D); private sealed record CurrentActor(Guid UserId, Guid DonViId);
}
