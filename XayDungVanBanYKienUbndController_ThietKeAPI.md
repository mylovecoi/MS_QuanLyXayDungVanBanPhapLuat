# Thiet ke API XayDungVanBanYKienUbndController

Route goc: `/api/xay-dung-van-ban/y-kien-ubnd`.
RoleAction: `VanBanQPPL.XayDungVanBan.YKienUbnd`.

| Method | Endpoint | Quyen | Chuc nang |
|---|---|---|---|
| POST | `/` | Create | Tao bo ho so lay y kien UBND tu ho so da trinh phe duyet |
| GET | `/{hoSoId}` | Index | Xem ket qua tong hop y kien UBND |
| PUT | `/{hoSoId}` | Edit | Cap nhat so lieu, ket luan, tong hop va giai trinh |
| GET | `/{hoSoId}/tai-lieu` | Index | Xem tai lieu ke thua va file tong hop |
| POST | `/{hoSoId}/tai-lieu` | Create | Dinh kem file tong hop, bien ban hoac giai trinh |
| DELETE | `/{hoSoId}/tai-lieu/{boHoSoTaiLieuId}` | Delete | Go tai lieu bo sung khi dang Nhap |
| GET | `/{hoSoId}/kiem-tra-truoc-gui` | Index | Kiem tra dieu kien ket thuc giai doan |
| POST | `/{hoSoId}/gui` | Approve | Gui ket qua y kien UBND va chuyen buoc/trang thai |
| POST | `/{hoSoId}/huy` | Delete | Huy mem bo ho so khi dang Nhap |

Lay y kien va xu ly phe duyet duoc thuc hien ben ngoai he thong. Phan mem chi luu ket qua tong hop, tai lieu lien quan va lich su xu ly.
