# Quy tac xay dung phan mem

## 1. Nguyen tac thiet ke phan quyen theo RoleAction

Quyen chuc nang cua he thong duoc thiet ke tap trung qua `QuanTriHeThongService`.

- `Users` gan voi `GroupPermision`.
- `GroupPermision` co cac `Permission`.
- `Permission` gan voi `RoleAction`.
- Cac cot quyen chinh gom: `Index`, `Create`, `Edit`, `Delete`, `Approve`, `Public`.

## 2. Quy tac PM ve RoleAction phan loai Detail

Moi `RoleAction` co phan loai `Detail` phai duoc thiet ke thanh 1 Form va 1 Controller rieng de thiet lap phan quyen.

Khi xay dung service, module, man hinh hoac chuc nang moi:

- Xac dinh truoc cac `RoleAction` can co.
- Neu `RoleAction.PhanLoai == "Detail"` thi bat buoc tao Form rieng tuong ung.
- Neu `RoleAction.PhanLoai == "Detail"` thi bat buoc tao Controller/API rieng tuong ung.
- Khong gom nhieu chuc nang `Detail` khac nhau vao cung mot Controller neu can phan quyen doc lap.
- Frontend phai route/man hinh theo dung Form da thiet ke cho `RoleAction`.
- Backend phai kiem tra quyen theo Controller, Action va loai quyen truoc khi thuc hien nghiep vu.

Muc tieu cua quy tac nay la dam bao moi chuc nang chi tiet co diem cau hinh quyen ro rang, de PM/admin co the thiet lap phan quyen rieng cho tung Form/Controller.

## 3. Ap dung cho cac service moi

Khi thiet ke service moi, tai lieu thiet ke phai co muc phan quyen rieng va liet ke:

- Danh sach `RoleAction`.
- `PhanLoai` cua tung `RoleAction`.
- Form tuong ung voi tung `RoleAction` phan loai `Detail`.
- Controller/API tuong ung voi tung `RoleAction` phan loai `Detail`.
- Mapping quyen `Index`, `Create`, `Edit`, `Delete`, `Approve`, `Public`.
- Neu co quyen du lieu rieng, phai ghi ro lop quyen du lieu ket hop voi quyen chuc nang nhu the nao.
