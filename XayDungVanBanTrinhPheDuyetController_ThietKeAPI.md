# Thiet ke API XayDungVanBanTrinhPheDuyetController

Route goc: `/api/xay-dung-van-ban/trinh-phe-duyet`.
RoleAction: `VanBanQPPL.XayDungVanBan.TrinhPheDuyet`.

| Method | Endpoint | Quyen | Chuc nang |
|---|---|---|---|
| POST | `/` | Create | Tao bo ho so trinh phe duyet tu ket qua tham dinh da gui |
| GET | `/{hoSoId}` | Index | Xem bo ho so trinh phe duyet hien hanh |
| PUT | `/{hoSoId}` | Edit | Cap nhat cap trinh, muc dich, to trinh, noi dung va don vi dong gui |
| GET | `/{hoSoId}/tai-lieu` | Index | Xem tai lieu ke thua va bo sung |
| POST | `/{hoSoId}/tai-lieu` | Create | Tai tai lieu bo sung khi ho so dang Nhap |
| DELETE | `/{hoSoId}/tai-lieu/{boHoSoTaiLieuId}` | Delete | Go tai lieu bo sung |
| GET | `/{hoSoId}/kiem-tra-truoc-gui` | Index | Tra dieu kien chua dat truoc khi gui |
| POST | `/{hoSoId}/gui` | Approve | Gui ho so trinh phe duyet, chuyen bo ho so sang DaGui |
| POST | `/{hoSoId}/huy` | Delete | Huy mem bo trinh khi dang Nhap |

`POST /` nhan `hoSoId`, `capTrinh` va `mucDichTrinh`. `PUT /{hoSoId}` cap nhat noi dung to trinh. API `gui` kiem tra co cap trinh, muc dich, noi dung trinh va it nhat mot file dinh kem; sau do chuyen bo ho so sang `DaGui`. He thong khong theo doi xu ly phe duyet ben ngoai.
