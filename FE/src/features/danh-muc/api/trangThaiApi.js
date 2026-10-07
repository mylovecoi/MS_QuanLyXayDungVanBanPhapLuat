import axiosClient from "../../../shared/api/axiosClient";

const TRANG_THAI_PATH = "/api/danh-muc/trang-thai";

function unwrapResponse(response) {
    const payload = response.data;

    if (payload?.isSuccess === false) {
        throw new Error(payload?.message || "Thao tác không thành công.");
    }

    return payload;
}

export async function getTrangThais({
    search = "",
    nhomTrangThai = "",
    pageSize = 10,
    pageCurrent = 1,
} = {}) {
    const response = await axiosClient.get(TRANG_THAI_PATH, {
        params: {
            search: search.trim() || undefined,
            nhomTrangThai: nhomTrangThai.trim() || undefined,
            pageSize,
            pageCurrent,
        },
        baseURL: "",
    });

    return unwrapResponse(response);
}

export async function getNextTrangThaiSortOrder() {
    const response = await axiosClient.get(
        `${TRANG_THAI_PATH}/next-sort-order`,
        {
            baseURL: "",
        }
    );

    return unwrapResponse(response).data ?? 1;
}

export async function createTrangThai(input) {
    const response = await axiosClient.post(TRANG_THAI_PATH, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function updateTrangThai(id, input) {
    const response = await axiosClient.put(`${TRANG_THAI_PATH}/${id}`, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function deleteTrangThai(id) {
    const response = await axiosClient.delete(`${TRANG_THAI_PATH}/${id}`, {
        baseURL: "",
    });

    return unwrapResponse(response);
}
