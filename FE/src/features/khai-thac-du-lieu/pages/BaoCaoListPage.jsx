import { useEffect, useMemo, useState } from "react";
import { useLocation } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import Badge from "../../../app/components/ui/badge/Badge.jsx";
import { Modal } from "../../../app/components/ui/modal";
import Input from "../../../app/components/forms/input/InputField.jsx";
import Label from "../../../app/components/forms/Label.jsx";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { getDanhMucBaoCao } from "../api/baoCaoApi";

const currentYear = new Date().getFullYear();
const emptyForm = {
  nam: String(currentYear),
  donViId: "",
  trangThaiId: "",
  tuNgay: "",
  denNgay: "",
};

const fallbackReports = [
  {
    maBaoCao: "DANG_KY_BAN_HANH",
    tenBaoCao: "Báo cáo đăng ký ban hành văn bản QPPL",
    moTa: "Tổng hợp hồ sơ đăng ký ban hành văn bản theo đơn vị, loại văn bản, trạng thái và thời gian.",
    nhomBaoCao: "Xây dựng văn bản",
    daCoDuLieu: true,
  },
  {
    maBaoCao: "DANH_MUC_VAN_BAN_QUY_DINH_CHI_TIET",
    tenBaoCao: "Báo cáo danh mục văn bản quy định chi tiết",
    moTa: "Theo dõi danh mục văn bản quy định chi tiết theo loại văn bản, lĩnh vực và trạng thái.",
    nhomBaoCao: "Văn bản pháp luật",
    daCoDuLieu: false,
  },
  {
    maBaoCao: "VAN_BAN_BAN_HANH",
    tenBaoCao: "Báo cáo văn bản do HĐND, UBND ban hành",
    moTa: "Tổng hợp văn bản đã ban hành, phân loại theo hình thức nghị quyết, quyết định và đơn vị.",
    nhomBaoCao: "Xây dựng văn bản",
    daCoDuLieu: true,
  },
  {
    maBaoCao: "THEO_GIAI_DOAN",
    tenBaoCao: "Báo cáo theo giai đoạn",
    moTa: "Tổng hợp hồ sơ, văn bản phát sinh, hoàn thành và chậm tiến độ theo khoảng thời gian.",
    nhomBaoCao: "Tổng hợp",
    daCoDuLieu: true,
  },
  {
    maBaoCao: "THI_HANH_PHAP_LUAT",
    tenBaoCao: "Báo cáo tổ chức thi hành pháp luật",
    moTa: "Tổng hợp kết quả tổ chức thi hành pháp luật theo kế hoạch, nhiệm vụ, đơn vị và tiến độ.",
    nhomBaoCao: "Thi hành pháp luật",
    daCoDuLieu: true,
  },
  {
    maBaoCao: "MUC_DO_HOAN_THANH_DON_VI",
    tenBaoCao: "Báo cáo mức độ hoàn thành nhiệm vụ theo đơn vị",
    moTa: "Đánh giá số lượng nhiệm vụ được giao và tỷ lệ hoàn thành của từng đơn vị.",
    nhomBaoCao: "Thi hành pháp luật",
    daCoDuLieu: true,
  },
];

const message = (error) =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  error?.response?.data ||
  error?.message ||
  "Không thể xử lý báo cáo.";

function itemsOf(response) {
  return response?.data || response?.items || [];
}

function buildReportUrl(report, form, basePath) {
  const params = new URLSearchParams();
  Object.entries(form).forEach(([key, value]) => {
    if (value) params.set(key, value);
  });
  const query = params.toString();
  return `${basePath}/${report.maBaoCao}${query ? `?${query}` : ""}`;
}

export default function BaoCaoListPage() {
  const location = useLocation();
  const [reports, setReports] = useState([]);
  const [donVis, setDonVis] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [selected, setSelected] = useState(null);
  const [form, setForm] = useState(emptyForm);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const grouped = useMemo(() => {
    return reports.reduce((acc, item) => {
      const group = item.nhomBaoCao || "Khác";
      acc[group] = acc[group] || [];
      acc[group].push(item);
      return acc;
    }, {});
  }, [reports]);

  const statusOptions = useMemo(() => statuses.filter((item) => item.trangThai !== false), [statuses]);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const [reportResult, donViResult, statusResult] = await Promise.allSettled([
        getDanhMucBaoCao(),
        getDonViOptions(),
        getTrangThais({ pageSize: 200, pageCurrent: 1 }),
      ]);
      if (reportResult.status === "fulfilled") {
        setReports(reportResult.value?.length ? reportResult.value : fallbackReports);
      } else {
        setReports(fallbackReports);
        setError(`Không tải được danh mục báo cáo từ API. Tạm dùng danh mục mặc định để tiếp tục thao tác. Chi tiết: ${message(reportResult.reason)}`);
      }
      if (donViResult.status === "fulfilled") setDonVis(donViResult.value || []);
      if (statusResult.status === "fulfilled") setStatuses(itemsOf(statusResult.value));
    } catch (requestError) {
      setError(message(requestError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, []);

  const openModal = (report) => {
    setSelected(report);
    setForm(emptyForm);
  };

  const update = (field, value) => setForm((current) => ({ ...current, [field]: value }));

  const viewReport = () => {
    if (!selected) return;
    const basePath = location.pathname.startsWith("/bao-cao-thong-ke")
      ? "/bao-cao-thong-ke"
      : "/khai-thac-du-lieu/bao-cao";
    window.open(buildReportUrl(selected, form, basePath), "_blank", "noopener,noreferrer");
    setSelected(null);
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Báo cáo thống kê</h1>
          <p className="mt-1 text-sm text-gray-500">Chọn báo cáo cần kết xuất, nhập tham số và mở kết quả ở tab riêng để in hoặc xuất dữ liệu.</p>
        </div>
        <button type="button" onClick={load} disabled={loading} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Tải lại</button>
      </div>

      {error && <Alert variant="error" title="Có lỗi" message={error} />}

      {loading ? (
        <div className="rounded-xl border bg-white p-10 text-center text-sm text-gray-500">Đang tải danh mục báo cáo...</div>
      ) : (
        <div className="space-y-5">
          {Object.entries(grouped).map(([group, items]) => (
            <div key={group} className="rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
              <div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]">
                <h2 className="font-semibold text-gray-800 dark:text-white/90">{group}</h2>
              </div>
              <div className="divide-y">
                {items.map((item) => (
                  <div key={item.maBaoCao} className="flex flex-wrap items-center justify-between gap-4 px-5 py-4">
                    <div>
                      <div className="flex flex-wrap items-center gap-2">
                        <h3 className="font-medium text-gray-800 dark:text-white/90">{item.tenBaoCao}</h3>
                        <Badge size="sm" color={item.daCoDuLieu ? "success" : "light"}>{item.daCoDuLieu ? "Đã có dữ liệu" : "Đang bổ sung"}</Badge>
                      </div>
                      <p className="mt-1 text-sm text-gray-500">{item.moTa}</p>
                      <div className="mt-1 text-xs text-gray-400">{item.maBaoCao}</div>
                    </div>
                    <button type="button" onClick={() => openModal(item)} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white">Kết xuất</button>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}

      <Modal isOpen={Boolean(selected)} onClose={() => setSelected(null)} className="max-w-4xl p-6">
        <div className="space-y-5">
          <div>
            <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">Thông tin kết xuất báo cáo</h2>
            <p className="mt-1 text-sm text-gray-500">{selected?.tenBaoCao}</p>
          </div>
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <Label>Năm</Label>
              <Input type="number" value={form.nam} onChange={(event) => update("nam", event.target.value)} />
            </div>
            <div>
              <Label>Đơn vị chủ trì</Label>
              <select value={form.donViId} onChange={(event) => update("donViId", event.target.value)} className="h-11 w-full rounded-lg border px-3 text-sm">
                <option value="">Tất cả đơn vị</option>
                {donVis.map((item) => <option key={item.id} value={item.id}>{item.tenDonVi || item.ten || item.maDonVi}</option>)}
              </select>
            </div>
            <div>
              <Label>Trạng thái</Label>
              <select value={form.trangThaiId} onChange={(event) => update("trangThaiId", event.target.value)} className="h-11 w-full rounded-lg border px-3 text-sm">
                <option value="">Tất cả trạng thái</option>
                {statusOptions.map((item) => <option key={item.id} value={item.id}>{item.tenTrangThai || item.maTrangThai}</option>)}
              </select>
            </div>
            <div>
              <Label>Từ ngày</Label>
              <Input type="date" value={form.tuNgay} onChange={(event) => update("tuNgay", event.target.value)} />
            </div>
            <div>
              <Label>Đến ngày</Label>
              <Input type="date" value={form.denNgay} onChange={(event) => update("denNgay", event.target.value)} />
            </div>
          </div>
          <div className="flex flex-wrap justify-end gap-3">
            <button type="button" onClick={() => setSelected(null)} className="rounded-lg border px-4 py-2 text-sm font-medium">Hủy</button>
            <button type="button" onClick={viewReport} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white">Xem báo cáo</button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
