import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import TextArea from "../../../app/components/forms/input/TextArea";
import { getQuyTrinhById } from "../../danh-muc/api/quyTrinhSoanThaoApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import {
  getHoSoXayDungVanBanById,
  getHoSoYKienUBND,
  getTaiLieuYKienUBND,
  guiKetQuaYKienUBND,
  kiemTraGuiKetQuaYKienUBND,
  updateHoSoYKienUBND,
  uploadTaiLieuYKienUBND,
} from "../api/xayDungVanBanApi";

const DEFAULT_PROCESSING_DAYS = 5;
const DEFAULT_WARNING_DAYS = 0;
const APPROVE_RESULT = "DONG_Y";
const RETURN_RESULT = "KHONG_DONG_Y";
const LEGACY_APPROVE_RESULT = "KHONG_CO_Y_KIEN_KHAC_NHAU";
const LEGACY_RETURN_RESULT = "CON_Y_KIEN_KHAC_NHAU";

const emptyForm = {
  ngayNhanYKien: "",
  tongSoThanhVienDuocLayYKien: "",
  soDongY: "",
  soKhongDongY: "",
  soYKienKhac: "",
  ketLuanTongHop: "",
  noiDungTongHop: "",
  noiDungGiaiTrinh: "",
  buocQuyTrinhTiepTheoId: "",
  trangThaiHoSoTiepTheoId: "",
  chuyenBuocId: "",
  loaiChuyenBuoc: "",
  lyDoTraLai: "",
  ngayChuyen: "",
  soNgayXuLy: DEFAULT_PROCESSING_DAYS,
  hanXuLy: "",
  soNgayCanhBao: DEFAULT_WARNING_DAYS,
  thoiGianCanhBao: "",
};

const errorMessage = (error) => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể xử lý yêu cầu.";
const dateValue = (value) => {
  if (!value) return "";
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return "";
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
};
const numberOrNull = (value) => value === "" || value == null ? null : Number(value);
const isReturnResult = (value) => value === RETURN_RESULT || value === LEGACY_RETURN_RESULT;
const statusInfo = (status) => status === "DaGui" || status === "DaHoanThanh" || status === "YeuCauBoSung" ? { label: "Đã xử lý", color: "success" } : { label: "Đang xử lý", color: "warning" };

function todayValue() {
  return dateValue(new Date());
}

function addDays(value, days) {
  const date = value ? new Date(value) : new Date();
  date.setDate(date.getDate() + (Number(days) || 0));
  return dateValue(date);
}

function subtractDays(value, days) {
  const date = value ? new Date(value) : new Date();
  date.setDate(date.getDate() - Math.max(0, Number(days) || 0));
  return dateValue(date);
}

function clampWarningDays(value, processingDays) {
  const warningDays = Math.max(0, Number(value) || 0);
  const maxDays = Math.max(1, Number(processingDays) || DEFAULT_PROCESSING_DAYS);
  return Math.min(warningDays, maxDays);
}

function deadlineDefaults(step, startDate = todayValue()) {
  const soNgayXuLy = step?.soNgayXuLyTieuChuan || DEFAULT_PROCESSING_DAYS;
  const soNgayCanhBao = clampWarningDays(step?.soNgayCanhBaoSapHan ?? DEFAULT_WARNING_DAYS, soNgayXuLy);
  const hanXuLy = addDays(startDate, soNgayXuLy);
  return { ngayChuyen: startDate, soNgayXuLy, hanXuLy, soNgayCanhBao, thoiGianCanhBao: subtractDays(hanXuLy, soNgayCanhBao) };
}

function normalizeResult(value) {
  if (value === LEGACY_RETURN_RESULT) return RETURN_RESULT;
  if (value === LEGACY_APPROVE_RESULT) return APPROVE_RESULT;
  return value || "";
}

function getSortedSteps(workflow) {
  return [...(workflow?.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0));
}

function getDecisionTransitions(workflow, currentStepId) {
  const steps = getSortedSteps(workflow);
  const stepMap = new Map(steps.map((step) => [step.id, step]));
  const currentStep = stepMap.get(currentStepId);
  const allTransitions = workflow?.chuyenBuocs || [];
  const direct = allTransitions.filter((item) => item.tuBuocId === currentStepId && !item.isKetThuc);
  const configured = direct.length ? direct : allTransitions.filter((item) => currentStep?.maBuoc && item.tuBuocMa === currentStep.maBuoc && !item.isKetThuc);
  const transitions = [...configured];

  if (!transitions.some(isApproveTransition)) {
    const banHanhStep = steps.find((step) => step.maBuoc === "BAN_HANH" || step.maBuoc === "THONG_QUA_BAN_HANH" || step.maBuoc?.includes("BAN_HANH"));
    if (banHanhStep) {
      transitions.push({
        id: "__approve_ban_hanh__",
        tuBuocId: currentStepId,
        tuBuocMa: currentStep?.maBuoc,
        denBuocId: banHanhStep.id,
        denBuocMa: banHanhStep.maBuoc,
        dieuKienKetQua: banHanhStep.maBuoc === "THONG_QUA_BAN_HANH" ? "DONG_Y_TRINH_HDND" : "DONG_Y_BAN_HANH",
        loaiChuyenBuoc: "Approve",
        laNhanhMacDinh: true,
        yeuCauNhapLyDo: false,
        isKetThuc: false,
      });
    }
  }

  if (!transitions.some(isReturnTransition)) {
    const trinhStep = steps.find((step) => step.maBuoc === "TRINH_UBND" || step.maBuoc === "TRINH_UBND_CHO_Y_KIEN");
    if (trinhStep) {
      transitions.push({
        id: "__return_trinh_ubnd__",
        tuBuocId: currentStepId,
        tuBuocMa: currentStep?.maBuoc,
        denBuocId: trinhStep.id,
        denBuocMa: trinhStep.maBuoc,
        dieuKienKetQua: "KHONG_DONG_Y_TRA_LAI",
        loaiChuyenBuoc: "Return",
        laNhanhMacDinh: false,
        yeuCauNhapLyDo: true,
        isKetThuc: false,
      });
    }
  }

  return transitions;
}

function isApproveTransition(transition) {
  const text = `${transition?.loaiChuyenBuoc || ""} ${transition?.dieuKienKetQua || ""} ${transition?.denBuocMa || ""}`.toUpperCase();
  return text.includes("APPROVE") || text.includes("DONG_Y") || text.includes("BAN_HANH") || text.includes("THONG_QUA");
}

function isReturnTransition(transition) {
  const text = `${transition?.loaiChuyenBuoc || ""} ${transition?.dieuKienKetQua || ""}`.toUpperCase();
  return text.includes("RETURN") || text.includes("TRA_LAI") || text.includes("KHONG_DONG") || text.includes("CO_Y_KIEN_KHAC");
}

function chooseTransition(transitions, result) {
  if (isReturnResult(result)) return transitions.find(isReturnTransition) || transitions.find((item) => item.yeuCauNhapLyDo) || transitions[0];
  return transitions.find(isApproveTransition) || transitions.find((item) => item.laNhanhMacDinh) || transitions[0];
}

function findStatusForTransition(statuses, transition, targetStep, fallbackStatusId) {
  const exact = statuses.find((status) => status.maTrangThai === transition?.dieuKienKetQua);
  if (exact) return exact.id;
  const targetCode = targetStep?.maBuoc || transition?.denBuocMa || "";
  if (isReturnTransition(transition)) return statuses.find((status) => status.maTrangThai === "DA_THAM_DINH")?.id || fallbackStatusId || statuses[0]?.id || "";
  if (targetCode.includes("BAN_HANH")) return statuses.find((status) => status.maTrangThai?.includes("BAN_HANH"))?.id || fallbackStatusId || statuses[0]?.id || "";
  if (targetCode.includes("THAM_TRA")) return statuses.find((status) => status.maTrangThai?.includes("THAM_TRA"))?.id || fallbackStatusId || statuses[0]?.id || "";
  return fallbackStatusId || statuses[0]?.id || "";
}

function getEffectiveTargetStep(transition, stepMap, steps) {
  const target = stepMap.get(transition?.denBuocId);
  if (!isReturnTransition(transition) || !target?.maBuoc?.includes("SOAN_THAO")) return target;
  return steps.find((step) => step.maBuoc === "TRINH_UBND" || step.maBuoc === "TRINH_UBND_CHO_Y_KIEN") || target;
}

export default function HoSoYKienUBNDPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [hoSo, setHoSo] = useState(null);
  const [data, setData] = useState(null);
  const [files, setFiles] = useState([]);
  const [workflow, setWorkflow] = useState(null);
  const [statuses, setStatuses] = useState([]);
  const [form, setForm] = useState(emptyForm);
  const [check, setCheck] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const isEditable = !data || data.trangThai === "Nhap";
  const status = statusInfo(data?.trangThai);
  const steps = useMemo(() => getSortedSteps(workflow), [workflow]);
  const stepMap = useMemo(() => new Map(steps.map((step) => [step.id, step])), [steps]);
  const transitions = useMemo(() => getDecisionTransitions(workflow, hoSo?.buocHienTaiId), [workflow, hoSo?.buocHienTaiId]);
  const selectedTransition = useMemo(() => transitions.find((item) => item.id === form.chuyenBuocId) || null, [transitions, form.chuyenBuocId]);
  const mustReturn = isReturnResult(form.ketLuanTongHop) || isReturnTransition(selectedTransition);
  const approveTransition = useMemo(() => transitions.find(isApproveTransition) || null, [transitions]);
  const returnTransition = useMemo(() => transitions.find(isReturnTransition) || null, [transitions]);
  const approveTargetStep = useMemo(() => approveTransition ? getEffectiveTargetStep(approveTransition, stepMap, steps) : null, [approveTransition, stepMap, steps]);
  const returnTargetStep = useMemo(() => returnTransition ? getEffectiveTargetStep(returnTransition, stepMap, steps) : null, [returnTransition, stepMap, steps]);

  const statusOptions = useMemo(() => statuses.filter((item) => item.trangThai).map((item) => ({ value: item.id, label: `${item.tenTrangThai || item.maTrangThai}${item.maTrangThai ? ` (${item.maTrangThai})` : ""}` })), [statuses]);
  const stepOptions = useMemo(() => steps.map((step) => ({ value: step.id, label: `${step.thuTuSapXep}. ${step.tenBuoc}` })), [steps]);

  const update = (key, value) => setForm((current) => ({ ...current, [key]: value }));

  const applyTransition = (transition, result, statusList = statuses, startDate = todayValue()) => {
    if (!transition) return;
    const targetStep = getEffectiveTargetStep(transition, stepMap, steps);
    setForm((current) => ({
      ...current,
      ketLuanTongHop: result || current.ketLuanTongHop,
      chuyenBuocId: transition.id,
      buocQuyTrinhTiepTheoId: targetStep?.id || transition.denBuocId,
      trangThaiHoSoTiepTheoId: findStatusForTransition(statusList, transition, targetStep, hoSo?.trangThaiHoSoId),
      loaiChuyenBuoc: transition.loaiChuyenBuoc || "",
      lyDoTraLai: isReturnTransition(transition) ? current.lyDoTraLai : "",
      ...deadlineDefaults(targetStep, startDate),
    }));
  };

  const setResult = (value) => {
    const normalized = normalizeResult(value);
    const transition = chooseTransition(transitions, normalized);
    const targetStep = transition ? getEffectiveTargetStep(transition, stepMap, steps) : null;
    setForm((current) => ({
      ...current,
      ketLuanTongHop: normalized,
      ...(transition ? {
        chuyenBuocId: transition.id,
        buocQuyTrinhTiepTheoId: targetStep?.id || transition.denBuocId,
        trangThaiHoSoTiepTheoId: findStatusForTransition(statuses, transition, targetStep, hoSo?.trangThaiHoSoId),
        loaiChuyenBuoc: transition.loaiChuyenBuoc || "",
        ...deadlineDefaults(targetStep, todayValue()),
      } : {}),
    }));
  };

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const detail = await getHoSoXayDungVanBanById(id);
      const [current, wf, stateResult] = await Promise.all([
        getHoSoYKienUBND(id).catch((e) => e?.response?.status === 404 ? null : Promise.reject(e)),
        detail.quyTrinhSoanThaoId ? getQuyTrinhById(detail.quyTrinhSoanThaoId) : Promise.resolve(null),
        getTrangThais({ nhomTrangThai: "HO_SO_XAY_DUNG_VAN_BAN", pageSize: 100, pageCurrent: 1 }),
      ]);
      const statusList = stateResult?.data || [];
      const directTransitions = getDecisionTransitions(wf, detail.buocHienTaiId);
      const result = normalizeResult(current?.ketLuanTongHop || APPROVE_RESULT);
      const transition = chooseTransition(directTransitions, result);
      const localStepMap = new Map(getSortedSteps(wf).map((step) => [step.id, step]));
      const localSteps = getSortedSteps(wf);
      const targetStep = transition ? getEffectiveTargetStep(transition, localStepMap, localSteps) : null;

      setHoSo(detail);
      setData(current);
      setWorkflow(wf);
      setStatuses(statusList);

      if (current) {
        setForm({
          ngayNhanYKien: dateValue(current.ngayNhanYKien),
          tongSoThanhVienDuocLayYKien: current.tongSoThanhVienDuocLayYKien ?? "",
          soDongY: current.soDongY ?? "",
          soKhongDongY: current.soKhongDongY ?? "",
          soYKienKhac: current.soYKienKhac ?? "",
          ketLuanTongHop: result,
          noiDungTongHop: current.noiDungTongHop || "",
          noiDungGiaiTrinh: current.noiDungGiaiTrinh || "",
          buocQuyTrinhTiepTheoId: targetStep?.id || transition?.denBuocId || "",
          trangThaiHoSoTiepTheoId: transition ? findStatusForTransition(statusList, transition, targetStep, detail.trangThaiHoSoId) : detail.trangThaiHoSoId || "",
          chuyenBuocId: transition?.id || "",
          loaiChuyenBuoc: transition?.loaiChuyenBuoc || "",
          lyDoTraLai: "",
          ...deadlineDefaults(targetStep),
        });
        setFiles(await getTaiLieuYKienUBND(id));
      }
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void load(); }, [id]);

  const save = async () => {
    if (!data) return;
    setSaving(true);
    setError("");
    try {
      const saved = await updateHoSoYKienUBND(id, {
        ngayNhanYKien: form.ngayNhanYKien || null,
        tongSoThanhVienDuocLayYKien: numberOrNull(form.tongSoThanhVienDuocLayYKien),
        soDongY: numberOrNull(form.soDongY),
        soKhongDongY: numberOrNull(form.soKhongDongY),
        soYKienKhac: numberOrNull(form.soYKienKhac),
        ketLuanTongHop: form.ketLuanTongHop || null,
        noiDungTongHop: form.noiDungTongHop || null,
        noiDungGiaiTrinh: form.noiDungGiaiTrinh || null,
      });
      setData(saved);
      setSuccess("Đã lưu tổng hợp ý kiến UBND.");
      return saved;
    } catch (e) {
      setError(errorMessage(e));
      throw e;
    } finally {
      setSaving(false);
    }
  };

  const upload = async (event) => {
    const file = event.target.files?.[0];
    if (!file || !data) return;
    try {
      setSaving(true);
      await uploadTaiLieuYKienUBND(id, { file, loaiTaiLieuId: crypto.randomUUID(), tenTaiLieu: file.name });
      setFiles(await getTaiLieuYKienUBND(id));
      setSuccess("Đã tải tài liệu ý kiến UBND.");
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      event.target.value = "";
      setSaving(false);
    }
  };

  const validate = async () => {
    try {
      const result = await kiemTraGuiKetQuaYKienUBND(id);
      setCheck(result);
      if (result.dat) setSuccess("Hồ sơ đủ điều kiện chuyển bước.");
    } catch (e) {
      setError(errorMessage(e));
    }
  };

  const send = async () => {
    if (!form.buocQuyTrinhTiepTheoId || !form.trangThaiHoSoTiepTheoId || !form.hanXuLy || !form.thoiGianCanhBao) {
      setError("Chọn nhánh chuyển, trạng thái và thời hạn xử lý trước khi chuyển bước.");
      return;
    }
    if (mustReturn && !form.lyDoTraLai.trim()) {
      setError("Nhập lý do trả lại hồ sơ trước khi chuyển bước.");
      return;
    }
    try {
      setSaving(true);
      await save();
      await guiKetQuaYKienUBND(id, {
        buocQuyTrinhTiepTheoId: form.buocQuyTrinhTiepTheoId,
        trangThaiHoSoTiepTheoId: form.trangThaiHoSoTiepTheoId,
        hanXuLy: form.hanXuLy || null,
        thoiGianCanhBao: form.thoiGianCanhBao || null,
        soNgayXuLy: Number(form.soNgayXuLy) || null,
        soNgayCanhBao: Number(form.soNgayCanhBao) || 0,
        loaiChuyenBuoc: form.loaiChuyenBuoc || selectedTransition?.loaiChuyenBuoc || null,
        lyDoTraLai: mustReturn ? form.lyDoTraLai.trim() : null,
        maBuocTiepTheo: stepMap.get(form.buocQuyTrinhTiepTheoId)?.maBuoc || selectedTransition?.denBuocMa || null,
      });
      navigate("/xay-dung-van-ban/y-kien-ubnd", { state: { success: mustReturn ? "Đã trả lại hồ sơ về bước trình." : "Đã chuyển hồ sơ sang bước tiếp theo." } });
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setSaving(false);
    }
  };

  if (!loading && data && !isEditable) {
    return (
      <div className="space-y-5">
        <div className="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Ý kiến thành viên UBND</h1>
            <p className="mt-1 text-sm text-gray-500">{hoSo ? `${hoSo.maHoSo} · ${hoSo.tenHoSo}` : "Hồ sơ đã xử lý."}</p>
          </div>
          <div className="flex gap-2">
            <button onClick={() => navigate("/xay-dung-van-ban/y-kien-ubnd")} className="rounded-lg border px-4 py-2 text-sm">Danh sách</button>
            <button onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${id}`, "_blank", "noopener,noreferrer")} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white">Xem timeline</button>
          </div>
        </div>
        <Alert variant="info" title="Hồ sơ đã xử lý" message="Hồ sơ đã được chuyển theo kết quả ý kiến UBND. Màn hình này chỉ cho phép theo dõi timeline xử lý." />
      </div>
    );
  }

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Ý kiến thành viên UBND</h1>
          <p className="mt-1 text-sm text-gray-500">{hoSo ? `${hoSo.maHoSo} · ${hoSo.tenHoSo}` : "Tổng hợp ý kiến thành viên UBND."}</p>
        </div>
        <div className="flex gap-2">
          <button onClick={() => navigate("/xay-dung-van-ban/y-kien-ubnd")} className="rounded-lg border px-4 py-2 text-sm">Danh sách</button>
          <button onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${id}`, "_blank", "noopener,noreferrer")} className="rounded-lg border px-4 py-2 text-sm">Xem hồ sơ</button>
        </div>
      </div>
      {error && <Alert variant="error" title="Có lỗi" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}
      {loading ? (
        <div className="py-16 text-center text-sm text-gray-500">Đang tải hồ sơ...</div>
      ) : !data ? (
        <Alert variant="warning" title="Chưa có hồ sơ ý kiến UBND" message="Vui lòng lập hồ sơ ý kiến từ màn danh sách." />
      ) : (
        <>
          <div className="rounded-xl border bg-white p-5">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="font-semibold">Tổng hợp ý kiến</h2>
              <Badge size="sm" color={status.color}>{status.label}</Badge>
            </div>
            <div className="grid gap-4 sm:grid-cols-3">
              <div><Label>Ngày nhận ý kiến</Label><Input type="date" value={form.ngayNhanYKien} onChange={(event) => update("ngayNhanYKien", event.target.value)} disabled={!isEditable} /></div>
              <div><Label>Tổng số thành viên</Label><Input type="number" min="0" value={form.tongSoThanhVienDuocLayYKien} onChange={(event) => update("tongSoThanhVienDuocLayYKien", event.target.value)} disabled={!isEditable} /></div>
              <div><Label>Số đồng ý</Label><Input type="number" min="0" value={form.soDongY} onChange={(event) => update("soDongY", event.target.value)} disabled={!isEditable} /></div>
              <div><Label>Số không đồng ý</Label><Input type="number" min="0" value={form.soKhongDongY} onChange={(event) => update("soKhongDongY", event.target.value)} disabled={!isEditable} /></div>
              <div><Label>Số ý kiến khác</Label><Input type="number" min="0" value={form.soYKienKhac} onChange={(event) => update("soYKienKhac", event.target.value)} disabled={!isEditable} /></div>
              <div><Label>Kết luận UBND *</Label><Select value={form.ketLuanTongHop} options={[{ value: "", label: "Chọn kết luận" }, { value: APPROVE_RESULT, label: "Đồng ý, chuyển sang ban hành/bước tiếp theo" }, { value: RETURN_RESULT, label: "Không đồng ý, trả lại bước trình" }]} onChange={setResult} disabled={!isEditable} /></div>
              <div className="sm:col-span-3"><Label>Nội dung tổng hợp *</Label><TextArea rows={4} value={form.noiDungTongHop} onChange={(value) => update("noiDungTongHop", value)} disabled={!isEditable} /></div>
              <div className="sm:col-span-3"><Label>Nội dung giải trình</Label><TextArea rows={3} value={form.noiDungGiaiTrinh} onChange={(value) => update("noiDungGiaiTrinh", value)} disabled={!isEditable} /></div>
            </div>
            {isEditable && <div className="mt-5 flex justify-end"><button onClick={save} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">Lưu tổng hợp</button></div>}
          </div>

          <div className="rounded-xl border bg-white p-5">
            <div className="mb-4 flex flex-wrap items-end justify-between gap-3">
              <div>
                <h2 className="font-semibold">Tài liệu ý kiến UBND</h2>
                <p className="text-sm text-gray-500">File tổng hợp ý kiến, biên bản hoặc tài liệu giải trình.</p>
              </div>
              {isEditable && <label className="cursor-pointer rounded-lg border px-4 py-2 text-sm"><input type="file" className="hidden" onChange={upload} disabled={saving} />Tải tài liệu</label>}
            </div>
            <div className="divide-y rounded-lg border">
              {files.length ? files.map((file) => (
                <div key={file.boHoSoTaiLieuId || file.id} className="flex items-center justify-between gap-3 px-4 py-3 text-sm">
                  <div><div className="font-medium">{file.tenTaiLieu}</div><div className="text-xs text-gray-500">{file.tenFile} · Phiên bản {file.phienBan}</div></div>
                  <Badge size="sm" color={file.hinhThucThem === "KeThua" ? "light" : "warning"}>{file.hinhThucThem === "KeThua" ? "Kế thừa" : "Bổ sung"}</Badge>
                </div>
              )) : <div className="p-6 text-center text-sm text-gray-500">Chưa có tài liệu.</div>}
            </div>
          </div>

          <div className="rounded-xl border bg-white p-5">
            <h2 className="mb-4 font-semibold">Chuyển hồ sơ theo kết quả ý kiến UBND</h2>
            {!approveTransition && !returnTransition && <Alert variant="warning" title="Chưa có nhánh chuyển" message="Danh mục quy trình chưa cấu hình nhánh chuyển từ bước ý kiến UBND. Vui lòng kiểm tra lại danh mục quy trình soạn thảo." />}
            <div className="mb-5 grid gap-3 md:grid-cols-2">
              <button
                type="button"
                onClick={() => applyTransition(approveTransition, APPROVE_RESULT)}
                disabled={!isEditable || !approveTransition}
                className={`rounded-lg border p-4 text-left transition disabled:cursor-not-allowed disabled:opacity-50 ${!mustReturn && form.ketLuanTongHop === APPROVE_RESULT ? "border-brand-500 bg-brand-50" : "border-gray-200 bg-white hover:border-brand-300"}`}
              >
                <div className="text-sm font-semibold text-gray-800">Đồng ý</div>
                <div className="mt-1 text-sm text-gray-500">Chuyển hồ sơ sang bước {approveTargetStep?.tenBuoc || "ban hành"}.</div>
              </button>
              <button
                type="button"
                onClick={() => applyTransition(returnTransition, RETURN_RESULT)}
                disabled={!isEditable || !returnTransition}
                className={`rounded-lg border p-4 text-left transition disabled:cursor-not-allowed disabled:opacity-50 ${mustReturn ? "border-error-500 bg-error-50" : "border-gray-200 bg-white hover:border-error-300"}`}
              >
                <div className="text-sm font-semibold text-gray-800">Không đồng ý, trả lại</div>
                <div className="mt-1 text-sm text-gray-500">Trả hồ sơ về bước {returnTargetStep?.tenBuoc || "trình UBND"} và nhập lý do trả lại.</div>
              </button>
            </div>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              <div><Label>Bước sau chuyển *</Label><Select value={form.buocQuyTrinhTiepTheoId} options={[{ value: "", label: "Chọn bước" }, ...stepOptions]} onChange={(value) => { const step = stepMap.get(value); setForm((current) => ({ ...current, buocQuyTrinhTiepTheoId: value, ...deadlineDefaults(step, current.ngayChuyen || todayValue()) })); }} disabled={!isEditable} /></div>
              <div><Label>Trạng thái sau chuyển *</Label><Select value={form.trangThaiHoSoTiepTheoId} options={[{ value: "", label: "Chọn trạng thái" }, ...statusOptions]} onChange={(value) => update("trangThaiHoSoTiepTheoId", value)} disabled={!isEditable} /></div>
              <div><Label>Ngày chuyển</Label><Input type="date" value={form.ngayChuyen || todayValue()} onChange={(event) => { const ngayChuyen = event.target.value; setForm((current) => { const hanXuLy = addDays(ngayChuyen, current.soNgayXuLy); return { ...current, ngayChuyen, hanXuLy, thoiGianCanhBao: subtractDays(hanXuLy, current.soNgayCanhBao) }; }); }} disabled={!isEditable} /></div>
              <div><Label>Số ngày xử lý</Label><Input type="number" min="1" value={form.soNgayXuLy} onChange={(event) => { const soNgayXuLy = Math.max(1, Number(event.target.value) || DEFAULT_PROCESSING_DAYS); setForm((current) => { const ngayChuyen = current.ngayChuyen || todayValue(); const soNgayCanhBao = clampWarningDays(current.soNgayCanhBao, soNgayXuLy); const hanXuLy = addDays(ngayChuyen, soNgayXuLy); return { ...current, ngayChuyen, soNgayXuLy, soNgayCanhBao, hanXuLy, thoiGianCanhBao: subtractDays(hanXuLy, soNgayCanhBao) }; }); }} disabled={!isEditable} /></div>
              <div><Label>Thời hạn xử lý *</Label><Input type="date" value={form.hanXuLy} onChange={(event) => { const hanXuLy = event.target.value; setForm((current) => ({ ...current, hanXuLy, thoiGianCanhBao: subtractDays(hanXuLy, current.soNgayCanhBao) })); }} disabled={!isEditable} /></div>
              <div><Label>Số ngày cảnh báo</Label><Input type="number" min="0" max={form.soNgayXuLy || DEFAULT_PROCESSING_DAYS} value={form.soNgayCanhBao} onChange={(event) => { setForm((current) => { const soNgayCanhBao = clampWarningDays(event.target.value, current.soNgayXuLy); const ngayChuyen = current.ngayChuyen || todayValue(); const hanXuLy = current.hanXuLy || addDays(ngayChuyen, current.soNgayXuLy); return { ...current, ngayChuyen, soNgayCanhBao, hanXuLy, thoiGianCanhBao: subtractDays(hanXuLy, soNgayCanhBao) }; }); }} disabled={!isEditable} /></div>
              <div><Label>Thời gian cảnh báo</Label><Input type="date" value={form.thoiGianCanhBao} onChange={(event) => update("thoiGianCanhBao", event.target.value)} disabled={!isEditable} /></div>
              {mustReturn && <div className="sm:col-span-2 lg:col-span-3"><Label>Lý do trả lại *</Label><TextArea rows={3} value={form.lyDoTraLai} onChange={(value) => update("lyDoTraLai", value)} disabled={!isEditable} /></div>}
            </div>
          </div>

          {isEditable && (
            <div className="flex flex-wrap justify-end gap-3">
              <button onClick={validate} disabled={saving} className="rounded-lg border px-4 py-2.5 text-sm font-medium">Kiểm tra trước khi chuyển</button>
              <button onClick={send} disabled={saving || check?.dat === false || (!approveTransition && !returnTransition)} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">{mustReturn ? "Trả lại hồ sơ" : "Chuyển bước"}</button>
            </div>
          )}
          {check && !check.dat && <Alert variant="warning" title="Chưa thể chuyển bước" message={check.dieuKienChuaDat?.join(" ")} />}
        </>
      )}
    </div>
  );
}
