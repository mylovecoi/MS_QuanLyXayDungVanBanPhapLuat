import Badge from "../app/components/ui/badge/Badge";
import BasicTableTwo from "../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {useEffect, useMemo, useState} from "react";
import {getRoleActionList} from "../shared/api/systemApi.js";


export default function RoleActionListPage() {
    const [roleActionData, setRoleActionData] = useState([]);
    const [loading, setLoading] = useState(false);

    const [search, setSearch] = useState("");
    const [pageSize, setPageSize] = useState(10);
    const [pageCurrent, setPageCurrent] = useState(1);
    const [totalCount, setTotalCount] = useState(0);

    useEffect(() => {
        const fetchRoleActions = async () => {
            try {
                setLoading(true);

                const response = await getRoleActionList({
                    search,
                    pageSize,
                    pageCurrent,
                });

                setRoleActionData(response?.data ?? []);
                setTotalCount(response?.totalCount ?? 0);
            } catch (error) {
                console.error("Lỗi lấy danh sách chức năng:", error);
            } finally {
                setLoading(false);
            }
        };

        void fetchRoleActions();
    }, [search, pageSize, pageCurrent]);

    const columns = useMemo(
        () => [
            {
                key: "sttSapXep",
                header: "#",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400",
            },

            {
                key: "phanLoai",
                header: "Phân loại",
                render: (item) => (
                    <div
                        style={{
                            paddingLeft: `${item.level * 16}px`,
                        }}
                    >
                        <Badge
                            size="sm"
                            color={
                                item.phanLoai === "Group"
                                    ? "primary"
                                    : "success"
                            }
                        >
                            {item.phanLoai}

                            {item.phanLoai === "Group" &&
                                item.level > 0 &&
                                ` ${item.level}`}
                        </Badge>
                    </div>
                ),
            },

            {
                key: "title",
                header: "Mô tả chức năng",
                render: (item) => (
                    <div
                        style={{
                            paddingLeft: `${item.level * 16}px`,
                        }}
                        className="font-medium text-gray-800 dark:text-white/90"
                    >
                        {item.title}
                    </div>
                ),
            },

            {
                key: "role",
                header: "Tương tác",
                render: (item) => (
                    <div className="space-y-1 text-sm">
                        <div>
                            <span className="font-medium text-gray-700 dark:text-gray-300">
                                Role:
                            </span>{" "}
                            {item.role}
                        </div>

                        {item.phanLoai !== "Group" && (
                            <>
                                <div>
                                    <span className="font-medium text-gray-700 dark:text-gray-300">
                                        Controller:
                                    </span>{" "}
                                    {item.controller}
                                </div>

                                <div>
                                    <span className="font-medium text-gray-700 dark:text-gray-300">
                                        Action:
                                    </span>{" "}
                                    {item.action}
                                </div>

                                <div>
                                    <span className="font-medium text-gray-700 dark:text-gray-300">
                                        Table:
                                    </span>{" "}
                                    {item.table}
                                </div>
                            </>
                        )}
                    </div>
                ),
            },

            {
                key: "useGroup",
                header: "Use",
            },

            {
                key: "status",
                header: "Trạng thái",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center",
                render: (item) => (
                    <Badge
                        size="sm"
                        color={
                            item.status === "Kích hoạt"
                                ? "success"
                                : "error"
                        }
                    >
                        {item.status}
                    </Badge>
                ),
            },

            {
                key: "actions",
                header: "Thao tác",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center",
                render: (item) => (
                    <div className="flex items-center justify-center gap-2">
                        {/* Thêm chức năng con */}
                        {item.phanLoai !== "Detail" && (
                            <button
                                type="button"
                                title="Thêm chức năng con"
                                className="flex h-8 w-8 items-center justify-center rounded-lg text-gray-500 transition hover:bg-gray-100 hover:text-brand-500 dark:text-gray-400 dark:hover:bg-white/[0.05]"
                            >
                                +
                            </button>
                        )}

                        {/* Edit */}
                        <button
                            type="button"
                            title="Chỉnh sửa"
                            className="flex h-8 w-8 items-center justify-center rounded-lg text-gray-500 transition hover:bg-gray-100 hover:text-brand-500 dark:text-gray-400 dark:hover:bg-white/[0.05]"
                        >
                            <svg
                                width="18"
                                height="18"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="2"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                            >
                                <path d="M12 20h9"/>
                                <path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4Z"/>
                            </svg>
                        </button>

                        {/* Delete */}
                        <button
                            type="button"
                            title="Xóa"
                            className="flex h-8 w-8 items-center justify-center rounded-lg text-gray-500 transition hover:bg-gray-100 hover:text-error-500 dark:text-gray-400 dark:hover:bg-white/[0.05]"
                        >
                            <svg
                                width="18"
                                height="18"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="2"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                            >
                                <path d="M3 6h18"/>
                                <path d="M8 6V4h8v2"/>
                                <path d="M19 6v14H5V6"/>
                                <path d="M10 11v5"/>
                                <path d="M14 11v5"/>
                            </svg>
                        </button>
                    </div>
                ),
            },
        ],
        []
    );

    return (
        <div>
            {/* Page header */}
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Danh sách chức năng
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý Danh sách chức năng
                    </p>
                </div>

                <button
                    type="button"
                    className="inline-flex items-center justify-center gap-2 rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-brand-600"
                >
                    <span className="text-lg leading-none">
                        +
                    </span>

                    Thêm mới
                </button>
            </div>

            {/* Reusable table */}
            <BasicTableTwo
                data={roleActionData}
                columns={columns}
                searchPlaceholder="Tìm kiếm chức năng..."
                searchFields={[
                    (item) => item.title,
                    (item) => item.phanLoai,
                    (item) => item.role,
                    (item) => item.controller,
                    (item) => item.action,
                    (item) => item.table,
                    (item) => item.useGroup,
                    (item) => item.status,
                ]}
            />
        </div>
    );
}