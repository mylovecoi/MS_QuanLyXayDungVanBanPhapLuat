import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {Modal} from "../../../app/components/ui/modal";
import {
    createVanBan,
    deleteVanBan,
    getNextVanBanSortOrder,
    getVanBans,
    updateVanBan,
} from "../api/vanBanApi";

const emptyForm = {
    id: "",
    tenLoaiVanBan: "",
    capChinhQuyen: "",
    chuTheBanHanh: "",
    kyHieuMau: "",
    thuTuSapXep: 1,
    trangThai: true,
    moTa: "",
    ghiChu: "",
};

function getErrorMessage(error, fallback) {
    return error?.response?.data?.message || error?.message || fallback;
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

export default function VanBanPage() {
    const [items, setItems] = useState([]);
    const [search, setSearch] = useState("");
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

    const fetchItems = async () => {
        try {
            setLoading(true);
            setError("");

            const response = await getVanBans({
                search,
                pageSize,
                pageCurrent,
            });

            setItems(response?.data ?? []);
            setTotalCount(response?.totalCount ?? 0);
        } catch (fetchError) {
            setError(
                getErrorMessage(
                    fetchError,
                    "Không thể tải danh sách loại văn bản."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void fetchItems();
    }, [search, pageSize, pageCurrent]);

    const openCreateModal = async () => {
        const nextSortOrder = await getNextVanBanSortOrder().catch(() => 1);

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
            tenLoaiVanBan: item.tenLoaiVanBan ?? "",
            capChinhQuyen: item.capChinhQuyen ?? "",
            chuTheBanHanh: item.chuTheBanHanh ?? "",
            kyHieuMau: item.kyHieuMau ?? "",
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
        if (!form.tenLoaiVanBan.trim()) {
            return "Tên loại văn bản không được để trống.";
        }

        if (!form.capChinhQuyen.trim()) {
            return "Cấp chính quyền không được để trống.";
        }

        if (!form.chuTheBanHanh.trim()) {
            return "Chủ thể ban hành không được để trống.";
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
            tenLoaiVanBan: form.tenLoaiVanBan.trim(),
            capChinhQuyen: form.capChinhQuyen.trim(),
            chuTheBanHanh: form.chuTheBanHanh.trim(),
            kyHieuMau: form.kyHieuMau.trim() || null,
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
                await updateVanBan(form.id, payload);
                setSuccess("Cập nhật loại văn bản thành công.");
            } else {
                await createVanBan(payload);
                setSuccess("Tạo mới loại văn bản thành công.");
            }

            setIsFormOpen(false);
            setForm(emptyForm);
            await fetchItems();
        } catch (submitError) {
            setError(
                getErrorMessage(submitError, "Không thể lưu loại văn bản.")
            );
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

    return (
        <div>
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Văn bản
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý danh mục loại văn bản, cấp chính quyền và chủ thể ban hành
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

                        <div className="w-full sm:col-span-9">
                            <Label>Tìm Kiếm</Label>
                            <Input
                                value={search}
                                onChange={(event) =>
                                    handleSearchChange(event.target.value)
                                }
                                placeholder="Tìm theo tên, cấp chính quyền, chủ thể ban hành, ký hiệu..."
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
                                    Loại văn bản
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Cấp chính quyền
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Chủ thể ban hành
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Ký hiệu mẫu
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Trạng thái
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
                                        colSpan="7"
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
                                                {item.tenLoaiVanBan}
                                            </div>
                                            {item.moTa && (
                                                <div className="mt-1 max-w-md truncate text-xs text-gray-500 dark:text-gray-400">
                                                    {item.moTa}
                                                </div>
                                            )}
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.capChinhQuyen}
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.chuTheBanHanh}
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.kyHieuMau || "-"}
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
                                                            title: "Xóa loại văn bản",
                                                            message: `Xóa loại văn bản "${item.tenLoaiVanBan}"?`,
                                                            action: () => deleteVanBan(item.id),
                                                            successMessage:
                                                                "Xóa loại văn bản thành công.",
                                                            errorMessage:
                                                                "Không thể xóa loại văn bản.",
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
                                        colSpan="7"
                                        className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                    >
                                        Không có dữ liệu loại văn bản.
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
                            {form.id
                                ? "Cập nhật loại văn bản"
                                : "Thêm mới loại văn bản"}
                        </h4>
                    </div>

                    <div className="space-y-4">
                        <div>
                            <Label>Tên loại văn bản</Label>
                            <Input
                                value={form.tenLoaiVanBan}
                                onChange={(event) =>
                                    setForm((current) => ({
                                        ...current,
                                        tenLoaiVanBan: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Cấp chính quyền</Label>
                                <Input
                                    value={form.capChinhQuyen}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            capChinhQuyen: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>

                            <div>
                                <Label>Chủ thể ban hành</Label>
                                <Input
                                    value={form.chuTheBanHanh}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            chuTheBanHanh: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>
                        </div>

                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Ký hiệu mẫu</Label>
                                <Input
                                    value={form.kyHieuMau}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            kyHieuMau: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
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
