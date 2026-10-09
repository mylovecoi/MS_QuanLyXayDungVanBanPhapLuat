import HoSoDetailPage from "./pages/HoSoDetailPage.jsx";
import HoSoFormPage from "./pages/HoSoFormPage.jsx";
import HoSoListPage from "./pages/HoSoListPage.jsx";
import HoSoYKienDongGopPage from "./pages/HoSoYKienDongGopPage.jsx";
import HoSoTrinhThamDinhPage from "./pages/HoSoTrinhThamDinhPage.jsx";
import HoSoTrinhThamDinhListPage from "./pages/HoSoTrinhThamDinhListPage.jsx";
import HoSoThamDinhListPage from "./pages/HoSoThamDinhListPage.jsx";
import HoSoThamDinhPage from "./pages/HoSoThamDinhPage.jsx";
import HoSoTrinhYKienUBNDListPage from "./pages/HoSoTrinhYKienUBNDListPage.jsx";
import HoSoTrinhYKienUBNDPage from "./pages/HoSoTrinhYKienUBNDPage.jsx";
import HoSoYKienUBNDListPage from "./pages/HoSoYKienUBNDListPage.jsx";
import HoSoYKienUBNDPage from "./pages/HoSoYKienUBNDPage.jsx";
import HoSoBanHanhListPage from "./pages/HoSoBanHanhListPage.jsx";
import HoSoBanHanhPage from "./pages/HoSoBanHanhPage.jsx";
import HoSoThamTraHDNDListPage from "./pages/HoSoThamTraHDNDListPage.jsx";
import HoSoThamTraHDNDPage from "./pages/HoSoThamTraHDNDPage.jsx";

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
  {
    path: "/xay-dung-van-ban/trinh-tham-dinh",
    element: <HoSoTrinhThamDinhListPage />,
  },
  {
    path: "/xay-dung-van-ban/trinh-tham-dinh/:id",
    element: <HoSoTrinhThamDinhPage />,
  },
  {
    path: "/admin/xay-dung-van-ban/ho-so/:id/trinh-tham-dinh",
    element: <HoSoTrinhThamDinhPage />,
  },
  {
    path: "/xay-dung-van-ban/tham-dinh",
    element: <HoSoThamDinhListPage />,
  },
  {
    path: "/xay-dung-van-ban/tham-dinh/:id",
    element: <HoSoThamDinhPage />,
  },
  {
    path: "/xay-dung-van-ban/trinh-phe-duyet",
    element: <HoSoTrinhYKienUBNDListPage />,
  },
  {
    path: "/xay-dung-van-ban/trinh-phe-duyet/:id",
    element: <HoSoTrinhYKienUBNDPage />,
  },
  {
    path: "/xay-dung-van-ban/trinh-y-kien-ubnd",
    element: <HoSoTrinhYKienUBNDListPage />,
  },
  {
    path: "/xay-dung-van-ban/trinh-y-kien-ubnd/:id",
    element: <HoSoTrinhYKienUBNDPage />,
  },
  {
    path: "/xay-dung-van-ban/y-kien-ubnd",
    element: <HoSoYKienUBNDListPage />,
  },
  {
    path: "/xay-dung-van-ban/y-kien-ubnd/:id",
    element: <HoSoYKienUBNDPage />,
  },
  {
    path: "/xay-dung-van-ban/ban-hanh",
    element: <HoSoBanHanhListPage />,
  },
  {
    path: "/xay-dung-van-ban/ban-hanh/:id",
    element: <HoSoBanHanhPage />,
  },
  {
    path: "/xay-dung-van-ban/tham-tra-hdnd",
    element: <HoSoThamTraHDNDListPage />,
  },
  {
    path: "/xay-dung-van-ban/tham-tra-hdnd/:id",
    element: <HoSoThamTraHDNDPage />,
  },
];
