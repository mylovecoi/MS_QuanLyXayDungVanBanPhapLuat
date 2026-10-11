import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import Badge from "../../../app/components/ui/badge/Badge.jsx";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getQuyTrinhs } from "../../danh-muc/api/quyTrinhSoanThaoApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { getVanBans } from "../../danh-muc/api/vanBanApi";
import { getDashboardLayout } from "../api/dashboardApi";

const message = (error) =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  error?.message ||
  "Không thể tải dashboard.";

function sizeClass(size) {
  switch (`${size || ""}`.toLowerCase()) {
    case "small": return "lg:col-span-3";
    case "large": return "lg:col-span-8";
    case "full": return "lg:col-span-12";
    default: return "lg:col-span-4";
  }
}

function displayLabel(type) {
  return {
    metric: "Thẻ số liệu",
    bar: "Biểu đồ cột",
    line: "Biểu đồ đường",
    donut: "Biểu đồ donut",
    table: "Bảng",
    list: "Danh sách",
  }[type] || type;
}

function WidgetBody({ widget }) {
  const data = widget.duLieu;
  const rows = data?.nhomSoLieu || [];
  const max = Math.max(...rows.map((row) => row.soLuong), 1);

  if (widget.dangHienThi === "table") {
    return (
      <div className="mt-4 divide-y rounded-lg border">
        {rows.length ? rows.map((row) => (
          <div key={row.nhom} className="flex items-center justify-between px-4 py-3 text-sm">
            <span className="text-gray-600">{row.nhom}</span>
            <span className="font-semibold text-gray-800">{row.soLuong}</span>
          </div>
        )) : <div className="p-4 text-sm text-gray-500">Chưa có số liệu chi tiết.</div>}
      </div>
    );
  }

  if (widget.dangHienThi === "bar" || widget.dangHienThi === "line" || widget.dangHienThi === "donut") {
    return (
      <div className="mt-4 space-y-3">
        {rows.length ? rows.map((row) => (
          <div key={row.nhom}>
            <div className="mb-1 flex justify-between text-xs text-gray-500">
              <span>{row.nhom}</span>
              <span>{row.soLuong}</span>
            </div>
            <div className="h-2 rounded-full bg-gray-100">
              <div className="h-2 rounded-full bg-brand-500" style={{ width: `${Math.max(8, Math.round(row.soLuong * 100 / max))}%` }} />
            </div>
          </div>
        )) : <div className="rounded-lg border p-4 text-sm text-gray-500">Chưa có số liệu biểu đồ.</div>}
      </div>
    );
  }

  if (widget.dangHienThi === "list") {
    return (
      <div className="mt-4 space-y-2">
        {rows.length ? rows.slice(0, 5).map((row) => (
          <div key={row.nhom} className="flex items-center justify-between rounded-lg border px-3 py-2 text-sm">
            <span>{row.nhom}</span>
            <Badge size="sm" color="warning">{row.soLuong}</Badge>
          </div>
        )) : <div className="rounded-lg border p-4 text-sm text-gray-500">Chưa có danh sách cần theo dõi.</div>}
      </div>
    );
  }

  return (
    <div className="mt-4">
      <div className="text-3xl font-semibold text-gray-800 dark:text-white/90">{data?.giaTri ?? 0}</div>
      <div className="mt-1 text-sm text-gray-500">{data?.donViTinh || "mục"}</div>
    </div>
  );
}

const currentYear = new Date().getFullYear();
const emptyFilters = {
  nam: String(currentYear),
  donViId: "",
  loaiVanBanId: "",
  quyTrinhSoanThaoId: "",
  trangThaiId: "",
  tuNgay: "",
  denNgay: "",
};

function itemsOf(response) {
  return response?.data || response?.items || [];
}

export default function DashboardKhaiThacDuLieuPage() {
  const navigate = useNavigate();
  const [layout, setLayout] = useState(null);
  const [filters, setFilters] = useState(emptyFilters);
  const [options, setOptions] = useState({ donVis: [], vanBans: [], quyTrinhs: [], trangThais: [] });
  const [loading, setLoading] = useState(true);
  const [loadingOptions, setLoadingOptions] = useState(true);
  const [error, setError] = useState("");

  const widgets = useMemo(() => (layout?.widgets || []).filter((item) => item.hienThi), [layout]);

  const queryFilters = useMemo(() => ({
    nam: filters.nam ? Number(filters.nam) : undefined,
    donViId: filters.donViId || undefined,
    loaiVanBanId: filters.loaiVanBanId || undefined,
    quyTrinhSoanThaoId: filters.quyTrinhSoanThaoId || undefined,
    trangThaiId: filters.trangThaiId || undefined,
    tuNgay: filters.tuNgay || undefined,
    denNgay: filters.denNgay || undefined,
  }), [filters]);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      setLayout(await getDashboardLayout(queryFilters));
    } catch (e) {
      setError(message(e));
    } finally {
      setLoading(false);
    }
  };

  const loadOptions = async () => {
    try {
      setLoadingOptions(true);
      const [donViResult, vanBanResult, quyTrinhResult, trangThaiResult] = await Promise.allSettled([
        getDonViOptions(),
        getVanBans({ pageSize: 200, pageCurrent: 1 }),
        getQuyTrinhs({ pageSize: 200, pageCurrent: 1 }),
        getTrangThais({ pageSize: 200, pageCurrent: 1 }),
      ]);
      setOptions({
        donVis: donViResult.status === "fulfilled" ? donViResult.value || [] : [],
        vanBans: vanBanResult.status === "fulfilled" ? itemsOf(vanBanResult.value) : [],
        quyTrinhs: quyTrinhResult.status === "fulfilled" ? itemsOf(quyTrinhResult.value) : [],
        trangThais: trangThaiResult.status === "fulfilled" ? itemsOf(trangThaiResult.value) : [],
      });
    } finally {
      setLoadingOptions(false);
    }
  };

  useEffect(() => {
    void loadOptions();
  }, []);

  useEffect(() => {
    void load();
  }, [queryFilters]);

  const updateFilter = (field, value) => {
    setFilters((current) => ({ ...current, [field]: value }));
  };

  const resetFilters = () => {
    setFilters(emptyFilters);
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Dashboard khai thác dữ liệu</h1>
          <p className="mt-1 text-sm text-gray-500">Tổng quan điều hành, tiến độ, cảnh báo và báo cáo theo cấu hình hiển thị.</p>
        </div>
        <div className="flex gap-2">
          <button type="button" onClick={load} disabled={loading} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Tải lại</button>
          <button type="button" onClick={() => navigate("/khai-thac-du-lieu/dashboard/cau-hinh")} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white">Cấu hình</button>
        </div>
      </div>

      {error && <Alert variant="error" title="Có lỗi" message={error} />}

      <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <div>
            <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">Năm</label>
            <input type="number" value={filters.nam} onChange={(event) => updateFilter("nam", event.target.value)} className="h-11 w-full rounded-lg border px-3 text-sm" />
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">Đơn vị</label>
            <select value={filters.donViId} onChange={(event) => updateFilter("donViId", event.target.value)} disabled={loadingOptions} className="h-11 w-full rounded-lg border px-3 text-sm">
              <option value="">Tất cả đơn vị</option>
              {options.donVis.map((item) => <option key={item.id} value={item.id}>{item.tenDonVi || item.ten || item.maDonVi}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">Loại văn bản</label>
            <select value={filters.loaiVanBanId} onChange={(event) => updateFilter("loaiVanBanId", event.target.value)} disabled={loadingOptions} className="h-11 w-full rounded-lg border px-3 text-sm">
              <option value="">Tất cả loại văn bản</option>
              {options.vanBans.map((item) => <option key={item.id} value={item.id}>{item.tenVanBan || item.ten || item.maVanBan}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">Quy trình</label>
            <select value={filters.quyTrinhSoanThaoId} onChange={(event) => updateFilter("quyTrinhSoanThaoId", event.target.value)} disabled={loadingOptions} className="h-11 w-full rounded-lg border px-3 text-sm">
              <option value="">Tất cả quy trình</option>
              {options.quyTrinhs.map((item) => <option key={item.id} value={item.id}>{item.tenQuyTrinh || item.ten || item.maQuyTrinh}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">Trạng thái</label>
            <select value={filters.trangThaiId} onChange={(event) => updateFilter("trangThaiId", event.target.value)} disabled={loadingOptions} className="h-11 w-full rounded-lg border px-3 text-sm">
              <option value="">Tất cả trạng thái</option>
              {options.trangThais.map((item) => <option key={item.id} value={item.id}>{item.tenTrangThai || item.maTrangThai}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">Từ ngày</label>
            <input type="date" value={filters.tuNgay} onChange={(event) => updateFilter("tuNgay", event.target.value)} className="h-11 w-full rounded-lg border px-3 text-sm" />
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">Đến ngày</label>
            <input type="date" value={filters.denNgay} onChange={(event) => updateFilter("denNgay", event.target.value)} className="h-11 w-full rounded-lg border px-3 text-sm" />
          </div>
          <div className="flex items-end gap-2">
            <button type="button" onClick={resetFilters} className="h-11 rounded-lg border px-4 text-sm font-medium">Xóa lọc</button>
            <button type="button" onClick={load} disabled={loading} className="h-11 rounded-lg bg-brand-500 px-4 text-sm font-medium text-white disabled:opacity-50">Áp dụng</button>
          </div>
        </div>
      </div>

      {loading ? (
        <div className="rounded-xl border bg-white p-10 text-center text-sm text-gray-500">Đang tải dashboard...</div>
      ) : (
        <div className="grid gap-4 lg:grid-cols-12">
          {widgets.length ? widgets.map((widget) => (
            <div key={widget.id} className={`${sizeClass(widget.kichThuoc)} rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]`}>
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h2 className="font-semibold text-gray-800 dark:text-white/90">{widget.tenWidget}</h2>
                  <p className="mt-1 text-sm text-gray-500">{widget.moTa}</p>
                </div>
                <Badge size="sm" color="light">{displayLabel(widget.dangHienThi)}</Badge>
              </div>
              <WidgetBody widget={widget} />
            </div>
          )) : (
            <div className="lg:col-span-12 rounded-xl border bg-white p-10 text-center text-sm text-gray-500">Chưa có widget nào được bật hiển thị.</div>
          )}
        </div>
      )}
    </div>
  );
}
