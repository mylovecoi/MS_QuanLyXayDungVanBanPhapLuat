import { useEffect, useMemo, useState } from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import { Modal } from "../../../app/components/ui/modal";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import {
  chotChamDiem,
  createChamDiem,
  deleteChamDiem,
  dieuChinhChamDiem,
  getChamDiem,
  getHoSoCanChamDiem,
  getLichSuChamDiem,
  huyChotChamDiem,
  tinhLaiChamDiem,
} from "../api/xayDungVanBanApi";

const ALL_VALUE = "__ALL__";
const STATUS_GROUP = "CHAM_DIEM_HO_SO";
const isGuid = (value) => /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(String(value || ""));

function message(error) {
  const payload = error?.response?.data;
  if (typeof payload === "string" && payload.trim()) return payload;
  if (payload?.message) return payload.message;
  if (payload?.title) return payload.title;
  const validationMessages = Object.values(payload?.errors || {}).flat().filter(Boolean);
  return validationMessages[0] || error?.message || "Không thể xử lý yêu cầu.";
}

function formatDate(value) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}

export default function HoSoChamDiemPage() {
  const [items, setItems] = useState([]);
  const [total, setTotal] = useState(0);
  const [search, setSearch] = useState("");
  const [trangThaiId, setTrangThaiId] = useState(ALL_VALUE);
  const [pageCurrent, setPageCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [statuses, setStatuses] = useState([]);
  const [selected, setSelected] = useState(null);
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [adjustTarget, setAdjustTarget] = useState(null);
  const [adjustForm, setAdjustForm] = useState({ diemDieuChinh: 0, lyDoDieuChinh: "" });
  const [statusDialog, setStatusDialog] = useState(null);

  const totalPages = Math.max(1, Math.ceil(total / pageSize));
  const statusMap = useMemo(() => new Map(statuses.map((item) => [item.id, item])), [statuses]);
  const statusOptions = [{ value: ALL_VALUE, label: "Tất cả trạng thái" }, ...statuses.map((item) => ({ value: item.id, label: item.tenTrangThai || item.maTrangThai }))];
  const selectedStatus = selected?.trangThaiId ? statusMap.get(selected.trangThaiId) : null;
  const selectedStatusCode = `${selectedStatus?.maTrangThai || ""}`.toUpperCase();
  const canEditSelected = selectedStatusCode === "NHAP";
  const canCancelSelected = selectedStatusCode === "DA_CHOT";
  const getStatusLabel = (id) => id ? (statusMap.get(id)?.tenTrangThai || statusMap.get(id)?.maTrangThai || "-") : "Chưa chấm";
  const getStatusCode = (id) => `${statusMap.get(id)?.maTrangThai || ""}`.toUpperCase();
  const statusDialogOptions = statuses
    .filter((item) => getStatusCode(item.id) === (statusDialog?.mode === "chot" ? "DA_CHOT" : "NHAP"))
    .map((item) => ({ value: item.id, label: item.tenTrangThai || item.maTrangThai }));

  const loadStatuses = async () => {
    try {
      const response = await getTrangThais({ nhomTrangThai: STATUS_GROUP, pageSize: 100, pageCurrent: 1 });
      setStatuses(response?.data || []);
    } catch {
      setStatuses([]);
    }
  };

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const response = await getHoSoCanChamDiem({
        search: search.trim() || undefined,
        trangThaiId: isGuid(trangThaiId) ? trangThaiId : undefined,
        pageCurrent,
        pageSize,
      });
      setItems(response?.items || response?.data || []);
      setTotal(response?.totalCount || 0);
    } catch (e) {
      setError(message(e));
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
  }, [search, trangThaiId, pageCurrent, pageSize]);

  const openDetail = async (item) => {
    try {
      setDetailLoading(true);
      setError("");
      const [detail, historyItems] = await Promise.all([
        getChamDiem(item.id),
        getLichSuChamDiem(item.id).catch(() => []),
      ]);
      setSelected(detail);
      setHistory(historyItems || []);
    } catch (e) {
      setError(message(e));
    } finally {
      setDetailLoading(false);
    }
  };

  const createForRecord = async (item) => {
    const trangThaiNhapId = statuses.find((status) => `${status.maTrangThai || ""}`.toUpperCase() === "NHAP")?.id || statuses[0]?.id;
    if (!trangThaiNhapId) {
      setError("Chưa có trạng thái chấm điểm để tạo lượt chấm.");
      return;
    }

    try {
      setSaving(true);
      setError("");
      setSuccess("");
      const created = await createChamDiem(item.hoSoId, { trangThaiNhapId, ghiChu: "Tạo từ giao diện chấm điểm" });
      setSuccess("Đã tạo lượt chấm điểm.");
      await load();
      if (created?.id) await openDetail(created);
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  const recalculate = async (id) => {
    try {
      setSaving(true);
      setError("");
      const detail = await tinhLaiChamDiem(id);
      setSelected(detail);
      setHistory(await getLichSuChamDiem(id).catch(() => []));
      setSuccess("Đã tính lại điểm.");
      await load();
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  const openAdjust = (row) => {
    setAdjustTarget(row);
    setAdjustForm({ diemDieuChinh: row.diemDieuChinh ?? 0, lyDoDieuChinh: row.lyDoDieuChinh || "" });
    setError("");
  };

  const submitAdjust = async () => {
    if (!selected || !adjustTarget) return;
    if (!adjustForm.lyDoDieuChinh.trim()) {
      setError("Nhập lý do điều chỉnh điểm.");
      return;
    }

    try {
      setSaving(true);
      const updated = await dieuChinhChamDiem(selected.id, adjustTarget.id, {
        diemDieuChinh: Number(adjustForm.diemDieuChinh) || 0,
        lyDoDieuChinh: adjustForm.lyDoDieuChinh.trim(),
      });
      setSelected(updated);
      setHistory(await getLichSuChamDiem(selected.id).catch(() => []));
      setAdjustTarget(null);
      setSuccess("Đã điều chỉnh điểm.");
      await load();
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  const openStatusDialog = (mode, score = selected) => {
    if (!score?.id) return;
    const trangThai = statuses.find((item) => {
      const code = `${item.maTrangThai || ""}`.toUpperCase();
      return mode === "chot" ? code === "DA_CHOT" : code === "NHAP";
    }) || statuses[0];
    if (!trangThai) {
      setError("Chưa có trạng thái phù hợp để chuyển chấm điểm.");
      return;
    }
    setStatusDialog({
      mode,
      scoreId: score.id,
      lanCham: score.lanCham,
      tongDiemChinhThuc: score.tongDiemChinhThuc,
      trangThaiId: trangThai.id,
      ghiChu: "",
    });
  };

  const submitStatus = async () => {
    if (!statusDialog?.scoreId) return;

    try {
      setSaving(true);
      const updated = statusDialog.mode === "chot"
        ? await chotChamDiem(statusDialog.scoreId, { trangThaiId: statusDialog.trangThaiId, ghiChu: statusDialog.ghiChu })
        : await huyChotChamDiem(statusDialog.scoreId, { trangThaiId: statusDialog.trangThaiId, ghiChu: statusDialog.ghiChu });
      if (selected?.id === updated?.id) {
        setSelected(updated);
        setHistory(await getLichSuChamDiem(updated.id).catch(() => []));
      }
      setStatusDialog(null);
      setSuccess(statusDialog.mode === "chot" ? "Đã chốt điểm và khóa chỉnh sửa." : "Đã bỏ chốt, có thể tiếp tục điều chỉnh điểm.");
      await load();
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  const removeScore = async (item) => {
    if (!window.confirm(`Xóa lượt chấm điểm hồ sơ "${item.tenHoSo}"?`)) return;

    try {
      setSaving(true);
      await deleteChamDiem(item.chamDiemId || item.id);
      if (selected?.id === (item.chamDiemId || item.id)) {
        setSelected(null);
        setHistory([]);
      }
      setSuccess("Đã xóa lượt chấm điểm.");
      await load();
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Chấm điểm hồ sơ xây dựng văn bản</h1>
          <p className="mt-1 text-sm text-gray-500">Theo dõi lượt chấm, điều chỉnh điểm và chốt kết quả đánh giá hồ sơ.</p>
        </div>
        <button type="button" onClick={load} disabled={loading} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Tải lại</button>
      </div>

      {error && <Alert variant="error" title="Không thể xử lý" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="grid gap-4 border-b border-gray-100 p-5 sm:grid-cols-12 dark:border-white/[0.05]">
          <div className="sm:col-span-2">
            <Label>Hiển thị</Label>
            <Select value={pageSize} onChange={(value) => { setPageSize(Number(value)); setPageCurrent(1); }} options={[10, 20, 50].map((value) => ({ value, label: `${value} hồ sơ` }))} />
          </div>
          <div className="sm:col-span-6">
            <Label>Tìm kiếm</Label>
            <Input value={search} onChange={(event) => { setSearch(event.target.value); setPageCurrent(1); }} placeholder="Tìm mã hồ sơ, tên hồ sơ, dự thảo..." />
          </div>
          <div className="sm:col-span-4">
            <Label>Trạng thái</Label>
            <Select value={trangThaiId} onChange={(value) => { setTrangThaiId(value); setPageCurrent(1); }} options={statusOptions} />
          </div>
        </div>

        <div className="max-w-full overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Hồ sơ</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Lần chấm</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Tổng điểm</th>
                  <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Ngày chấm</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Ngày chốt</th>
                  <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Quy trình</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <tr><td colSpan="7" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr>
              ) : items.length ? items.map((item) => {
                const rowStatusCode = getStatusCode(item.trangThaiId);
                const score = item.chamDiemId ? { id: item.chamDiemId, lanCham: item.lanCham, tongDiemChinhThuc: item.tongDiemChinhThuc } : null;
                return (
                <tr key={item.hoSoId} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                  <td className="px-5 py-4">
                    <div className="font-medium text-gray-800 dark:text-white/90">{item.tenHoSo}</div>
                    <div className="mt-1 text-xs text-gray-500">{item.maHoSo} · {item.tenDuThaoVanBan}</div>
                    <div className="mt-1 text-xs text-gray-500">Năm {item.namXayDung}</div>
                  </td>
                  <td className="px-5 py-4 text-center">
                    <div>{item.lanCham ? <Badge size="sm" color="light">Lần {item.lanCham}</Badge> : <span className="text-xs text-gray-500">Chưa chấm</span>}</div>
                    {item.trangThaiId && <div className="mt-1 text-xs text-gray-500">{getStatusLabel(item.trangThaiId)}</div>}
                  </td>
                  <td className="px-5 py-4 text-center font-medium text-gray-800 dark:text-white/90">{item.tongDiemChinhThuc ?? "-"}</td>
                  <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{formatDate(item.ngayCham)}</td>
                  <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{formatDate(item.ngayChot)}</td>
                  <td className="px-5 py-4 text-center"><Badge size="sm" color={item.daHoanThanhQuyTrinh ? "success" : "warning"}>{item.daHoanThanhQuyTrinh ? "Đã hoàn thành" : "Đang xử lý"}</Badge></td>
                  <td className="px-5 py-4">
                    <div className="flex flex-wrap justify-center gap-1">
                      {item.chamDiemId && <button type="button" onClick={() => openDetail({ id: item.chamDiemId })} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50">Chi tiết</button>}
                      {score && rowStatusCode === "NHAP" && <button type="button" onClick={() => openStatusDialog("chot", score)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-success-600 hover:bg-success-50 disabled:opacity-50">Chốt điểm</button>}
                      {score && rowStatusCode === "DA_CHOT" && <button type="button" onClick={() => openStatusDialog("huy", score)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-warning-600 hover:bg-warning-50 disabled:opacity-50">Bỏ chốt</button>}
                      <button type="button" onClick={() => createForRecord(item)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 disabled:opacity-50">Chấm mới</button>
                      {item.chamDiemId && rowStatusCode === "NHAP" && <button type="button" onClick={() => removeScore(item)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-error-600 hover:bg-error-50 disabled:opacity-50">Xóa</button>}
                    </div>
                  </td>
                </tr>
                );
              }) : (
                <tr><td colSpan="7" className="px-5 py-10 text-center text-sm text-gray-500">Không có hồ sơ để chấm điểm.</td></tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="flex items-center justify-between border-t border-gray-100 px-5 py-4 text-sm text-gray-500 dark:border-white/[0.05]">
          <span>{total} lượt chấm</span>
          <div className="flex items-center gap-2">
            <button type="button" disabled={pageCurrent === 1 || loading} onClick={() => setPageCurrent((value) => value - 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Trước</button>
            <span className="px-3 py-2">{pageCurrent}/{totalPages}</span>
            <button type="button" disabled={pageCurrent >= totalPages || loading} onClick={() => setPageCurrent((value) => value + 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Sau</button>
          </div>
        </div>
      </div>

      <Modal isOpen={!!selected} onClose={() => !saving && setSelected(null)} className="max-w-6xl p-6">
        {selected && <div className="grid max-h-[75vh] gap-5 overflow-y-auto pr-1 xl:grid-cols-[minmax(0,1.2fr)_minmax(0,0.8fr)]">
          <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="flex flex-wrap items-start justify-between gap-3 border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]">
              <div>
                <h2 className="font-semibold text-gray-800 dark:text-white/90">Chi tiết chấm điểm</h2>
                <p className="mt-1 text-sm text-gray-500">Lần {selected.lanCham} · Tổng điểm chính thức {selected.tongDiemChinhThuc}</p>
                {selectedStatus && <div className="mt-2"><Badge size="sm" color={selectedStatusCode === "DA_CHOT" ? "success" : selectedStatusCode === "DA_HUY" ? "error" : "light"}>{selectedStatus.tenTrangThai || selectedStatus.maTrangThai}</Badge></div>}
              </div>
              <div className="flex flex-wrap gap-2">
                <button type="button" onClick={() => recalculate(selected.id)} disabled={saving || detailLoading || !canEditSelected} className="rounded-lg border px-3 py-2 text-xs font-medium disabled:opacity-50">Tính lại</button>
                <button type="button" onClick={() => openStatusDialog("chot", selected)} disabled={saving || detailLoading || !canEditSelected} className="rounded-lg bg-brand-500 px-3 py-2 text-xs font-medium text-white disabled:opacity-50">Chốt điểm</button>
                <button type="button" onClick={() => openStatusDialog("huy", selected)} disabled={saving || detailLoading || !canCancelSelected} className="rounded-lg border px-3 py-2 text-xs font-medium disabled:opacity-50">Bỏ chốt</button>
              </div>
            </div>
            <div className="max-w-full overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Tiêu chí</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Đầu vào</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Tự động</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Điều chỉnh</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Chính thức</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                  {detailLoading ? (
                    <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải chi tiết...</td></tr>
                  ) : selected.chiTiets?.length ? selected.chiTiets.map((row) => (
                    <tr key={row.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                      <td className="px-5 py-4">
                        <div className="font-medium text-gray-800 dark:text-white/90">{row.tenTieuChi}</div>
                        <div className="mt-1 text-xs text-gray-500">{row.maTieuChi} · {row.nhanMucDiem || "-"}</div>
                        {row.giaiThichDiem && <div className="mt-2 text-xs text-gray-500">{row.giaiThichDiem}</div>}
                        {row.lyDoDieuChinh && <div className="mt-2 text-xs text-warning-600">Lý do: {row.lyDoDieuChinh}</div>}
                      </td>
                      <td className="px-5 py-4 text-center text-sm">{row.giaTriDauVao}</td>
                      <td className="px-5 py-4 text-center text-sm">{row.diemTuDong}</td>
                      <td className="px-5 py-4 text-center text-sm">{row.diemDieuChinh}</td>
                      <td className="px-5 py-4 text-center font-medium">{row.diemChinhThuc}/{row.diemToiDa}</td>
                      <td className="px-5 py-4 text-center">
                        <button type="button" onClick={() => openAdjust(row)} disabled={saving || !canEditSelected} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50 disabled:opacity-50">Điều chỉnh</button>
                      </td>
                    </tr>
                  )) : (
                    <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Không có chi tiết điểm.</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="mb-4">
              <h2 className="font-semibold text-gray-800 dark:text-white/90">Lịch sử thao tác</h2>
              <p className="mt-1 text-sm text-gray-500">Ghi nhận thay đổi trong quá trình chấm điểm.</p>
            </div>
            <div className="space-y-3">
              {history.length ? history.map((item) => (
                <div key={item.id} className="rounded-lg border p-3 text-sm">
                  <div className="flex items-center justify-between gap-3">
                    <Badge size="sm" color="light">{item.loaiThaoTac}</Badge>
                    <span className="text-xs text-gray-500">{formatDate(item.thoiGianThucHien)}</span>
                  </div>
                  <div className="mt-2 text-gray-700 dark:text-gray-300">{item.noiDung}</div>
                </div>
              )) : (
                <div className="rounded-lg border p-6 text-center text-sm text-gray-500">Chưa có lịch sử thao tác.</div>
              )}
            </div>
          </div>
        </div>}
      </Modal>

      <Modal isOpen={!!adjustTarget} onClose={() => !saving && setAdjustTarget(null)} className="max-w-xl p-6">
        <div className="space-y-4">
          <div>
            <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">Điều chỉnh điểm</h2>
            <p className="mt-1 text-sm text-gray-500">{adjustTarget?.tenTieuChi}</p>
          </div>
          <div>
            <Label>Điểm điều chỉnh *</Label>
            <Input type="number" step="0.01" value={adjustForm.diemDieuChinh} onChange={(event) => setAdjustForm((current) => ({ ...current, diemDieuChinh: event.target.value }))} />
          </div>
          <div>
            <Label>Lý do điều chỉnh *</Label>
            <TextArea rows={4} value={adjustForm.lyDoDieuChinh} onChange={(value) => setAdjustForm((current) => ({ ...current, lyDoDieuChinh: value }))} />
          </div>
          <div className="flex justify-end gap-3 border-t pt-4">
            <button type="button" onClick={() => setAdjustTarget(null)} disabled={saving} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Hủy</button>
            <button type="button" onClick={submitAdjust} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">Lưu điều chỉnh</button>
          </div>
        </div>
      </Modal>

      <Modal isOpen={!!statusDialog} onClose={() => !saving && setStatusDialog(null)} className="max-w-xl p-6">
        <div className="space-y-4">
          <div>
            <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">{statusDialog?.mode === "chot" ? "Chốt điểm" : "Bỏ chốt điểm"}</h2>
            <p className="mt-1 text-sm text-gray-500">Lần {statusDialog?.lanCham} · Tổng điểm {statusDialog?.tongDiemChinhThuc}</p>
          </div>
          <div>
            <Label>Trạng thái sau xử lý</Label>
            <Select value={statusDialog?.trangThaiId || ""} onChange={(value) => setStatusDialog((current) => ({ ...current, trangThaiId: value }))} options={statusDialogOptions} />
          </div>
          <div>
            <Label>Ghi chú</Label>
            <TextArea rows={4} value={statusDialog?.ghiChu || ""} onChange={(value) => setStatusDialog((current) => ({ ...current, ghiChu: value }))} />
          </div>
          <div className="flex justify-end gap-3 border-t pt-4">
            <button type="button" onClick={() => setStatusDialog(null)} disabled={saving} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Hủy</button>
            <button type="button" onClick={submitStatus} disabled={saving || !statusDialog?.trangThaiId} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">{statusDialog?.mode === "chot" ? "Chốt điểm" : "Bỏ chốt"}</button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
