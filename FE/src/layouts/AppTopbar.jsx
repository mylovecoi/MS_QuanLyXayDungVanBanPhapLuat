import {useState} from "react";

import MegaMenu from "../components/MegaMenu/MegaMenu.jsx";
import {navItems, othersItems} from "../config/menuConfig.jsx";

const AppTopbar = () => {
    const [isMegaMenuOpen, setIsMegaMenuOpen] = useState(false);

    const allMenus = [
        ...navItems,
        ...othersItems,
    ];

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
            onMouseLeave={() => setIsMegaMenuOpen(false)}
        >
            {/* TOPBAR */}
            <div className="flex justify-center px-4">
                <div className="flex items-center gap-1">
                    {allMenus.map((item) => (
                        <div
                            key={item.name}
                            onMouseEnter={() => setIsMegaMenuOpen(true)}
                        >
                            <div
                                className="
                                    flex
                                    cursor-pointer
                                    items-center
                                    gap-2
                                    rounded-lg
                                    px-4
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
                                <span className="menu-item-icon-size">
                                    {item.icon}
                                </span>

                                <span className="whitespace-nowrap">
                                    {item.name}
                                </span>
                            </div>
                        </div>
                    ))}
                </div>
            </div>

            {/* MEGA MENU */}
            {isMegaMenuOpen && (
                <MegaMenu
                    items={allMenus}
                    onClose={() => setIsMegaMenuOpen(false)}
                />
            )}
        </div>
    );
};

export default AppTopbar;