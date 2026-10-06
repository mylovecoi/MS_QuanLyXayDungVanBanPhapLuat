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

export function getSystemInfo() {
  return quanTriHeThongRequest<SystemInfo>('/he-thong/cau-hinh-he-thong');
}

export function getFrontendMenu() {
  return quanTriHeThongRequest<FrontendMenu>('/auth/frontend-menu');
}
