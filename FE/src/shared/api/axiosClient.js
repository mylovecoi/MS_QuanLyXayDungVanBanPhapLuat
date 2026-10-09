import axios from "axios";

const qthtApiBaseUrl =
    import.meta.env.VITE_QTHT_API_URL ||
    import.meta.env.VITE_API_URL ||
    "/api/qtht";

const axiosClient = axios.create({
    baseURL: qthtApiBaseUrl,
    timeout: 80000,
    headers: {
        "Content-Type": "application/json",
    },
});

axiosClient.interceptors.request.use(
    (config) => {
        const accessToken = localStorage.getItem("accessToken");

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);

axiosClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // xử lý token hết hạn sau
        }

        return Promise.reject(error);
    }
);

export default axiosClient;
