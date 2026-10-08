import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getQuyTrinhById } from "../../danh-muc/api/quyTrinhSoanThaoApi";
import { getVanBans } from "../../danh-muc/api/vanBanApi";
import { getHoSoXayDungVanBanById, getHoSoXayDungVanBanTimeline } from "../api/xayDungVanBanApi";

const loaiBoHoSoMap = {
  1: "Soạn thảo",
  2: "Trình thẩm định",
  3: "Thẩm định",
  4: "Trình UBND",
  5: "Ý kiến UBND",
  6: "Trình HĐND thẩm tra",
  7: "Ban hành",
};

const trangThaiBoHoSoMap = {
  1: "Nháp",
  2: "Đã gửi",
  3: "Đang xử lý",
  4: "Hoàn thành",
  5: "Trả lại",
};

function getErrorMessage(error, fallback = "Không thể xử lý yêu cầu.") {
  return error?.response?.data?.message || error?.message || fallback;
}

function formatDate(value) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}

function formatDateTime(value) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("vi-VN", { hour: "2-digit", minute: "2-digit", day: "2-digit", month: "2-digit", year: "numeric" }).format(new Date(value));
}

function InfoItem({ label, value }) {
  return (
    <div>
      <div className="text-xs font-medium uppercase tracking-wide text-gray-500 dark:text-gray-400">{label}</div>
      <div className="mt-1 text-sm font-medium text-gray-800 dark:text-white/90">{value || "-"}</div>
    </div>
  );
}

function getStepStatusMeta(status) {
  if (status === "completed") {
    return {
      label: "Đã hoàn thành",
      badgeColor: "success",
      nodeClass: "border-success-500 bg-success-50 text-success-600 dark:bg-success-500/15 dark:text-success-500",
      lineClass: "bg-success-200 dark:bg-success-500/30",
    };
  }

  if (status === "current") {
    return {
      label: "Đang thực hiện",
      badgeColor: "warning",
      nodeClass: "border-warning-500 bg-warning-50 text-warning-600 dark:bg-warning-500/15 dark:text-warning-500",
      lineClass: "bg-gray-200 dark:bg-white/[0.08]",
    };
  }

  return {
    label: "Chưa thực hiện",
    badgeColor: "light",
    nodeClass: "border-gray-300 bg-gray-50 text-gray-500 dark:border-gray-700 dark:bg-white/[0.04] dark:text-gray-400",
    lineClass: "bg-gray-200 dark:bg-white/[0.08]",
  };
}

export default function HoSoDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [data, setData] = useState(null);
  const [timeline, setTimeline] = useState([]);
  const [vanBans, setVanBans] = useState([]);
  const [quyTrinh, setQuyTrinh] = useState(null);
  const [donVis, setDonVis] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const vanBanMap = useMemo(() => new Map(vanBans.map((item) => [item.id, item.tenLoaiVanBan || item.ten || item.ma])), [vanBans]);
  const donViMap = useMemo(() => new Map(donVis.map((item) => [item.id, item.tenDonVi || item.ten])), [donVis]);
  const stepMap = useMemo(() => new Map((quyTrinh?.buocQuyTrinhs || []).map((step) => [step.id, step.tenBuoc])), [quyTrinh]);
  const workflowSteps = useMemo(() => {
    const steps = [...(quyTrinh?.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0));
    const currentStep = steps.find((step) => step.id === data?.buocHienTaiId);
    const currentOrder = currentStep?.thuTuSapXep || 0;

    return steps.map((step) => {
      const relatedBoHoSos = (data?.boHoSos || [])
        .filter((item) => item.buocQuyTrinhId === step.id)
        .sort((a, b) => new Date(b.ngayHoanThanh || b.ngayGui || b.ngayTao || 0) - new Date(a.ngayHoanThanh || a.ngayGui || a.ngayTao || 0));
      const latestBoHoSo = relatedBoHoSos[0];
      const movedFromStep = timeline.some((item) => item.buocQuyTrinhTruocId === step.id && item.buocQuyTrinhSauId && item.buocQuyTrinhSauId !== step.id);
      const isCurrent = step.id === data?.buocHienTaiId;
      const isCompleted = !isCurrent && (
        latestBoHoSo?.trangThai === 4 ||
        movedFromStep ||
        (currentOrder > 0 && (step.thuTuSapXep || 0) < currentOrder)
      );

      return {
        ...step,
        latestBoHoSo,
        status: isCurrent ? "current" : isCompleted ? "completed" : "pending",
      };
    });
  }, [quyTrinh, data, timeline]);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const detail = await getHoSoXayDungVanBanById(id);
      setData(detail);

      const [timelineResult, vanBanResponse, donViResponse, quyTrinhResult] = await Promise.all([
        getHoSoXayDungVanBanTimeline(id).catch(() => []),
        getVanBans({ pageSize: 100, pageCurrent: 1 }).catch(() => ({ data: [] })),
        getDonViOptions().catch(() => []),
        detail?.quyTrinhSoanThaoId ? getQuyTrinhById(detail.quyTrinhSoanThaoId).catch(() => null) : Promise.resolve(null),
      ]);

      setTimeline(timelineResult || []);
      setVanBans(vanBanResponse?.data || []);
      setDonVis(donViResponse || []);
      setQuyTrinh(quyTrinhResult);
    } catch (loadError) {
      setError(getErrorMessage(loadError, "Không thể tải chi tiết hồ sơ."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, [id]);

  const currentStepName = stepMap.get(data?.buocHienTaiId) || data?.buocHienTaiId;

  return (
    <div className="space-y-5">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <button type="button" onClick={() => navigate("/admin/xay-dung-van-ban/ho-so")} className="mb-3 text-sm font-medium text-brand-500 hover:text-brand-600">
            Quay lại danh sách
          </button>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Chi tiết hồ sơ xây dựng văn bản</h1>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Thông tin hồ sơ, tiến độ xử lý và lịch sử chuyển bước.</p>
        </div>
        {data && <Badge size="sm" color="light">{data.namXayDung}</Badge>}
      </div>

      {error && <Alert variant="error" title="Không thể tải dữ liệu" message={error} />}

      {loading ? (
        <div className="rounded-xl border border-gray-200 bg-white p-10 text-center text-sm text-gray-500 dark:border-white/[0.05] dark:bg-white/[0.03]">Đang tải thông tin hồ sơ...</div>
      ) : data ? (
        <>
          <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="border-b border-gray-100 pb-5 dark:border-white/[0.05]">
              <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">{data.tenHoSo}</h2>
              <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">{data.tenDuThaoVanBan}</p>
              <div className="mt-3 flex flex-wrap gap-2 text-xs text-gray-500">
                <span className="rounded-md bg-gray-100 px-2 py-1 dark:bg-white/[0.06]">{data.maHoSo}</span>
                <span className="rounded-md bg-gray-100 px-2 py-1 dark:bg-white/[0.06]">{currentStepName || "Chưa xác định bước"}</span>
              </div>
            </div>
            <div className="grid gap-5 pt-5 sm:grid-cols-2 lg:grid-cols-4">
              <InfoItem label="Loại văn bản" value={vanBanMap.get(data.danhMucVanBanId) || data.danhMucVanBanId} />
              <InfoItem label="Quy trình" value={quyTrinh?.tenQuyTrinh || data.quyTrinhSoanThaoId} />
              <InfoItem label="Đơn vị chủ trì" value={donViMap.get(data.donViChuTriSoanThaoId) || data.donViChuTriSoanThaoId} />
              <InfoItem label="Ngày tạo" value={formatDate(data.createdAt)} />
              <InfoItem label="Bắt đầu dự kiến" value={formatDate(data.thoiGianDuKienBatDau)} />
              <InfoItem label="Hoàn thành dự kiến" value={formatDate(data.thoiGianDuKienHoanThanh)} />
              <InfoItem label="Năm xây dựng" value={data.namXayDung} />
              <InfoItem label="Mô tả" value={data.moTa} />
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="mb-5">
              <h3 className="font-semibold text-gray-800 dark:text-white/90">Timeline quy trình</h3>
              <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Các bước được hiển thị theo thứ tự cấu hình trong quy trình soạn thảo.</p>
            </div>
            {workflowSteps.length ? (
              <div className="space-y-0">
                {workflowSteps.map((step, index) => {
                  const meta = getStepStatusMeta(step.status);
                  const isLast = index === workflowSteps.length - 1;
                  return (
                    <div key={step.id} className="relative grid grid-cols-[44px_1fr] gap-4">
                      {!isLast && <div className={`absolute left-[21px] top-11 h-full w-0.5 ${meta.lineClass}`} />}
                      <div className={`relative z-10 flex h-11 w-11 items-center justify-center rounded-full border-2 text-sm font-semibold ${meta.nodeClass}`}>
                        {step.status === "completed" ? "✓" : step.thuTuSapXep}
                      </div>
                      <div className={`pb-6 ${isLast ? "pb-0" : ""}`}>
                        <div className="rounded-lg border border-gray-200 p-4 dark:border-white/[0.05]">
                          <div className="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
                            <div>
                              <div className="font-medium text-gray-800 dark:text-white/90">{step.tenBuoc}</div>
                              <div className="mt-1 text-xs text-gray-500">{step.maBuoc} · {step.loaiBuoc || "XuLy"}</div>
                            </div>
                            <Badge size="sm" color={meta.badgeColor}>{meta.label}</Badge>
                          </div>
                          <div className="mt-4 grid gap-3 text-sm sm:grid-cols-3">
                            <InfoItem label="Thời hạn xử lý" value={step.soNgayXuLyTieuChuan ? `${step.soNgayXuLyTieuChuan} ngày` : "-"} />
                            <InfoItem label="Ngày gửi" value={formatDate(step.latestBoHoSo?.ngayGui)} />
                            <InfoItem label="Ngày hoàn thành" value={formatDate(step.latestBoHoSo?.ngayHoanThanh)} />
                          </div>
                          {step.moTa && <div className="mt-3 text-sm text-gray-600 dark:text-gray-300">{step.moTa}</div>}
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            ) : (
              <div className="rounded-lg border border-dashed border-gray-300 px-4 py-8 text-center text-sm text-gray-500 dark:border-gray-700 dark:text-gray-400">
                Chưa tải được cấu hình các bước của quy trình.
              </div>
            )}
          </div>

          <div className="rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]">
              <h3 className="font-semibold text-gray-800 dark:text-white/90">Bộ hồ sơ nghiệp vụ</h3>
            </div>
            <div className="max-w-full overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Loại bộ hồ sơ</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Bước quy trình</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Trạng thái</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Lần xử lý</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Ngày gửi</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Hoàn thành</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                  {(data.boHoSos || []).length ? data.boHoSos.map((item) => (
                    <tr key={item.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                      <td className="px-5 py-4 text-sm font-medium text-gray-800 dark:text-white/90">{loaiBoHoSoMap[item.loaiBoHoSo] || item.loaiBoHoSo}</td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{stepMap.get(item.buocQuyTrinhId) || item.buocQuyTrinhId}</td>
                      <td className="px-5 py-4 text-center"><Badge size="sm" color="light">{trangThaiBoHoSoMap[item.trangThai] || item.trangThai}</Badge></td>
                      <td className="px-5 py-4 text-center text-sm text-gray-500">{item.lanXuLy}</td>
                      <td className="px-5 py-4 text-center text-sm text-gray-500">{formatDate(item.ngayGui)}</td>
                      <td className="px-5 py-4 text-center text-sm text-gray-500">{formatDate(item.ngayHoanThanh)}</td>
                    </tr>
                  )) : (
                    <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có bộ hồ sơ nghiệp vụ.</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]">
              <h3 className="font-semibold text-gray-800 dark:text-white/90">Lịch sử xử lý</h3>
            </div>
            <div className="max-w-full overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Thời gian</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Hành động</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Chuyển bước</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Nội dung</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Lý do</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                  {timeline.length ? timeline.map((item) => (
                    <tr key={item.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                      <td className="px-5 py-4 text-sm text-gray-500">{formatDateTime(item.thoiGianXuLy)}</td>
                      <td className="px-5 py-4 text-sm font-medium text-gray-800 dark:text-white/90">{item.hanhDong}</td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.tenBuocTruoc || "-"} → {item.tenBuocSau || "-"}</td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.noiDung || "-"}</td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.lyDo || "-"}</td>
                    </tr>
                  )) : (
                    <tr><td colSpan="5" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có lịch sử xử lý.</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </>
      ) : null}
    </div>
  );
}
