import axiosClient from "../../../shared/api/axiosClient";

const base = "/api/khai-thac-du-lieu/bao-cao";

export async function getDanhMucBaoCao() {
  const response = await axiosClient.get(`${base}/danh-muc`, { baseURL: "" });
  return response.data || [];
}

export async function getBaoCao(maBaoCao, filters = {}) {
  const response = await axiosClient.get(`${base}/${maBaoCao}`, {
    baseURL: "",
    params: {
      nam: filters.nam || undefined,
      donViId: filters.donViId || undefined,
      loaiVanBanId: filters.loaiVanBanId || undefined,
      quyTrinhSoanThaoId: filters.quyTrinhSoanThaoId || undefined,
      trangThaiId: filters.trangThaiId || undefined,
      tuNgay: filters.tuNgay || undefined,
      denNgay: filters.denNgay || undefined,
    },
  });
  return response.data;
}
