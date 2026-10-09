import ViecDuocGiaoPage from "./pages/ViecDuocGiaoPage.jsx";
import BaoCaoTienDoPage from "./pages/BaoCaoTienDoPage.jsx";
import DanhGiaBaoCaoPage from "./pages/DanhGiaBaoCaoPage.jsx";
import BaoCaoTongHopPage from "./pages/BaoCaoTongHopPage.jsx";

export const thiHanhPhapLuatRoutes = [
  { path: "/thi-hanh-phap-luat/viec-duoc-giao", element: <ViecDuocGiaoPage /> },
  { path: "/thi-hanh-phap-luat/viec-duoc-giao/:phanCongId", element: <BaoCaoTienDoPage /> },
  { path: "/thi-hanh-phap-luat/danh-gia", element: <DanhGiaBaoCaoPage /> },
  { path: "/thi-hanh-phap-luat/tong-hop", element: <BaoCaoTongHopPage /> },
];
