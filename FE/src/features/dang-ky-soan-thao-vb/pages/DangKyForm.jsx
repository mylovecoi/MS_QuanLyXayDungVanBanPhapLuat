import {useEffect, useMemo, useState} from "react";
import {useNavigate, useParams, useSearchParams} from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge.jsx";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {getDonViOptions} from "../../danh-muc/api/donViApi";
import {getVanBans} from "../../danh-muc/api/vanBanApi";
import {getQuyTrinhById} from "../../danh-muc/api/quyTrinhSoanThaoApi";
import DatePicker from "../../../app/components/forms/date-picker.jsx";

import {
    createDangKyXayDungVanBan,
    deleteDangKyXayDungVanBan,
    getDangKyXayDungVanBanById,
    getDangKyXayDungVanBanTimeline,
    getHanhDongKhaDung,
    updateDangKyXayDungVanBan,
    xuLyDangKyXayDungVanBan,
} from "../api/dangKyXayDungVanBanApi";

const CURRENT_YEAR = new Date().getFullYear();
const EDITABLE_STATUS_CODES = new Set(["DANG_SOAN_THAO", "BI_TRA_LAI"]);
const COMPLETED_STATUS_CODES = new Set(["HOAN_THANH", "DA_CHUYEN_QUY_TRINH_XAY_DUNG"]);
const SEND_ACTION_CODE = "TRINH_PHE_DUYET";

const initialForm = {
    tenHoSo: "",
    tenVanBanDuKien: "",
    loaiVanBanId: "",
    donViSoanThaoId: "",
    donViPheDuyetId: "",
    namDangKy: CURRENT_YEAR,
    canCuDeXuat: "",
    suCanThiet: "",
    noiDungChinhSach: "",
    duKienThoiGianTrinh: "",
};

function getErrorMessage(error, fallback = "Không thể xử lý yêu cầu.") {
    const data = error?.response?.data;

    if (typeof data === "string") {
        return data;
    }

    return data?.message || error?.message || fallback;
}

function toDateInput(value) {
    return value ? String(value).slice(0, 10) : "";
}

function formatDateTime(value) {
    if (!value) {
        return "-";
    }

    return new Intl.DateTimeFormat("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    }).format(new Date(value));
}

function formatDate(value) {
    if (!value) {
        return "-";
    }

    return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}

function getStepStatusMeta(status) {
    if (status === "completed") {
        return {
            label: "Đã hoàn thành",
            badgeColor: "success",
            nodeClass:
                "border-success-500 bg-success-50 text-success-600 dark:bg-success-500/15 dark:text-success-500",
            lineClass: "bg-success-200 dark:bg-success-500/30",
        };
    }

    if (status === "current") {
        return {
            label: "Đang thực hiện",
            badgeColor: "warning",
            nodeClass:
                "border-warning-500 bg-warning-50 text-warning-600 dark:bg-warning-500/15 dark:text-warning-500",
            lineClass: "bg-gray-200 dark:bg-white/[0.08]",
        };
    }

    return {
        label: "Chưa thực hiện",
        badgeColor: "light",
        nodeClass:
            "border-gray-300 bg-gray-50 text-gray-500 dark:border-gray-700 dark:bg-white/[0.04] dark:text-gray-400",
        lineClass: "bg-gray-200 dark:bg-white/[0.08]",
    };
}

function emptyToNull(value) {
    return value?.trim() ? value.trim() : null;
}

function asOptions(items, valueKey, labelGetter) {
    return (items || []).map((item) => ({
        value: item[valueKey],
        label: labelGetter(item),
    }));
}

function Field({label, required, children}) {
    return (
        <div>
            <Label>
                {label}
                {required && <span className="text-error-500"> *</span>}
            </Label>

            <div className="mt-1.5">
                {children}
            </div>
        </div>
    );
}

function readUserInfo() {
    try {
        return JSON.parse(localStorage.getItem("userInfo") || "{}");
    } catch {
        return {};
    }
}

export default function DangKyForm() {
    const {id} = useParams();
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    const isEdit = Boolean(id);
    const isView = isEdit && searchParams.get("mode") === "view";

    const [form, setForm] = useState(initialForm);
    const [detail, setDetail] = useState(null);
    const [timeline, setTimeline] = useState([]);
    const [quyTrinh, setQuyTrinh] = useState(null);
    const [actions, setActions] = useState([]);

    const [vanBans, setVanBans] = useState([]);
    const [donVis, setDonVis] = useState([]);

    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const isEditable = isEdit
        ? EDITABLE_STATUS_CODES.has(detail?.maTrangThaiHoSo)
        : true;
    const isReadOnly = isView || (isEdit && !isEditable);
    const sendAction = actions.find((item) => item.maHanhDong === SEND_ACTION_CODE);

    const workflowSteps = useMemo(() => {
        const relatedStepIds = new Set([
            detail?.buocHienTaiId,
            ...timeline.flatMap((item) => [item.tuBuocId, item.denBuocId]),
        ].filter(Boolean));
        const stepByBusinessKey = new Map();

        for (const step of quyTrinh?.buocQuyTrinhs || []) {
            const key = String(step.maBuoc || step.id).trim().toUpperCase();
            const current = stepByBusinessKey.get(key);

            if (!current) {
                stepByBusinessKey.set(key, step);
                continue;
            }

            const currentIsUsed = relatedStepIds.has(current.id);
            const nextIsUsed = relatedStepIds.has(step.id);

            if ((!currentIsUsed && nextIsUsed) ||
                (current.thuTuSapXep || 0) > (step.thuTuSapXep || 0)) {
                stepByBusinessKey.set(key, step);
            }
        }

        const steps = [...stepByBusinessKey.values()].sort(
            (a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0)
        );
        const stepIdsInWorkflow = new Set(steps.map((step) => step.id));
        const scopedTimeline = timeline.filter(
            (item) => !item.tuBuocId || !item.denBuocId ||
                stepIdsInWorkflow.has(item.tuBuocId) ||
                stepIdsInWorkflow.has(item.denBuocId)
        );
        const currentStep = steps.find((step) => step.id === detail?.buocHienTaiId);
        const currentOrder = currentStep?.thuTuSapXep || 0;
        const isFinal = COMPLETED_STATUS_CODES.has(detail?.maTrangThaiHoSo);

        return steps.map((step) => {
            const enteredItems = scopedTimeline
                .filter((item) => item.denBuocId === step.id)
                .sort((a, b) => new Date(a.ngayXuLy || 0) - new Date(b.ngayXuLy || 0));
            const movedItems = scopedTimeline
                .filter((item) => item.tuBuocId === step.id && item.denBuocId && item.denBuocId !== step.id)
                .sort((a, b) => new Date(b.ngayXuLy || 0) - new Date(a.ngayXuLy || 0));
            const relatedItems = scopedTimeline
                .filter((item) => item.tuBuocId === step.id || item.denBuocId === step.id)
                .sort((a, b) => new Date(b.ngayXuLy || 0) - new Date(a.ngayXuLy || 0));
            const isCurrent = step.id === detail?.buocHienTaiId;
            const isCompleted = isFinal || movedItems.length > 0 ||
                (currentOrder > 0 && (step.thuTuSapXep || 0) < currentOrder);

            return {
                ...step,
                enteredAt: enteredItems[0]?.ngayXuLy,
                completedAt: movedItems[0]?.ngayXuLy || (isFinal && isCurrent ? relatedItems[0]?.ngayXuLy : null),
                latestTimeline: relatedItems[0],
                status: isCompleted ? "completed" : isCurrent ? "current" : "pending",
            };
        });
    }, [detail, quyTrinh, timeline]);

    const getDonViName = (donViId) =>
        donVis.find((item) => item.id === donViId)?.tenDonVi ||
        donVis.find((item) => item.id === donViId)?.ten ||
        "";

    const updateForm = (key, value) => {
        setForm((current) => ({
            ...current,
            [key]: value,
        }));
    };

    useEffect(() => {
        const load = async () => {
            try {
                setLoading(true);
                setError("");

                const [vanBanResponse, donViResponse] = await Promise.all([
                    getVanBans({
                        pageSize: 200,
                        pageCurrent: 1,
                    }),
                    getDonViOptions(),
                ]);

                setVanBans(vanBanResponse?.data || []);
                setDonVis(donViResponse || []);

                if (isEdit) {
                    const [detailResult, timelineResult, actionResult] =
                        await Promise.all([
                            getDangKyXayDungVanBanById(id),
                            getDangKyXayDungVanBanTimeline(id).catch(() => []),
                            getHanhDongKhaDung(id).catch(() => []),
                        ]);
                    const quyTrinhResult = detailResult?.quyTrinhSoanThaoId
                        ? await getQuyTrinhById(detailResult.quyTrinhSoanThaoId).catch(() => null)
                        : null;

                    setDetail(detailResult);
                    setTimeline(timelineResult || []);
                    setQuyTrinh(quyTrinhResult);
                    setActions(actionResult || []);

                    setForm({
                        tenHoSo: detailResult.tenHoSo || "",
                        tenVanBanDuKien: detailResult.tenVanBanDuKien || "",
                        loaiVanBanId: detailResult.loaiVanBanId || "",
                        donViSoanThaoId: detailResult.donViSoanThaoId || "",
                        donViPheDuyetId: detailResult.donViPheDuyetId || "",
                        namDangKy:
                            detailResult.namDangKy || CURRENT_YEAR,
                        canCuDeXuat: detailResult.canCuDeXuat || "",
                        suCanThiet: detailResult.suCanThiet || "",
                        noiDungChinhSach:
                            detailResult.noiDungChinhSach || "",
                        duKienThoiGianTrinh:
                            toDateInput(detailResult.duKienThoiGianTrinh),
                    });
                }
            } catch (loadError) {
                setError(
                    getErrorMessage(
                        loadError,
                        "Không thể tải dữ liệu nhập hồ sơ đăng ký."
                    )
                );
            } finally {
                setLoading(false);
            }
        };

        void load();
    }, [id, isEdit]);

    const validate = () => {
        if (!form.tenHoSo.trim()) {
            return "Vui lòng nhập tên hồ sơ.";
        }

        if (!form.tenVanBanDuKien.trim()) {
            return "Vui lòng nhập tên văn bản dự kiến.";
        }

        if (!form.loaiVanBanId) {
            return "Vui lòng chọn loại văn bản.";
        }

        if (!form.donViSoanThaoId) {
            return "Vui lòng chọn đơn vị soạn thảo.";
        }

        if (!form.donViPheDuyetId) {
            return "Vui lòng chọn đơn vị phê duyệt.";
        }

        if (
            !form.namDangKy ||
            Number(form.namDangKy) < 2000 ||
            Number(form.namDangKy) > 9999
        ) {
            return "Năm đăng ký không hợp lệ.";
        }

        return "";
    };

    const buildPayload = () => {
        const userInfo = readUserInfo();

        return {
            tenHoSo: form.tenHoSo.trim(),
            tenVanBanDuKien: form.tenVanBanDuKien.trim(),
            loaiVanBanId: form.loaiVanBanId,
            donViSoanThaoId: form.donViSoanThaoId,
            donViPheDuyetId: form.donViPheDuyetId,
            namDangKy: Number(form.namDangKy),

            canCuDeXuat: emptyToNull(form.canCuDeXuat),
            suCanThiet: emptyToNull(form.suCanThiet),
            noiDungChinhSach: emptyToNull(form.noiDungChinhSach),
            duKienThoiGianTrinh: form.duKienThoiGianTrinh || null,

            nguoiXuLyId: userInfo.userId,
            tenNguoiXuLy: userInfo.displayName || userInfo.username || "",
            tenDonViXuLy: getDonViName(userInfo.donViId) ||
                getDonViName(form.donViSoanThaoId),
        };
    };

    const buildActionPayload = (action, message) => {
        const userInfo = readUserInfo();

        return {
            hanhDongId: action.hanhDongId || action.id,
            noiDungXuLy: message,
            lyDoTraLai: null,
            nguoiXuLyId: userInfo.userId,
            tenNguoiXuLy: userInfo.displayName || userInfo.username || "",
            donViXuLyId: userInfo.donViId || form.donViSoanThaoId,
            tenDonViXuLy: getDonViName(userInfo.donViId) ||
                getDonViName(form.donViSoanThaoId),
            tenBuocTu: detail?.tenBuocHienTai,
            tenBuocDen: null,
            tenTrangThaiTruoc: detail?.tenTrangThaiHoSo,
            tenTrangThaiSau: null,
        };
    };

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (isReadOnly) {
            return;
        }

        const validationMessage = validate();

        if (validationMessage) {
            setError(validationMessage);
            return;
        }

        try {
            setSaving(true);
            setError("");
            setSuccess("");

            if (isEdit) {
                await updateDangKyXayDungVanBan(
                    id,
                    buildPayload()
                );

                setSuccess("Cập nhật hồ sơ đăng ký thành công.");
            } else {
                const payload = buildPayload();

                const result = await createDangKyXayDungVanBan(payload);

                setSuccess("Tạo hồ sơ đăng ký thành công.");

                const createdId = result?.id;

                if (createdId) {
                    setTimeout(() => {
                        navigate(
                            `/dang-ky-xay-dung-van-ban/ho-so/${createdId}`
                        );
                    }, 300);

                    return;
                }
            }

            setTimeout(() => {
                navigate(
                    "/dang-ky-xay-dung-van-ban/ho-so"
                );
            }, 500);
        } catch (saveError) {
            setError(
                getErrorMessage(
                    saveError,
                    "Không thể lưu hồ sơ đăng ký."
                )
            );
        } finally {
            setSaving(false);
        }
    };

    const handleSend = async () => {
        if (!sendAction) {
            setError("Hồ sơ chưa có hành động gửi phù hợp.");
            return;
        }

        const validationMessage = validate();
        if (validationMessage) {
            setError(validationMessage);
            return;
        }

        if (!window.confirm("Gửi hồ sơ đăng ký sang đơn vị phê duyệt?")) {
            return;
        }

        try {
            setSaving(true);
            setError("");
            setSuccess("");

            await updateDangKyXayDungVanBan(id, buildPayload());
            await xuLyDangKyXayDungVanBan(
                id,
                buildActionPayload(sendAction, "Trình phê duyệt hồ sơ đăng ký")
            );

            setSuccess("Đã gửi hồ sơ đăng ký.");
            setTimeout(() => {
                navigate(`/dang-ky-xay-dung-van-ban/ho-so/${id}?mode=view`);
            }, 500);
        } catch (sendError) {
            setError(getErrorMessage(sendError, "Không thể gửi hồ sơ đăng ký."));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        const label = detail?.maHoSo || form.tenHoSo || "hồ sơ này";

        if (!window.confirm(`Xóa hồ sơ "${label}"?`)) {
            return;
        }

        try {
            setSaving(true);
            setError("");
            setSuccess("");

            await deleteDangKyXayDungVanBan(id);
            setSuccess("Xóa hồ sơ đăng ký thành công.");
            setTimeout(() => {
                navigate("/dang-ky-xay-dung-van-ban/ho-so");
            }, 400);
        } catch (deleteError) {
            setError(getErrorMessage(deleteError, "Không thể xóa hồ sơ đăng ký."));
        } finally {
            setSaving(false);
        }
    };

    const vanBanOptions = asOptions(
        vanBans,
        "id",
        (item) =>
            item.tenLoaiVanBan ||
            item.ten ||
            item.maLoaiVanBan ||
            item.ma
    );

    const donViOptions = asOptions(
        donVis,
        "id",
        (item) =>
            item.tenDonVi ||
            item.ten ||
            item.maDonVi
    );

    return (
        <form
            className="space-y-5"
            onSubmit={handleSubmit}
        >
            {/* Header */}
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <button
                        type="button"
                        onClick={() =>
                            navigate(
                                "/dang-ky-xay-dung-van-ban/ho-so"
                            )
                        }
                        className="mb-3 text-sm font-medium text-brand-500 hover:text-brand-600"
                    >
                        Quay lại danh sách
                    </button>

                    <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
                        {isReadOnly
                            ? "Chi tiết hồ sơ đăng ký"
                            : isEdit
                            ? "Cập nhật hồ sơ đăng ký"
                            : "Tạo hồ sơ đăng ký xây dựng văn bản"}
                    </h1>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        {isReadOnly
                            ? "Theo dõi thông tin đề xuất xây dựng văn bản."
                            : "Nhập thông tin đề xuất xây dựng văn bản."}
                    </p>
                    {detail && (
                        <div className="mt-3 flex flex-wrap gap-2">
                            <Badge size="sm" color="light">
                                {detail.maHoSo}
                            </Badge>
                            <Badge size="sm" color="primary">
                                {detail.tenTrangThaiHoSo || detail.maTrangThaiHoSo}
                            </Badge>
                            <Badge size="sm" color="light">
                                {detail.tenBuocHienTai || detail.maBuocHienTai}
                            </Badge>
                        </div>
                    )}
                </div>

                <div className="flex flex-wrap gap-2">
                    <button
                        type="button"
                        onClick={() =>
                            navigate(
                                "/dang-ky-xay-dung-van-ban/ho-so"
                            )
                        }
                        className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300"
                        disabled={saving}
                    >
                        {isReadOnly ? "Quay lại" : "Hủy"}
                    </button>

                    {isReadOnly ? (
                        <button
                            type="button"
                            onClick={() =>
                                navigate(
                                    `/dang-ky-xay-dung-van-ban/ho-so/${id}`
                                )
                            }
                            disabled={!isEditable}
                            className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600"
                        >
                            Chỉnh sửa
                        </button>
                    ) : (
                        <button
                            type="submit"
                            className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300"
                            disabled={loading || saving}
                        >
                            {saving ? "Đang lưu..." : "Lưu"}
                        </button>
                    )}

                    {isEdit && isEditable && (
                        <>
                            <button
                                type="button"
                                onClick={() => void handleSend()}
                                disabled={loading || saving || !sendAction}
                                className="inline-flex h-11 items-center justify-center rounded-lg bg-success-500 px-5 text-sm font-medium text-white transition hover:bg-success-600 disabled:opacity-50"
                            >
                                Gửi
                            </button>
                            <button
                                type="button"
                                onClick={() => void handleDelete()}
                                disabled={saving}
                                className="inline-flex h-11 items-center justify-center rounded-lg border border-error-200 bg-white px-5 text-sm font-medium text-error-500 transition hover:bg-error-50 disabled:opacity-50 dark:border-error-500/30 dark:bg-transparent"
                            >
                                Xóa
                            </button>
                        </>
                    )}
                </div>
            </div>

            {error && (
                <Alert
                    variant="error"
                    title="Có lỗi"
                    message={error}
                />
            )}

            {success && (
                <Alert
                    variant="success"
                    title="Hoàn tất"
                    message={success}
                />
            )}

            {/* Thông tin hồ sơ */}
            <div
                className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
                {loading ? (
                    <div className="py-12 text-center text-sm text-gray-500 dark:text-gray-400">
                        Đang tải dữ liệu nhập liệu...
                    </div>
                ) : (
                    <div className="grid gap-5 lg:grid-cols-2">
                        <Field
                            label="Tên hồ sơ"
                            required
                        >
                            <Input
                                value={form.tenHoSo}
                                onChange={(event) =>
                                    updateForm(
                                        "tenHoSo",
                                        event.target.value
                                    )
                                }
                                disabled={saving || isReadOnly}
                            />
                        </Field>

                        <Field
                            label="Tên văn bản dự kiến"
                            required
                        >
                            <Input
                                value={form.tenVanBanDuKien}
                                onChange={(event) =>
                                    updateForm(
                                        "tenVanBanDuKien",
                                        event.target.value
                                    )
                                }
                                disabled={saving || isReadOnly}
                            />
                        </Field>

                        <Field
                            label="Loại văn bản"
                            required
                        >
                            <Select
                                value={form.loaiVanBanId}
                                placeholder="Chọn loại văn bản"
                                options={vanBanOptions}
                                onChange={(value) =>
                                    updateForm(
                                        "loaiVanBanId",
                                        value
                                    )
                                }
                                disabled={saving || isReadOnly}
                            />
                        </Field>

                        <Field
                            label="Đơn vị soạn thảo"
                            required
                        >
                            <Select
                                value={form.donViSoanThaoId}
                                placeholder="Chọn đơn vị soạn thảo"
                                options={donViOptions}
                                onChange={(value) =>
                                    updateForm(
                                        "donViSoanThaoId",
                                        value
                                    )
                                }
                                disabled={saving || isReadOnly}
                            />
                        </Field>

                        <Field
                            label="Đơn vị phê duyệt"
                            required
                        >
                            <Select
                                value={form.donViPheDuyetId}
                                placeholder="Chọn đơn vị phê duyệt"
                                options={donViOptions}
                                onChange={(value) =>
                                    updateForm(
                                        "donViPheDuyetId",
                                        value
                                    )
                                }
                                disabled={saving || isReadOnly}
                            />
                        </Field>

                        <Field
                            label="Năm đăng ký"
                            required
                        >
                            <Input
                                type="number"
                                min="2000"
                                max="9999"
                                value={form.namDangKy}
                                onChange={(event) =>
                                    updateForm(
                                        "namDangKy",
                                        event.target.value
                                    )
                                }
                                disabled={saving || isReadOnly}
                            />
                        </Field>

                        <Field label="Dự kiến thời gian trình">
                            <DatePicker
                                id="duKienThoiGianTrinh"
                                mode="single"
                                placeholder="Chọn ngày dự kiến trình"
                                defaultDate={form.duKienThoiGianTrinh || undefined}
                                onChange={(selectedDates, dateStr) => {
                                    updateForm("duKienThoiGianTrinh", dateStr || "");
                                }}
                                disabled={saving || isReadOnly}
                            />
                        </Field>
                    </div>
                )}
            </div>

            {/* Nội dung đề xuất */}
            <div
                className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
                <h2 className="text-base font-semibold text-gray-800 dark:text-white/90">
                    Nội dung đề xuất
                </h2>

                <div className="mt-5 grid gap-5">
                    <Field label="Căn cứ đề xuất">
                        <TextArea
                            rows={4}
                            value={form.canCuDeXuat}
                            onChange={(value) =>
                                updateForm(
                                    "canCuDeXuat",
                                    value
                                )
                            }
                            disabled={loading || saving || isReadOnly}
                        />
                    </Field>

                    <Field label="Sự cần thiết">
                        <TextArea
                            rows={4}
                            value={form.suCanThiet}
                            onChange={(value) =>
                                updateForm(
                                    "suCanThiet",
                                    value
                                )
                            }
                            disabled={loading || saving || isReadOnly}
                        />
                    </Field>

                    <Field label="Nội dung chính sách">
                        <TextArea
                            rows={5}
                            value={form.noiDungChinhSach}
                            onChange={(value) =>
                                updateForm(
                                    "noiDungChinhSach",
                                    value
                                )
                            }
                            disabled={loading || saving || isReadOnly}
                        />
                    </Field>
                </div>
            </div>

            {isEdit && (
                <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
                    <div className="mb-5">
                        <h2 className="text-base font-semibold text-gray-800 dark:text-white/90">
                            Timeline quy trình
                        </h2>
                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            Các bước được hiển thị theo thứ tự cấu hình trong quy trình đăng ký.
                        </p>
                    </div>

                    {workflowSteps.length ? (
                        <div>
                            {workflowSteps.map((step, index) => {
                                const meta = getStepStatusMeta(step.status);
                                const isLast = index === workflowSteps.length - 1;

                                return (
                                    <div
                                        key={step.id}
                                        className="relative grid grid-cols-[44px_1fr] gap-4"
                                    >
                                        {!isLast && (
                                            <div
                                                className={`absolute left-[21px] top-11 h-full w-0.5 ${meta.lineClass}`}
                                            />
                                        )}
                                        <div
                                            className={`relative z-10 flex h-11 w-11 items-center justify-center rounded-full border-2 text-sm font-semibold ${meta.nodeClass}`}
                                        >
                                            {step.status === "completed" ? "✓" : step.thuTuSapXep}
                                        </div>
                                        <div className={isLast ? "" : "pb-6"}>
                                            <div className="rounded-lg border border-gray-200 p-4 dark:border-white/[0.05]">
                                                <div className="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
                                                    <div>
                                                        <div className="font-medium text-gray-800 dark:text-white/90">
                                                            {step.tenBuoc}
                                                        </div>
                                                        <div className="mt-1 text-xs text-gray-500">
                                                            {step.maBuoc} · {step.loaiBuoc || "XuLy"}
                                                        </div>
                                                    </div>
                                                    <Badge
                                                        size="sm"
                                                        color={meta.badgeColor}
                                                    >
                                                        {meta.label}
                                                    </Badge>
                                                </div>

                                                <div className="mt-4 grid gap-3 text-sm sm:grid-cols-3">
                                                    <Field label="Thời hạn xử lý">
                                                        <div className="text-sm font-medium text-gray-800 dark:text-white/90">
                                                            {step.soNgayXuLyTieuChuan
                                                                ? `${step.soNgayXuLyTieuChuan} ngày`
                                                                : "-"}
                                                        </div>
                                                    </Field>
                                                    <Field label="Ngày gửi">
                                                        <div className="text-sm font-medium text-gray-800 dark:text-white/90">
                                                            {formatDate(step.enteredAt)}
                                                        </div>
                                                    </Field>
                                                    <Field label="Ngày hoàn thành">
                                                        <div className="text-sm font-medium text-gray-800 dark:text-white/90">
                                                            {formatDate(step.completedAt)}
                                                        </div>
                                                    </Field>
                                                </div>

                                                {step.latestTimeline?.noiDungXuLy && (
                                                    <div className="mt-3 text-sm text-gray-600 dark:text-gray-300">
                                                        {step.latestTimeline.noiDungXuLy}
                                                    </div>
                                                )}
                                                {step.moTa && (
                                                    <div className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                                                        {step.moTa}
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    ) : (
                        <div className="rounded-lg border border-dashed border-gray-300 px-4 py-8 text-center text-sm text-gray-500 dark:border-gray-700 dark:text-gray-400">
                            Chưa tải được cấu hình các bước của quy trình.
                        </div>
                    )}
                </div>
            )}

            {isEdit && (
                <div className="rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
                    <div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]">
                        <h2 className="text-base font-semibold text-gray-800 dark:text-white/90">
                            Lịch sử xử lý
                        </h2>
                    </div>

                    <div className="max-w-full overflow-x-auto">
                        <table className="w-full">
                            <thead>
                            <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">
                                    Thời gian
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">
                                    Hành động
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">
                                    Chuyển bước
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">
                                    Người xử lý
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">
                                    Nội dung
                                </th>
                            </tr>
                            </thead>
                            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                            {timeline.length ? (
                                timeline.map((item) => (
                                    <tr
                                        key={item.id}
                                        className="align-top hover:bg-gray-50 dark:hover:bg-white/[0.02]"
                                    >
                                        <td className="px-5 py-4 text-sm text-gray-500">
                                            {formatDateTime(item.ngayXuLy)}
                                        </td>
                                        <td className="px-5 py-4">
                                            <div className="font-medium text-gray-800 dark:text-white/90">
                                                {item.tenHanhDong || item.maHanhDong}
                                            </div>
                                            <div className="mt-1 text-xs text-gray-500">
                                                {item.tenTrangThaiTruoc || "-"} → {item.tenTrangThaiSau || "-"}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">
                                            {item.tenBuocTu || "-"} → {item.tenBuocDen || "-"}
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">
                                            <div>{item.tenNguoiXuLy || "-"}</div>
                                            <div className="mt-1 text-xs text-gray-500">
                                                {item.tenDonViXuLy || "-"}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">
                                            <div>{item.noiDungXuLy || "-"}</div>
                                            {item.lyDoTraLai && (
                                                <div className="mt-1 text-xs text-error-500">
                                                    Lý do: {item.lyDoTraLai}
                                                </div>
                                            )}
                                        </td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td
                                        colSpan="5"
                                        className="px-5 py-10 text-center text-sm text-gray-500"
                                    >
                                        Chưa có lịch sử xử lý.
                                    </td>
                                </tr>
                            )}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}
        </form>
    );
}

