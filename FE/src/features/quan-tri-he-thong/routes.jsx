import  RoleActionListPage  from "../../pages/RoleActionListPage.jsx";
import GroupPermissionPage from "./pages/GroupPermissionPage.jsx";
import LogEntryPage from "./pages/LogEntryPage.jsx";
import SystemInfoPage from "./pages/SystemInfoPage.jsx";
import UserAccountPage from "./pages/UserAccountPage.jsx";

export const quanTriHeThongRoutes = [
  {
    path: "/admin/he-thong/danh-sach-chuc-nang",
    element: <RoleActionListPage />,
  },
  {
    path: "/admin/he-thong/cau-hinh-he-thong",
    element: <SystemInfoPage />,
  },
  {
    path: "/admin/he-thong/tai-khoan-truy-cap",
    element: <UserAccountPage />,
  },
  {
    path: "/admin/he-thong/nhom-quyen-truy-cap",
    element: <GroupPermissionPage />,
  },
  {
    path: "/admin/he-thong/nhat-ky-he-thong",
    element: <LogEntryPage />,
  },
];
