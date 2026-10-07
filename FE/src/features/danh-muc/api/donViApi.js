import axiosClient from "../../../shared/api/axiosClient";

const DON_VI_PATH = "/api/danh-muc/don-vi";

function unwrapResponse(response) {
    const payload = response.data;

    if (payload?.isSuccess === false) {
        throw new Error(payload?.message || "Thao tác không thành công.");
    }

    return payload;
}

export async function getDonVis({
    search = "",
    pageSize = 10,
    pageCurrent = 1,
} = {}) {
    const response = await axiosClient.get(DON_VI_PATH, {
        params: {
            search: search.trim() || undefined,
            pageSize,
            pageCurrent,
        },
        baseURL: "",
    });

    return unwrapResponse(response);
}

export async function getDonViOptions() {
    const response = await axiosClient.get(`${DON_VI_PATH}/options`, {
        baseURL: "",
    });

    return unwrapResponse(response).data ?? [];
}

export async function getNextDonViSortOrder(donViChuQuanId) {
    const response = await axiosClient.get(`${DON_VI_PATH}/next-sort-order`, {
        params: {
            donViChuQuanId,
        },
        baseURL: "",
    });

    return unwrapResponse(response).data ?? 1;
}

export async function createDonVi(input) {
    const response = await axiosClient.post(DON_VI_PATH, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function updateDonVi(id, input) {
    const response = await axiosClient.put(`${DON_VI_PATH}/${id}`, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function deleteDonVi(id) {
    const response = await axiosClient.delete(`${DON_VI_PATH}/${id}`, {
        baseURL: "",
    });

    return unwrapResponse(response);
}
