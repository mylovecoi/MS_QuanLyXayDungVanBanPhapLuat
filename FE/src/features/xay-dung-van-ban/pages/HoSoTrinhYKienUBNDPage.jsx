import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import TextArea from "../../../app/components/forms/input/TextArea";
import { getDonVis } from "../../danh-muc/api/donViApi";
import { getQuyTrinhById } from "../../danh-muc/api/quyTrinhSoanThaoApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { getHoSoTrinhYKienUBND, getHoSoXayDungVanBanById, getTaiLieuTrinhYKienUBND, guiYKienUBND, kiemTraGuiYKienUBND, updateHoSoTrinhYKienUBND, uploadTaiLieuTrinhYKienUBND } from "../api/xayDungVanBanApi";

const errorMessage = e => e?.response?.data?.message || e?.response?.data || e?.message || "Không thể xử lý yêu cầu.";
const dateValue = value => value ? String(value).slice(0, 10) : "";
const toDateInputValue = value => {
  const date = value ? new Date(value) : new Date();
  if (Number.isNaN(date.getTime())) return "";
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
};
const DEFAULT_PROCESSING_DAYS = 5;
const DEFAULT_WARNING_DAYS = 0;
const emptyForm = { capTrinh: "UBND tỉnh", mucDichTrinh: "Trình UBND tỉnh lấy ý kiến thành viên đối với hồ sơ xây dựng văn bản", soToTrinh: "", ngayToTrinh: "", noiDungTrinh: "", donViDongGuiId: "", buocQuyTrinhTiepTheoId: "", trangThaiHoSoTiepTheoId: "", ngayTrinh: "", soNgayXuLy: DEFAULT_PROCESSING_DAYS, hanXuLy: "", soNgayCanhBao: DEFAULT_WARNING_DAYS, thoiGianCanhBao: "" };
const statusInfo = status => status === "DaGui" ? { label: "Đã trình lấy ý kiến", color: "success" } : { label: "Đang nhập", color: "warning" };
const positiveNumber = (value, fallback) => Math.max(1, Number(value) || fallback);
const nonNegativeNumber = value => Math.max(0, Number(value) || 0);

function todayValue() {
  return toDateInputValue();
}

function addDays(value, days) {
  const date = value ? new Date(value) : new Date();
  date.setDate(date.getDate() + (Number(days) || 0));
  return toDateInputValue(date);
}

function subtractDays(value, days) {
  const date = value ? new Date(value) : new Date();
  date.setDate(date.getDate() - Math.max(0, Number(days) || 0));
  return toDateInputValue(date);
}

function deadlineDefaults(step, startDate = todayValue()) {
  const soNgayXuLy = positiveNumber(step?.soNgayXuLyTieuChuan, DEFAULT_PROCESSING_DAYS);
  const soNgayCanhBao = nonNegativeNumber(step?.soNgayCanhBaoSapHan ?? DEFAULT_WARNING_DAYS);
  const hanXuLy = addDays(startDate, soNgayXuLy);
  return { ngayTrinh: startDate, soNgayXuLy, hanXuLy, soNgayCanhBao, thoiGianCanhBao: subtractDays(hanXuLy, soNgayCanhBao) };
}

function buildSchedule({ ngayTrinh = todayValue(), soNgayXuLy = DEFAULT_PROCESSING_DAYS, soNgayCanhBao = DEFAULT_WARNING_DAYS, hanXuLy } = {}) {
  const startDate = ngayTrinh || todayValue();
  const processingDays = positiveNumber(soNgayXuLy, DEFAULT_PROCESSING_DAYS);
  const warningDays = nonNegativeNumber(soNgayCanhBao);
  const deadline = hanXuLy || addDays(startDate, processingDays);

  return {
    ngayTrinh: startDate,
    soNgayXuLy: processingDays,
    hanXuLy: deadline,
    soNgayCanhBao: warningDays,
    thoiGianCanhBao: subtractDays(deadline, warningDays),
  };
}

function nextSteps(workflow, currentStepId) {
  const steps = [...(workflow?.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0));
  const map = new Map(steps.map(step => [step.id, step]));
  const trinhStep = steps.find(step => step.maBuoc === "TRINH_UBND_CHO_Y_KIEN" || step.maBuoc === "TRINH_UBND");
  const fromStepId = trinhStep?.id || currentStepId;
  const direct = (workflow?.chuyenBuocs || [])
    .filter(x => x.tuBuocId === fromStepId && !x.isKetThuc && x.loaiChuyenBuoc !== "Return" && x.loaiChuyenBuoc !== "Reject")
    .sort((a, b) => Number(b.laNhanhMacDinh) - Number(a.laNhanhMacDinh))
    .map(x => map.get(x.denBuocId))
    .filter(Boolean);
  const layYKienSteps = direct.filter(step => step.maBuoc === "LAY_Y_KIEN_UBND");
  if (layYKienSteps.length) return layYKienSteps;
  if (direct.length) return direct;
  const currentOrder = map.get(fromStepId)?.thuTuSapXep || 0;
  return steps.filter(step => (step.thuTuSapXep || 0) > currentOrder);
}

function statusForStep(statuses, step, fallbackStatusId) {
  const targetCode = step?.maBuoc || "";
  if (targetCode === "LAY_Y_KIEN_UBND") return statuses.find(status => status.maTrangThai === "CHO_Y_KIEN_UBND")?.id || fallbackStatusId || statuses[0]?.id || "";
  return fallbackStatusId || statuses[0]?.id || "";
}

export default function HoSoTrinhYKienUBNDPage() {
  const { id } = useParams(); const navigate = useNavigate();
  const [hoSo, setHoSo] = useState(null), [data, setData] = useState(null), [files, setFiles] = useState([]), [workflow, setWorkflow] = useState(null), [donVis, setDonVis] = useState([]), [statuses, setStatuses] = useState([]);
  const [form, setForm] = useState(emptyForm), [check, setCheck] = useState(null), [loading, setLoading] = useState(true), [saving, setSaving] = useState(false), [error, setError] = useState(""), [success, setSuccess] = useState("");
  const isEditable = !data || data.trangThai === "Nhap";
  const status = statusInfo(data?.trangThai);
  const stepOptions = useMemo(() => nextSteps(workflow, hoSo?.buocHienTaiId).map(x => ({ value: x.id, label: `${x.thuTuSapXep}. ${x.tenBuoc}` })), [workflow, hoSo]);
  const statusOptions = useMemo(() => statuses.filter(x => x.trangThai).map(x => ({ value: x.id, label: x.tenTrangThai || x.maTrangThai })), [statuses]);
  const update = (key, value) => setForm(current => ({ ...current, [key]: value }));
  const updateNextStep = value => {
    const step = nextSteps(workflow, hoSo?.buocHienTaiId).find(x => x.id === value);
    setForm(current => ({
      ...current,
      buocQuyTrinhTiepTheoId: value,
      trangThaiHoSoTiepTheoId: statusForStep(statuses, step, current.trangThaiHoSoTiepTheoId),
      ...deadlineDefaults(step, current.ngayTrinh || todayValue()),
    }));
  };
  const load = async () => { try { setLoading(true); setError(""); const detail = await getHoSoXayDungVanBanById(id); const [current, wf, units, stateResult] = await Promise.all([getHoSoTrinhYKienUBND(id).catch(e => e?.response?.status === 404 ? null : Promise.reject(e)), detail.quyTrinhSoanThaoId ? getQuyTrinhById(detail.quyTrinhSoanThaoId) : Promise.resolve(null), getDonVis({ pageSize: 200, pageCurrent: 1 }), getTrangThais({ nhomTrangThai: "HO_SO_XAY_DUNG_VAN_BAN", pageSize: 100, pageCurrent: 1 })]); const statusList = stateResult?.data || []; setHoSo(detail); setData(current); setWorkflow(wf); setDonVis(units?.data || []); setStatuses(statusList); const next = nextSteps(wf, detail.buocHienTaiId)[0]; const deadlines = deadlineDefaults(next); if (current) { setForm({ capTrinh: current.capTrinh || emptyForm.capTrinh, mucDichTrinh: current.mucDichTrinh || emptyForm.mucDichTrinh, soToTrinh: current.soToTrinh || "", ngayToTrinh: dateValue(current.ngayToTrinh), noiDungTrinh: current.noiDungTrinh || "", donViDongGuiId: current.donViDongGuiId || "", buocQuyTrinhTiepTheoId: next?.id || "", trangThaiHoSoTiepTheoId: statusForStep(statusList, next, detail.trangThaiHoSoId), ...deadlines }); setFiles(await getTaiLieuTrinhYKienUBND(id)); } } catch (e) { setError(errorMessage(e)); } finally { setLoading(false); } };
  useEffect(() => { void load(); }, [id]);
  const save = async () => { if (!data) return; try { setSaving(true); setError(""); setData(await updateHoSoTrinhYKienUBND(id, { capTrinh: form.capTrinh, mucDichTrinh: form.mucDichTrinh, soToTrinh: form.soToTrinh || null, ngayToTrinh: form.ngayToTrinh || null, noiDungTrinh: form.noiDungTrinh || null, donViDongGuiId: form.donViDongGuiId || null })); setSuccess("Đã lưu hồ sơ trình lấy ý kiến UBND."); } catch (e) { setError(errorMessage(e)); } finally { setSaving(false); } };
  const upload = async event => { const file = event.target.files?.[0]; if (!file || !data) return; try { setSaving(true); await uploadTaiLieuTrinhYKienUBND(id, { file, loaiTaiLieuId: crypto.randomUUID(), tenTaiLieu: file.name }); setFiles(await getTaiLieuTrinhYKienUBND(id)); setSuccess("Đã tải tài liệu trình lấy ý kiến UBND."); } catch (e) { setError(errorMessage(e)); } finally { event.target.value = ""; setSaving(false); } };
  const validate = async () => { try { const result = await kiemTraGuiYKienUBND(id); setCheck(result); if (result.dat) setSuccess("Hồ sơ đủ điều kiện gửi trình lấy ý kiến UBND."); } catch (e) { setError(errorMessage(e)); } };
  const send = async () => { if (!form.buocQuyTrinhTiepTheoId || !form.trangThaiHoSoTiepTheoId || !form.hanXuLy || !form.thoiGianCanhBao) { setError("Chọn bước, trạng thái, thời hạn xử lý và thời gian cảnh báo trước khi gửi trình."); return; } try { setSaving(true); await guiYKienUBND(id, { ngayTrinh: form.ngayTrinh || new Date().toISOString(), buocQuyTrinhTiepTheoId: form.buocQuyTrinhTiepTheoId, trangThaiHoSoTiepTheoId: form.trangThaiHoSoTiepTheoId, hanXuLy: form.hanXuLy || null, thoiGianCanhBao: form.thoiGianCanhBao || null, soNgayXuLy: Number(form.soNgayXuLy) || null, soNgayCanhBao: Number(form.soNgayCanhBao) || 0 }); navigate("/xay-dung-van-ban/trinh-y-kien-ubnd", { state: { success: "Đã gửi hồ sơ trình lấy ý kiến UBND." } }); } catch (e) { setError(errorMessage(e)); } finally { setSaving(false); } };

  if (!loading && data && !isEditable) {
    return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Trình lấy ý kiến UBND</h1><p className="mt-1 text-sm text-gray-500">{hoSo ? `${hoSo.maHoSo} · ${hoSo.tenHoSo}` : "Hồ sơ đã trình lấy ý kiến UBND."}</p></div><div className="flex gap-2"><button onClick={() => navigate("/xay-dung-van-ban/trinh-y-kien-ubnd")} className="rounded-lg border px-4 py-2 text-sm">Danh sách</button><button onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${id}`, "_blank", "noopener,noreferrer")} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white">Xem timeline</button></div></div><Alert variant="info" title="Hồ sơ đã trình" message="Hồ sơ đã được gửi sang bước tiếp theo. Màn hình này chỉ cho phép theo dõi timeline xử lý." /></div>;
  }

  return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Trình lấy ý kiến UBND</h1><p className="mt-1 text-sm text-gray-500">{hoSo ? `${hoSo.maHoSo} · ${hoSo.tenHoSo}` : "Lập hồ sơ trình UBND tỉnh lấy ý kiến thành viên."}</p></div><div className="flex gap-2"><button onClick={() => navigate("/xay-dung-van-ban/trinh-y-kien-ubnd")} className="rounded-lg border px-4 py-2 text-sm">Danh sách</button><button onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${id}`, "_blank", "noopener,noreferrer")} className="rounded-lg border px-4 py-2 text-sm">Xem hồ sơ</button></div></div>{error && <Alert variant="error" title="Có lỗi" message={error} />}{success && <Alert variant="success" title="Hoàn tất" message={success} />}{loading ? <div className="py-16 text-center text-sm text-gray-500">Đang tải hồ sơ...</div> : !data ? <Alert variant="warning" title="Chưa có hồ sơ trình" message="Vui lòng lập hồ sơ trình từ màn danh sách trình lấy ý kiến UBND." /> : <><div className="rounded-xl border bg-white p-5"><div className="mb-4 flex items-center justify-between"><h2 className="font-semibold">Thông tin trình</h2><Badge size="sm" color={status.color}>{status.label}</Badge></div><div className="grid gap-4 sm:grid-cols-2"><div><Label>Cấp trình *</Label><Input value={form.capTrinh} onChange={e => update("capTrinh", e.target.value)} disabled={!isEditable} /></div><div><Label>Mục đích trình *</Label><Input value={form.mucDichTrinh} onChange={e => update("mucDichTrinh", e.target.value)} disabled={!isEditable} /></div><div><Label>Số tờ trình</Label><Input value={form.soToTrinh} onChange={e => update("soToTrinh", e.target.value)} disabled={!isEditable} /></div><div><Label>Ngày tờ trình</Label><Input type="date" value={form.ngayToTrinh} onChange={e => update("ngayToTrinh", e.target.value)} disabled={!isEditable} /></div><div><Label>Đơn vị đồng gửi</Label><Select value={form.donViDongGuiId} placeholder="Chọn đơn vị" options={[{ value: "", label: "Không chọn" }, ...donVis.map(x => ({ value: x.id, label: x.tenDonVi || x.ten || x.maDonVi }))]} onChange={value => update("donViDongGuiId", value)} disabled={!isEditable} /></div><div><Label>Bước tiếp theo *</Label><Select value={form.buocQuyTrinhTiepTheoId} options={[{ value: "", label: "Chọn bước" }, ...stepOptions]} onChange={updateNextStep} disabled={!isEditable} /></div><div><Label>Trạng thái sau gửi *</Label><Select value={form.trangThaiHoSoTiepTheoId} options={[{ value: "", label: "Chọn trạng thái" }, ...statusOptions]} onChange={value => update("trangThaiHoSoTiepTheoId", value)} disabled={!isEditable} /></div><div><Label>Ngày trình</Label><Input type="date" value={form.ngayTrinh} onChange={e => setForm(current => ({ ...current, ...buildSchedule({ ngayTrinh: e.target.value, soNgayXuLy: current.soNgayXuLy, soNgayCanhBao: current.soNgayCanhBao }) }))} disabled={!isEditable} /></div><div><Label>Số ngày xử lý</Label><Input type="number" min="1" value={form.soNgayXuLy} onChange={e => setForm(current => ({ ...current, ...buildSchedule({ ngayTrinh: current.ngayTrinh, soNgayXuLy: e.target.value, soNgayCanhBao: current.soNgayCanhBao }) }))} disabled={!isEditable} /></div><div><Label>Thời hạn xử lý *</Label><Input type="date" value={form.hanXuLy} onChange={e => setForm(current => ({ ...current, ...buildSchedule({ ngayTrinh: current.ngayTrinh, soNgayXuLy: current.soNgayXuLy, soNgayCanhBao: current.soNgayCanhBao, hanXuLy: e.target.value }) }))} disabled={!isEditable} /></div><div><Label>Số ngày cảnh báo</Label><Input type="number" min="0" value={form.soNgayCanhBao} onChange={e => setForm(current => ({ ...current, ...buildSchedule({ ngayTrinh: current.ngayTrinh, soNgayXuLy: current.soNgayXuLy, soNgayCanhBao: e.target.value, hanXuLy: current.hanXuLy }) }))} disabled={!isEditable} /></div><div><Label>Thời gian cảnh báo *</Label><Input type="date" value={form.thoiGianCanhBao} onChange={e => update("thoiGianCanhBao", e.target.value)} disabled={!isEditable} /></div><div className="sm:col-span-2"><Label>Nội dung trình *</Label><TextArea rows={4} value={form.noiDungTrinh} onChange={value => update("noiDungTrinh", value)} disabled={!isEditable} /></div></div>{isEditable && <div className="mt-5 flex flex-wrap justify-end gap-3"><button onClick={save} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">Lưu hồ sơ trình</button></div>}</div><div className="rounded-xl border bg-white p-5"><div className="mb-4 flex flex-wrap items-end justify-between gap-3"><div><h2 className="font-semibold">Tài liệu trình lấy ý kiến UBND</h2><p className="text-sm text-gray-500">Tài liệu kế thừa từ thẩm định và tài liệu bổ sung cho hồ sơ trình.</p></div>{isEditable && <label className="cursor-pointer rounded-lg border px-4 py-2 text-sm"><input type="file" className="hidden" onChange={upload} disabled={saving} />Tải tài liệu</label>}</div><div className="divide-y rounded-lg border">{files.length ? files.map(file => <div key={file.boHoSoTaiLieuId || file.id} className="flex items-center justify-between gap-3 px-4 py-3 text-sm"><div><div className="font-medium">{file.tenTaiLieu}</div><div className="text-xs text-gray-500">{file.tenFile} · Phiên bản {file.phienBan}</div></div><Badge size="sm" color={file.hinhThucThem === "KeThua" ? "light" : "warning"}>{file.hinhThucThem === "KeThua" ? "Kế thừa" : "Bổ sung"}</Badge></div>) : <div className="p-6 text-center text-sm text-gray-500">Chưa có tài liệu.</div>}</div></div>{isEditable && <div className="flex flex-wrap justify-end gap-3"><button onClick={validate} disabled={saving} className="rounded-lg border px-4 py-2.5 text-sm font-medium">Kiểm tra trước khi gửi</button><button onClick={send} disabled={saving || check?.dat === false} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">Gửi trình</button></div>}{check && !check.dat && <Alert variant="warning" title="Chưa thể gửi hồ sơ" message={check.dieuKienChuaDat?.join(" ")} />}</>}</div>;
}

