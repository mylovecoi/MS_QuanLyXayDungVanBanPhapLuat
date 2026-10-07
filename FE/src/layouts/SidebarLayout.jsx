import {useSidebar} from "../context/SidebarContext";
import {Outlet} from "react-router";
import AppHeader from "./AppHeader.jsx";
import BackDrop from "./BackDrop";
import AppSidebar from "./AppSidebar";
import {getFrontendMenu} from "../shared/api/systemApi.js";
import {useEffect} from "react";

const SidebarLayout = () => {
    const {
        isExpanded,
        isHovered,
        isMobileOpen,
    } = useSidebar();

    useEffect(() => {
        const loadMenu = async () => {
            try {
                const data = await getFrontendMenu();
            } catch (error) {
            }
        };

        void loadMenu();
    }, []);

    return (
        <div className="min-h-screen xl:flex">
            <div>
                <AppSidebar/>
                <BackDrop/>
            </div>

            <div
                className={`flex min-h-screen flex-1 flex-col transition-all duration-300 ease-in-out ${
                    isExpanded || isHovered
                        ? "lg:ml-[300px]"
                        : "lg:ml-[90px]"
                } ${
                    isMobileOpen ? "ml-0" : ""
                }`}
            >
                <AppHeader/>

                <main className="flex-1">
                    <div className="w-full p-4 md:p-6">
                        <Outlet/>
                    </div>
                </main>
            </div>
        </div>
    );
};

export default SidebarLayout;

