# MS_QuanLyXayDungVanBanPhapLuat

Solution moi de lam lai he thong theo cau truc tach ro Backend va Frontend.

## Cau truc

```text
BE/
  services/
    QuanTriHeThongService/
    DanhMucService/
  shared/
    DataAccess/
Fe/
  src/
```

## Backend

Backend hien dang chuyen thang tu source cu:

- `QuanTriHeThongService`
- `DanhMucService`
- `DataAccess` de hai service tiep tuc build duoc trong giai doan dau

Tat ca project da duoc doi target framework sang `net10.0`.

Quy tac thiet ke chung:

- Khi xay dung service/module moi, ap dung quy tac RoleAction phan loai `Detail`: moi `Detail` phai co 1 thu muc chua cac Form chinh; Form chi tiet lien quan  va 1 Controller rieng de thiet lap phan quyen.
- Chi tiet xem `TaiLieu/QuyTacXayDungPhanMem.md`.

Lenh build:

```powershell
dotnet restore .\MS_QuanLyXayDungVanBanPhapLuat.slnx --configfile .\NuGet.Config
dotnet build .\MS_QuanLyXayDungVanBanPhapLuat.slnx --no-restore
```

## Frontend

Thu muc `Fe` la ReactJS skeleton bang Vite + TypeScript. May hien tai can co `npm`, `pnpm`, hoac `yarn` trong PATH de cai dependencies va chay dev server.

```powershell
cd Fe
npm install
npm run dev
```

## Viec tiep theo

1. Tach dan `DataAccess` thanh cac DbContext/module ro rang cho tung service.
2. Chuan hoa lai API contract cua `QuanTriHeThongService` va `DanhMucService`.
3. Ket noi ReactJS voi hai service qua API client rieng.
4. Bo sung auth, layout, menu va route cho FE.
