import { useEffect, useMemo, useState } from "react";
import { Dropdown } from "../ui/dropdown/Dropdown.jsx";
import { DropdownItem } from "../ui/dropdown/DropdownItem.jsx";
import { Link } from "react-router";
import { danhDauDaXemCanhBao, danhDauDaXemNhacViec, getCanhBao, getNhacViecCuaToi } from "../../../features/khai-thac-du-lieu/api/canhBaoApi";

const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "";

export default function NotificationDropdown() {
    const [isOpen, setIsOpen] = useState(false);
    const [alerts, setAlerts] = useState([]);
    const [totalUnread, setTotalUnread] = useState(0);
    const [error, setError] = useState("");
    const notifying = totalUnread > 0;

    const visibleAlerts = useMemo(() => alerts.slice(0, 5), [alerts]);

    const loadAlerts = async () => {
        try {
            setError("");
            const [alertResponse, reminders] = await Promise.all([getCanhBao({
                trangThaiXuLy: "MOI",
                pageSize: 5,
                pageCurrent: 1,
            }), getNhacViecCuaToi()]);
            const alertItems = alertResponse?.items ?? alertResponse?.data ?? [];
            const reminderItems = Array.isArray(reminders) ? reminders : [];
            const merged = [
                ...alertItems.map((item) => ({ ...item, loaiThongBao: "CANH_BAO" })),
                ...reminderItems.map((item) => ({
                    ...item,
                    loaiThongBao: "NHAC_VIEC",
                    tieuDe: item.tieuDe,
                    noiDung: item.noiDung,
                    mucDo: item.mucDoUuTien,
                    ngayPhatSinh: item.ngayGui,
                })),
            ].sort((a, b) => new Date(b.ngayPhatSinh || 0) - new Date(a.ngayPhatSinh || 0));
            setAlerts(merged);
            setTotalUnread((alertResponse?.totalCount ?? alertItems.length ?? 0) + reminderItems.length);
        } catch {
            setAlerts([]);
            setTotalUnread(0);
            setError("Không thể tải cảnh báo.");
        }
    };

    useEffect(() => {
        loadAlerts();
        const interval = window.setInterval(loadAlerts, 60000);
        return () => window.clearInterval(interval);
    }, []);

    const toggleDropdown = () => {
        setIsOpen(!isOpen);
    };

    const closeDropdown = () => {
        setIsOpen(false);
    };

    const handleClick = () => {
        toggleDropdown();
        if (!isOpen) {
            loadAlerts();
        }
    };

    const notificationUrl = (alert) => {
        const canhBaoId = alert.loaiThongBao === "NHAC_VIEC" ? alert.canhBaoId : alert.id;
        return `/canh-bao/thong-minh?canhBaoId=${canhBaoId}`;
    };

    const handleNotificationClick = (alert) => {
        if (alert.loaiThongBao === "NHAC_VIEC" && alert.canhBaoId && alert.id) {
            danhDauDaXemNhacViec(alert.canhBaoId, alert.id).catch(() => {});
        }
        if (alert.loaiThongBao === "CANH_BAO" && alert.id) {
            danhDauDaXemCanhBao(alert.id).catch(() => {});
        }
        setAlerts((current) => current.filter((item) => !(item.loaiThongBao === alert.loaiThongBao && item.id === alert.id)));
        setTotalUnread((current) => Math.max(0, current - 1));
        closeDropdown();
    };

    return (
        <div className="relative">
            <button
                type="button"
                className="modern-icon-button relative flex h-11 w-11 items-center justify-center rounded-2xl border border-[var(--admin-border)] bg-white text-gray-500 transition-colors hover:bg-brand-50 hover:text-brand-600 dark:bg-white/[0.04] dark:text-gray-400 dark:hover:bg-white/[0.08] dark:hover:text-white"
                onClick={handleClick}
            >
                {notifying && (
                    <span className="absolute -right-1 -top-1 z-10 flex min-h-5 min-w-5 items-center justify-center rounded-full bg-orange-500 px-1.5 text-[10px] font-semibold text-white">
                        {totalUnread > 9 ? "9+" : totalUnread}
                        <span className="absolute inline-flex h-full w-full rounded-full bg-orange-400 opacity-50 animate-ping" />
                    </span>
                )}

                <svg
                    className="fill-current"
                    width="20"
                    height="20"
                    viewBox="0 0 20 20"
                    xmlns="http://www.w3.org/2000/svg"
                >
                    <path
                        fillRule="evenodd"
                        clipRule="evenodd"
                        d="M10.75 2.29248C10.75 1.87827 10.4143 1.54248 10 1.54248C9.58583 1.54248 9.25004 1.87827 9.25004 2.29248V2.83613C6.08266 3.20733 3.62504 5.9004 3.62504 9.16748V14.4591H3.33337C2.91916 14.4591 2.58337 14.7949 2.58337 15.2091C2.58337 15.6234 2.91916 15.9591 3.33337 15.9591H4.37504H15.625H16.6667C17.0809 15.9591 17.4167 15.6234 16.6667 14.4591H16.375V9.16748C16.375 5.9004 13.9174 3.20733 10.75 2.83613V2.29248ZM14.875 14.4591V9.16748C14.875 6.47509 12.6924 4.29248 10 4.29248C7.30765 4.29248 5.12504 6.47509 5.12504 9.16748V14.4591H14.875ZM8.00004 17.7085C8.00004 18.1228 8.33583 18.4585 8.75004 18.4585H11.25C11.6643 18.4585 12 18.1228 12 17.7085C12 17.2943 11.6643 16.9585 11.25 16.9585H8.75004C8.33583 16.9585 8.00004 17.2943 8.00004 17.7085Z"
                        fill="currentColor"
                    />
                </svg>
            </button>

            <Dropdown
                isOpen={isOpen}
                onClose={closeDropdown}
                className="absolute -right-[240px] mt-[17px] flex h-[480px] w-[350px] flex-col rounded-[var(--admin-radius-lg)] border border-[var(--admin-border)] bg-white p-3 shadow-[var(--admin-shadow-panel)] dark:bg-gray-dark sm:w-[361px] lg:right-0"
            >
                <div className="flex items-center justify-between pb-3 mb-3 border-b border-gray-100 dark:border-gray-700">
                    <h5 className="text-lg font-semibold text-gray-800 dark:text-gray-200">
                        Cảnh báo
                    </h5>

                    <button
                        type="button"
                        onClick={closeDropdown}
                        className="text-gray-500 transition dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"
                    >
                        <svg
                            className="fill-current"
                            width="24"
                            height="24"
                            viewBox="0 0 24 24"
                        >
                            <path
                                fillRule="evenodd"
                                clipRule="evenodd"
                                d="M6.21967 7.28131C5.92678 6.98841 5.92678 6.51354 6.21967 6.22065C6.51256 5.92775 6.98744 5.92775 7.28033 6.22065L11.999 10.9393L16.7176 6.22078C17.0105 5.92789 17.4854 5.92788 17.7782 6.22078C18.0711 6.51367 18.0711 6.98855 17.7782 7.28144L13.0597 12L17.7782 16.7186C18.0711 17.0115 18.0711 17.4863 17.7782 17.7792C17.4854 18.0722 17.0105 18.0722 16.7176 17.7792L11.999 13.0607L7.28033 17.7794C6.98744 18.0722 6.51256 18.0722 6.21967 17.7794C5.92678 17.4865 5.92678 17.0116 6.21967 16.7187L10.9384 12L6.21967 7.28131Z"
                                fill="currentColor"
                            />
                        </svg>
                    </button>
                </div>

                <ul className="flex flex-col h-auto overflow-y-auto custom-scrollbar">
                    {error ? (
                        <li className="px-3 py-8 text-center text-sm text-gray-500 dark:text-gray-400">{error}</li>
                    ) : visibleAlerts.length ? visibleAlerts.map((alert) => (
                        <li key={`${alert.loaiThongBao}-${alert.id}`}>
                            <DropdownItem
                                tag="a"
                                to={notificationUrl(alert)}
                                onItemClick={() => handleNotificationClick(alert)}
                                className="flex gap-3 rounded-lg border-b border-gray-100 p-3 hover:bg-gray-100 dark:border-gray-800 dark:hover:bg-white/5"
                            >
                                <span className="mt-1 flex h-9 w-9 shrink-0 items-center justify-center rounded-2xl bg-orange-50 text-orange-600 dark:bg-orange-500/10">
                                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <path d="M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0Z" />
                                        <path d="M12 9v4" />
                                        <path d="M12 17h.01" />
                                    </svg>
                                </span>

                                <span className="block min-w-0">
                                    <span className="mb-1 block line-clamp-2 text-sm font-medium text-gray-800 dark:text-white/90">
                                        {alert.tieuDe}
                                    </span>
                                    <span className="line-clamp-2 text-xs text-gray-500 dark:text-gray-400">
                                        {alert.noiDung}
                                    </span>
                                    <span className="mt-2 flex items-center gap-2 text-xs text-gray-500 dark:text-gray-400">
                                        <span>{alert.loaiThongBao === "NHAC_VIEC" ? "Nhắc việc" : "Cảnh báo"}</span>
                                        <span className="h-1 w-1 rounded-full bg-gray-400" />
                                        <span>{alert.mucDo}</span>
                                        <span className="h-1 w-1 rounded-full bg-gray-400" />
                                        <span>{formatDate(alert.ngayPhatSinh)}</span>
                                    </span>
                                </span>
                            </DropdownItem>
                        </li>
                    )) : (
                        <li className="px-3 py-8 text-center text-sm text-gray-500 dark:text-gray-400">Không có cảnh báo mới.</li>
                    )}
                </ul>

                <Link
                    to="/canh-bao/thong-minh"
                    onClick={closeDropdown}
                    className="block px-4 py-2 mt-3 text-sm font-medium text-center text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-100 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700"
                >
                    Xem tất cả cảnh báo
                </Link>
            </Dropdown>
        </div>
    );
}
