import Badge from "../../../app/components/ui/badge/Badge.jsx";
import BasicTableTwo from "../../../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import {getDanhSachDangKyXayDungVanBan} from "../api/dangKyXayDungVanBanApi.js";

const badgeColor = (color) => ({
    blue: "primary",
    green: "success",
    red: "error",
    orange: "warning",
    cyan: "info",
    purple: "primary",
    gray: "light",
}[color] || "primary");

export default function DanhSachDangKyPage() {
    const [data, setData] = useState([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let mounted = true;
        const load = async () => {
            try {
                setLoading(true);
                setError("");
                const result = await getDanhSachDangKyXayDungVanBan({pageSize: 200});
                if (mounted) {
                    setData(result.items || []);
                }
            } catch (err) {
                if (mounted) {
                    setError(err?.response?.data?.message || err?.message || "Không thể tải danh sách đăng ký.");
                }
            } finally {
                if (mounted) {
                    setLoading(false);
                }
            }
        };
        void load();
        return () => {
            mounted = false;
        };
    }, []);

    const columns = useMemo(
        () => [
            {
                key: "stt",
                header: "#",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400",
                render: (item, rowIndex) => rowIndex + 1,
            },

            {
                key: "maHoSo",
                header: "Mã hồ sơ",
                render: (item) => (
                    <div className="font-medium text-gray-800 dark:text-white/90">
                        {item.maHoSo || "-"}
                    </div>
                ),
            },

            {
                key: "tenHoSo",
                header: "Tên hồ sơ",
                render: (item) => (
                    <div className="max-w-[280px] font-medium text-gray-800 dark:text-white/90">
                        {item.tenHoSo || "-"}
                    </div>
                ),
            },

            {
                key: "tenVanBanDuKien",
                header: "Tên dự thảo văn bản",
                render: (item) => (
                    <div className="max-w-[320px] text-gray-700 dark:text-gray-300">
                        {item.tenVanBanDuKien || "-"}
                    </div>
                ),
            },

            {
                key: "namDangKy",
                header: "Năm xây dựng",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400",
                render: (item) => (
                    <span>{item.namDangKy || "-"}</span>
                ),
            },

            {
                key: "trangThai",
                header: "Trạng thái",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center",
                render: (item) => (
                    <Badge
                        size="sm"
                        color={badgeColor(item.mauTrangThaiHoSo)}
                    >
                        {item.tenTrangThaiHoSo || item.maTrangThaiHoSo || "-"}
                    </Badge>
                ),
            },

            {
                key: "createdAt",
                header: "Ngày tạo",
                render: (item) => (
                    <span className="text-sm text-gray-600 dark:text-gray-300">
                        {item.createdAt
                            ? new Date(
                                item.createdAt
                            ).toLocaleDateString("vi-VN")
                            : "-"}
                    </span>
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
                        {/* Xem */}
                        <button
                            type="button"
                            title="Xem chi tiết"
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
                                <path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z"/>
                                <circle
                                    cx="12"
                                    cy="12"
                                    r="3"
                                />
                            </svg>
                        </button>

                        {/* Chỉnh sửa */}
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

                        {/* Xóa */}
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
                        Danh sách đăng ký
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý danh sách đăng ký xây dựng văn bản
                    </p>
                </div>
            </div>

            {/* Table */}
            <BasicTableTwo
                data={data}
                columns={columns}
                searchPlaceholder="Tìm kiếm hồ sơ..."
                emptyText={loading ? "Đang tải dữ liệu..." : "Không có hồ sơ đăng ký."}
                searchFields={[
                    (item) => item.maHoSo,
                    (item) => item.tenHoSo,
                    (item) => item.tenVanBanDuKien,
                    (item) => item.namDangKy,
                    (item) => item.tenTrangThaiHoSo,
                ]}
            />
            {error && <div className="mt-4"><Alert variant="error" title="Không thể tải dữ liệu" message={error}/></div>}
        </div>
    );
}
