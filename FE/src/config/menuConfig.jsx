import {
    BoxCubeIcon,
    CalenderIcon,
    GridIcon,
    HorizontaLDots,
    ListIcon,
    PageIcon,
    PieChartIcon,
    PlugInIcon,
    TableIcon,
    UserCircleIcon,
    HouseIcon,
    FolderNghiepVu,
    MenuIcon,
} from "../assets/icons/index.js";

export const navItems = [
    {
        icon: <HouseIcon />,
        name: "Trang chủ",
        path: "/",
    },
    {
        icon: <FolderNghiepVu />,
        name: "Nghiệp vụ",
        subItems: [
            {
                name: "Hệ thống văn bản",
                path: "/HeThongVanBan",
                pro: false,
            },
        ],
    },
    {
        icon: <MenuIcon />,
        name: "Văn bản pháp luật",
        path: "/VanBanPhapLuat",
    },
    {
        icon: <GridIcon />,
        name: "Dashboard",
        subItems: [
            {
                name: "Ecommerce",
                path: "/Ecommerce",
                pro: false,
            },
        ],
    },
    {
        icon: <CalenderIcon />,
        name: "Calendar",
        path: "/calendar",
    },
    {
        icon: <UserCircleIcon />,
        name: "User Profile",
        path: "/profile",
    },
    {
        name: "Forms",
        icon: <ListIcon />,
        subItems: [
            {
                name: "Form Elements",
                path: "/form-elements",
                pro: false,
            },
        ],
    },
    {
        name: "Tables",
        icon: <TableIcon />,
        subItems: [
            {
                name: "Basic Tables",
                path: "/basic-tables",
                pro: false,
            },
        ],
    },
    {
        name: "Pages",
        icon: <PageIcon />,
        subItems: [
            {
                name: "Blank Page",
                path: "/blank",
                pro: false,
            },
            {
                name: "404 Error",
                path: "/error-404",
                pro: false,
            },
        ],
    },
];

export const othersItems = [
    {
        icon: <PieChartIcon />,
        name: "Charts",
        subItems: [
            {
                name: "Line Chart",
                path: "/line-chart",
                pro: false,
            },
            {
                name: "Bar Chart",
                path: "/bar-chart",
                pro: false,
            },
        ],
    },
    {
        icon: <BoxCubeIcon />,
        name: "UI Elements",
        subItems: [
            {
                name: "Alerts",
                path: "/alerts",
                pro: false,
            },
            {
                name: "Avatar",
                path: "/avatars",
                pro: false,
            },
            {
                name: "Badge",
                path: "/badge",
                pro: false,
            },
            {
                name: "Buttons",
                path: "/buttons",
                pro: false,
            },
            {
                name: "Images",
                path: "/images",
                pro: false,
            },
            {
                name: "Videos",
                path: "/videos",
                pro: false,
            },
        ],
    },
    {
        icon: <PlugInIcon />,
        name: "Authentication",
        subItems: [
            {
                name: "Sign In",
                path: "/signin",
                pro: false,
            },
            {
                name: "Sign Up",
                path: "/signup",
                pro: false,
            },
        ],
    },
];