import {useEffect, useState} from "react";

import MegaMenu from "../app/components/MegaMenu/MegaMenu.jsx";
import {getFrontendMenu} from "../shared/api/systemApi.js";

const AppTopbar = () => {
    const [activeMenu, setActiveMenu] = useState(null);
    const [allMenus, setAllMenus] = useState([]);

    useEffect(() => {
        const loadMenu = async () => {
            try {
                const data = await getFrontendMenu();
                setAllMenus(data?.items || []);
            } catch (error) {
                console.error("Lỗi lấy menu:", error);
                setAllMenus([]);
            }
        };

        void loadMenu();
    }, []);

    return (
        <div
            className="
                relative
                border-b
                border-gray-200
                bg-white
                dark:border-gray-800
                dark:bg-gray-900
            "
            onMouseLeave={() => setActiveMenu(null)}
        >
            {/* TOPBAR */}
            <div className="flex justify-start px-8">
                <div className="flex items-center gap-3">
                    {allMenus.map((item) => (
                        <div
                            key={item.id}
                            onMouseEnter={() => setActiveMenu(item)}
                        >
                            <div
                                className="
                                    flex
                                    cursor-pointer
                                    items-center
                                    gap-2
                                    rounded-lg
                                    px-5
                                    py-3
                                    text-sm
                                    font-medium
                                    text-gray-700
                                    transition-colors
                                    hover:bg-gray-100
                                    hover:text-brand-500
                                    dark:text-gray-300
                                    dark:hover:bg-white/5
                                    dark:hover:text-brand-400
                                "
                                            >
                                <span className="whitespace-nowrap">
                                    {item.title}
                                </span>
                            </div>
                        </div>
                    ))}
                </div>
            </div>

            {/* MEGA MENU */}
            {activeMenu && (
                <MegaMenu
                    item={activeMenu}
                    onClose={() => setActiveMenu(null)}
                />
            )}
        </div>
    );
};

export default AppTopbar;