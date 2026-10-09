import axiosClient from "../../../shared/api/axiosClient";

const DANH_SACH_PATH = "/api/xay-dung-van-ban/danh-sach";
const SOAN_THAO_PATH = "/api/xay-dung-van-ban/soan-thao";

export async function getHoSoXayDungVanBans({
  search = "",
  danhMucVanBanId,
  quyTrinhSoanThaoId,
  buocHienTaiId,
  trangThaiHoSoId,
  donViChuTriSoanThaoId,
  nguoiPhuTrachId,
  namXayDung,
  pageSize = 10,
  pageCurrent = 1,
} = {}) {
  const response = await axiosClient.get(DANH_SACH_PATH, {
    baseURL: "",
    params: {
      search: search.trim() || undefined,
      danhMucVanBanId: danhMucVanBanId || undefined,
      quyTrinhSoanThaoId: quyTrinhSoanThaoId || undefined,
      buocHienTaiId: buocHienTaiId || undefined,
      trangThaiHoSoId: trangThaiHoSoId || undefined,
      donViChuTriSoanThaoId: donViChuTriSoanThaoId || undefined,
      nguoiPhuTrachId: nguoiPhuTrachId || undefined,
      namXayDung: namXayDung || undefined,
      pageSize,
      pageCurrent,
    },
  });

  return response.data;
}

export async function getHoSoXayDungVanBanById(id) {
  const response = await axiosClient.get(`${DANH_SACH_PATH}/${id}`, {
    baseURL: "",
  });

  return response.data;
}

export async function getHoSoXayDungVanBanTimeline(id) {
  const response = await axiosClient.get(`${DANH_SACH_PATH}/${id}/timeline`, {
    baseURL: "",
  });

  return response.data;
}

export async function getHoSoSoanThaoById(id) {
  const response = await axiosClient.get(`${SOAN_THAO_PATH}/${id}`, {
    baseURL: "",
  });

  return response.data;
}

export async function createHoSoSoanThao(input) {
  const response = await axiosClient.post(SOAN_THAO_PATH, input, {
    baseURL: "",
  });

  return response.data;
}

export async function updateHoSoSoanThao(id, input) {
  const response = await axiosClient.put(`${SOAN_THAO_PATH}/${id}`, input, {
    baseURL: "",
  });

  return response.data;
}

export async function getTaiLieuSoanThao(id) {
  const response = await axiosClient.get(`${SOAN_THAO_PATH}/${id}/tai-lieu`, {
    baseURL: "",
  });

  return response.data;
}

export async function uploadTaiLieuSoanThao(id, { file, loaiTaiLieuId, tenTaiLieu }) {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("loaiTaiLieuId", loaiTaiLieuId);
  formData.append("tenTaiLieu", tenTaiLieu);

  const response = await axiosClient.post(`${SOAN_THAO_PATH}/${id}/tai-lieu`, formData, {
    baseURL: "",
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return response.data;
}

export async function deleteHoSoSoanThao(id) {
  const response = await axiosClient.delete(`${SOAN_THAO_PATH}/${id}`, {
    baseURL: "",
  });

  return response.data;
}

export async function getYKienDonVi(hoSoId) {
  const response = await axiosClient.get(`${SOAN_THAO_PATH}/${hoSoId}/y-kien-don-vi`, {
    baseURL: "",
  });

  return response.data;
}

export async function createYKienDonVi(hoSoId, input) {
  const response = await axiosClient.post(`${SOAN_THAO_PATH}/${hoSoId}/y-kien-don-vi`, input, {
    baseURL: "",
  });

  return response.data;
}

export async function updateYKienDonVi(hoSoId, id, input) {
  const response = await axiosClient.put(`${SOAN_THAO_PATH}/${hoSoId}/y-kien-don-vi/${id}`, input, {
    baseURL: "",
  });

  return response.data;
}

export async function deleteYKienDonVi(hoSoId, id) {
  const response = await axiosClient.delete(`${SOAN_THAO_PATH}/${hoSoId}/y-kien-don-vi/${id}`, {
    baseURL: "",
  });

  return response.data;
}

export async function getTongHopYKien(hoSoId) {
  const response = await axiosClient.get(`${SOAN_THAO_PATH}/${hoSoId}/tong-hop-y-kien`, {
    baseURL: "",
  });

  return response.data;
}

export async function updateTongHopYKien(hoSoId, input) {
  const response = await axiosClient.put(`${SOAN_THAO_PATH}/${hoSoId}/tong-hop-y-kien`, input, {
    baseURL: "",
  });

  return response.data;
}

export async function getFileTongHopYKien(hoSoId) {
  const response = await axiosClient.get(`${SOAN_THAO_PATH}/${hoSoId}/file-tong-hop-y-kien`, {
    baseURL: "",
  });

  return response.data;
}

export async function uploadFileTongHopYKien(hoSoId, { file, loaiTaiLieuId }) {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("loaiTaiLieuId", loaiTaiLieuId);

  const response = await axiosClient.post(`${SOAN_THAO_PATH}/${hoSoId}/file-tong-hop-y-kien`, formData, {
    baseURL: "",
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return response.data;
}

export async function kiemTraTruocTrinhThamDinh(hoSoId) {
  const response = await axiosClient.get(`${SOAN_THAO_PATH}/${hoSoId}/kiem-tra-truoc-trinh-tham-dinh`, {
    baseURL: "",
  });

  return response.data;
}

export async function trinhThamDinh(hoSoId, input) {
  const response = await axiosClient.post(`${SOAN_THAO_PATH}/${hoSoId}/trinh-tham-dinh`, input, {
    baseURL: "",
  });

  return response.data;
}

const TRINH_THAM_DINH_PATH = "/api/xay-dung-van-ban/trinh-tham-dinh";
export async function getDanhSachHoSoTrinhThamDinh() { return (await axiosClient.get(TRINH_THAM_DINH_PATH, { baseURL: "" })).data; }
export async function getHoSoNguonTrinhThamDinh() { return (await axiosClient.get(`${TRINH_THAM_DINH_PATH}/ho-so-nguon`, { baseURL: "" })).data; }
export async function getHoSoTrinhThamDinh(hoSoId) { return (await axiosClient.get(`${TRINH_THAM_DINH_PATH}/${hoSoId}`, { baseURL: "" })).data; }
export async function createHoSoTrinhThamDinh(input) { return (await axiosClient.post(TRINH_THAM_DINH_PATH, input, { baseURL: "" })).data; }
export async function updateHoSoTrinhThamDinh(hoSoId, input) { return (await axiosClient.put(`${TRINH_THAM_DINH_PATH}/${hoSoId}`, input, { baseURL: "" })).data; }
export async function getTaiLieuTrinhThamDinh(hoSoId) { return (await axiosClient.get(`${TRINH_THAM_DINH_PATH}/${hoSoId}/tai-lieu`, { baseURL: "" })).data; }
export async function uploadTaiLieuTrinhThamDinh(hoSoId, { file, loaiTaiLieuId, tenTaiLieu, loaiDinhKem }) { const form = new FormData(); form.append("file", file); form.append("loaiTaiLieuId", loaiTaiLieuId); form.append("tenTaiLieu", tenTaiLieu); form.append("loaiDinhKem", loaiDinhKem); return (await axiosClient.post(`${TRINH_THAM_DINH_PATH}/${hoSoId}/tai-lieu`, form, { baseURL: "", headers: { "Content-Type": "multipart/form-data" } })).data; }
export async function taiXuongTaiLieuTrinhThamDinh(hoSoId, fileId, fileName) { const response = await axiosClient.get(`${TRINH_THAM_DINH_PATH}/${hoSoId}/tai-lieu/${fileId}/tai-xuong`, { baseURL: "", responseType: "blob" }); const url = URL.createObjectURL(response.data); const link = document.createElement("a"); link.href = url; link.download = fileName || "tai-lieu"; document.body.appendChild(link); link.click(); link.remove(); URL.revokeObjectURL(url); }
export async function kiemTraGuiThamDinh(hoSoId) { return (await axiosClient.get(`${TRINH_THAM_DINH_PATH}/${hoSoId}/kiem-tra-truoc-gui-tham-dinh`, { baseURL: "" })).data; }
export async function guiThamDinh(hoSoId, input) { return (await axiosClient.post(`${TRINH_THAM_DINH_PATH}/${hoSoId}/gui-tham-dinh`, input, { baseURL: "" })).data; }

const THAM_DINH_PATH = "/api/xay-dung-van-ban/tham-dinh";
export async function getDanhSachHoSoThamDinh() { return (await axiosClient.get(THAM_DINH_PATH, { baseURL: "" })).data; }
export async function getHoSoThamDinh(hoSoId) { return (await axiosClient.get(`${THAM_DINH_PATH}/${hoSoId}`, { baseURL: "" })).data; }
export async function createHoSoThamDinh(hoSoId) { return (await axiosClient.post(THAM_DINH_PATH, { hoSoId }, { baseURL: "" })).data; }
export async function tiepNhanThamDinh(hoSoId, input) { return (await axiosClient.post(`${THAM_DINH_PATH}/${hoSoId}/tiep-nhan`, input, { baseURL: "" })).data; }
export async function capNhatKetQuaThamDinh(hoSoId, input) { return (await axiosClient.put(`${THAM_DINH_PATH}/${hoSoId}/ket-qua`, input, { baseURL: "" })).data; }
export async function getTaiLieuThamDinh(hoSoId) { return (await axiosClient.get(`${THAM_DINH_PATH}/${hoSoId}/tai-lieu`, { baseURL: "" })).data; }
export async function uploadTaiLieuThamDinh(hoSoId, { file, loaiTaiLieuId, tenTaiLieu }) { const form = new FormData(); form.append("file", file); form.append("loaiTaiLieuId", loaiTaiLieuId); form.append("tenTaiLieu", tenTaiLieu); return (await axiosClient.post(`${THAM_DINH_PATH}/${hoSoId}/tai-lieu`, form, { baseURL: "", headers: { "Content-Type": "multipart/form-data" } })).data; }
export async function taiXuongTaiLieuThamDinh(hoSoId, fileId, fileName) { const response = await axiosClient.get(`${THAM_DINH_PATH}/${hoSoId}/tai-lieu/${fileId}/tai-xuong`, { baseURL: "", responseType: "blob" }); const url = URL.createObjectURL(response.data); const link = document.createElement("a"); link.href = url; link.download = fileName || "tai-lieu"; document.body.appendChild(link); link.click(); link.remove(); URL.revokeObjectURL(url); }
export async function kiemTraGuiKetQuaThamDinh(hoSoId) { return (await axiosClient.get(`${THAM_DINH_PATH}/${hoSoId}/kiem-tra-truoc-gui-ket-qua`, { baseURL: "" })).data; }
export async function yeuCauBoSungThamDinh(hoSoId, input) { return (await axiosClient.post(`${THAM_DINH_PATH}/${hoSoId}/yeu-cau-bo-sung`, input, { baseURL: "" })).data; }
export async function traLaiTrinhThamDinh(hoSoId, input) { return (await axiosClient.post(`${THAM_DINH_PATH}/${hoSoId}/tra-lai-trinh-tham-dinh`, input, { baseURL: "" })).data; }
export async function guiKetQuaThamDinh(hoSoId, input) { return (await axiosClient.post(`${THAM_DINH_PATH}/${hoSoId}/gui-ket-qua`, input, { baseURL: "" })).data; }
export async function soSanhDuThaoThamDinh(hoSoId, input) { return (await axiosClient.post(`${THAM_DINH_PATH}/${hoSoId}/so-sanh-du-thao`, input, { baseURL: "" })).data; }

const TRINH_PHE_DUYET_PATH = "/api/xay-dung-van-ban/trinh-phe-duyet";
export async function getDanhSachHoSoTrinhPheDuyet() { return (await axiosClient.get(TRINH_PHE_DUYET_PATH, { baseURL: "" })).data; }
export async function getHoSoTrinhPheDuyet(hoSoId) { return (await axiosClient.get(`${TRINH_PHE_DUYET_PATH}/${hoSoId}`, { baseURL: "" })).data; }
export async function createHoSoTrinhPheDuyet(input) { return (await axiosClient.post(TRINH_PHE_DUYET_PATH, input, { baseURL: "" })).data; }
export async function updateHoSoTrinhPheDuyet(hoSoId, input) { return (await axiosClient.put(`${TRINH_PHE_DUYET_PATH}/${hoSoId}`, input, { baseURL: "" })).data; }
export async function getTaiLieuTrinhPheDuyet(hoSoId) { return (await axiosClient.get(`${TRINH_PHE_DUYET_PATH}/${hoSoId}/tai-lieu`, { baseURL: "" })).data; }
export async function uploadTaiLieuTrinhPheDuyet(hoSoId, { file, loaiTaiLieuId, tenTaiLieu }) { const form = new FormData(); form.append("file", file); form.append("loaiTaiLieuId", loaiTaiLieuId); form.append("tenTaiLieu", tenTaiLieu); return (await axiosClient.post(`${TRINH_PHE_DUYET_PATH}/${hoSoId}/tai-lieu`, form, { baseURL: "", headers: { "Content-Type": "multipart/form-data" } })).data; }
export async function kiemTraGuiPheDuyet(hoSoId) { return (await axiosClient.get(`${TRINH_PHE_DUYET_PATH}/${hoSoId}/kiem-tra-truoc-gui`, { baseURL: "" })).data; }
export async function guiPheDuyet(hoSoId, input) { return (await axiosClient.post(`${TRINH_PHE_DUYET_PATH}/${hoSoId}/gui`, input, { baseURL: "" })).data; }

export const getDanhSachHoSoTrinhYKienUBND = getDanhSachHoSoTrinhPheDuyet;
export const getHoSoTrinhYKienUBND = getHoSoTrinhPheDuyet;
export const createHoSoTrinhYKienUBND = createHoSoTrinhPheDuyet;
export const updateHoSoTrinhYKienUBND = updateHoSoTrinhPheDuyet;
export const getTaiLieuTrinhYKienUBND = getTaiLieuTrinhPheDuyet;
export const uploadTaiLieuTrinhYKienUBND = uploadTaiLieuTrinhPheDuyet;
export const kiemTraGuiYKienUBND = kiemTraGuiPheDuyet;
export const guiYKienUBND = guiPheDuyet;

const Y_KIEN_UBND_PATH = "/api/xay-dung-van-ban/y-kien-ubnd";
export async function getDanhSachHoSoYKienUBND() { return (await axiosClient.get(Y_KIEN_UBND_PATH, { baseURL: "" })).data; }
export async function getHoSoYKienUBND(hoSoId) { return (await axiosClient.get(`${Y_KIEN_UBND_PATH}/${hoSoId}`, { baseURL: "" })).data; }
export async function createHoSoYKienUBND(input) { return (await axiosClient.post(Y_KIEN_UBND_PATH, input, { baseURL: "" })).data; }
export async function updateHoSoYKienUBND(hoSoId, input) { return (await axiosClient.put(`${Y_KIEN_UBND_PATH}/${hoSoId}`, input, { baseURL: "" })).data; }
export async function getTaiLieuYKienUBND(hoSoId) { return (await axiosClient.get(`${Y_KIEN_UBND_PATH}/${hoSoId}/tai-lieu`, { baseURL: "" })).data; }
export async function uploadTaiLieuYKienUBND(hoSoId, { file, loaiTaiLieuId, tenTaiLieu }) { const form = new FormData(); form.append("file", file); form.append("loaiTaiLieuId", loaiTaiLieuId); form.append("tenTaiLieu", tenTaiLieu); return (await axiosClient.post(`${Y_KIEN_UBND_PATH}/${hoSoId}/tai-lieu`, form, { baseURL: "", headers: { "Content-Type": "multipart/form-data" } })).data; }
export async function kiemTraGuiKetQuaYKienUBND(hoSoId) { return (await axiosClient.get(`${Y_KIEN_UBND_PATH}/${hoSoId}/kiem-tra-truoc-gui`, { baseURL: "" })).data; }
export async function guiKetQuaYKienUBND(hoSoId, input) { return (await axiosClient.post(`${Y_KIEN_UBND_PATH}/${hoSoId}/gui`, input, { baseURL: "" })).data; }
