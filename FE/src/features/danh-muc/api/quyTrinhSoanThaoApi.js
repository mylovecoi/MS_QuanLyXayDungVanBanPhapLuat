import axiosClient from "../../../shared/api/axiosClient";

const PATH = "/api/danh-muc/quy-trinh-soan-thao";

function unwrap(response) {
  if (response.data?.isSuccess === false) throw new Error(response.data?.message || "Thao tác không thành công.");
  return response.data;
}

export async function getQuyTrinhs({ search = "", pageSize = 10, pageCurrent = 1 } = {}) {
  return unwrap(await axiosClient.get(PATH, { baseURL: "", params: { search: search.trim() || undefined, pageSize, pageCurrent } }));
}

export async function getQuyTrinhById(id) { return unwrap(await axiosClient.get(`${PATH}/${id}`, { baseURL: "" })).data; }
export async function createQuyTrinh(input) { return unwrap(await axiosClient.post(PATH, input, { baseURL: "" })).data; }
export async function updateQuyTrinh(id, input) { return unwrap(await axiosClient.put(`${PATH}/${id}`, input, { baseURL: "" })).data; }
export async function deleteQuyTrinh(id) { return unwrap(await axiosClient.delete(`${PATH}/${id}`, { baseURL: "" })); }
export async function updateBuocQuyTrinh(quyTrinhId, buocId, input) { return unwrap(await axiosClient.put(`${PATH}/${quyTrinhId}/buoc/${buocId}`, input, { baseURL: "" })).data; }
export async function deleteBuocQuyTrinh(quyTrinhId, buocId) { return unwrap(await axiosClient.delete(`${PATH}/${quyTrinhId}/buoc/${buocId}`, { baseURL: "" })); }
export async function updateChuyenBuocQuyTrinh(quyTrinhId, chuyenBuocId, input) { return unwrap(await axiosClient.put(`${PATH}/${quyTrinhId}/chuyen-buoc/${chuyenBuocId}`, input, { baseURL: "" })).data; }
export async function deleteChuyenBuocQuyTrinh(quyTrinhId, chuyenBuocId) { return unwrap(await axiosClient.delete(`${PATH}/${quyTrinhId}/chuyen-buoc/${chuyenBuocId}`, { baseURL: "" })); }
