import {useEffect, useState} from "react";
import {useNavigate, useParams} from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {getDonViOptions} from "../../danh-muc/api/donViApi";
import {getVanBans} from "../../danh-muc/api/vanBanApi";
import {
    createDangKyXayDungVanBan,
    getDangKyXayDungVanBanById,
    updateDangKyXayDungVanBan,
} from "../api/dangKyXayDungVanBanApi";

const CURRENT_YEAR = new Date().getFullYear();

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

export default function DangKyForm() {
    const {id} = useParams();
    const navigate = useNavigate();

    const isEdit = Boolean(id);

    const [form, setForm] = useState(initialForm);

    const [vanBans, setVanBans] = useState([]);
    const [donVis, setDonVis] = useState([]);

    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

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
                    const detail = await getDangKyXayDungVanBanById(id);

                    setForm({
                        tenHoSo: detail.tenHoSo || "",
                        tenVanBanDuKien: detail.tenVanBanDuKien || "",
                        loaiVanBanId: detail.loaiVanBanId || "",
                        donViSoanThaoId: detail.donViSoanThaoId || "",
                        donViPheDuyetId: detail.donViPheDuyetId || "",
                        namDangKy:
                            detail.namDangKy || CURRENT_YEAR,
                        canCuDeXuat: detail.canCuDeXuat || "",
                        suCanThiet: detail.suCanThiet || "",
                        noiDungChinhSach:
                            detail.noiDungChinhSach || "",
                        duKienThoiGianTrinh:
                            toDateInput(detail.duKienThoiGianTrinh),
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

    const buildPayload = () => ({
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
    });

    const handleSubmit = async (event) => {
        event.preventDefault();

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
                const result =
                    await createDangKyXayDungVanBan(
                        buildPayload()
                    );

                setSuccess("Tạo hồ sơ đăng ký thành công.");

                const createdId = result?.id;

                if (createdId) {
                    setTimeout(() => {
                        navigate(
                            `/admin/dang-ky-xay-dung-van-ban/ho-so/${createdId}`
                        );
                    }, 300);

                    return;
                }
            }

            setTimeout(() => {
                navigate(
                    "/admin/dang-ky-xay-dung-van-ban/danh-sach"
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
                        {isEdit
                            ? "Cập nhật hồ sơ đăng ký"
                            : "Tạo hồ sơ đăng ký xây dựng văn bản"}
                    </h1>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Nhập thông tin đề xuất xây dựng văn bản.
                    </p>
                </div>

                <div className="flex gap-2">
                    <button
                        type="button"
                        onClick={() =>
                            navigate(
                                "/admin/dang-ky-xay-dung-van-ban/danh-sach"
                            )
                        }
                        className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300"
                        disabled={saving}
                    >
                        Hủy
                    </button>

                    <button
                        type="submit"
                        className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300"
                        disabled={loading || saving}
                    >
                        {saving ? "Đang lưu..." : "Lưu"}
                    </button>
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
                                disabled={saving}
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
                                disabled={saving}
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
                                disabled={saving}
                            />
                        </Field>

                        <Field label="Dự kiến thời gian trình">
                            <Input
                                type="date"
                                value={
                                    form.duKienThoiGianTrinh
                                }
                                onChange={(event) =>
                                    updateForm(
                                        "duKienThoiGianTrinh",
                                        event.target.value
                                    )
                                }
                                disabled={saving}
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
                            disabled={loading || saving}
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
                            disabled={loading || saving}
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
                            disabled={loading || saving}
                        />
                    </Field>
                </div>
            </div>
        </form>
    );
}

