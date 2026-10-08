import {BrowserRouter as Router, Routes, Route} from "react-router";

import {ScrollToTop} from "./components/common/ScrollToTop.jsx";
import AppLayout from "../layouts/AppLayout.jsx";
import SignIn from "../pages/AuthPage/SignIn.jsx";
import SignUp from "../pages/AuthPage/SignUp";
import NotFound from "../pages/OtherPage/NotFound.jsx";
import TrangChu from "../pages/Dashboard/TrangChu/TrangChu.jsx";
import Home from "../pages/Dashboard/Home.jsx";
import UserProfiles from "../pages/UserProfiles.jsx";
import Calendar from "../pages/Calendar.jsx";
import Blank from "../pages/Blank.jsx";
import FormElements from "../pages/Forms/FormElements.jsx";
import BasicTables from "../pages/Tables/BasicTables.jsx";
import Alerts from "../pages/UiElements/Alerts.jsx";
import Badges from "../pages/UiElements/Badges.jsx";
import Avatars from "../pages/UiElements/Avatars.jsx";
import Buttons from "../pages/UiElements/Buttons.jsx";
import Images from "../pages/UiElements/Images.jsx";
import Videos from "../pages/UiElements/Videos.jsx";
import LineChart from "../pages/Charts/LineChart.jsx";
import BarChart from "../pages/Charts/BarChart.jsx";
import LayoutSettings from "../pages/Settings/LayoutSettings.jsx";
import {appFeatureRoutes} from "./routes.jsx";
import ProtectedRoute from "./components/auth/ProtectedRoute.jsx";
import DanhSachDangKyPage from "../features/quan-tri-he-thong/pages/DanhSachDangKyPage.jsx";
import HoSoDangKyPage from "../features/quan-tri-he-thong/pages/HoSoDangKyPage.jsx";
import KetQuaDangKyPage from "../features/quan-tri-he-thong/pages/KetQuaDangKyPage.jsx";

export function App() {
  return (
      <Router>
        <ScrollToTop/>

        <Routes>
          {/* Auth */}
          <Route path="/signin" element={<SignIn/>}/>
          <Route path="/signup" element={<SignUp/>}/>

          {/* Require authentication */}
          <Route element={<ProtectedRoute/>}>
            <Route element={<AppLayout/>}>
              <Route path="/" element={<TrangChu/>}/>
              <Route path="/ecommerce" element={<Home/>}/>
              <Route path="/dang-ky-xay-dung-van-ban/danh-sach" element={<DanhSachDangKyPage/>}/>
              <Route path="/dang-ky-xay-dung-van-ban/ho-so" element={<HoSoDangKyPage/>}/>
              <Route path="/dang-ky-xay-dung-van-ban/ket-qua" element={<KetQuaDangKyPage/>}/>

              <Route
                  path="/settings/layout"
                  element={<LayoutSettings/>}
              />

              <Route path="/profile" element={<UserProfiles/>}/>
              <Route path="/calendar" element={<Calendar/>}/>
              <Route path="/blank" element={<Blank/>}/>

              <Route
                  path="/form-elements"
                  element={<FormElements/>}
              />

              <Route
                  path="/basic-tables"
                  element={<BasicTables/>}
              />

              <Route path="/alerts" element={<Alerts/>}/>
              <Route path="/avatars" element={<Avatars/>}/>
              <Route path="/badge" element={<Badges/>}/>
              <Route path="/buttons" element={<Buttons/>}/>
              <Route path="/images" element={<Images/>}/>
              <Route path="/videos" element={<Videos/>}/>

              <Route path="/line-chart" element={<LineChart/>}/>
              <Route path="/bar-chart" element={<BarChart/>}/>

              {/* Project feature routes */}
              {appFeatureRoutes.map((route) => (
                  <Route
                      key={route.path}
                      path={route.path}
                      element={route.element}
                  />
              ))}
            </Route>
          </Route>

          {/* 404 */}
          <Route path="*" element={<NotFound/>}/>
        </Routes>
      </Router>
  );
}