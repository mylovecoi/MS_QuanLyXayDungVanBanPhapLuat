import TraCuuPage from "./pages/TraCuuPage.jsx";
import CanhBaoThongMinhPage from "./pages/CanhBaoThongMinhPage.jsx";
import DashboardKhaiThacDuLieuPage from "./pages/DashboardKhaiThacDuLieuPage.jsx";
import DashboardCauHinhPage from "./pages/DashboardCauHinhPage.jsx";
import BaoCaoListPage from "./pages/BaoCaoListPage.jsx";
import BaoCaoKetQuaPage from "./pages/BaoCaoKetQuaPage.jsx";
export const khaiThacDuLieuRoutes = [
  { path: "/dashboard", element: <DashboardKhaiThacDuLieuPage /> },
  { path: "/dashboard/tong-quan", element: <DashboardKhaiThacDuLieuPage /> },
  { path: "/dashboard/cau-hinh", element: <DashboardCauHinhPage /> },
  { path: "/canh-bao/thong-minh", element: <CanhBaoThongMinhPage /> },
  { path: "/bao-cao-thong-ke", element: <BaoCaoListPage /> },
  { path: "/bao-cao-thong-ke/:maBaoCao", element: <BaoCaoKetQuaPage /> },
  { path: "/khai-thac-du-lieu/dashboard", element: <DashboardKhaiThacDuLieuPage /> },
  { path: "/khai-thac-du-lieu/dashboard/cau-hinh", element: <DashboardCauHinhPage /> },
  { path: "/khai-thac-du-lieu/canh-bao/thong-minh", element: <CanhBaoThongMinhPage /> },
  { path: "/khai-thac-du-lieu/bao-cao", element: <BaoCaoListPage /> },
  { path: "/khai-thac-du-lieu/bao-cao/:maBaoCao", element: <BaoCaoKetQuaPage /> },
  { path: "canh-bao/thong-minh", element: <CanhBaoThongMinhPage /> },
  { path: "tra-cuu/tong-hop", element: <TraCuuPage source="tong-hop" /> },
  { path: "tra-cuu/dang-ky-xay-dung-van-ban", element: <TraCuuPage source="dang-ky-xay-dung-van-ban" /> },
  { path: "tra-cuu/xay-dung-van-ban", element: <TraCuuPage source="xay-dung-van-ban" /> },
  { path: "tra-cuu/thi-hanh-phap-luat", element: <TraCuuPage source="thi-hanh-phap-luat" /> },
];