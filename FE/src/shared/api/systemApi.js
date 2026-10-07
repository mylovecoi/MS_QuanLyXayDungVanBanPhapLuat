import { quanTriHeThongRequest } from './quanTriHeThongApi';

// function toCurrentUserHeaders(user) {
//   const headers = {
//     'X-User-Id': user.userId,
//     'X-Username': user.username,
//     'X-Group-Permission-Id': user.groupPermissionId,
//     'X-Is-SSA': String(user.isSSA),
//   };
//
//   if (user.donViId) {
//     headers['X-Don-Vi-Id'] = user.donViId;
//   }
//
//   return headers;
// }

export function getSystemInfo() {
  return quanTriHeThongRequest(
      '/he-thong/cau-hinh-he-thong'
  );
}

export function getFrontendMenu() {
  return quanTriHeThongRequest('/auth/frontend-menu');
}