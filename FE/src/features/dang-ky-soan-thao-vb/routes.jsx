import DanhSachDangKyPage from "./pages/DanhSachDangKyPage.jsx";
import HoSoDangKyPage from "./pages/HoSoDangKyPage.jsx";
import KetQuaDangKyPage from "./pages/KetQuaDangKyPage.jsx";
import DangKyForm from "./pages/DangKyForm.jsx";

export const dangKyVanBanRoutes = [
    {
        path: "/dang-ky-xay-dung-van-ban/danh-sach",
        element: <DanhSachDangKyPage />,
    },
    {
        path: "/dang-ky-xay-dung-van-ban/ho-so",
        element: <HoSoDangKyPage />,
    },
    {
        path: "/dang-ky-xay-dung-van-ban/ket-qua",
        element: <KetQuaDangKyPage />,
    },
    {
        path: "/dang-ky-xay-dung-van-ban/ho-so/them-moi",
        element: <DangKyForm />,
    },
    {
        path: "/dang-ky-xay-dung-van-ban/ho-so/:id",
        element: <DangKyForm />,
    },
]
