import  RoleActionListPage  from "../../pages/RoleActionListPage.jsx";
import GroupPermissionPage from "./pages/GroupPermissionPage.jsx";
import LogEntryPage from "./pages/LogEntryPage.jsx";
import SystemInfoPage from "./pages/SystemInfoPage.jsx";
import UserAccountPage from "./pages/UserAccountPage.jsx";

export const quanTriHeThongRoutes = [
  {
    path: "/he-thong/danh-sach-chuc-nang",
    element: <RoleActionListPage />,
  },
  {
    path: "/he-thong/cau-hinh-he-thong",
    element: <SystemInfoPage />,
  },
  {
    path: "/he-thong/tai-khoan-truy-cap",
    element: <UserAccountPage />,
  },
  {
    path: "/he-thong/nhom-quyen-truy-cap",
    element: <GroupPermissionPage />,
  },
  {
    path: "/he-thong/nhat-ky-he-thong",
    element: <LogEntryPage />,
  },
];
