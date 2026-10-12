import BasicTableTwo from "../../../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {useCallback, useEffect, useMemo, useState} from "react";
import {useNavigate} from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import TrangThaiBadge from "../../../shared/components/TrangThaiBadge.jsx";
import {
    getDanhSachDangKyXayDungVanBan,
} from "../api/dangKyXayDungVanBanApi.js";

const COMPLETED_STATUS_CODE = "HOAN_THANH";

const toError = (error) =>
    error?.response?.data?.message ||
    error?.response?.data?.title ||
    (typeof error?.response?.data === "string"
        ? error.response.data
        : null) ||
    error?.message ||
    "Không thể tải danh sách đăng ký.";

export default function DanhSachDangKyPage() {
    const navigate = useNavigate();

    const [data, setData] = useState([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(true);

    const loadData = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await getDanhSachDangKyXayDungVanBan({
                pageSize: 200,
            });

            setData(
                (result?.items || []).filter(
                    (item) => item.maTrangThaiHoSo === COMPLETED_STATUS_CODE
                )
            );
        } catch (err) {
            setError(toError(err));
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        void loadData();
    }, [loadData]);

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
                cellClassName: "px-5 py-4 text-center",
                render: (item) => (
                    <TrangThaiBadge
                        tenTrangThai={item.tenTrangThaiHoSo}
                        maMauHex={item.mauTrangThaiHoSo}
                    />
                ),
            },
            {
                key: "createdAt",
                header: "Ngày tạo",
                render: (item) => (
                    <span className="text-sm text-gray-600 dark:text-gray-300">
                        {item.createdAt
                            ? new Date(item.createdAt).toLocaleDateString(
                                "vi-VN"
                            )
                            : "-"}
                    </span>
                ),
            },
            {
                key: "actions",
                header: "Thao tác",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName: "px-5 py-4 text-center",
                render: (item) => (
                    <div className="flex items-center justify-center gap-2">
                        <button
                            type="button"
                            title="Xem chi tiết"
                            onClick={() =>
                                navigate(
                                    `/dang-ky-xay-dung-van-ban/ho-so/${item.id}?mode=view`
                                )
                            }
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
                                <circle cx="12" cy="12" r="3"/>
                            </svg>
                        </button>
                    </div>
                ),
            },
        ],
        [navigate]
    );

    const searchFields = useMemo(
        () => [
            (item) => item.maHoSo,
            (item) => item.tenHoSo,
            (item) => item.tenVanBanDuKien,
            (item) => item.namDangKy,
            (item) => item.tenTrangThaiHoSo,
        ],
        []
    );

    return (
        <div>
            {/* Tiêu đề trang */}
            <div className="mb-5">
                <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
                    Danh sách đăng ký
                </h1>

                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                    Chỉ hiển thị các hồ sơ đăng ký đã hoàn thành.
                </p>
            </div>

            {/* Lỗi API */}
            {error && (
                <div className="mb-5">
                    <Alert
                        variant="error"
                        title="Không thể tải dữ liệu"
                        message={error}
                    />
                </div>
            )}

            {/* Bảng danh sách */}
            {loading ? (
                <div
                    className="rounded-xl border border-gray-200 bg-white px-5 py-12 text-center text-sm text-gray-500 dark:border-white/[0.05] dark:bg-white/[0.03] dark:text-gray-400">
                    Đang tải dữ liệu đăng ký...
                </div>
            ) : (
                <BasicTableTwo
                    data={data}
                    columns={columns}
                    searchPlaceholder="Tìm kiếm hồ sơ..."
                    searchFields={searchFields}
                    pageSizeOptions={[5, 10, 20, 100]}
                    emptyText="Không có hồ sơ đăng ký đã hoàn thành."
                />
            )}
        </div>
    );
}

