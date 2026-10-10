import {
    useCallback,
    useEffect,
    useState,
} from "react";
import {Link, useLocation} from "react-router";
import {
    ChevronDownIcon,
    HorizontaLDots,
    MenuIcon,
} from "../assets/icons/index.js";
import {useSidebar} from "../context/SidebarContext";
import {getFrontendMenu} from "../shared/api/systemApi.js";

const normalizePath = (path = "") => {
    const normalized = path.split("?")[0].split("#")[0].replace(/\/+$/, "");
    return normalized || "/";
};

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
        (path) => {
            const currentPath = normalizePath(location.pathname);
            const targetPath = normalizePath(path);

            if (!targetPath || targetPath === "/") {
                return currentPath === "/";
            }

            return currentPath === targetPath ||
                currentPath.startsWith(`${targetPath}/`);
        },
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
        <ul className="flex w-full flex-col gap-1.5">
            {items.map((item, index) => {
                const key = parentKey
                    ? `${parentKey}-${index}`
                    : `${index}`;

                const hasChildren = item.children?.length > 0;
                const isOpen = !!openSubmenu[key];

                return (
                    <li
                        key={item.roleActionId || item.title || key}
                        className="w-full"
                    >
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
                                    <MenuIcon/>
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
                                        <ul className="ml-3 mt-2 w-[calc(100%-0.75rem)] space-y-1 border-l border-[var(--admin-border)] pl-2">
                                            {item.children.map(
                                                (child, childIndex) => {
                                                    const childKey =
                                                        `${key}-${childIndex}`;

                                                    const childHasChildren =
                                                        child.children?.length > 0;

                                                    return (
                                                        <li
                                                            key={
                                                                child.roleActionId ||
                                                                child.title ||
                                                                childKey
                                                            }
                                                            className="w-full"
                                                        >
                                                            {childHasChildren ? (
                                                                renderMenuItems(
                                                                    [child],
                                                                    childKey
                                                                )
                                                            ) : child.url ? (
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
                                                            ) : (
                                                                <div
                                                                    className="menu-dropdown-item !justify-start text-left w-full"
                                                                >
                                                                    <span className="mr-2 w-3 shrink-0">
                                                                        -
                                                                    </span>

                                                                    <span className="text-left">
                                                                        {
                                                                            child.title
                                                                        }
                                                                    </span>
                                                                </div>
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
            className={`modern-admin-sidebar fixed left-0 top-0 z-50 mt-16 flex h-[calc(100vh-1rem)] flex-col rounded-r-[var(--admin-radius-xl)] border border-l-0 border-[var(--admin-border)] bg-[var(--admin-surface)] px-4 text-gray-900 shadow-[var(--admin-shadow-panel)] transition-all duration-300 ease-in-out dark:bg-gray-900 lg:bottom-4 lg:left-4 lg:top-4 lg:mt-0 lg:h-[calc(100vh-2rem)] lg:rounded-[var(--admin-radius-xl)] lg:border ${
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
                className={`modern-admin-sidebar-logo flex py-5 ${
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
                    <div className="flex flex-col gap-4 rounded-[var(--admin-radius-md)] bg-gray-50/80 p-2 dark:bg-white/[0.03]">
                        <div>
                            <h2
                                className={`mb-3 flex px-2 text-xs font-semibold uppercase leading-[20px] tracking-[0.08em] text-gray-400 ${
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
