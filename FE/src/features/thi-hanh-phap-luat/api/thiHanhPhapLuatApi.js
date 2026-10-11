import axiosClient from "../../../shared/api/axiosClient";

const VIEC_DUOC_GIAO_PATH = "/api/thi-hanh-phap-luat/viec-duoc-giao";
const TIEN_DO_PATH = "/api/thi-hanh-phap-luat/tien-do";
const DANH_GIA_PATH = "/api/thi-hanh-phap-luat/danh-gia";
const TONG_HOP_PATH = "/api/thi-hanh-phap-luat/tong-hop";
const DANH_SACH_PATH = "/api/thi-hanh-phap-luat/danh-sach";
const KE_HOACH_PATH = "/api/thi-hanh-phap-luat/ke-hoach";
const NOI_DUNG_PATH = "/api/thi-hanh-phap-luat/noi-dung-ke-hoach";
const PHAN_CONG_PATH = "/api/thi-hanh-phap-luat/phan-cong";

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

export async function getKeHoachThiHanh({ nam, trangThaiId, search } = {}) {
  const response = await axiosClient.get(DANH_SACH_PATH, { baseURL: "", params: { nam: nam || undefined, trangThaiId: trangThaiId || undefined, search: search?.trim() || undefined } });
  return response.data;
}

export async function getKeHoachThiHanhById(id) {
  const response = await axiosClient.get(`${DANH_SACH_PATH}/${id}`, { baseURL: "" });
  return response.data;
}

export async function getKeHoachTimeline(id) {
  const response = await axiosClient.get(`${DANH_SACH_PATH}/${id}/timeline`, { baseURL: "" });
  return response.data;
}

export async function createKeHoachThiHanh(input) {
  const response = await axiosClient.post(KE_HOACH_PATH, input, { baseURL: "" });
  return response.data;
}

export async function startKeHoachThiHanh(id, input) {
  const response = await axiosClient.post(`${KE_HOACH_PATH}/${id}/bat-dau-thuc-hien`, input, { baseURL: "" });
  return response.data;
}

export async function getNoiDungKeHoach(keHoachId) {
  const response = await axiosClient.get(NOI_DUNG_PATH, { baseURL: "", params: { keHoachId } });
  return response.data;
}

export async function createNoiDungKeHoach(input) {
  const response = await axiosClient.post(NOI_DUNG_PATH, input, { baseURL: "" });
  return response.data;
}

export async function deleteNoiDungKeHoach(id) {
  await axiosClient.delete(`${NOI_DUNG_PATH}/${id}`, { baseURL: "" });
}

export async function getPhanCong(noiDungKeHoachId) {
  const response = await axiosClient.get(PHAN_CONG_PATH, { baseURL: "", params: { noiDungKeHoachId } });
  return response.data;
}

export async function createPhanCong(input) {
  const response = await axiosClient.post(PHAN_CONG_PATH, input, { baseURL: "" });
  return response.data;
}

export async function deletePhanCong(id) {
  await axiosClient.delete(`${PHAN_CONG_PATH}/${id}`, { baseURL: "" });
}

export async function getBaoCaoTongHop(keHoachId) {
  const response = await axiosClient.get(TONG_HOP_PATH, { baseURL: "", params: { keHoachId } });
  return response.data;
}

export async function getBaoCaoTongHopById(id) {
  const response = await axiosClient.get(`${TONG_HOP_PATH}/${id}`, { baseURL: "" });
  return response.data;
}

export async function getChiTietBaoCaoTongHop(id) {
  const response = await axiosClient.get(`${TONG_HOP_PATH}/${id}/chi-tiet`, { baseURL: "" });
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
