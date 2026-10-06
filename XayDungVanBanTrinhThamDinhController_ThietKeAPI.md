# Thiet ke API XayDungVanBanTrinhThamDinhController

Route goc: `/api/xay-dung-van-ban/trinh-tham-dinh`.
RoleAction: `VanBanQPPL.XayDungVanBan.TrinhThamDinh`.

| Method | Endpoint | Quyen | Muc dich |
|---|---|---|---|
| POST | `/` | Create | Tao bo ho so trinh tham dinh, ke thua tai lieu hien hanh tu soan thao |
| GET | `/{hoSoId}` | Index | Xem bo ho so trinh tham dinh hien hanh |
| PUT | `/{hoSoId}` | Edit | Cap nhat thong tin to trinh va de nghi tham dinh |
| GET | `/{hoSoId}/tai-lieu` | Index | Xem tai lieu ke thua va tai lieu bo sung |
| POST | `/{hoSoId}/tai-lieu` | Create | Tai tai lieu bo sung khi bo ho so dang Nhap |
| DELETE | `/{hoSoId}/tai-lieu/{boHoSoTaiLieuId}` | Delete | Go tai lieu bo sung, khong go tai lieu ke thua |
| GET | `/{hoSoId}/kiem-tra-truoc-gui-tham-dinh` | Index | Tra dieu kien chua dat truoc khi gui |
| POST | `/{hoSoId}/gui-tham-dinh` | Approve | Gui ho so, chuyen bo nguon va bo trinh sang DaGui, chuyen buoc/trang thai ho so |
| POST | `/{hoSoId}/huy-trinh-tham-dinh` | Delete | Huy mem bo ho so khi dang Nhap |

`POST /` chi tao bo ho so o trang thai `Nhap`; `POST /{hoSoId}/gui-tham-dinh` moi thay doi buoc va trang thai cua ho so.
