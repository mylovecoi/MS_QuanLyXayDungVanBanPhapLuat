import {
    quanTriHeThongCommand,
    quanTriHeThongPagedRequest,
    quanTriHeThongRequest,
} from "../../../shared/api/quanTriHeThongApi";

export function getRoleActions(
    search = "",
    pageSize = 50,
    pageCurrent = 1
) {
    const params = new URLSearchParams({
        pageSize: String(pageSize),
        pageCurrent: String(pageCurrent),
    });

    if (search.trim()) {
        params.set("search", search.trim());
    }

    return quanTriHeThongPagedRequest(
        `/he-thong/danh-sach-chuc-nang?${params.toString()}`
    );
}

export function updateRoleAction(input) {
    return quanTriHeThongRequest(
        `/he-thong/danh-sach-chuc-nang/${input.id}`,
        {
            method: "PUT",
            body: {
                id: input.id,
                sttSapXep: input.sttSapXep,
                phanLoai: input.phanLoai,
                role: input.role,
                parentId: input.parentId,
                title: input.title,
                controller: input.controller,
                action: input.action,
                parameter: input.parameter,
                table: input.table,
                status: input.status,
                useGroup: input.useGroup,
                frontendPath: input.frontendPath,
                isVisibleInMenu: input.isVisibleInMenu,
                clientApp: input.clientApp,
                menuTitle: input.menuTitle,
                menuIcon: input.menuIcon,
                icon: input.icon,
            },
        }
    );
}

export function deleteRoleAction(id) {
    return quanTriHeThongCommand(
        `/he-thong/danh-sach-chuc-nang/${id}`,
        {
            method: "DELETE",
        }
    );
}