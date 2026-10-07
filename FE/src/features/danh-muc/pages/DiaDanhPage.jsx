import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {Modal} from "../../../app/components/ui/modal";
import {
    createDiaDanh,
    deleteDiaDanh,
    getDiaDanhOptions,
    getDiaDanhs,
    getNextDiaDanhSortOrder,
    updateDiaDanh,
} from "../api/diaDanhApi";

const ROOT_PARENT_ID = "00000000-0000-0000-0000-000000000000";

const emptyForm = {
    id: "",
    tenDiaDanh: "",
    level: 0,
    sttSapXep: 1,
    diaDanhCapTrenId: ROOT_PARENT_ID,
};

function getErrorMessage(error, fallback) {
    return (
        error?.response?.data?.message ||
        error?.message ||
        fallback
    );
}

function usePaginationPages(pageCurrent, totalPages) {
    return useMemo(() => {
        const visiblePages = 5;
        const halfWindow = Math.floor(visiblePages / 2);
        let startPage = Math.max(1, pageCurrent - halfWindow);
        const endPage = Math.min(
            totalPages,
            startPage + visiblePages - 1
        );

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

export default function DiaDanhPage() {
    const [items, setItems] = useState([]);
    const [options, setOptions] = useState([]);
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

    const parentOptions = useMemo(() => {
        const currentId = form.id;

        return [
            {
                value: ROOT_PARENT_ID,
                label: "Cấp gốc",
            },
            ...options
                .filter((item) => item.id !== currentId)
                .map((item) => ({
                    value: item.id,
                    label: `${"— ".repeat(item.level)}${item.tenDiaDanh}`,
                })),
        ];
    }, [form.id, options]);

    const fetchItems = async () => {
        try {
            setLoading(true);
            setError("");

            const response = await getDiaDanhs({
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
                    "Không thể tải danh sách địa danh."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    const fetchOptions = async () => {
        try {
            const data = await getDiaDanhOptions();
            setOptions(data ?? []);
        } catch (optionError) {
            setError(
                getErrorMessage(
                    optionError,
                    "Không thể tải danh sách địa danh cha."
                )
            );
        }
    };

    useEffect(() => {
        void fetchItems();
    }, [search, pageSize, pageCurrent]);

    useEffect(() => {
        void fetchOptions();
    }, []);

    const openCreateModal = async () => {
        const nextSortOrder = await getNextDiaDanhSortOrder(ROOT_PARENT_ID).catch(
            () => 1
        );

        setForm({
            ...emptyForm,
            sttSapXep: nextSortOrder,
        });
        setIsFormOpen(true);
        setError("");
        setSuccess("");
    };

    const openEditModal = (item) => {
        setForm({
            id: item.id,
            tenDiaDanh: item.tenDiaDanh ?? "",
            level: item.level ?? 0,
            sttSapXep: item.sttSapXep ?? 1,
            diaDanhCapTrenId: item.diaDanhCapTrenId || ROOT_PARENT_ID,
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

    const handleParentChange = async (parentId) => {
        const parent = options.find((item) => item.id === parentId);
        const nextLevel =
            parentId === ROOT_PARENT_ID ? 0 : Number(parent?.level ?? 0) + 1;
        const nextSortOrder = await getNextDiaDanhSortOrder(parentId).catch(
            () => form.sttSapXep || 1
        );

        setForm((current) => ({
            ...current,
            diaDanhCapTrenId: parentId,
            level: nextLevel,
            sttSapXep: nextSortOrder,
        }));
    };

    const validate = () => {
        if (!form.tenDiaDanh.trim()) {
            return "Tên địa danh không được để trống.";
        }

        if (Number(form.level) < 0) {
            return "Level không hợp lệ.";
        }

        if (Number(form.sttSapXep) < 0) {
            return "STT sắp xếp không hợp lệ.";
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
            tenDiaDanh: form.tenDiaDanh.trim(),
            level: Number(form.level),
            sttSapXep: Number(form.sttSapXep),
            diaDanhCapTrenId: form.diaDanhCapTrenId || ROOT_PARENT_ID,
        };

        try {
            setSubmitting(true);
            setError("");
            setSuccess("");

            if (form.id) {
                await updateDiaDanh(form.id, payload);
                setSuccess("Cập nhật địa danh thành công.");
            } else {
                await createDiaDanh(payload);
                setSuccess("Tạo mới địa danh thành công.");
            }

            setIsFormOpen(false);
            setForm(emptyForm);
            await fetchOptions();
            await fetchItems();
        } catch (submitError) {
            setError(
                getErrorMessage(
                    submitError,
                    "Không thể lưu địa danh."
                )
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
            await fetchOptions();
            await fetchItems();
        } catch (actionError) {
            setError(
                getErrorMessage(
                    actionError,
                    confirmAction.errorMessage
                )
            );
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
                        Địa danh
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý danh mục địa danh theo cấu trúc phân cấp
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
                                placeholder="Tìm theo tên địa danh..."
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
                                    Địa danh
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Level
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    STT
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Cấp trên
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
                                        colSpan="5"
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
                                            <div
                                                style={{
                                                    paddingLeft: `${item.level * 16}px`,
                                                }}
                                                className="font-medium text-gray-800 dark:text-white/90"
                                            >
                                                {item.tenDiaDanh}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-center">
                                            <Badge size="sm" color={item.level === 0 ? "primary" : "success"}>
                                                {item.level}
                                            </Badge>
                                        </td>
                                        <td className="px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400">
                                            {item.sttSapXep}
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.tenDiaDanhChuQuan || "Cấp gốc"}
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
                                                            title: "Xóa địa danh",
                                                            message: `Xóa địa danh "${item.tenDiaDanh}"? Các địa danh con cũng sẽ bị xóa.`,
                                                            action: () => deleteDiaDanh(item.id),
                                                            successMessage:
                                                                "Xóa địa danh thành công.",
                                                            errorMessage:
                                                                "Không thể xóa địa danh.",
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
                                        colSpan="5"
                                        className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                    >
                                        Không có dữ liệu địa danh.
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
                className="max-w-xl p-6"
            >
                <form onSubmit={handleSubmit} className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            {form.id ? "Cập nhật địa danh" : "Thêm mới địa danh"}
                        </h4>
                    </div>

                    <div className="space-y-4">
                        <div>
                            <Label>Tên địa danh</Label>
                            <Input
                                value={form.tenDiaDanh}
                                onChange={(event) =>
                                    setForm((current) => ({
                                        ...current,
                                        tenDiaDanh: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Địa danh cấp trên</Label>
                            <Select
                                value={form.diaDanhCapTrenId}
                                options={parentOptions}
                                onChange={handleParentChange}
                            />
                        </div>

                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Level</Label>
                                <Input
                                    type="number"
                                    min="0"
                                    value={form.level}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            level: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>

                            <div>
                                <Label>STT sắp xếp</Label>
                                <Input
                                    type="number"
                                    min="0"
                                    value={form.sttSapXep}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            sttSapXep: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>
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

