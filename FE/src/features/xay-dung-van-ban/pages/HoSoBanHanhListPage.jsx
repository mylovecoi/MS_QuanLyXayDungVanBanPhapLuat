import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import { getDanhSachHoSoBanHanh } from "../api/xayDungVanBanApi";

const errorMessage = error => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể tải dữ liệu.";
const dateText = value => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const dateTimeText = value => value ? new Intl.DateTimeFormat("vi-VN", { dateStyle: "short", timeStyle: "short" }).format(new Date(value)) : "-";
const statusInfo = status => status === "DaHoanThanh" ? { label: "Đã ban hành", color: "success" } : status === "Nhap" ? { label: "Chờ cập nhật ban hành", color: "warning" } : { label: status || "-", color: "light" };

export default function HoSoBanHanhListPage() {
  const navigate = useNavigate();
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const load = async () => { try { setLoading(true); setError(""); setItems(await getDanhSachHoSoBanHanh()); } catch (e) { setError(errorMessage(e)); } finally { setLoading(false); } };
  useEffect(() => { void load(); }, []);
  const shown = useMemo(() => {
    const q = search.trim().toLowerCase();
    return q ? items.filter(x => [x.maHoSo, x.tenHoSo, x.tenDuThaoVanBan, x.soVanBan, x.ketQua].some(v => v?.toLowerCase().includes(q))) : items;
  }, [items, search]);

  return <div className="space-y-5">
    <div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Ban hành văn bản</h1><p className="mt-1 text-sm text-gray-500">Cập nhật kết quả ban hành sau khi hồ sơ được UBND đồng ý.</p></div><button onClick={load} className="rounded-lg border px-4 py-2 text-sm">Tải lại</button></div>
    {error && <Alert variant="error" title="Không thể xử lý" message={error} />}
    <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]"><div className="max-w-xl"><Input value={search} onChange={e => setSearch(e.target.value)} placeholder="Tìm mã hồ sơ, tên hồ sơ, số văn bản..." /></div></div><div className="max-w-full overflow-x-auto"><table className="w-full"><thead><tr className="border-b border-gray-100 dark:border-white/[0.05]"><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Hồ sơ</th><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Dự thảo</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Trạng thái</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thời gian</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Kết quả ban hành</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th></tr></thead><tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">{loading ? <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr> : shown.length ? shown.map(item => { const status = statusInfo(item.trangThai); const done = item.trangThai === "DaHoanThanh"; return <tr key={item.hoSoId} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]"><td className="px-5 py-4"><div className="font-medium text-gray-800 dark:text-white/90">{item.tenHoSo}</div><div className="mt-1 text-xs text-gray-500">{item.maHoSo} · Năm {item.namXayDung}</div></td><td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.tenDuThaoVanBan || "-"}</td><td className="px-5 py-4 text-center"><Badge size="sm" color={status.color}>{status.label}</Badge></td><td className="px-5 py-4 text-center text-xs text-gray-500"><div>Hạn: {dateText(item.thoiGianDuKienHoanThanh)}</div><div>Hoàn thành: {dateTimeText(item.ngayHoanThanh)}</div></td><td className="px-5 py-4 text-center text-xs text-gray-500"><div>{item.ketQua || "-"}</div><div>{item.soVanBan || "-"} · {dateText(item.ngayBanHanh)}</div></td><td className="px-5 py-4 text-center">{done ? <button onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${item.hoSoId}`, "_blank", "noopener,noreferrer")} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-gray-100">Xem timeline</button> : <div className="flex flex-wrap justify-center gap-1"><button onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${item.hoSoId}`, "_blank", "noopener,noreferrer")} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500">Xem hồ sơ</button><button onClick={() => navigate(`/xay-dung-van-ban/ban-hanh/${item.hoSoId}`)} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-gray-100">Cập nhật ban hành</button></div>}</td></tr>; }) : <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có hồ sơ ở bước ban hành.</td></tr>}</tbody></table></div></div>
  </div>;
}
