# Ke hoach xay dung XayDungVanBanService

## 1. Pham vi

`XayDungVanBanService` quan ly quy trinh xay dung van ban QPPL cap tinh sau khi ho so dang ky/de xuat danh muc da duoc phe duyet va chuyen sang giai doan xay dung van ban.

Pham vi nghiep vu gom 2 luong chinh:

- Xay dung Quyết định UBND tỉnh.
- Xay dung Nghị quyết HĐND tỉnh.

Service phu trach:

- Tao va quan ly ho so xay dung du thao van ban.
- Quan ly file dinh kem, y kien gop y, ho so trinh tham dinh, ket qua tham dinh.
- Quan ly ho so trinh UBND/HĐND, y kien thanh vien UBND, tham tra HĐND neu la Nghị quyết.
- Cap nhat ket qua van ban duoc ban hanh.
- Ghi timeline xu ly va lich su chuyen trang thai.

Service khong phu trach:

- Quan ly danh muc loai van ban, don vi, can bo, quy trinh, buoc quy trinh.
- Quan ly nguoi dung, nhom quyen, RoleAction, Permission.
- Xu ly cac buoc dien ra ngoai he thong neu tai lieu nghiep vu da xac dinh chi cap nhat ket qua len he thong.

Danh muc dung chung nam tai `DanhMucService`.
Quyen chuc nang nam tai `QuanTriHeThongService`.

## 2. Nguyen tac thiet ke

### 2.1. Phuong an gop

Dung chung 1 service, 1 bo bang va 1 bo controller theo buoc nghiep vu cho ca Quyết định UBND va Nghị quyết HĐND.

Khong tach DB rieng cho tung loai van ban.

Ho so phan biet bang:

- `DanhMucVanBanId`: loai van ban theo bang `dm.DanhMucVanBans`.
- `QuyTrinhSoanThaoId`: quy trinh xu ly ap dung cho ho so.
- `BuocHienTaiId`: buoc hien tai trong quy trinh.
- `TrangThaiHoSoId`: trang thai nghiep vu hien tai.

Khong tao them truong/bang `LoaiVanBan` rieng neu trung nghia voi `DanhMucVanBans`.

### 2.2. Quan he giua DanhMucVanBan va QuyTrinhSoanThao

`DanhMucVanBans` da phan biet loai van ban qua cac truong:

- `TenLoaiVanBan`
- `CapChinhQuyen`
- `ChuTheBanHanh`
- `KyHieuMau`

`DanhMucQuyTrinhSoanThaos` da co:

- `DanhMucVanBanId`
- `DanhMucVanBanIds`
- `LoaiQuyTrinh`

Khi tao ho so, can validate:

- `DanhMucVanBanId` cua ho so phai nam trong `DanhMucQuyTrinhSoanThao.DanhMucVanBanIds`, hoac bang `DanhMucQuyTrinhSoanThao.DanhMucVanBanId`.
- `QuyTrinhSoanThaoId` phai dang kich hoat.
- Quy trinh duoc chon phai co `LoaiQuyTrinh` phu hop voi nghiep vu xay dung van ban.

## 3. DB de xuat

### 3.1. Bang chinh

`HoSoXayDungVanBans`

- `Id`
- `MaHoSo`
- `TenHoSo`
- `TenDuThaoVanBan`
- `DanhMucVanBanId`
- `QuyTrinhSoanThaoId`
- `BuocHienTaiId`
- `TrangThaiHoSoId`
- `DonViChuTriSoanThaoId`
- `NguoiPhuTrachId`
- `NamXayDung`
- `ThoiGianDuKienBatDau`
- `ThoiGianDuKienHoanThanh`
- `MoTa`
- `HoSoDangKyXayDungVanBanId`
- `IsDeleted`
- `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`

### 3.2. Bang phu

`HoSoXayDungVanBanDonViPhoiHops`

- Don vi phoi hop cua ho so.

`HoSoXayDungVanBanFiles`

- File theo ho so va loai file nghiep vu.
- Can luu `LoaiFile`, `TenFile`, `DuongDanFile`, `DungLuong`, `MimeType`, `MoTa`.

`HoSoXayDungVanBanLichSuXuLys`

- Timeline xu ly.
- Luu buoc/trang thai truoc sau, hanh dong, nguoi xu ly, don vi xu ly, noi dung, ly do.

`HoSoXayDungVanBanYKienGopYs`

- Y kien gop y trong giai doan soan thao.
- Co the dung cho y kien gop y noi bo hoac y kien tong hop.

`HoSoXayDungVanBanThamDinhs`

- Ho so va ket qua tham dinh cua So Tu phap.
- Luu bao cao tham dinh, ket qua dat/chua dat, yeu cau bo sung neu co.

`HoSoXayDungVanBanTrinhPheDuyets`

- Ho so trinh UBND tinh/HĐND tinh.
- Luu ngay trinh, noi dung trinh, file trinh, trang thai trinh.

`HoSoXayDungVanBanYKienThanhViens`

- Tong hop y kien thanh vien UBND tinh.
- Luu ket qua dong y/khong dong y, noi dung giai trinh, file tong hop.

`HoSoXayDungVanBanThamTraHdnds`

- Chi ap dung khi `DanhMucVanBanId` la Nghị quyết HĐND hoac quy trinh co buoc tham tra HĐND.
- Luu bao cao tham tra va y kien thao luan HĐND.

`HoSoXayDungVanBanKetQuaBanHanhs`

- Ket qua van ban duoc ban hanh/thong qua.
- Luu so van ban, ngay ban hanh, co quan ban hanh, nguoi ky, chuc vu, file ket qua.

### 3.3. Bang cau hinh quy trinh trong service

Neu cac bang danh muc hien co chua du de quan ly trang thai nghiep vu rieng cua service, bo sung:

- `XayDungTrangThaiHoSos`
- `XayDungHanhDongXuLys`
- `XayDungCauHinhChuyenTrangThais`

Neu co the tai su dung `DanhMucBuocQuyTrinhs` va `DanhMucChuyenBuocQuyTrinhs`, uu tien tai su dung de tranh lap cau hinh.

### 3.4. Index can co

- `DanhMucVanBanId`, `NamXayDung`
- `QuyTrinhSoanThaoId`, `BuocHienTaiId`, `TrangThaiHoSoId`
- `DonViChuTriSoanThaoId`, `TrangThaiHoSoId`, `CreatedAt`
- `NguoiPhuTrachId`, `CreatedAt`
- `CreatedBy`, `CreatedAt`
- `HoSoDangKyXayDungVanBanId` neu co lien ket tu service dang ky.

## 4. Controller de xuat theo RoleAction Detail

Theo quy tac PM: moi `RoleAction` co `PhanLoai = Detail` phai co 1 Form va 1 Controller rieng.

### 4.1. Danh sach controller

| RoleAction | Controller | Route |
|---|---|---|
| `VanBanQPPL.XayDungVanBan.DanhSach` | `XayDungVanBanDanhSachController` | `/api/xay-dung-van-ban/danh-sach` |
| `VanBanQPPL.XayDungVanBan.SoanThao` | `XayDungVanBanSoanThaoController` | `/api/xay-dung-van-ban/soan-thao` |
| `VanBanQPPL.XayDungVanBan.TrinhThamDinh` | `XayDungVanBanTrinhThamDinhController` | `/api/xay-dung-van-ban/trinh-tham-dinh` |
| `VanBanQPPL.XayDungVanBan.ThamDinh` | `XayDungVanBanThamDinhController` | `/api/xay-dung-van-ban/tham-dinh` |
| `VanBanQPPL.XayDungVanBan.TrinhPheDuyet` | `XayDungVanBanTrinhPheDuyetController` | `/api/xay-dung-van-ban/trinh-phe-duyet` |
| `VanBanQPPL.XayDungVanBan.YKienUbnd` | `XayDungVanBanYKienUbndController` | `/api/xay-dung-van-ban/y-kien-ubnd` |
| `VanBanQPPL.XayDungVanBan.ThamTraHdnd` | `XayDungVanBanThamTraHdndController` | `/api/xay-dung-van-ban/tham-tra-hdnd` |
| `VanBanQPPL.XayDungVanBan.BanHanh` | `XayDungVanBanBanHanhController` | `/api/xay-dung-van-ban/ban-hanh` |

### 4.2. Phan bo API chinh

`XayDungVanBanDanhSachController`

- Danh sach ho so.
- Tim kiem/loc theo `DanhMucVanBanId`, nam, don vi, buoc, trang thai.
- Xem chi tiet tong quan.
- Ket xuat danh sach neu can.

`XayDungVanBanSoanThaoController`

- Tao ho so xay dung van ban.
- Sua thong tin ho so trong giai doan soan thao.
- Xoa mem ho so neu trang thai cho phep.
- Quan ly file soan thao.
- Quan ly y kien gop y.

`XayDungVanBanTrinhThamDinhController`

- Tao ho so trinh tham dinh ke thua tu ho so soan thao.
- Bo sung to trinh va file trinh tham dinh.
- Gui ho so sang So Tu phap.
- Cap nhat, bo sung, gui lai ho so neu bi yeu cau bo sung.

`XayDungVanBanThamDinhController`

- So Tu phap tiep nhan ho so.
- Tao/cap nhat ho so tham dinh.
- Dinh kem bao cao ket qua tham dinh.
- Gui ket qua tham dinh cho don vi soan thao.
- Yeu cau bo sung ho so neu chua dat.

`XayDungVanBanTrinhPheDuyetController`

- Tao/cap nhat ho so trinh UBND tinh/HĐND tinh.
- Luu tru ho so da trinh.
- Theo doi trang thai trinh phe duyet/cho y kien.

`XayDungVanBanYKienUbndController`

- Cap nhat tong hop y kien thanh vien UBND tinh.
- Cap nhat giai trinh, bo sung ho so khi co y kien khac nhau.

`XayDungVanBanThamTraHdndController`

- Tao/cap nhat ho so trinh HĐND tham tra du thao Nghị quyết.
- Cap nhat bao cao ket qua tham tra.
- Cap nhat tong hop y kien thao luan HĐND.

`XayDungVanBanBanHanhController`

- Cap nhat van ban duoc ban hanh/thong qua.
- Luu file ket qua ban hanh.
- Hoan thanh quy trinh.

## 5. RoleActions de xuat

### 5.1. Group

| PhanLoai | Role | Title | Controller | Action |
|---|---|---|---|---|
| `Group` | `VanBanQPPL.XayDungVanBan` | Xay dung van ban QPPL | null | null |

### 5.2. Detail

| PhanLoai | Role | Title | Controller | Action |
|---|---|---|---|---|
| `Detail` | `VanBanQPPL.XayDungVanBan.DanhSach` | Danh sach xay dung van ban | `XayDungVanBanDanhSach` | `Index` |
| `Detail` | `VanBanQPPL.XayDungVanBan.SoanThao` | To chuc soan thao va y kien gop y | `XayDungVanBanSoanThao` | `Index` |
| `Detail` | `VanBanQPPL.XayDungVanBan.TrinhThamDinh` | Trinh tham dinh du thao van ban | `XayDungVanBanTrinhThamDinh` | `Index` |
| `Detail` | `VanBanQPPL.XayDungVanBan.ThamDinh` | Tham dinh du thao van ban | `XayDungVanBanThamDinh` | `Index` |
| `Detail` | `VanBanQPPL.XayDungVanBan.TrinhPheDuyet` | Trinh phe duyet/cho y kien du thao van ban | `XayDungVanBanTrinhPheDuyet` | `Index` |
| `Detail` | `VanBanQPPL.XayDungVanBan.YKienUbnd` | Y kien thanh vien UBND tinh | `XayDungVanBanYKienUbnd` | `Index` |
| `Detail` | `VanBanQPPL.XayDungVanBan.ThamTraHdnd` | Tham tra HĐND du thao Nghị quyết | `XayDungVanBanThamTraHdnd` | `Index` |
| `Detail` | `VanBanQPPL.XayDungVanBan.BanHanh` | Ban hanh/thong qua van ban | `XayDungVanBanBanHanh` | `Index` |

### 5.3. Mapping quyen

| Quyen | Y nghia |
|---|---|
| `Index` | Xem danh sach, chi tiet, file, timeline |
| `Create` | Tao ho so/buoc nghiep vu |
| `Edit` | Cap nhat ho so, file, noi dung buoc |
| `Delete` | Xoa mem ho so/buoc/file |
| `Approve` | Gui, tiep nhan, tra lai, yeu cau bo sung, xac nhan hoan thanh buoc |
| `Public` | Ket xuat, cong khai, ban hanh neu can tach voi `Approve` |

## 6. Luong quy trinh

### 6.1. Quyết định UBND

1. Soan thao, lay y kien gop y.
2. Gui ho so tham dinh.
3. So Tu phap tham dinh.
4. Trinh UBND tinh xem xet/thong qua.
5. Cap nhat y kien thanh vien UBND tinh.
6. Cap nhat Quyết định duoc ban hanh.

### 6.2. Nghị quyết HĐND

1. Soan thao, lay y kien gop y.
2. Gui ho so tham dinh.
3. So Tu phap tham dinh.
4. Trinh UBND tinh xem xet/cho y kien.
5. Cap nhat y kien thanh vien UBND tinh.
6. Trinh HĐND tinh tham tra, cap nhat ket qua tham tra/y kien thao luan.
7. Cap nhat Nghị quyết duoc thong qua/ban hanh.

## 7. Phan quyen du lieu

Phan quyen gom 2 lop:

- Quyen chuc nang tu `QuanTriHeThongService`.
- Quyen du lieu trong `XayDungVanBanService`.

Quyen du lieu de xuat:

- Don vi chu tri soan thao duoc tao/sua ho so cua minh theo trang thai cho phep.
- Nguoi phu trach duoc xem va cap nhat cac phan duoc giao.
- So Tu phap duoc tiep nhan/tham dinh ho so gui den.
- Don vi UBND/HĐND co tham quyen duoc xem/cap nhat cac buoc lien quan neu nghiep vu dua len he thong.
- SSA/Admin duoc xem va thao tac toan bo.

## 8. Ke hoach thuc hien

### Giai doan 1. Tao nen service

- Tao project `BE/services/XayDungVanBanService`.
- Them project vao solution.
- Cau hinh `DbContext`, connection string va migration.
- Them `ICurrentUserContext`.
- Them cau truc thu muc Application/Domain/Infrastructure/Controllers.

### Giai doan 2. Thiet ke DB va migration

- Tao entity bang chinh `HoSoXayDungVanBan`.
- Tao cac bang file, lich su, don vi phoi hop.
- Tao cac bang nghiep vu: y kien, tham dinh, trinh phe duyet, y kien UBND, tham tra HĐND, ket qua ban hanh.
- Them index.
- Tao migration initial.

### Giai doan 3. API nen

- DTO danh sach/filter/paging.
- DTO chi tiet ho so.
- API danh sach/chi tiet.
- API tao/sua/xoa mem.
- API file.
- API timeline.
- API hanh dong kha dung.

### Giai doan 4. Nghiep vu chuyen buoc

- Seed/cau hinh trang thai, hanh dong, chuyen trang thai.
- Kiem tra trang thai truoc khi cho thao tac.
- Kiem tra `DanhMucVanBanId` hop le voi `QuyTrinhSoanThaoId`.
- Ghi timeline moi lan chuyen buoc.

### Giai doan 5. Tich hop phan quyen

- Seed `RoleActions` cho `XayDungVanBanService`.
- Tao client noi bo hoi quyen tu `QuanTriHeThongService`.
- Ap quyen `Index`, `Create`, `Edit`, `Delete`, `Approve`, `Public`.
- Ket hop quyen chuc nang voi quyen du lieu.

### Giai doan 6. Frontend

- Form danh sach.
- Form soan thao/gop y.
- Form trinh tham dinh.
- Form tham dinh.
- Form trinh phe duyet.
- Form y kien UBND.
- Form tham tra HĐND.
- Form ban hanh.

### Giai doan 7. Kiem thu

- Tao ho so Quyết định UBND.
- Tao ho so Nghị quyết HĐND.
- Kiem tra quy trinh Quyết định khong di qua buoc tham tra HĐND.
- Kiem tra quy trinh Nghị quyết co buoc tham tra HĐND.
- Don vi chu tri chi sua ho so cua minh theo trang thai cho phep.
- So Tu phap chi tham dinh ho so gui den.
- User thieu quyen chuc nang khong thao tac duoc.
- SSA/Admin thao tac toan bo.

## 9. Ghi chu thiet ke

- Uu tien dung `DanhMucVanBanId` thay vi `LoaiVanBanId` de dong bo voi `DanhMucVanBans`.
- `QuyTrinhSoanThaoId` khong thay the `DanhMucVanBanId`; no chi xac dinh quy trinh xu ly.
- Neu mot quy trinh ap dung cho nhieu loai van ban, `DanhMucVanBanId` tren ho so la bat buoc de biet ho so thuc te dang xay dung loai van ban nao.
- Khong hardcode id danh muc/quy trinh trong service neu co the tra theo ma danh muc/ma quy trinh.
- Cac route/controller phai khop voi `RoleAction.Controller` de phan quyen chuc nang.
