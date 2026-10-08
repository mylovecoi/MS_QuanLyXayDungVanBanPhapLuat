import {Link} from "react-router";
import {MenuIcon} from "../../../assets/icons/index.js";

const MenuTree = ({items, level = 0, onClose}) => {
    return (
        <div
            className={
                level === 0
                    ? "grid grid-cols-2 gap-x-6 gap-y-6 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5"
                    : "space-y-1"
            }
        >
            {items.map((item) => (
                <div key={item.id}>
                    {/* MENU */}
                    {item.url ? (
                        <Link
                            to={item.url}
                            onClick={onClose}
                            className={`
                                flex
                                items-center
                                rounded-md
                                px-3
                                py-2
                                text-sm
                                transition-colors
                                ${
                                level === 0
                                    ? "font-semibold text-gray-800 dark:text-white"
                                    : "text-gray-600 dark:text-gray-400"
                            }
                                hover:bg-gray-100
                                hover:text-brand-500
                                dark:hover:bg-white/5
                                dark:hover:text-brand-400
                            `}
                        >
                            {level === 0 ? (
                                <span className="mr-2 flex h-5 w-5 shrink-0 items-center justify-center">
                                    <MenuIcon />
                                </span>
                            ) : (
                                <span className="mr-2 flex w-3 shrink-0 items-center justify-center">
                                    -
                                </span>
                            )}

                            <span className="min-w-0 text-left">
                                {item.title}
                            </span>
                        </Link>
                    ) : (
                        <div
                            className={`
                                flex
                                items-center
                                rounded-md
                                px-3
                                py-2
                                text-sm
                                ${
                                level === 0
                                    ? "font-semibold text-gray-800 dark:text-white"
                                    : "font-medium text-gray-700 dark:text-gray-300"
                            }
                            `}
                        >
                            {level === 0 ? (
                                <span className="mr-2 flex h-5 w-5 shrink-0 items-center justify-center">
                                    <MenuIcon />
                                </span>
                            ) : (
                                <span className="mr-2 flex w-3 shrink-0 items-center justify-center">
                                    -
                                </span>
                            )}

                            <span className="min-w-0 text-left">
                                {item.title}
                            </span>
                        </div>
                    )}

                    {/* MENU CON */}
                    {item.children?.length > 0 && (
                        <div
                            className={
                                level === 0
                                    ? "mt-1 pl-3"
                                    : "ml-5"
                            }
                        >
                            <MenuTree
                                items={item.children}
                                level={level + 1}
                                onClose={onClose}
                            />
                        </div>
                    )}
                </div>
            ))}
        </div>
    );
};

const MegaMenu = ({item, onClose}) => {
    if (!item) {
        return null;
    }

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
            <div className="w-full px-4 py-6">
                <MenuTree
                    items={item.children || []}
                    onClose={onClose}
                />
            </div>
        </div>
    );
};

export default MegaMenu;