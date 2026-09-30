import { ApiRequestOptions, ApiResponse, PagedApiResponse } from './types';

const QUAN_TRI_HE_THONG_API_BASE = '/api/qtht';

export async function quanTriHeThongRequest<T>(
  path: string,
  options: ApiRequestOptions = {}
): Promise<T> {
  const response = await fetch(`${QUAN_TRI_HE_THONG_API_BASE}${path}`, {
    method: options.method ?? 'GET',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      ...options.headers
    },
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const payload = (await response.json().catch(() => null)) as ApiResponse<T> | null;

  if (!response.ok || payload?.isSuccess === false) {
    throw new Error(payload?.message || 'Không thể kết nối đến dịch vụ Quản trị hệ thống.');
  }

  if (!payload || payload.data === undefined) {
    throw new Error('Dữ liệu trả về không hợp lệ.');
  }

  return payload.data;
}

export async function quanTriHeThongPagedRequest<T>(
  path: string,
  options: ApiRequestOptions = {}
): Promise<PagedApiResponse<T>> {
  const response = await fetch(`${QUAN_TRI_HE_THONG_API_BASE}${path}`, {
    method: options.method ?? 'GET',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      ...options.headers
    },
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const payload = (await response.json().catch(() => null)) as PagedApiResponse<T> | null;

  if (!response.ok || payload?.isSuccess === false) {
    throw new Error(payload?.message || 'Không thể kết nối đến dịch vụ Quản trị hệ thống.');
  }

  if (!payload || !Array.isArray(payload.data)) {
    throw new Error('Dữ liệu trả về không hợp lệ.');
  }

  return payload;
}

export async function quanTriHeThongCommand(
  path: string,
  options: ApiRequestOptions = {}
): Promise<void> {
  const response = await fetch(`${QUAN_TRI_HE_THONG_API_BASE}${path}`, {
    method: options.method ?? 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      ...options.headers
    },
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const payload = (await response.json().catch(() => null)) as ApiResponse<unknown> | null;

  if (!response.ok || payload?.isSuccess === false) {
    throw new Error(payload?.message || 'Thao tác không thành công.');
  }
}
