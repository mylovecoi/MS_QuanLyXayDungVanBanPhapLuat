import TraCuuPage from "./pages/TraCuuPage.jsx";
import CanhBaoThongMinhPage from "./pages/CanhBaoThongMinhPage.jsx";
import DashboardKhaiThacDuLieuPage from "./pages/DashboardKhaiThacDuLieuPage.jsx";
import DashboardCauHinhPage from "./pages/DashboardCauHinhPage.jsx";
import BaoCaoListPage from "./pages/BaoCaoListPage.jsx";
import BaoCaoKetQuaPage from "./pages/BaoCaoKetQuaPage.jsx";
export const khaiThacDuLieuRoutes = [
  { path: "canh-bao/thong-minh", element: <CanhBaoThongMinhPage /> },
  { path: "tra-cuu/tong-hop", element: <TraCuuPage source="tong-hop" /> },
  { path: "tra-cuu/dang-ky-xay-dung-van-ban", element: <TraCuuPage source="dang-ky-xay-dung-van-ban" /> },
  { path: "tra-cuu/xay-dung-van-ban", element: <TraCuuPage source="xay-dung-van-ban" /> },
  { path: "tra-cuu/thi-hanh-phap-luat", element: <TraCuuPage source="thi-hanh-phap-luat" /> },
];
