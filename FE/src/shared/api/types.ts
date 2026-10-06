// export type ApiResponse<T> = {
//   isSuccess: boolean;
//   message?: string;
//   data?: T;
// };
//
// export type PagedApiResponse<T> = Omit<ApiResponse<T[]>, 'data'> & {
//   data: T[];
//   totalCount: number;
//   pageSize: number;
//   pageCurrent: number;
// };
//
// export type ApiRequestOptions = {
//   method?: 'GET' | 'POST' | 'PUT' | 'DELETE';
//   body?: unknown;
//   headers?: Record<string, string>;
// };
