# Thiet ke API XayDungVanBanThamDinhController

Route goc: `/api/xay-dung-van-ban/tham-dinh`.
RoleAction: `VanBanQPPL.XayDungVanBan.ThamDinh`.

| Method | Endpoint | Quyen | Chuc nang |
|---|---|---|---|
| POST | `/` | Create | Tao bo ho so tham dinh tu bo TrinhThamDinh da gui |
| GET | `/{hoSoId}` | Index | Xem ho so tham dinh hien hanh |
| POST | `/{hoSoId}/tiep-nhan` | Edit | Tiep nhan ho so |
| GET | `/{hoSoId}/tai-lieu` | Index | Xem tai lieu ke thua va tai lieu bo sung |
| POST | `/{hoSoId}/tai-lieu` | Create | Tai tai lieu phuc vu/keta qua tham dinh |
| DELETE | `/{hoSoId}/tai-lieu/{boHoSoTaiLieuId}` | Delete | Go tai lieu bo sung |
| PUT | `/{hoSoId}/ket-qua` | Edit | Cap nhat ket qua va ket luan tham dinh |
| GET | `/{hoSoId}/kiem-tra-truoc-gui-ket-qua` | Index | Kiem tra dieu kien gui ket qua |
| POST | `/{hoSoId}/yeu-cau-bo-sung` | Approve | Yeu cau bo sung va chuyen ve SoanThao |
| POST | `/{hoSoId}/gui-ket-qua` | Approve | Gui ket qua va chuyen sang buoc ke tiep |
| POST | `/{hoSoId}/huy-tham-dinh` | Delete | Huy mem bo tham dinh chua gui ket qua |
| POST | `/{hoSoId}/so-sanh-du-thao` | Index | So sanh hai file DOCX thuoc cung ho so |
| GET | `/{hoSoId}/so-sanh-du-thao/{soSanhId}` | Index | Xem ket qua so sanh da luu |
| GET | `/{hoSoId}/so-sanh-du-thao/{soSanhId}/bao-cao` | Index | Lay bao cao HTML so sanh |

Tao va tiep nhan la hai thao tac rieng. Hai nhanh `yeu-cau-bo-sung` va `gui-ket-qua` loai tru nhau. Moi thay doi trang thai, ke thua tai lieu va lich su xu ly nam trong transaction.
