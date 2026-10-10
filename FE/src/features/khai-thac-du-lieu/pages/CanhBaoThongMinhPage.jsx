import { useEffect, useMemo, useState } from "react";
import { useNavigate, useSearchParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import Badge from "../../../app/components/ui/badge/Badge.jsx";
import Input from "../../../app/components/forms/input/InputField.jsx";
import Label from "../../../app/components/forms/Label.jsx";
import Select from "../../../app/components/forms/Select.jsx";
import TextArea from "../../../app/components/forms/input/TextArea.jsx";
import { Modal } from "../../../app/components/ui/modal/index.jsx";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getUserAccounts } from "../../quan-tri-he-thong/api/userAccountApi";
import {
  danhDauDaXemCanhBao,
  getCanhBao,
  getLichSuCanhBao,
  getNhacViecCanhBao,
  hoanThanhNhacViec,
  huyNhacViec,
  quetCanhBaoTuDong,
  taoNhacViecCanhBao,
  xacNhanXuLyCanhBao,
} from "../api/canhBaoApi";

const ALL_VALUE = "__ALL__";

const toError = (error) =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  (typeof error?.response?.data === "string" ? error.response.data : null) ||
  error?.message ||
  "Không thể tải cảnh báo.";

const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const dateValue = (value) => value ? new Date(value).toISOString().slice(0, 10) : "";

const reminderInitial = {
  tieuDe: "",
  noiDung: "",
  nguoiNhanId: "",
  donViNhanId: "",
  hanXuLy: "",
  thoiGianNhac: "",
  mucDoUuTien: "TRUNG_BINH",
};

function severityColor(value) {
  const code = `${value || ""}`.toUpperCase();
  if (code.includes("KHAN") || code.includes("CAO")) return "error";
  if (code.includes("TRUNG")) return "warning";
  return "light";
}

function statusColor(value) {
  const code = `${value || ""}`.toUpperCase();
  if (code === "DA_XU_LY" || code === "DA_HOAN_THANH") return "success";
  if (code === "DANG_XU_LY" || code === "DA_GUI") return "warning";
  return "error";
}

function getObjectUrl(item) {
  if (item.doiTuongNguon === "DANG_KY_XAY_DUNG_VAN_BAN") {
    return `/dang-ky-xay-dung-van-ban/ho-so/${item.doiTuongNguonId}`;
  }

  return "";
}

function defaultReminderForm(item) {
  const priority = item?.mucDo === "KHAN_CAP" || item?.mucDo === "CAO" ? item.mucDo : "TRUNG_BINH";
  return {
    ...reminderInitial,
    tieuDe: item?.tieuDe ? `Nhắc xử lý: ${item.tieuDe}` : "",
    noiDung: item?.noiDung || "",
    donViNhanId: item?.donViNhanId || "",
    hanXuLy: dateValue(item?.hanXuLy),
    mucDoUuTien: priority,
  };
}

function actionLabel(value) {
  const labels = {
    TAO_CANH_BAO: "Tạo cảnh báo",
    CAP_NHAT_CANH_BAO: "Cập nhật cảnh báo",
    XEM_CANH_BAO: "Xem cảnh báo",
    XU_LY_CANH_BAO: "Xử lý cảnh báo",
    TAO_NHAC_VIEC: "Tạo nhắc việc",
    XEM_NHAC_VIEC: "Xem nhắc việc",
    HOAN_THANH_NHAC_VIEC: "Hoàn thành nhắc việc",
    HUY_NHAC_VIEC: "Hủy nhắc việc",
    CAP_NHAT_TRANG_THAI_CANH_BAO: "Cập nhật trạng thái",
    TU_DONG_TAO_CANH_BAO: "Tự động tạo cảnh báo",
    TU_DONG_CAP_NHAT_CANH_BAO: "Tự động cập nhật cảnh báo",
    TU_DONG_DONG_CANH_BAO: "Tự động đóng cảnh báo",
    TU_DONG_CHUYEN_NHAC_VIEC_QUA_HAN: "Tự động chuyển quá hạn",
  };

  return labels[value] || value || "-";
}

export default function CanhBaoThongMinhPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const focusCanhBaoId = searchParams.get("canhBaoId");
  const [items, setItems] = useState([]);
  const [total, setTotal] = useState(0);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [severity, setSeverity] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [selectedAlert, setSelectedAlert] = useState(null);
  const [reminders, setReminders] = useState([]);
  const [histories, setHistories] = useState([]);
  const [reminderForm, setReminderForm] = useState(reminderInitial);
  const [reminderLoading, setReminderLoading] = useState(false);
  const [donVis, setDonVis] = useState([]);
  const [users, setUsers] = useState([]);
  const [actionModal, setActionModal] = useState({ type: "", item: null, note: "" });
  const [openedDeepLinkId, setOpenedDeepLinkId] = useState("");

  const statusOptions = useMemo(() => [
    { value: ALL_VALUE, label: "Tất cả trạng thái" },
    { value: "MOI", label: "Mới" },
    { value: "DANG_XU_LY", label: "Đang xử lý" },
    { value: "DA_XU_LY", label: "Đã xử lý" },
  ], []);

  const severityOptions = useMemo(() => [
    { value: ALL_VALUE, label: "Tất cả mức độ" },
    { value: "THAP", label: "Thấp" },
    { value: "TRUNG_BINH", label: "Trung bình" },
    { value: "CAO", label: "Cao" },
    { value: "KHAN_CAP", label: "Khẩn cấp" },
  ], []);

  const priorityOptions = useMemo(() => [
    { value: "THAP", label: "Thấp" },
    { value: "TRUNG_BINH", label: "Trung bình" },
    { value: "CAO", label: "Cao" },
    { value: "KHAN_CAP", label: "Khẩn cấp" },
  ], []);

  const donViOptions = useMemo(() => [
    { value: "", label: "Không chọn đơn vị" },
    ...donVis.map((item) => ({
      value: item.id,
      label: item.tenDonVi || item.ten || item.maDonVi || item.id,
    })),
  ], [donVis]);

  const userOptions = useMemo(() => [
    { value: "", label: "Không chọn người nhận" },
    ...users.map((item) => ({
      value: item.id,
      label: `${item.name || item.username || item.email || item.id}${item.username ? ` (${item.username})` : ""}`,
    })),
  ], [users]);

  const donViMap = useMemo(() => new Map(donVis.map((item) => [
    item.id,
    item.tenDonVi || item.ten || item.maDonVi || item.id,
  ])), [donVis]);

  const userMap = useMemo(() => new Map(users.map((item) => [
    item.id,
    item.name || item.username || item.email || item.id,
  ])), [users]);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const data = await getCanhBao({
        search: search.trim() || undefined,
        trangThaiXuLy: status || undefined,
        mucDo: severity || undefined,
        pageSize: 100,
        pageCurrent: 1,
      });
      const nextItems = data?.items ?? data?.data ?? [];
      setItems(Array.isArray(nextItems) ? nextItems : []);
      setTotal(data?.totalCount ?? nextItems.length ?? 0);
    } catch (requestError) {
      setItems([]);
      setTotal(0);
      setError(toError(requestError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
    loadOptions();
  }, []);

  const loadOptions = async () => {
    const [donViResult, userResult] = await Promise.allSettled([
      getDonViOptions(),
      getUserAccounts({ pageSize: 200, pageCurrent: 1 }),
    ]);

    if (donViResult.status === "fulfilled") {
      setDonVis(Array.isArray(donViResult.value) ? donViResult.value : []);
    }

    if (userResult.status === "fulfilled") {
      setUsers(Array.isArray(userResult.value?.data) ? userResult.value.data : []);
    }
  };

  const loadReminders = async (alertId) => {
    try {
      setReminderLoading(true);
      const [reminderData, historyData] = await Promise.all([
        getNhacViecCanhBao(alertId),
        getLichSuCanhBao(alertId),
      ]);
      setReminders(Array.isArray(reminderData) ? reminderData : []);
      setHistories(Array.isArray(historyData) ? historyData : []);
    } catch (requestError) {
      setReminders([]);
      setHistories([]);
      setError(toError(requestError));
    } finally {
      setReminderLoading(false);
    }
  };

  const runGenerator = async () => {
    try {
      setSaving(true);
      setError("");
      const result = await quetCanhBaoTuDong();
      setSuccess(`Đã quét ${result.soDoiTuongDuocKiemTra || 0} hồ sơ, tạo mới ${result.soCanhBaoTaoMoi || 0}, cập nhật ${result.soCanhBaoCapNhat || 0}, chuyển quá hạn ${result.soNhacViecChuyenQuaHan || 0} nhắc việc.`);
      await load();
    } catch (requestError) {
      setError(toError(requestError));
    } finally {
      setSaving(false);
    }
  };

  const openReminderModal = async (item) => {
    setSelectedAlert(item);
    setReminderForm(defaultReminderForm(item));
    setSuccess("");
    await loadReminders(item.id);
  };

  const closeReminderModal = () => {
    if (saving) return;
    setSelectedAlert(null);
    setReminders([]);
    setHistories([]);
  };

  useEffect(() => {
    if (!focusCanhBaoId || openedDeepLinkId === focusCanhBaoId || !items.length) return;

    const item = items.find((current) => current.id === focusCanhBaoId);
    if (!item) return;

    setOpenedDeepLinkId(focusCanhBaoId);
    openReminderModal(item);
  }, [focusCanhBaoId, items, openedDeepLinkId]);

  const closeActionModal = () => {
    if (saving) return;
    setActionModal({ type: "", item: null, note: "" });
  };

  const updateReminderForm = (key, value) => {
    setReminderForm((current) => ({ ...current, [key]: value }));
  };

  const createReminder = async () => {
    if (!selectedAlert) return;
    if (!reminderForm.tieuDe.trim() || !reminderForm.noiDung.trim()) {
      setError("Nhập tiêu đề và nội dung nhắc việc.");
      return;
    }

    try {
      setSaving(true);
      setError("");
      await taoNhacViecCanhBao(selectedAlert.id, {
        tieuDe: reminderForm.tieuDe.trim(),
        noiDung: reminderForm.noiDung.trim(),
        nguoiNhanId: reminderForm.nguoiNhanId || null,
        donViNhanId: reminderForm.donViNhanId || null,
        hanXuLy: reminderForm.hanXuLy || null,
        thoiGianNhac: reminderForm.thoiGianNhac || null,
        mucDoUuTien: reminderForm.mucDoUuTien || "TRUNG_BINH",
      });
      setSuccess("Đã thêm nhắc việc cho cảnh báo.");
      setReminderForm(defaultReminderForm(selectedAlert));
      await loadReminders(selectedAlert.id);
      await load();
    } catch (requestError) {
      setError(toError(requestError));
    } finally {
      setSaving(false);
    }
  };

  const openCompleteReminderModal = (reminder) => {
    setActionModal({ type: "REMINDER_COMPLETE", item: reminder, note: reminder.ghiChuHoanThanh || "" });
  };

  const openCompleteAlertModal = (item) => {
    setActionModal({ type: "ALERT_COMPLETE", item, note: item.ghiChuXuLy || "" });
  };

  const submitActionModal = async () => {
    if (!actionModal.item) return;

    try {
      setSaving(true);
      if (actionModal.type === "REMINDER_COMPLETE") {
        await hoanThanhNhacViec(actionModal.item.canhBaoId, actionModal.item.id, { ghiChuHoanThanh: actionModal.note || null });
        await loadReminders(actionModal.item.canhBaoId);
        setSuccess("Đã hoàn thành nhắc việc.");
      }

      if (actionModal.type === "ALERT_COMPLETE") {
        await xacNhanXuLyCanhBao(actionModal.item.id, { ghiChuXuLy: actionModal.note || null });
        setSuccess("Đã xác nhận xử lý cảnh báo.");
        await load();
      }

      setActionModal({ type: "", item: null, note: "" });
    } catch (requestError) {
      setError(toError(requestError));
    } finally {
      setSaving(false);
    }
  };

  const cancelReminder = async (reminder) => {
    try {
      setSaving(true);
      await huyNhacViec(reminder.canhBaoId, reminder.id);
      await loadReminders(reminder.canhBaoId);
    } catch (requestError) {
      setError(toError(requestError));
    } finally {
      setSaving(false);
    }
  };

  const markRead = async (item) => {
    try {
      setSaving(true);
      await danhDauDaXemCanhBao(item.id);
      await load();
    } catch (requestError) {
      setError(toError(requestError));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Cảnh báo thông minh</h1>
          <p className="mt-1 text-sm text-gray-500">Theo dõi cảnh báo, tạo nhắc việc và xử lý đúng hạn.</p>
        </div>
        <button type="button" onClick={runGenerator} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">
          {saving ? "Đang quét..." : "Quét cảnh báo"}
        </button>
      </div>

      {error && <Alert variant="error" title="Có lỗi" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      <div className="grid gap-3 lg:grid-cols-[1fr_220px_220px_auto]">
        <Input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Tìm tiêu đề hoặc nội dung cảnh báo..." />
        <Select value={status || ALL_VALUE} onChange={(value) => setStatus(value === ALL_VALUE ? "" : value)} options={statusOptions} />
        <Select value={severity || ALL_VALUE} onChange={(value) => setSeverity(value === ALL_VALUE ? "" : value)} options={severityOptions} />
        <button type="button" onClick={load} disabled={loading} className="rounded-lg border px-4 py-2.5 text-sm font-medium text-gray-700 disabled:opacity-50 dark:text-gray-300">
          Tải dữ liệu
        </button>
      </div>

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="border-b border-gray-100 px-5 py-4 text-sm text-gray-500 dark:border-white/[0.05]">
          Tổng số {total} cảnh báo
        </div>
        <div className="max-w-full overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Cảnh báo</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Mức độ</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Trạng thái</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Hạn xử lý</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <tr><td colSpan="5" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải cảnh báo...</td></tr>
              ) : items.length ? items.map((item) => {
                const objectUrl = getObjectUrl(item);
                return (
                  <tr key={item.id} className="align-top hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                    <td className="px-5 py-4">
                      <div className="font-medium text-gray-800 dark:text-white/90">{item.tieuDe}</div>
                      <div className="mt-1 max-w-2xl text-sm text-gray-500">{item.noiDung}</div>
                      <div className="mt-2 text-xs text-gray-400">{item.maCanhBao} · {formatDate(item.ngayPhatSinh)}</div>
                    </td>
                    <td className="px-5 py-4 text-center"><Badge size="sm" color={severityColor(item.mucDo)}>{item.mucDo}</Badge></td>
                    <td className="px-5 py-4 text-center"><Badge size="sm" color={statusColor(item.trangThaiXuLy)}>{item.trangThaiXuLy}</Badge></td>
                    <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{formatDate(item.hanXuLy)}</td>
                    <td className="px-5 py-4 text-center">
                      <div className="flex flex-col items-center gap-1">
                        {objectUrl && <button type="button" onClick={() => navigate(objectUrl)} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100">Xem hồ sơ</button>}
                        <button type="button" onClick={() => openReminderModal(item)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50">Nhắc việc</button>
                        {item.trangThaiXuLy === "MOI" && <button type="button" onClick={() => markRead(item)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50">Đã xem</button>}
                        {item.trangThaiXuLy !== "DA_XU_LY" && <button type="button" onClick={() => openCompleteAlertModal(item)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-success-600 hover:bg-success-50">Xử lý</button>}
                      </div>
                    </td>
                  </tr>
                );
              }) : (
                <tr><td colSpan="5" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có cảnh báo phù hợp.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      <Modal isOpen={Boolean(selectedAlert)} onClose={closeReminderModal} className="max-w-3xl p-6">
        <div className="space-y-5">
          <div className="pr-10">
            <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">Nhắc việc cảnh báo</h2>
            <p className="mt-1 text-sm text-gray-500">{selectedAlert?.tieuDe}</p>
          </div>

          <div className="grid gap-4 sm:grid-cols-2">
            <div className="sm:col-span-2">
              <Label>Tiêu đề nhắc việc *</Label>
              <Input value={reminderForm.tieuDe} onChange={(event) => updateReminderForm("tieuDe", event.target.value)} />
            </div>
            <div className="sm:col-span-2">
              <Label>Nội dung *</Label>
              <TextArea rows={4} value={reminderForm.noiDung} onChange={(value) => updateReminderForm("noiDung", value)} />
            </div>
            <div>
              <Label>Người nhận</Label>
              <Select value={reminderForm.nguoiNhanId} onChange={(value) => updateReminderForm("nguoiNhanId", value)} options={userOptions} />
            </div>
            <div>
              <Label>Đơn vị nhận</Label>
              <Select value={reminderForm.donViNhanId} onChange={(value) => updateReminderForm("donViNhanId", value)} options={donViOptions} />
            </div>
            <div>
              <Label>Hạn xử lý</Label>
              <Input type="date" value={reminderForm.hanXuLy} onChange={(event) => updateReminderForm("hanXuLy", event.target.value)} />
            </div>
            <div>
              <Label>Thời gian nhắc</Label>
              <Input type="date" value={reminderForm.thoiGianNhac} onChange={(event) => updateReminderForm("thoiGianNhac", event.target.value)} />
            </div>
            <div>
              <Label>Mức ưu tiên</Label>
              <Select value={reminderForm.mucDoUuTien} onChange={(value) => updateReminderForm("mucDoUuTien", value)} options={priorityOptions} />
            </div>
          </div>

          <div className="flex justify-end gap-3">
            <button type="button" onClick={closeReminderModal} disabled={saving} className="rounded-lg border px-4 py-2 text-sm font-medium">Đóng</button>
            <button type="button" onClick={createReminder} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">
              {saving ? "Đang lưu..." : "Thêm nhắc việc"}
            </button>
          </div>

          <div className="border-t border-gray-100 pt-4 dark:border-white/[0.05]">
            <h3 className="mb-3 text-sm font-semibold text-gray-800 dark:text-white/90">Nhắc việc liên quan</h3>
            {reminderLoading ? (
              <div className="py-6 text-center text-sm text-gray-500">Đang tải nhắc việc...</div>
            ) : reminders.length ? (
              <div className="space-y-3">
                {reminders.map((reminder) => (
                  <div key={reminder.id} className="rounded-2xl border border-gray-100 p-4 dark:border-white/[0.05]">
                    <div className="flex flex-wrap items-start justify-between gap-3">
                      <div>
                        <div className="font-medium text-gray-800 dark:text-white/90">{reminder.tieuDe}</div>
                        <div className="mt-1 text-sm text-gray-500">{reminder.noiDung}</div>
                        <div className="mt-2 text-xs text-gray-400">
                          Người nhận: {userMap.get(reminder.nguoiNhanId) || "-"} · Đơn vị: {donViMap.get(reminder.donViNhanId) || "-"}
                        </div>
                        <div className="mt-1 text-xs text-gray-400">Hạn: {formatDate(reminder.hanXuLy)} · Nhắc: {formatDate(reminder.thoiGianNhac)}</div>
                      </div>
                      <div className="flex items-center gap-2">
                        <Badge size="sm" color={severityColor(reminder.mucDoUuTien)}>{reminder.mucDoUuTien}</Badge>
                        <Badge size="sm" color={statusColor(reminder.trangThai)}>{reminder.trangThai}</Badge>
                      </div>
                    </div>
                    {reminder.trangThai !== "DA_HOAN_THANH" && reminder.trangThai !== "HUY" && (
                      <div className="mt-3 flex justify-end gap-2">
                        <button type="button" onClick={() => cancelReminder(reminder)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-500 hover:bg-gray-100">Hủy</button>
                        <button type="button" onClick={() => openCompleteReminderModal(reminder)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-success-600 hover:bg-success-50">Hoàn thành</button>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            ) : (
              <div className="rounded-2xl border border-dashed border-gray-200 px-4 py-8 text-center text-sm text-gray-500 dark:border-white/[0.08]">
                Chưa có nhắc việc nào cho cảnh báo này.
              </div>
            )}
          </div>

          <div className="border-t border-gray-100 pt-4 dark:border-white/[0.05]">
            <h3 className="mb-3 text-sm font-semibold text-gray-800 dark:text-white/90">Lịch sử xử lý</h3>
            {reminderLoading ? (
              <div className="py-6 text-center text-sm text-gray-500">Đang tải lịch sử...</div>
            ) : histories.length ? (
              <div className="space-y-3">
                {histories.map((history) => (
                  <div key={history.id} className="rounded-2xl border border-gray-100 p-4 dark:border-white/[0.05]">
                    <div className="flex flex-wrap items-start justify-between gap-3">
                      <div>
                        <div className="font-medium text-gray-800 dark:text-white/90">{actionLabel(history.hanhDong)}</div>
                        {history.noiDung && <div className="mt-1 text-sm text-gray-500">{history.noiDung}</div>}
                        <div className="mt-2 text-xs text-gray-400">
                          Người thực hiện: {userMap.get(history.nguoiThucHienId) || history.nguoiThucHienId || "Hệ thống"} · Đơn vị: {donViMap.get(history.donViThucHienId) || "-"}
                        </div>
                      </div>
                      <div className="text-right text-xs text-gray-400">
                        <div>{formatDate(history.thoiGian)}</div>
                        {(history.trangThaiTruoc || history.trangThaiSau) && (
                          <div className="mt-1">{history.trangThaiTruoc || "-"} → {history.trangThaiSau || "-"}</div>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="rounded-2xl border border-dashed border-gray-200 px-4 py-8 text-center text-sm text-gray-500 dark:border-white/[0.08]">
                Chưa có lịch sử xử lý.
              </div>
            )}
          </div>
        </div>
      </Modal>

      <Modal isOpen={Boolean(actionModal.type)} onClose={closeActionModal} className="max-w-xl p-6">
        <div className="space-y-5">
          <div className="pr-10">
            <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">
              {actionModal.type === "ALERT_COMPLETE" ? "Xử lý cảnh báo" : "Hoàn thành nhắc việc"}
            </h2>
            <p className="mt-1 text-sm text-gray-500">
              {actionModal.item?.tieuDe}
            </p>
          </div>

          <div>
            <Label>Ghi chú</Label>
            <TextArea
              rows={4}
              value={actionModal.note}
              onChange={(value) => setActionModal((current) => ({ ...current, note: value }))}
              placeholder="Nhập ghi chú xử lý..."
            />
          </div>

          <div className="flex justify-end gap-3">
            <button type="button" onClick={closeActionModal} disabled={saving} className="rounded-lg border px-4 py-2 text-sm font-medium">Hủy</button>
            <button type="button" onClick={submitActionModal} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">
              {saving ? "Đang lưu..." : "Xác nhận"}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
