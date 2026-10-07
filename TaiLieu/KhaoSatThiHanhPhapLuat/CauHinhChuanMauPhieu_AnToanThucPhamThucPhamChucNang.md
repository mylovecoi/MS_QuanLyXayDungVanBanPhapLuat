# Cau hinh mau phieu khao sat an toan thuc pham thuc pham chuc nang

Tai lieu nay chuan hoa hai mau phieu de khai bao trong `KhaoSatThiHanhPhapLuatService`. File Word phat hanh cho nguoi tra loi duoc luu kem mau phieu. He thong nhap cau hinh bang Word co bang du lieu chuan, khong tu phan tich checkbox trong file phat hanh.

## Ma mau phieu

| Ma nhom | Ma mau phieu de xuat | Nhom doi tuong | So cau hoi |
| --- | --- | --- | ---: |
| CQQL_TPCN | CQQL_TPCN_V1 | Co quan quan ly nha nuoc | 16 |
| NGUOI_DAN_TPCN | NGUOI_DAN_TPCN_V1 | Nguoi dan | 16 |

## Cau hinh cau hoi

`BatBuoc` ap dung cho file ket qua tong hop. `MauSoTyLe` cua cac cau hoi lua chon la `PHIEU_HOP_LE`: ty le = so luong lua chon / tong so phieu hop le cua cau hoi. Cac cau chon nhieu duoc phep co tong ty le lon hon 100 phan tram.

| Mau phieu | Cau hoi chon nhieu | Cau hoi y kien tu do | Cau hoi bat buoc |
| --- | --- | --- | --- |
| CQQL_TPCN_V1 | Q6, Q14 | Q16 | Q1-Q15 |
| NGUOI_DAN_TPCN_V1 | Q3, Q6, Q7, Q8, Q15 | Q16 | Q1-Q15 |

Q16 cua hai mau la noi dung kien nghi. Neu mau phieu co lua chon Co/Khong thi khai bao lua chon binh thuong, dong thoi dat `CoYKienTuDo = true` de nhap noi dung kien nghi kem theo.

## Bang Word cau hinh chuan

Bang Word dung de upload mau phieu phai co ba cot bat buoc: `Ma cau hoi`, `Noi dung`, `Loai`. Cac cot bo sung duoc parser ho tro:

| Ma cau hoi | Noi dung | Loai | Ma dap an | Dap an | Bat buoc | Cho phep nhieu lua chon | Mau so ty le | Co y kien tu do |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Q6 | Theo ong ba hanh vi vi pham pho bien hien nay la gi | LUA_CHON | A | Quang cao sai su that | Co | Co | PHIEU_HOP_LE | Khong |
| Q6 | Theo ong ba hanh vi vi pham pho bien hien nay la gi | LUA_CHON | B | Kinh doanh hang gia hang nhai | Co | Co | PHIEU_HOP_LE | Khong |
| Q16 | Kien nghi de xuat | VAN_BAN |  |  | Khong | Khong | PHIEU_HOP_LE | Co |

Gia tri nhan cho cot boolean la `Co`, `Khong`, `X`, `true`, `false`, `1` hoac `0`. `Mau so ty le` chi nhan `PHIEU_HOP_LE` hoac `TONG_LUOT_CHON`.

## File ket qua tong hop

File ket qua nhap vao he thong dung cung bang ma cau hoi va ma dap an, bo sung hai cot `So luong` va `Tong so tra loi`. He thong tu tinh ty le, khong nhap ty le tu Word.

| Ma cau hoi | Noi dung | Loai | Ma dap an | Dap an | So luong | Tong so tra loi |
| --- | --- | --- | --- | --- | ---: | ---: |
| Q6 | Theo ong ba hanh vi vi pham pho bien hien nay la gi | LUA_CHON | A | Quang cao sai su that | 15 | 65 |
| Q6 | Theo ong ba hanh vi vi pham pho bien hien nay la gi | LUA_CHON | B | Kinh doanh hang gia hang nhai | 19 | 65 |
| Q16 | Kien nghi de xuat | VAN_BAN |  |  |  |  |

Voi cau mot lua chon, tong `So luong` cua cac dap an khong duoc vuot `Tong so tra loi`. Voi cau nhieu lua chon, tung dap an khong vuot `Tong so tra loi`, nhung tong cac dap an co the vuot gia tri nay.
