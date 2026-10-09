import { useEffect, useState } from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import TextArea from "../../../app/components/forms/input/TextArea";
import { Modal } from "../../../app/components/ui/modal";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { danhGiaDat, danhGiaKhongDat, getBaoCaoChoDanhGia, yeuCauBoSungBaoCao } from "../api/thiHanhPhapLuatApi";

const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const dateValue = (value) => value ? String(value).slice(0, 10) : "";
const errorMessage = (error) => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể xử lý đánh giá.";
const tomorrow = () => { const value = new Date(); value.setDate(value.getDate() + 1); return dateValue(value.toISOString()); };

export default function DanhGiaBaoCaoPage() {
  const [items, setItems] = useState([]);
  const [reportStatuses, setReportStatuses] = useState([]);
  const [contentStatuses, setContentStatuses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [selected, setSelected] = useState(null);
  const [action, setAction] = useState("");
  const [comment, setComment] = useState("");
  const [deadline, setDeadline] = useState(tomorrow());

  const load = async () => {
    try {
      setLoading(true); setError("");
      const [reports, reportStatusResponse, contentStatusResponse] = await Promise.all([
        getBaoCaoChoDanhGia(),
        getTrangThais({ nhomTrangThai: "BAO_CAO_TIEN_DO_THI_HANH", pageSize: 100, pageCurrent: 1 }),
        getTrangThais({ nhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT", pageSize: 100, pageCurrent: 1 }),
      ]);
      setItems(reports);
      setReportStatuses(reportStatusResponse?.data || []);
      setContentStatuses(contentStatusResponse?.data || []);
    } catch (requestError) { setError(errorMessage(requestError)); }
    finally { setLoading(false); }
  };

  useEffect(() => { void load(); }, []);

  const open = (item, nextAction) => { setSelected(item); setAction(nextAction); setComment(""); setDeadline(tomorrow()); setError(""); };
  const statusId = (itemsToSearch, code) => itemsToSearch.find((item) => item.maTrangThai === code && item.trangThai)?.id;
  const complete = async () => {
    if (!selected) return;
    const reportStatusId = statusId(reportStatuses, action === "bo-sung" ? "CAN_BO_SUNG" : "DA_XAC_NHAN");
    const contentStatusId = statusId(contentStatuses, action === "dat" ? "DAT" : action === "khong-dat" ? "KHONG_DAT" : "YEU_CAU_BO_SUNG");
    if (!reportStatusId || !contentStatusId) { setError("Chưa cấu hình đầy đủ trạng thái đánh giá trong danh mục."); return; }
    if (action === "bo-sung" && !comment.trim()) { setError("Nhập nội dung yêu cầu bổ sung."); return; }
    try {
      setSaving(true); setError("");
      if (action === "dat") await danhGiaDat(selected.baoCaoTienDoId, { trangThaiBaoCaoId: reportStatusId, trangThaiNoiDungId: contentStatusId, nhanXet: comment || null });
      if (action === "khong-dat") await danhGiaKhongDat(selected.baoCaoTienDoId, { trangThaiBaoCaoId: reportStatusId, trangThaiNoiDungId: contentStatusId, nhanXet: comment || null });
      if (action === "bo-sung") await yeuCauBoSungBaoCao(selected.baoCaoTienDoId, { trangThaiBaoCaoId: reportStatusId, trangThaiNoiDungId: contentStatusId, noiDungYeuCau: comment.trim(), hanBoSung: deadline, nhanXet: comment.trim() });
      setSuccess(action === "dat" ? "Đã xác nhận báo cáo đạt." : action === "khong-dat" ? "Đã ghi nhận báo cáo không đạt." : "Đã gửi yêu cầu bổ sung.");
      setSelected(null); await load();
    } catch (requestError) { setError(errorMessage(requestError)); }
    finally { setSaving(false); }
  };

  const actionLabel = action === "dat" ? "Xác nhận đạt" : action === "khong-dat" ? "Xác nhận không đạt" : "Yêu cầu bổ sung";
  return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Đánh giá báo cáo thực hiện</h1><p className="mt-1 text-sm text-gray-500">Các báo cáo đã gửi từ đơn vị thực hiện, chờ đơn vị chủ trì đánh giá.</p></div><button type="button" onClick={load} className="rounded-lg border px-4 py-2 text-sm font-medium">Tải lại</button></div>{error && <Alert variant="error" title="Có lỗi" message={error} />}{success && <Alert variant="success" title="Hoàn tất" message={success} />}<div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="max-w-full overflow-x-auto"><table className="w-full"><thead><tr className="border-b border-gray-100 dark:border-white/[0.05]"><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Kế hoạch / đầu việc</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Kỳ / tiến độ</th><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Kết quả báo cáo</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th></tr></thead><tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">{loading ? <tr><td colSpan="4" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr> : !items.length ? <tr><td colSpan="4" className="px-5 py-10 text-center text-sm text-gray-500">Không có báo cáo nào chờ đánh giá.</td></tr> : items.map((item) => <tr key={item.baoCaoTienDoId} className="align-top hover:bg-gray-50 dark:hover:bg-white/[0.02]"><td className="px-5 py-4"><div className="font-medium text-gray-800 dark:text-white/90">{item.tenNoiDung}</div><div className="mt-1 text-xs text-gray-500">{item.maNoiDung} · {item.maKeHoach} · {item.tenKeHoach}</div><div className="mt-1 text-xs text-gray-500">Hạn: {formatDate(item.hanHoanThanh)}</div></td><td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300"><div>{item.kyBaoCao}</div><div className="mt-1 font-medium text-brand-500">{item.tyLeHoanThanh}%</div><div className="mt-1 text-xs text-gray-500">Gửi: {formatDate(item.ngayBaoCao)}</div></td><td className="max-w-md px-5 py-4 text-sm text-gray-600 dark:text-gray-300"><div>{item.ketQua || "Chưa nêu kết quả."}</div>{item.khoKhan && <div className="mt-2 text-xs text-warning-600">Vướng mắc: {item.khoKhan}</div>}{item.kienNghi && <div className="mt-1 text-xs text-gray-500">Kiến nghị: {item.kienNghi}</div>}</td><td className="px-5 py-4"><div className="flex flex-col gap-1"><button type="button" onClick={() => open(item, "dat")} className="rounded-lg px-3 py-2 text-xs font-medium text-success-600 hover:bg-success-50">Đạt</button><button type="button" onClick={() => open(item, "bo-sung")} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50">Yêu cầu bổ sung</button><button type="button" onClick={() => open(item, "khong-dat")} className="rounded-lg px-3 py-2 text-xs font-medium text-error-500 hover:bg-error-50">Không đạt</button></div></td></tr>)}</tbody></table></div></div><Modal isOpen={Boolean(selected)} onClose={() => !saving && setSelected(null)} className="max-w-lg p-6"><div className="space-y-4"><div><h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">{actionLabel}</h2><p className="mt-1 text-sm text-gray-500">{selected?.tenNoiDung} · {selected?.kyBaoCao}</p></div>{action === "bo-sung" && <div><Label>Hạn bổ sung *</Label><Input type="date" min={dateValue(new Date().toISOString())} value={deadline} onChange={(event) => setDeadline(event.target.value)} /></div>}<div><Label>{action === "bo-sung" ? "Nội dung yêu cầu bổ sung *" : "Nhận xét"}</Label><TextArea rows={4} value={comment} onChange={setComment} /></div>{error && <Alert variant="error" title="Không thể thực hiện" message={error} />}<div className="flex justify-end gap-3"><button type="button" disabled={saving} onClick={() => setSelected(null)} className="rounded-lg border px-4 py-2 text-sm font-medium">Hủy</button><button type="button" disabled={saving} onClick={complete} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">{saving ? "Đang xử lý..." : actionLabel}</button></div></div></Modal></div>;
}
