// import { quanTriHeThongRequest } from './httpClient';
//
// export type LoginApiResult = {
//   userId: string;
//   username: string;
//   displayName?: string;
//   donViId?: string;
//   groupPermissionId: string;
//   isSSA: boolean;
//   firstLogin: boolean;
//   mustChangePassword: boolean;
// };
//
// export function loginApi(username: string, password: string) {
//   return quanTriHeThongRequest<LoginApiResult>('/auth/login', {
//     method: 'POST',
//     body: {
//       username,
//       password
//     }
//   });
// }
import axiosClient from './axiosClient';

export const loginApi = async (username, password) => {
  const response = await axiosClient.post('/auth/login', {
    username,
    password,
  });

  return response.data;
};