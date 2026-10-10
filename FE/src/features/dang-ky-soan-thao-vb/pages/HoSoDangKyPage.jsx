import Badge from "../../../app/components/ui/badge/Badge.jsx";
import BasicTableTwo from "../../../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {useEffect, useMemo, useState} from "react";
import {useNavigate, useSearchParams} from "react-router";
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

export default function HoSoDangKyPage() {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const [data, setData] = useState([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(true);
    const donViId = searchParams.get("donViId");

    useEffect(() => {
        let mounted = true;
        const load = async () => {
            try {
                setLoading(true);
                setError("");
                const result = await getDanhSachDangKyXayDungVanBan({
                    pageSize: 200,
                    ...(donViId ? {donViId} : {}),
                });
                if (mounted) {
                    setData(result.items || []);
                }
            } catch (err) {
                if (mounted) {
                    setError(err?.response?.data?.message || err?.message || "Không thể tải hồ sơ đăng ký.");
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
    }, [donViId]);

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
                    <Badge size="sm" color={badgeColor(item.mauTrangThaiHoSo)}>
                        {item.tenTrangThaiHoSo || item.maTrangThaiHoSo || "-"}
                    </Badge>
                ),
            },
            {
                key: "createdAt",
                header: "Ngày đăng ký",
                render: (item) => (
                    <span className="text-sm text-gray-600 dark:text-gray-300">
                        {item.createdAt
                            ? new Date(item.createdAt).toLocaleDateString("vi-VN")
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
                        <button
                            type="button"
                            onClick={() => navigate(`/dang-ky-xay-dung-van-ban/ho-so/${item.id}`)}
                            className="rounded-lg px-3 py-2 text-sm font-medium text-brand-500 transition hover:bg-brand-50 dark:hover:bg-white/[0.05]"
                        >
                            Xem
                        </button>
                    </div>
                ),
            },
        ],
        []
    );

    return (
        <div className="space-y-5">
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Hồ sơ đăng ký
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý hồ sơ đăng ký xây dựng văn bản
                    </p>
                </div>

                <button type="button" onClick={() => navigate("/dang-ky-xay-dung-van-ban/ho-so/them-moi")}
                        className="inline-flex items-center justify-center gap-2 rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-brand-600">
                    <span className="text-lg leading-none"> + </span>
                    Thêm mới
                </button>
            </div>

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
