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
