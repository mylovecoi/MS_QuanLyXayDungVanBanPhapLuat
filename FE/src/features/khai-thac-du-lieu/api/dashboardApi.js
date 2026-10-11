import axiosClient from "../../../shared/api/axiosClient";

const base = "/api/khai-thac-du-lieu/dashboard";

export const getDashboardLayout = async (filters = {}) => {
  const response = await axiosClient.get(`${base}/layout`, {
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
};

export const updateDashboardLayout = async (widgets) => {
  const response = await axiosClient.put(`${base}/layout`, { widgets }, { baseURL: "" });
  return response.data;
};
