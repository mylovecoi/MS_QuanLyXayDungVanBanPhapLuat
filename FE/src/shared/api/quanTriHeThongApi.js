import axiosClient from './axiosClient';

export async function quanTriHeThongRequest(
    path,
    options = {}
) {
  try {
    const response = await axiosClient.request({
      url: path,
      method: options.method ?? 'GET',
      headers: options.headers,
      data: options.body,
    });

    const payload = response.data;

    if (payload?.isSuccess === false) {
      throw new Error(
          payload?.message ||
          'Không thể kết nối đến dịch vụ Quản trị hệ thống.'
      );
    }

    if (!payload || payload.data === undefined) {
      throw new Error('Dữ liệu trả về không hợp lệ.');
    }

    return payload.data;
  } catch (error) {
    if (error.response) {
      const payload = error.response.data;

      throw new Error(
          payload?.message ||
          'Không thể kết nối đến dịch vụ Quản trị hệ thống.'
      );
    }

    throw error;
  }
}

export async function quanTriHeThongPagedRequest(
    path,
    options = {}
) {
  try {
    const response = await axiosClient.request({
      url: path,
      method: options.method ?? 'GET',
      headers: options.headers,
      data: options.body,
    });

    const payload = response.data;

    if (payload?.isSuccess === false) {
      throw new Error(
          payload?.message ||
          'Không thể kết nối đến dịch vụ Quản trị hệ thống.'
      );
    }

    if (!payload || !Array.isArray(payload.data)) {
      throw new Error('Dữ liệu trả về không hợp lệ.');
    }

    return payload;
  } catch (error) {
    if (error.response) {
      const payload = error.response.data;

      throw new Error(
          payload?.message ||
          'Không thể kết nối đến dịch vụ Quản trị hệ thống.'
      );
    }

    throw error;
  }
}

export async function quanTriHeThongCommand(
    path,
    options = {}
) {
  try {
    const response = await axiosClient.request({
      url: path,
      method: options.method ?? 'POST',
      headers: options.headers,
      data: options.body,
    });

    const payload = response.data;

    if (payload?.isSuccess === false) {
      throw new Error(
          payload?.message || 'Thao tác không thành công.'
      );
    }
  } catch (error) {
    if (error.response) {
      const payload = error.response.data;

      throw new Error(
          payload?.message || 'Thao tác không thành công.'
      );
    }

    throw error;
  }
}