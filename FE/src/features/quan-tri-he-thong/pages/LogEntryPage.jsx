import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {Modal} from "../../../app/components/ui/modal";
import {getLogEntries} from "../api/logEntryApi";

function getErrorMessage(error, fallback) {
    return (
        error?.response?.data?.message ||
        error?.message ||
        fallback
    );
}

function toDateInputValue(date) {
    return date.toISOString().slice(0, 10);
}

function getMonthStart() {
    const now = new Date();
    return toDateInputValue(new Date(now.getFullYear(), now.getMonth(), 1));
}

function formatDateTime(value) {
    if (!value) {
        return "";
    }

    return new Intl.DateTimeFormat("vi-VN", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
    }).format(new Date(value));
}

function methodColor(method) {
    const normalized = String(method || "").toUpperCase();

    if (normalized === "GET") {
        return "info";
    }

    if (normalized === "POST") {
        return "success";
    }

    if (normalized === "PUT" || normalized === "PATCH") {
        return "warning";
    }

    if (normalized === "DELETE") {
        return "error";
    }

    return "light";
}

function usePaginationPages(pageCurrent, totalPages) {
    return useMemo(() => {
        const visiblePages = 5;
        const halfWindow = Math.floor(visiblePages / 2);
        let startPage = Math.max(1, pageCurrent - halfWindow);
        const endPage = Math.min(
            totalPages,
            startPage + visiblePages - 1
        );

        startPage = Math.max(
            1,
            Math.min(startPage, endPage - visiblePages + 1)
        );

        return Array.from(
            {
                length: endPage - startPage + 1,
            },
            (_, index) => startPage + index
        );
    }, [pageCurrent, totalPages]);
}

function PaginationFooter({
    pageCurrent,
    pageSize,
    totalCount,
    loading,
    onPageChange,
}) {
    const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
    const pages = usePaginationPages(pageCurrent, totalPages);

    return (
        <div className="flex flex-col gap-4 border-t border-gray-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-white/[0.05]">
            <div className="text-sm text-gray-500 dark:text-gray-400">
                Showing{" "}
                {totalCount === 0
                    ? 0
                    : (pageCurrent - 1) * pageSize + 1}{" "}
                to {Math.min(pageCurrent * pageSize, totalCount)} of {totalCount} entries
            </div>

            <div className="flex items-center gap-2">
                <button
                    type="button"
                    onClick={() => onPageChange(Math.max(1, pageCurrent - 1))}
                    disabled={pageCurrent === 1 || loading}
                    className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                >
                    Previous
                </button>

                <div className="flex items-center gap-1">
                    {pages[0] > 1 && (
                        <>
                            <button
                                type="button"
                                onClick={() => onPageChange(1)}
                                disabled={loading}
                                className="h-9 min-w-9 rounded-lg px-3 text-sm font-medium text-gray-700 hover:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                            >
                                1
                            </button>
                            {pages[0] > 2 && (
                                <span className="px-1 text-sm text-gray-400">
                                    ...
                                </span>
                            )}
                        </>
                    )}

                    {pages.map((page) => (
                        <button
                            key={page}
                            type="button"
                            onClick={() => onPageChange(page)}
                            disabled={loading}
                            className={`h-9 min-w-9 rounded-lg px-3 text-sm font-medium disabled:cursor-not-allowed disabled:opacity-50 ${
                                pageCurrent === page
                                    ? "bg-brand-500 text-white"
                                    : "text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                            }`}
                        >
                            {page}
                        </button>
                    ))}

                    {pages[pages.length - 1] < totalPages && (
                        <>
                            {pages[pages.length - 1] < totalPages - 1 && (
                                <span className="px-1 text-sm text-gray-400">
                                    ...
                                </span>
                            )}
                            <button
                                type="button"
                                onClick={() => onPageChange(totalPages)}
                                disabled={loading}
                                className="h-9 min-w-9 rounded-lg px-3 text-sm font-medium text-gray-700 hover:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                            >
                                {totalPages}
                            </button>
                        </>
                    )}
                </div>

                <button
                    type="button"
                    onClick={() =>
                        onPageChange(Math.min(totalPages, pageCurrent + 1))
                    }
                    disabled={pageCurrent >= totalPages || loading}
                    className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                >
                    Next
                </button>
            </div>
        </div>
    );
}

export default function LogEntryPage() {
    const [items, setItems] = useState([]);
    const [search, setSearch] = useState("");
    const [fromDate, setFromDate] = useState(getMonthStart());
    const [toDate, setToDate] = useState(toDateInputValue(new Date()));
    const [pageSize, setPageSize] = useState(10);
    const [pageCurrent, setPageCurrent] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [selectedLog, setSelectedLog] = useState(null);

    const fetchLogs = async () => {
        try {
            setLoading(true);
            setError("");

            const response = await getLogEntries({
                search,
                fromDate,
                toDate,
                pageSize,
                pageCurrent,
            });

            setItems(response?.data ?? []);
            setTotalCount(response?.totalCount ?? 0);
        } catch (fetchError) {
            setError(
                getErrorMessage(
                    fetchError,
                    "Không thể tải nhật ký hệ thống."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void fetchLogs();
    }, [search, fromDate, toDate, pageSize, pageCurrent]);

    const handleSearchChange = (value) => {
        setSearch(value);
        setPageCurrent(1);
    };

    const handleDateChange = (field, value) => {
        if (field === "fromDate") {
            setFromDate(value);

            if (toDate && value && toDate < value) {
                setToDate(value);
            }
        } else {
            setToDate(value);

            if (fromDate && value && value < fromDate) {
                setFromDate(value);
            }
        }

        setPageCurrent(1);
    };

    const prettyRequest = useMemo(() => {
        if (!selectedLog?.request) {
            return "";
        }

        try {
            return JSON.stringify(JSON.parse(selectedLog.request), null, 2);
        } catch {
            return selectedLog.request;
        }
    }, [selectedLog]);

    return (
        <div>
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Nhật ký hệ thống
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Theo dõi các truy cập, thao tác và request phát sinh trong hệ thống
                    </p>
                </div>
            </div>

            <div className="space-y-5">
                {error && (
                    <Alert
                        variant="error"
                        title="Không thể xử lý"
                        message={error}
                    />
                )}

                <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
                    <div className="grid grid-cols-1 gap-4 border-b border-gray-100 px-5 py-4 sm:grid-cols-12 dark:border-white/[0.05]">
                        <div className="w-full sm:col-span-2">
                            <Label>Hiển Thị</Label>
                            <Select
                                value={pageSize}
                                onChange={(value) => {
                                    setPageSize(Number(value));
                                    setPageCurrent(1);
                                }}
                                options={[10, 20, 50, 100].map((value) => ({
                                    value,
                                    label: `${value} thông tin`,
                                }))}
                            />
                        </div>

                        <div className="w-full sm:col-span-2">
                            <Label>Từ ngày</Label>
                            <Input
                                type="date"
                                value={fromDate}
                                onChange={(event) =>
                                    handleDateChange("fromDate", event.target.value)
                                }
                            />
                        </div>

                        <div className="w-full sm:col-span-2">
                            <Label>Đến ngày</Label>
                            <Input
                                type="date"
                                value={toDate}
                                onChange={(event) =>
                                    handleDateChange("toDate", event.target.value)
                                }
                            />
                        </div>

                        <div className="w-full sm:col-span-6">
                            <Label>Tìm Kiếm</Label>
                            <Input
                                value={search}
                                onChange={(event) =>
                                    handleSearchChange(event.target.value)
                                }
                                placeholder="Tìm username, URL, method hoặc IP..."
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
                                        <circle cx="11" cy="11" r="8"/>
                                        <path d="m21 21-4.3-4.3"/>
                                    </svg>
                                }
                                suffix={
                                    search && (
                                        <button
                                            type="button"
                                            onClick={() =>
                                                handleSearchChange("")
                                            }
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
                                                <path d="M18 6 6 18"/>
                                                <path d="m6 6 12 12"/>
                                            </svg>
                                        </button>
                                    )
                                }
                            />
                        </div>
                    </div>

                    <div className="max-w-full overflow-x-auto">
                        <table className="w-full">
                            <thead>
                            <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Thời gian
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Người dùng
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Request
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    IP
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Thao tác
                                </th>
                            </tr>
                            </thead>

                            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                            {loading ? (
                                <tr>
                                    <td
                                        colSpan="5"
                                        className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                    >
                                        Đang tải dữ liệu...
                                    </td>
                                </tr>
                            ) : items.length > 0 ? (
                                items.map((item) => (
                                    <tr
                                        key={item.id}
                                        className="transition hover:bg-gray-50 dark:hover:bg-white/[0.02]"
                                    >
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {formatDateTime(item.createdDate)}
                                        </td>
                                        <td className="px-5 py-4">
                                            <div className="font-medium text-gray-800 dark:text-white/90">
                                                {item.username || "Không xác định"}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4">
                                            <div className="flex items-center gap-2">
                                                <Badge
                                                    size="sm"
                                                    color={methodColor(item.method)}
                                                >
                                                    {item.method || "N/A"}
                                                </Badge>
                                                <span className="max-w-xl truncate text-sm text-gray-500 dark:text-gray-400">
                                                    {item.url || "Không có URL"}
                                                </span>
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.ipAddress || "-"}
                                        </td>
                                        <td className="px-5 py-4 text-center">
                                            <button
                                                type="button"
                                                onClick={() => setSelectedLog(item)}
                                                className="inline-flex h-8 min-w-8 items-center justify-center rounded-lg px-2 text-xs font-medium text-gray-500 transition hover:bg-gray-100 hover:text-brand-500 dark:text-gray-400 dark:hover:bg-white/[0.05]"
                                            >
                                                Chi tiết
                                            </button>
                                        </td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td
                                        colSpan="5"
                                        className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                    >
                                        Không có dữ liệu nhật ký hệ thống.
                                    </td>
                                </tr>
                            )}
                            </tbody>
                        </table>
                    </div>

                    <PaginationFooter
                        pageCurrent={pageCurrent}
                        pageSize={pageSize}
                        totalCount={totalCount}
                        loading={loading}
                        onPageChange={setPageCurrent}
                    />
                </div>
            </div>

            <Modal
                isOpen={Boolean(selectedLog)}
                onClose={() => setSelectedLog(null)}
                className="max-w-3xl p-6"
            >
                <div className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            Chi tiết nhật ký
                        </h4>
                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            {selectedLog && formatDateTime(selectedLog.createdDate)}
                        </p>
                    </div>

                    <div className="grid gap-4 sm:grid-cols-2">
                        <div>
                            <Label>Người dùng</Label>
                            <div className="text-sm text-gray-700 dark:text-gray-300">
                                {selectedLog?.username || "Không xác định"}
                            </div>
                        </div>
                        <div>
                            <Label>IP</Label>
                            <div className="text-sm text-gray-700 dark:text-gray-300">
                                {selectedLog?.ipAddress || "-"}
                            </div>
                        </div>
                        <div>
                            <Label>Method</Label>
                            <div>
                                <Badge
                                    size="sm"
                                    color={methodColor(selectedLog?.method)}
                                >
                                    {selectedLog?.method || "N/A"}
                                </Badge>
                            </div>
                        </div>
                        <div>
                            <Label>URL</Label>
                            <div className="break-all text-sm text-gray-700 dark:text-gray-300">
                                {selectedLog?.url || "-"}
                            </div>
                        </div>
                    </div>

                    <div>
                        <Label>Request</Label>
                        <pre className="max-h-96 overflow-auto rounded-lg border border-gray-200 bg-gray-50 p-4 text-xs text-gray-700 dark:border-gray-800 dark:bg-gray-900 dark:text-gray-300">
                            {prettyRequest || "Không có dữ liệu request."}
                        </pre>
                    </div>
                </div>
            </Modal>
        </div>
    );
}

