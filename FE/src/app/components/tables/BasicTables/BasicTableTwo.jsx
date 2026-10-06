import {useMemo, useState} from "react";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../ui/table";

import Badge from "../../ui/badge/Badge";
import Select from "../../forms/Select.jsx";
import Input from "../../forms/input/InputField.jsx";

const tableData = [
    {
        id: 1,
        user: {
            image: "/images/user/user-17.jpg",
            name: "Lindsey Curtis",
            role: "Web Designer",
        },
        projectName: "Agency Website",
        team: {
            images: [
                "/images/user/user-22.jpg",
                "/images/user/user-23.jpg",
                "/images/user/user-24.jpg",
            ],
        },
        budget: "3.9K",
        status: "Active",
    },
    {
        id: 2,
        user: {
            image: "/images/user/user-18.jpg",
            name: "Kaiya George",
            role: "Project Manager",
        },
        projectName: "Technology",
        team: {
            images: ["/images/user/user-25.jpg", "/images/user/user-26.jpg"],
        },
        budget: "24.9K",
        status: "Pending",
    },
    {
        id: 3,
        user: {
            image: "/images/user/user-17.jpg",
            name: "Zain Geidt",
            role: "Content Writing",
        },
        projectName: "Blog Writing",
        team: {
            images: ["/images/user/user-27.jpg"],
        },
        budget: "12.7K",
        status: "Active",
    },
    {
        id: 4,
        user: {
            image: "/images/user/user-20.jpg",
            name: "Abram Schleifer",
            role: "Digital Marketer",
        },
        projectName: "Social Media",
        team: {
            images: [
                "/images/user/user-28.jpg",
                "/images/user/user-29.jpg",
                "/images/user/user-30.jpg",
            ],
        },
        budget: "2.8K",
        status: "Cancel",
    },
    {
        id: 5,
        user: {
            image: "/images/user/user-21.jpg",
            name: "Carla George",
            role: "Front-end Developer",
        },
        projectName: "Website",
        team: {
            images: [
                "/images/user/user-31.jpg",
                "/images/user/user-32.jpg",
                "/images/user/user-33.jpg",
            ],
        },
        budget: "4.5K",
        status: "Active",
    },
    {
        id: 6,
        user: {
            image: "/images/user/user-17.jpg",
            name: "John Smith",
            role: "UI Designer",
        },
        projectName: "Dashboard",
        team: {
            images: ["/images/user/user-22.jpg"],
        },
        budget: "8.2K",
        status: "Pending",
    },
    {
        id: 7,
        user: {
            image: "/images/user/user-18.jpg",
            name: "Emma Wilson",
            role: "Developer",
        },
        projectName: "Mobile App",
        team: {
            images: [
                "/images/user/user-23.jpg",
                "/images/user/user-24.jpg",
            ],
        },
        budget: "15.4K",
        status: "Active",
    },
    {
        id: 8,
        user: {
            image: "/images/user/user-20.jpg",
            name: "Michael Brown",
            role: "Product Manager",
        },
        projectName: "E-commerce",
        team: {
            images: ["/images/user/user-25.jpg"],
        },
        budget: "21.3K",
        status: "Cancel",
    },
    {
        id: 9,
        user: {
            image: "/images/user/user-21.jpg",
            name: "Sophia Davis",
            role: "Marketing",
        },
        projectName: "Landing Page",
        team: {
            images: [
                "/images/user/user-26.jpg",
                "/images/user/user-27.jpg",
            ],
        },
        budget: "6.7K",
        status: "Active",
    },
    {
        id: 10,
        user: {
            image: "/images/user/user-17.jpg",
            name: "William Taylor",
            role: "Backend Developer",
        },
        projectName: "API System",
        team: {
            images: ["/images/user/user-28.jpg"],
        },
        budget: "18.9K",
        status: "Pending",
    },
    {
        id: 11,
        user: {
            image: "/images/user/user-18.jpg",
            name: "Olivia Martin",
            role: "UX Designer",
        },
        projectName: "CRM System",
        team: {
            images: ["/images/user/user-29.jpg"],
        },
        budget: "11.2K",
        status: "Active",
    },
    {
        id: 12,
        user: {
            image: "/images/user/user-20.jpg",
            name: "James Anderson",
            role: "Developer",
        },
        projectName: "Admin Portal",
        team: {
            images: [
                "/images/user/user-30.jpg",
                "/images/user/user-31.jpg",
            ],
        },
        budget: "14.5K",
        status: "Cancel",
    },
];

export default function BasicTableTwo() {
    const [search, setSearch] = useState("");
    const [pageCurrent, setPageCurrent] = useState(1);
    const [pageSize, setPageSize] = useState(5);

    const filteredData = useMemo(() => {
        const keyword = search.trim().toLowerCase();

        if (!keyword) {
            return tableData;
        }

        return tableData.filter((item) =>
            [
                item.user.name,
                item.user.role,
                item.projectName,
                item.status,
                item.budget,
            ]
                .join(" ")
                .toLowerCase()
                .includes(keyword)
        );
    }, [search]);

    const totalRecord = filteredData.length;

    const totalPages = Math.max(
        1,
        Math.ceil(totalRecord / pageSize)
    );

    const currentPage = Math.min(
        pageCurrent,
        totalPages
    );

    const paginatedData = useMemo(() => {
        const startIndex = (currentPage - 1) * pageSize;

        return filteredData.slice(
            startIndex,
            startIndex + pageSize
        );
    }, [filteredData, currentPage, pageSize]);

    const handleSearch = (value) => {
        setSearch(value);
        setPageCurrent(1);
    };

    const handlePageSizeChange = (event) => {
        setPageSize(Number(event.target.value));
        setPageCurrent(1);
    };

    const handlePrevious = () => {
        if (currentPage > 1) {
            setPageCurrent((prev) => prev - 1);
        }
    };

    const handleNext = () => {
        if (currentPage < totalPages) {
            setPageCurrent((prev) => prev + 1);
        }
    };

    return (
        <div
            className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="grid grid-cols-1 gap-4 border-b border-gray-100 px-5 py-4 sm:grid-cols-12 dark:border-white/[0.05]">
                {/* Page size - 3/12 */}
                <div className="w-full sm:col-span-3">
                    <label className="mb-2 block text-xs font-medium text-gray-500 dark:text-gray-400">
                        Hiển Thị
                    </label>

                    <Select
                        value={pageSize}
                        onChange={(value) => {
                            setPageSize(Number(value));
                            setPageCurrent(1);
                        }}
                        options={[
                            { value: 5, label: "5 thông tin" },
                            { value: 10, label: "10 thông tin" },
                            { value: 20, label: "20 thông tin" },
                            { value: 100, label: "100 thông tin" },
                        ]}
                    />
                </div>

                {/* Search - 9/12 */}
                <div className="w-full sm:col-span-9">
                    <label className="mb-2 block text-xs font-medium text-gray-500 dark:text-gray-400">
                        Tìm Kiếm
                    </label>

                    <Input
                        value={search}
                        onChange={(event) => handleSearch(event.target.value)}
                        placeholder="Search..."
                        prefix={
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                width="18"
                                height="18"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="2"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                            >
                                <circle cx="11" cy="11" r="8" />
                                <path d="m21 21-4.3-4.3" />
                            </svg>
                        }
                        suffix={
                            search && (
                                <button
                                    type="button"
                                    onClick={() => handleSearch("")}
                                    className="flex h-6 w-6 items-center justify-center rounded-full text-gray-400 transition hover:bg-gray-100 hover:text-gray-600 dark:hover:bg-gray-800 dark:hover:text-gray-200"
                                    aria-label="Clear search"
                                >
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        width="16"
                                        height="16"
                                        viewBox="0 0 24 24"
                                        fill="none"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    >
                                        <path d="M18 6 6 18" />
                                        <path d="m6 6 12 12" />
                                    </svg>
                                </button>
                            )
                        }
                    />
                </div>
            </div>

            {/* Table */}
            <div className="max-w-full overflow-x-auto">
                <Table>
                    <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
                        <TableRow>
                            <TableCell
                                isHeader
                                className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                            >
                                User
                            </TableCell>

                            <TableCell
                                isHeader
                                className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                            >
                                Project Name
                            </TableCell>

                            <TableCell
                                isHeader
                                className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                            >
                                Team
                            </TableCell>

                            <TableCell
                                isHeader
                                className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                            >
                                Status
                            </TableCell>

                            <TableCell
                                isHeader
                                className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                            >
                                Budget
                            </TableCell>
                        </TableRow>
                    </TableHeader>

                    <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                        {paginatedData.map((order) => (
                            <TableRow key={order.id}>
                                <TableCell className="px-5 py-4 sm:px-6 text-start">
                                    <div className="flex items-center gap-3">
                                        <div className="w-10 h-10 overflow-hidden rounded-full">
                                            <img
                                                width={40}
                                                height={40}
                                                src={order.user.image}
                                                alt={order.user.name}
                                            />
                                        </div>

                                        <div>
                                            <span
                                                className="block font-medium text-gray-800 text-theme-sm dark:text-white/90">
                                                {order.user.name}
                                            </span>

                                            <span className="block text-gray-500 text-theme-xs dark:text-gray-400">
                                                {order.user.role}
                                            </span>
                                        </div>
                                    </div>
                                </TableCell>

                                <TableCell
                                    className="px-4 py-3 text-gray-500 text-start text-theme-sm dark:text-gray-400">
                                    {order.projectName}
                                </TableCell>

                                <TableCell
                                    className="px-4 py-3 text-gray-500 text-start text-theme-sm dark:text-gray-400">
                                    <div className="flex -space-x-2">
                                        {order.team.images.map(
                                            (teamImage, index) => (
                                                <div
                                                    key={index}
                                                    className="w-6 h-6 overflow-hidden border-2 border-white rounded-full dark:border-gray-900"
                                                >
                                                    <img
                                                        width={24}
                                                        height={24}
                                                        src={teamImage}
                                                        alt={`Team member ${
                                                            index + 1
                                                        }`}
                                                        className="w-full size-6"
                                                    />
                                                </div>
                                            )
                                        )}
                                    </div>
                                </TableCell>

                                <TableCell
                                    className="px-4 py-3 text-gray-500 text-start text-theme-sm dark:text-gray-400">
                                    <Badge
                                        size="sm"
                                        color={
                                            order.status === "Active"
                                                ? "success"
                                                : order.status === "Pending"
                                                    ? "warning"
                                                    : "error"
                                        }
                                    >
                                        {order.status}
                                    </Badge>
                                </TableCell>

                                <TableCell className="px-4 py-3 text-gray-500 text-theme-sm dark:text-gray-400">
                                    {order.budget}
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>

            {/* Pagination */}
            <div
                className="flex flex-col gap-4 border-t border-gray-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-white/[0.05]">

                <div className="text-sm text-gray-500 dark:text-gray-400">
                    Showing{" "}
                    {totalRecord === 0
                        ? 0
                        : (currentPage - 1) * pageSize + 1}
                    {" "}
                    to{" "}
                    {Math.min(currentPage * pageSize, totalRecord)}
                    {" "}
                    of {totalRecord} entries
                </div>

                <div className="flex items-center gap-2">
                    <button
                        type="button"
                        onClick={handlePrevious}
                        disabled={currentPage === 1}
                        className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                    >
                        Previous
                    </button>

                    <div className="flex items-center gap-1">
                        {Array.from(
                            {length: totalPages},
                            (_, index) => index + 1
                        ).map((page) => (
                            <button
                                key={page}
                                type="button"
                                onClick={() => setPageCurrent(page)}
                                className={`h-9 min-w-9 rounded-lg px-3 text-sm font-medium ${
                                    currentPage === page
                                        ? "bg-brand-500 text-white"
                                        : "text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                                }`}
                            >
                                {page}
                            </button>
                        ))}
                    </div>

                    <button
                        type="button"
                        onClick={handleNext}
                        disabled={currentPage === totalPages}
                        className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                    >
                        Next
                    </button>
                </div>
            </div>
        </div>
    );
}