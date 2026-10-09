import axiosClient from "../../../shared/api/axiosClient";

const VIEC_DUOC_GIAO_PATH = "/api/thi-hanh-phap-luat/viec-duoc-giao";
const TIEN_DO_PATH = "/api/thi-hanh-phap-luat/tien-do";
const DANH_GIA_PATH = "/api/thi-hanh-phap-luat/danh-gia";
const TONG_HOP_PATH = "/api/thi-hanh-phap-luat/tong-hop";
const DANH_SACH_PATH = "/api/thi-hanh-phap-luat/danh-sach";

export async function getViecDuocGiao({ nam, keHoachId, chiQuaHan } = {}) {
  const response = await axiosClient.get(VIEC_DUOC_GIAO_PATH, {
    baseURL: "",
    params: {
      nam: nam || undefined,
      keHoachId: keHoachId || undefined,
      chiQuaHan: chiQuaHan || undefined,
    },
  });

  return response.data;
}

export async function getBaoCaoTienDo(noiDungKeHoachId) {
  const response = await axiosClient.get(TIEN_DO_PATH, {
    baseURL: "",
    params: { noiDungKeHoachId },
  });

  return response.data;
}

export async function createBaoCaoTienDo(input) {
  const response = await axiosClient.post(TIEN_DO_PATH, input, { baseURL: "" });
  return response.data;
}

export async function updateBaoCaoTienDo(id, input) {
  const response = await axiosClient.put(`${TIEN_DO_PATH}/${id}`, input, { baseURL: "" });
  return response.data;
}

export async function guiBaoCaoTienDo(id, input) {
  const response = await axiosClient.post(`${TIEN_DO_PATH}/${id}/gui`, input, { baseURL: "" });
  return response.data;
}

export async function getBaoCaoChoDanhGia({ nam } = {}) {
  const response = await axiosClient.get(`${DANH_GIA_PATH}/cho-danh-gia`, { baseURL: "", params: { nam: nam || undefined } });
  return response.data;
}

export async function danhGiaDat(id, input) {
  const response = await axiosClient.post(`${DANH_GIA_PATH}/${id}/dat`, input, { baseURL: "" });
  return response.data;
}

export async function danhGiaKhongDat(id, input) {
  const response = await axiosClient.post(`${DANH_GIA_PATH}/${id}/khong-dat`, input, { baseURL: "" });
  return response.data;
}

export async function yeuCauBoSungBaoCao(id, input) {
  const response = await axiosClient.post(`${DANH_GIA_PATH}/${id}/yeu-cau-bo-sung`, input, { baseURL: "" });
  return response.data;
}

export async function getKeHoachThiHanh() {
  const response = await axiosClient.get(DANH_SACH_PATH, { baseURL: "" });
  return response.data;
}

export async function getBaoCaoTongHop(keHoachId) {
  const response = await axiosClient.get(TONG_HOP_PATH, { baseURL: "", params: { keHoachId } });
  return response.data;
}

export async function createBaoCaoTongHop(input) {
  const response = await axiosClient.post(TONG_HOP_PATH, input, { baseURL: "" });
  return response.data;
}

export async function chotBaoCaoTongHop(id, input) {
  const response = await axiosClient.post(`${TONG_HOP_PATH}/${id}/chot`, input, { baseURL: "" });
  return response.data;
}
