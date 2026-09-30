import { quanTriHeThongRequest } from './httpClient';

export type LoginApiResult = {
  userId: string;
  username: string;
  displayName?: string;
  donViId?: string;
  groupPermissionId: string;
  isSSA: boolean;
  firstLogin: boolean;
  mustChangePassword: boolean;
};

export function loginApi(username: string, password: string) {
  return quanTriHeThongRequest<LoginApiResult>('/auth/login', {
    method: 'POST',
    body: {
      username,
      password
    }
  });
}
