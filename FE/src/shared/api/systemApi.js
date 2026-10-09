import {
  quanTriHeThongRequest,
  quanTriHeThongPagedRequest,
} from './quanTriHeThongApi';

export function getSystemInfo() {
  return quanTriHeThongRequest(
      '/he-thong/cau-hinh-he-thong'
  );
}

export function getRoleActionList(params) {
  return quanTriHeThongPagedRequest(
      '/he-thong/danh-sach-chuc-nang',
      {
        params,
      }
  );
}

export function getFrontendMenu() {
  return quanTriHeThongRequest('/he-thong/auth/frontend-menu');
}
