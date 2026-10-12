SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @QuyTrinh uniqueidentifier = '33333333-3333-3333-3333-333333333301';
DECLARE @BuocDeXuat uniqueidentifier = '33333333-3333-3333-3333-333333333311';
DECLARE @BuocTrinhHoSoCu uniqueidentifier = '33333333-3333-3333-3333-333333333312';
DECLARE @BuocPheDuyetCapNhat uniqueidentifier = '33333333-3333-3333-3333-333333333313';
DECLARE @BuocCapNhatKetQuaCu uniqueidentifier = '33333333-3333-3333-3333-333333333314';
DECLARE @BuocHoanThanhCu uniqueidentifier = '33333333-3333-3333-3333-333333333315';

DECLARE @ChuyenDeXuatSangKetQua uniqueidentifier = '33333333-3333-3333-3333-333333333321';
DECLARE @ChuyenKetQuaTraLaiDeXuat uniqueidentifier = '33333333-3333-3333-3333-333333333324';
DECLARE @ChuyenKetQuaKetThuc uniqueidentifier = '33333333-3333-3333-3333-333333333326';

DECLARE @TrangThaiDangSoanThao uniqueidentifier = '11111111-1111-1111-1111-111111111102';
DECLARE @TrangThaiDaTrinh uniqueidentifier = '11111111-1111-1111-1111-111111111103';
DECLARE @TrangThaiDaPheDuyet uniqueidentifier = '11111111-1111-1111-1111-111111111104';
DECLARE @TrangThaiBiTraLai uniqueidentifier = '11111111-1111-1111-1111-111111111105';
DECLARE @TrangThaiKhongPheDuyet uniqueidentifier = '11111111-1111-1111-1111-111111111106';
DECLARE @TrangThaiDaCapNhatKetQua uniqueidentifier = '11111111-1111-1111-1111-111111111107';
DECLARE @TrangThaiHoanThanh uniqueidentifier = '11111111-1111-1111-1111-111111111108';
DECLARE @TrangThaiDaChuyenQuyTrinh uniqueidentifier = '11111111-1111-1111-1111-111111111109';

DECLARE @HanhDongCapNhatHoSo uniqueidentifier = '22222222-2222-2222-2222-222222222202';
DECLARE @HanhDongTrinhPheDuyet uniqueidentifier = '22222222-2222-2222-2222-222222222203';
DECLARE @HanhDongPheDuyet uniqueidentifier = '22222222-2222-2222-2222-222222222204';
DECLARE @HanhDongTraLai uniqueidentifier = '22222222-2222-2222-2222-222222222205';
DECLARE @HanhDongKhongPheDuyet uniqueidentifier = '22222222-2222-2222-2222-222222222206';
DECLARE @HanhDongCapNhatKetQua uniqueidentifier = '22222222-2222-2222-2222-222222222207';
DECLARE @HanhDongHoanThanh uniqueidentifier = '22222222-2222-2222-2222-222222222208';
DECLARE @HanhDongKhoiTaoQuyTrinh uniqueidentifier = '22222222-2222-2222-2222-222222222209';

UPDATE dm.DanhMucQuyTrinhSoanThaos
SET TenQuyTrinh = N'Đề xuất danh mục / Đăng ký xây dựng QPPL',
    LoaiQuyTrinh = N'DangKyXayDungQPPL',
    MoTa = N'Quy trình đăng ký xây dựng văn bản QPPL theo tài liệu QT đăng ký',
    UpdatedDate = SYSDATETIME()
WHERE Id = @QuyTrinh;

UPDATE dm.DanhMucBuocQuyTrinhs
SET MaBuoc = N'DE_XUAT_DANG_KY',
    TenBuoc = N'Đề xuất danh mục/Đăng ký xây dựng văn bản QPPL',
    ThuTuSapXep = 1,
    LoaiBuoc = N'NhapLieu',
    BatBuoc = 1,
    YeuCauFileDinhKem = 0,
    MoTa = N'Cơ quan chủ trì lập hồ sơ, trình hồ sơ đến cơ quan có thẩm quyền và hệ thống lưu trữ hồ sơ đề xuất/đăng ký.',
    UpdatedDate = SYSDATETIME()
WHERE Id = @BuocDeXuat;

UPDATE dm.DanhMucBuocQuyTrinhs
SET MaBuoc = N'PHE_DUYET_CAP_NHAT_KET_QUA',
    TenBuoc = N'Phê duyệt và cập nhật kết quả',
    ThuTuSapXep = 2,
    LoaiBuoc = N'XuLy',
    BatBuoc = 1,
    YeuCauFileDinhKem = 1,
    MoTa = N'Việc phê duyệt diễn ra ngoài hệ thống; cơ quan chủ trì cập nhật kết quả phê duyệt lên phần mềm.',
    UpdatedDate = SYSDATETIME()
WHERE Id = @BuocPheDuyetCapNhat;

UPDATE dbo.DangKyXayDungVanBans
SET BuocHienTaiId = CASE
        WHEN BuocHienTaiId IN (@BuocDeXuat, @BuocTrinhHoSoCu) THEN @BuocDeXuat
        WHEN BuocHienTaiId IN (@BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu) THEN @BuocPheDuyetCapNhat
        ELSE BuocHienTaiId
    END,
    UpdatedAt = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND BuocHienTaiId IN (@BuocDeXuat, @BuocTrinhHoSoCu, @BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu);

UPDATE dbo.DangKyXayDungVanBanLichSuXuLys
SET TuBuocId = CASE
        WHEN TuBuocId IN (@BuocDeXuat, @BuocTrinhHoSoCu) THEN @BuocDeXuat
        WHEN TuBuocId IN (@BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu) THEN @BuocPheDuyetCapNhat
        ELSE TuBuocId
    END,
    DenBuocId = CASE
        WHEN DenBuocId IN (@BuocDeXuat, @BuocTrinhHoSoCu) THEN @BuocDeXuat
        WHEN DenBuocId IN (@BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu) THEN @BuocPheDuyetCapNhat
        ELSE DenBuocId
    END,
    TenBuocTu = CASE
        WHEN TuBuocId IN (@BuocDeXuat, @BuocTrinhHoSoCu) THEN N'Đề xuất danh mục/Đăng ký xây dựng văn bản QPPL'
        WHEN TuBuocId IN (@BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu) THEN N'Phê duyệt và cập nhật kết quả'
        ELSE TenBuocTu
    END,
    TenBuocDen = CASE
        WHEN DenBuocId IN (@BuocDeXuat, @BuocTrinhHoSoCu) THEN N'Đề xuất danh mục/Đăng ký xây dựng văn bản QPPL'
        WHEN DenBuocId IN (@BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu) THEN N'Phê duyệt và cập nhật kết quả'
        ELSE TenBuocDen
    END,
    UpdatedAt = SYSDATETIME()
WHERE DangKyXayDungVanBanId IN (
    SELECT Id FROM dbo.DangKyXayDungVanBans WHERE QuyTrinhSoanThaoId = @QuyTrinh
);

UPDATE dm.DanhMucTrangThaiNghiepVuQuyTrinhs
SET BuocQuyTrinhId = CASE
        WHEN BuocQuyTrinhId IN (@BuocDeXuat, @BuocTrinhHoSoCu) THEN @BuocDeXuat
        WHEN BuocQuyTrinhId IN (@BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu) THEN @BuocPheDuyetCapNhat
        ELSE BuocQuyTrinhId
    END,
    UpdatedDate = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND BuocQuyTrinhId IN (@BuocDeXuat, @BuocTrinhHoSoCu, @BuocPheDuyetCapNhat, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu);

UPDATE dm.DanhMucChuyenBuocQuyTrinhs
SET TuBuocId = @BuocDeXuat,
    DenBuocId = @BuocPheDuyetCapNhat,
    DieuKienKetQua = N'TRINH_PHE_DUYET',
    LoaiChuyenBuoc = N'Forward',
    LaNhanhMacDinh = 1,
    YeuCauNhapLyDo = 0,
    IsKetThuc = 0,
    MoTa = N'Trình hồ sơ đề xuất/đăng ký sang bước phê duyệt và cập nhật kết quả.',
    UpdatedDate = SYSDATETIME()
WHERE Id = @ChuyenDeXuatSangKetQua;

UPDATE dm.DanhMucChuyenBuocQuyTrinhs
SET TuBuocId = @BuocPheDuyetCapNhat,
    DenBuocId = @BuocDeXuat,
    DieuKienKetQua = N'TRA_LAI',
    LoaiChuyenBuoc = N'Return',
    LaNhanhMacDinh = 0,
    YeuCauNhapLyDo = 1,
    IsKetThuc = 0,
    MoTa = N'Trả hồ sơ về bước đề xuất/đăng ký để chỉnh sửa.',
    UpdatedDate = SYSDATETIME()
WHERE Id = @ChuyenKetQuaTraLaiDeXuat;

UPDATE dm.DanhMucChuyenBuocQuyTrinhs
SET TuBuocId = @BuocPheDuyetCapNhat,
    DenBuocId = @BuocPheDuyetCapNhat,
    DieuKienKetQua = N'HOAN_THANH',
    LoaiChuyenBuoc = N'Complete',
    LaNhanhMacDinh = 0,
    YeuCauNhapLyDo = 0,
    IsKetThuc = 1,
    MoTa = N'Kết thúc quy trình đăng ký sau khi cập nhật kết quả.',
    UpdatedDate = SYSDATETIME()
WHERE Id = @ChuyenKetQuaKetThuc;

DELETE FROM dm.DanhMucChuyenBuocQuyTrinhs
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND Id NOT IN (@ChuyenDeXuatSangKetQua, @ChuyenKetQuaTraLaiDeXuat, @ChuyenKetQuaKetThuc);

DELETE FROM dm.DanhMucBuocQuyTrinhs
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND Id IN (@BuocTrinhHoSoCu, @BuocCapNhatKetQuaCu, @BuocHoanThanhCu);

UPDATE dbo.DangKyCauHinhChuyenTrangThais
SET BuocHienTaiId = @BuocDeXuat,
    BuocTiepTheoId = @BuocDeXuat,
    ChuyenBuocId = NULL,
    UpdatedAt = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND HanhDongId = @HanhDongCapNhatHoSo;

UPDATE dbo.DangKyCauHinhChuyenTrangThais
SET BuocHienTaiId = @BuocDeXuat,
    BuocTiepTheoId = @BuocPheDuyetCapNhat,
    ChuyenBuocId = @ChuyenDeXuatSangKetQua,
    UpdatedAt = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND HanhDongId = @HanhDongTrinhPheDuyet;

UPDATE dbo.DangKyCauHinhChuyenTrangThais
SET BuocHienTaiId = @BuocPheDuyetCapNhat,
    BuocTiepTheoId = @BuocPheDuyetCapNhat,
    ChuyenBuocId = NULL,
    UpdatedAt = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND HanhDongId IN (@HanhDongPheDuyet, @HanhDongCapNhatKetQua, @HanhDongKhoiTaoQuyTrinh);

UPDATE dbo.DangKyCauHinhChuyenTrangThais
SET BuocHienTaiId = @BuocPheDuyetCapNhat,
    BuocTiepTheoId = @BuocDeXuat,
    ChuyenBuocId = @ChuyenKetQuaTraLaiDeXuat,
    UpdatedAt = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND HanhDongId = @HanhDongTraLai;

UPDATE dbo.DangKyCauHinhChuyenTrangThais
SET BuocHienTaiId = @BuocPheDuyetCapNhat,
    BuocTiepTheoId = @BuocPheDuyetCapNhat,
    ChuyenBuocId = @ChuyenKetQuaKetThuc,
    UpdatedAt = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @QuyTrinh
  AND HanhDongId IN (@HanhDongKhongPheDuyet, @HanhDongHoanThanh);

COMMIT TRANSACTION;

SELECT qt.MaQuyTrinh, qt.TenQuyTrinh, COUNT(b.Id) AS SoBuoc
FROM dm.DanhMucQuyTrinhSoanThaos qt
LEFT JOIN dm.DanhMucBuocQuyTrinhs b ON b.QuyTrinhSoanThaoId = qt.Id
WHERE qt.Id = @QuyTrinh
GROUP BY qt.MaQuyTrinh, qt.TenQuyTrinh;

SELECT b.ThuTuSapXep, b.MaBuoc, b.TenBuoc, b.LoaiBuoc, b.Id
FROM dm.DanhMucBuocQuyTrinhs b
WHERE b.QuyTrinhSoanThaoId = @QuyTrinh
ORDER BY b.ThuTuSapXep, b.MaBuoc;

SELECT hs.Id, hs.MaHoSo, b.MaBuoc AS MaBuocHienTai, b.TenBuoc AS TenBuocHienTai
FROM dbo.DangKyXayDungVanBans hs
LEFT JOIN dm.DanhMucBuocQuyTrinhs b ON b.Id = hs.BuocHienTaiId
WHERE hs.Id = '494738a2-bef7-54b5-bc6a-d6feb7463e1c';
