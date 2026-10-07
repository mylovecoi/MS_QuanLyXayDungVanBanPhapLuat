import {
    useCallback,
    useEffect,
    useState,
} from "react";
import {Link, useLocation} from "react-router";
import {ChevronDownIcon, HorizontaLDots} from "../assets/icons/index.js";
import {useSidebar} from "../context/SidebarContext";
import {getFrontendMenu} from "../shared/api/systemApi.js";
import {MenuIcon} from "../assets/icons/index.js";

const AppSidebar = () => {
    const {
        isExpanded,
        isMobileOpen,
        isHovered,
        setIsHovered,
    } = useSidebar();

    const [menuItems, setMenuItems] = useState([]);
    const [menuLoading, setMenuLoading] = useState(true);

    const location = useLocation();

    const [openSubmenu, setOpenSubmenu] = useState({});

    const isActive = useCallback(
        (path) => location.pathname === path,
        [location.pathname]
    );

    useEffect(() => {
        const loadMenu = async () => {
            try {
                setMenuLoading(true);

                const data = await getFrontendMenu();
                setMenuItems(data?.items || []);
            } catch (error) {
                setMenuItems([]);
            } finally {
                setMenuLoading(false);
            }
        };

        void loadMenu();
    }, []);

    const findActiveParent = useCallback(
        (items, parentKey = "") => {
            for (let index = 0; index < items.length; index++) {
                const item = items[index];

                const key = parentKey
                    ? `${parentKey}-${index}`
                    : `${index}`;

                if (item.url && isActive(item.url)) {
                    return parentKey;
                }

                if (item.children?.length) {
                    const result = findActiveParent(
                        item.children,
                        key
                    );

                    if (result !== null) {
                        return result;
                    }
                }
            }

            return null;
        },
        [isActive]
    );


    useEffect(() => {
        if (!menuItems.length) {
            return;
        }

        const activeParent = findActiveParent(menuItems);

        if (activeParent !== null && activeParent !== "") {
            setOpenSubmenu((prev) => ({
                ...prev,
                [activeParent]: true,
            }));
        }
    }, [menuItems, location.pathname, findActiveParent]);


    const handleSubmenuToggle = (key) => {
        setOpenSubmenu((prev) => ({
            ...prev,
            [key]: !prev[key],
        }));
    };


    const renderMenuItems = (items, parentKey = "") => (
        <ul className="flex flex-col gap-4 w-full">
            {items.map((item, index) => {
                const key = parentKey
                    ? `${parentKey}-${index}`
                    : `${index}`;

                const hasChildren = item.children?.length > 0;
                const isOpen = !!openSubmenu[key];

                return (
                    <li key={item.title || key} className="w-full">
                        {hasChildren ? (
                            <button
                                onClick={() =>
                                    handleSubmenuToggle(key)
                                }
                                className={`menu-item group w-full ${
                                    isOpen
                                        ? "menu-item-active"
                                        : "menu-item-inactive"
                                } cursor-pointer ${
                                    !isExpanded && !isHovered
                                        ? "lg:justify-center"
                                        : "lg:justify-start"
                                }`}
                            >
                            <span
                                className={`menu-item-icon-size ${
                                    isOpen
                                        ? "menu-item-icon-active"
                                        : "menu-item-icon-inactive"
                                }`}
                            >
                                <MenuIcon />
                            </span>

                                {(isExpanded ||
                                    isHovered ||
                                    isMobileOpen) && (
                                    <span className="menu-item-text !text-left flex-1 min-w-0">
                                    {item.title}
                                </span>
                                )}

                                {(isExpanded ||
                                    isHovered ||
                                    isMobileOpen) && (
                                    <ChevronDownIcon
                                        className={`ml-auto shrink-0 w-5 h-5 transition-transform duration-200 ${
                                            isOpen
                                                ? "rotate-180 text-brand-500"
                                                : ""
                                        }`}
                                    />
                                )}
                            </button>
                        ) : (
                            item.url && (
                                <Link
                                    to={item.url}
                                    className={`menu-item group w-full ${
                                        isActive(item.url)
                                            ? "menu-item-active"
                                            : "menu-item-inactive"
                                    }`}
                                >
                                <span
                                    className={`menu-item-icon-size ${
                                        isActive(item.url)
                                            ? "menu-item-icon-active"
                                            : "menu-item-icon-inactive"
                                    }`}
                                >
                                    {/* Menu con không có icon */}
                                </span>

                                    {(isExpanded ||
                                        isHovered ||
                                        isMobileOpen) && (
                                        <span className="menu-item-text !text-left">
                                        {item.title}
                                    </span>
                                    )}
                                </Link>
                            )
                        )}

                        {hasChildren &&
                            (isExpanded ||
                                isHovered ||
                                isMobileOpen) && (
                                <div
                                    className={`grid overflow-hidden transition-[grid-template-rows] duration-300 ${
                                        isOpen
                                            ? "grid-rows-[1fr]"
                                            : "grid-rows-[0fr]"
                                    }`}
                                >
                                    <div className="min-h-0">
                                        <ul className="mt-2 space-y-1 ml-3 w-[calc(100%-0.75rem)]">
                                            {item.children.map(
                                                (child, childIndex) => {
                                                    const childKey = `${key}-${childIndex}`;

                                                    return (
                                                        <li
                                                            key={
                                                                child.title ||
                                                                childKey
                                                            }
                                                            className="w-full"
                                                        >
                                                            {child.children?.length ? (
                                                                renderMenuItems(
                                                                    [child],
                                                                    childKey
                                                                )
                                                            ) : (
                                                                child.url && (
                                                                    <Link
                                                                        to={
                                                                            child.url
                                                                        }
                                                                        className={`menu-dropdown-item !justify-start text-left w-full ${
                                                                            isActive(
                                                                                child.url
                                                                            )
                                                                                ? "menu-dropdown-item-active"
                                                                                : "menu-dropdown-item-inactive"
                                                                        }`}
                                                                    >
                                                                    <span className="mr-2 w-3 shrink-0">
                                                                        -
                                                                    </span>

                                                                        <span className="text-left">
                                                                        {
                                                                            child.title
                                                                        }
                                                                    </span>
                                                                    </Link>
                                                                )
                                                            )}
                                                        </li>
                                                    );
                                                }
                                            )}
                                        </ul>
                                    </div>
                                </div>
                            )}
                    </li>
                );
            })}
        </ul>
    );

    return (
        <aside
            className={`fixed mt-16 flex flex-col lg:mt-0 top-0 px-5 left-0 bg-white dark:bg-gray-900 dark:border-gray-800 text-gray-900 h-screen transition-all duration-300 ease-in-out z-50 border-r border-gray-200 ${
                isExpanded || isMobileOpen
                    ? "w-[300px]"
                    : isHovered
                        ? "w-[290px]"
                        : "w-[90px]"
            } ${
                isMobileOpen
                    ? "translate-x-0"
                    : "-translate-x-full"
            } lg:translate-x-0`}
            onMouseEnter={() => !isExpanded && setIsHovered(true)}
            onMouseLeave={() => setIsHovered(false)}
        >
            <div
                className={`py-4  flex ${
                    !isExpanded && !isHovered
                        ? "lg:justify-center"
                        : "justify-center"
                }`}
            >
                <Link to="/">
                    <div>
                        {isExpanded || isHovered || isMobileOpen ? (
                            <>
                                <img
                                    className="dark:hidden mr-3"
                                    src="/images/logo/logo-Life.png"
                                    alt="Logo"
                                    width={150}
                                    height={40}
                                />

                                <img
                                    className="hidden dark:block mr-3"
                                    src="/images/logo/logo-Life.png"
                                    alt="Logo"
                                    width={150}
                                    height={40}
                                />
                            </>
                        ) : (
                            <img
                                src="/images/logo/logo-Life.png"
                                alt="Logo"
                                width={80}
                                height={50}
                            />
                        )}
                    </div>
                </Link>
            </div>

            <div className="flex flex-col overflow-y-auto duration-300 ease-linear no-scrollbar">
                <nav className="mb-6">
                    <div className="flex flex-col gap-4">
                        <div>
                            <h2
                                className={`mb-4 text-xs uppercase flex leading-[20px] text-gray-400 ${
                                    !isExpanded && !isHovered
                                        ? "lg:justify-center"
                                        : "justify-start"
                                }`}
                            >
                                {isExpanded ||
                                isHovered ||
                                isMobileOpen ? (
                                    "Menu"
                                ) : (
                                    <HorizontaLDots className="size-6"/>
                                )}
                            </h2>

                            {menuLoading ? (
                                <div className="text-sm text-gray-400">
                                    Đang tải menu...
                                </div>
                            ) : (
                                renderMenuItems(menuItems)
                            )}
                        </div>
                    </div>
                </nav>
            </div>
        </aside>
    );
};

export default AppSidebar;