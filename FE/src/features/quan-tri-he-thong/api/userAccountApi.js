import {
    quanTriHeThongCommand,
    quanTriHeThongPagedRequest,
    quanTriHeThongRequest,
} from "../../../shared/api/quanTriHeThongApi";

const USER_ACCOUNT_PATH = "/he-thong/tai-khoan-truy-cap";

export function getUserAccounts({
    search = "",
    pageSize = 10,
    pageCurrent = 1,
    level = "",
} = {}) {
    const params = new URLSearchParams({
        pageSize: String(pageSize),
        pageCurrent: String(pageCurrent),
    });

    if (search.trim()) {
        params.set("search", search.trim());
    }

    if (level.trim()) {
        params.set("level", level.trim());
    }

    return quanTriHeThongPagedRequest(
        `${USER_ACCOUNT_PATH}?${params.toString()}`
    );
}

export function getGroupPermissionOptions() {
    return quanTriHeThongRequest(
        `${USER_ACCOUNT_PATH}/group-permissions`
    );
}

export function updateUserAccount(id, input) {
    return quanTriHeThongRequest(`${USER_ACCOUNT_PATH}/${id}`, {
        method: "PUT",
        body: input,
    });
}

export function duplicateUserAccount(input) {
    return quanTriHeThongRequest(`${USER_ACCOUNT_PATH}/duplicate`, {
        method: "POST",
        body: input,
    });
}

export function resetUserAccountPassword(id) {
    return quanTriHeThongCommand(
        `${USER_ACCOUNT_PATH}/${id}/reset-password`,
        {
            method: "POST",
        }
    );
}

export function changeUserAccountStatus(id, status) {
    return quanTriHeThongCommand(
        `${USER_ACCOUNT_PATH}/${id}/change-status`,
        {
            method: "POST",
            body: {
                status,
            },
        }
    );
}

export function deleteUserAccount(id) {
    return quanTriHeThongCommand(`${USER_ACCOUNT_PATH}/${id}`, {
        method: "DELETE",
    });
}

