import { useEffect, useState } from "react";
import {
    deleteRoleAction,
    getRoleActions,
    updateRoleAction,
} from "../features/quan-tri-he-thong/api/roleActionApi";
const formatValue = (value) => {
    if (!value) {
        return "-";
    }

    return value;
};

function StatusBadge({ item }) {
    return (
        <span
            className={
                item.status === "Kích hoạt"
                    ? "status-badge active"
                    : "status-badge"
            }
        >
            {item.status}
        </span>
    );
}

const toInputValue = (value) => value ?? "";

export function RoleActionListPage() {
    const [items, setItems] = useState([]);
    const [search, setSearch] = useState("");
    const [pageSize, setPageSize] = useState("10");
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState("");
    const [successMessage, setSuccessMessage] = useState("");
    const [totalCount, setTotalCount] = useState(0);
    const [modal, setModal] = useState(null);
    const [editForm, setEditForm] = useState(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [reloadKey, setReloadKey] = useState(0);

    useEffect(() => {
        let isMounted = true;

        async function loadData() {
            setIsLoading(true);
            setErrorMessage("");

            try {
                const result = await getRoleActions(
                    search,
                    pageSize === "all" ? 1000 : Number(pageSize),
                    1
                );

                if (!isMounted) {
                    return;
                }

                setItems(result.data);
                setTotalCount(result.totalCount);
            } catch (error) {
                if (!isMounted) {
                    return;
                }

                setItems([]);
                setTotalCount(0);

                setErrorMessage(
                    error instanceof Error
                        ? error.message
                        : "Không tải được danh sách chức năng."
                );
            } finally {
                if (isMounted) {
                    setIsLoading(false);
                }
            }
        }

        const timeoutId = window.setTimeout(loadData, 250);

        return () => {
            isMounted = false;
            window.clearTimeout(timeoutId);
        };
    }, [pageSize, reloadKey, search]);

    const visibleMenuCount = items.filter(
        (item) => item.isVisibleInMenu
    ).length;

    const frontendCount = items.filter(
        (item) => item.frontendPath
    ).length;

    const openEditModal = (item) => {
        setSuccessMessage("");
        setErrorMessage("");

        setEditForm({
            id: item.id,
            sttSapXep: item.sttSapXep,
            phanLoai: item.phanLoai,
            role: item.role,
            parentId: item.parentId,
            title: item.title,
            controller: item.controller,
            action: item.action,
            parameter: item.parameter,
            table: item.table,
            status: item.status,
            useGroup: item.useGroup,
            frontendPath: item.frontendPath,
            isVisibleInMenu: item.isVisibleInMenu,
            clientApp: item.clientApp,
            menuTitle: item.menuTitle,
            menuIcon: item.menuIcon,
            icon: item.icon,
        });

        setModal({
            type: "edit",
            item,
        });
    };

    const openDeleteModal = (item) => {
        setSuccessMessage("");
        setErrorMessage("");

        setModal({
            type: "delete",
            item,
        });
    };

    const closeModal = () => {
        setModal(null);
        setEditForm(null);
        setIsSubmitting(false);
    };

    const handleEditSubmit = async (event) => {
        event.preventDefault();

        if (!editForm) {
            return;
        }

        setIsSubmitting(true);
        setErrorMessage("");
        setSuccessMessage("");

        try {
            await updateRoleAction(editForm);

            setSuccessMessage(
                "Cập nhật chức năng thành công."
            );

            setReloadKey((current) => current + 1);

            closeModal();
        } catch (error) {
            setErrorMessage(
                error instanceof Error
                    ? error.message
                    : "Không cập nhật được chức năng."
            );

            setIsSubmitting(false);
        }
    };

    const handleDeleteConfirm = async () => {
        if (modal?.type !== "delete") {
            return;
        }

        setIsSubmitting(true);
        setErrorMessage("");
        setSuccessMessage("");

        try {
            await deleteRoleAction(modal.item.id);

            setSuccessMessage(
                "Xóa chức năng thành công."
            );

            setReloadKey((current) => current + 1);

            closeModal();
        } catch (error) {
            setErrorMessage(
                error instanceof Error
                    ? error.message
                    : "Không xóa được chức năng."
            );

            setIsSubmitting(false);
        }
    };

    return (
        <section className="role-page">
            <div className="page-heading">
                <div>
                    <p className="eyebrow">
                        Quản trị hệ thống
                    </p>

                    <h2>Danh sách chức năng</h2>

                    <p>
                        Quản lý chức năng, đường dẫn frontend và
                        cách hiển thị trên menu hệ thống.
                    </p>
                </div>

                <div className="page-actions">
                    <button
                        className="secondary-button"
                        type="button"
                    >
                        Nhập mới
                    </button>

                    <button
                        className="primary-button compact"
                        type="button"
                    >
                        Thêm chức năng
                    </button>
                </div>
            </div>

            <div className="metric-grid">
                <article>
                    <span>Tổng số</span>
                    <strong>{totalCount}</strong>
                </article>

                <article>
                    <span>Hiển thị menu</span>
                    <strong>{visibleMenuCount}</strong>
                </article>

                <article>
                    <span>Có đường dẫn FE</span>
                    <strong>{frontendCount}</strong>
                </article>
            </div>

            <div className="role-list-panel">
                <div className="toolbar">
                    <input
                        aria-label="Tìm kiếm chức năng"
                        onChange={(event) =>
                            setSearch(event.target.value)
                        }
                        placeholder="Tìm theo tên, mã role, controller..."
                        type="search"
                        value={search}
                    />

                    <label className="page-size-control">
                        <span>Hiển thị</span>

                        <select
                            onChange={(event) =>
                                setPageSize(event.target.value)
                            }
                            value={pageSize}
                        >
                            <option value="10">10</option>
                            <option value="20">20</option>
                            <option value="all">
                                Tất cả
                            </option>
                        </select>
                    </label>
                </div>

                {successMessage ? (
                    <p className="success-message">
                        {successMessage}
                    </p>
                ) : null}

                {errorMessage ? (
                    <p className="menu-error">
                        {errorMessage}
                    </p>
                ) : null}

                <div className="table-wrap">
                    <table className="data-table">
                        <thead>
                        <tr>
                            <th>STT</th>
                            <th>Chức năng</th>
                            <th>Role</th>
                            <th>Loại</th>
                            <th>Menu</th>
                            <th>Trạng thái</th>
                            <th>Thao tác</th>
                        </tr>
                        </thead>

                        <tbody>
                        {isLoading ? (
                            <tr>
                                <td colSpan={7}>
                                    Đang tải dữ liệu...
                                </td>
                            </tr>
                        ) : null}

                        {!isLoading &&
                        items.length === 0 ? (
                            <tr>
                                <td colSpan={7}>
                                    Không có dữ liệu phù hợp.
                                </td>
                            </tr>
                        ) : null}

                        {!isLoading
                            ? items.map((item) => (
                                <tr key={item.id}>
                                    <td>
                                        {item.sttSapXep}
                                    </td>

                                    <td>
                                        <strong>
                                            {item.title ||
                                                item.menuTitle ||
                                                item.role}
                                        </strong>

                                        <small>
                                            {formatValue(
                                                item.frontendPath
                                            )}
                                        </small>
                                    </td>

                                    <td>{item.role}</td>

                                    <td>
                                        {item.phanLoai}
                                    </td>

                                    <td>
                                        {item.isVisibleInMenu
                                            ? "Có"
                                            : "Không"}
                                    </td>

                                    <td>
                                        <StatusBadge
                                            item={item}
                                        />
                                    </td>

                                    <td>
                                        <div className="row-actions">
                                            <button
                                                className="table-action edit"
                                                onClick={() =>
                                                    openEditModal(
                                                        item
                                                    )
                                                }
                                                type="button"
                                            >
                                                Chỉnh sửa
                                            </button>

                                            <button
                                                className="table-action delete"
                                                onClick={() =>
                                                    openDeleteModal(
                                                        item
                                                    )
                                                }
                                                type="button"
                                            >
                                                Xóa
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))
                            : null}
                        </tbody>
                    </table>
                </div>
            </div>

            {modal?.type === "edit" &&
            editForm ? (
                <div
                    className="modal-backdrop"
                    role="presentation"
                >
                    <form
                        className="modal-card edit-modal"
                        onSubmit={handleEditSubmit}
                    >
                        <div className="modal-header">
                            <div>
                                <p className="eyebrow">
                                    Chỉnh sửa
                                </p>

                                <h3>
                                    {modal.item.title ||
                                        modal.item.role}
                                </h3>
                            </div>

                            <button
                                className="icon-button"
                                onClick={closeModal}
                                type="button"
                                aria-label="Đóng"
                            >
                                ×
                            </button>
                        </div>

                        <div className="form-grid">
                            <label>
                                <span>
                                    Tên chức năng
                                </span>

                                <input
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            title: event.target.value,
                                        })
                                    }
                                    value={toInputValue(
                                        editForm.title
                                    )}
                                />
                            </label>

                            <label>
                                <span>
                                    Menu title
                                </span>

                                <input
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            menuTitle:
                                            event.target.value,
                                        })
                                    }
                                    value={toInputValue(
                                        editForm.menuTitle
                                    )}
                                />
                            </label>

                            <label>
                                <span>Role</span>

                                <input
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            role: event.target.value,
                                        })
                                    }
                                    value={editForm.role}
                                />
                            </label>

                            <label>
                                <span>
                                    Đường dẫn FE
                                </span>

                                <input
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            frontendPath:
                                            event.target.value,
                                        })
                                    }
                                    value={toInputValue(
                                        editForm.frontendPath
                                    )}
                                />
                            </label>

                            <label>
                                <span>
                                    Controller
                                </span>

                                <input
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            controller:
                                            event.target.value,
                                        })
                                    }
                                    value={toInputValue(
                                        editForm.controller
                                    )}
                                />
                            </label>

                            <label>
                                <span>Action</span>

                                <input
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            action:
                                            event.target.value,
                                        })
                                    }
                                    value={toInputValue(
                                        editForm.action
                                    )}
                                />
                            </label>

                            <label>
                                <span>Loại</span>

                                <select
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            phanLoai:
                                            event.target.value,
                                        })
                                    }
                                    value={
                                        editForm.phanLoai
                                    }
                                >
                                    <option value="Group">
                                        Group
                                    </option>

                                    <option value="Detail">
                                        Detail
                                    </option>
                                </select>
                            </label>

                            <label>
                                <span>
                                    Trạng thái
                                </span>

                                <select
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            status:
                                            event.target.value,
                                        })
                                    }
                                    value={
                                        editForm.status
                                    }
                                >
                                    <option value="Kích hoạt">
                                        Kích hoạt
                                    </option>

                                    <option value="Không kích hoạt">
                                        Không kích hoạt
                                    </option>
                                </select>
                            </label>

                            <label>
                                <span>Thứ tự</span>

                                <input
                                    min="0"
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            sttSapXep:
                                                Number(
                                                    event.target.value
                                                ),
                                        })
                                    }
                                    type="number"
                                    value={
                                        editForm.sttSapXep
                                    }
                                />
                            </label>

                            <label className="checkbox-field">
                                <input
                                    checked={
                                        editForm.isVisibleInMenu
                                    }
                                    onChange={(event) =>
                                        setEditForm({
                                            ...editForm,
                                            isVisibleInMenu:
                                            event.target
                                                .checked,
                                        })
                                    }
                                    type="checkbox"
                                />

                                <span>
                                    Hiển thị trên menu
                                </span>
                            </label>
                        </div>

                        <div className="modal-actions">
                            <button
                                className="secondary-button"
                                onClick={closeModal}
                                type="button"
                            >
                                Hủy
                            </button>

                            <button
                                className="primary-button compact"
                                disabled={isSubmitting}
                                type="submit"
                            >
                                {isSubmitting
                                    ? "Đang lưu..."
                                    : "Lưu thay đổi"}
                            </button>
                        </div>
                    </form>
                </div>
            ) : null}

            {modal?.type === "delete" ? (
                <div
                    className="modal-backdrop"
                    role="presentation"
                >
                    <div className="modal-card confirm-modal">
                        <div className="modal-header">
                            <div>
                                <p className="eyebrow">
                                    Xóa chức năng
                                </p>

                                <h3>
                                    {modal.item.title ||
                                        modal.item.role}
                                </h3>
                            </div>

                            <button
                                className="icon-button"
                                onClick={closeModal}
                                type="button"
                                aria-label="Đóng"
                            >
                                ×
                            </button>
                        </div>

                        <p>
                            Bạn có chắc chắn muốn xóa chức
                            năng này? Thao tác này có thể ảnh
                            hưởng đến phân quyền và menu
                            người dùng.
                        </p>

                        <div className="modal-actions">
                            <button
                                className="secondary-button"
                                onClick={closeModal}
                                type="button"
                            >
                                Hủy
                            </button>

                            <button
                                className="danger-button"
                                disabled={isSubmitting}
                                onClick={
                                    handleDeleteConfirm
                                }
                                type="button"
                            >
                                {isSubmitting
                                    ? "Đang xóa..."
                                    : "Xóa chức năng"}
                            </button>
                        </div>
                    </div>
                </div>
            ) : null}
        </section>
    );
}