import { danhMucRoutes } from '../features/danh-muc/routes.jsx';
import { quanTriHeThongRoutes } from '../features/quan-tri-he-thong/routes.jsx';

export const appFeatureRoutes = [
  ...quanTriHeThongRoutes,
  ...danhMucRoutes
];
