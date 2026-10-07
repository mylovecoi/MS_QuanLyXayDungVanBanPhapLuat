import axiosClient from "../../../shared/api/axiosClient";

const VAN_BAN_PATH = "/api/danh-muc/van-ban";

function unwrapResponse(response) {
    const payload = response.data;

    if (payload?.isSuccess === false) {
        throw new Error(payload?.message || "Thao tác không thành công.");
    }

    return payload;
}

export async function getVanBans({
    search = "",
    pageSize = 10,
    pageCurrent = 1,
} = {}) {
    const response = await axiosClient.get(VAN_BAN_PATH, {
        params: {
            search: search.trim() || undefined,
            pageSize,
            pageCurrent,
        },
        baseURL: "",
    });

    return unwrapResponse(response);
}

export async function getNextVanBanSortOrder() {
    const response = await axiosClient.get(`${VAN_BAN_PATH}/next-sort-order`, {
        baseURL: "",
    });

    return unwrapResponse(response).data ?? 1;
}

export async function createVanBan(input) {
    const response = await axiosClient.post(VAN_BAN_PATH, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function updateVanBan(id, input) {
    const response = await axiosClient.put(`${VAN_BAN_PATH}/${id}`, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function deleteVanBan(id) {
    const response = await axiosClient.delete(`${VAN_BAN_PATH}/${id}`, {
        baseURL: "",
    });

    return unwrapResponse(response);
}
