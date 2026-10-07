import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import ComponentCard from "../../../app/components/common/ComponentCard";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {
    getSystemInfo,
    saveSystemInfo,
} from "../api/systemInfoApi";

const defaultForm = {
    id: "00000000-0000-0000-0000-000000000000",
    appName: "",
    copyright: "",
    mfgDate: "",
    expDate: "",
    loginLock: 5,
    train: false,
    isChatBot: false,
    isOPT: false,
    menuLayout: "vertical",
};

const menuLayoutOptions = [
    {
        value: "vertical",
        label: "Menu dọc",
    },
    {
        value: "horizontal",
        label: "Menu ngang",
    },
];

function toDateInputValue(value) {
    if (!value) {
        return "";
    }

    return String(value).slice(0, 10);
}

function toFormValue(data) {
    return {
        ...defaultForm,
        ...data,
        mfgDate: toDateInputValue(data?.mfgDate),
        expDate: toDateInputValue(data?.expDate),
        loginLock: data?.loginLock ?? defaultForm.loginLock,
        menuLayout: data?.menuLayout || defaultForm.menuLayout,
    };
}

function ToggleField({label, description, checked, onChange, disabled}) {
    return (
        <button
            type="button"
            role="switch"
            aria-checked={checked}
            disabled={disabled}
            onClick={() => onChange(!checked)}
            className={`flex w-full items-center justify-between gap-4 rounded-lg border px-4 py-3 text-left transition ${
                disabled
                    ? "cursor-not-allowed border-gray-200 bg-gray-50 opacity-70 dark:border-gray-800 dark:bg-gray-900"
                    : "border-gray-200 bg-white hover:border-brand-300 dark:border-gray-800 dark:bg-white/[0.03]"
            }`}
        >
            <span>
                <span className="block text-sm font-medium text-gray-800 dark:text-white/90">
                    {label}
                </span>

                {description && (
                    <span className="mt-1 block text-xs text-gray-500 dark:text-gray-400">
                        {description}
                    </span>
                )}
            </span>

            <span
                className={`relative h-6 w-11 shrink-0 rounded-full transition ${
                    checked
                        ? "bg-brand-500"
                        : "bg-gray-200 dark:bg-white/10"
                }`}
            >
                <span
                    className={`absolute left-0.5 top-0.5 h-5 w-5 rounded-full bg-white shadow-theme-sm transition ${
                        checked ? "translate-x-full" : "translate-x-0"
                    }`}
                />
            </span>
        </button>
    );
}

export default function SystemInfoPage() {
    const [form, setForm] = useState(defaultForm);
    const [savedForm, setSavedForm] = useState(defaultForm);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const isDirty = useMemo(
        () => JSON.stringify(form) !== JSON.stringify(savedForm),
        [form, savedForm]
    );

    useEffect(() => {
        let isMounted = true;

        const fetchSystemInfo = async () => {
            try {
                setLoading(true);
                setError("");

                const data = await getSystemInfo();
                const nextForm = toFormValue(data);

                if (isMounted) {
                    setForm(nextForm);
                    setSavedForm(nextForm);
                }
            } catch (fetchError) {
                if (isMounted) {
                    setError(
                        fetchError.message ||
                        "Không thể tải cấu hình hệ thống."
                    );
                }
            } finally {
                if (isMounted) {
                    setLoading(false);
                }
            }
        };

        void fetchSystemInfo();

        return () => {
            isMounted = false;
        };
    }, []);

    const updateField = (field, value) => {
        setForm((current) => ({
            ...current,
            [field]: value,
        }));
        setSuccess("");
    };

    const validate = () => {
        if (Number(form.loginLock) < 0) {
            return "Số lần khóa đăng nhập không được nhỏ hơn 0.";
        }

        if (form.mfgDate && form.expDate && form.expDate < form.mfgDate) {
            return "Ngày hết hạn phải lớn hơn hoặc bằng ngày bắt đầu.";
        }

        if (!["vertical", "horizontal"].includes(form.menuLayout)) {
            return "Kiểu hiển thị menu không hợp lệ.";
        }

        return "";
    };

    const handleSubmit = async (event) => {
        event.preventDefault();

        const validationMessage = validate();
        if (validationMessage) {
            setError(validationMessage);
            setSuccess("");
            return;
        }

        try {
            setSaving(true);
            setError("");
            setSuccess("");

            const payload = {
                ...form,
                loginLock: Number(form.loginLock),
                mfgDate: form.mfgDate || null,
                expDate: form.expDate || null,
            };

            const savedData = await saveSystemInfo(payload);
            const nextForm = toFormValue(savedData);

            setForm(nextForm);
            setSavedForm(nextForm);
            setSuccess(
                "Cập nhật cấu hình hệ thống thành công. Vui lòng đăng nhập lại để áp dụng đầy đủ thay đổi."
            );
        } catch (saveError) {
            setError(
                saveError.message ||
                "Không thể cập nhật cấu hình hệ thống."
            );
        } finally {
            setSaving(false);
        }
    };

    const handleReset = () => {
        setForm(savedForm);
        setError("");
        setSuccess("");
    };

    return (
        <div>
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Cấu hình hệ thống
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Thiết lập thông tin ứng dụng, đăng nhập và giao diện hệ thống
                    </p>
                </div>
            </div>

            <form onSubmit={handleSubmit} className="space-y-5">
                {error && (
                    <Alert
                        variant="error"
                        title="Không thể xử lý"
                        message={error}
                    />
                )}

                {success && (
                    <Alert
                        variant="success"
                        title="Đã lưu cấu hình"
                        message={success}
                    />
                )}

                <ComponentCard
                    title="Thông tin ứng dụng"
                    desc="Các thông tin hiển thị chung cho hệ thống."
                >
                    <div className="grid gap-5 lg:grid-cols-2">
                        <div>
                            <Label htmlFor="appName">
                                Tên ứng dụng
                            </Label>
                            <Input
                                id="appName"
                                value={form.appName}
                                onChange={(event) =>
                                    updateField(
                                        "appName",
                                        event.target.value
                                    )
                                }
                                placeholder="Nhập tên ứng dụng"
                                disabled={loading || saving}
                            />
                        </div>

                        <div>
                            <Label htmlFor="copyright">
                                Copyright
                            </Label>
                            <Input
                                id="copyright"
                                value={form.copyright}
                                onChange={(event) =>
                                    updateField(
                                        "copyright",
                                        event.target.value
                                    )
                                }
                                placeholder="Nhập thông tin bản quyền"
                                disabled={loading || saving}
                            />
                        </div>
                    </div>
                </ComponentCard>

                <ComponentCard
                    title="Thời hạn và đăng nhập"
                    desc="Ràng buộc thời hạn sử dụng và chính sách khóa tài khoản."
                >
                    <div className="grid gap-5 lg:grid-cols-3">
                        <div>
                            <Label htmlFor="mfgDate">
                                Ngày bắt đầu
                            </Label>
                            <Input
                                id="mfgDate"
                                type="date"
                                value={form.mfgDate}
                                onChange={(event) =>
                                    updateField(
                                        "mfgDate",
                                        event.target.value
                                    )
                                }
                                disabled={loading || saving}
                            />
                        </div>

                        <div>
                            <Label htmlFor="expDate">
                                Ngày hết hạn
                            </Label>
                            <Input
                                id="expDate"
                                type="date"
                                value={form.expDate}
                                onChange={(event) =>
                                    updateField(
                                        "expDate",
                                        event.target.value
                                    )
                                }
                                disabled={loading || saving}
                            />
                        </div>

                        <div>
                            <Label htmlFor="loginLock">
                                Số lần đăng nhập sai
                            </Label>
                            <Input
                                id="loginLock"
                                type="number"
                                min="0"
                                step="1"
                                value={form.loginLock}
                                onChange={(event) =>
                                    updateField(
                                        "loginLock",
                                        event.target.value
                                    )
                                }
                                placeholder="Nhập số lần"
                                disabled={loading || saving}
                            />
                        </div>
                    </div>
                </ComponentCard>

                <ComponentCard
                    title="Giao diện và tính năng"
                    desc="Bật tắt các tính năng nền và kiểu hiển thị menu."
                >
                    <div className="grid gap-5 lg:grid-cols-2">
                        <div>
                            <Label>
                                Kiểu hiển thị menu
                            </Label>
                            <Select
                                value={form.menuLayout}
                                options={menuLayoutOptions}
                                onChange={(value) =>
                                    updateField("menuLayout", value)
                                }
                                placeholder="Chọn kiểu menu"
                                className={
                                    loading || saving
                                        ? "pointer-events-none opacity-60"
                                        : ""
                                }
                            />
                        </div>

                        <div className="grid gap-3 sm:grid-cols-3 lg:col-span-2">
                            <ToggleField
                                label="Train"
                                description="Chế độ huấn luyện"
                                checked={form.train}
                                disabled={loading || saving}
                                onChange={(value) =>
                                    updateField("train", value)
                                }
                            />

                            <ToggleField
                                label="Chatbot"
                                description="Trợ lý hội thoại"
                                checked={form.isChatBot}
                                disabled={loading || saving}
                                onChange={(value) =>
                                    updateField("isChatBot", value)
                                }
                            />

                            <ToggleField
                                label="OTP"
                                description="Xác thực bổ sung"
                                checked={form.isOPT}
                                disabled={loading || saving}
                                onChange={(value) =>
                                    updateField("isOPT", value)
                                }
                            />
                        </div>
                    </div>
                </ComponentCard>

                <div className="flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
                    <button
                        type="button"
                        onClick={handleReset}
                        disabled={!isDirty || loading || saving}
                        className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03]"
                    >
                        Hoàn tác
                    </button>

                    <button
                        type="submit"
                        disabled={!isDirty || loading || saving}
                        className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:cursor-not-allowed disabled:bg-brand-300 disabled:opacity-70"
                    >
                        {saving ? "Đang lưu..." : "Lưu cấu hình"}
                    </button>
                </div>
            </form>
        </div>
    );
}

