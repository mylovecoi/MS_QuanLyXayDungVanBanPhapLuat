import {useEffect, useMemo, useState} from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {Modal} from "../../../app/components/ui/modal";
import {
    changeUserAccountStatus,
    deleteUserAccount,
    duplicateUserAccount,
    getGroupPermissionOptions,
    getUserAccounts,
    resetUserAccountPassword,
    updateUserAccount,
} from "../api/userAccountApi";

const statusOptions = [
    {value: "Kích hoạt", label: "Kích hoạt"},
    {value: "Chờ kích hoạt", label: "Chờ kích hoạt"},
    {value: "Khóa", label: "Khóa"},
];

const contentOptions = [
    {value: "Fixted", label: "Fixted"},
    {value: "Max", label: "Max"},
];

const levelOptions = [
    {value: "__all", label: "Tất cả cấp"},
    {value: "Nhà nước", label: "Nhà nước"},
    {value: "Tỉnh", label: "Tỉnh"},
    {value: "Huyện", label: "Huyện"},
    {value: "Xã", label: "Xã"},
];

const emptyEditForm = {
    id: "",
    username: "",
    name: "",
    email: "",
    status: "Kích hoạt",
    password: "",
    content: "Fixted",
    groupPermissionId: "",
};

const emptyDuplicateForm = {
    sourceUserId: "",
    sourceUsername: "",
    username: "",
    name: "",
    email: "",
};

function getErrorMessage(error, fallback) {
    return (
        error?.response?.data?.message ||
        error?.message ||
        fallback
    );
}

function statusColor(status) {
    if (status === "Kích hoạt") {
        return "success";
    }

    if (status === "Chờ kích hoạt") {
        return "warning";
    }

    return "error";
}

function ActionButton({children, onClick, tone = "default", disabled}) {
    const toneClasses = {
        default: "text-gray-500 hover:bg-gray-100 hover:text-brand-500 dark:text-gray-400 dark:hover:bg-white/[0.05]",
        danger: "text-gray-500 hover:bg-gray-100 hover:text-error-500 dark:text-gray-400 dark:hover:bg-white/[0.05]",
        success: "text-gray-500 hover:bg-gray-100 hover:text-success-500 dark:text-gray-400 dark:hover:bg-white/[0.05]",
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

export default function UserAccountPage() {
    const [items, setItems] = useState([]);
    const [groupOptions, setGroupOptions] = useState([]);
    const [search, setSearch] = useState("");
    const [level, setLevel] = useState("");
    const [pageSize, setPageSize] = useState(10);
    const [pageCurrent, setPageCurrent] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [editForm, setEditForm] = useState(emptyEditForm);
    const [duplicateForm, setDuplicateForm] = useState(emptyDuplicateForm);
    const [confirmAction, setConfirmAction] = useState(null);

    const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

    const paginationPages = useMemo(() => {
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

    const permissionSelectOptions = useMemo(
        () =>
            groupOptions.map((item) => ({
                value: item.value,
                label: item.displayName,
            })),
        [groupOptions]
    );

    const fetchAccounts = async () => {
        try {
            setLoading(true);
            setError("");

            const response = await getUserAccounts({
                search,
                level,
                pageSize,
                pageCurrent,
            });

            setItems(response?.data ?? []);
            setTotalCount(response?.totalCount ?? 0);
        } catch (fetchError) {
            setError(
                getErrorMessage(
                    fetchError,
                    "Không thể tải danh sách tài khoản truy cập."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void fetchAccounts();
    }, [search, level, pageSize, pageCurrent]);

    useEffect(() => {
        const fetchOptions = async () => {
            try {
                const data = await getGroupPermissionOptions();
                setGroupOptions(data ?? []);
            } catch (optionError) {
                setError(
                    getErrorMessage(
                        optionError,
                        "Không thể tải danh sách nhóm quyền."
                    )
                );
            }
        };

        void fetchOptions();
    }, []);

    const openEditModal = (item) => {
        setEditForm({
            id: item.id,
            username: item.username,
            name: item.name ?? "",
            email: item.email ?? "",
            status: item.status || "Kích hoạt",
            password: "",
            content: item.content || "Fixted",
            groupPermissionId: item.groupPermissionId || "",
        });
        setSuccess("");
        setError("");
    };

    const openDuplicateModal = (item) => {
        setDuplicateForm({
            sourceUserId: item.id,
            sourceUsername: item.username,
            username: `${item.username}_copy`,
            name: item.name ? `${item.name} copy` : "",
            email: "",
        });
        setSuccess("");
        setError("");
    };

    const closeEditModal = () => {
        if (!submitting) {
            setEditForm(emptyEditForm);
        }
    };

    const closeDuplicateModal = () => {
        if (!submitting) {
            setDuplicateForm(emptyDuplicateForm);
        }
    };

    const validateEdit = () => {
        if (!editForm.name.trim()) {
            return "Tên tài khoản không được để trống.";
        }

        if (!editForm.email.trim()) {
            return "Email không được để trống.";
        }

        if (!editForm.groupPermissionId) {
            return "Vui lòng chọn nhóm quyền truy cập.";
        }

        return "";
    };

    const validateDuplicate = () => {
        if (
            !duplicateForm.username.trim() ||
            !duplicateForm.name.trim() ||
            !duplicateForm.email.trim()
        ) {
            return "Thông tin nhân bản không được để trống.";
        }

        return "";
    };

    const handleEditSubmit = async (event) => {
        event.preventDefault();

        const validationMessage = validateEdit();
        if (validationMessage) {
            setError(validationMessage);
            setSuccess("");
            return;
        }

        try {
            setSubmitting(true);
            setError("");
            setSuccess("");

            await updateUserAccount(editForm.id, {
                name: editForm.name.trim(),
                email: editForm.email.trim(),
                status: editForm.status,
                password: editForm.password.trim() || null,
                content: editForm.content,
                groupPermissionId: editForm.groupPermissionId,
            });

            setSuccess("Cập nhật tài khoản truy cập thành công.");
            setEditForm(emptyEditForm);
            await fetchAccounts();
        } catch (submitError) {
            setError(
                getErrorMessage(
                    submitError,
                    "Không thể cập nhật tài khoản truy cập."
                )
            );
        } finally {
            setSubmitting(false);
        }
    };

    const handleDuplicateSubmit = async (event) => {
        event.preventDefault();

        const validationMessage = validateDuplicate();
        if (validationMessage) {
            setError(validationMessage);
            setSuccess("");
            return;
        }

        try {
            setSubmitting(true);
            setError("");
            setSuccess("");

            await duplicateUserAccount({
                sourceUserId: duplicateForm.sourceUserId,
                username: duplicateForm.username.trim(),
                name: duplicateForm.name.trim(),
                email: duplicateForm.email.trim(),
            });

            setSuccess("Nhân bản tài khoản truy cập thành công.");
            setDuplicateForm(emptyDuplicateForm);
            await fetchAccounts();
        } catch (submitError) {
            setError(
                getErrorMessage(
                    submitError,
                    "Không thể nhân bản tài khoản truy cập."
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
            await fetchAccounts();
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

    const handleLevelChange = (value) => {
        setLevel(value === "__all" ? "" : value);
        setPageCurrent(1);
    };

    const handlePageSizeChange = (value) => {
        setPageSize(Number(value));
        setPageCurrent(1);
    };

    return (
        <div>
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                        Tài khoản truy cập
                    </h3>

                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Quản lý người dùng, nhóm quyền và trạng thái truy cập hệ thống
                    </p>
                </div>
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

                <div
                    className="
                        overflow-hidden
                        rounded-xl
                        border
                        border-gray-200
                        bg-white
                        dark:border-white/[0.05]
                        dark:bg-white/[0.03]
                    "
                >
                    <div
                        className="
                            grid
                            grid-cols-1
                            gap-4
                            border-b
                            border-gray-100
                            px-5
                            py-4
                            sm:grid-cols-12
                            dark:border-white/[0.05]
                        "
                    >
                        <div className="w-full sm:col-span-3">
                            <Label>
                                Hiển Thị
                            </Label>
                            <Select
                                value={pageSize}
                                onChange={handlePageSizeChange}
                                options={[10, 20, 50, 100].map((value) => ({
                                    value,
                                    label: `${value} thông tin`,
                                }))}
                            />
                        </div>

                        <div className="w-full sm:col-span-3">
                            <Label>
                                Cấp tài khoản
                            </Label>
                            <Select
                                value={level || "__all"}
                                onChange={handleLevelChange}
                                options={levelOptions}
                            />
                        </div>

                        <div className="w-full sm:col-span-6">
                            <Label>
                                Tìm Kiếm
                            </Label>
                            <Input
                                value={search}
                                onChange={(event) =>
                                    handleSearchChange(event.target.value)
                                }
                                placeholder="Tìm theo username, họ tên hoặc email..."
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
                                    Tài khoản
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Nhóm quyền
                                </th>
                                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                    Cấp / Content
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
                                            <div className="font-medium text-gray-800 dark:text-white/90">
                                                {item.name || item.username}
                                            </div>
                                            <div className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                                                {item.username}
                                            </div>
                                            <div className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                                                {item.email}
                                            </div>
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            {item.groupPermissionName || "Chưa có nhóm quyền"}
                                        </td>
                                        <td className="px-5 py-4 text-sm text-gray-500 dark:text-gray-400">
                                            <div>{item.level || "Không xác định"}</div>
                                            <div className="mt-1 text-xs">
                                                Content: {item.content || "Fixted"}
                                            </div>
                                            {item.firstLogin && (
                                                <div className="mt-2">
                                                    <Badge size="sm" color="warning">
                                                        Lần đầu đăng nhập
                                                    </Badge>
                                                </div>
                                            )}
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
                                                    onClick={() => openEditModal(item)}
                                                >
                                                    Sửa
                                                </ActionButton>
                                                <ActionButton
                                                    onClick={() => openDuplicateModal(item)}
                                                >
                                                    Nhân bản
                                                </ActionButton>
                                                <ActionButton
                                                    onClick={() =>
                                                        setConfirmAction({
                                                            title: "Đặt lại mật khẩu",
                                                            message: `Đặt lại mật khẩu mặc định cho tài khoản ${item.username}?`,
                                                            action: () =>
                                                                resetUserAccountPassword(item.id),
                                                            successMessage:
                                                                "Đặt lại mật khẩu mặc định thành công.",
                                                            errorMessage:
                                                                "Không thể đặt lại mật khẩu.",
                                                        })
                                                    }
                                                >
                                                    Reset
                                                </ActionButton>
                                                <ActionButton
                                                    tone={
                                                        item.status === "Kích hoạt"
                                                            ? "danger"
                                                            : "success"
                                                    }
                                                    onClick={() => {
                                                        const nextStatus =
                                                            item.status === "Kích hoạt"
                                                                ? "Khóa"
                                                                : "Kích hoạt";

                                                        setConfirmAction({
                                                            title: "Cập nhật trạng thái",
                                                            message: `Chuyển tài khoản ${item.username} sang trạng thái "${nextStatus}"?`,
                                                            action: () =>
                                                                changeUserAccountStatus(
                                                                    item.id,
                                                                    nextStatus
                                                                ),
                                                            successMessage:
                                                                "Cập nhật trạng thái tài khoản thành công.",
                                                            errorMessage:
                                                                "Không thể cập nhật trạng thái tài khoản.",
                                                        });
                                                    }}
                                                >
                                                    {item.status === "Kích hoạt"
                                                        ? "Khóa"
                                                        : "Mở"}
                                                </ActionButton>
                                                <ActionButton
                                                    tone="danger"
                                                    onClick={() =>
                                                        setConfirmAction({
                                                            title: "Xóa tài khoản",
                                                            message: `Xóa tài khoản ${item.username}? Thao tác này không thể hoàn tác.`,
                                                            action: () =>
                                                                deleteUserAccount(item.id),
                                                            successMessage:
                                                                "Xóa tài khoản truy cập thành công.",
                                                            errorMessage:
                                                                "Không thể xóa tài khoản truy cập.",
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
                                        Không có dữ liệu tài khoản truy cập.
                                    </td>
                                </tr>
                            )}
                            </tbody>
                        </table>
                    </div>

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
                                onClick={() =>
                                    setPageCurrent((current) =>
                                        Math.max(1, current - 1)
                                    )
                                }
                                disabled={pageCurrent === 1 || loading}
                                className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                            >
                                Previous
                            </button>

                            <div className="flex items-center gap-1">
                                {paginationPages[0] > 1 && (
                                    <>
                                        <button
                                            type="button"
                                            onClick={() => setPageCurrent(1)}
                                            disabled={loading}
                                            className="h-9 min-w-9 rounded-lg px-3 text-sm font-medium text-gray-700 hover:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                                        >
                                            1
                                        </button>

                                        {paginationPages[0] > 2 && (
                                            <span className="px-1 text-sm text-gray-400">
                                                ...
                                            </span>
                                        )}
                                    </>
                                )}

                                {paginationPages.map((page) => (
                                    <button
                                        key={page}
                                        type="button"
                                        onClick={() => setPageCurrent(page)}
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

                                {paginationPages[paginationPages.length - 1] < totalPages && (
                                    <>
                                        {paginationPages[paginationPages.length - 1] < totalPages - 1 && (
                                            <span className="px-1 text-sm text-gray-400">
                                                ...
                                            </span>
                                        )}

                                        <button
                                            type="button"
                                            onClick={() =>
                                                setPageCurrent(totalPages)
                                            }
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
                                    setPageCurrent((current) =>
                                        Math.min(totalPages, current + 1)
                                    )
                                }
                                disabled={pageCurrent >= totalPages || loading}
                                className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                            >
                                Next
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <Modal
                isOpen={Boolean(editForm.id)}
                onClose={closeEditModal}
                className="max-w-2xl p-6"
            >
                <form onSubmit={handleEditSubmit} className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            Cập nhật tài khoản
                        </h4>
                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            {editForm.username}
                        </p>
                    </div>

                    <div className="grid gap-4 sm:grid-cols-2">
                        <div>
                            <Label>Họ tên</Label>
                            <Input
                                value={editForm.name}
                                onChange={(event) =>
                                    setEditForm((current) => ({
                                        ...current,
                                        name: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Email</Label>
                            <Input
                                value={editForm.email}
                                onChange={(event) =>
                                    setEditForm((current) => ({
                                        ...current,
                                        email: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>

                        <div>
                            <Label>Trạng thái</Label>
                            <Select
                                value={editForm.status}
                                options={statusOptions}
                                onChange={(value) =>
                                    setEditForm((current) => ({
                                        ...current,
                                        status: value,
                                    }))
                                }
                            />
                        </div>

                        <div>
                            <Label>Content</Label>
                            <Select
                                value={editForm.content}
                                options={contentOptions}
                                onChange={(value) =>
                                    setEditForm((current) => ({
                                        ...current,
                                        content: value,
                                    }))
                                }
                            />
                        </div>

                        <div>
                            <Label>Nhóm quyền</Label>
                            <Select
                                value={editForm.groupPermissionId}
                                options={permissionSelectOptions}
                                placeholder="Chọn nhóm quyền"
                                onChange={(value) =>
                                    setEditForm((current) => ({
                                        ...current,
                                        groupPermissionId: value,
                                    }))
                                }
                            />
                        </div>

                        <div>
                            <Label>Mật khẩu mới</Label>
                            <Input
                                type="password"
                                value={editForm.password}
                                onChange={(event) =>
                                    setEditForm((current) => ({
                                        ...current,
                                        password: event.target.value,
                                    }))
                                }
                                placeholder="Bỏ trống nếu không đổi"
                                disabled={submitting}
                            />
                        </div>
                    </div>

                    <div className="flex justify-end gap-3">
                        <button
                            type="button"
                            onClick={closeEditModal}
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
                isOpen={Boolean(duplicateForm.sourceUserId)}
                onClose={closeDuplicateModal}
                className="max-w-xl p-6"
            >
                <form onSubmit={handleDuplicateSubmit} className="space-y-5">
                    <div>
                        <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            Nhân bản tài khoản
                        </h4>
                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            Từ tài khoản {duplicateForm.sourceUsername}
                        </p>
                    </div>

                    <div className="space-y-4">
                        <div>
                            <Label>Username mới</Label>
                            <Input
                                value={duplicateForm.username}
                                onChange={(event) =>
                                    setDuplicateForm((current) => ({
                                        ...current,
                                        username: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>
                        <div>
                            <Label>Họ tên</Label>
                            <Input
                                value={duplicateForm.name}
                                onChange={(event) =>
                                    setDuplicateForm((current) => ({
                                        ...current,
                                        name: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>
                        <div>
                            <Label>Email</Label>
                            <Input
                                value={duplicateForm.email}
                                onChange={(event) =>
                                    setDuplicateForm((current) => ({
                                        ...current,
                                        email: event.target.value,
                                    }))
                                }
                                disabled={submitting}
                            />
                        </div>
                    </div>

                    <div className="flex justify-end gap-3">
                        <button
                            type="button"
                            onClick={closeDuplicateModal}
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
                            {submitting ? "Đang nhân bản..." : "Nhân bản"}
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

