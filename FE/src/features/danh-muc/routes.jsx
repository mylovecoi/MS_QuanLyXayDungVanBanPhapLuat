import DiaDanhPage from "./pages/DiaDanhPage.jsx";
import DonViPage from "./pages/DonViPage.jsx";
import TrangThaiPage from "./pages/TrangThaiPage.jsx";
import VanBanPage from "./pages/VanBanPage.jsx";

export const danhMucRoutes = [
  {
    path: "/admin/danh-muc/dia-danh",
    element: <DiaDanhPage />,
  },
  {
    path: "/admin/danh-muc/don-vi",
    element: <DonViPage />,
  },
  {
    path: "/admin/danh-muc/van-ban",
    element: <VanBanPage />,
  },
  {
    path: "/admin/danh-muc/trang-thai",
    element: <TrangThaiPage />,
  },
];
