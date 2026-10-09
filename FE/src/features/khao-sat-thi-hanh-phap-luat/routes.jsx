import CuocKhaoSatListPage from "./pages/CuocKhaoSatListPage.jsx";
import CuocKhaoSatDetailPage from "./pages/CuocKhaoSatDetailPage.jsx";
import MauPhieuKhaoSatPage from "./pages/MauPhieuKhaoSatPage.jsx";
import NhapKetQuaKhaoSatPage from "./pages/NhapKetQuaKhaoSatPage.jsx";
import RaSoatKetQuaKhaoSatPage from "./pages/RaSoatKetQuaKhaoSatPage.jsx";
import DashboardKhaoSatPage from "./pages/DashboardKhaoSatPage.jsx";
import BaoCaoKhaoSatPage from "./pages/BaoCaoKhaoSatPage.jsx";

export const khaoSatThiHanhPhapLuatRoutes = [
  { path: "/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat", element: <CuocKhaoSatListPage /> },
  { path: "/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat/:id", element: <CuocKhaoSatDetailPage /> },
  { path: "/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat/:id/mau-phieu", element: <MauPhieuKhaoSatPage /> },
  { path: "/khao-sat-thi-hanh-phap-luat/nhap-ket-qua", element: <NhapKetQuaKhaoSatPage /> },
  { path: "/khao-sat-thi-hanh-phap-luat/ra-soat", element: <RaSoatKetQuaKhaoSatPage /> },
  { path: "/khao-sat-thi-hanh-phap-luat/dashboard", element: <DashboardKhaoSatPage /> },
  { path: "/khao-sat-thi-hanh-phap-luat/bao-cao", element: <BaoCaoKhaoSatPage /> },
];
