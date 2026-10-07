import {
    quanTriHeThongCommand,
    quanTriHeThongPagedRequest,
    quanTriHeThongRequest,
} from "../../../shared/api/quanTriHeThongApi";

const GROUP_PERMISSION_PATH = "/he-thong/nhom-quyen-truy-cap";

export function getGroupPermissions({
    search = "",
    pageSize = 10,
    pageCurrent = 1,
} = {}) {
    const params = new URLSearchParams({
        pageSize: String(pageSize),
        pageCurrent: String(pageCurrent),
    });

    if (search.trim()) {
        params.set("search", search.trim());
    }

    return quanTriHeThongPagedRequest(
        `${GROUP_PERMISSION_PATH}?${params.toString()}`
    );
}

export function getTemplateGroups() {
    return quanTriHeThongRequest(
        `${GROUP_PERMISSION_PATH}/template-groups`
    );
}

export function createGroupPermission(input) {
    return quanTriHeThongRequest(GROUP_PERMISSION_PATH, {
        method: "POST",
        body: input,
    });
}

export function updateGroupPermission(id, input) {
    return quanTriHeThongRequest(`${GROUP_PERMISSION_PATH}/${id}`, {
        method: "PUT",
        body: input,
    });
}

export function deleteGroupPermission(id) {
    return quanTriHeThongCommand(`${GROUP_PERMISSION_PATH}/${id}`, {
        method: "DELETE",
    });
}

export function getGroupPermissionDetails(
    groupId,
    {
        search = "",
        pageSize = 10,
        pageCurrent = 1,
    } = {}
) {
    const params = new URLSearchParams({
        pageSize: String(pageSize),
        pageCurrent: String(pageCurrent),
    });

    if (search.trim()) {
        params.set("search", search.trim());
    }

    return quanTriHeThongPagedRequest(
        `${GROUP_PERMISSION_PATH}/${groupId}/permissions?${params.toString()}`
    );
}

export function updateGroupPermissionDetail(groupId, permissionId, input) {
    return quanTriHeThongRequest(
        `${GROUP_PERMISSION_PATH}/${groupId}/permissions/${permissionId}`,
        {
            method: "PUT",
            body: input,
        }
    );
}

