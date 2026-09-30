import { quanTriHeThongRequest } from './httpClient';

export type MenuLayout = 'vertical' | 'horizontal';

export type SystemInfo = {
  id: string;
  appName?: string;
  copyright?: string;
  mfgDate: string;
  expDate: string;
  loginLock: number;
  train: boolean;
  isChatBot: boolean;
  isOPT: boolean;
  menuLayout: MenuLayout;
};

export type FrontendMenuItem = {
  roleActionId: string;
  parentRoleActionId?: string | null;
  role: string;
  title: string;
  url?: string | null;
  phanLoai?: string | null;
  level: number;
  sttSapXep: number;
  controller?: string | null;
  action?: string | null;
  parameter?: string | null;
  icon?: string | null;
  clientApp?: string | null;
  children: FrontendMenuItem[];
};

export type FrontendMenu = {
  items: FrontendMenuItem[];
};

export type CurrentUserHeaders = {
  userId: string;
  username: string;
  donViId?: string;
  groupPermissionId: string;
  isSSA: boolean;
};

function toCurrentUserHeaders(user: CurrentUserHeaders) {
  const headers: Record<string, string> = {
    'X-User-Id': user.userId,
    'X-Username': user.username,
    'X-Group-Permission-Id': user.groupPermissionId,
    'X-Is-SSA': String(user.isSSA)
  };

  if (user.donViId) {
    headers['X-Don-Vi-Id'] = user.donViId;
  }

  return headers;
}

export function getSystemInfo() {
  return quanTriHeThongRequest<SystemInfo>('/he-thong/cau-hinh-he-thong');
}

export function getFrontendMenu(user: CurrentUserHeaders) {
  return quanTriHeThongRequest<FrontendMenu>('/auth/frontend-menu', {
    headers: toCurrentUserHeaders(user)
  });
}
