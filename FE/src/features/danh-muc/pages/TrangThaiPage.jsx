import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {Modal} from "../../../app/components/ui/modal";
import {
    createTrangThai,
    deleteTrangThai,
    getNextTrangThaiSortOrder,
    getTrangThais,
    updateTrangThai,
} from "../api/trangThaiApi";

const emptyForm = {
    id: "",
    nhomTrangThai: "",
    maTrangThai: "",
    tenTrangThai: "",
    maMauHex: "#2563EB",
    thuTuSapXep: 1,
    trangThai: true,
    moTa: "",
    ghiChu: "",
};

function getErrorMessage(error, fallback) {
    return error?.response?.data?.message || error?.message || fallback;
}

function normalizeColor(value) {
    const color = value?.trim();
    if (!color) {
        return "#2563EB";
    }

    return color.startsWith("#") ? color : `#${color}`;
}

function usePaginationPages(pageCurrent, totalPages) {
    return useMemo(() => {
        const visiblePages = 5;
        const halfWindow = Math.floor(visiblePages / 2);
        let startPage = Math.max(1, pageCurrent - halfWindow);
        const endPage = Math.min(totalPages, startPage + visiblePages - 1);

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

function ActionButton({children, onClick, tone = "default", disabled}) {
    const toneClasses = {
        default: "text-gray-500 hover:bg-gray-100 hover:text-brand-500 dark:text-gray-400 dark:hover:bg-white/[0.05]",
        danger: "text-gray-500 hover:bg-gray-100 hover:text-error-500 dark:text-gray-400 dark:hover:bg-white/[0.05]",
    };

    return (
        <button
            type="button"
            onClick={onClick}
            disabled={disabled}
            className={`inline-flex h-8 min-w-8 items-center justify-center rounded-lg px-2 text-xs font-medium transition disabled:cursor-not-allowed disabled:opacity-40 ${toneClasses[tone]}`}
        >
            {children}
        </button>
    );
}

export default function TrangThaiPage() {
    const [items, setItems] = useState([]);
    const [search, setSearch] = useState("");
    const [nhomTrangThai, setNhomTrangThai] = useState("");
    const [pageSize, setPageSize] = useState(10);
    const [pageCurrent, setPageCurrent] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [form, setForm] = useState(emptyForm);
    const [isFormOpen, setIsFormOpen] = useState(false);
    const [confirmAction, setConfirmAction] = useState(null);

    const groupOptions = useMemo(() => {
        const values = Array.from(
            new Set(
                items
                    .map((item) => item.nhomTrangThai)
                    .filter((value) => value && value.trim())
            )
        );

        return [
            {
                value: "",
                label: "Tất cả nhóm",
            },
            ...values.map((value) => ({
                value,
                label: value,
            })),
        ];
    }, [items]);

    const fetchItems = async () => {
        try {
            setLoading(true);
            setError("");

            const response = await getTrangThais({
                search,
                nhomTrangThai,
                pageSize,
                pageCurrent,
            });

            setItems(response?.data ?? []);
            setTotalCount(response?.totalCount ?? 0);
        } catch (fetchError) {
            setError(
                getErrorMessage(
                    fetchError,
                    "Không thể tải danh sách trạng thái."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void fetchItems();
    }, [search, nhomTrangThai, pageSize, pageCurrent]);

    const openCreateModal = async () => {
        const nextSortOrder = await getNextTrangThaiSortOrder().catch(() => 1);

        setForm({
            ...emptyForm,
            thuTuSapXep: nextSortOrder,
        });
        setIsFormOpen(true);
        setError("");
        setSuccess("");
    };

    const openEditModal = (item) => {
        setForm({
            id: item.id,
            nhomTrangThai: item.nhomTrangThai ?? "",
            maTrangThai: item.maTrangThai ?? "",
            tenTrangThai: item.tenTrangThai ?? "",
            maMauHex: normalizeColor(item.maMauHex),
            thuTuSapXep: item.thuTuSapXep ?? 1,
            trangThai: Boolean(item.trangThai),
            moTa: item.moTa ?? "",
            ghiChu: item.ghiChu ?? "",
        });
        setIsFormOpen(true);
        setError("");
        setSuccess("");
    };

    const closeFormModal = () => {
        if (!submitting) {
            setIsFormOpen(false);
            setForm(emptyForm);
        }
    };

    const validate = () => {
        if (!form.nhomTrangThai.trim()) {
            return "Nhóm trạng thái không được để trống.";
        }

        if (!form.maTrangThai.trim()) {
            return "Mã trạng thái không được để trống.";
        }

        if (!form.tenTrangThai.trim()) {
            return "Tên trạng thái không được để trống.";
        }

        if (!form.maMauHex.trim()) {
            return "Mã màu hiển thị không được để trống.";
        }

        if (Number(form.thuTuSapXep) < 0) {
            return "Thứ tự sắp xếp không hợp lệ.";
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

        const payload = {
            nhomTrangThai: form.nhomTrangThai.trim(),
            maTrangThai: form.maTrangThai.trim(),
            tenTrangThai: form.tenTrangThai.trim(),
            maMauHex: normalizeColor(form.maMauHex),
            thuTuSapXep: Number(form.thuTuSapXep),
            trangThai: Boolean(form.trangThai),
            moTa: form.moTa.trim() || null,
            ghiChu: form.ghiChu.trim() || null,
        };

        try {
            setSubmitting(true);
            setError("");
            setSuccess("");

            if (form.id) {
                await updateTrangThai(form.id, payload);
                setSuccess("Cập nhật trạng thái thành công.");
            } else {
                await createTrangThai(payload);
                setSuccess("Tạo mới trạng thái thành công.");
            }

            setIsFormOpen(false);
            setForm(emptyForm);
            await fetchItems();
        } catch (submitError) {
            setError(getErrorMessage(submitError, "Không thể lưu trạng thái."));
        } finally {
            setSubmitting(false);
        }
    };

    const runConfirmAction = async () => {
        if (!confirmAction) {
            return;
        }

        try {
            setSubmitting(true);
            setError("");
            setSuccess("");

            await confirmAction.action();
            setSuccess(confirmAction.successMessage);
            setConfirmAction(null);
            await fetchItems();
        } catch (actionError) {
            setError(getErrorMessage(actionError, confirmAction.errorMessage));
        } finally {
            setSubmitting(false);
        }
    };

    const handleSearchChange = (value) => {
        setSearch(value);
        setPageCurrent(1);
    };

    const handleGroupChange = (value) => {
        setNhomTrangThai(value);
        setPageCurrent(1);
    };

    return (
        <div>
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Trạng thái
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý nhóm trạng thái, mã trạng thái và màu hiển thị
                    </p>
                </div>

                <button
                    type="button"
                    onClick={openCreateModal}
                    className="inline-flex items-center justify-center gap-2 rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-brand-600"
                >
                    <span className="text-lg leading-none">+</span>
                    Thêm mới
                </button>
            </div>

            <div className="space-y-5">
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
                        title="Hoàn tất"
                        message={success}
                    />
                )}

                <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
                    <div className="grid grid-cols-1 gap-4 border-b border-gray-100 px-5 py-4 sm:grid-cols-12 dark:border-white/[0.05]">
                        <div className="w-full sm:col-span-3">
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

                        <div className="w-full sm:col-span-3">
                            <Label>Nhóm trạng thái</Label>
                            <Select
                                value={nhomTrangThai}
                                onChange={handleGroupChange}
                                options={groupOptions}
                            />
                        </div>

                        <div className="w-full sm:col-span-6">
                            <Label>Tìm Kiếm</Label>
                            <Input
                                value={search}
                                onChange={(event) =>
                                    handleSearchChange(event.target.value)
                                }
                                placeholder="Tìm theo mã, tên trạng thái, màu, mô tả..."
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
                                    Trạng thái
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Nhóm
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Màu
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Sử dụng
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Thứ tự
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
                                        colSpan="6"
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
                                        <td className="px-5 py-4">
                                            <div className="font-medium text-gray-800 dark:text-white/90">
                                                {item.tenTrangThai}
                                            </div>
                                            <div className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                                                {item.maTrangThai}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.nhomTrangThai}
                                        </td>
                                        <td className="px-5 py-4">
                                            <div className="flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400">
                                                <span
                                                    className="h-5 w-5 rounded-full border border-gray-200 dark:border-white/10"
                                                    style={{
                                                        backgroundColor: normalizeColor(
                                                            item.maMauHex
                                                        ),
                                                    }}
                                                />
                                                {normalizeColor(item.maMauHex)}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-center">
                                            <Badge
                                                size="sm"
                                                color={
                                                    item.trangThai
                                                        ? "success"
                                                        : "light"
                                                }
                                            >
                                                {item.trangThai
                                                    ? "Đang dùng"
                                                    : "Tạm dừng"}
                                            </Badge>
                                        </td>
                                        <td className="px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400">
                                            {item.thuTuSapXep}
                                        </td>
                                        <td className="px-5 py-4">
                                            <div className="flex flex-wrap items-center justify-center gap-1">
                                                <ActionButton
                                                    onClick={() => openEditModal(item)}
                                                >
                                                    Sửa
                                                </ActionButton>
                                                <ActionButton
                                                    tone="danger"
                                                    onClick={() =>
                                                        setConfirmAction({
                                                            title: "Xóa trạng thái",
                                                            message: `Xóa trạng thái "${item.tenTrangThai}"?`,
                                                            action: () => deleteTrangThai(item.id),
                                                            successMessage:
                                                                "Xóa trạng thái thành công.",
                                                            errorMessage:
                                                                "Không thể xóa trạng thái.",
                                                        })
                                                    }
                                                >
                                                    Xóa
                                                </ActionButton>
                                            </div>
                                        </td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td
                                        colSpan="6"
                                        className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                    >
                                        Không có dữ liệu trạng thái.
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
                isOpen={isFormOpen}
                onClose={closeFormModal}
                className="max-w-3xl p-6"
            >
                <form onSubmit={handleSubmit} className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            {form.id ? "Cập nhật trạng thái" : "Thêm mới trạng thái"}
                        </h4>
                    </div>

                    <div className="space-y-4">
                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Nhóm trạng thái</Label>
                                <Input
                                    value={form.nhomTrangThai}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            nhomTrangThai: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>

                            <div>
                                <Label>Mã trạng thái</Label>
                                <Input
                                    value={form.maTrangThai}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            maTrangThai: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>Tên trạng thái</Label>
                            <Input
                                value={form.tenTrangThai}
                                onChange={(event) =>
                                    setForm((current) => ({
                                        ...current,
                                        tenTrangThai: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Mã màu hiển thị</Label>
                                <Input
                                    value={form.maMauHex}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            maMauHex: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                    suffix={
                                        <span
                                            className="h-5 w-5 rounded-full border border-gray-200 dark:border-white/10"
                                            style={{
                                                backgroundColor: normalizeColor(
                                                    form.maMauHex
                                                ),
                                            }}
                                        />
                                    }
                                />
                            </div>

                            <div>
                                <Label>Thứ tự sắp xếp</Label>
                                <Input
                                    type="number"
                                    min="0"
                                    value={form.thuTuSapXep}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            thuTuSapXep: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>Mô tả</Label>
                            <Input
                                value={form.moTa}
                                onChange={(event) =>
                                    setForm((current) => ({
                                        ...current,
                                        moTa: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Ghi chú</Label>
                            <Input
                                value={form.ghiChu}
                                onChange={(event) =>
                                    setForm((current) => ({
                                        ...current,
                                        ghiChu: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Trạng thái</Label>
                            <label className="flex h-11 items-center gap-3 rounded-lg border border-gray-300 bg-white px-4 text-sm font-medium text-gray-700 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
                                <input
                                    type="checkbox"
                                    checked={form.trangThai}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            trangThai: event.target.checked,
                                        }))
                                    }
                                    disabled={submitting}
                                    className="h-4 w-4 rounded border-gray-300 text-brand-500 focus:ring-brand-500"
                                />
                                Đang sử dụng
                            </label>
                        </div>
                    </div>

                    <div className="flex justify-end gap-3">
                        <button
                            type="button"
                            onClick={closeFormModal}
                            disabled={submitting}
                            className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400"
                        >
                            Hủy
                        </button>
                        <button
                            type="submit"
                            disabled={submitting}
                            className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300"
                        >
                            {submitting ? "Đang lưu..." : "Lưu"}
                        </button>
                    </div>
                </form>
            </Modal>

            <Modal
                isOpen={Boolean(confirmAction)}
                onClose={() => !submitting && setConfirmAction(null)}
                className="max-w-md p-6"
            >
                <div className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            {confirmAction?.title}
                        </h4>
                        <p className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                            {confirmAction?.message}
                        </p>
                    </div>

                    <div className="flex justify-end gap-3">
                        <button
                            type="button"
                            onClick={() => setConfirmAction(null)}
                            disabled={submitting}
                            className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400"
                        >
                            Hủy
                        </button>
                        <button
                            type="button"
                            onClick={runConfirmAction}
                            disabled={submitting}
                            className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300"
                        >
                            {submitting ? "Đang xử lý..." : "Xác nhận"}
                        </button>
                    </div>
                </div>
            </Modal>
        </div>
    );
}
