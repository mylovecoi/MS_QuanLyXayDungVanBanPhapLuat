# Thiet ke API XayDungVanBanSoanThaoController

## 1. Pham vi

Controller quan ly giai doan soan thao du thao van ban, nhap ket qua lay y kien ma don vi chu tri da thuc hien ben ngoai he thong, va chuan bi ho so trinh tham dinh.

He thong khong gui yeu cau lay y kien va khong quan ly file phan hoi rieng cua tung don vi. Nguoi dung nhap ket qua y kien tung don vi, sau do dinh kem mot file tong hop y kien cho ho so soan thao.

Route goc: `/api/xay-dung-van-ban/soan-thao`.

RoleAction: `VanBanQPPL.XayDungVanBan.SoanThao`.

## 2. API

| Giai doan | Method | Endpoint | Quyen | Trang thai |
|---|---|---|---|---|
| Ho so | POST | `/` | Create | Tao ho so va bo ho so SoanThao lan 1 | Lam trong lo hien tai |
| Ho so | GET | `/{hoSoId}` | Index | Xem chi tiet soan thao | Lam trong lo hien tai |
| Ho so | PUT | `/{hoSoId}` | Edit | Cap nhat ho so va noi dung soan thao | Lam trong lo hien tai |
| Ho so | DELETE | `/{hoSoId}` | Delete | Xoa mem ho so con o buoc soan thao | Lam trong lo hien tai |
| Y kien don vi | GET | `/{hoSoId}/y-kien-don-vi` | Index | Danh sach y kien da nhan tu don vi | Da thuc hien |
| Y kien don vi | POST | `/{hoSoId}/y-kien-don-vi` | Create | Nhap y kien cua mot don vi | Da thuc hien |
| Y kien don vi | PUT | `/{hoSoId}/y-kien-don-vi/{id}` | Edit | Cap nhat y kien da nhap | Da thuc hien |
| Y kien don vi | DELETE | `/{hoSoId}/y-kien-don-vi/{id}` | Delete | Xoa mem y kien nhap sai | Da thuc hien |
| Tong hop | GET | `/{hoSoId}/tong-hop-y-kien` | Index | Xem noi dung tong hop, tiep thu va giai trinh | Da thuc hien |
| Tong hop | PUT | `/{hoSoId}/tong-hop-y-kien` | Edit | Cap nhat tong hop, tiep thu va giai trinh | Da thuc hien |
| Tai lieu | GET | `/{hoSoId}/tai-lieu` | Index | Danh sach tai lieu soan thao va phien ban | Da thuc hien |
| Tai lieu | POST | `/{hoSoId}/tai-lieu` | Create | Tai tai lieu soan thao, tao phien ban moi | Da thuc hien |
| Tai lieu | DELETE | `/{hoSoId}/tai-lieu/{fileId}` | Delete | Xoa mem tai lieu chua duoc trinh | Da thuc hien |
| File tong hop | GET | `/{hoSoId}/file-tong-hop-y-kien` | Index | Lay file tong hop y kien hien hanh va lich su phien ban | Da thuc hien |
| File tong hop | POST | `/{hoSoId}/file-tong-hop-y-kien` | Create | Tai file tong hop y kien, tao phien ban moi | Da thuc hien |
| Trinh tham dinh | GET | `/{hoSoId}/kiem-tra-truoc-trinh-tham-dinh` | Index | Tra cac dieu kien chua dat truoc khi trinh | Da thuc hien |
| Trinh tham dinh | POST | `/{hoSoId}/trinh-tham-dinh` | Approve | Da chuyen sang XayDungVanBanTrinhThamDinhController de tao va gui ho so theo tung buoc | Khong ap dung |

## 3. Du lieu y kien don vi

Can bo sung bang `HoSoXayDungVanBanYKienDonVis`:

| Cot | Ghi chu |
|---|---|
| Id | Khoa chinh |
| HoSoXayDungVanBanId | Ho so soan thao |
| DonViGopYId | Don vi da gui y kien ben ngoai he thong |
| NgayNhan | Ngay don vi chu tri nhan duoc y kien |
| KetQua | DongY, KhongDongY, YKienKhac, KhongPhanHoi |
| NoiDungYKien | Noi dung da nhap tu ket qua ben ngoai he thong |
| IsDeleted, audit fields | Xoa mem va truy vet |

Mot don vi chi co mot y kien hien hanh tren mot ho so. Neu co bo sung, cap nhat ban ghi va ghi timeline; khong luu file rieng theo tung don vi.

File tong hop y kien dung `HoSoXayDungVanBanFiles`, phan biet bang `LoaiTaiLieuId` cua danh muc tai lieu tong hop y kien. Id loai tai lieu nay duoc luu tai `HoSoXayDungVanBanSoanThaos.LoaiTaiLieuTongHopYKienId`, file duoc lien ket voi bo ho so SoanThao va co version.

## 4. Quy tac xu ly

- Moi API kiem tra quyen qua QuanTriHeThongService voi controller `XayDungVanBanSoanThao`.
- SSA co toan quyen trong giai doan hien tai. Data-scope don vi va nguoi phu trach se duoc them khi mo quyen cho nguoi dung thuong.
- Tao, cap nhat, xoa mem va trinh tham dinh deu ghi timeline append-only.
- Tao ho so, tao bo ho so SoanThao, chi tiet soan thao va timeline TaoMoi phai nam trong mot transaction.
- Chi sua/xoa khi bo ho so SoanThao dang la buoc hien tai va co trang thai Nhap.
- Trinh tham dinh chi duoc phep khi du dieu kien du lieu va tai lieu. Chuyen buoc va ghi timeline phai trong cung transaction.
- `DanhMucVanBanId`, quy trinh, buoc va trang thai se duoc validate qua DanhMucService truoc khi mo API cho nguoi dung thuong.

## 5. DTO lo khoi tao

`TaoHoSoSoanThaoRequest` nhan ten ho so, ten du thao, cac Id danh muc/quy trinh/buoc/trang thai, don vi chu tri, nguoi phu trach, nam xay dung va noi dung soan thao.

`CapNhatHoSoSoanThaoRequest` cap nhat thong tin ho so va noi dung soan thao, khong tu y doi quy trinh, buoc hoac trang thai.

## 6. Thu tu thuc hien

1. Ho so soan thao va timeline.
2. Migration va CRUD y kien don vi.
3. Tong hop y kien va file tong hop.
4. Upload/version tai lieu soan thao.
5. Kiem tra dieu kien va trinh tham dinh.
