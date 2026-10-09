import axiosClient from "../../../shared/api/axiosClient";

const BASE_PATH = "/api/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat";

export async function getCuocKhaoSats({ keyword = "", trangThaiId, donViChuTriId, page = 1, pageSize = 20 } = {}) {
  const response = await axiosClient.get(BASE_PATH, { baseURL: "", params: { keyword: keyword.trim() || undefined, trangThaiId: trangThaiId || undefined, donViChuTriId: donViChuTriId || undefined, page, pageSize } });
  return response.data;
}

export async function getCuocKhaoSat(id) {
  const response = await axiosClient.get(`${BASE_PATH}/${id}`, { baseURL: "" });
  return response.data;
}

export async function uploadMauPhieu({ cuocKhaoSatId, nhomDoiTuongId, phienBan, file }) {
  const form = new FormData();
  form.append("cuocKhaoSatId", cuocKhaoSatId);
  form.append("nhomDoiTuongId", nhomDoiTuongId);
  form.append("phienBan", String(phienBan));
  form.append("file", file);
  const response = await axiosClient.post("/api/khao-sat-thi-hanh-phap-luat/mau-phieu/upload", form, { baseURL: "", headers: { "Content-Type": "multipart/form-data" } });
  return response.data;
}

export async function getCauHoiMauPhieu(id) {
  const response = await axiosClient.get(`/api/khao-sat-thi-hanh-phap-luat/mau-phieu/${id}/cau-hoi`, { baseURL: "" });
  return response.data;
}

export async function getKetQuaKhaoSat({ cuocKhaoSatId, nhomDoiTuongId } = {}) {
  const response = await axiosClient.get("/api/khao-sat-thi-hanh-phap-luat/nop-phieu/ket-qua-tong-hop", { baseURL: "", params: { cuocKhaoSatId, nhomDoiTuongId: nhomDoiTuongId || undefined } });
  return response.data;
}

export async function getLoiNhapKetQua(id) {
  const response = await axiosClient.get(`/api/khao-sat-thi-hanh-phap-luat/nop-phieu/ket-qua-tong-hop/${id}/loi`, { baseURL: "" });
  return response.data;
}

export async function uploadKetQuaKhaoSat(input) {
  const form = new FormData();
  Object.entries(input).forEach(([key, value]) => { if (value !== null && value !== undefined && value !== "") form.append(key, value); });
  const response = await axiosClient.post("/api/khao-sat-thi-hanh-phap-luat/nop-phieu/ket-qua-tong-hop/upload", form, { baseURL: "", headers: { "Content-Type": "multipart/form-data" } });
  return response.data;
}

export async function xacNhanKetQuaKhaoSat(id, trangThaiXacNhanId) {
  const response = await axiosClient.post(`/api/khao-sat-thi-hanh-phap-luat/ra-soat/${id}/xac-nhan`, null, { baseURL: "", params: { trangThaiXacNhanId } });
  return response.data;
}

export async function yeuCauBoSungKetQuaKhaoSat(id, { trangThaiCanBoSungId, noiDung }) {
  const response = await axiosClient.post(`/api/khao-sat-thi-hanh-phap-luat/ra-soat/${id}/yeu-cau-bo-sung`, null, { baseURL: "", params: { trangThaiCanBoSungId, noiDung } });
  return response.data;
}

export async function getDashboardKhaoSat(cuocKhaoSatId) {
  const response = await axiosClient.get("/api/khao-sat-thi-hanh-phap-luat/dashboard", { baseURL: "", params: { cuocKhaoSatId } });
  return response.data;
}

export async function taoBaoCaoKhaoSat(input) {
  const response = await axiosClient.post("/api/khao-sat-thi-hanh-phap-luat/bao-cao/snapshot", input, { baseURL: "" });
  return response.data;
}

export async function chotBaoCaoKhaoSat(id) {
  const response = await axiosClient.post(`/api/khao-sat-thi-hanh-phap-luat/bao-cao/${id}/chot`, null, { baseURL: "" });
  return response.data;
}

export async function xuatBaoCaoKhaoSat(id, fileName) {
  const response = await axiosClient.post(`/api/khao-sat-thi-hanh-phap-luat/bao-cao/${id}/xuat-word`, null, { baseURL: "", responseType: "blob" });
  const url = URL.createObjectURL(response.data); const link = document.createElement("a"); link.href = url; link.download = fileName || "bao-cao-khao-sat.docx"; document.body.appendChild(link); link.click(); link.remove(); URL.revokeObjectURL(url);
}
