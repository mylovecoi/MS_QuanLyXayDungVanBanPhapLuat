import {danhMucRoutes} from '../features/danh-muc/routes.jsx';
import {quanTriHeThongRoutes} from '../features/quan-tri-he-thong/routes.jsx';
import {khaiThacDuLieuRoutes} from '../features/khai-thac-du-lieu/routes.jsx';
import {xayDungVanBanRoutes} from '../features/xay-dung-van-ban/routes.jsx';
import {dangKyVanBanRoutes} from '../features/dang-ky-soan-thao-vb/routes.jsx';
import {thiHanhPhapLuatRoutes} from '../features/thi-hanh-phap-luat/routes.jsx';
import {khaoSatThiHanhPhapLuatRoutes} from '../features/khao-sat-thi-hanh-phap-luat/routes.jsx';

export const appFeatureRoutes = [
    ...quanTriHeThongRoutes,
    ...danhMucRoutes,
    ...khaiThacDuLieuRoutes,
    ...xayDungVanBanRoutes,
    ...dangKyVanBanRoutes,
    ...thiHanhPhapLuatRoutes,
    ...khaoSatThiHanhPhapLuatRoutes
];
