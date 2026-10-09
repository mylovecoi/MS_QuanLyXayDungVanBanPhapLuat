import axiosClient from "./axiosClient";

export const loginApi = async (username, password) => {
    const response = await axiosClient.post("/he-thong/auth/login", {
        username,
        password,
    });

    return response.data;
};
