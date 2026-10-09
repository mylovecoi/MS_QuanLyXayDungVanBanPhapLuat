import axiosClient from "../../../shared/api/axiosClient";

const BASE_URL = "/api/dang-ky-xay-dung-van-ban/ho-so";
const LIST_URL = "/api/dang-ky-xay-dung-van-ban/danh-sach";
const RESULT_URL = "/api/dang-ky-xay-dung-van-ban/ket-qua";

function unwrapResponse(response) {
    const payload = response.data;

    if (payload.isSuccess === false) {
        throw new Error(payload?.message || "Thao tác không thành công.");
    }
    return payload;
}

export async function getDangKyXayDungVanBanById(id) {
    const response = await axiosClient.get(`${BASE_URL}/${id}`, {
        baseURL: "",
    });
    return unwrapResponse(response).data;
}

export async function getDanhSachDangKyXayDungVanBan(params = {}) {
    const response = await axiosClient.get(LIST_URL, {
        baseURL: "",
        params,
    });
    return unwrapResponse(response);
}

export async function getKetQuaDangKyXayDungVanBan(params = {}) {
    const response = await axiosClient.get(RESULT_URL, {
        baseURL: "",
        params,
    });
    return unwrapResponse(response);
}

export async function createDangKyXayDungVanBan(data) {
    const response = await axiosClient.post(BASE_URL, data, {
        baseURL: "",
    });
    return unwrapResponse(response).data;
}

export async function updateDangKyXayDungVanBan(id, data) {
    const response = await axiosClient.put(`${BASE_URL}/${id}`, data, {
        baseURL: "",
    });
    return unwrapResponse(response);
}

export async function deleteDangKyXayDungVanBan(id) {
    const response = await axiosClient.delete(`${BASE_URL}/${id}`, {
        baseURL: "",
    });
    return unwrapResponse(response);
}

export async function getDangKyXayDungVanBanTimeline(id) {
    const response = await axiosClient.get(`${BASE_URL}/${id}/timeline`, {
        baseURL: "",
    });
    return unwrapResponse(response).data;
}

export async function getHanhDongKhaDung(id) {
    const response = await axiosClient.get(
        `${BASE_URL}/${id}/hanh-dong-kha-dung`,
        {baseURL: ""}
    );
    return unwrapResponse(response).data;
}

export async function getDangKyXayDungVanBanFiles(id) {
    const response = await axiosClient.get(`${BASE_URL}/${id}/files`, {
        baseURL: "",
    });
    return unwrapResponse(response).data;
}

export async function uploadDangKyXayDungVanBanFile(id, formData) {
    const response = await axiosClient.post(
        `${BASE_URL}/${id}/files`,
        formData,
        {baseURL: ""}
    );
    return unwrapResponse(response).data;
}

export async function deleteDangKyXayDungVanBanFile(id, fileId) {
    const response = await axiosClient.delete(
        `${BASE_URL}/${id}/files/${fileId}`,
        {baseURL: ""}
    );
    return unwrapResponse(response);
}

export async function xuLyDangKyXayDungVanBan(id, data) {
    const response = await axiosClient.post(
        `${BASE_URL}/${id}/xu-ly`,
        data,
        {baseURL: ""}
    );
    return unwrapResponse(response);
}

export async function khoiTaoQuyTrinhXayDung(id, data) {
    const response = await axiosClient.post(
        `${BASE_URL}/${id}/khoi-tao-quy-trinh-xay-dung`,
        data,
        {baseURL: ""}
    );
    return unwrapResponse(response);
}
