import { danhMucRoutes } from '../features/danh-muc/routes';
import { quanTriHeThongRoutes } from '../features/quan-tri-he-thong/routes';

export const appFeatureRoutes = [
  ...quanTriHeThongRoutes,
  ...danhMucRoutes
];
