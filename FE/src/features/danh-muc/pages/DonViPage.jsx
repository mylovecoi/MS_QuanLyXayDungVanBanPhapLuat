import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {Modal} from "../../../app/components/ui/modal";
import {
    createDonVi,
    deleteDonVi,
    getDonViOptions,
    getDonVis,
    getNextDonViSortOrder,
    updateDonVi,
} from "../api/donViApi";

const ROOT_PARENT_ID = "00000000-0000-0000-0000-000000000000";

const emptyForm = {
    id: "",
    tenDonVi: "",
    level: 0,
    sttSapXep: 1,
    donViChuQuanId: ROOT_PARENT_ID,
    diaChi: "",
    maQHNS: "",
    soDienThoai: "",
    chucDanhQuanLy: "",
    hoVaTenNguoiQuanLy: "",
    phanLoaiDonVi: "",
    tinhNangThanhToan: true,
};

function getErrorMessage(error, fallback) {
    return error?.response?.data?.message || error?.message || fallback;
}

function getMaQhns(item) {
    return item?.maQHNS ?? item?.maQhns ?? "";
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

export default function DonViPage() {
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
                    label: `${"— ".repeat(item.level)}${item.tenDonVi}`,
                })),
        ];
    }, [form.id, options]);

    const fetchItems = async () => {
        try {
            setLoading(true);
            setError("");

            const response = await getDonVis({
                search,
                pageSize,
                pageCurrent,
            });

            setItems(response?.data ?? []);
            setTotalCount(response?.totalCount ?? 0);
        } catch (fetchError) {
            setError(
                getErrorMessage(fetchError, "Không thể tải danh sách đơn vị.")
            );
        } finally {
            setLoading(false);
        }
    };

    const fetchOptions = async () => {
        try {
            const data = await getDonViOptions();
            setOptions(data ?? []);
        } catch (optionError) {
            setError(
                getErrorMessage(
                    optionError,
                    "Không thể tải danh sách đơn vị chủ quản."
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
        const nextSortOrder = await getNextDonViSortOrder(ROOT_PARENT_ID).catch(
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
            tenDonVi: item.tenDonVi ?? "",
            level: item.level ?? 0,
            sttSapXep: item.sttSapXep ?? 1,
            donViChuQuanId: item.donViChuQuanId || ROOT_PARENT_ID,
            diaChi: item.diaChi ?? "",
            maQHNS: getMaQhns(item),
            soDienThoai: item.soDienThoai ?? "",
            chucDanhQuanLy: item.chucDanhQuanLy ?? "",
            hoVaTenNguoiQuanLy: item.hoVaTenNguoiQuanLy ?? "",
            phanLoaiDonVi: item.phanLoaiDonVi ?? "",
            tinhNangThanhToan: Boolean(item.tinhNangThanhToan),
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
        const nextSortOrder = await getNextDonViSortOrder(parentId).catch(
            () => form.sttSapXep || 1
        );

        setForm((current) => ({
            ...current,
            donViChuQuanId: parentId,
            level: nextLevel,
            sttSapXep: nextSortOrder,
        }));
    };

    const validate = () => {
        if (!form.tenDonVi.trim()) {
            return "Tên đơn vị không được để trống.";
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
            tenDonVi: form.tenDonVi.trim(),
            level: Number(form.level),
            sttSapXep: Number(form.sttSapXep),
            donViChuQuanId: form.donViChuQuanId || ROOT_PARENT_ID,
            diaChi: form.diaChi.trim() || null,
            maQHNS: form.maQHNS.trim() || null,
            soDienThoai: form.soDienThoai.trim() || null,
            chucDanhQuanLy: form.chucDanhQuanLy.trim() || null,
            hoVaTenNguoiQuanLy: form.hoVaTenNguoiQuanLy.trim() || null,
            phanLoaiDonVi: form.phanLoaiDonVi.trim() || null,
            tinhNangThanhToan: Boolean(form.tinhNangThanhToan),
        };

        try {
            setSubmitting(true);
            setError("");
            setSuccess("");

            if (form.id) {
                await updateDonVi(form.id, payload);
                setSuccess("Cập nhật đơn vị thành công.");
            } else {
                await createDonVi(payload);
                setSuccess("Tạo mới đơn vị thành công.");
            }

            setIsFormOpen(false);
            setForm(emptyForm);
            await fetchOptions();
            await fetchItems();
        } catch (submitError) {
            setError(getErrorMessage(submitError, "Không thể lưu đơn vị."));
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
                        Đơn vị
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý danh mục đơn vị theo cấu trúc phân cấp
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
                                placeholder="Tìm theo tên, địa chỉ, phân loại đơn vị..."
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
                                    Đơn vị
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Mã QHNS
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Đơn vị chủ quản
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Người quản lý
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Thanh toán
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    STT
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
                                            <div
                                                style={{
                                                    paddingLeft: `${item.level * 16}px`,
                                                }}
                                            >
                                                <div className="font-medium text-gray-800 dark:text-white/90">
                                                    {item.tenDonVi}
                                                </div>
                                                <div className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                                                    {item.phanLoaiDonVi || "Chưa phân loại"}
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {getMaQhns(item) || "-"}
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.tenDonViChuQuan || "Cấp gốc"}
                                        </td>
                                        <td className="px-5 py-4">
                                            <div className="text-sm font-medium text-gray-700 dark:text-gray-300">
                                                {item.hoVaTenNguoiQuanLy || "-"}
                                            </div>
                                            <div className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                                                {item.chucDanhQuanLy || item.soDienThoai || "-"}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-center">
                                            <Badge
                                                size="sm"
                                                color={
                                                    item.tinhNangThanhToan
                                                        ? "success"
                                                        : "light"
                                                }
                                            >
                                                {item.tinhNangThanhToan
                                                    ? "Có"
                                                    : "Không"}
                                            </Badge>
                                        </td>
                                        <td className="px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400">
                                            {item.sttSapXep}
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
                                                            title: "Xóa đơn vị",
                                                            message: `Xóa đơn vị "${item.tenDonVi}"?`,
                                                            action: () => deleteDonVi(item.id),
                                                            successMessage:
                                                                "Xóa đơn vị thành công.",
                                                            errorMessage:
                                                                "Không thể xóa đơn vị.",
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
                                        Không có dữ liệu đơn vị.
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
                            {form.id ? "Cập nhật đơn vị" : "Thêm mới đơn vị"}
                        </h4>
                    </div>

                    <div className="space-y-4">
                        <div>
                            <Label>Tên đơn vị</Label>
                            <Input
                                value={form.tenDonVi}
                                onChange={(event) =>
                                    setForm((current) => ({
                                        ...current,
                                        tenDonVi: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Đơn vị chủ quản</Label>
                            <Select
                                value={form.donViChuQuanId}
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

                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Mã QHNS</Label>
                                <Input
                                    value={form.maQHNS}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            maQHNS: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>

                            <div>
                                <Label>Phân loại đơn vị</Label>
                                <Input
                                    value={form.phanLoaiDonVi}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            phanLoaiDonVi: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>
                        </div>

                        <div>
                            <Label>Địa chỉ</Label>
                            <Input
                                value={form.diaChi}
                                onChange={(event) =>
                                    setForm((current) => ({
                                        ...current,
                                        diaChi: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Họ và tên người quản lý</Label>
                                <Input
                                    value={form.hoVaTenNguoiQuanLy}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            hoVaTenNguoiQuanLy: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>

                            <div>
                                <Label>Chức danh quản lý</Label>
                                <Input
                                    value={form.chucDanhQuanLy}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            chucDanhQuanLy: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>
                        </div>

                        <div className="grid gap-4 sm:grid-cols-2">
                            <div>
                                <Label>Số điện thoại</Label>
                                <Input
                                    value={form.soDienThoai}
                                    onChange={(event) =>
                                        setForm((current) => ({
                                            ...current,
                                            soDienThoai: event.target.value,
                                        }))
                                    }
                                    disabled={submitting}
                                />
                            </div>

                            <div>
                                <Label>Tính năng thanh toán</Label>
                                <label className="flex h-11 items-center gap-3 rounded-lg border border-gray-300 bg-white px-4 text-sm font-medium text-gray-700 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
                                    <input
                                        type="checkbox"
                                        checked={form.tinhNangThanhToan}
                                        onChange={(event) =>
                                            setForm((current) => ({
                                                ...current,
                                                tinhNangThanhToan:
                                                event.target.checked,
                                            }))
                                        }
                                        disabled={submitting}
                                        className="h-4 w-4 rounded border-gray-300 text-brand-500 focus:ring-brand-500"
                                    />
                                    Cho phép thanh toán
                                </label>
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
