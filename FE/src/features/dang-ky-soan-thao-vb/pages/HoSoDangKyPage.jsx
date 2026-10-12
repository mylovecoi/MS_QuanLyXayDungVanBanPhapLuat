import Badge from "../../../app/components/ui/badge/Badge.jsx";
import BasicTableTwo from "../../../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {useCallback, useEffect, useMemo, useState} from "react";
import {useNavigate, useSearchParams} from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import {
    deleteDangKyXayDungVanBan,
    getDanhSachDangKyXayDungVanBan,
    getHanhDongKhaDung,
    xuLyDangKyXayDungVanBan,
} from "../api/dangKyXayDungVanBanApi.js";

const SEND_ACTION_CODE = "TRINH_PHE_DUYET";
const EDITABLE_STATUS_CODES = new Set(["DANG_SOAN_THAO", "BI_TRA_LAI"]);

const badgeColor = (color) => ({
    blue: "primary",
    green: "success",
    red: "error",
    orange: "warning",
    cyan: "info",
    purple: "primary",
    gray: "light",
}[color] || "primary");

const errorMessage = (error, fallback) =>
    error?.response?.data?.message ||
    error?.message ||
    fallback;

function readUserInfo() {
    try {
        return JSON.parse(localStorage.getItem("userInfo") || "{}");
    } catch {
        return {};
    }
}

export default function HoSoDangKyPage() {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();

    const [data, setData] = useState([]);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(true);
    const [processingId, setProcessingId] = useState("");

    const donViId = searchParams.get("donViId");
    const userInfo = useMemo(() => readUserInfo(), []);

    const load = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await getDanhSachDangKyXayDungVanBan({
                pageSize: 200,
                ...(donViId ? {donViId} : {}),
            });

            setData(result.items || []);
        } catch (err) {
            setError(errorMessage(err, "Không thể tải hồ sơ đăng ký."));
        } finally {
            setLoading(false);
        }
    }, [donViId]);

    useEffect(() => {
        void load();
    }, [load]);

    const openView = useCallback((item) => {
        navigate(`/dang-ky-xay-dung-van-ban/ho-so/${item.id}?mode=view`);
    }, [navigate]);

    const openEdit = useCallback((item) => {
        navigate(`/dang-ky-xay-dung-van-ban/ho-so/${item.id}`);
    }, [navigate]);

    const canEditItem = useCallback(
        (item) => EDITABLE_STATUS_CODES.has(item.maTrangThaiHoSo),
        []
    );

    const buildActionPayload = useCallback((item, action) => ({
        hanhDongId: action.hanhDongId || action.id,
        noiDungXuLy: "Trình phê duyệt hồ sơ đăng ký",
        lyDoTraLai: null,
        nguoiXuLyId: userInfo.userId,
        tenNguoiXuLy: userInfo.displayName || userInfo.username || "",
        donViXuLyId: userInfo.donViId || item.donViSoanThaoId,
        tenDonViXuLy: "",
        tenBuocTu: item.tenBuocHienTai,
        tenBuocDen: null,
        tenTrangThaiTruoc: item.tenTrangThaiHoSo,
        tenTrangThaiSau: null,
    }), [userInfo]);

    const sendItem = useCallback(async (item) => {
        const label = item.maHoSo || item.tenHoSo || "hồ sơ này";

        if (!window.confirm(`Gửi hồ sơ "${label}" sang đơn vị phê duyệt?`)) {
            return;
        }

        try {
            setProcessingId(item.id);
            setError("");
            setSuccess("");

            const actions = await getHanhDongKhaDung(item.id);
            const sendAction = actions.find(
                (action) => action.maHanhDong === SEND_ACTION_CODE
            );

            if (!sendAction) {
                throw new Error("Hồ sơ chưa có hành động gửi phù hợp.");
            }

            await xuLyDangKyXayDungVanBan(
                item.id,
                buildActionPayload(item, sendAction)
            );
            setSuccess("Đã gửi hồ sơ đăng ký.");
            await load();
        } catch (err) {
            setError(errorMessage(err, "Không thể gửi hồ sơ đăng ký."));
        } finally {
            setProcessingId("");
        }
    }, [buildActionPayload, load]);

    const deleteItem = useCallback(async (item) => {
        const label = item.maHoSo || item.tenHoSo || "hồ sơ này";

        if (!window.confirm(`Xóa hồ sơ "${label}"?`)) {
            return;
        }

        try {
            setProcessingId(item.id);
            setError("");
            setSuccess("");

            await deleteDangKyXayDungVanBan(item.id);
            setSuccess("Xóa hồ sơ đăng ký thành công.");
            await load();
        } catch (err) {
            setError(errorMessage(err, "Không thể xóa hồ sơ đăng ký."));
        } finally {
            setProcessingId("");
        }
    }, [load]);

    const actionButtonClass =
        "rounded-lg px-3 py-2 text-xs font-medium transition hover:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50 dark:hover:bg-white/[0.05]";

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
                    <Badge
                        size="sm"
                        color={badgeColor(item.mauTrangThaiHoSo)}
                    >
                        {item.tenTrangThaiHoSo ||
                            item.maTrangThaiHoSo ||
                            "-"}
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
                cellClassName: "px-5 py-4 text-center",
                render: (item) => {
                    const editable = canEditItem(item);
                    const busy = processingId === item.id;

                    return (
                        <div className="flex flex-wrap items-center justify-center gap-1">
                            <button
                                type="button"
                                onClick={() => openView(item)}
                                className={`${actionButtonClass} text-gray-600 hover:text-brand-500`}
                            >
                                Xem
                            </button>

                            {editable && (
                                <>
                                    <button
                                        type="button"
                                        onClick={() => openEdit(item)}
                                        className={`${actionButtonClass} text-brand-500`}
                                    >
                                        Sửa
                                    </button>
                                    <button
                                        type="button"
                                        onClick={() => void sendItem(item)}
                                        disabled={busy}
                                        className={`${actionButtonClass} text-success-600`}
                                    >
                                        Gửi
                                    </button>
                                    <button
                                        type="button"
                                        onClick={() => void deleteItem(item)}
                                        disabled={busy}
                                        className={`${actionButtonClass} text-error-500`}
                                    >
                                        Xóa
                                    </button>
                                </>
                            )}
                        </div>
                    );
                },
            },
        ],
        [
            actionButtonClass,
            canEditItem,
            deleteItem,
            openEdit,
            openView,
            processingId,
            sendItem,
        ]
    );

    return (
        <div>
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
                        Hồ sơ đăng ký
                    </h1>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý hồ sơ đăng ký xây dựng văn bản
                    </p>
                </div>

                <button
                    type="button"
                    onClick={() =>
                        navigate("/dang-ky-xay-dung-van-ban/ho-so/them-moi")
                    }
                    className="inline-flex items-center justify-center gap-2 rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-brand-600"
                >
                    <span className="text-lg leading-none">+</span>
                    Thêm mới
                </button>
            </div>

            {error && (
                <div className="mb-5">
                    <Alert
                        variant="error"
                        title="Không thể xử lý"
                        message={error}
                    />
                </div>
            )}

            {success && (
                <div className="mb-5">
                    <Alert
                        variant="success"
                        title="Hoàn tất"
                        message={success}
                    />
                </div>
            )}

            <BasicTableTwo
                data={data}
                columns={columns}
                searchPlaceholder="Tìm kiếm hồ sơ..."
                emptyText={
                    loading
                        ? "Đang tải dữ liệu..."
                        : "Không có hồ sơ đăng ký."
                }
                searchFields={[
                    (item) => item.maHoSo,
                    (item) => item.tenHoSo,
                    (item) => item.tenVanBanDuKien,
                    (item) => item.namDangKy,
                    (item) => item.tenTrangThaiHoSo,
                ]}
            />
        </div>
    );
}
