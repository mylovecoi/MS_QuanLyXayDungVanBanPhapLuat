import axiosClient from "../../../shared/api/axiosClient";

const base = "/api/khai-thac-du-lieu/canh-bao";

export const getCanhBao = async (params = {}) => {
  const response = await axiosClient.get(base, { baseURL: "", params });
  return response.data;
};

export const danhDauDaXemCanhBao = async (id) => {
  const response = await axiosClient.post(`${base}/${id}/danh-dau-da-xem`, {}, { baseURL: "" });
  return response.data;
};

export const xacNhanXuLyCanhBao = async (id, input = {}) => {
  const response = await axiosClient.post(`${base}/${id}/xac-nhan-xu-ly`, input, { baseURL: "" });
  return response.data;
};

export const quetCanhBaoTuDong = async () => {
  const response = await axiosClient.post(`${base}/quet-tu-dong`, {}, { baseURL: "" });
  return response.data;
};

export const getNhacViecCanhBao = async (canhBaoId) => {
  const response = await axiosClient.get(`${base}/${canhBaoId}/nhac-viec`, { baseURL: "" });
  return response.data;
};

export const getLichSuCanhBao = async (canhBaoId) => {
  const response = await axiosClient.get(`${base}/${canhBaoId}/lich-su`, { baseURL: "" });
  return response.data;
};

export const getNhacViecCuaToi = async () => {
  const response = await axiosClient.get(`${base}/nhac-viec`, { baseURL: "" });
  return response.data;
};

export const taoNhacViecCanhBao = async (canhBaoId, input) => {
  const response = await axiosClient.post(`${base}/${canhBaoId}/nhac-viec`, input, { baseURL: "" });
  return response.data;
};

export const danhDauDaXemNhacViec = async (canhBaoId, nhacViecId) => {
  const response = await axiosClient.post(`${base}/${canhBaoId}/nhac-viec/${nhacViecId}/danh-dau-da-xem`, {}, { baseURL: "" });
  return response.data;
};

export const hoanThanhNhacViec = async (canhBaoId, nhacViecId, input = {}) => {
  const response = await axiosClient.post(`${base}/${canhBaoId}/nhac-viec/${nhacViecId}/hoan-thanh`, input, { baseURL: "" });
  return response.data;
};

export const huyNhacViec = async (canhBaoId, nhacViecId) => {
  const response = await axiosClient.post(`${base}/${canhBaoId}/nhac-viec/${nhacViecId}/huy`, {}, { baseURL: "" });
  return response.data;
};
