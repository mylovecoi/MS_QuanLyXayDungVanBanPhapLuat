import Badge from "../../../app/components/ui/badge/Badge.jsx";
import BasicTableTwo from "../../../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {useCallback, useEffect, useMemo, useState} from "react";
import {useNavigate} from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import {
    capNhatKetQuaPheDuyet,
    getDangKyXayDungVanBanById,
    getDangKyXayDungVanBanFiles,
    getDangKyXayDungVanBanTimeline,
    getHanhDongKhaDung,
    getKetQuaDangKyXayDungVanBan,
    xuLyDangKyXayDungVanBan,
} from "../api/dangKyXayDungVanBanApi.js";

const ACTION_CODES = {
    approve: "PHE_DUYET",
    return: "TRA_LAI",
    complete: "HOAN_THANH",
};

const ACTION_LABELS = {
    approve: "Phê duyệt",
    return: "Trả lại",
    updateDecision: "Cập nhật quyết định",
    complete: "Hoàn thành quy trình",
};

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

const formatDate = (value) =>
    value ? new Date(value).toLocaleDateString("vi-VN") : "-";

const emptyToNull = (value) => {
    const text = String(value || "").trim();
    return text ? text : null;
};

function readUserInfo() {
    try {
        return JSON.parse(localStorage.getItem("userInfo") || "{}");
    } catch {
        return {};
    }
}

function DetailItem({label, value}) {
    return (
        <div>
            <div className="text-xs font-medium uppercase text-gray-500 dark:text-gray-400">
                {label}
            </div>
            <div className="mt-1 text-sm text-gray-800 dark:text-white/90">
                {value || "-"}
            </div>
        </div>
    );
}

export default function KetQuaDangKyPage() {
    const navigate = useNavigate();
    const [data, setData] = useState([]);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(true);
    const [processingId, setProcessingId] = useState("");

    const [selected, setSelected] = useState(null);
    const [detail, setDetail] = useState(null);
    const [timeline, setTimeline] = useState([]);
    const [files, setFiles] = useState([]);
    const [actions, setActions] = useState([]);
    const [detailLoading, setDetailLoading] = useState(false);

    const [modal, setModal] = useState(null);
    const [returnReason, setReturnReason] = useState("");
    const [actionNote, setActionNote] = useState("");
    const [decisionForm, setDecisionForm] = useState({
        ketQua: "Đã phê duyệt",
        soVanBan: "",
        ngayVanBan: "",
        coQuanPheDuyetId: "",
        tenCoQuanPheDuyet: "",
        nguoiKy: "",
        chucVuNguoiKy: "",
        noiDungKetQua: "",
        fileKetQuaId: "",
    });

    const userInfo = useMemo(() => readUserInfo(), []);

    const load = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await getKetQuaDangKyXayDungVanBan({
                pageSize: 200,
            });

            setData(result.items || []);
        } catch (err) {
            setError(errorMessage(err, "Không thể tải kết quả đăng ký."));
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        void load();
    }, [load]);

    const loadDetail = useCallback(async (item) => {
        try {
            setDetailLoading(true);
            setError("");
            setSelected(item);

            const [detailResult, timelineResult, actionsResult, filesResult] =
                await Promise.all([
                    getDangKyXayDungVanBanById(item.id),
                    getDangKyXayDungVanBanTimeline(item.id),
                    getHanhDongKhaDung(item.id),
                    getDangKyXayDungVanBanFiles(item.id),
                ]);

            setDetail(detailResult);
            setTimeline(timelineResult || []);
            setActions(actionsResult || []);
            setFiles(filesResult || []);

            return {
                detail: detailResult,
                actions: actionsResult || [],
                files: filesResult || [],
            };
        } catch (err) {
            setError(errorMessage(err, "Không thể tải dữ liệu hồ sơ."));
            return null;
        } finally {
            setDetailLoading(false);
        }
    }, []);

    const refreshAfterAction = useCallback(async (itemId) => {
        await load();

        const current = data.find((item) => item.id === itemId) || selected;
        if (current) {
            await loadDetail(current);
        }
    }, [data, load, loadDetail, selected]);

    const openData = useCallback((item) => {
        navigate(`/dang-ky-xay-dung-van-ban/ho-so/${item.id}?mode=view`);
    }, [navigate]);

    const openAction = useCallback(async (item, type) => {
        const loaded = await loadDetail(item);
        if (!loaded) {
            return;
        }

        setReturnReason("");
        setActionNote("");
        setDecisionForm({
            ketQua: "Đã phê duyệt",
            soVanBan: "",
            ngayVanBan: new Date().toISOString().slice(0, 10),
            coQuanPheDuyetId:
                userInfo.donViId ||
                loaded.detail?.donViPheDuyetId ||
                "",
            tenCoQuanPheDuyet: userInfo.tenDonVi || userInfo.donViTen || "",
            nguoiKy: "",
            chucVuNguoiKy: "",
            noiDungKetQua: "",
            fileKetQuaId: "",
        });
        setModal({type, item, detail: loaded.detail, actions: loaded.actions});
    }, [loadDetail, userInfo]);

    const findAction = useCallback((type) => {
        const code = ACTION_CODES[type];
        return (modal?.actions || actions).find(
            (action) => action.maHanhDong === code
        );
    }, [actions, modal]);

    const buildActionPayload = useCallback((action, item, currentDetail, note, reason = null) => ({
        hanhDongId: action.hanhDongId || action.id,
        noiDungXuLy: note,
        lyDoTraLai: reason,
        nguoiXuLyId: userInfo.userId,
        tenNguoiXuLy: userInfo.displayName || userInfo.username || "",
        donViXuLyId:
            userInfo.donViId ||
            currentDetail?.donViPheDuyetId ||
            currentDetail?.donViSoanThaoId ||
            item?.donViPheDuyetId ||
            item?.donViSoanThaoId,
        tenDonViXuLy: userInfo.tenDonVi || userInfo.donViTen || "",
        tenBuocTu: currentDetail?.tenBuocHienTai || item?.tenBuocHienTai,
        tenBuocDen: null,
        tenTrangThaiTruoc: currentDetail?.tenTrangThaiHoSo || item?.tenTrangThaiHoSo,
        tenTrangThaiSau: null,
    }), [userInfo]);

    const handleSubmitAction = useCallback(async (event) => {
        event.preventDefault();

        if (!modal) {
            return;
        }

        const currentDetail = modal.detail || detail;
        const item = modal.item || selected;

        try {
            setProcessingId(item.id);
            setError("");
            setSuccess("");

            if (modal.type === "updateDecision") {
                if (!decisionForm.ketQua.trim()) {
                    throw new Error("Vui lòng nhập kết quả phê duyệt.");
                }

                if (!decisionForm.coQuanPheDuyetId) {
                    throw new Error("Không xác định được cơ quan phê duyệt.");
                }

                await capNhatKetQuaPheDuyet(item.id, {
                    ketQua: decisionForm.ketQua.trim(),
                    soVanBan: emptyToNull(decisionForm.soVanBan),
                    ngayVanBan: decisionForm.ngayVanBan || null,
                    coQuanPheDuyetId: decisionForm.coQuanPheDuyetId,
                    tenCoQuanPheDuyet:
                        emptyToNull(decisionForm.tenCoQuanPheDuyet) ||
                        userInfo.tenDonVi ||
                        userInfo.donViTen ||
                        "Cơ quan phê duyệt",
                    nguoiKy: emptyToNull(decisionForm.nguoiKy),
                    chucVuNguoiKy: emptyToNull(decisionForm.chucVuNguoiKy),
                    noiDungKetQua: emptyToNull(decisionForm.noiDungKetQua),
                    fileKetQuaId: decisionForm.fileKetQuaId || null,
                });
                setSuccess("Đã cập nhật quyết định/kết quả phê duyệt.");
            } else {
                const action = findAction(modal.type);

                if (!action) {
                    throw new Error("Hồ sơ chưa có hành động phù hợp.");
                }

                if (modal.type === "return" && !returnReason.trim()) {
                    throw new Error("Vui lòng nhập lý do trả lại.");
                }

                const note = actionNote.trim() ||
                    (modal.type === "approve"
                        ? "Phê duyệt hồ sơ đăng ký"
                        : modal.type === "complete"
                            ? "Hoàn thành quy trình đăng ký"
                            : "Trả lại hồ sơ đăng ký");

                await xuLyDangKyXayDungVanBan(
                    item.id,
                    buildActionPayload(
                        action,
                        item,
                        currentDetail,
                        note,
                        modal.type === "return" ? returnReason.trim() : null
                    )
                );

                setSuccess(
                    modal.type === "approve"
                        ? "Đã phê duyệt hồ sơ."
                        : modal.type === "complete"
                            ? "Đã hoàn thành quy trình."
                            : "Đã trả lại hồ sơ."
                );
            }

            setModal(null);
            await refreshAfterAction(item.id);
        } catch (err) {
            setError(errorMessage(err, "Không thể xử lý hồ sơ."));
        } finally {
            setProcessingId("");
        }
    }, [
        actionNote,
        buildActionPayload,
        decisionForm,
        detail,
        findAction,
        modal,
        refreshAfterAction,
        returnReason,
        selected,
        userInfo,
    ]);

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
                key: "trangThai",
                header: "Trạng thái",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName: "px-5 py-4 text-center",
                render: (item) => (
                    <Badge size="sm" color={badgeColor(item.mauTrangThaiHoSo)}>
                        {item.tenTrangThaiHoSo || item.maTrangThaiHoSo || "-"}
                    </Badge>
                ),
            },
            {
                key: "ketQua",
                header: "Kết quả",
                render: (item) => (
                    <span className="text-sm text-gray-700 dark:text-gray-300">
                        {item.ketQua || "-"}
                    </span>
                ),
            },
            {
                key: "ngayKetQua",
                header: "Ngày kết quả",
                render: (item) => (
                    <span className="text-sm text-gray-600 dark:text-gray-300">
                        {formatDate(item.ngayKetQua)}
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
                    const busy = processingId === item.id;

                    return (
                        <div className="flex flex-wrap items-center justify-center gap-1">
                            <button
                                type="button"
                                onClick={() => openData(item)}
                                className={`${actionButtonClass} text-gray-600 hover:text-brand-500`}
                            >
                                Xem dữ liệu
                            </button>

                            {item.maTrangThaiHoSo === "DA_TRINH_PHE_DUYET" && (
                                <>
                                    <button
                                        type="button"
                                        disabled={busy}
                                        onClick={() => void openAction(item, "return")}
                                        className={`${actionButtonClass} text-error-500`}
                                    >
                                        Trả lại
                                    </button>
                                    <button
                                        type="button"
                                        disabled={busy}
                                        onClick={() => void openAction(item, "approve")}
                                        className={`${actionButtonClass} text-success-600`}
                                    >
                                        Phê duyệt
                                    </button>
                                </>
                            )}

                            {item.maTrangThaiHoSo === "DA_PHE_DUYET" && (
                                <button
                                    type="button"
                                    disabled={busy}
                                    onClick={() => void openAction(item, "updateDecision")}
                                    className={`${actionButtonClass} text-brand-500`}
                                >
                                    Cập quyết định
                                </button>
                            )}

                            {item.maTrangThaiHoSo === "DA_CAP_NHAT_KET_QUA" && (
                                <button
                                    type="button"
                                    disabled={busy}
                                    onClick={() => void openAction(item, "complete")}
                                    className={`${actionButtonClass} text-success-600`}
                                >
                                    Hoàn thành
                                </button>
                            )}
                        </div>
                    );
                },
            },
        ],
        [actionButtonClass, openAction, openData, processingId]
    );

    return (
        <div>
            <div className="mb-5">
                <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
                    Kết quả đăng ký
                </h1>

                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                    Xử lý trả lại, cập nhật quyết định và hoàn thành quy trình đăng ký xây dựng văn bản
                </p>
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
                searchPlaceholder="Tìm kiếm kết quả..."
                emptyText={
                    loading
                        ? "Đang tải dữ liệu..."
                        : "Không có hồ sơ cần xử lý kết quả."
                }
                searchFields={[
                    (item) => item.maHoSo,
                    (item) => item.tenHoSo,
                    (item) => item.tenVanBanDuKien,
                    (item) => item.namDangKy,
                    (item) => item.ketQua,
                    (item) => item.tenTrangThaiHoSo,
                ]}
            />

            {modal && (
                <div className="fixed inset-0 z-99999 flex items-center justify-center bg-gray-900/40 px-4 py-6">
                    <div className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-5 shadow-xl dark:bg-gray-900">
                        <div className="flex items-start justify-between gap-4">
                            <div>
                                <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                                    {ACTION_LABELS[modal.type]}
                                </h2>
                                <p className="mt-1 text-sm text-gray-500">
                                    {modal.item?.maHoSo} · {modal.item?.tenHoSo}
                                </p>
                            </div>
                            <button
                                type="button"
                                onClick={() => setModal(null)}
                                className="rounded-lg px-3 py-2 text-sm text-gray-500 hover:bg-gray-100 dark:hover:bg-white/[0.05]"
                            >
                                Đóng
                            </button>
                        </div>

                        <form className="mt-5 space-y-4" onSubmit={handleSubmitAction}>
                            {modal.type === "return" && (
                                <div>
                                    <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                        Lý do trả lại *
                                    </label>
                                    <textarea
                                        value={returnReason}
                                        onChange={(event) => setReturnReason(event.target.value)}
                                        rows={4}
                                        className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                    />
                                </div>
                            )}

                            {(modal.type === "approve" || modal.type === "complete") && (
                                <div>
                                    <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                        Nội dung xử lý
                                    </label>
                                    <textarea
                                        value={actionNote}
                                        onChange={(event) => setActionNote(event.target.value)}
                                        rows={3}
                                        placeholder={
                                            modal.type === "approve"
                                                ? "Phê duyệt hồ sơ đăng ký"
                                                : "Hoàn thành quy trình đăng ký"
                                        }
                                        className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                    />
                                </div>
                            )}

                            {modal.type === "updateDecision" && (
                                <div className="grid gap-4 md:grid-cols-2">
                                    <div>
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            Kết quả *
                                        </label>
                                        <input
                                            value={decisionForm.ketQua}
                                            onChange={(event) => setDecisionForm((current) => ({...current, ketQua: event.target.value}))}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        />
                                    </div>
                                    <div>
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            Số quyết định
                                        </label>
                                        <input
                                            value={decisionForm.soVanBan}
                                            onChange={(event) => setDecisionForm((current) => ({...current, soVanBan: event.target.value}))}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        />
                                    </div>
                                    <div>
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            Ngày quyết định
                                        </label>
                                        <input
                                            type="date"
                                            value={decisionForm.ngayVanBan}
                                            onChange={(event) => setDecisionForm((current) => ({...current, ngayVanBan: event.target.value}))}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        />
                                    </div>
                                    <div>
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            Cơ quan phê duyệt
                                        </label>
                                        <input
                                            value={decisionForm.tenCoQuanPheDuyet}
                                            onChange={(event) => setDecisionForm((current) => ({...current, tenCoQuanPheDuyet: event.target.value}))}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        />
                                    </div>
                                    <div>
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            Người ký
                                        </label>
                                        <input
                                            value={decisionForm.nguoiKy}
                                            onChange={(event) => setDecisionForm((current) => ({...current, nguoiKy: event.target.value}))}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        />
                                    </div>
                                    <div>
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            Chức vụ người ký
                                        </label>
                                        <input
                                            value={decisionForm.chucVuNguoiKy}
                                            onChange={(event) => setDecisionForm((current) => ({...current, chucVuNguoiKy: event.target.value}))}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        />
                                    </div>
                                    <div className="md:col-span-2">
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            File quyết định
                                        </label>
                                        <select
                                            value={decisionForm.fileKetQuaId}
                                            onChange={(event) => setDecisionForm((current) => ({...current, fileKetQuaId: event.target.value}))}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        >
                                            <option value="">Không chọn file</option>
                                            {files.map((file) => (
                                                <option key={file.id} value={file.id}>
                                                    {file.tenFile}
                                                </option>
                                            ))}
                                        </select>
                                    </div>
                                    <div className="md:col-span-2">
                                        <label className="mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300">
                                            Nội dung kết quả
                                        </label>
                                        <textarea
                                            value={decisionForm.noiDungKetQua}
                                            onChange={(event) => setDecisionForm((current) => ({...current, noiDungKetQua: event.target.value}))}
                                            rows={4}
                                            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 dark:border-gray-700 dark:bg-gray-900 dark:text-white"
                                        />
                                    </div>
                                </div>
                            )}

                            <div className="flex justify-end gap-2 pt-2">
                                <button
                                    type="button"
                                    onClick={() => setModal(null)}
                                    className="rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                                >
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    disabled={Boolean(processingId)}
                                    className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white transition hover:bg-brand-600 disabled:cursor-not-allowed disabled:opacity-60"
                                >
                                    Lưu
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}
