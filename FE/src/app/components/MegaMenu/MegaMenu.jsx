import {Link} from "react-router";

const MegaMenu = ({items, onClose}) => {
    return (
        <div
            className="
                absolute
                left-0
                right-0
                top-full
                z-50
                border-b
                border-gray-200
                bg-white
                shadow-lg
                dark:border-gray-800
                dark:bg-gray-900
            "
        >
            <div className="mx-auto max-w-[1600px] px-8 py-8">
                <div
                    className="
                        grid
                        grid-cols-2
                        gap-x-8
                        gap-y-8
                        md:grid-cols-3
                        lg:grid-cols-4
                        xl:grid-cols-5
                    "
                >
                    {items.map((item) => (
                        <div key={item.name}>
                            {/* MENU CHA */}
                            <div
                                className="
                                    mb-3
                                    flex
                                    items-center
                                    gap-2
                                    text-sm
                                    font-semibold
                                    text-gray-800
                                    dark:text-white
                                "
                            >
                                <span className="menu-item-icon-size">
                                    {item.icon}
                                </span>

                                <span>
                                    {item.name}
                                </span>
                            </div>

                            {/* MENU CON */}
                            {item.subItems?.length > 0 && (
                                <div className="space-y-1">
                                    {item.subItems.map((subItem) => (
                                        <Link
                                            key={subItem.name}
                                            to={subItem.path}
                                            onClick={onClose}
                                            className="
                                                block
                                                rounded-md
                                                px-3
                                                py-2
                                                text-sm
                                                text-gray-600
                                                transition-colors
                                                hover:bg-gray-100
                                                hover:text-brand-500
                                                dark:text-gray-400
                                                dark:hover:bg-white/5
                                                dark:hover:text-brand-400
                                            "
                                        >
                                            {subItem.name}
                                        </Link>
                                    ))}
                                </div>
                            )}

                            {/* MENU KHÔNG CÓ SUBMENU */}
                            {!item.subItems?.length && item.path && (
                                <Link
                                    to={item.path}
                                    onClick={onClose}
                                    className="
                                        block
                                        rounded-md
                                        px-3
                                        py-2
                                        text-sm
                                        text-gray-600
                                        transition-colors
                                        hover:bg-gray-100
                                        hover:text-brand-500
                                        dark:text-gray-400
                                        dark:hover:bg-white/5
                                        dark:hover:text-brand-400
                                    "
                                >
                                    Mở trang
                                </Link>
                            )}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default MegaMenu;