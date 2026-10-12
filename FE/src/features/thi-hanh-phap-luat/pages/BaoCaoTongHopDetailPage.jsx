import { useEffect, useMemo, useState } from "react";
import { Link, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { getBaoCaoTongHopById, getChiTietBaoCaoTongHop, getNoiDungKeHoach } from "../api/thiHanhPhapLuatApi";

const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const errorMessage = (error) => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể tải báo cáo tổng hợp.";

function parseSummary(value) {
  if (!value) return null;
  try { return JSON.parse(value); } catch { return null; }
}

export default function BaoCaoTongHopDetailPage() {
  const { id } = useParams();
  const [report, setReport] = useState(null);
  const [details, setDetails] = useState([]);
  const [contents, setContents] = useState([]);
  const [reportStatuses, setReportStatuses] = useState([]);
  const [contentStatuses, setContentStatuses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const summary = useMemo(() => parseSummary(report?.soLieuTongHopJson), [report]);
  const contentById = useMemo(() => new Map(contents.map((item) => [item.id, item])), [contents]);
  const contentStatusById = useMemo(() => new Map(contentStatuses.map((item) => [item.id, item])), [contentStatuses]);
  const reportStatus = reportStatuses.find((item) => item.id === report?.trangThaiId);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true); setError("");
        const currentReport = await getBaoCaoTongHopById(id);
        const [detailItems, contentItems, reportStatusResponse, contentStatusResponse] = await Promise.all([
          getChiTietBaoCaoTongHop(id),
          getNoiDungKeHoach(currentReport.keHoachId),
          getTrangThais({ nhomTrangThai: "BAO_CAO_TONG_HOP_THI_HANH", pageSize: 100, pageCurrent: 1 }),
          getTrangThais({ nhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT", pageSize: 100, pageCurrent: 1 }),
        ]);
        setReport(currentReport); setDetails(detailItems); setContents(contentItems); setReportStatuses(reportStatusResponse?.data || []); setContentStatuses(contentStatusResponse?.data || []);
      } catch (requestError) { setError(errorMessage(requestError)); }
      finally { setLoading(false); }
    };
    void load();
  }, [id]);

  if (loading) return <div className="py-16 text-center text-sm text-gray-500">Đang tải báo cáo...</div>;
  if (error) return <div className="space-y-4"><Link to="/thi-hanh-phap-luat/tong-hop" className="text-sm font-medium text-brand-500 hover:underline">← Quay lại danh sách báo cáo</Link><Alert variant="error" title="Có lỗi" message={error} /></div>;
  if (!report) return null;

  return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><Link to="/thi-hanh-phap-luat/tong-hop" className="text-sm font-medium text-brand-500 hover:underline">← Báo cáo tổng hợp</Link><h1 className="mt-2 text-2xl font-semibold text-gray-800 dark:text-white/90">{report.maBaoCao}</h1><p className="mt-1 text-sm text-gray-500">Chi tiết báo cáo tổng hợp thi hành pháp luật.</p></div><Badge size="md" color={reportStatus?.maTrangThai === "DA_CHOT" ? "success" : "warning"}>{reportStatus?.tenTrangThai || reportStatus?.maTrangThai || "-"}</Badge></div><div className="grid gap-4 rounded-xl border border-gray-200 bg-white p-5 sm:grid-cols-2 lg:grid-cols-4 dark:border-white/[0.05] dark:bg-white/[0.03]"><div><p className="text-xs text-gray-500">Kỳ báo cáo</p><p className="mt-1 font-medium text-gray-800 dark:text-white/90">{report.kyBaoCao}</p></div><div><p className="text-xs text-gray-500">Thời gian</p><p className="mt-1 font-medium text-gray-800 dark:text-white/90">{formatDate(report.tuNgay)} - {formatDate(report.denNgay)}</p></div><div><p className="text-xs text-gray-500">Ngày lập</p><p className="mt-1 font-medium text-gray-800 dark:text-white/90">{formatDate(report.createdAt)}</p></div><div><p className="text-xs text-gray-500">Ngày chốt</p><p className="mt-1 font-medium text-gray-800 dark:text-white/90">{formatDate(report.ngayChot)}</p></div></div>{summary ? <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4"><SummaryCard label="Tổng đầu việc" value={summary.tongSoNoiDung} /><SummaryCard label="Đạt" value={summary.dat} color="text-success-600" /><SummaryCard label="Không đạt" value={summary.khongDat} color="text-error-600" /><SummaryCard label="Hoàn thành bình quân" value={`${Number(summary.tyLeHoanThanhBinhQuan || 0).toFixed(1)}%`} color="text-brand-500" /></div> : <Alert variant="info" title="Báo cáo đang ở trạng thái nháp" message="Số liệu tổng hợp sẽ được tạo khi thực hiện chốt báo cáo." />}<div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]"><h2 className="font-semibold text-gray-800 dark:text-white/90">Kết quả theo đầu việc</h2></div><div className="max-w-full overflow-x-auto"><table className="w-full min-w-[900px]"><thead><tr className="border-b border-gray-100 dark:border-white/[0.05]"><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Đầu việc</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Trạng thái</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Hoàn thành</th><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Kết quả / khó khăn / kiến nghị</th></tr></thead><tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">{!details.length ? <tr><td colSpan="4" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có số liệu chi tiết. Báo cáo nháp sẽ có số liệu sau khi chốt.</td></tr> : details.map((item) => { const content = contentById.get(item.noiDungKeHoachId); const status = contentStatusById.get(item.trangThaiId); return <tr key={item.id}><td className="px-5 py-4"><div className="font-medium text-gray-800 dark:text-white/90">{content?.tenNoiDung || "Đầu việc"}</div><div className="mt-1 text-xs text-gray-500">{content?.maNoiDung || item.noiDungKeHoachId}</div></td><td className="px-5 py-4 text-center"><Badge size="sm" color={status?.maTrangThai === "DAT" ? "success" : "error"}>{status?.tenTrangThai || "-"}</Badge></td><td className="px-5 py-4 text-center text-sm font-medium text-gray-800 dark:text-white/90">{item.tyLeHoanThanh}%</td><td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300"><DetailText label="Kết quả" value={item.ketQua} /><DetailText label="Khó khăn" value={item.khoKhan} /><DetailText label="Kiến nghị" value={item.kienNghi} /></td></tr>; })}</tbody></table></div></div></div>;
}

function SummaryCard({ label, value, color = "text-gray-800 dark:text-white/90" }) { return <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]"><p className="text-sm text-gray-500">{label}</p><p className={`mt-2 text-2xl font-semibold ${color}`}>{value ?? 0}</p></div>; }
function DetailText({ label, value }) { return value ? <p className="mb-1"><span className="font-medium text-gray-700 dark:text-gray-200">{label}: </span>{value}</p> : null; }
