import axiosClient from "../../../shared/api/axiosClient";

const BASE_URL = "/dang-ky-xay-dung-van-ban/ho-so";

export const getDangKyXayDungVanBanById = (id) => {
    return axiosClient.get(`${BASE_URL}/${id}`);
};

export const createDangKyXayDungVanBan = (data) => {
    return axiosClient.post(BASE_URL, data);
};

export const updateDangKyXayDungVanBan = (id, data) => {
    return axiosClient.put(`${BASE_URL}/${id}`, data);
};

export const deleteDangKyXayDungVanBan = (id) => {
    return axiosClient.delete(`${BASE_URL}/${id}`);
};

export const getDangKyXayDungVanBanTimeline = (id) => {
    return axiosClient.get(`${BASE_URL}/${id}/timeline`);
};

export const getHanhDongKhaDung = (id) => {
    return axiosClient.get(`${BASE_URL}/${id}/hanh-dong-kha-dung`);
};

export const getDangKyXayDungVanBanFiles = (id) => {
    return axiosClient.get(`${BASE_URL}/${id}/files`);
};

export const uploadDangKyXayDungVanBanFile = (id, formData) => {
    return axiosClient.post(`${BASE_URL}/${id}/files`, formData);
};

export const deleteDangKyXayDungVanBanFile = (id, fileId) => {
    return axiosClient.delete(
        `${BASE_URL}/${id}/files/${fileId}`
    );
};

export const xuLyDangKyXayDungVanBan = (id, data) => {
    return axiosClient.post(
        `${BASE_URL}/${id}/xu-ly`,
        data
    );
};

export const khoiTaoQuyTrinhXayDung = (id, data) => {
    return axiosClient.post(
        `${BASE_URL}/${id}/khoi-tao-quy-trinh-xay-dung`,
        data
    );
};

