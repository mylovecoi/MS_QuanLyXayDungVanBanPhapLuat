import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import { Modal } from "../../../app/components/ui/modal";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getQuyTrinhById, getQuyTrinhs } from "../../danh-muc/api/quyTrinhSoanThaoApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { getVanBans } from "../../danh-muc/api/vanBanApi";
import { deleteHoSoSoanThao, getHoSoXayDungVanBans, kiemTraTruocTrinhThamDinh, trinhThamDinh } from "../api/xayDungVanBanApi";

const ALL_VALUE = "__ALL__";
const HO_SO_STATUS_GROUP = "HO_SO_XAY_DUNG_VAN_BAN";
const DEFAULT_PROCESSING_DAYS = 5;
const EDITABLE_STATUS_CODES = new Set(["NHAP", "TRA_LAI", "BI_TRA_LAI", "DANG_XU_LY"]);
const emptyTransfer = { buocQuyTrinhTiepTheoId: "", trangThaiHoSoTiepTheoId: "", donViNhanThamDinhId: "", ngayChuyen: "", soNgayXuLy: DEFAULT_PROCESSING_DAYS, hanDeNghiTraKetQua: "", soNgayCanhBao: 0, thoiGianCanhBao: "", noiDungGhiChu: "" };

function getErrorMessage(error, fallback = "Không thể xử lý yêu cầu.") {
  return error?.response?.data?.message || error?.message || fallback;
}

function formatDate(value) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}

function toDateInput(value) {
  const date = value ? new Date(value) : new Date();
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function addDaysToDateInput(startDate, days) {
  const date = startDate ? new Date(startDate) : new Date();
  date.setDate(date.getDate() + (Number(days) || DEFAULT_PROCESSING_DAYS));
  return toDateInput(date);
}

function subtractDaysToDateInput(startDate, days) {
  const date = startDate ? new Date(startDate) : new Date();
  date.setDate(date.getDate() - Math.max(0, Number(days) || 0));
  return toDateInput(date);
}

function clampWarningDays(value, processingDays) {
  const warningDays = Math.max(0, Number(value) || 0);
  const maxDays = Math.max(1, Number(processingDays) || DEFAULT_PROCESSING_DAYS);
  return Math.min(warningDays, maxDays);
}

function canNhapYKien(item) {
  return Boolean(item?.id && item?.quyTrinhSoanThaoId && item?.buocHienTaiId);
}

function hasYKien(item) {
  return Number(item?.soYKienDonVi || 0) > 0;
}

function canModifyByStatus(status) {
  const code = `${status?.maTrangThai || ""}`.toUpperCase();
  const name = `${status?.tenTrangThai || ""}`.toLowerCase();
  return EDITABLE_STATUS_CODES.has(code) || name.includes("nháp") || name.includes("trả lại");
}

function Field({ label, required, children }) {
  return (
    <div>
      <Label>
        {label}
        {required && <span className="text-error-500"> *</span>}
      </Label>
      <div className="mt-1.5">{children}</div>
    </div>
  );
}

function PaginationFooter({ pageCurrent, pageSize, totalCount, loading, onPageChange }) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const pages = useMemo(() => {
    const start = Math.max(1, Math.min(pageCurrent - 2, totalPages - 4));
    const end = Math.min(totalPages, start + 4);
    return Array.from({ length: end - start + 1 }, (_, index) => start + index);
  }, [pageCurrent, totalPages]);

  return (
    <div className="flex flex-col gap-4 border-t border-gray-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-white/[0.05]">
      <div className="text-sm text-gray-500 dark:text-gray-400">
        Hiển thị {totalCount === 0 ? 0 : (pageCurrent - 1) * pageSize + 1} đến {Math.min(pageCurrent * pageSize, totalCount)} trong {totalCount} hồ sơ
      </div>
      <div className="flex items-center gap-2">
        <button type="button" onClick={() => onPageChange(Math.max(1, pageCurrent - 1))} disabled={pageCurrent === 1 || loading} className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 disabled:opacity-50 dark:border-gray-700 dark:text-gray-300">Trước</button>
        <div className="flex items-center gap-1">
          {pages.map((page) => (
            <button key={page} type="button" onClick={() => onPageChange(page)} disabled={loading} className={`h-9 min-w-9 rounded-lg px-3 text-sm font-medium disabled:opacity-50 ${pageCurrent === page ? "bg-brand-500 text-white" : "text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/[0.05]"}`}>
              {page}
            </button>
          ))}
        </div>
        <button type="button" onClick={() => onPageChange(Math.min(totalPages, pageCurrent + 1))} disabled={pageCurrent >= totalPages || loading} className="rounded-lg border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 disabled:opacity-50 dark:border-gray-700 dark:text-gray-300">Sau</button>
      </div>
    </div>
  );
}

export default function HoSoListPage() {
  const navigate = useNavigate();
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState("");
  const [pageSize, setPageSize] = useState(10);
  const [pageCurrent, setPageCurrent] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [filters, setFilters] = useState({ danhMucVanBanId: "", quyTrinhSoanThaoId: "", donViChuTriSoanThaoId: "", namXayDung: "" });
  const [vanBans, setVanBans] = useState([]);
  const [quyTrinhs, setQuyTrinhs] = useState([]);
  const [donVis, setDonVis] = useState([]);
  const [trangThais, setTrangThais] = useState([]);
  const [transferItem, setTransferItem] = useState(null);
  const [transferWorkflow, setTransferWorkflow] = useState(null);
  const [transfer, setTransfer] = useState(emptyTransfer);
  const [checking, setChecking] = useState(null);
  const [loading, setLoading] = useState(false);
  const [transferLoading, setTransferLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const vanBanMap = useMemo(() => new Map(vanBans.map((item) => [item.id, item.tenLoaiVanBan || item.ten || item.ma])), [vanBans]);
  const quyTrinhMap = useMemo(() => new Map(quyTrinhs.map((item) => [item.id, item.tenQuyTrinh || item.ten])), [quyTrinhs]);
  const donViMap = useMemo(() => new Map(donVis.map((item) => [item.id, item.tenDonVi || item.ten])), [donVis]);
  const trangThaiMap = useMemo(() => new Map(trangThais.map((item) => [item.id, item])), [trangThais]);
  const transferSteps = useMemo(() => [...(transferWorkflow?.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0)), [transferWorkflow]);
  const transferStepMap = useMemo(() => new Map(transferSteps.map((item) => [item.id, item])), [transferSteps]);
  const nextStepOptions = useMemo(() => {
    if (!transferItem) return [];
    const transitions = transferWorkflow?.chuyenBuocs || [];
    const directSteps = transitions
      .filter((item) => item.tuBuocId === transferItem.buocHienTaiId && !item.isKetThuc && item.loaiChuyenBuoc !== "Return" && item.loaiChuyenBuoc !== "Reject")
      .sort((a, b) => Number(b.laNhanhMacDinh) - Number(a.laNhanhMacDinh))
      .map((item) => transferStepMap.get(item.denBuocId))
      .filter(Boolean);
    const currentOrder = transferStepMap.get(transferItem.buocHienTaiId)?.thuTuSapXep || 0;
    const fallbackSteps = transferSteps.filter((item) => (item.thuTuSapXep || 0) > currentOrder);
    const source = directSteps.length ? directSteps : fallbackSteps;
    return source.map((item) => ({ value: item.id, label: `${item.thuTuSapXep}. ${item.tenBuoc}` }));
  }, [transferWorkflow, transferItem, transferStepMap, transferSteps]);
  const statusOptions = useMemo(() => trangThais.map((item) => ({ value: item.id, label: `${item.tenTrangThai || item.maTrangThai} (${item.maTrangThai})` })), [trangThais]);
  const donViOptions = useMemo(() => donVis.map((item) => ({ value: item.id, label: item.tenDonVi || item.ten || item.maDonVi })), [donVis]);
  const loadLookups = async () => {
    try {
      const [vanBanResponse, quyTrinhResponse, donViResponse, trangThaiResponse] = await Promise.all([
        getVanBans({ pageSize: 100, pageCurrent: 1 }),
        getQuyTrinhs({ pageSize: 100, pageCurrent: 1 }),
        getDonViOptions(),
        getTrangThais({ nhomTrangThai: HO_SO_STATUS_GROUP, pageSize: 100, pageCurrent: 1 }),
      ]);
      setVanBans(vanBanResponse?.data || []);
      setQuyTrinhs(quyTrinhResponse?.data || []);
      setDonVis(donViResponse || []);
      setTrangThais(trangThaiResponse?.data || []);
    } catch (lookupError) {
      setError(getErrorMessage(lookupError, "Không thể tải dữ liệu danh mục."));
    }
  };

  const loadItems = async () => {
    try {
      setLoading(true);
      setError("");
      const response = await getHoSoXayDungVanBans({
        search,
        pageSize,
        pageCurrent,
        danhMucVanBanId: filters.danhMucVanBanId,
        quyTrinhSoanThaoId: filters.quyTrinhSoanThaoId,
        donViChuTriSoanThaoId: filters.donViChuTriSoanThaoId,
        namXayDung: filters.namXayDung,
      });
      setItems(response?.items || []);
      setTotalCount(response?.totalCount || 0);
    } catch (loadError) {
      setError(getErrorMessage(loadError, "Không thể tải danh sách hồ sơ."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadLookups();
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => void loadItems(), 250);
    return () => clearTimeout(timer);
  }, [search, pageSize, pageCurrent, filters]);

  const updateFilter = (key, value) => {
    setFilters((current) => ({ ...current, [key]: value === ALL_VALUE ? "" : value }));
    setPageCurrent(1);
  };

  const closeTransferModal = () => {
    if (transferLoading) return;
    setTransferItem(null);
    setTransferWorkflow(null);
    setTransfer(emptyTransfer);
    setChecking(null);
  };

  const openTransferModal = async (item) => {
    try {
      setTransferLoading(true);
      setError("");
      setSuccess("");
      setTransferItem(item);
      setChecking(null);
      const workflow = await getQuyTrinhById(item.quyTrinhSoanThaoId);
      const steps = [...(workflow?.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0));
      const stepMap = new Map(steps.map((step) => [step.id, step]));
      const transitions = workflow?.chuyenBuocs || [];
      const directSteps = transitions
        .filter((transition) => transition.tuBuocId === item.buocHienTaiId && !transition.isKetThuc && transition.loaiChuyenBuoc !== "Return" && transition.loaiChuyenBuoc !== "Reject")
        .sort((a, b) => Number(b.laNhanhMacDinh) - Number(a.laNhanhMacDinh))
        .map((transition) => stepMap.get(transition.denBuocId))
        .filter(Boolean);
      const currentOrder = stepMap.get(item.buocHienTaiId)?.thuTuSapXep || 0;
      const fallbackSteps = steps.filter((step) => (step.thuTuSapXep || 0) > currentOrder);
      const nextStep = (directSteps.length ? directSteps : fallbackSteps)[0];
      const defaultStatus =
        trangThais.find((status) => status.maTrangThai === "CHO_THAM_DINH" || status.maTrangThai === "DANG_THAM_DINH") ||
        trangThais.find((status) => status.id !== item.trangThaiHoSoId) ||
        trangThais[0];
      const ngayChuyen = toDateInput();
      const soNgayXuLy = nextStep?.soNgayXuLyTieuChuan || DEFAULT_PROCESSING_DAYS;
      const soNgayCanhBao = clampWarningDays(nextStep?.soNgayCanhBaoSapHan ?? 0, soNgayXuLy);
      const hanDeNghiTraKetQua = addDaysToDateInput(ngayChuyen, soNgayXuLy);
      setTransferWorkflow(workflow);
      setTransfer({
        buocQuyTrinhTiepTheoId: nextStep?.id || "",
        trangThaiHoSoTiepTheoId: defaultStatus?.id || "",
        donViNhanThamDinhId: nextStep?.donViTiepNhanMacDinhId || "",
        ngayChuyen,
        soNgayXuLy,
        hanDeNghiTraKetQua,
        soNgayCanhBao,
        thoiGianCanhBao: subtractDaysToDateInput(hanDeNghiTraKetQua, soNgayCanhBao),
        noiDungGhiChu: "",
      });
    } catch (openError) {
      setTransferItem(null);
      setError(getErrorMessage(openError, "Không thể tải cấu hình chuyển bước."));
    } finally {
      setTransferLoading(false);
    }
  };

  const checkBeforeTransfer = async () => {
    if (!transferItem) return null;
    try {
      setError("");
      const result = await kiemTraTruocTrinhThamDinh(transferItem.id);
      setChecking(result);
      return result;
    } catch (checkError) {
      setError(getErrorMessage(checkError, "Không thể kiểm tra điều kiện chuyển bước."));
      return null;
    }
  };

  const submitTransfer = async () => {
    if (!transferItem) return;
    if (!transfer.buocQuyTrinhTiepTheoId || !transfer.trangThaiHoSoTiepTheoId || !transfer.donViNhanThamDinhId) {
      setError("Vui lòng chọn bước tiếp theo, trạng thái và đơn vị tiếp nhận.");
      return;
    }

    try {
      setTransferLoading(true);
      setError("");
      setSuccess("");
      const checkResult = await checkBeforeTransfer();
      if (checkResult && checkResult.dat === false) {
        setError("Hồ sơ chưa đủ điều kiện chuyển bước. Vui lòng kiểm tra danh sách điều kiện trong cửa sổ chuyển bước.");
        return;
      }
      await trinhThamDinh(transferItem.id, transfer);
      setTransferItem(null);
      setTransferWorkflow(null);
      setTransfer(emptyTransfer);
      setChecking(null);
      setSuccess("Đã chuyển hồ sơ sang bước tiếp theo.");
      await loadItems();
    } catch (sendError) {
      setError(getErrorMessage(sendError, "Không thể chuyển hồ sơ sang bước tiếp theo."));
    } finally {
      setTransferLoading(false);
    }
  };

  const removeHoSo = async (item) => {
    if (!window.confirm(`Xóa hồ sơ "${item.tenHoSo}"?`)) return;

    try {
      setLoading(true);
      setError("");
      setSuccess("");
      await deleteHoSoSoanThao(item.id);
      setSuccess("Đã xóa hồ sơ.");
      await loadItems();
    } catch (deleteError) {
      setError(getErrorMessage(deleteError, "Không thể xóa hồ sơ."));
    } finally {
      setLoading(false);
    }
  };

  const currentYear = new Date().getFullYear();
  const yearOptions = [{ value: ALL_VALUE, label: "Tất cả năm" }, ...Array.from({ length: 8 }, (_, index) => currentYear - index).map((year) => ({ value: year, label: String(year) }))];

  return (
    <div className="space-y-5">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Hồ sơ xây dựng văn bản</h1>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Quản lý hồ sơ xây dựng Quyết định UBND tỉnh và Nghị quyết HĐND tỉnh.</p>
        </div>
        <button type="button" onClick={() => navigate("/admin/xay-dung-van-ban/ho-so/them-moi")} className="inline-flex items-center justify-center rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-brand-600">
          + Thêm hồ sơ
        </button>
      </div>

      {error && <Alert variant="error" title="Không thể xử lý" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="grid gap-4 border-b border-gray-100 px-5 py-4 sm:grid-cols-12 dark:border-white/[0.05]">
          <div className="sm:col-span-2">
            <Label>Hiển thị</Label>
            <Select value={pageSize} onChange={(value) => { setPageSize(Number(value)); setPageCurrent(1); }} options={[10, 20, 50, 100].map((value) => ({ value, label: `${value} hồ sơ` }))} />
          </div>
          <div className="sm:col-span-4">
            <Label>Tìm kiếm</Label>
            <Input value={search} onChange={(event) => { setSearch(event.target.value); setPageCurrent(1); }} placeholder="Tìm mã hồ sơ, tên hồ sơ, tên dự thảo..." />
          </div>
          <div className="sm:col-span-3">
            <Label>Đơn vị chủ trì soạn thảo</Label>
            <Select value={filters.donViChuTriSoanThaoId || ALL_VALUE} onChange={(value) => updateFilter("donViChuTriSoanThaoId", value)} options={[{ value: ALL_VALUE, label: "Tất cả" }, ...donVis.map((item) => ({ value: item.id, label: item.tenDonVi || item.ten || item.maDonVi }))]} />
          </div>
          <div className="sm:col-span-3">
            <Label>Loại văn bản</Label>
            <Select value={filters.danhMucVanBanId || ALL_VALUE} onChange={(value) => updateFilter("danhMucVanBanId", value)} options={[{ value: ALL_VALUE, label: "Tất cả" }, ...vanBans.map((item) => ({ value: item.id, label: item.tenLoaiVanBan || item.ten || item.ma }))]} />
          </div>
          <div className="sm:col-span-2">
            <Label>Quy trình</Label>
            <Select value={filters.quyTrinhSoanThaoId || ALL_VALUE} onChange={(value) => updateFilter("quyTrinhSoanThaoId", value)} options={[{ value: ALL_VALUE, label: "Tất cả" }, ...quyTrinhs.map((item) => ({ value: item.id, label: item.tenQuyTrinh || item.maQuyTrinh }))]} />
          </div>
          <div className="sm:col-span-2">
            <Label>Năm</Label>
            <Select value={filters.namXayDung || ALL_VALUE} onChange={(value) => updateFilter("namXayDung", value)} options={yearOptions} />
          </div>
        </div>

        <div className="max-w-full overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Hồ sơ</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Loại / quy trình</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Đơn vị chủ trì</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Trạng thái hồ sơ</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Năm</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Ngày tạo</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <tr><td colSpan="7" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr>
              ) : items.length ? items.map((item) => {
                const trangThai = trangThaiMap.get(item.trangThaiHoSoId);
                const canModify = canModifyByStatus(trangThai);
                return (
                  <tr key={item.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                    <td className="px-5 py-4">
                      <div className="font-medium text-gray-800 dark:text-white/90">{item.tenHoSo}</div>
                      <div className="mt-1 text-xs text-gray-500">{item.maHoSo} · {item.tenDuThaoVanBan}</div>
                    </td>
                    <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">
                      <div>{vanBanMap.get(item.danhMucVanBanId) || item.danhMucVanBanId}</div>
                      <div className="mt-1 text-xs text-gray-500">{quyTrinhMap.get(item.quyTrinhSoanThaoId) || item.quyTrinhSoanThaoId}</div>
                    </td>
                    <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{donViMap.get(item.donViChuTriSoanThaoId) || item.donViChuTriSoanThaoId}</td>
                    <td className="px-5 py-4 text-center">
                      <Badge size="sm" color={canModify ? "warning" : "light"}>{trangThai?.tenTrangThai || trangThai?.maTrangThai || item.trangThaiHoSoId || "-"}</Badge>
                      {trangThai?.maTrangThai && <div className="mt-1 text-xs text-gray-500">{trangThai.maTrangThai}</div>}
                    </td>
                    <td className="px-5 py-4 text-center"><Badge size="sm" color="light">{item.namXayDung}</Badge></td>
                    <td className="px-5 py-4 text-center text-sm text-gray-500">{formatDate(item.createdAt)}</td>
                    <td className="px-5 py-4">
                      <div className="flex flex-wrap justify-center gap-1">
                        <button type="button" onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${item.id}`, "_blank", "noopener,noreferrer")} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500">Xem chi tiết</button>
                        {canModify && canNhapYKien(item) && (
                          <button type="button" onClick={() => navigate(`/admin/xay-dung-van-ban/ho-so/${item.id}/y-kien-dong-gop`)} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500">Nhập ý kiến</button>
                        )}
                        {canModify && hasYKien(item) && (
                          <button type="button" onClick={() => navigate(`/admin/xay-dung-van-ban/ho-so/${item.id}/trinh-tham-dinh`)} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500">Trình thẩm định</button>
                        )}
                        {canModify && hasYKien(item) && (
                          <button type="button" onClick={() => openTransferModal(item)} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500">Chuyển bước</button>
                        )}
                        {canModify && (
                          <>
                            <button type="button" onClick={() => navigate(`/admin/xay-dung-van-ban/ho-so/${item.id}/chinh-sua`)} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500">Sửa</button>
                            <button type="button" onClick={() => removeHoSo(item)} className="rounded-lg px-3 py-2 text-xs font-medium text-error-600 hover:bg-error-50">Xóa</button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                );
              }) : (
                <tr><td colSpan="7" className="px-5 py-10 text-center text-sm text-gray-500">Không có hồ sơ xây dựng văn bản.</td></tr>
              )}
            </tbody>
          </table>
        </div>

        <PaginationFooter pageCurrent={pageCurrent} pageSize={pageSize} totalCount={totalCount} loading={loading} onPageChange={setPageCurrent} />
      </div>

      <Modal isOpen={Boolean(transferItem)} onClose={closeTransferModal} className="max-w-5xl p-6">
        <div className="space-y-5">
          <div>
            <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">Chuyển hồ sơ sang bước tiếp theo</h2>
            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">{transferItem?.tenHoSo || "Chọn bước, trạng thái và đơn vị nhận để chuyển hồ sơ."}</p>
          </div>
          <div className="grid gap-5 lg:grid-cols-3">
            <Field label="Bước tiếp theo" required>
              <Select
                value={transfer.buocQuyTrinhTiepTheoId}
                placeholder="Chọn bước"
                options={nextStepOptions}
                onChange={(value) => {
                  const step = transferStepMap.get(value);
                  const soNgayXuLy = step?.soNgayXuLyTieuChuan || DEFAULT_PROCESSING_DAYS;
                  const soNgayCanhBao = clampWarningDays(step?.soNgayCanhBaoSapHan ?? 0, soNgayXuLy);
                  setTransfer((current) => {
                    const hanDeNghiTraKetQua = addDaysToDateInput(current.ngayChuyen, soNgayXuLy);
                    return {
                      ...current,
                      buocQuyTrinhTiepTheoId: value,
                      donViNhanThamDinhId: step?.donViTiepNhanMacDinhId || "",
                      soNgayXuLy,
                      hanDeNghiTraKetQua,
                      soNgayCanhBao,
                      thoiGianCanhBao: subtractDaysToDateInput(hanDeNghiTraKetQua, soNgayCanhBao),
                    };
                  });
                }}
              />
            </Field>
            <Field label="Trạng thái sau chuyển" required>
              <Select
                value={transfer.trangThaiHoSoTiepTheoId}
                placeholder="Chọn trạng thái"
                options={statusOptions}
                onChange={(value) => setTransfer((current) => ({ ...current, trangThaiHoSoTiepTheoId: value }))}
              />
            </Field>
            <Field label="Đơn vị tiếp nhận" required>
              <Select
                value={transfer.donViNhanThamDinhId}
                placeholder="Chọn đơn vị"
                options={donViOptions}
                onChange={(value) => setTransfer((current) => ({ ...current, donViNhanThamDinhId: value }))}
              />
            </Field>
            <Field label="Ngày chuyển">
              <Input
                type="date"
                value={transfer.ngayChuyen}
                onChange={(event) => {
                  const ngayChuyen = event.target.value;
                  setTransfer((current) => ({
                    ...current,
                    ngayChuyen,
                    hanDeNghiTraKetQua: addDaysToDateInput(ngayChuyen, current.soNgayXuLy),
                    thoiGianCanhBao: subtractDaysToDateInput(addDaysToDateInput(ngayChuyen, current.soNgayXuLy), current.soNgayCanhBao),
                  }));
                }}
                disabled={transferLoading}
              />
            </Field>
            <Field label="Thời hạn xử lý">
              <Input
                type="date"
                value={transfer.hanDeNghiTraKetQua}
                onChange={(event) => setTransfer((current) => ({ ...current, hanDeNghiTraKetQua: event.target.value, thoiGianCanhBao: subtractDaysToDateInput(event.target.value, current.soNgayCanhBao) }))}
                disabled={transferLoading}
              />
            </Field>
            <Field label="Số ngày xử lý">
              <Input
                type="number"
                min="1"
                value={transfer.soNgayXuLy}
                onChange={(event) => {
                  const soNgayXuLy = Number(event.target.value) || DEFAULT_PROCESSING_DAYS;
                  setTransfer((current) => {
                    const soNgayCanhBao = clampWarningDays(current.soNgayCanhBao, soNgayXuLy);
                    const hanDeNghiTraKetQua = addDaysToDateInput(current.ngayChuyen, soNgayXuLy);
                    return {
                      ...current,
                      soNgayXuLy,
                      soNgayCanhBao,
                      hanDeNghiTraKetQua,
                      thoiGianCanhBao: subtractDaysToDateInput(hanDeNghiTraKetQua, soNgayCanhBao),
                    };
                  });
                }}
                disabled={transferLoading}
              />
            </Field>
            <Field label="Số ngày cảnh báo">
              <Input
                type="number"
                min="0"
                max={transfer.soNgayXuLy || DEFAULT_PROCESSING_DAYS}
                value={transfer.soNgayCanhBao}
                onChange={(event) => {
                  const soNgayCanhBao = clampWarningDays(event.target.value, transfer.soNgayXuLy);
                  setTransfer((current) => ({
                    ...current,
                    soNgayCanhBao,
                    thoiGianCanhBao: subtractDaysToDateInput(current.hanDeNghiTraKetQua, soNgayCanhBao),
                  }));
                }}
                disabled={transferLoading}
              />
            </Field>
            <Field label="Thời gian cảnh báo">
              <Input
                type="date"
                value={transfer.thoiGianCanhBao}
                onChange={(event) => setTransfer((current) => ({ ...current, thoiGianCanhBao: event.target.value }))}
                disabled={transferLoading}
              />
            </Field>
            <div className="lg:col-span-3">
              <Field label="Ghi chú chuyển bước">
                <TextArea rows={3} value={transfer.noiDungGhiChu} onChange={(value) => setTransfer((current) => ({ ...current, noiDungGhiChu: value }))} disabled={transferLoading} />
              </Field>
            </div>
          </div>
          {checking?.dieuKienChuaDat?.length > 0 && (
            <div className="rounded-lg border border-warning-200 bg-warning-50 p-4 text-sm text-warning-700">
              <div className="font-medium">Điều kiện chưa đạt</div>
              <ul className="mt-2 list-disc space-y-1 pl-5">
                {checking.dieuKienChuaDat.map((item) => <li key={item}>{item}</li>)}
              </ul>
            </div>
          )}
          <div className="flex justify-end gap-3 border-t border-gray-100 pt-5 dark:border-white/[0.05]">
            <button type="button" onClick={closeTransferModal} disabled={transferLoading} className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
              Hủy
            </button>
            <button type="button" onClick={checkBeforeTransfer} disabled={transferLoading} className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
              Kiểm tra điều kiện
            </button>
            <button type="button" onClick={submitTransfer} disabled={transferLoading} className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300">
              {transferLoading ? "Đang xử lý..." : "Chuyển hồ sơ"}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
