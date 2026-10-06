using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanSoanThaoService(
    XayDungVanBanDbContext dbContext,
    ICurrentUserContext currentUserContext,
    IWebHostEnvironment environment) : IXayDungVanBanSoanThaoService
{
    public async Task<XayDungVanBanSoanThaoDto> CreateAsync(
        TaoHoSoSoanThaoRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        EnsureRequiredIds(request);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var hoSo = new HoSoXayDungVanBan
        {
            MaHoSo = $"XDVB-{request.NamXayDung}-{Guid.NewGuid():N}",
            TenHoSo = request.TenHoSo.Trim(),
            TenDuThaoVanBan = request.TenDuThaoVanBan.Trim(),
            DanhMucVanBanId = request.DanhMucVanBanId,
            QuyTrinhSoanThaoId = request.QuyTrinhSoanThaoId,
            BuocHienTaiId = request.BuocHienTaiId,
            TrangThaiHoSoId = request.TrangThaiHoSoId,
            DonViChuTriSoanThaoId = request.DonViChuTriSoanThaoId,
            NguoiPhuTrachId = request.NguoiPhuTrachId,
            NamXayDung = request.NamXayDung,
            ThoiGianDuKienBatDau = request.ThoiGianDuKienBatDau,
            ThoiGianDuKienHoanThanh = request.ThoiGianDuKienHoanThanh,
            MoTa = request.MoTa,
            HoSoDangKyXayDungVanBanId = request.HoSoDangKyXayDungVanBanId,
            CreatedBy = actor.UserId.ToString()
        };

        var boHoSo = new BoHoSoNghiepVu
        {
            HoSoXayDungVanBanId = hoSo.Id,
            BuocQuyTrinhId = request.BuocHienTaiId,
            LoaiBoHoSo = LoaiBoHoSo.SoanThao,
            TrangThai = TrangThaiBoHoSo.Nhap,
            LanXuLy = 1,
            NguoiLapId = actor.UserId,
            DonViLapId = actor.DonViId,
            CreatedBy = actor.UserId.ToString()
        };

        dbContext.HoSoXayDungVanBans.Add(hoSo);
        dbContext.BoHoSoNghiepVus.Add(boHoSo);
        dbContext.HoSoXayDungVanBanSoanThaos.Add(new HoSoXayDungVanBanSoanThao
        {
            BoHoSoNghiepVuId = boHoSo.Id,
            CanCuXayDung = request.CanCuXayDung,
            PhamViDieuChinh = request.PhamViDieuChinh,
            NoiDungChinhSach = request.NoiDungChinhSach
        });
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            hoSo.Id,
            boHoSo.Id,
            "TAO_MOI_HO_SO_SOAN_THAO",
            "Tạo mới hồ sơ soạn thảo",
            actor));

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ToDto(hoSo, boHoSo, request.CanCuXayDung, request.PhamViDieuChinh, request.NoiDungChinhSach);
    }

    public async Task<XayDungVanBanSoanThaoDto?> GetByIdAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await GetDraftAsync(hoSoId, tracking: false, cancellationToken);
        return data is null ? null : ToDto(data.HoSo, data.BoHoSo, data.SoanThao.CanCuXayDung, data.SoanThao.PhamViDieuChinh, data.SoanThao.NoiDungChinhSach);
    }

    public async Task<XayDungVanBanSoanThaoDto?> UpdateAsync(
        Guid hoSoId,
        CapNhatHoSoSoanThaoRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null)
        {
            return null;
        }

        EnsureCanModify(data.HoSo, data.BoHoSo);
        data.HoSo.TenHoSo = request.TenHoSo.Trim();
        data.HoSo.TenDuThaoVanBan = request.TenDuThaoVanBan.Trim();
        data.HoSo.NguoiPhuTrachId = request.NguoiPhuTrachId;
        data.HoSo.NamXayDung = request.NamXayDung;
        data.HoSo.ThoiGianDuKienBatDau = request.ThoiGianDuKienBatDau;
        data.HoSo.ThoiGianDuKienHoanThanh = request.ThoiGianDuKienHoanThanh;
        data.HoSo.MoTa = request.MoTa;
        data.HoSo.UpdatedAt = DateTime.UtcNow;
        data.HoSo.UpdatedBy = actor.UserId.ToString();
        data.SoanThao.CanCuXayDung = request.CanCuXayDung;
        data.SoanThao.PhamViDieuChinh = request.PhamViDieuChinh;
        data.SoanThao.NoiDungChinhSach = request.NoiDungChinhSach;
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            data.HoSo.Id,
            data.BoHoSo.Id,
            "CAP_NHAT_HO_SO_SOAN_THAO",
            "Cập nhật hồ sơ soạn thảo",
            actor));

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(data.HoSo, data.BoHoSo, data.SoanThao.CanCuXayDung, data.SoanThao.PhamViDieuChinh, data.SoanThao.NoiDungChinhSach);
    }

    public async Task<bool> DeleteAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null)
        {
            return false;
        }

        EnsureCanModify(data.HoSo, data.BoHoSo);
        data.HoSo.IsDeleted = true;
        data.HoSo.UpdatedAt = DateTime.UtcNow;
        data.HoSo.UpdatedBy = actor.UserId.ToString();
        data.BoHoSo.IsDeleted = true;
        data.BoHoSo.UpdatedAt = DateTime.UtcNow;
        data.BoHoSo.UpdatedBy = actor.UserId.ToString();
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            data.HoSo.Id,
            data.BoHoSo.Id,
            "XOA_MEM_HO_SO_SOAN_THAO",
            "Xóa mềm hồ sơ soạn thảo",
            actor));

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<XayDungVanBanYKienDonViDto>?> GetYKienDonViAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default)
    {
        var exists = await GetDraftAsync(hoSoId, tracking: false, cancellationToken) is not null;
        if (!exists)
        {
            return null;
        }

        return await dbContext.HoSoXayDungVanBanYKienDonVis
            .AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted)
            .OrderByDescending(x => x.NgayNhan)
            .ThenBy(x => x.CreatedAt)
            .Select(x => ToYKienDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<XayDungVanBanYKienDonViDto?> CreateYKienDonViAsync(
        Guid hoSoId,
        TaoYKienDonViRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null)
        {
            return null;
        }

        EnsureCanModify(data.HoSo, data.BoHoSo);
        EnsureYKienRequest(request.DonViGopYId, request.KetQua);
        var exists = await dbContext.HoSoXayDungVanBanYKienDonVis
            .AnyAsync(x => x.HoSoXayDungVanBanId == hoSoId
                && x.DonViGopYId == request.DonViGopYId
                && !x.IsDeleted, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Đơn vị này đã có ý kiến trên hồ sơ.");
        }

        var entity = new HoSoXayDungVanBanYKienDonVi
        {
            HoSoXayDungVanBanId = hoSoId,
            DonViGopYId = request.DonViGopYId,
            NgayNhan = request.NgayNhan,
            KetQua = request.KetQua.Trim(),
            NoiDungYKien = request.NoiDungYKien,
            CreatedBy = actor.UserId.ToString()
        };
        dbContext.HoSoXayDungVanBanYKienDonVis.Add(entity);
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            hoSoId,
            data.BoHoSo.Id,
            "THEM_Y_KIEN_DON_VI",
            $"Nhập ý kiến của đơn vị {request.DonViGopYId}",
            actor));

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToYKienDto(entity);
    }

    public async Task<XayDungVanBanYKienDonViDto?> UpdateYKienDonViAsync(
        Guid hoSoId,
        Guid id,
        CapNhatYKienDonViRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null)
        {
            return null;
        }

        EnsureCanModify(data.HoSo, data.BoHoSo);
        EnsureYKienKetQua(request.KetQua);
        var entity = await dbContext.HoSoXayDungVanBanYKienDonVis
            .FirstOrDefaultAsync(x => x.Id == id && x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.NgayNhan = request.NgayNhan;
        entity.KetQua = request.KetQua.Trim();
        entity.NoiDungYKien = request.NoiDungYKien;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            hoSoId,
            data.BoHoSo.Id,
            "CAP_NHAT_Y_KIEN_DON_VI",
            $"Cập nhật ý kiến của đơn vị {entity.DonViGopYId}",
            actor));

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToYKienDto(entity);
    }

    public async Task<bool> DeleteYKienDonViAsync(
        Guid hoSoId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null)
        {
            return false;
        }

        EnsureCanModify(data.HoSo, data.BoHoSo);
        var entity = await dbContext.HoSoXayDungVanBanYKienDonVis
            .FirstOrDefaultAsync(x => x.Id == id && x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor.UserId.ToString();
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            hoSoId,
            data.BoHoSo.Id,
            "XOA_MEM_Y_KIEN_DON_VI",
            $"Xóa mềm ý kiến của đơn vị {entity.DonViGopYId}",
            actor));

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<XayDungVanBanTongHopYKienDto?> GetTongHopYKienAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default)
    {
        var data = await GetDraftAsync(hoSoId, tracking: false, cancellationToken);
        if (data is null)
        {
            return null;
        }

        var counts = await GetYKienCountsAsync(hoSoId, cancellationToken);
        return new XayDungVanBanTongHopYKienDto(
            counts.TongSoYKien,
            counts.SoDongY,
            counts.SoKhongDongY,
            counts.SoYKienKhac,
            counts.SoKhongPhanHoi,
            data.SoanThao.NoiDungTongHopTiepThuGiaiTrinh,
            data.SoanThao.LoaiTaiLieuTongHopYKienId);
    }

    public async Task<XayDungVanBanTongHopYKienDto?> UpdateTongHopYKienAsync(
        Guid hoSoId,
        CapNhatTongHopYKienRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null)
        {
            return null;
        }

        EnsureCanModify(data.HoSo, data.BoHoSo);
        data.SoanThao.NoiDungTongHopTiepThuGiaiTrinh = request.NoiDungTongHopTiepThuGiaiTrinh;
        var counts = await GetYKienCountsAsync(hoSoId, cancellationToken);
        data.SoanThao.TongSoYKien = counts.TongSoYKien;
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            hoSoId,
            data.BoHoSo.Id,
            "CAP_NHAT_TONG_HOP_Y_KIEN",
            "Cập nhật tổng hợp, tiếp thu và giải trình ý kiến",
            actor));

        await dbContext.SaveChangesAsync(cancellationToken);
        return new XayDungVanBanTongHopYKienDto(
            counts.TongSoYKien,
            counts.SoDongY,
            counts.SoKhongDongY,
            counts.SoYKienKhac,
            counts.SoKhongPhanHoi,
            data.SoanThao.NoiDungTongHopTiepThuGiaiTrinh,
            data.SoanThao.LoaiTaiLieuTongHopYKienId);
    }

    public async Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetFileTongHopYKienAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default)
    {
        var data = await GetDraftAsync(hoSoId, tracking: false, cancellationToken);
        if (data is null)
        {
            return null;
        }

        if (data.SoanThao.LoaiTaiLieuTongHopYKienId is not Guid loaiTaiLieuId)
        {
            return Array.Empty<XayDungVanBanTaiLieuDto>();
        }

        return await dbContext.HoSoXayDungVanBanFiles
            .AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId
                && x.LoaiTaiLieuId == loaiTaiLieuId
                && !x.IsDeleted)
            .OrderByDescending(x => x.PhienBan)
            .Select(x => ToTaiLieuDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<XayDungVanBanTaiLieuDto?> UploadFileTongHopYKienAsync(
        Guid hoSoId,
        TaiFileTongHopYKienRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        if (request.LoaiTaiLieuId == Guid.Empty || request.NoiDung.Length == 0)
        {
            throw new InvalidOperationException("Thiếu loại tài liệu hoặc file tổng hợp ý kiến.");
        }

        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null)
        {
            return null;
        }

        EnsureCanModify(data.HoSo, data.BoHoSo);
        var safeFileName = Path.GetFileName(request.TenFile);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            throw new InvalidOperationException("Tên file không hợp lệ.");
        }

        var relativeDirectory = Path.Combine("uploads", "xay-dung-van-ban", hoSoId.ToString("N"));
        var physicalDirectory = Path.Combine(environment.ContentRootPath, relativeDirectory);
        Directory.CreateDirectory(physicalDirectory);
        var storageFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{safeFileName}";
        var physicalPath = Path.Combine(physicalDirectory, storageFileName);
        var relativePath = Path.Combine(relativeDirectory, storageFileName).Replace('\\', '/');

        try
        {
            await using (var output = File.Create(physicalPath))
            {
                await request.NoiDung.CopyToAsync(output, cancellationToken);
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var previousLoaiTaiLieuId = data.SoanThao.LoaiTaiLieuTongHopYKienId;
            var files = await dbContext.HoSoXayDungVanBanFiles
                .Where(x => x.HoSoXayDungVanBanId == hoSoId
                    && !x.IsDeleted
                    && (x.LoaiTaiLieuId == request.LoaiTaiLieuId
                        || (previousLoaiTaiLieuId.HasValue && x.LoaiTaiLieuId == previousLoaiTaiLieuId.Value)))
                .ToListAsync(cancellationToken);
            var sameTypeFiles = files.Where(x => x.LoaiTaiLieuId == request.LoaiTaiLieuId).ToList();
            var nextVersion = sameTypeFiles.Count == 0 ? 1 : sameTypeFiles.Max(x => x.PhienBan) + 1;
            foreach (var currentFile in files.Where(x => x.IsCurrent))
            {
                currentFile.IsCurrent = false;
                currentFile.UpdatedAt = DateTime.UtcNow;
                currentFile.UpdatedBy = actor.UserId.ToString();
            }

            var file = new HoSoXayDungVanBanFile
            {
                HoSoXayDungVanBanId = hoSoId,
                LoaiTaiLieuId = request.LoaiTaiLieuId,
                TenTaiLieu = "Tổng hợp ý kiến",
                PhienBan = nextVersion,
                TenFile = safeFileName,
                DuongDanFile = relativePath,
                MimeType = request.MimeType,
                DungLuong = request.NoiDung.Length,
                IsCurrent = true,
                NguoiTaiLenId = actor.UserId,
                CreatedBy = actor.UserId.ToString()
            };
            dbContext.HoSoXayDungVanBanFiles.Add(file);
            dbContext.BoHoSoNghiepVuTaiLieus.Add(new BoHoSoNghiepVuTaiLieu
            {
                BoHoSoNghiepVuId = data.BoHoSo.Id,
                HoSoXayDungVanBanFileId = file.Id,
                LoaiTaiLieuId = request.LoaiTaiLieuId,
                HinhThucThem = "TaoMoi",
                ThuTu = nextVersion,
                CreatedBy = actor.UserId.ToString()
            });
            data.SoanThao.LoaiTaiLieuTongHopYKienId = request.LoaiTaiLieuId;
            dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
                hoSoId,
                data.BoHoSo.Id,
                "CAP_NHAT_FILE_TONG_HOP_Y_KIEN",
                "Cập nhật file tổng hợp ý kiến",
                actor,
                file.Id));

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return ToTaiLieuDto(file);
        }
        catch
        {
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            throw;
        }
    }

    public async Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default)
    {
        var data = await GetDraftAsync(hoSoId, tracking: false, cancellationToken);
        if (data is null) return null;

        return await dbContext.HoSoXayDungVanBanFiles
            .AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted
                && (!data.SoanThao.LoaiTaiLieuTongHopYKienId.HasValue
                    || x.LoaiTaiLieuId != data.SoanThao.LoaiTaiLieuTongHopYKienId.Value))
            .OrderBy(x => x.TenTaiLieu).ThenByDescending(x => x.PhienBan)
            .Select(x => ToTaiLieuDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(
        Guid hoSoId,
        TaiTaiLieuSoanThaoRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        if (request.LoaiTaiLieuId == Guid.Empty || request.NoiDung.Length == 0
            || string.IsNullOrWhiteSpace(request.TenTaiLieu))
        {
            throw new InvalidOperationException("Thiếu loại, tên tài liệu hoặc file tải lên.");
        }

        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null) return null;
        EnsureCanModify(data.HoSo, data.BoHoSo);
        if (data.SoanThao.LoaiTaiLieuTongHopYKienId == request.LoaiTaiLieuId)
        {
            throw new InvalidOperationException("Loại tài liệu tổng hợp ý kiến phải được tải lên qua API file tổng hợp ý kiến.");
        }

        var safeFileName = Path.GetFileName(request.TenFile);
        if (string.IsNullOrWhiteSpace(safeFileName)) throw new InvalidOperationException("Tên file không hợp lệ.");

        var relativeDirectory = Path.Combine("uploads", "xay-dung-van-ban", hoSoId.ToString("N"));
        var physicalDirectory = Path.Combine(environment.ContentRootPath, relativeDirectory);
        Directory.CreateDirectory(physicalDirectory);
        var storageFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{safeFileName}";
        var physicalPath = Path.Combine(physicalDirectory, storageFileName);

        try
        {
            await using (var output = File.Create(physicalPath))
            {
                await request.NoiDung.CopyToAsync(output, cancellationToken);
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var previousFiles = await dbContext.HoSoXayDungVanBanFiles
                .Where(x => x.HoSoXayDungVanBanId == hoSoId && x.LoaiTaiLieuId == request.LoaiTaiLieuId && !x.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var previousFile in previousFiles.Where(x => x.IsCurrent))
            {
                previousFile.IsCurrent = false;
                previousFile.UpdatedAt = DateTime.UtcNow;
                previousFile.UpdatedBy = actor.UserId.ToString();
            }

            var file = new HoSoXayDungVanBanFile
            {
                HoSoXayDungVanBanId = hoSoId,
                LoaiTaiLieuId = request.LoaiTaiLieuId,
                TenTaiLieu = request.TenTaiLieu.Trim(),
                PhienBan = previousFiles.Count == 0 ? 1 : previousFiles.Max(x => x.PhienBan) + 1,
                TenFile = safeFileName,
                DuongDanFile = Path.Combine(relativeDirectory, storageFileName).Replace('\\', '/'),
                MimeType = request.MimeType,
                DungLuong = request.NoiDung.Length,
                IsCurrent = true,
                NguoiTaiLenId = actor.UserId,
                CreatedBy = actor.UserId.ToString()
            };
            dbContext.HoSoXayDungVanBanFiles.Add(file);
            dbContext.BoHoSoNghiepVuTaiLieus.Add(new BoHoSoNghiepVuTaiLieu
            {
                BoHoSoNghiepVuId = data.BoHoSo.Id,
                HoSoXayDungVanBanFileId = file.Id,
                LoaiTaiLieuId = file.LoaiTaiLieuId,
                HinhThucThem = "TaoMoi",
                ThuTu = file.PhienBan,
                CreatedBy = actor.UserId.ToString()
            });
            dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
                hoSoId, data.BoHoSo.Id, "TAI_TAI_LIEU_SOAN_THAO", $"Tải tài liệu {file.TenTaiLieu}", actor, file.Id));
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

    public async Task<bool> DeleteTaiLieuAsync(Guid hoSoId, Guid fileId, CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null) return false;
        EnsureCanModify(data.HoSo, data.BoHoSo);

        var file = await dbContext.HoSoXayDungVanBanFiles.FirstOrDefaultAsync(x =>
            x.Id == fileId && x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted, cancellationToken);
        if (file is null) return false;
        if (data.SoanThao.LoaiTaiLieuTongHopYKienId == file.LoaiTaiLieuId)
        {
            throw new InvalidOperationException("File tổng hợp ý kiến không thể xóa qua API tài liệu soạn thảo.");
        }

        file.IsDeleted = true;
        file.IsCurrent = false;
        file.UpdatedAt = DateTime.UtcNow;
        file.UpdatedBy = actor.UserId.ToString();
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(CreateTimeline(
            hoSoId, data.BoHoSo.Id, "XOA_MEM_TAI_LIEU_SOAN_THAO", $"Xóa tài liệu {file.TenTaiLieu}", actor, file.Id));
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DieuKienTrinhThamDinhDto?> KiemTraTruocTrinhThamDinhAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default)
    {
        var data = await GetDraftAsync(hoSoId, tracking: false, cancellationToken);
        if (data is null) return null;
        var dieuKienChuaDat = await GetDieuKienChuaDatAsync(data, cancellationToken);
        return new DieuKienTrinhThamDinhDto(dieuKienChuaDat.Count == 0, dieuKienChuaDat);
    }

    public async Task<TrinhThamDinhDto?> TrinhThamDinhAsync(
        Guid hoSoId,
        TrinhThamDinhRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = RequireActor();
        if (request.BuocQuyTrinhTiepTheoId == Guid.Empty || request.TrangThaiHoSoTiepTheoId == Guid.Empty
            || request.DonViNhanThamDinhId == Guid.Empty)
        {
            throw new InvalidOperationException("Thiếu bước, trạng thái hoặc đơn vị nhận thẩm định.");
        }

        var data = await GetDraftAsync(hoSoId, tracking: true, cancellationToken);
        if (data is null) return null;
        EnsureCanModify(data.HoSo, data.BoHoSo);
        var dieuKienChuaDat = await GetDieuKienChuaDatAsync(data, cancellationToken);
        if (dieuKienChuaDat.Count > 0) throw new InvalidOperationException(string.Join(" ", dieuKienChuaDat));

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var newBoHoSo = new BoHoSoNghiepVu
        {
            HoSoXayDungVanBanId = hoSoId,
            BuocQuyTrinhId = request.BuocQuyTrinhTiepTheoId,
            LoaiBoHoSo = LoaiBoHoSo.TrinhThamDinh,
            TrangThai = TrangThaiBoHoSo.Nhap,
            LanXuLy = 1,
            BoHoSoNguonId = data.BoHoSo.Id,
            NguoiLapId = actor.UserId,
            DonViLapId = actor.DonViId,
            NoiDungGhiChu = request.NoiDungGhiChu,
            CreatedBy = actor.UserId.ToString()
        };
        dbContext.BoHoSoNghiepVus.Add(newBoHoSo);
        dbContext.HoSoXayDungVanBanTrinhThamDinhs.Add(new HoSoXayDungVanBanTrinhThamDinh
        {
            BoHoSoNghiepVuId = newBoHoSo.Id,
            NgayGuiThamDinh = now,
            DonViNhanThamDinhId = request.DonViNhanThamDinhId,
            NoiDungDeNghiThamDinh = request.NoiDungGhiChu
        });

        var sourceLinks = await (
            from link in dbContext.BoHoSoNghiepVuTaiLieus
            join file in dbContext.HoSoXayDungVanBanFiles on link.HoSoXayDungVanBanFileId equals file.Id
            where link.BoHoSoNghiepVuId == data.BoHoSo.Id
                  && !link.IsDeleted
                  && !file.IsDeleted
                  && file.IsCurrent
            select new { Link = link, File = file }).ToListAsync(cancellationToken);
        foreach (var source in sourceLinks)
        {
            dbContext.BoHoSoNghiepVuTaiLieus.Add(new BoHoSoNghiepVuTaiLieu
            {
                BoHoSoNghiepVuId = newBoHoSo.Id,
                HoSoXayDungVanBanFileId = source.Link.HoSoXayDungVanBanFileId,
                LoaiTaiLieuId = source.Link.LoaiTaiLieuId,
                HinhThucThem = "KeThua",
                BoHoSoTaiLieuNguonId = source.Link.Id,
                BatBuoc = source.Link.BatBuoc,
                ThuTu = source.Link.ThuTu,
                CreatedBy = actor.UserId.ToString()
            });
        }

        var previousBuocId = data.HoSo.BuocHienTaiId;
        var previousTrangThaiId = data.HoSo.TrangThaiHoSoId;
        data.BoHoSo.TrangThai = TrangThaiBoHoSo.DaGui;
        data.BoHoSo.NgayGui = now;
        data.BoHoSo.UpdatedAt = now;
        data.BoHoSo.UpdatedBy = actor.UserId.ToString();
        data.HoSo.BuocHienTaiId = request.BuocQuyTrinhTiepTheoId;
        data.HoSo.TrangThaiHoSoId = request.TrangThaiHoSoTiepTheoId;
        data.HoSo.UpdatedAt = now;
        data.HoSo.UpdatedBy = actor.UserId.ToString();
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(new HoSoXayDungVanBanLichSuXuLy
        {
            HoSoXayDungVanBanId = hoSoId,
            BoHoSoNghiepVuId = newBoHoSo.Id,
            BuocQuyTrinhTruocId = previousBuocId,
            BuocQuyTrinhSauId = request.BuocQuyTrinhTiepTheoId,
            TrangThaiTruocId = previousTrangThaiId,
            TrangThaiSauId = request.TrangThaiHoSoTiepTheoId,
            HanhDong = "TRINH_THAM_DINH",
            NoiDung = "Trình hồ sơ sang giai đoạn thẩm định",
            NguoiXuLyId = actor.UserId,
            DonViXuLyId = actor.DonViId,
            CreatedBy = actor.UserId.ToString()
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new TrinhThamDinhDto(hoSoId, data.BoHoSo.Id, newBoHoSo.Id,
            data.HoSo.BuocHienTaiId, data.HoSo.TrangThaiHoSoId);
    }

    private async Task<IReadOnlyList<string>> GetDieuKienChuaDatAsync(DraftData data, CancellationToken cancellationToken)
    {
        var conditions = new List<string>();
        if (string.IsNullOrWhiteSpace(data.SoanThao.CanCuXayDung)) conditions.Add("Chưa cập nhật căn cứ xây dựng.");
        if (string.IsNullOrWhiteSpace(data.SoanThao.PhamViDieuChinh)) conditions.Add("Chưa cập nhật phạm vi điều chỉnh.");
        if (string.IsNullOrWhiteSpace(data.SoanThao.NoiDungChinhSach)) conditions.Add("Chưa cập nhật nội dung chính sách.");
        if (string.IsNullOrWhiteSpace(data.SoanThao.NoiDungTongHopTiepThuGiaiTrinh)) conditions.Add("Chưa cập nhật nội dung tổng hợp, tiếp thu và giải trình ý kiến.");

        var hasCurrentDraftFile = await dbContext.HoSoXayDungVanBanFiles.AnyAsync(x =>
            x.HoSoXayDungVanBanId == data.HoSo.Id && !x.IsDeleted && x.IsCurrent
            && (!data.SoanThao.LoaiTaiLieuTongHopYKienId.HasValue || x.LoaiTaiLieuId != data.SoanThao.LoaiTaiLieuTongHopYKienId.Value), cancellationToken);
        if (!hasCurrentDraftFile) conditions.Add("Chưa có tài liệu dự thảo hiện hành.");

        var hasCurrentSummaryFile = data.SoanThao.LoaiTaiLieuTongHopYKienId.HasValue
            && await dbContext.HoSoXayDungVanBanFiles.AnyAsync(x => x.HoSoXayDungVanBanId == data.HoSo.Id
                && x.LoaiTaiLieuId == data.SoanThao.LoaiTaiLieuTongHopYKienId.Value
                && !x.IsDeleted && x.IsCurrent, cancellationToken);
        if (!hasCurrentSummaryFile) conditions.Add("Chưa có file tổng hợp ý kiến hiện hành.");
        return conditions;
    }

    private async Task<DraftData?> GetDraftAsync(Guid hoSoId, bool tracking, CancellationToken cancellationToken)
    {
        var hoSos = tracking ? dbContext.HoSoXayDungVanBans : dbContext.HoSoXayDungVanBans.AsNoTracking();
        var boHoSos = tracking ? dbContext.BoHoSoNghiepVus : dbContext.BoHoSoNghiepVus.AsNoTracking();
        var soanThaos = tracking ? dbContext.HoSoXayDungVanBanSoanThaos : dbContext.HoSoXayDungVanBanSoanThaos.AsNoTracking();

        return await (
            from hoSo in hoSos
            join boHoSo in boHoSos on hoSo.Id equals boHoSo.HoSoXayDungVanBanId
            join soanThao in soanThaos on boHoSo.Id equals soanThao.BoHoSoNghiepVuId
            where hoSo.Id == hoSoId
                  && !hoSo.IsDeleted
                  && !boHoSo.IsDeleted
                  && boHoSo.LoaiBoHoSo == LoaiBoHoSo.SoanThao
            orderby boHoSo.LanXuLy descending
            select new DraftData(hoSo, boHoSo, soanThao))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static void EnsureRequiredIds(TaoHoSoSoanThaoRequest request)
    {
        if (request.DanhMucVanBanId == Guid.Empty
            || request.QuyTrinhSoanThaoId == Guid.Empty
            || request.BuocHienTaiId == Guid.Empty
            || request.TrangThaiHoSoId == Guid.Empty
            || request.DonViChuTriSoanThaoId == Guid.Empty)
        {
            throw new InvalidOperationException("Thiếu thông tin danh mục, quy trình, bước, trạng thái hoặc đơn vị chủ trì.");
        }
    }

    private static void EnsureCanModify(HoSoXayDungVanBan hoSo, BoHoSoNghiepVu boHoSo)
    {
        if (hoSo.BuocHienTaiId != boHoSo.BuocQuyTrinhId || boHoSo.TrangThai != TrangThaiBoHoSo.Nhap)
        {
            throw new InvalidOperationException("Hồ sơ không còn ở trạng thái được phép cập nhật trong giai đoạn soạn thảo.");
        }
    }

    private static void EnsureYKienRequest(Guid donViGopYId, string ketQua)
    {
        if (donViGopYId == Guid.Empty)
        {
            throw new InvalidOperationException("Chưa chọn đơn vị góp ý.");
        }

        EnsureYKienKetQua(ketQua);
    }

    private static void EnsureYKienKetQua(string ketQua)
    {
        if (ketQua is not ("DongY" or "KhongDongY" or "YKienKhac" or "KhongPhanHoi"))
        {
            throw new InvalidOperationException("Kết quả ý kiến không hợp lệ.");
        }
    }

    private CurrentActor RequireActor()
    {
        if (!currentUserContext.IsAuthenticated
            || currentUserContext.UserId is null
            || currentUserContext.DonViId is null
            || currentUserContext.DonViId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý.");
        }

        return new CurrentActor(currentUserContext.UserId.Value, currentUserContext.DonViId.Value);
    }

    private async Task<YKienCounts> GetYKienCountsAsync(Guid hoSoId, CancellationToken cancellationToken)
    {
        var items = await dbContext.HoSoXayDungVanBanYKienDonVis
            .AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted)
            .Select(x => x.KetQua)
            .ToListAsync(cancellationToken);

        return new YKienCounts(
            items.Count,
            items.Count(x => x == "DongY"),
            items.Count(x => x == "KhongDongY"),
            items.Count(x => x == "YKienKhac"),
            items.Count(x => x == "KhongPhanHoi"));
    }

    private static HoSoXayDungVanBanLichSuXuLy CreateTimeline(
        Guid hoSoId,
        Guid boHoSoId,
        string hanhDong,
        string noiDung,
        CurrentActor actor,
        Guid? fileId = null) => new()
    {
        HoSoXayDungVanBanId = hoSoId,
        BoHoSoNghiepVuId = boHoSoId,
        HanhDong = hanhDong,
        NoiDung = noiDung,
        NguoiXuLyId = actor.UserId,
        DonViXuLyId = actor.DonViId,
        HoSoXayDungVanBanFileId = fileId,
        CreatedBy = actor.UserId.ToString()
    };

    private static XayDungVanBanSoanThaoDto ToDto(
        HoSoXayDungVanBan hoSo,
        BoHoSoNghiepVu boHoSo,
        string? canCuXayDung,
        string? phamViDieuChinh,
        string? noiDungChinhSach) => new(
        hoSo.Id,
        boHoSo.Id,
        hoSo.MaHoSo,
        hoSo.TenHoSo,
        hoSo.TenDuThaoVanBan,
        hoSo.DanhMucVanBanId,
        hoSo.QuyTrinhSoanThaoId,
        hoSo.BuocHienTaiId,
        hoSo.TrangThaiHoSoId,
        hoSo.DonViChuTriSoanThaoId,
        hoSo.NguoiPhuTrachId,
        hoSo.NamXayDung,
        hoSo.ThoiGianDuKienBatDau,
        hoSo.ThoiGianDuKienHoanThanh,
        hoSo.MoTa,
        canCuXayDung,
        phamViDieuChinh,
        noiDungChinhSach,
        hoSo.CreatedAt);

    private static XayDungVanBanYKienDonViDto ToYKienDto(HoSoXayDungVanBanYKienDonVi entity) => new(
        entity.Id,
        entity.DonViGopYId,
        entity.NgayNhan,
        entity.KetQua,
        entity.NoiDungYKien,
        entity.CreatedAt,
        entity.UpdatedAt);

    private static XayDungVanBanTaiLieuDto ToTaiLieuDto(HoSoXayDungVanBanFile entity) => new(
        entity.Id,
        entity.LoaiTaiLieuId,
        entity.TenTaiLieu,
        entity.PhienBan,
        entity.TenFile,
        entity.DuongDanFile,
        entity.MimeType,
        entity.DungLuong,
        entity.IsCurrent,
        entity.NgayTaiLen);

    private sealed record DraftData(
        HoSoXayDungVanBan HoSo,
        BoHoSoNghiepVu BoHoSo,
        HoSoXayDungVanBanSoanThao SoanThao);

    private sealed record CurrentActor(Guid UserId, Guid DonViId);

    private sealed record YKienCounts(
        int TongSoYKien,
        int SoDongY,
        int SoKhongDongY,
        int SoYKienKhac,
        int SoKhongPhanHoi);
}
