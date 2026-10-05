// import { SidebarProvider, useSidebar } from "../context/SidebarContext";
// import { Outlet } from "react-router";
// import AppHeader from "./AppHeader.jsx";
// import Backdrop from "./Backdrop";
// import AppSidebar from "./AppSidebar";
// import Footer from "../components/footer/Footer.jsx";
//
// const LayoutContent = () => {
//     const { isExpanded, isHovered, isMobileOpen } = useSidebar();
//
//     return (
//         <div className="min-h-screen xl:flex">
//             <div>
//                 <AppSidebar />
//                 <Backdrop />
//             </div>
//
//             <div
//                 className={`flex min-h-screen flex-1 flex-col transition-all duration-300 ease-in-out ${
//                     isExpanded || isHovered
//                         ? "lg:ml-[290px]"
//                         : "lg:ml-[90px]"
//                 } ${isMobileOpen ? "ml-0" : ""}`}
//             >
//                 <AppHeader />
//
//                 {/* Nội dung trang */}
//                 <main className="flex-1">
//                     {/*<div className="p-4 mx-auto max-w-(--breakpoint-2xl) md:p-6">*/}
//                     <div className="w-full p-4 md:p-6">
//                         <Outlet />
//                     </div>
//                 </main>
//
//                 <Footer />
//             </div>
//         </div>
//     );
// };
//
// const AppLayout = () => {
//     return (
//         <SidebarProvider>
//             <LayoutContent />
//         </SidebarProvider>
//     );
// };
//
// export default AppLayout;
import {SidebarProvider} from "../context/SidebarContext";
import {LayoutProvider, useLayout} from "../context/LayoutContext";

import SidebarLayout from "./SidebarLayout.jsx";
import TopbarLayout from "./TopbarLayout.jsx";

const LayoutContent = () => {
    const {layoutMode} = useLayout();

    if (layoutMode === "topbar") {
        return <TopbarLayout/>;
    }

    return <SidebarLayout/>;
};

const AppLayout = () => {
    return (
        <LayoutProvider>
            <SidebarProvider>
                <LayoutContent/>
            </SidebarProvider>
        </LayoutProvider>
    );
};

export default AppLayout;