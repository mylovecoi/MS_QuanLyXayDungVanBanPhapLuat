import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import TextArea from "../../../app/components/forms/input/TextArea";
import { getDonVis } from "../../danh-muc/api/donViApi";
import { getHoSoXayDungVanBanById, getHoSoBanHanh, getTaiLieuBanHanh, hoanThanhBanHanh, kiemTraHoanThanhBanHanh, updateHoSoBanHanh, uploadTaiLieuBanHanh } from "../api/xayDungVanBanApi";

const emptyForm = { ketQua: "BAN_HANH", soVanBan: "", ngayBanHanh: "", coQuanBanHanhId: "", nguoiKyId: "", chucVuNguoiKy: "", ngayCoHieuLuc: "", noiDungKetQua: "", lyDoKhongThongQua: "" };
const errorMessage = error => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể xử lý yêu cầu.";
const dateValue = value => value ? String(value).slice(0, 10) : "";
const statusInfo = status => status === "DaHoanThanh" ? { label: "Đã ban hành", color: "success" } : status === "Nhap" ? { label: "Đang cập nhật", color: "warning" } : { label: status || "-", color: "light" };

export default function HoSoBanHanhPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [hoSo, setHoSo] = useState(null), [data, setData] = useState(null), [files, setFiles] = useState([]), [donVis, setDonVis] = useState([]);
  const [form, setForm] = useState(emptyForm), [check, setCheck] = useState(null), [loading, setLoading] = useState(true), [saving, setSaving] = useState(false), [error, setError] = useState(""), [success, setSuccess] = useState("");
  const isEditable = data?.trangThai === "Nhap";
  const update = (key, value) => setForm(current => ({ ...current, [key]: value }));
  const load = async () => {
    try {
      setLoading(true); setError("");
      const [detail, banHanh, units] = await Promise.all([getHoSoXayDungVanBanById(id), getHoSoBanHanh(id), getDonVis({ pageSize: 200, pageCurrent: 1 })]);
      setHoSo(detail); setData(banHanh); setDonVis(units?.data || []);
      setForm({ ketQua: banHanh.ketQua || "BAN_HANH", soVanBan: banHanh.soVanBan || "", ngayBanHanh: dateValue(banHanh.ngayBanHanh), coQuanBanHanhId: banHanh.coQuanBanHanhId || detail.donViChuTriSoanThaoId || "", nguoiKyId: banHanh.nguoiKyId || "", chucVuNguoiKy: banHanh.chucVuNguoiKy || "", ngayCoHieuLuc: dateValue(banHanh.ngayCoHieuLuc), noiDungKetQua: banHanh.noiDungKetQua || "", lyDoKhongThongQua: banHanh.lyDoKhongThongQua || "" });
      setFiles(await getTaiLieuBanHanh(id));
    } catch (e) { setError(errorMessage(e)); } finally { setLoading(false); }
  };
  useEffect(() => { void load(); }, [id]);
  const save = async () => {
    try {
      setSaving(true); setError(""); setSuccess("");
      const result = await updateHoSoBanHanh(id, { ...form, coQuanBanHanhId: form.coQuanBanHanhId || null, nguoiKyId: form.nguoiKyId || null, ngayBanHanh: form.ngayBanHanh || null, ngayCoHieuLuc: form.ngayCoHieuLuc || null });
      setData(result); setSuccess("Đã lưu kết quả ban hành.");
    } catch (e) { setError(errorMessage(e)); } finally { setSaving(false); }
  };
  const upload = async event => {
    const file = event.target.files?.[0];
    if (!file) return;
    try {
      setSaving(true); setError(""); setSuccess("");
      await uploadTaiLieuBanHanh(id, { file, loaiTaiLieuId: crypto.randomUUID(), tenTaiLieu: file.name });
      setFiles(await getTaiLieuBanHanh(id)); setSuccess("Đã tải tài liệu ban hành.");
    } catch (e) { setError(errorMessage(e)); } finally { event.target.value = ""; setSaving(false); }
  };
  const validate = async () => {
    try { const result = await kiemTraHoanThanhBanHanh(id); setCheck(result); if (result.dat) setSuccess("Hồ sơ đủ điều kiện hoàn thành ban hành."); } catch (e) { setError(errorMessage(e)); }
  };
  const complete = async () => {
    try { setSaving(true); await hoanThanhBanHanh(id); navigate("/xay-dung-van-ban/ban-hanh", { state: { success: "Đã hoàn thành ban hành." } }); } catch (e) { setError(errorMessage(e)); } finally { setSaving(false); }
  };
  const status = statusInfo(data?.trangThai);

  return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Cập nhật ban hành</h1><p className="mt-1 text-sm text-gray-500">{hoSo ? `${hoSo.maHoSo} · ${hoSo.tenHoSo}` : "Cập nhật kết quả và tài liệu ban hành."}</p></div><div className="flex gap-2"><button onClick={() => navigate("/xay-dung-van-ban/ban-hanh")} className="rounded-lg border px-4 py-2 text-sm">Danh sách</button><button onClick={() => window.open(`/admin/xay-dung-van-ban/ho-so/chi-tiet/${id}`, "_blank", "noopener,noreferrer")} className="rounded-lg border px-4 py-2 text-sm">Xem hồ sơ</button></div></div>{error && <Alert variant="error" title="Có lỗi" message={error} />}{success && <Alert variant="success" title="Hoàn tất" message={success} />}{loading ? <div className="py-16 text-center text-sm text-gray-500">Đang tải hồ sơ...</div> : <><div className="rounded-xl border bg-white p-5"><div className="mb-4 flex items-center justify-between"><h2 className="font-semibold">Kết quả ban hành</h2><Badge size="sm" color={status.color}>{status.label}</Badge></div><div className="grid gap-4 sm:grid-cols-2"><div><Label>Kết quả *</Label><Select value={form.ketQua} onChange={value => update("ketQua", value)} disabled={!isEditable} options={[{ value: "BAN_HANH", label: "Ban hành" }, { value: "KHONG_BAN_HANH", label: "Không ban hành" }]} /></div><div><Label>Cơ quan ban hành</Label><Select value={form.coQuanBanHanhId} onChange={value => update("coQuanBanHanhId", value)} disabled={!isEditable} options={[{ value: "", label: "Chọn cơ quan" }, ...donVis.map(x => ({ value: x.id, label: x.tenDonVi || x.ten || x.maDonVi }))]} /></div><div><Label>Số văn bản</Label><Input value={form.soVanBan} onChange={e => update("soVanBan", e.target.value)} disabled={!isEditable} /></div><div><Label>Ngày ban hành</Label><Input type="date" value={form.ngayBanHanh} onChange={e => update("ngayBanHanh", e.target.value)} disabled={!isEditable} /></div><div><Label>Ngày có hiệu lực</Label><Input type="date" value={form.ngayCoHieuLuc} onChange={e => update("ngayCoHieuLuc", e.target.value)} disabled={!isEditable} /></div><div><Label>Chức vụ người ký</Label><Input value={form.chucVuNguoiKy} onChange={e => update("chucVuNguoiKy", e.target.value)} disabled={!isEditable} /></div><div className="sm:col-span-2"><Label>Nội dung kết quả</Label><TextArea rows={3} value={form.noiDungKetQua} onChange={value => update("noiDungKetQua", value)} disabled={!isEditable} /></div><div className="sm:col-span-2"><Label>Lý do không ban hành</Label><TextArea rows={3} value={form.lyDoKhongThongQua} onChange={value => update("lyDoKhongThongQua", value)} disabled={!isEditable || form.ketQua !== "KHONG_BAN_HANH"} /></div></div>{isEditable && <div className="mt-5 flex flex-wrap justify-end gap-3"><button onClick={save} disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">Lưu kết quả</button></div>}</div><div className="rounded-xl border bg-white p-5"><div className="mb-4 flex flex-wrap items-end justify-between gap-3"><div><h2 className="font-semibold">Tài liệu ban hành</h2><p className="text-sm text-gray-500">Tải văn bản ban hành hoặc tài liệu kết quả kèm theo.</p></div>{isEditable && <label className="cursor-pointer rounded-lg border px-4 py-2.5 text-sm font-medium"><input type="file" className="hidden" onChange={upload} disabled={saving} />Tải file</label>}</div><div className="divide-y rounded-lg border">{files.length ? files.map(file => <div key={file.id} className="flex flex-wrap items-center justify-between gap-3 px-4 py-3 text-sm"><div><div className="font-medium">{file.tenTaiLieu}</div><div className="text-xs text-gray-500">{file.tenFile} · Phiên bản {file.phienBan}</div></div></div>) : <div className="p-6 text-center text-sm text-gray-500">Chưa có tài liệu.</div>}</div></div>{isEditable && <div className="flex flex-wrap justify-end gap-3"><button onClick={validate} disabled={saving} className="rounded-lg border px-4 py-2.5 text-sm font-medium">Kiểm tra trước khi hoàn thành</button><button onClick={complete} disabled={saving || check?.dat === false} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">Hoàn thành ban hành</button></div>}{check && !check.dat && <Alert variant="warning" title="Chưa thể hoàn thành" message={check.dieuKienChuaDat?.join(" ")} />}</>}</div>;
}
