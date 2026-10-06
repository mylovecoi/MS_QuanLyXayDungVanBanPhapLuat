import { ApiRequestOptions, ApiResponse, PagedApiResponse } from './types';

const API_BASES = {
  quanTriHeThong: '/api/qtht',
  danhMuc: '/api/danh-muc',
  xayDungVanBan: '/api/xay-dung-van-ban',
  dangKyXayDungVanBan: '/api/dang-ky-xay-dung-van-ban'
} as const;
const AUTH_STORAGE_KEY = 'ms_xdvb_auth';

function getAccessToken() {
  try {
    const rawValue = window.localStorage.getItem(AUTH_STORAGE_KEY);
    if (!rawValue) {
      return undefined;
    }

    const parsedValue = JSON.parse(rawValue) as { accessToken?: string };
    return parsedValue.accessToken;
  } catch {
    return undefined;
  }
}

function buildHeaders(headers?: Record<string, string>) {
  const accessToken = getAccessToken();

  return {
    Accept: 'application/json',
    'Content-Type': 'application/json',
    ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
    ...headers
  };
}

async function apiRequest<T>(
  baseUrl: string,
  path: string,
  options: ApiRequestOptions,
  fallbackMessage: string
): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: options.method ?? 'GET',
    headers: buildHeaders(options.headers),
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const payload = (await response.json().catch(() => null)) as ApiResponse<T> | null;

  if (!response.ok || payload?.isSuccess === false) {
    throw new Error(payload?.message || fallbackMessage);
  }

  if (!payload || payload.data === undefined) {
    throw new Error('Dữ liệu trả về không hợp lệ.');
  }

  return payload.data;
}

async function apiPagedRequest<T>(
  baseUrl: string,
  path: string,
  options: ApiRequestOptions,
  fallbackMessage: string
): Promise<PagedApiResponse<T>> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: options.method ?? 'GET',
    headers: buildHeaders(options.headers),
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const payload = (await response.json().catch(() => null)) as PagedApiResponse<T> | null;

  if (!response.ok || payload?.isSuccess === false) {
    throw new Error(payload?.message || fallbackMessage);
  }

  if (!payload || !Array.isArray(payload.data)) {
    throw new Error('Dữ liệu trả về không hợp lệ.');
  }

  return payload;
}

async function apiCommand(
  baseUrl: string,
  path: string,
  options: ApiRequestOptions,
  fallbackMessage: string
): Promise<void> {
  const response = await fetch(`${baseUrl}${path}`, {
    method: options.method ?? 'POST',
    headers: buildHeaders(options.headers),
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const payload = (await response.json().catch(() => null)) as ApiResponse<unknown> | null;

  if (!response.ok || payload?.isSuccess === false) {
    throw new Error(payload?.message || fallbackMessage);
  }
}

export async function quanTriHeThongRequest<T>(
  path: string,
  options: ApiRequestOptions = {}
): Promise<T> {
  return apiRequest<T>(
    API_BASES.quanTriHeThong,
    path,
    options,
    'Không thể kết nối đến dịch vụ Quản trị hệ thống.'
  );
}

export async function quanTriHeThongPagedRequest<T>(
  path: string,
  options: ApiRequestOptions = {}
): Promise<PagedApiResponse<T>> {
  return apiPagedRequest<T>(
    API_BASES.quanTriHeThong,
    path,
    options,
    'Không thể kết nối đến dịch vụ Quản trị hệ thống.'
  );
}

export const quanTriHeThongCommand = (path: string, options: ApiRequestOptions = {}) =>
  apiCommand(API_BASES.quanTriHeThong, path, options, 'Thao tác không thành công.');

export const danhMucRequest = <T>(path: string, options: ApiRequestOptions = {}) =>
  apiRequest<T>(API_BASES.danhMuc, path, options, 'Không thể kết nối đến dịch vụ Danh mục.');

export const danhMucPagedRequest = <T>(path: string, options: ApiRequestOptions = {}) =>
  apiPagedRequest<T>(API_BASES.danhMuc, path, options, 'Không thể kết nối đến dịch vụ Danh mục.');

export const danhMucCommand = (path: string, options: ApiRequestOptions = {}) =>
  apiCommand(API_BASES.danhMuc, path, options, 'Thao tác không thành công.');

export const xayDungVanBanRequest = <T>(path: string, options: ApiRequestOptions = {}) =>
  apiRequest<T>(
    API_BASES.xayDungVanBan,
    path,
    options,
    'Không thể kết nối đến dịch vụ Xây dựng văn bản.'
  );

export const xayDungVanBanPagedRequest = <T>(path: string, options: ApiRequestOptions = {}) =>
  apiPagedRequest<T>(
    API_BASES.xayDungVanBan,
    path,
    options,
    'Không thể kết nối đến dịch vụ Xây dựng văn bản.'
  );

export const xayDungVanBanCommand = (path: string, options: ApiRequestOptions = {}) =>
  apiCommand(API_BASES.xayDungVanBan, path, options, 'Thao tác không thành công.');

export const dangKyXayDungVanBanRequest = <T>(path: string, options: ApiRequestOptions = {}) =>
  apiRequest<T>(
    API_BASES.dangKyXayDungVanBan,
    path,
    options,
    'Không thể kết nối đến dịch vụ Đăng ký xây dựng văn bản.'
  );

export const dangKyXayDungVanBanPagedRequest = <T>(path: string, options: ApiRequestOptions = {}) =>
  apiPagedRequest<T>(
    API_BASES.dangKyXayDungVanBan,
    path,
    options,
    'Không thể kết nối đến dịch vụ Đăng ký xây dựng văn bản.'
  );

export const dangKyXayDungVanBanCommand = (path: string, options: ApiRequestOptions = {}) =>
  apiCommand(API_BASES.dangKyXayDungVanBan, path, options, 'Thao tác không thành công.');
