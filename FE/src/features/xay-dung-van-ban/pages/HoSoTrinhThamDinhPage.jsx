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
import { createHoSoTrinhThamDinh, getHoSoTrinhThamDinh, getHoSoXayDungVanBanById, getTaiLieuTrinhThamDinh, guiThamDinh, kiemTraGuiThamDinh, taiXuongTaiLieuTrinhThamDinh, updateHoSoTrinhThamDinh, uploadTaiLieuTrinhThamDinh } from "../api/xayDungVanBanApi";

const DEFAULT_PROCESSING_DAYS = 5;
const emptyForm = { buocQuyTrinhTiepTheoId: "", donViNhanThamDinhId: "", soToTrinh: "", ngayToTrinh: "", noiDungDeNghiThamDinh: "", ngayChuyen: "", soNgayXuLy: DEFAULT_PROCESSING_DAYS, hanDeNghiTraKetQua: "", soNgayCanhBao: 0, thoiGianCanhBao: "", noiDungGhiChu: "", trangThaiHoSoTiepTheoId: "", fileDuThaoId: "" };
const errorMessage = error => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể xử lý yêu cầu.";
const dateValue = value => value ? String(value).slice(0, 10) : "";
const todayValue = () => dateValue(new Date().toISOString());
const addDays = (startDate, days) => { const date = startDate ? new Date(startDate) : new Date(); date.setDate(date.getDate() + Math.max(0, Number(days) || 0)); return dateValue(date.toISOString()); };
const subtractDays = (startDate, days) => { const date = startDate ? new Date(startDate) : new Date(); date.setDate(date.getDate() - Math.max(0, Number(days) || 0)); return dateValue(date.toISOString()); };
const clampWarningDays = (value, processingDays) => Math.min(Math.max(0, Number(value) || 0), Math.max(1, Number(processingDays) || DEFAULT_PROCESSING_DAYS));
const deadlineDefaults = (step, startDate = todayValue()) => {
  const soNgayXuLy = Number(step?.soNgayXuLyTieuChuan) || DEFAULT_PROCESSING_DAYS;
  const soNgayCanhBao = clampWarningDays(step?.soNgayCanhBaoSapHan ?? 0, soNgayXuLy);
  const hanDeNghiTraKetQua = addDays(startDate, soNgayXuLy);
  return { ngayChuyen: startDate, soNgayXuLy, hanDeNghiTraKetQua, soNgayCanhBao, thoiGianCanhBao: subtractDays(hanDeNghiTraKetQua, soNgayCanhBao) };
};
const statusCodes = new Set(["CHO_THAM_DINH", "DANG_THAM_DINH"]);
const documentType = file => file.loaiDinhKem === "DU_THAO" ? { label: "File dự thảo", color: "primary" } : file.loaiDinhKem === "KE_THUA" || file.hinhThucThem === "KeThua" ? { label: "Tài liệu kế thừa", color: "light" } : { label: "Tài liệu kèm tờ trình", color: "warning" };
const submissionStatus = (status, soLanTraLai = 0) => status === "DaGui" ? { label: "Đã gửi", color: "success" } : status === "BiTraLai" ? { label: soLanTraLai > 0 ? `Bị trả lại lần ${soLanTraLai}` : "Bị trả lại", color: "error" } : { label: "Đang nhập", color: "warning" };

export default function HoSoTrinhThamDinhPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [hoSo, setHoSo] = useState(null), [workflow, setWorkflow] = useState(null), [donVis, setDonVis] = useState([]), [trangThais, setTrangThais] = useState([]);
  const [data, setData] = useState(null), [files, setFiles] = useState([]), [form, setForm] = useState(emptyForm), [uploadType, setUploadType] = useState("TO_TRINH");
  const [loading, setLoading] = useState(true), [saving, setSaving] = useState(false), [error, setError] = useState(""), [success, setSuccess] = useState(""), [checkResult, setCheckResult] = useState(null);
  const steps = useMemo(() => [...(workflow?.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0)), [workflow]);
  const stageStatuses = useMemo(() => { const active = trangThais.filter(x => x.trangThai); const matched = active.filter(x => statusCodes.has(x.maTrangThai)); return matched.length ? matched : active; }, [trangThais]);
  const isEditable = !data || data.trangThai === "Nhap" || data.trangThai === "BiTraLai";
  const currentStatus = data ? submissionStatus(data.trangThai, data.soLanTraLai) : null;
  const update = (key, value) => setForm(current => ({ ...current, [key]: value }));

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const detail = await getHoSoXayDungVanBanById(id);
      const [wf, unitResult, statusResult, submission] = await Promise.all([
        detail.quyTrinhSoanThaoId ? getQuyTrinhById(detail.quyTrinhSoanThaoId) : Promise.resolve(null),
        getDonVis({ pageSize: 200, pageCurrent: 1 }),
        getTrangThais({ nhomTrangThai: "HO_SO_XAY_DUNG_VAN_BAN", pageSize: 100, pageCurrent: 1 }),
        getHoSoTrinhThamDinh(id).catch(e => e?.response?.status === 404 ? null : Promise.reject(e)),
      ]);
      const sourceSteps = wf?.buocQuyTrinhs || [];
      const targetStep = submission ? sourceSteps.find(step => step.id === submission.buocQuyTrinhId) : sourceSteps[sourceSteps.findIndex(step => step.id === detail.buocHienTaiId) + 1];
      const defaults = deadlineDefaults(targetStep);
      setHoSo(detail); setWorkflow(wf); setDonVis(unitResult?.data || []); setTrangThais(statusResult?.data || []); setData(submission);
      if (submission) {
        const hanDeNghiTraKetQua = dateValue(submission.hanDeNghiTraKetQua) || defaults.hanDeNghiTraKetQua;
        setForm({ ...defaults, buocQuyTrinhTiepTheoId: submission.buocQuyTrinhId || "", donViNhanThamDinhId: submission.donViNhanThamDinhId || "", soToTrinh: submission.soToTrinh || "", ngayToTrinh: dateValue(submission.ngayToTrinh), noiDungDeNghiThamDinh: submission.noiDungDeNghiThamDinh || "", hanDeNghiTraKetQua, thoiGianCanhBao: subtractDays(hanDeNghiTraKetQua, defaults.soNgayCanhBao), noiDungGhiChu: submission.noiDungGhiChu || "", trangThaiHoSoTiepTheoId: detail.trangThaiHoSoId || "", fileDuThaoId: submission.fileDuThaoId || "" });
        setFiles(await getTaiLieuTrinhThamDinh(id));
      } else {
        setForm(current => ({ ...current, ...defaults, buocQuyTrinhTiepTheoId: targetStep?.id || "", donViNhanThamDinhId: targetStep?.donViTiepNhanMacDinhId || "" }));
      }
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => { void load(); }, [id]);

  const create = async () => {
    if (!form.buocQuyTrinhTiepTheoId || !form.donViNhanThamDinhId) { setError("Vui lòng chọn bước thẩm định và đơn vị nhận thẩm định."); return; }
    try {
      setSaving(true);
      const result = await createHoSoTrinhThamDinh({ hoSoId: id, buocQuyTrinhTiepTheoId: form.buocQuyTrinhTiepTheoId, donViNhanThamDinhId: form.donViNhanThamDinhId, noiDungGhiChu: form.noiDungGhiChu || null });
      setData(result); setFiles(await getTaiLieuTrinhThamDinh(id)); setSuccess("Đã lập hồ sơ trình thẩm định và kế thừa tài liệu từ hồ sơ soạn thảo.");
    } catch (e) { setError(errorMessage(e)); } finally { setSaving(false); }
  };
  const save = async () => {
    if (!data) return create();
    try {
      setSaving(true);
      setData(await updateHoSoTrinhThamDinh(id, { soToTrinh: form.soToTrinh || null, ngayToTrinh: form.ngayToTrinh || null, donViNhanThamDinhId: form.donViNhanThamDinhId, noiDungDeNghiThamDinh: form.noiDungDeNghiThamDinh || null, hanDeNghiTraKetQua: form.hanDeNghiTraKetQua || null, noiDungGhiChu: form.noiDungGhiChu || null }));
      setSuccess("Cập nhật hồ sơ trình thẩm định thành công.");
    } catch (e) { setError(errorMessage(e)); } finally { setSaving(false); }
  };
  const upload = async event => {
    const file = event.target.files?.[0];
    if (!file || !data) return;
    if (uploadType === "DU_THAO" && !file.name.toLowerCase().endsWith(".docx")) { setError("File dự thảo phải có định dạng .docx."); event.target.value = ""; return; }
    try {
      setSaving(true);
      const uploaded = await uploadTaiLieuTrinhThamDinh(id, { file, loaiTaiLieuId: crypto.randomUUID(), tenTaiLieu: file.name, loaiDinhKem: uploadType });
      if (uploadType === "DU_THAO") update("fileDuThaoId", uploaded.id);
      setFiles(await getTaiLieuTrinhThamDinh(id)); setSuccess(uploadType === "DU_THAO" ? "Đã tải và chốt file dự thảo cho lần gửi này." : "Đã thêm tài liệu kèm tờ trình.");
    } catch (e) { setError(errorMessage(e)); } finally { event.target.value = ""; setSaving(false); }
  };
  const download = async file => { try { setError(""); await taiXuongTaiLieuTrinhThamDinh(id, file.id, file.tenFile); } catch (e) { setError(errorMessage(e)); } };
  const check = async () => {
    if (!form.fileDuThaoId) { setCheckResult({ dat: false, dieuKienChuaDat: ["Chưa chốt file dự thảo .docx cho lần gửi này. Hãy tải file ở loại File dự thảo."] }); return; }
    try { const result = await kiemTraGuiThamDinh(id); setCheckResult(result); if (result.dat) setSuccess("Hồ sơ đã đáp ứng điều kiện gửi thẩm định."); } catch (e) { setError(errorMessage(e)); }
  };
  const send = async () => {
    if (!data || !form.trangThaiHoSoTiepTheoId || !form.fileDuThaoId) { setError("Vui lòng tải và chốt file dự thảo .docx mới, sau đó chọn trạng thái trước khi gửi."); return; }
    try {
      setSaving(true);
      await guiThamDinh(id, { buocQuyTrinhTiepTheoId: data.buocQuyTrinhId, trangThaiHoSoTiepTheoId: form.trangThaiHoSoTiepTheoId, fileDuThaoId: form.fileDuThaoId, hanXuLy: form.hanDeNghiTraKetQua || null, thoiGianCanhBao: form.thoiGianCanhBao || null, soNgayXuLy: Number(form.soNgayXuLy) || null, soNgayCanhBao: Number(form.soNgayCanhBao) || 0 });
      navigate("/xay-dung-van-ban/trinh-tham-dinh", { state: { success: "Đã gửi hồ sơ thẩm định." } });
    } catch (e) { setError(errorMessage(e)); } finally { setSaving(false); }
  };
  const updateStep = value => {
    const step = steps.find(item => item.id === value);
    const defaults = deadlineDefaults(step, form.ngayChuyen || todayValue());
    setForm(current => ({ ...current, ...defaults, buocQuyTrinhTiepTheoId: value, donViNhanThamDinhId: step?.donViTiepNhanMacDinhId || current.donViNhanThamDinhId }));
  };
  const updateNgayChuyen = value => setForm(current => { const hanDeNghiTraKetQua = addDays(value, current.soNgayXuLy); return { ...current, ngayChuyen: value, hanDeNghiTraKetQua, thoiGianCanhBao: subtractDays(hanDeNghiTraKetQua, current.soNgayCanhBao) }; });
  const updateSoNgayXuLy = value => setForm(current => { const soNgayXuLy = Number(value) || DEFAULT_PROCESSING_DAYS; const soNgayCanhBao = clampWarningDays(current.soNgayCanhBao, soNgayXuLy); const hanDeNghiTraKetQua = addDays(current.ngayChuyen, soNgayXuLy); return { ...current, soNgayXuLy, soNgayCanhBao, hanDeNghiTraKetQua, thoiGianCanhBao: subtractDays(hanDeNghiTraKetQua, soNgayCanhBao) }; });
  const updateSoNgayCanhBao = value => setForm(current => { const soNgayCanhBao = clampWarningDays(value, current.soNgayXuLy); return { ...current, soNgayCanhBao, thoiGianCanhBao: subtractDays(current.hanDeNghiTraKetQua, soNgayCanhBao) }; });

  return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Trình thẩm định</h1><p className="mt-1 text-sm text-gray-500">{hoSo ? `${hoSo.maHoSo} · ${hoSo.tenHoSo}` : "Lập và gửi hồ sơ đề nghị thẩm định."}</p></div><div className="flex gap-2"><button onClick={() => navigate("/xay-dung-van-ban/trinh-tham-dinh")} className="rounded-lg border px-4 py-2 text-sm">Danh sách</button><button onClick={() => navigate(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${id}`)} className="rounded-lg border px-4 py-2 text-sm">Xem hồ sơ</button></div></div>{error && <Alert variant="error" title="Có lỗi" message={error} />}{success && <Alert variant="success" title="Hoàn tất" message={success} />}{data?.lyDoTraLai && <Alert variant="warning" title={currentStatus?.label || "Bị trả lại"} message={`${data.lyDoTraLai} Vui lòng đính kèm file dự thảo lần ${(data.soLanTraLai || 0) + 1} trước khi gửi lại.`} />}{loading ? <div className="py-16 text-center text-sm text-gray-500">Đang tải hồ sơ...</div> : <><div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="mb-4 flex items-center justify-between"><h2 className="font-semibold">Hồ sơ trình thẩm định</h2>{currentStatus && <Badge size="sm" color={currentStatus.color}>{currentStatus.label}</Badge>}</div><div className="grid gap-4 sm:grid-cols-2"><div><Label>Bước thẩm định *</Label><Select value={form.buocQuyTrinhTiepTheoId} disabled={Boolean(data)} options={[{ value: "", label: "Chọn bước" }, ...steps.map(x => ({ value: x.id, label: x.tenBuoc }))]} onChange={updateStep} /></div><div><Label>Đơn vị nhận thẩm định *</Label><Select value={form.donViNhanThamDinhId} disabled={!isEditable} options={[{ value: "", label: "Chọn đơn vị" }, ...donVis.map(x => ({ value: x.id, label: x.tenDonVi || x.ten || x.maDonVi }))]} onChange={value => update("donViNhanThamDinhId", value)} /></div><div><Label>Số tờ trình</Label><Input value={form.soToTrinh} onChange={e => update("soToTrinh", e.target.value)} disabled={!data || !isEditable} /></div><div><Label>Ngày tờ trình</Label><Input type="date" value={form.ngayToTrinh} onChange={e => update("ngayToTrinh", e.target.value)} disabled={!data || !isEditable} /></div><div><Label>Ngày chuyển</Label><Input type="date" value={form.ngayChuyen} onChange={e => updateNgayChuyen(e.target.value)} disabled={!data || !isEditable} /></div><div><Label>Số ngày xử lý</Label><Input type="number" min="1" value={form.soNgayXuLy} onChange={e => updateSoNgayXuLy(e.target.value)} disabled={!data || !isEditable} /></div><div><Label>Hạn đề nghị trả kết quả</Label><Input type="date" value={form.hanDeNghiTraKetQua} onChange={e => setForm(current => ({ ...current, hanDeNghiTraKetQua: e.target.value, thoiGianCanhBao: subtractDays(e.target.value, current.soNgayCanhBao) }))} disabled={!data || !isEditable} /></div><div><Label>Số ngày cảnh báo</Label><Input type="number" min="0" max={form.soNgayXuLy || DEFAULT_PROCESSING_DAYS} value={form.soNgayCanhBao} onChange={e => updateSoNgayCanhBao(e.target.value)} disabled={!data || !isEditable} /></div><div><Label>Thời gian cảnh báo</Label><Input type="date" value={form.thoiGianCanhBao} onChange={e => update("thoiGianCanhBao", e.target.value)} disabled={!data || !isEditable} /></div><div><Label>Trạng thái sau khi gửi *</Label><Select value={form.trangThaiHoSoTiepTheoId} options={[{ value: "", label: "Chọn trạng thái thẩm định" }, ...stageStatuses.map(x => ({ value: x.id, label: x.tenTrangThai || x.maTrangThai }))]} onChange={value => update("trangThaiHoSoTiepTheoId", value)} disabled={!data || !isEditable} /></div><div className="sm:col-span-2"><Label>Nội dung đề nghị thẩm định *</Label><TextArea rows={4} value={form.noiDungDeNghiThamDinh} onChange={value => update("noiDungDeNghiThamDinh", value)} disabled={!data || !isEditable} /></div><div className="sm:col-span-2"><Label>Ghi chú</Label><TextArea rows={2} value={form.noiDungGhiChu} onChange={value => update("noiDungGhiChu", value)} disabled={!isEditable} /></div></div>{isEditable && <div className="mt-5 flex flex-wrap justify-end gap-3"><button onClick={save} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">{data ? "Lưu cập nhật" : "Lập hồ sơ và kế thừa tài liệu"}</button></div>}</div>{data && <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="mb-4 flex flex-wrap items-end justify-between gap-3"><div><h2 className="font-semibold">Tài liệu trình thẩm định</h2><p className="text-sm text-gray-500">Hiển thị riêng tài liệu kế thừa, tài liệu kèm tờ trình và file dự thảo dùng để gửi thẩm định.</p></div>{isEditable && <div className="flex flex-wrap items-end gap-2"><div className="min-w-52"><Label>Loại file tải lên</Label><Select value={uploadType} options={[{ value: "TO_TRINH", label: "Tài liệu kèm tờ trình" }, { value: "DU_THAO", label: "File dự thảo (.docx)" }]} onChange={setUploadType} /></div><label className="cursor-pointer rounded-lg border px-4 py-2.5 text-sm font-medium"><input type="file" className="hidden" onChange={upload} disabled={saving} />Tải file</label></div>}</div><div className="divide-y rounded-lg border dark:border-white/[0.05]">{files.length ? files.map(file => { const type = documentType(file); return <div key={file.id} className="flex flex-wrap items-center justify-between gap-3 px-4 py-3 text-sm"><div><div className="font-medium">{file.tenTaiLieu}</div><div className="text-xs text-gray-500">{file.tenFile} · Phiên bản {file.phienBan}</div></div><div className="flex items-center gap-2"><Badge size="sm" color={type.color}>{type.label}</Badge><button onClick={() => download(file)} className="rounded-md border px-3 py-1.5 text-xs font-medium">Tải xuống</button></div></div>; }) : <div className="p-6 text-center text-sm text-gray-500">Chưa có tài liệu.</div>}</div></div>}{data && isEditable && <div className="flex flex-wrap justify-end gap-3"><button onClick={check} disabled={saving} className="rounded-lg border px-4 py-2.5 text-sm font-medium">Kiểm tra trước khi gửi</button><button onClick={send} disabled={saving || checkResult?.dat === false} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">Gửi thẩm định</button></div>}{checkResult && !checkResult.dat && <Alert variant="warning" title="Chưa thể gửi hồ sơ" message={checkResult.dieuKienChuaDat?.join(" ")} />}</>}</div>;
}
