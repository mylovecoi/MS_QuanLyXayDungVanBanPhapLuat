import axiosClient from "../../../shared/api/axiosClient";

const DIA_DANH_PATH = "/api/danh-muc/dia-danh";

function unwrapResponse(response) {
    const payload = response.data;

    if (payload?.isSuccess === false) {
        throw new Error(payload?.message || "Thao tác không thành công.");
    }

    return payload;
}

export async function getDiaDanhs({
    search = "",
    pageSize = 10,
    pageCurrent = 1,
} = {}) {
    const response = await axiosClient.get(DIA_DANH_PATH, {
        params: {
            search: search.trim() || undefined,
            pageSize,
            pageCurrent,
        },
        baseURL: "",
    });

    return unwrapResponse(response);
}

export async function getDiaDanhOptions() {
    const response = await axiosClient.get(`${DIA_DANH_PATH}/options`, {
        baseURL: "",
    });

    return unwrapResponse(response).data ?? [];
}

export async function getNextDiaDanhSortOrder(parentId) {
    const response = await axiosClient.get(
        `${DIA_DANH_PATH}/next-sort-order`,
        {
            params: {
                parentId,
            },
            baseURL: "",
        }
    );

    return unwrapResponse(response).data ?? 1;
}

export async function createDiaDanh(input) {
    const response = await axiosClient.post(DIA_DANH_PATH, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function updateDiaDanh(id, input) {
    const response = await axiosClient.put(`${DIA_DANH_PATH}/${id}`, input, {
        baseURL: "",
    });

    return unwrapResponse(response).data;
}

export async function deleteDiaDanh(id) {
    const response = await axiosClient.delete(`${DIA_DANH_PATH}/${id}`, {
        baseURL: "",
    });

    return unwrapResponse(response);
}

