import axiosClient from "../../../shared/api/axiosClient";

const PATH = "/api/danh-muc/quy-trinh-soan-thao";

function unwrap(response) {
  if (response.data?.isSuccess === false) throw new Error(response.data?.message || "Thao tác không thành công.");
  return response.data;
}

export async function getQuyTrinhs({ search = "", pageSize = 10, pageCurrent = 1 } = {}) {
  return unwrap(await axiosClient.get(PATH, { baseURL: "", params: { search: search.trim() || undefined, pageSize, pageCurrent } }));
}

export async function createQuyTrinh(input) { return unwrap(await axiosClient.post(PATH, input, { baseURL: "" })).data; }
export async function updateQuyTrinh(id, input) { return unwrap(await axiosClient.put(`${PATH}/${id}`, input, { baseURL: "" })).data; }
export async function deleteQuyTrinh(id) { return unwrap(await axiosClient.delete(`${PATH}/${id}`, { baseURL: "" })); }
