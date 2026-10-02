# Tai lieu thiet ke va ke hoach xay dung DangKyXayDungVanBanService

## 1. Muc tieu

`DangKyXayDungVanBanService` quan ly nghiep vu "De xuat danh muc / Dang ky xay dung van ban QPPL".

Service nay phu trach:

- Tao va cap nhat ho so dang ky xay dung van ban.
- Trinh ho so den don vi phe duyet.
- Ghi nhan ket qua phe duyet, tra lai hoac khong phe duyet.
- Cap nhat ket qua, file quyet dinh/danh muc duoc phe duyet.
- Theo doi buoc hien tai va timeline xu ly cua tung ho so.
- Khoi tao/tham chieu sang quy trinh xay dung van ban QPPL sau khi ho so dang ky hoan thanh.

Service nay khong quan ly:

- Danh muc quy trinh, buoc quy trinh, chuyen buoc quy trinh.
- Danh muc loai van ban.
- Danh muc don vi.
- Nguoi dung, vai tro, quyen.

Nhung du lieu tren duoc tham chieu logic tu `DanhMucService` va `QuanTriHeThongService`.

## 2. Nguyen tac thiet ke

- Tach rieng du lieu danh muc va du lieu van hanh.
- Khong hardcode trang thai, hanh dong trong code nghiep vu.
- Buoc hien tai va trang thai hien tai cua ho so duoc luu bang Id.
- Timeline luu snapshot ten buoc, ten trang thai, ten nguoi xu ly, ten don vi xu ly de dam bao lich su khong bi thay doi khi danh muc doi ten.
- Khong bat buoc foreign key vat ly sang bang cua service khac; chi luu Id tham chieu logic.
- Ho so dang ky sau khi hoan thanh co the tao ho so xay dung van ban theo 1 trong 2 quy trinh:
  - Quy trinh xay dung van ban QPPL cap tinh doi voi Quyet dinh UBND.
  - Quy trinh xay dung van ban QPPL cap tinh doi voi Nghi quyet HDND.

## 3. Cac bang du lieu de xuat

### 3.1. DangKyXayDungVanBans

Bang ho so dang ky chinh.

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| MaHoSo | nvarchar(50) | Ma ho so dang ky |
| TenHoSo | nvarchar(500) | Ten ho so |
| TenVanBanDuKien | nvarchar(500) | Ten van ban QPPL du kien |
| LoaiVanBanId | uniqueidentifier | Tham chieu loai van ban |
| QuyTrinhSoanThaoId | uniqueidentifier | Quy trinh dang ky |
| BuocHienTaiId | uniqueidentifier | Buoc hien tai |
| TrangThaiHoSoId | uniqueidentifier | Trang thai hien tai |
| DonViSoanThaoId | uniqueidentifier | Don vi soan thao/dang ky |
| DonViPheDuyetId | uniqueidentifier | Don vi phe duyet |
| NamDangKy | int | Nam dang ky |
| CanCuDeXuat | nvarchar(max) | Can cu de xuat |
| SuCanThiet | nvarchar(max) | Su can thiet |
| NoiDungChinhSach | nvarchar(max) | Noi dung/chinh sach chinh |
| DuKienThoiGianTrinh | datetime2 | Thoi gian du kien trinh |
| KetQuaPheDuyetId | uniqueidentifier null | Ket qua phe duyet moi nhat |
| DaKhoiTaoQuyTrinhXayDung | bit | Da khoi tao quy trinh xay dung chua |
| HoSoXayDungVanBanId | uniqueidentifier null | Ho so xay dung van ban duoc tao |
| QuyTrinhXayDungTiepTheoId | uniqueidentifier null | Quy trinh tiep theo |
| NgayKhoiTaoQuyTrinhXayDung | datetime2 null | Ngay khoi tao quy trinh xay dung |
| IsDeleted | bit | Xoa mem |
| CreatedAt | datetime2 | Ngay tao |
| CreatedBy | nvarchar(100) | Nguoi tao |
| UpdatedAt | datetime2 null | Ngay cap nhat |
| UpdatedBy | nvarchar(100) null | Nguoi cap nhat |

### 3.2. DangKyXayDungVanBanFiles

Bang file dinh kem cua ho so.

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| DangKyXayDungVanBanId | uniqueidentifier | Ho so dang ky |
| LoaiFile | nvarchar(100) | ToTrinh, DuThaoVanBan, TaiLieuKhac, VanBanPheDuyet |
| TenFile | nvarchar(500) | Ten file |
| DuongDanFile | nvarchar(1000) | Duong dan luu file |
| DungLuong | bigint | Dung luong |
| MimeType | nvarchar(255) | Kieu file |
| MoTa | nvarchar(500) null | Mo ta |
| CreatedAt | datetime2 | Ngay tai len |
| CreatedBy | nvarchar(100) | Nguoi tai len |

### 3.3. DangKyXayDungVanBanLichSuXuLys

Bang timeline xu ly cua ho so.

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| DangKyXayDungVanBanId | uniqueidentifier | Ho so dang ky |
| TuBuocId | uniqueidentifier null | Buoc truoc |
| DenBuocId | uniqueidentifier null | Buoc sau |
| ChuyenBuocId | uniqueidentifier null | Cau hinh chuyen buoc |
| TenBuocTu | nvarchar(255) null | Snapshot ten buoc truoc |
| TenBuocDen | nvarchar(255) null | Snapshot ten buoc sau |
| HanhDongId | uniqueidentifier | Hanh dong xu ly |
| MaHanhDongSnapshot | nvarchar(100) | Snapshot ma hanh dong |
| TenHanhDongSnapshot | nvarchar(255) | Snapshot ten hanh dong |
| TrangThaiTruocId | uniqueidentifier null | Trang thai truoc |
| TrangThaiSauId | uniqueidentifier null | Trang thai sau |
| TenTrangThaiTruocSnapshot | nvarchar(255) null | Snapshot trang thai truoc |
| TenTrangThaiSauSnapshot | nvarchar(255) null | Snapshot trang thai sau |
| NoiDungXuLy | nvarchar(max) null | Noi dung xu ly |
| LyDoTraLai | nvarchar(max) null | Ly do tra lai |
| NguoiXuLyId | uniqueidentifier | Nguoi xu ly |
| TenNguoiXuLy | nvarchar(255) | Snapshot ten nguoi xu ly |
| DonViXuLyId | uniqueidentifier | Don vi xu ly |
| TenDonViXuLy | nvarchar(255) | Snapshot ten don vi |
| NgayXuLy | datetime2 | Thoi diem xu ly |

### 3.4. DangKyXayDungVanBanKetQuaPheDuyets

Bang ket qua phe duyet cua ho so.

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| DangKyXayDungVanBanId | uniqueidentifier | Ho so dang ky |
| KetQua | nvarchar(100) | DongY, KhongDongY, YeuCauBoSung |
| SoVanBan | nvarchar(100) null | So van ban phe duyet |
| NgayVanBan | datetime2 null | Ngay van ban |
| CoQuanPheDuyetId | uniqueidentifier | Don vi phe duyet |
| TenCoQuanPheDuyet | nvarchar(255) | Snapshot ten don vi phe duyet |
| NguoiKy | nvarchar(255) null | Nguoi ky |
| ChucVuNguoiKy | nvarchar(255) null | Chuc vu nguoi ky |
| NoiDungKetQua | nvarchar(max) null | Noi dung ket qua |
| FileKetQuaId | uniqueidentifier null | File ket qua |
| CreatedAt | datetime2 | Ngay nhap |
| CreatedBy | nvarchar(100) | Nguoi nhap |

### 3.5. DangKyXayDungVanBanLienKetQuyTrinhs

Bang lien ket tu ho so dang ky sang ho so xay dung van ban.

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| DangKyXayDungVanBanId | uniqueidentifier | Ho so dang ky nguon |
| HoSoXayDungVanBanId | uniqueidentifier | Ho so xay dung van ban duoc tao |
| LoaiVanBanId | uniqueidentifier | Loai van ban |
| QuyTrinhXayDungId | uniqueidentifier | Quy trinh xay dung tiep theo |
| MaQuyTrinhXayDung | nvarchar(100) | Snapshot ma quy trinh |
| TenQuyTrinhXayDung | nvarchar(500) | Snapshot ten quy trinh |
| NgayLienKet | datetime2 | Ngay lien ket |
| CreatedBy | nvarchar(100) | Nguoi thuc hien |

## 4. Bang cau hinh trang thai va hanh dong dong

De tranh hardcode trang thai, service can co cac bang cau hinh sau.

### 4.1. DangKyTrangThaiHoSos

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| MaTrangThai | nvarchar(100) | Ma trang thai |
| TenTrangThai | nvarchar(255) | Ten hien thi |
| MoTa | nvarchar(500) null | Mo ta |
| MauHienThi | nvarchar(50) null | Mau badge |
| ThuTuSapXep | int | Thu tu |
| LaTrangThaiKetThuc | bit | La trang thai ket thuc |
| TrangThai | bit | Con su dung |

Du lieu khoi tao goi y:

| MaTrangThai | TenTrangThai |
|---|---|
| MOI_TAO | Moi tao |
| DANG_SOAN_THAO | Dang soan thao |
| DA_TRINH_PHE_DUYET | Da trinh phe duyet |
| DA_PHE_DUYET | Da phe duyet |
| BI_TRA_LAI | Bi tra lai |
| KHONG_PHE_DUYET | Khong phe duyet |
| DA_CAP_NHAT_KET_QUA | Da cap nhat ket qua |
| HOAN_THANH | Hoan thanh |
| DA_CHUYEN_QUY_TRINH_XAY_DUNG | Da chuyen quy trinh xay dung |

### 4.2. DangKyHanhDongXuLys

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| MaHanhDong | nvarchar(100) | Ma hanh dong |
| TenHanhDong | nvarchar(255) | Ten hien thi |
| MoTa | nvarchar(500) null | Mo ta |
| LoaiHanhDong | nvarchar(100) | Create, Update, Forward, Return, Approve, Reject, Complete, StartNextWorkflow |
| YeuCauLyDo | bit | Co bat buoc ly do khong |
| YeuCauFileDinhKem | bit | Co bat buoc file khong |
| ThuTuSapXep | int | Thu tu |
| TrangThai | bit | Con su dung |

Du lieu khoi tao goi y:

| MaHanhDong | TenHanhDong | LoaiHanhDong |
|---|---|---|
| TAO_MOI | Tao moi | Create |
| CAP_NHAT_HO_SO | Cap nhat ho so | Update |
| TRINH_PHE_DUYET | Trinh phe duyet | Forward |
| PHE_DUYET | Phe duyet | Approve |
| TRA_LAI | Tra lai | Return |
| KHONG_PHE_DUYET | Khong phe duyet | Reject |
| CAP_NHAT_KET_QUA | Cap nhat ket qua | Update |
| HOAN_THANH | Hoan thanh | Complete |
| KHOI_TAO_QUY_TRINH_XAY_DUNG | Khoi tao quy trinh xay dung | StartNextWorkflow |

### 4.3. DangKyCauHinhChuyenTrangThais

Bang cau hinh trang thai tiep theo theo buoc hien tai, trang thai hien tai va hanh dong.

| Cot | Kieu du lieu | Ghi chu |
|---|---|---|
| Id | uniqueidentifier | Khoa chinh |
| QuyTrinhSoanThaoId | uniqueidentifier | Quy trinh ap dung |
| BuocHienTaiId | uniqueidentifier | Buoc hien tai |
| TrangThaiHienTaiId | uniqueidentifier | Trang thai hien tai |
| HanhDongId | uniqueidentifier | Hanh dong duoc phep |
| BuocTiepTheoId | uniqueidentifier | Buoc tiep theo |
| TrangThaiTiepTheoId | uniqueidentifier | Trang thai tiep theo |
| ChuyenBuocId | uniqueidentifier null | Tham chieu cau hinh chuyen buoc |
| NhomNhapLieu | nvarchar(100) | DonViSoanThao, DonViPheDuyet, HeThong |
| YeuCauLyDo | bit | Bat buoc ly do |
| YeuCauFileDinhKem | bit | Bat buoc file |
| LaKetThuc | bit | La cau hinh ket thuc |
| TrangThai | bit | Con su dung |

## 5. Luong nghiep vu chinh

### 5.1. Tao moi ho so

1. Don vi soan thao tao ho so.
2. Service tao ban ghi `DangKyXayDungVanBans`.
3. Gan quy trinh dang ky tu `DanhMucQuyTrinhSoanThaos`.
4. Gan buoc dau tien va trang thai dau tien tu cau hinh.
5. Ghi timeline hanh dong `TAO_MOI`.

### 5.2. Trinh phe duyet

1. Lay ho so hien tai.
2. Lay `BuocHienTaiId` va `TrangThaiHoSoId`.
3. Tim cau hinh trong `DangKyCauHinhChuyenTrangThais` theo hanh dong `TRINH_PHE_DUYET`.
4. Neu hop le thi cap nhat buoc/trang thai tiep theo.
5. Ghi timeline.

### 5.3. Phe duyet hoac tra lai

1. Don vi phe duyet chon hanh dong `PHE_DUYET`, `TRA_LAI` hoac `KHONG_PHE_DUYET`.
2. Service kiem tra cau hinh chuyen trang thai.
3. Neu `TRA_LAI` va cau hinh yeu cau ly do, bat buoc nhap ly do.
4. Ghi ket qua phe duyet neu co.
5. Cap nhat buoc/trang thai.
6. Ghi timeline.

### 5.4. Cap nhat ket qua va hoan thanh

1. Don vi soan thao cap nhat so van ban, ngay van ban, file quyet dinh/danh muc.
2. Service ghi file va ket qua.
3. Thuc hien hanh dong `CAP_NHAT_KET_QUA`.
4. Thuc hien hanh dong `HOAN_THANH` khi du dieu kien.

### 5.5. Khoi tao quy trinh xay dung van ban

1. Chi cho phep khi ho so da hoan thanh va ket qua la dong y.
2. Dua vao `LoaiVanBanId` de chon quy trinh tiep theo:
   - Quyet dinh UBND -> quy trinh `XD_QD_UBND_TINH`.
   - Nghi quyet HDND -> quy trinh `XD_NQ_HDND_TINH`.
3. Tao ho so xay dung van ban o service xay dung van ban.
4. Ghi lien ket vao `DangKyXayDungVanBanLienKetQuyTrinhs`.
5. Cap nhat `DaKhoiTaoQuyTrinhXayDung = true`.
6. Ghi timeline hanh dong `KHOI_TAO_QUY_TRINH_XAY_DUNG`.

## 6. API de xuat

| Method | Endpoint | Muc dich |
|---|---|---|
| GET | /api/dang-ky-xay-dung-van-ban | Danh sach ho so |
| GET | /api/dang-ky-xay-dung-van-ban/{id} | Chi tiet ho so |
| POST | /api/dang-ky-xay-dung-van-ban | Tao moi ho so |
| PUT | /api/dang-ky-xay-dung-van-ban/{id} | Cap nhat ho so |
| POST | /api/dang-ky-xay-dung-van-ban/{id}/files | Tai file |
| DELETE | /api/dang-ky-xay-dung-van-ban/{id}/files/{fileId} | Xoa file |
| GET | /api/dang-ky-xay-dung-van-ban/{id}/timeline | Timeline ho so |
| GET | /api/dang-ky-xay-dung-van-ban/{id}/hanh-dong-kha-dung | Lay cac hanh dong duoc phep |
| POST | /api/dang-ky-xay-dung-van-ban/{id}/xu-ly | Thuc hien hanh dong xu ly |
| POST | /api/dang-ky-xay-dung-van-ban/{id}/ket-qua-phe-duyet | Cap nhat ket qua phe duyet |
| POST | /api/dang-ky-xay-dung-van-ban/{id}/khoi-tao-quy-trinh-xay-dung | Khoi tao quy trinh tiep theo |

## 7. Cac DTO chinh

### 7.1. TaoDangKyXayDungVanBanRequest

- TenHoSo
- TenVanBanDuKien
- LoaiVanBanId
- DonViSoanThaoId
- DonViPheDuyetId
- NamDangKy
- CanCuDeXuat
- SuCanThiet
- NoiDungChinhSach
- DuKienThoiGianTrinh

### 7.2. XuLyDangKyXayDungVanBanRequest

- HanhDongId
- NoiDungXuLy
- LyDoTraLai
- FileIds

### 7.3. CapNhatKetQuaPheDuyetRequest

- KetQua
- SoVanBan
- NgayVanBan
- CoQuanPheDuyetId
- NguoiKy
- ChucVuNguoiKy
- NoiDungKetQua
- FileKetQuaId

## 8. Cac buoc thuc hien code

### Buoc 1. Tao skeleton service

- Tao thu muc service `BE/services/DangKyXayDungVanBanService`.
- Tao cac project:
  - Domain
  - Application
  - Infrastructure
  - API
- Cau hinh solution, references va dependency injection theo pattern cac service hien co.

### Buoc 2. Tao Domain Entities

Tao cac entity:

- `DangKyXayDungVanBanEntity`
- `DangKyXayDungVanBanFileEntity`
- `DangKyXayDungVanBanLichSuXuLyEntity`
- `DangKyXayDungVanBanKetQuaPheDuyetEntity`
- `DangKyXayDungVanBanLienKetQuyTrinhEntity`
- `DangKyTrangThaiHoSoEntity`
- `DangKyHanhDongXuLyEntity`
- `DangKyCauHinhChuyenTrangThaiEntity`

### Buoc 3. Tao Persistence Entities va DbContext

- Tao cac bang persistence tuong ung.
- Cau hinh index:
  - `MaHoSo`
  - `DangKyXayDungVanBanId`
  - `TrangThaiHoSoId`
  - `BuocHienTaiId`
  - `CreatedAt`
- Cau hinh soft delete neu service hien co dang dung.

### Buoc 4. Tao migration

- Tao migration khoi tao bang.
- Kiem tra SQL migration.
- Build service.
- Cap nhat database.

### Buoc 5. Seed du lieu cau hinh ban dau

Seed:

- Trang thai ho so.
- Hanh dong xu ly.
- Cau hinh chuyen trang thai.

Du lieu danh muc quy trinh, buoc, chuyen buoc van nam o `DanhMucService`.

### Buoc 6. Tao Application Services

Can co cac service ung dung:

- Tao ho so.
- Cap nhat ho so.
- Lay danh sach ho so.
- Lay chi tiet ho so.
- Lay timeline.
- Lay hanh dong kha dung.
- Xu ly chuyen trang thai.
- Cap nhat ket qua phe duyet.
- Khoi tao quy trinh xay dung van ban.

### Buoc 7. Tao Controller/API

Tao API controller theo cac endpoint da de xuat.

Controller chi dieu phoi request/response, khong chua logic nghiep vu phuc tap.

### Buoc 8. Tich hop service khac

Can tich hop logic/doc:

- `DanhMucService`: lay quy trinh, buoc, chuyen buoc, loai van ban, don vi.
- `QuanTriHeThongService`: lay nguoi dung, quyen, don vi cua nguoi dung.
- Service xay dung van ban: tao ho so xay dung sau khi dang ky hoan thanh.

### Buoc 9. Xay dung test

Test can co:

- Tao moi ho so.
- Cap nhat ho so.
- Trinh phe duyet hop le.
- Khong cho phep hanh dong sai trang thai.
- Tra lai bat buoc ly do.
- Phe duyet ghi ket qua.
- Timeline dung thu tu.
- Hoan thanh ho so.
- Khoi tao quy trinh xay dung van ban chi khi ho so da hoan thanh.

### Buoc 10. Ban giao

Ban giao gom:

- Source code service.
- Migration SQL.
- Tai lieu API.
- File seed du lieu cau hinh.
- Tai lieu mo ta bang du lieu.
- Huong dan test va kich ban nghiem thu.

## 9. Dieu kien nghiem thu

- Tao duoc ho so dang ky.
- Xem duoc danh sach va chi tiet ho so.
- Xem duoc ho so dang o buoc nao.
- Xem duoc timeline tung ho so.
- Trang thai va hanh dong lay tu bang cau hinh, khong hardcode trong code xu ly.
- Don vi soan thao thuc hien duoc tao, cap nhat, trinh, cap nhat ket qua.
- Don vi phe duyet thuc hien duoc phe duyet, tra lai, khong phe duyet.
- Ho so da hoan thanh co the khoi tao/tham chieu sang quy trinh xay dung van ban.
- Build thanh cong.
- Migration tao bang thanh cong.

## 10. Ghi chu kien truc microservice

- `DangKyXayDungVanBanService` co database/schema rieng ve mat logical ownership.
- Trong truong hop tam thoi dung chung database, khong nen tao foreign key vat ly sang bang service khac.
- Cac tham chieu lien service luu bang Id va snapshot ten tai thoi diem xu ly.
- Giao tiep voi service khac nen thong qua API/client hoac message/event khi can mo rong.
