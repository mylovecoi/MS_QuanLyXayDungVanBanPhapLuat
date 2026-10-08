import HoSoDetailPage from "./pages/HoSoDetailPage.jsx";
import HoSoFormPage from "./pages/HoSoFormPage.jsx";
import HoSoListPage from "./pages/HoSoListPage.jsx";
import HoSoYKienDongGopPage from "./pages/HoSoYKienDongGopPage.jsx";

export const xayDungVanBanRoutes = [
  {
    path: "/xay-dung-van-ban/danh-sach",
    element: <HoSoListPage />,
  },
  {
    path: "/xay-dung-van-ban/danh-sach/them-moi",
    element: <HoSoFormPage />,
  },
  {
    path: "/xay-dung-van-ban/danh-sach/:id",
    element: <HoSoFormPage />,
  },
  {
    path: "/xay-dung-van-ban/danh-sach/chi-tiet/:id",
    element: <HoSoDetailPage />,
  },
  {
    path: "/xay-dung-van-ban/danh-sach/:id/y-kien-dong-gop",
    element: <HoSoYKienDongGopPage />,
  },
  {
    path: "/xay-dung-van-ban/danh-sach/:id/chinh-sua",
    element: <HoSoFormPage />,
  },
  {
    path: "/xay-dung-van-ban/soan-thao",
    element: <HoSoListPage />,
  },
  {
    path: "/xay-dung-van-ban/soan-thao/them-moi",
    element: <HoSoFormPage />,
  },
  {
    path: "/admin/xay-dung-van-ban/ho-so",
    element: <HoSoListPage />,
  },
  {
    path: "/admin/xay-dung-van-ban/ho-so/them-moi",
    element: <HoSoFormPage />,
  },
  {
    path: "/admin/xay-dung-van-ban/ho-so/:id",
    element: <HoSoFormPage />,
  },
  {
    path: "/admin/xay-dung-van-ban/ho-so/chi-tiet/:id",
    element: <HoSoDetailPage />,
  },
  {
    path: "/admin/xay-dung-van-ban/ho-so/:id/y-kien-dong-gop",
    element: <HoSoYKienDongGopPage />,
  },
  {
    path: "/admin/xay-dung-van-ban/ho-so/:id/chinh-sua",
    element: <HoSoFormPage />,
  },
];
