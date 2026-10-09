using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanThamTraHdndService(XayDungVanBanDbContext db, ICurrentUserContext user, IWebHostEnvironment environment) : IXayDungVanBanThamTraHdndService
{
    public async Task<IReadOnlyList<HoSoThamTraHdndListItemDto>> GetListAsync(CancellationToken ct = default) =>
        await (from b in db.BoHoSoNghiepVus.AsNoTracking()
               join d in db.HoSoXayDungVanBanThamTraHdnds.AsNoTracking() on b.Id equals d.BoHoSoNghiepVuId
               join h in db.HoSoXayDungVanBans.AsNoTracking() on b.HoSoXayDungVanBanId equals h.Id
               where b.LoaiBoHoSo == LoaiBoHoSo.ThamTraHdnd && !b.IsDeleted && !h.IsDeleted
               orderby b.CreatedAt descending
               select new HoSoThamTraHdndListItemDto(h.Id, b.Id, h.MaHoSo, h.TenHoSo, h.TenDuThaoVanBan, h.NamXayDung, b.TrangThai.ToString(), b.NgayTao, h.ThoiGianDuKienHoanThanh, d.NgayTrinhThamTra, d.NgayNhanKetQuaThamTra, d.KetQuaThamTra)).ToListAsync(ct);

    public async Task<XayDungVanBanThamTraHdndDto?> GetAsync(Guid id, CancellationToken ct = default) { var x = await LoadAsync(id, false, ct); return x is null ? null : Dto(id, x.B, x.D); }

    public async Task<XayDungVanBanThamTraHdndDto?> UpdateAsync(Guid id, CapNhatThamTraHdndRequest r, CancellationToken ct = default)
    {
        var x = await LoadAsync(id, true, ct); if (x is null) return null; EnsureNhap(x.B);
        x.D.BanHdndThamTraId = r.BanHdndThamTraId; x.D.NgayTrinhThamTra = r.NgayTrinhThamTra; x.D.NgayNhanKetQuaThamTra = r.NgayNhanKetQuaThamTra;
        x.D.KetQuaThamTra = r.KetQuaThamTra?.Trim(); x.D.NoiDungKienNghi = r.NoiDungKienNghi?.Trim(); x.D.NoiDungTiepThuGiaiTrinh = r.NoiDungTiepThuGiaiTrinh?.Trim();
        x.D.NgayNhanYKienThaoLuan = r.NgayNhanYKienThaoLuan; x.D.NoiDungTongHopYKienThaoLuan = r.NoiDungTongHopYKienThaoLuan?.Trim();
        await db.SaveChangesAsync(ct); return Dto(id, x.B, x.D);
    }

    public async Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid id, CancellationToken ct = default)
    {
        var x = await LoadAsync(id, false, ct); if (x is null) return null;
        return await (from l in db.BoHoSoNghiepVuTaiLieus.AsNoTracking() join f in db.HoSoXayDungVanBanFiles.AsNoTracking() on l.HoSoXayDungVanBanFileId equals f.Id where l.BoHoSoNghiepVuId == x.B.Id && !l.IsDeleted && !f.IsDeleted orderby f.NgayTaiLen descending select FileDto(f)).ToListAsync(ct);
    }

    public async Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid id, TaiTaiLieuThamTraHdndRequest r, CancellationToken ct = default)
    {
        var x = await LoadAsync(id, true, ct); if (x is null) return null; EnsureNhap(x.B); var a = Actor(x.B.DonViLapId);
        if (r.LoaiTaiLieuId == Guid.Empty || r.NoiDung.Length == 0) throw new InvalidOperationException("Thiếu tài liệu thẩm tra.");
        var name = Path.GetFileName(r.TenFile); var rel = Path.Combine("uploads", "xay-dung-van-ban", id.ToString("N")); var dir = Path.Combine(environment.ContentRootPath, rel); Directory.CreateDirectory(dir);
        var stored = $"{Guid.NewGuid():N}_{name}"; var path = Path.Combine(dir, stored); await using (var output = File.Create(path)) await r.NoiDung.CopyToAsync(output, ct);
        var version = (await db.HoSoXayDungVanBanFiles.Where(f => f.HoSoXayDungVanBanId == id && f.LoaiTaiLieuId == r.LoaiTaiLieuId && !f.IsDeleted).Select(f => (int?)f.PhienBan).MaxAsync(ct) ?? 0) + 1;
        var file = new HoSoXayDungVanBanFile { HoSoXayDungVanBanId = id, LoaiTaiLieuId = r.LoaiTaiLieuId, TenTaiLieu = r.TenTaiLieu, TenFile = name, DuongDanFile = Path.Combine(rel, stored).Replace('\\', '/'), MimeType = r.MimeType, DungLuong = r.NoiDung.Length, PhienBan = version, NguoiTaiLenId = a.UserId, CreatedBy = a.UserId.ToString() };
        db.HoSoXayDungVanBanFiles.Add(file); db.BoHoSoNghiepVuTaiLieus.Add(new() { BoHoSoNghiepVuId = x.B.Id, HoSoXayDungVanBanFileId = file.Id, LoaiTaiLieuId = file.LoaiTaiLieuId, HinhThucThem = "TaoMoi", CreatedBy = a.UserId.ToString() });
        await db.SaveChangesAsync(ct); return FileDto(file);
    }

    public async Task<DieuKienGuiThamTraHdndDto?> KiemTraAsync(Guid id, CancellationToken ct = default) { var x = await LoadAsync(id, false, ct); if (x is null) return null; var c = await Conditions(x, ct); return new(c.Count == 0, c); }

    public async Task<XayDungVanBanThamTraHdndDto?> GuiAsync(Guid id, GuiKetQuaThamTraHdndRequest r, CancellationToken ct = default)
    {
        var x = await LoadAsync(id, true, ct); if (x is null) return null; EnsureNhap(x.B); var c = await Conditions(x, ct); if (c.Count > 0) throw new InvalidOperationException(string.Join(" ", c));
        if (r.BuocQuyTrinhTiepTheoId == Guid.Empty || r.TrangThaiHoSoTiepTheoId == Guid.Empty) throw new InvalidOperationException("Thiếu bước hoặc trạng thái tiếp theo.");
        var a = Actor(x.B.DonViLapId); var now = DateTime.UtcNow; await using var tx = await db.Database.BeginTransactionAsync(ct);
        x.B.TrangThai = TrangThaiBoHoSo.DaHoanThanh; x.B.NgayGui = now; x.B.NgayHoanThanh = now; x.B.UpdatedAt = now; x.B.UpdatedBy = a.UserId.ToString();
        var h = await db.HoSoXayDungVanBans.FirstAsync(z => z.Id == id, ct); var pb = h.BuocHienTaiId; var ps = h.TrangThaiHoSoId; h.BuocHienTaiId = r.BuocQuyTrinhTiepTheoId; h.TrangThaiHoSoId = r.TrangThaiHoSoTiepTheoId; if (r.HanXuLy.HasValue) h.ThoiGianDuKienHoanThanh = r.HanXuLy.Value; h.UpdatedAt = now; h.UpdatedBy = a.UserId.ToString();
        await TaoHoSoBanHanhNeuChuaCo(id, r.BuocQuyTrinhTiepTheoId, x.B, a, ct); ThemNhacTienDoNeuCo(id, r, a);
        db.HoSoXayDungVanBanLichSuXuLys.Add(new() { HoSoXayDungVanBanId = id, BoHoSoNghiepVuId = x.B.Id, HanhDong = "GUI_KET_QUA_THAM_TRA_HDND", NoiDung = "Gửi kết quả thẩm tra HĐND sang bước tiếp theo.", NguoiXuLyId = a.UserId, DonViXuLyId = a.DonViId, ThoiGianXuLy = now, BuocQuyTrinhTruocId = pb, BuocQuyTrinhSauId = r.BuocQuyTrinhTiepTheoId, TrangThaiTruocId = ps, TrangThaiSauId = r.TrangThaiHoSoTiepTheoId, CreatedBy = a.UserId.ToString() });
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return Dto(id, x.B, x.D);
    }

    private async Task TaoHoSoBanHanhNeuChuaCo(Guid id, Guid buocId, BoHoSoNghiepVu source, ActorData a, CancellationToken ct)
    {
        if (await db.BoHoSoNghiepVus.AnyAsync(x => x.HoSoXayDungVanBanId == id && x.BuocQuyTrinhId == buocId && x.LoaiBoHoSo == LoaiBoHoSo.BanHanh && !x.IsDeleted, ct)) return;
        var lan = (await db.BoHoSoNghiepVus.Where(x => x.HoSoXayDungVanBanId == id && x.BuocQuyTrinhId == buocId && !x.IsDeleted).Select(x => (int?)x.LanXuLy).MaxAsync(ct) ?? 0) + 1;
        var b = new BoHoSoNghiepVu { HoSoXayDungVanBanId = id, BuocQuyTrinhId = buocId, LoaiBoHoSo = LoaiBoHoSo.BanHanh, TrangThai = TrangThaiBoHoSo.Nhap, LanXuLy = lan, BoHoSoNguonId = source.Id, NguoiLapId = a.UserId, DonViLapId = a.DonViId, CreatedBy = a.UserId.ToString() };
        db.BoHoSoNghiepVus.Add(b); db.HoSoXayDungVanBanKetQuaBanHanhs.Add(new() { BoHoSoNghiepVuId = b.Id });
        var links = await db.BoHoSoNghiepVuTaiLieus.Where(x => x.BoHoSoNghiepVuId == source.Id && !x.IsDeleted).ToListAsync(ct);
        foreach (var l in links) db.BoHoSoNghiepVuTaiLieus.Add(new() { BoHoSoNghiepVuId = b.Id, HoSoXayDungVanBanFileId = l.HoSoXayDungVanBanFileId, LoaiTaiLieuId = l.LoaiTaiLieuId, HinhThucThem = "KeThua", BoHoSoTaiLieuNguonId = l.Id, CreatedBy = a.UserId.ToString() });
    }

    private void ThemNhacTienDoNeuCo(Guid id, GuiKetQuaThamTraHdndRequest r, ActorData a) { if (!r.ThoiGianCanhBao.HasValue) return; db.HoSoXayDungVanBanNhacTienDos.Add(new() { HoSoXayDungVanBanId = id, LoaiNhacNho = "CANH_BAO_SAP_HAN", TrangThaiXuLy = "DA_GUI", NoiDungNhacNho = $"Cảnh báo sắp đến hạn xử lý bước tiếp theo. Hạn xử lý: {r.HanXuLy:dd/MM/yyyy}.", NguoiGuiId = a.UserId, DonViGuiId = a.DonViId, NgayGui = r.ThoiGianCanhBao.Value, CreatedBy = a.UserId.ToString() }); }
    private async Task<List<string>> Conditions(Data x, CancellationToken ct) { var c = new List<string>(); if (string.IsNullOrWhiteSpace(x.D.KetQuaThamTra)) c.Add("Chưa cập nhật kết quả thẩm tra."); if (!x.D.NgayNhanKetQuaThamTra.HasValue) c.Add("Chưa nhập ngày nhận kết quả thẩm tra."); if (!await db.BoHoSoNghiepVuTaiLieus.AnyAsync(z => z.BoHoSoNghiepVuId == x.B.Id && !z.IsDeleted, ct)) c.Add("Chưa có tài liệu thẩm tra."); return c; }
    private async Task<Data?> LoadAsync(Guid id, bool tracking, CancellationToken ct) { var b = tracking ? db.BoHoSoNghiepVus : db.BoHoSoNghiepVus.AsNoTracking(); var d = tracking ? db.HoSoXayDungVanBanThamTraHdnds : db.HoSoXayDungVanBanThamTraHdnds.AsNoTracking(); return await (from bo in b join detail in d on bo.Id equals detail.BoHoSoNghiepVuId where bo.HoSoXayDungVanBanId == id && bo.LoaiBoHoSo == LoaiBoHoSo.ThamTraHdnd && !bo.IsDeleted orderby bo.CreatedAt descending select new Data(bo, detail)).FirstOrDefaultAsync(ct); }
    private static void EnsureNhap(BoHoSoNghiepVu b) { if (b.TrangThai != TrangThaiBoHoSo.Nhap) throw new InvalidOperationException("Hồ sơ thẩm tra đã chuyển bước, không được cập nhật."); }
    private ActorData Actor(Guid? fallbackDonViId = null) { if (!user.IsAuthenticated || user.UserId is not Guid u || u == Guid.Empty) throw new UnauthorizedAccessException("Người dùng chưa đăng nhập."); var d = user.DonViId is { } current && current != Guid.Empty ? current : fallbackDonViId.GetValueOrDefault(); if (d == Guid.Empty) throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý."); return new(u, d); }
    private static XayDungVanBanThamTraHdndDto Dto(Guid id, BoHoSoNghiepVu b, HoSoXayDungVanBanThamTraHdnd d) => new(id, b.Id, b.BuocQuyTrinhId, b.TrangThai.ToString(), d.BanHdndThamTraId, d.NgayTrinhThamTra, d.NgayNhanKetQuaThamTra, d.KetQuaThamTra, d.NoiDungKienNghi, d.NoiDungTiepThuGiaiTrinh, d.NgayNhanYKienThaoLuan, d.NoiDungTongHopYKienThaoLuan);
    private static XayDungVanBanTaiLieuDto FileDto(HoSoXayDungVanBanFile f) => new(f.Id, f.LoaiTaiLieuId, f.TenTaiLieu, f.PhienBan, f.TenFile, f.DuongDanFile, f.MimeType, f.DungLuong, f.IsCurrent, f.NgayTaiLen);
    private sealed record Data(BoHoSoNghiepVu B, HoSoXayDungVanBanThamTraHdnd D); private sealed record ActorData(Guid UserId, Guid DonViId);
}
