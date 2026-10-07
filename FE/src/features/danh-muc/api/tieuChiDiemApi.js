import axiosClient from "../../../shared/api/axiosClient";
const PATH = "/api/danh-muc/tieu-chi-diem";
const unwrap = (response) => { if (response.data?.isSuccess === false) throw new Error(response.data?.message || "Thao tác không thành công."); return response.data; };
export const getTieuChiDiems = async ({ search = "", pageSize = 10, pageCurrent = 1 } = {}) => unwrap(await axiosClient.get(PATH, { baseURL: "", params: { search: search.trim() || undefined, pageSize, pageCurrent } }));
export const createTieuChiDiem = async (input) => unwrap(await axiosClient.post(PATH, input, { baseURL: "" })).data;
export const updateTieuChiDiem = async (id, input) => unwrap(await axiosClient.put(`${PATH}/${id}`, input, { baseURL: "" })).data;
export const deleteTieuChiDiem = async (id) => unwrap(await axiosClient.delete(`${PATH}/${id}`, { baseURL: "" }));
