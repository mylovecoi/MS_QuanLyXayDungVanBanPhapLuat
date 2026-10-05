# Ke hoach xay dung chuc nang DangKyXayDungVanBanService

## 1. Pham vi

Chuc nang quan ly quy trinh De xuat danh muc / Dang ky xay dung van ban QPPL.

Service phu trach:

- Tao, cap nhat, xoa mem ho so dang ky.
- Quan ly danh sach, chi tiet, file dinh kem, timeline.
- Trinh phe duyet, phe duyet, tra lai, khong phe duyet, cap nhat ket qua, hoan thanh.
- Lien ket sang quy trinh xay dung van ban sau khi ho so dang ky hoan thanh.

Service khong phu trach:

- Quan ly nguoi dung, nhom quyen, quyen chuc nang.
- Quan ly danh muc don vi, loai van ban, quy trinh, buoc quy trinh.

Quyen nguoi dung nam tai `QuanTriHeThongService`.

## 2. Nguyen tac phan quyen

Phan quyen gom 2 lop.

Lop quyen chuc nang lay tu `QuanTriHeThongService`:

- `Users` gan voi `GroupPermision`.
- `GroupPermision` co cac `Permission`.
- `Permission` gan voi `RoleAction`.
- Cac co quyen chinh: `Index`, `Create`, `Edit`, `Delete`, `Approve`, `Public`.
- Quy tac PM chung: moi `RoleAction` co phan loai `Detail` phai xay dung 1 Form va 1 Controller rieng de thiet lap phan quyen. Chi tiet xem `TaiLieu/QuyTacXayDungPhanMem.md`.

Lop quyen du lieu nam trong `DangKyXayDungVanBanService`:

- Don vi soan thao: `User.DanhMucDonViId == DangKyXayDungVanBan.DonViSoanThaoId`.
- Don vi phe duyet: `User.DanhMucDonViId == DangKyXayDungVanBan.DonViPheDuyetId`.
- Nguoi tao ho so: `DangKyXayDungVanBan.CreatedBy == User.Id`.
- SSA/Admin duoc xem va thao tac toan bo.

## 3. Ma tran nguoi dung

| Nhom nguoi dung | Dieu kien du lieu | Thao tac |
|---|---|---|
| CVCQST | Thuoc don vi soan thao cua ho so | Tao, sua khi dang soan thao/bi tra lai, tai file, trinh phe duyet, cap nhat ket qua, hoan thanh, khoi tao quy trinh tiep theo |
| Nguoi tao ho so | `CreatedBy == User.Id` | Xem/sua ho so cua minh theo trang thai cho phep |
| CVSTP | Co role/permission phoi hop kiem tra | Xem, kiem tra, gop y ho so da trinh neu nghiep vu yeu cau |
| Co quan phe duyet | Thuoc don vi phe duyet cua ho so | Xem ho so trinh den, phe duyet, tra lai, khong phe duyet |
| SSA/Admin | `User.SSA == true` hoac permission quan tri | Toan quyen |

## 4. DB hien tai

Da dap ung loi nghiep vu:

- `DangKyXayDungVanBans`
- `DangKyXayDungVanBanFiles`
- `DangKyXayDungVanBanLichSuXuLys`
- `DangKyXayDungVanBanKetQuaPheDuyets`
- `DangKyXayDungVanBanLienKetQuyTrinhs`
- `DangKyTrangThaiHoSos`
- `DangKyHanhDongXuLys`
- `DangKyCauHinhChuyenTrangThais`

Mot van ban co the dang ky nhieu lan, vi vay khong tao unique theo ten van ban, loai van ban, nam dang ky hoac don vi soan thao. Chi can unique `MaHoSo`.

Can bo sung:

- Index theo `NamDangKy`, `LoaiVanBanId`.
- Composite index theo `DonViSoanThaoId`, `TrangThaiHoSoId`, `CreatedAt`.
- Composite index theo `DonViPheDuyetId`, `TrangThaiHoSoId`, `CreatedAt`.
- Index theo `CreatedBy`, `CreatedAt` de phuc vu man hinh "ho so cua toi".
- Chuan hoa `CreatedBy`, `UpdatedBy` luu `UserId` dang string hoac bo sung cot `NguoiTaoId`, `NguoiCapNhatId` trong dot nang cap sau.

## 5. Man hinh chuc nang

### 5.1. Danh sach ho so

- Tim kiem theo ma ho so, ten ho so, ten van ban.
- Loc theo nam dang ky, loai van ban, trang thai, buoc, don vi soan thao, don vi phe duyet.
- Loc theo pham vi du lieu cua user: tat ca, don vi soan thao, don vi phe duyet, cua toi.
- Xem chi tiet, sua, xoa mem, trinh phe duyet, xuat danh sach.

### 5.2. Tao/sua ho so

- Ten ho so.
- Ten van ban du kien.
- Loai van ban.
- Don vi soan thao.
- Don vi phe duyet.
- Nam dang ky.
- Can cu de xuat.
- Su can thiet.
- Noi dung chinh sach.
- Du kien thoi gian trinh.

### 5.3. Chi tiet ho so

- Thong tin chung.
- Trang thai/buoc hien tai.
- File dinh kem.
- Timeline xu ly.
- Hanh dong kha dung theo trang thai va quyen user.

### 5.4. Ket qua phe duyet

- Ket qua.
- So van ban.
- Ngay van ban.
- Co quan phe duyet.
- Nguoi ky, chuc vu.
- Noi dung ket qua.
- File ket qua.

## 6. Ke hoach thuc hien

### Giai doan 1. Hoan thien backend nen

- Them DTO filter/paging danh sach.
- Them DTO ket qua phan trang.
- Them API danh sach co filter nghiep vu va paging.
- Them API xoa mem ho so.
- Them index DB phuc vu loc theo don vi, trang thai, nam, loai van ban, nguoi tao.

### Giai doan 2. Tich hop phan quyen

- Hoan thien `PermissionChecker` trong `QuanTriHeThongService`.
- Them `RoleAction` cho module dang ky xay dung van ban.
- Them client/noi bo de `DangKyXayDungVanBanService` hoi quyen user.
- Lay current user tu token/header noi bo thay vi nhan user id tu query.

### Giai doan 3. Hoan thien nghiep vu

- Kiem tra trang thai truoc khi cho sua/xoa/trinh.
- Kiem tra don vi soan thao/don vi phe duyet theo user.
- Bo sung bang y kien phoi hop neu So Tu phap can nhap y kien tren he thong.
- Hoan thien dieu kien cap nhat ket qua va hoan thanh.

### Giai doan 4. Xay dung frontend

- Man hinh danh sach.
- Man hinh tao/sua.
- Man hinh chi tiet.
- Tab file dinh kem.
- Tab timeline.
- Tab ket qua phe duyet.
- Hien thi nut thao tac theo `hanh-dong-kha-dung` va permission.

### Giai doan 5. Kiem thu

- CVCQST tao, sua, trinh ho so.
- Mot van ban duoc dang ky nhieu lan.
- Don vi phe duyet chi thay ho so gui den minh.
- Tra lai/khong phe duyet bat buoc ly do.
- Cap nhat ket qua ghi dung file va timeline.
- User khong co quyen khong xem/sua/xu ly duoc.
- SSA/Admin thao tac toan bo.

## 7. Tien do thuc hien

Da thuc hien ngay 2026-10-05:

- Them DTO filter/paging danh sach ho so.
- Them DTO ket qua phan trang.
- Cap nhat API danh sach sang co paging va filter nghiep vu.
- Them API xoa mem ho so.
- Them service xoa mem ho so.
- Them index phuc vu danh sach, loc theo nam, loai van ban, don vi, trang thai va nguoi tao.
- Them migration `AddDangKyListIndexes`.
- Cap nhat EF model snapshot.
- Build thanh cong `DangKyXayDungVanBanService`.
- Them `ICurrentUserContext` vao `DangKyXayDungVanBanService`.
- Doc current user tu claim/header noi bo `X-User-Id`, `X-Username`, `X-Don-Vi-Id`, `X-Group-Permission-Id`, `X-Is-SSA`.
- Ap dung data-scope cho danh sach, chi tiet, timeline, file va hanh dong kha dung.
- Bo phu thuoc vao `nguoiXoaId`, `nguoiTaiLenId` tu query/form; audit lay theo current user server-side.
- Kiem tra don vi soan thao/don vi phe duyet/trang thai truoc khi sua, xoa, tai file, xu ly, cap nhat ket qua va khoi tao quy trinh tiep theo.
- Build thanh cong `DangKyXayDungVanBanService` sau khi tich hop current-user context.
- Tach controller theo quy tac PM moi `RoleAction` phan loai `Detail` co 1 Controller rieng:
  - `DangKyXayDungVanBanDanhSachController`
  - `DangKyXayDungVanBanHoSoController`
  - `DangKyXayDungVanBanKetQuaController`
- Ra soat va bo hardcode `NguoiTaiLenId`/`Guid.Empty` gia trong upload file; lich su khoi tao quy trinh dung `DangKySeedIds.HanhDong.KhoiTaoQuyTrinhXayDung`.
- Cap nhat script seed `RoleActions` de lay `RoleGroupId` cha theo `Role = VanBanQPPL`, khong tro cung GUID cha.

Ghi chu bao mat:

- Khong truyen `UserId`, `DonViId`, `IsSsa` qua query string de loc du lieu vi day la du lieu co the bi gia mao tu client.
- Data-scope hien lay tu current-user context server-side/header noi bo. Buoc tiep theo la noi client noi bo sang `QuanTriHeThongService` de kiem tra lop quyen chuc nang `Index`, `Create`, `Edit`, `Delete`, `Approve`, `Public`.
