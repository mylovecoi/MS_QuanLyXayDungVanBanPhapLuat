import { useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import TextArea from "../../../app/components/forms/input/TextArea";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { createBaoCaoTienDo, getBaoCaoTienDo, getViecDuocGiao, guiBaoCaoTienDo, updateBaoCaoTienDo } from "../api/thiHanhPhapLuatApi";

const emptyForm = { kyBaoCao: "", tyLeHoanThanh: 0, ketQua: "", khoKhan: "", kienNghi: "" };
const errorMessage = (error) => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể xử lý báo cáo.";
const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const currentPeriod = () => `Tháng ${new Date().getMonth() + 1}/${new Date().getFullYear()}`;

export default function BaoCaoTienDoPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { phanCongId } = useParams();
  const selectedReportId = new URLSearchParams(location.search).get("baoCaoId");
  const [assignment, setAssignment] = useState(null);
  const [reports, setReports] = useState([]);
  const [reportStatuses, setReportStatuses] = useState([]);
  const [contentStatuses, setContentStatuses] = useState([]);
  const [activeReportId, setActiveReportId] = useState(selectedReportId || "");
  const [form, setForm] = useState({ ...emptyForm, kyBaoCao: currentPeriod() });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const statusById = useMemo(() => new Map(reportStatuses.map((item) => [item.id, item])), [reportStatuses]);
  const activeReport = useMemo(() => reports.find((item) => item.id === activeReportId) || null, [reports, activeReportId]);
  const activeStatus = activeReport ? statusById.get(activeReport.trangThaiId) : null;
  const editable = !activeReport || ["NHAP", "CAN_BO_SUNG"].includes(activeStatus?.maTrangThai);

  const applyReport = (report) => {
    setActiveReportId(report?.id || "");
    setForm(report ? {
      kyBaoCao: report.kyBaoCao || "",
      tyLeHoanThanh: report.tyLeHoanThanh ?? 0,
      ketQua: report.ketQua || "",
      khoKhan: report.khoKhan || "",
      kienNghi: report.kienNghi || "",
    } : { ...emptyForm, kyBaoCao: currentPeriod() });
  };

  const load = async () => {
    try {
      setLoading(true); setError("");
      const [assignments, reportStatusResponse, contentStatusResponse] = await Promise.all([
        getViecDuocGiao(),
        getTrangThais({ nhomTrangThai: "BAO_CAO_TIEN_DO_THI_HANH", pageSize: 100, pageCurrent: 1 }),
        getTrangThais({ nhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT", pageSize: 100, pageCurrent: 1 }),
      ]);
      const selectedAssignment = assignments.find((item) => item.phanCongId === phanCongId);
      if (!selectedAssignment) { setError("Không tìm thấy đầu việc hoặc bạn không được phân công đầu việc này."); return; }
      const loadedReports = await getBaoCaoTienDo(selectedAssignment.noiDungKeHoachId);
      setAssignment(selectedAssignment);
      setReports(loadedReports);
      setReportStatuses(reportStatusResponse?.data || []);
      setContentStatuses(contentStatusResponse?.data || []);
      const preferred = loadedReports.find((item) => item.id === selectedReportId) || loadedReports.find((item) => item.phanCongThiHanhId === phanCongId && ["NHAP", "CAN_BO_SUNG"].includes((reportStatusResponse?.data || []).find((status) => status.id === item.trangThaiId)?.maTrangThai)) || null;
      applyReport(preferred);
    } catch (requestError) { setError(errorMessage(requestError)); }
    finally { setLoading(false); }
  };

  useEffect(() => { void load(); }, [phanCongId]);

  const update = (field, value) => setForm((current) => ({ ...current, [field]: value }));
  const save = async () => {
    if (!assignment) return;
    if (!form.kyBaoCao.trim()) { setError("Nhập kỳ báo cáo."); return; }
    const progress = Number(form.tyLeHoanThanh);
    if (Number.isNaN(progress) || progress < 0 || progress > 100) { setError("Tỷ lệ hoàn thành phải từ 0 đến 100."); return; }
    try {
      setSaving(true); setError(""); setSuccess("");
      if (activeReport) {
        await updateBaoCaoTienDo(activeReport.id, { tyLeHoanThanh: progress, ketQua: form.ketQua || null, khoKhan: form.khoKhan || null, kienNghi: form.kienNghi || null });
        setSuccess("Đã cập nhật báo cáo nháp.");
      } else {
        const draftStatus = reportStatuses.find((item) => item.maTrangThai === "NHAP" && item.trangThai);
        if (!draftStatus) { setError("Chưa cấu hình trạng thái NHAP cho nhóm BÁO CÁO TIẾN ĐỘ THI HÀNH."); return; }
        const created = await createBaoCaoTienDo({ noiDungKeHoachId: assignment.noiDungKeHoachId, phanCongThiHanhId: assignment.phanCongId, kyBaoCao: form.kyBaoCao.trim(), tyLeHoanThanh: progress, ketQua: form.ketQua || null, khoKhan: form.khoKhan || null, kienNghi: form.kienNghi || null, trangThaiId: draftStatus.id });
        setSuccess("Đã tạo báo cáo nháp.");
        navigate(`/thi-hanh-phap-luat/viec-duoc-giao/${assignment.phanCongId}?baoCaoId=${created.id}`, { replace: true });
        await load();
      }
    } catch (requestError) { setError(errorMessage(requestError)); }
    finally { setSaving(false); }
  };

  const submit = async () => {
    if (!activeReport) { setError("Lưu báo cáo nháp trước khi gửi."); return; }
    const reportStatus = reportStatuses.find((item) => item.maTrangThai === "DA_GUI" && item.trangThai);
    const contentStatus = contentStatuses.find((item) => item.maTrangThai === "CHO_DANH_GIA" && item.trangThai);
    if (!reportStatus || !contentStatus) { setError("Chưa cấu hình trạng thái DA_GUI hoặc CHO_DANH_GIA trong danh mục."); return; }
    try {
      setSaving(true); setError(""); setSuccess("");
      await guiBaoCaoTienDo(activeReport.id, { trangThaiBaoCaoId: reportStatus.id, trangThaiNoiDungId: contentStatus.id, ghiChu: null });
      setSuccess("Đã gửi báo cáo để đơn vị chủ trì đánh giá.");
      await load();
    } catch (requestError) { setError(errorMessage(requestError)); }
    finally { setSaving(false); }
  };

  return <div className="space-y-5">
    <div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Cập nhật báo cáo tiến độ</h1><p className="mt-1 text-sm text-gray-500">{assignment ? `${assignment.maKeHoach} · ${assignment.tenNoiDung}` : "Báo cáo kết quả thực hiện đầu việc được giao."}</p></div><button type="button" onClick={() => navigate("/thi-hanh-phap-luat/viec-duoc-giao")} className="rounded-lg border px-4 py-2 text-sm font-medium">Danh sách việc được giao</button></div>
    {error && <Alert variant="error" title="Có lỗi" message={error} />}{success && <Alert variant="success" title="Hoàn tất" message={success} />}
    {loading ? <div className="py-16 text-center text-sm text-gray-500">Đang tải dữ liệu...</div> : assignment && <div className="space-y-5"><div className="rounded-xl border bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="grid gap-4 sm:grid-cols-3"><div><div className="text-xs text-gray-500">Hạn thực hiện</div><div className="mt-1 font-medium">{formatDate(assignment.hanThucHien)}</div></div><div><div className="text-xs text-gray-500">Tiến độ đầu việc</div><div className="mt-1"><Badge size="sm" color={assignment.tyLeHoanThanh >= 100 ? "success" : "warning"}>{assignment.tyLeHoanThanh ?? 0}%</Badge></div></div><div><div className="text-xs text-gray-500">Trạng thái báo cáo</div><div className="mt-1 font-medium">{activeStatus?.tenTrangThai || activeStatus?.maTrangThai || "Chưa lập"}</div></div></div></div><div className="rounded-xl border bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="mb-5 flex flex-wrap items-center justify-between gap-3"><div><h2 className="font-semibold text-gray-800 dark:text-white/90">Báo cáo theo kỳ</h2><p className="text-sm text-gray-500">Chỉ báo cáo nháp hoặc cần bổ sung mới được chỉnh sửa.</p></div>{reports.length > 0 && <select value={activeReportId} onChange={(event) => applyReport(reports.find((item) => item.id === event.target.value) || null)} className="rounded-lg border px-3 py-2 text-sm"><option value="">Lập báo cáo mới</option>{reports.map((item) => <option key={item.id} value={item.id}>{item.kyBaoCao} · {statusById.get(item.trangThaiId)?.tenTrangThai || "Báo cáo"}</option>)}</select>}</div><div className="grid gap-4 sm:grid-cols-2"><div><Label>Kỳ báo cáo *</Label><Input value={form.kyBaoCao} onChange={(event) => update("kyBaoCao", event.target.value)} disabled={!editable || Boolean(activeReport)} placeholder="Ví dụ: Tháng 10/2026" /></div><div><Label>Tỷ lệ hoàn thành (%) *</Label><Input type="number" min="0" max="100" value={form.tyLeHoanThanh} onChange={(event) => update("tyLeHoanThanh", event.target.value)} disabled={!editable} /></div><div className="sm:col-span-2"><Label>Kết quả thực hiện</Label><TextArea rows={4} value={form.ketQua} onChange={(value) => update("ketQua", value)} disabled={!editable} /></div><div><Label>Khó khăn, vướng mắc</Label><TextArea rows={4} value={form.khoKhan} onChange={(value) => update("khoKhan", value)} disabled={!editable} /></div><div><Label>Kiến nghị</Label><TextArea rows={4} value={form.kienNghi} onChange={(value) => update("kienNghi", value)} disabled={!editable} /></div></div>{editable && <div className="mt-5 flex flex-wrap justify-end gap-3"><button type="button" onClick={save} disabled={saving} className="rounded-lg border px-4 py-2.5 text-sm font-medium disabled:opacity-50">{saving ? "Đang lưu..." : activeReport ? "Lưu cập nhật" : "Tạo báo cáo nháp"}</button>{activeReport && <button type="button" onClick={submit} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">Gửi báo cáo</button>}</div>}</div></div>}
  </div>;
}
