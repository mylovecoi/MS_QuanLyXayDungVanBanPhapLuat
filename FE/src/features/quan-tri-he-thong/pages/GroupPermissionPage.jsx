import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {Modal} from "../../../app/components/ui/modal";
import {
    createGroupPermission,
    deleteGroupPermission,
    getGroupPermissionDetails,
    getGroupPermissions,
    getTemplateGroups,
    updateGroupPermission,
    updateGroupPermissionDetail,
} from "../api/groupPermissionApi";

const statusOptions = [
    {value: "Kích hoạt", label: "Kích hoạt"},
    {value: "Khóa", label: "Khóa"},
];

const emptyGroupForm = {
    id: "",
    name: "",
    description: "",
    status: "Kích hoạt",
    templateGroup: "",
};

function getErrorMessage(error, fallback) {
    return (
        error?.response?.data?.message ||
        error?.message ||
        fallback
    );
}

function statusColor(status) {
    return status === "Kích hoạt" ? "success" : "error";
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

function PermissionCheckbox({checked, disabled, onChange, label}) {
    return (
        <label className="inline-flex items-center justify-center">
            <input
                type="checkbox"
                checked={Boolean(checked)}
                disabled={disabled}
                onChange={(event) => onChange(event.target.checked)}
                aria-label={label}
                className="h-4 w-4 rounded border-gray-300 text-brand-500 focus:ring-brand-500 disabled:cursor-not-allowed disabled:opacity-40 dark:border-gray-700 dark:bg-gray-900"
            />
        </label>
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

export default function GroupPermissionPage() {
    const [items, setItems] = useState([]);
    const [templateOptions, setTemplateOptions] = useState([]);
    const [search, setSearch] = useState("");
    const [pageSize, setPageSize] = useState(10);
    const [pageCurrent, setPageCurrent] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [groupForm, setGroupForm] = useState(emptyGroupForm);
    const [isGroupModalOpen, setIsGroupModalOpen] = useState(false);
    const [confirmAction, setConfirmAction] = useState(null);
    const [permissionGroup, setPermissionGroup] = useState(null);
    const [permissions, setPermissions] = useState([]);
    const [permissionSearch, setPermissionSearch] = useState("");
    const [permissionPageSize, setPermissionPageSize] = useState(10);
    const [permissionPageCurrent, setPermissionPageCurrent] = useState(1);
    const [permissionTotalCount, setPermissionTotalCount] = useState(0);
    const [permissionLoading, setPermissionLoading] = useState(false);
    const [updatingPermissionId, setUpdatingPermissionId] = useState("");

    const templateSelectOptions = useMemo(
        () =>
            templateOptions.map((item) => ({
                value: item.value,
                label: item.displayName,
            })),
        [templateOptions]
    );

    const fetchGroups = async () => {
        try {
            setLoading(true);
            setError("");

            const response = await getGroupPermissions({
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
                    "Không thể tải danh sách nhóm quyền."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    const fetchPermissions = async () => {
        if (!permissionGroup?.id) {
            return;
        }

        try {
            setPermissionLoading(true);
            setError("");

            const response = await getGroupPermissionDetails(permissionGroup.id, {
                search: permissionSearch,
                pageSize: permissionPageSize,
                pageCurrent: permissionPageCurrent,
            });

            setPermissions(response?.data ?? []);
            setPermissionTotalCount(response?.totalCount ?? 0);
        } catch (fetchError) {
            setError(
                getErrorMessage(
                    fetchError,
                    "Không thể tải danh sách quyền chi tiết."
                )
            );
        } finally {
            setPermissionLoading(false);
        }
    };

    useEffect(() => {
        void fetchGroups();
    }, [search, pageSize, pageCurrent]);

    useEffect(() => {
        const fetchTemplates = async () => {
            try {
                const data = await getTemplateGroups();
                setTemplateOptions(data ?? []);
            } catch (templateError) {
                setError(
                    getErrorMessage(
                        templateError,
                        "Không thể tải danh sách nhóm quyền mẫu."
                    )
                );
            }
        };

        void fetchTemplates();
    }, []);

    useEffect(() => {
        void fetchPermissions();
    }, [
        permissionGroup?.id,
        permissionSearch,
        permissionPageSize,
        permissionPageCurrent,
    ]);

    const openCreateModal = () => {
        setGroupForm(emptyGroupForm);
        setIsGroupModalOpen(true);
        setError("");
        setSuccess("");
    };

    const openEditModal = (item) => {
        setGroupForm({
            id: item.id,
            name: item.name ?? "",
            description: item.description ?? "",
            status: item.status || "Kích hoạt",
            templateGroup: "",
        });
        setIsGroupModalOpen(true);
        setError("");
        setSuccess("");
    };

    const closeGroupModal = () => {
        if (!submitting) {
            setIsGroupModalOpen(false);
            setGroupForm(emptyGroupForm);
        }
    };

    const validateGroup = () => {
        if (!groupForm.name.trim()) {
            return "Nhóm quyền là bắt buộc.";
        }

        if (!groupForm.status.trim()) {
            return "Trạng thái là bắt buộc.";
        }

        if (!groupForm.id && !groupForm.templateGroup) {
            return "Nhóm quyền mẫu là bắt buộc khi tạo mới.";
        }

        return "";
    };

    const handleGroupSubmit = async (event) => {
        event.preventDefault();

        const validationMessage = validateGroup();
        if (validationMessage) {
            setError(validationMessage);
            setSuccess("");
            return;
        }

        try {
            setSubmitting(true);
            setError("");
            setSuccess("");

            const payload = {
                name: groupForm.name.trim(),
                description: groupForm.description.trim() || null,
                status: groupForm.status,
                templateGroup: groupForm.templateGroup || null,
            };

            if (groupForm.id) {
                await updateGroupPermission(groupForm.id, payload);
                setSuccess("Cập nhật nhóm quyền thành công.");
            } else {
                await createGroupPermission(payload);
                setSuccess("Tạo mới nhóm quyền thành công.");
            }

            setIsGroupModalOpen(false);
            setGroupForm(emptyGroupForm);
            await fetchGroups();
        } catch (submitError) {
            setError(
                getErrorMessage(
                    submitError,
                    "Không thể lưu nhóm quyền."
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
            await fetchGroups();
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

    const handlePermissionToggle = async (permission, key, value) => {
        if (!permissionGroup?.id) {
            return;
        }

        const nextPermission = {
            ...permission,
            [key]: value,
        };

        try {
            setUpdatingPermissionId(permission.id);
            setError("");
            setSuccess("");

            const updated = await updateGroupPermissionDetail(
                permissionGroup.id,
                permission.id,
                {
                    index: nextPermission.index,
                    create: nextPermission.create,
                    edit: nextPermission.edit,
                    delete: nextPermission.delete,
                    approve: nextPermission.approve,
                    public: nextPermission.public,
                }
            );

            setPermissions((current) =>
                current.map((item) =>
                    item.id === permission.id ? updated : item
                )
            );
            setSuccess("Cập nhật quyền chi tiết thành công.");
        } catch (permissionError) {
            setError(
                getErrorMessage(
                    permissionError,
                    "Không thể cập nhật quyền chi tiết."
                )
            );
        } finally {
            setUpdatingPermissionId("");
        }
    };

    const handleSearchChange = (value) => {
        setSearch(value);
        setPageCurrent(1);
    };

    const handlePermissionSearchChange = (value) => {
        setPermissionSearch(value);
        setPermissionPageCurrent(1);
    };

    return (
        <div>
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Nhóm quyền truy cập
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý nhóm quyền và phân quyền chi tiết theo chức năng
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
                                placeholder="Tìm theo tên nhóm quyền, mô tả hoặc trạng thái..."
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
                                    Nhóm quyền
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Mô tả
                                </th>
                                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Trạng thái
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
                                        colSpan="4"
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
                                                {item.name}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.description || "Không có mô tả"}
                                        </td>
                                        <td className="px-5 py-4 text-center">
                                            <Badge
                                                size="sm"
                                                color={statusColor(item.status)}
                                            >
                                                {item.status}
                                            </Badge>
                                        </td>
                                        <td className="px-5 py-4">
                                            <div className="flex flex-wrap items-center justify-center gap-1">
                                                <ActionButton
                                                    onClick={() => {
                                                        setPermissionGroup(item);
                                                        setPermissionSearch("");
                                                        setPermissionPageCurrent(1);
                                                    }}
                                                >
                                                    Phân quyền
                                                </ActionButton>
                                                <ActionButton
                                                    onClick={() => openEditModal(item)}
                                                >
                                                    Sửa
                                                </ActionButton>
                                                <ActionButton
                                                    tone="danger"
                                                    onClick={() =>
                                                        setConfirmAction({
                                                            title: "Xóa nhóm quyền",
                                                            message: `Xóa nhóm quyền "${item.name}"? Toàn bộ quyền chi tiết của nhóm này cũng sẽ bị xóa.`,
                                                            action: () =>
                                                                deleteGroupPermission(item.id),
                                                            successMessage:
                                                                "Xóa nhóm quyền thành công.",
                                                            errorMessage:
                                                                "Không thể xóa nhóm quyền.",
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
                                        colSpan="4"
                                        className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                    >
                                        Không có dữ liệu nhóm quyền.
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
                isOpen={isGroupModalOpen}
                onClose={closeGroupModal}
                className="max-w-xl p-6"
            >
                <form onSubmit={handleGroupSubmit} className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            {groupForm.id ? "Cập nhật nhóm quyền" : "Thêm mới nhóm quyền"}
                        </h4>
                    </div>

                    <div className="space-y-4">
                        <div>
                            <Label>Tên nhóm quyền</Label>
                            <Input
                                value={groupForm.name}
                                onChange={(event) =>
                                    setGroupForm((current) => ({
                                        ...current,
                                        name: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Mô tả</Label>
                            <Input
                                value={groupForm.description}
                                onChange={(event) =>
                                    setGroupForm((current) => ({
                                        ...current,
                                        description: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Trạng thái</Label>
                            <Select
                                value={groupForm.status}
                                options={statusOptions}
                                onChange={(value) =>
                                    setGroupForm((current) => ({
                                        ...current,
                                        status: value,
                                    }))
                                }
                            />
                        </div>

                        {!groupForm.id && (
                            <div>
                                <Label>Nhóm quyền mẫu</Label>
                                <Select
                                    value={groupForm.templateGroup}
                                    options={templateSelectOptions}
                                    placeholder="Chọn nhóm quyền mẫu"
                                    onChange={(value) =>
                                        setGroupForm((current) => ({
                                            ...current,
                                            templateGroup: value,
                                        }))
                                    }
                                />
                            </div>
                        )}
                    </div>

                    <div className="flex justify-end gap-3">
                        <button
                            type="button"
                            onClick={closeGroupModal}
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
                isOpen={Boolean(permissionGroup)}
                onClose={() => !permissionLoading && setPermissionGroup(null)}
                className="max-w-6xl p-6"
            >
                <div className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            Phân quyền chi tiết
                        </h4>
                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            {permissionGroup?.name}
                        </p>
                    </div>

                    <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
                        <div className="grid grid-cols-1 gap-4 border-b border-gray-100 px-5 py-4 sm:grid-cols-12 dark:border-white/[0.05]">
                            <div className="w-full sm:col-span-3">
                                <Label>Hiển Thị</Label>
                                <Select
                                    value={permissionPageSize}
                                    onChange={(value) => {
                                        setPermissionPageSize(Number(value));
                                        setPermissionPageCurrent(1);
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
                                    value={permissionSearch}
                                    onChange={(event) =>
                                        handlePermissionSearchChange(event.target.value)
                                    }
                                    placeholder="Tìm theo chức năng hoặc role..."
                                />
                            </div>
                        </div>

                        <div className="max-w-full overflow-x-auto">
                            <table className="w-full">
                                <thead>
                                <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Chức năng
                                    </th>
                                    {["Index", "Create", "Edit", "Delete", "Approve", "Public"].map((label) => (
                                        <th
                                            key={label}
                                            className="px-3 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400"
                                        >
                                            {label}
                                        </th>
                                    ))}
                                </tr>
                                </thead>

                                <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                                {permissionLoading ? (
                                    <tr>
                                        <td
                                            colSpan="7"
                                            className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                        >
                                            Đang tải dữ liệu...
                                        </td>
                                    </tr>
                                ) : permissions.length > 0 ? (
                                    permissions.map((item) => {
                                        const isGroup = item.phanLoai === "Group";
                                        const disabled =
                                            updatingPermissionId === item.id;

                                        return (
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
                                                            {item.title || item.role}
                                                        </div>
                                                        <div className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                                                            {item.role}
                                                        </div>
                                                        <div className="mt-2">
                                                            <Badge
                                                                size="sm"
                                                                color={isGroup ? "primary" : "success"}
                                                            >
                                                                {item.phanLoai}
                                                            </Badge>
                                                        </div>
                                                    </div>
                                                </td>
                                                {[
                                                    ["index", "Index"],
                                                    ["create", "Create"],
                                                    ["edit", "Edit"],
                                                    ["delete", "Delete"],
                                                    ["approve", "Approve"],
                                                    ["public", "Public"],
                                                ].map(([key, label]) => (
                                                    <td
                                                        key={key}
                                                        className="px-3 py-4 text-center"
                                                    >
                                                        <PermissionCheckbox
                                                            label={label}
                                                            checked={item[key]}
                                                            disabled={disabled}
                                                            onChange={(value) =>
                                                                handlePermissionToggle(
                                                                    item,
                                                                    key,
                                                                    value
                                                                )
                                                            }
                                                        />
                                                    </td>
                                                ))}
                                            </tr>
                                        );
                                    })
                                ) : (
                                    <tr>
                                        <td
                                            colSpan="7"
                                            className="px-5 py-10 text-center text-sm text-gray-500 dark:text-gray-400"
                                        >
                                            Không có dữ liệu quyền chi tiết.
                                        </td>
                                    </tr>
                                )}
                                </tbody>
                            </table>
                        </div>

                        <PaginationFooter
                            pageCurrent={permissionPageCurrent}
                            pageSize={permissionPageSize}
                            totalCount={permissionTotalCount}
                            loading={permissionLoading}
                            onPageChange={setPermissionPageCurrent}
                        />
                    </div>
                </div>
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

