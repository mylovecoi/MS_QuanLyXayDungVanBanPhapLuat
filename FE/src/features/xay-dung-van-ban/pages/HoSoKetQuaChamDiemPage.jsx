import { useEffect, useMemo, useState } from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import { Modal } from "../../../app/components/ui/modal";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { getChamDiem, getDanhSachChamDiem, xuatKetQuaChamDiemExcel } from "../api/xayDungVanBanApi";

const STATUS_GROUP = "CHAM_DIEM_HO_SO";
const CLOSED_STATUS_CODE = "DA_CHOT";

function errorMessage(error) {
  return error?.response?.data?.message || error?.message || "Không thể xử lý yêu cầu.";
}

function formatDate(value) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}

function dateForFileName() {
  const now = new Date();
  return `${now.getFullYear()}${String(now.getMonth() + 1).padStart(2, "0")}${String(now.getDate()).padStart(2, "0")}`;
}

function classifyScore(score) {
  const value = Number(score);
  if (!Number.isFinite(value)) return "-";
  if (value >= 90) return "Tốt";
  if (value >= 70) return "Khá";
  if (value >= 50) return "Trung bình";
  return "Chưa đạt";
}

export default function HoSoKetQuaChamDiemPage() {
  const [items, setItems] = useState([]);
  const [total, setTotal] = useState(0);
  const [search, setSearch] = useState("");
  const [pageCurrent, setPageCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [statuses, setStatuses] = useState([]);
  const [selected, setSelected] = useState(null);
  const [loading, setLoading] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [error, setError] = useState("");

  const totalPages = Math.max(1, Math.ceil(total / pageSize));
  const closedStatus = useMemo(
    () => statuses.find((item) => `${item.maTrangThai || ""}`.toUpperCase() === CLOSED_STATUS_CODE),
    [statuses]
  );

  const loadStatuses = async () => {
    try {
      const response = await getTrangThais({ nhomTrangThai: STATUS_GROUP, pageSize: 100, pageCurrent: 1 });
      setStatuses(response?.data || []);
    } catch {
      setStatuses([]);
    }
  };

  const load = async () => {
    if (!closedStatus?.id) {
      setItems([]);
      setTotal(0);
      return;
    }

    try {
      setLoading(true);
      setError("");
      const response = await getDanhSachChamDiem({
        search: search.trim() || undefined,
        trangThaiId: closedStatus.id,
        pageCurrent,
        pageSize,
      });
      setItems(response?.items || response?.data || []);
      setTotal(response?.totalCount || 0);
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadStatuses();
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => void load(), 250);
    return () => clearTimeout(timer);
  }, [closedStatus?.id, search, pageCurrent, pageSize]);

  const openDetail = async (item) => {
    try {
      setDetailLoading(true);
      setError("");
      const detail = await getChamDiem(item.id);
      setSelected({
        ...detail,
        maHoSo: item.maHoSo,
        tenHoSo: item.tenHoSo,
        tenDuThaoVanBan: item.tenDuThaoVanBan,
      });
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setDetailLoading(false);
    }
  };

  const exportExcel = async () => {
    if (!closedStatus?.id) {
      setError("Chưa cấu hình trạng thái DA_CHOT cho nhóm CHAM_DIEM_HO_SO.");
      return;
    }

    try {
      setExporting(true);
      setError("");
      await xuatKetQuaChamDiemExcel({
        search: search.trim() || undefined,
        trangThaiId: closedStatus.id,
      }, `ket-qua-cham-diem-${dateForFileName()}.xlsx`);
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setExporting(false);
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Kết quả chấm điểm</h1>
          <p className="mt-1 text-sm text-gray-500">Tra cứu các lượt chấm đã chốt, xem giải thích điểm và xuất dữ liệu phục vụ tổng hợp.</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <button type="button" onClick={load} disabled={loading} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Tải lại</button>
          <button type="button" onClick={exportExcel} disabled={exporting || loading || !total} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">
            {exporting ? "Đang xuất..." : "Xuất Excel"}
          </button>
        </div>
      </div>

      {error && <Alert variant="error" title="Có lỗi" message={error} />}
      {!closedStatus?.id && <Alert variant="warning" title="Thiếu cấu hình trạng thái" message="Chưa tìm thấy trạng thái DA_CHOT trong nhóm CHAM_DIEM_HO_SO." />}

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="grid gap-4 border-b border-gray-100 p-5 sm:grid-cols-12 dark:border-white/[0.05]">
          <div className="sm:col-span-2">
            <Label>Hiển thị</Label>
            <select
              value={pageSize}
              onChange={(event) => { setPageSize(Number(event.target.value)); setPageCurrent(1); }}
              className="h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 shadow-theme-xs focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/10 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90"
            >
              {[10, 20, 50].map((value) => <option key={value} value={value}>{value} hồ sơ</option>)}
            </select>
          </div>
          <div className="sm:col-span-10">
            <Label>Tìm kiếm</Label>
            <Input value={search} onChange={(event) => { setSearch(event.target.value); setPageCurrent(1); }} placeholder="Tìm mã hồ sơ, tên hồ sơ, dự thảo..." />
          </div>
        </div>

        <div className="max-w-full overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Hồ sơ</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Dự thảo</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Lần chấm</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Tổng điểm</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Xếp loại</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Ngày chốt</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <tr><td colSpan="7" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr>
              ) : items.length ? items.map((item) => (
                <tr key={item.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                  <td className="px-5 py-4">
                    <div className="font-medium text-gray-800 dark:text-white/90">{item.tenHoSo}</div>
                    <div className="mt-1 text-xs text-gray-500">{item.maHoSo} · Năm {item.namXayDung}</div>
                  </td>
                  <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.tenDuThaoVanBan || "-"}</td>
                  <td className="px-5 py-4 text-center"><Badge size="sm" color="light">Lần {item.lanCham}</Badge></td>
                  <td className="px-5 py-4 text-center text-base font-semibold text-gray-800 dark:text-white/90">{item.tongDiemChinhThuc ?? "-"}</td>
                  <td className="px-5 py-4 text-center"><Badge size="sm" color={Number(item.tongDiemChinhThuc) >= 70 ? "success" : "warning"}>{classifyScore(item.tongDiemChinhThuc)}</Badge></td>
                  <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{formatDate(item.ngayChot)}</td>
                  <td className="px-5 py-4 text-center">
                    <button type="button" onClick={() => openDetail(item)} disabled={detailLoading} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50 disabled:opacity-50">Chi tiết</button>
                  </td>
                </tr>
              )) : (
                <tr><td colSpan="7" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có kết quả chấm điểm đã chốt.</td></tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="flex items-center justify-between border-t border-gray-100 px-5 py-4 text-sm text-gray-500 dark:border-white/[0.05]">
          <span>{total} kết quả</span>
          <div className="flex items-center gap-2">
            <button type="button" disabled={pageCurrent === 1 || loading} onClick={() => setPageCurrent((value) => value - 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Trước</button>
            <span className="px-3 py-2">{pageCurrent}/{totalPages}</span>
            <button type="button" disabled={pageCurrent >= totalPages || loading} onClick={() => setPageCurrent((value) => value + 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Sau</button>
          </div>
        </div>
      </div>

      <Modal isOpen={!!selected} onClose={() => setSelected(null)} className="max-w-5xl p-6">
        <div className="space-y-4">
          <div className="flex flex-wrap items-start justify-between gap-3">
            <div>
              <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">Chi tiết kết quả chấm điểm</h2>
              <p className="mt-1 text-sm text-gray-500">{selected?.maHoSo} · {selected?.tenHoSo}</p>
            </div>
            <div className="text-right">
              <div className="text-2xl font-semibold text-gray-800 dark:text-white/90">{selected?.tongDiemChinhThuc ?? "-"}</div>
              <div className="text-sm text-gray-500">{classifyScore(selected?.tongDiemChinhThuc)}</div>
            </div>
          </div>
          <div className="max-h-[60vh] max-w-full overflow-auto rounded-lg border">
            <table className="w-full">
              <thead>
                <tr className="border-b bg-gray-50 dark:bg-white/[0.03]">
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500">Tiêu chí</th>
                  <th className="px-4 py-3 text-center text-xs font-medium text-gray-500">Đầu vào</th>
                  <th className="px-4 py-3 text-center text-xs font-medium text-gray-500">Tự động</th>
                  <th className="px-4 py-3 text-center text-xs font-medium text-gray-500">Điều chỉnh</th>
                  <th className="px-4 py-3 text-center text-xs font-medium text-gray-500">Chính thức</th>
                </tr>
              </thead>
              <tbody className="divide-y">
                {selected?.chiTiets?.length ? selected.chiTiets.map((row) => (
                  <tr key={row.id} className="align-top">
                    <td className="px-4 py-3">
                      <div className="font-medium text-gray-800 dark:text-white/90">{row.tenTieuChi}</div>
                      <div className="mt-1 text-xs text-gray-500">{row.maTieuChi} · {row.nhanMucDiem || "-"}</div>
                      {row.giaiThichDiem && <div className="mt-2 text-xs text-gray-500">{row.giaiThichDiem}</div>}
                      {row.lyDoDieuChinh && <div className="mt-2 text-xs text-warning-600">Lý do điều chỉnh: {row.lyDoDieuChinh}</div>}
                    </td>
                    <td className="px-4 py-3 text-center text-sm">{row.giaTriDauVao}</td>
                    <td className="px-4 py-3 text-center text-sm">{row.diemTuDong}</td>
                    <td className="px-4 py-3 text-center text-sm">{row.diemDieuChinh}</td>
                    <td className="px-4 py-3 text-center font-medium">{row.diemChinhThuc}/{row.diemToiDa}</td>
                  </tr>
                )) : (
                  <tr><td colSpan="5" className="px-4 py-8 text-center text-sm text-gray-500">Không có chi tiết điểm.</td></tr>
                )}
              </tbody>
            </table>
          </div>
          <div className="flex justify-end border-t pt-4">
            <button type="button" onClick={() => setSelected(null)} className="rounded-lg border px-4 py-2 text-sm font-medium">Đóng</button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
