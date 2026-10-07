import axiosClient from "../../../shared/api/axiosClient";

const base = "/api/khai-thac-du-lieu/tra-cuu";
export const searchTraCuu = async (source, filters) => {
  const response = await axiosClient.get(`${base}/${source}`, { baseURL: "", params: filters });
  return response.data;
};
