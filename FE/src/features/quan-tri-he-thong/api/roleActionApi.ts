// import { quanTriHeThongCommand, quanTriHeThongPagedRequest, quanTriHeThongRequest } from '../../../shared/api/quanTriHeThongApi';
//
// export type RoleAction = {
//   id: string;
//   sttSapXep: number;
//   phanLoai: string;
//   level: number;
//   role: string;
//   parentId?: string | null;
//   parentTitle?: string | null;
//   title?: string | null;
//   controller?: string | null;
//   action?: string | null;
//   parameter?: string | null;
//   table?: string | null;
//   status: string;
//   useGroup?: string | null;
//   frontendPath?: string | null;
//   isVisibleInMenu: boolean;
//   clientApp?: string | null;
//   menuTitle?: string | null;
//   menuIcon?: string | null;
//   icon?: string | null;
// };
//
// export type RoleActionUpdateInput = {
//   id: string;
//   sttSapXep: number;
//   phanLoai: string;
//   role: string;
//   parentId?: string | null;
//   title?: string | null;
//   controller?: string | null;
//   action?: string | null;
//   parameter?: string | null;
//   table?: string | null;
//   status: string;
//   useGroup?: string | null;
//   frontendPath?: string | null;
//   isVisibleInMenu: boolean;
//   clientApp?: string | null;
//   menuTitle?: string | null;
//   menuIcon?: string | null;
//   icon?: string | null;
// };
//
// export function getRoleActions(search = '', pageSize = 50, pageCurrent = 1) {
//   const params = new URLSearchParams({
//     pageSize: String(pageSize),
//     pageCurrent: String(pageCurrent)
//   });
//
//   if (search.trim()) {
//     params.set('search', search.trim());
//   }
//
//   return quanTriHeThongPagedRequest<RoleAction>(`/he-thong/danh-sach-chuc-nang?${params.toString()}`);
// }
//
// export function updateRoleAction(input: RoleActionUpdateInput) {
//   return quanTriHeThongRequest<RoleAction>(`/he-thong/danh-sach-chuc-nang/${input.id}`, {
//     method: 'PUT',
//     body: {
//       id: input.id,
//       sttSapXep: input.sttSapXep,
//       phanLoai: input.phanLoai,
//       role: input.role,
//       parentId: input.parentId,
//       title: input.title,
//       controller: input.controller,
//       action: input.action,
//       parameter: input.parameter,
//       table: input.table,
//       status: input.status,
//       useGroup: input.useGroup,
//       frontendPath: input.frontendPath,
//       isVisibleInMenu: input.isVisibleInMenu,
//       clientApp: input.clientApp,
//       menuTitle: input.menuTitle,
//       menuIcon: input.menuIcon,
//       icon: input.icon
//     }
//   });
// }
//
// export function deleteRoleAction(id: string) {
//   return quanTriHeThongCommand(`/he-thong/danh-sach-chuc-nang/${id}`, {
//     method: 'DELETE'
//   });
// }
