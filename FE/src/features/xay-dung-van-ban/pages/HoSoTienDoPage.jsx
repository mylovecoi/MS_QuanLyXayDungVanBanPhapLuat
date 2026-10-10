import { useEffect, useMemo, useState } from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import {
  getChiTietTienDoXayDungVanBan,
  getNhacNhoTienDo,
  getTienDoXayDungVanBan,
  phanHoiNhacNhoTienDo,
  taoNhacNhoTienDo,
  xacNhanXuLyNhacNhoTienDo,
} from "../api/xayDungVanBanApi";

const ALL_VALUE = "__ALL__";

function message(error) {
  return error?.response?.data?.message || error?.message || "Không thể xử lý yêu cầu.";
}

function formatDate(value) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}

function statusColor(value) {
  const normalized = `${value || ""}`.toUpperCase();
  if (normalized.includes("QUA") || normalized.includes("TRE")) return "error";
  if (normalized.includes("SAP") || normalized.includes("CANH")) return "warning";
  if (normalized.includes("DUNG") || normalized.includes("HOAN")) return "success";
  return "light";
}

export default function HoSoTienDoPage() {
  const [items, setItems] = useState([]);
  const [total, setTotal] = useState(0);
  const [search, setSearch] = useState("");
  const [tinhTrangTienDo, setTinhTrangTienDo] = useState("");
  const [pageCurrent, setPageCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [selected, setSelected] = useState(null);
  const [reminders, setReminders] = useState([]);
  const [reminderText, setReminderText] = useState("");
  const [loading, setLoading] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const totalPages = Math.max(1, Math.ceil(total / pageSize));
  const progressOptions = useMemo(() => [
    { value: ALL_VALUE, label: "Tất cả tình trạng" },
    { value: "DUNG_HAN", label: "Đúng hạn" },
    { value: "SAP_DEN_HAN", label: "Sắp đến hạn" },
    { value: "QUA_HAN", label: "Quá hạn" },
    { value: "DA_HOAN_THANH", label: "Đã hoàn thành" },
  ], []);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const response = await getTienDoXayDungVanBan({
        search: search.trim() || undefined,
        tinhTrangTienDo: tinhTrangTienDo || undefined,
        pageCurrent,
        pageSize,
      });
      setItems(response?.items || []);
      setTotal(response?.totalCount || 0);
    } catch (e) {
      setError(message(e));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => void load(), 250);
    return () => clearTimeout(timer);
  }, [search, tinhTrangTienDo, pageCurrent, pageSize]);

  const openDetail = async (item) => {
    try {
      setDetailLoading(true);
      setError("");
      const [detail, noteList] = await Promise.all([
        getChiTietTienDoXayDungVanBan(item.hoSoId),
        getNhacNhoTienDo(item.hoSoId),
      ]);
      setSelected(detail);
      setReminders(noteList || []);
      setReminderText("");
    } catch (e) {
      setError(message(e));
    } finally {
      setDetailLoading(false);
    }
  };

  const createReminder = async () => {
    if (!selected || !reminderText.trim()) {
      setError("Vui lòng nhập nội dung nhắc tiến độ.");
      return;
    }

    try {
      setSaving(true);
      setError("");
      setSuccess("");
      await taoNhacNhoTienDo(selected.hoSoId, {
        noiDungNhacNho: reminderText.trim(),
        loaiNhacNho: "NHAC_TIEN_DO",
      });
      setReminders(await getNhacNhoTienDo(selected.hoSoId));
      setReminderText("");
      setSuccess("Đã gửi nhắc tiến độ.");
      await load();
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  const replyReminder = async (reminder) => {
    const phanHoi = window.prompt("Nhập phản hồi nhắc tiến độ:", reminder.phanHoi || "");
    if (phanHoi === null) return;

    try {
      setSaving(true);
      await phanHoiNhacNhoTienDo(reminder.hoSoXayDungVanBanId, reminder.id, { phanHoi });
      setReminders(await getNhacNhoTienDo(reminder.hoSoXayDungVanBanId));
      setSuccess("Đã lưu phản hồi.");
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  const confirmReminder = async (reminder) => {
    const ghiChuXuLy = window.prompt("Ghi chú xác nhận xử lý:", reminder.ghiChuXuLy || "");
    if (ghiChuXuLy === null) return;

    try {
      setSaving(true);
      await xacNhanXuLyNhacNhoTienDo(reminder.hoSoXayDungVanBanId, reminder.id, { ghiChuXuLy });
      setReminders(await getNhacNhoTienDo(reminder.hoSoXayDungVanBanId));
      setSuccess("Đã xác nhận xử lý nhắc tiến độ.");
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
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Theo dõi tiến độ xây dựng văn bản</h1>
          <p className="mt-1 text-sm text-gray-500">Theo dõi hạn xử lý, tình trạng tiến độ và nhắc nhở hồ sơ.</p>
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
            <Label>Tình trạng tiến độ</Label>
            <Select value={tinhTrangTienDo || ALL_VALUE} onChange={(value) => { setTinhTrangTienDo(value === ALL_VALUE ? "" : value); setPageCurrent(1); }} options={progressOptions} />
          </div>
        </div>

        <div className="max-w-full overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Hồ sơ</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thời hạn</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Tình trạng</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Còn lại</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Nhắc nhở</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr>
              ) : items.length ? items.map((item) => (
                <tr key={item.hoSoId} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                  <td className="px-5 py-4">
                    <div className="font-medium text-gray-800 dark:text-white/90">{item.tenHoSo}</div>
                    <div className="mt-1 text-xs text-gray-500">{item.maHoSo} · {item.tenDuThaoVanBan}</div>
                  </td>
                  <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">
                    {formatDate(item.thoiGianDuKienBatDau)} - {formatDate(item.thoiGianDuKienHoanThanh)}
                  </td>
                  <td className="px-5 py-4 text-center"><Badge size="sm" color={statusColor(item.tinhTrangTienDo)}>{item.tinhTrangTienDo}</Badge></td>
                  <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{item.soNgayConLai} ngày</td>
                  <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{item.soLanNhacNho}</td>
                  <td className="px-5 py-4 text-center">
                    <button type="button" onClick={() => openDetail(item)} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50">Chi tiết</button>
                  </td>
                </tr>
              )) : (
                <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Không có hồ sơ tiến độ.</td></tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="flex items-center justify-between border-t border-gray-100 px-5 py-4 text-sm text-gray-500 dark:border-white/[0.05]">
          <span>{total} hồ sơ</span>
          <div className="flex items-center gap-2">
            <button type="button" disabled={pageCurrent === 1 || loading} onClick={() => setPageCurrent((value) => value - 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Trước</button>
            <span className="px-3 py-2">{pageCurrent}/{totalPages}</span>
            <button type="button" disabled={pageCurrent >= totalPages || loading} onClick={() => setPageCurrent((value) => value + 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Sau</button>
          </div>
        </div>
      </div>

      {selected && (
        <div className="grid gap-5 lg:grid-cols-[minmax(0,0.9fr)_minmax(0,1.1fr)]">
          <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="mb-4 flex items-start justify-between gap-3">
              <div>
                <h2 className="font-semibold text-gray-800 dark:text-white/90">{selected.tenHoSo}</h2>
                <p className="mt-1 text-sm text-gray-500">{selected.maHoSo} · {selected.tenDuThaoVanBan}</p>
              </div>
              <Badge size="sm" color={statusColor(selected.tinhTrangTienDo)}>{selected.tinhTrangTienDo}</Badge>
            </div>
            {detailLoading ? (
              <div className="py-8 text-center text-sm text-gray-500">Đang tải chi tiết...</div>
            ) : (
              <dl className="grid gap-3 text-sm sm:grid-cols-2">
                <div><dt className="text-gray-500">Bắt đầu dự kiến</dt><dd className="font-medium text-gray-800 dark:text-white/90">{formatDate(selected.thoiGianDuKienBatDau)}</dd></div>
                <div><dt className="text-gray-500">Hoàn thành dự kiến</dt><dd className="font-medium text-gray-800 dark:text-white/90">{formatDate(selected.thoiGianDuKienHoanThanh)}</dd></div>
                <div><dt className="text-gray-500">Số ngày còn lại</dt><dd className="font-medium text-gray-800 dark:text-white/90">{selected.soNgayConLai}</dd></div>
                <div><dt className="text-gray-500">Lần nhắc gần nhất</dt><dd className="font-medium text-gray-800 dark:text-white/90">{formatDate(selected.lanNhacNhoGanNhat)}</dd></div>
              </dl>
            )}
          </div>

          <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="mb-4">
              <h2 className="font-semibold text-gray-800 dark:text-white/90">Nhắc tiến độ</h2>
              <p className="mt-1 text-sm text-gray-500">Gửi nhắc nhở, ghi nhận phản hồi và xác nhận xử lý.</p>
            </div>
            <div className="space-y-3">
              <TextArea rows={3} value={reminderText} onChange={setReminderText} disabled={saving} />
              <div className="flex justify-end">
                <button type="button" onClick={createReminder} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">Gửi nhắc tiến độ</button>
              </div>
            </div>
            <div className="mt-5 divide-y rounded-lg border">
              {reminders.length ? reminders.map((item) => (
                <div key={item.id} className="px-4 py-3 text-sm">
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <div className="font-medium text-gray-800 dark:text-white/90">{item.noiDungNhacNho}</div>
                      <div className="mt-1 text-xs text-gray-500">{formatDate(item.ngayGui)} · {item.trangThaiXuLy}</div>
                    </div>
                    <Badge size="sm" color={item.ngayXacNhanXuLy ? "success" : "warning"}>{item.ngayXacNhanXuLy ? "Đã xử lý" : "Đang theo dõi"}</Badge>
                  </div>
                  {item.phanHoi && <div className="mt-2 rounded-lg bg-gray-50 p-2 text-gray-600 dark:bg-white/[0.04] dark:text-gray-300">Phản hồi: {item.phanHoi}</div>}
                  <div className="mt-3 flex flex-wrap justify-end gap-2">
                    <button type="button" onClick={() => replyReminder(item)} disabled={saving} className="rounded-lg border px-3 py-2 text-xs font-medium disabled:opacity-50">Phản hồi</button>
                    <button type="button" onClick={() => confirmReminder(item)} disabled={saving || Boolean(item.ngayXacNhanXuLy)} className="rounded-lg border px-3 py-2 text-xs font-medium disabled:opacity-50">Xác nhận xử lý</button>
                  </div>
                </div>
              )) : (
                <div className="p-6 text-center text-sm text-gray-500">Chưa có nhắc tiến độ.</div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
