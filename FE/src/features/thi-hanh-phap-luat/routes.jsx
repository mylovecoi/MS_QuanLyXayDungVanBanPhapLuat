import ViecDuocGiaoPage from "./pages/ViecDuocGiaoPage.jsx";
import BaoCaoTienDoPage from "./pages/BaoCaoTienDoPage.jsx";
import DanhGiaBaoCaoPage from "./pages/DanhGiaBaoCaoPage.jsx";
import BaoCaoTongHopPage from "./pages/BaoCaoTongHopPage.jsx";
import BaoCaoTongHopDetailPage from "./pages/BaoCaoTongHopDetailPage.jsx";
import { Navigate } from "react-router";
import KeHoachThiHanhListPage from "./pages/KeHoachThiHanhListPage.jsx";
import KeHoachThiHanhDetailPage from "./pages/KeHoachThiHanhDetailPage.jsx";

export const thiHanhPhapLuatRoutes = [
  { path: "/thi-hanh-phap-luat/danh-sach", element: <KeHoachThiHanhListPage /> },
  { path: "/thi-hanh-phap-luat/ke-hoach", element: <Navigate to="/thi-hanh-phap-luat/danh-sach" replace /> },
  { path: "/thi-hanh-phap-luat/ke-hoach/:id", element: <KeHoachThiHanhDetailPage /> },
  { path: "/thi-hanh-phap-luat/noi-dung-ke-hoach", element: <Navigate to="/thi-hanh-phap-luat/danh-sach" replace /> },
  { path: "/thi-hanh-phap-luat/phan-cong", element: <Navigate to="/thi-hanh-phap-luat/danh-sach" replace /> },
  { path: "/thi-hanh-phap-luat/tien-do", element: <Navigate to="/thi-hanh-phap-luat/viec-duoc-giao" replace /> },
  { path: "/thi-hanh-phap-luat/viec-duoc-giao", element: <ViecDuocGiaoPage /> },
  { path: "/thi-hanh-phap-luat/viec-duoc-giao/:phanCongId", element: <BaoCaoTienDoPage /> },
  { path: "/thi-hanh-phap-luat/danh-gia", element: <DanhGiaBaoCaoPage /> },
  { path: "/thi-hanh-phap-luat/tong-hop/:id", element: <BaoCaoTongHopDetailPage /> },
  { path: "/thi-hanh-phap-luat/tong-hop", element: <BaoCaoTongHopPage /> },
];
