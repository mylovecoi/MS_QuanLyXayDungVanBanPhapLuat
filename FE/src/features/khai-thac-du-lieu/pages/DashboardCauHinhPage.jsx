import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import Input from "../../../app/components/forms/input/InputField.jsx";
import Label from "../../../app/components/forms/Label.jsx";
import { getDashboardLayout, updateDashboardLayout } from "../api/dashboardApi";

const displayOptions = [
  { value: "metric", label: "Thẻ số liệu" },
  { value: "bar", label: "Biểu đồ cột" },
  { value: "line", label: "Biểu đồ đường" },
  { value: "donut", label: "Biểu đồ donut" },
  { value: "table", label: "Bảng" },
  { value: "list", label: "Danh sách" },
];

const sizeOptions = [
  { value: "small", label: "Nhỏ" },
  { value: "medium", label: "Vừa" },
  { value: "large", label: "Rộng" },
  { value: "full", label: "Toàn hàng" },
];

const message = (error) =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  error?.message ||
  "Không thể xử lý cấu hình dashboard.";

function reorder(items, id, direction) {
  const next = [...items].sort((a, b) => a.thuTu - b.thuTu);
  const index = next.findIndex((item) => item.id === id);
  const target = index + direction;
  if (index < 0 || target < 0 || target >= next.length) return items;
  [next[index], next[target]] = [next[target], next[index]];
  return next.map((item, idx) => ({ ...item, thuTu: idx + 1 }));
}

export default function DashboardCauHinhPage() {
  const navigate = useNavigate();
  const [widgets, setWidgets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const data = await getDashboardLayout();
      setWidgets((data.widgets || []).map((item, index) => ({ ...item, thuTu: item.thuTu || index + 1 })).sort((a, b) => a.thuTu - b.thuTu));
    } catch (e) {
      setError(message(e));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, []);

  const update = (id, patch) => {
    setWidgets((current) => current.map((item) => item.id === id ? { ...item, ...patch } : item));
  };

  const save = async () => {
    try {
      setSaving(true);
      setError("");
      setSuccess("");
      const input = widgets.map((item, index) => ({
        widgetId: item.id,
        hienThi: Boolean(item.hienThi),
        thuTu: index + 1,
        dangHienThi: item.dangHienThi || item.loaiWidget || "metric",
        kichThuoc: item.kichThuoc || "medium",
        cauHinhBoLocJson: item.cauHinhBoLocJson || "{}",
        tuDongLamMoiPhut: item.tuDongLamMoiPhut ? Number(item.tuDongLamMoiPhut) : null,
      }));
      const data = await updateDashboardLayout(input);
      setWidgets((data.widgets || []).sort((a, b) => a.thuTu - b.thuTu));
      setSuccess("Đã lưu cấu hình dashboard.");
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
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Cấu hình Dashboard</h1>
          <p className="mt-1 text-sm text-gray-500">Bật/tắt, chọn dạng hiển thị và sắp xếp thứ tự widget trên dashboard.</p>
        </div>
        <div className="flex gap-2">
          <button type="button" onClick={() => navigate("/khai-thac-du-lieu/dashboard")} className="rounded-lg border px-4 py-2 text-sm font-medium">Xem dashboard</button>
          <button type="button" onClick={save} disabled={saving || loading} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">{saving ? "Đang lưu..." : "Lưu cấu hình"}</button>
        </div>
      </div>

      {error && <Alert variant="error" title="Có lỗi" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        {loading ? (
          <div className="p-10 text-center text-sm text-gray-500">Đang tải cấu hình...</div>
        ) : (
          <div className="divide-y">
            {widgets.map((item, index) => (
              <div key={item.id} className="grid gap-4 p-5 lg:grid-cols-12">
                <div className="lg:col-span-4">
                  <div className="flex items-start gap-3">
                    <input type="checkbox" checked={item.hienThi} onChange={(event) => update(item.id, { hienThi: event.target.checked })} className="mt-1 h-4 w-4 rounded border-gray-300" />
                    <div>
                      <div className="font-semibold text-gray-800 dark:text-white/90">{item.tenWidget}</div>
                      <div className="mt-1 text-sm text-gray-500">{item.moTa}</div>
                      <div className="mt-1 text-xs text-gray-400">{item.maWidget}</div>
                    </div>
                  </div>
                </div>
                <div className="lg:col-span-2">
                  <Label>Dạng hiển thị</Label>
                  <select value={item.dangHienThi || "metric"} onChange={(event) => update(item.id, { dangHienThi: event.target.value })} className="h-11 w-full rounded-lg border px-3 text-sm">
                    {displayOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
                  </select>
                </div>
                <div className="lg:col-span-2">
                  <Label>Kích thước</Label>
                  <select value={item.kichThuoc || "medium"} onChange={(event) => update(item.id, { kichThuoc: event.target.value })} className="h-11 w-full rounded-lg border px-3 text-sm">
                    {sizeOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
                  </select>
                </div>
                <div className="lg:col-span-2">
                  <Label>Tự làm mới (phút)</Label>
                  <Input type="number" min="0" value={item.tuDongLamMoiPhut || ""} onChange={(event) => update(item.id, { tuDongLamMoiPhut: event.target.value })} />
                </div>
                <div className="flex items-end justify-end gap-2 lg:col-span-2">
                  <button type="button" disabled={index === 0} onClick={() => setWidgets((current) => reorder(current, item.id, -1))} className="rounded-lg border px-3 py-2 text-sm disabled:opacity-50">Lên</button>
                  <button type="button" disabled={index === widgets.length - 1} onClick={() => setWidgets((current) => reorder(current, item.id, 1))} className="rounded-lg border px-3 py-2 text-sm disabled:opacity-50">Xuống</button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
