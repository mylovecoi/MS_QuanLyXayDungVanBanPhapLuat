import axiosClient from "../../../shared/api/axiosClient";

const base = "/api/khai-thac-du-lieu/tra-cuu";
const endpoints = {
    "tong-hop": `${base}/tong-hop`,
    "dang-ky-xay-dung-van-ban": `${base}/dang-ky-xay-dung-van-ban`,
    "xay-dung-van-ban": `${base}/xay-dung-van-ban`,
    "thi-hanh-phap-luat": `${base}/thi-hanh-phap-luat`,
};

export const searchTraCuu = async (source, filters) => {
    const endpoint = endpoints[source];
    if (!endpoint) {
        throw new Error(`Không tìm thấy API tra cứu cho nguồn: ${source}`);
    }
    const response = await axiosClient.get(endpoint, {params: filters,});
    return response.data;
}
