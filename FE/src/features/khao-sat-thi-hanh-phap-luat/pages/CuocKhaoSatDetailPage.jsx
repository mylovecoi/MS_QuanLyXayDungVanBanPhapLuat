import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import Input from "../../../app/components/forms/input/InputField";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import { Modal } from "../../../app/components/ui/modal";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import {
  createDoiTuongKhaoSat,
  createNhomDoiTuongKhaoSat,
  getCauHoiMauPhieu,
  getCuocKhaoSat,
  uploadMauPhieu,
} from "../api/khaoSatThiHanhPhapLuatApi";

const today = () => new Date().toISOString().slice(0, 10);
const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const errorMessage = (error) => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể xử lý yêu cầu.";
const emptyGroup = () => ({ maNhom: "", tenNhom: "", thuTu: "1" });
const emptySubject = () => ({ nhomDoiTuongKhaoSatId: "", mauPhieuKhaoSatId: "", donViId: "", canBoId: "", hanNop: today(), trangThaiId: "" });

export default function CuocKhaoSatDetailPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const [data, setData] = useState(null);
  const [statuses, setStatuses] = useState([]);
  const [units, setUnits] = useState([]);
  const [tab, setTab] = useState("thong-tin");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [selectedTemplateId, setSelectedTemplateId] = useState("");
  const [questions, setQuestions] = useState([]);
  const [groupId, setGroupId] = useState("");
  const [version, setVersion] = useState(1);
  const [file, setFile] = useState(null);
  const [groupOpen, setGroupOpen] = useState(false);
  const [groupForm, setGroupForm] = useState(emptyGroup());
  const [subjectOpen, setSubjectOpen] = useState(false);
  const [subjectForm, setSubjectForm] = useState(emptySubject());

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const result = await getCuocKhaoSat(id);
      setData(result);
      setSelectedTemplateId((current) => current || result.mauPhieu?.[0]?.id || "");
      setGroupId((current) => current || result.nhomDoiTuong?.[0]?.id || "");
    } catch (requestError) {
      setError(errorMessage(requestError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
    void Promise.all([
      getTrangThais({ nhomTrangThai: "DOI_TUONG_KHAO_SAT", pageSize: 100, pageCurrent: 1 }),
      getDonViOptions(),
    ]).then(([statusResult, unitResult]) => {
      setStatuses(statusResult?.data || []);
      setUnits(unitResult || []);
    }).catch((requestError) => setError(errorMessage(requestError)));
  }, [id]);

  useEffect(() => {
    if (!selectedTemplateId) {
      setQuestions([]);
      return;
    }
    getCauHoiMauPhieu(selectedTemplateId).then(setQuestions).catch((requestError) => setError(errorMessage(requestError)));
  }, [selectedTemplateId]);

  const survey = data?.cuocKhaoSat;
  const templates = data?.mauPhieu || [];
  const groups = data?.nhomDoiTuong || [];
  const subjects = data?.donViKhaoSat || [];
  const groupMap = useMemo(() => new Map(groups.map((item) => [item.id, item])), [groups]);
  const unitMap = useMemo(() => new Map(units.map((item) => [item.id, item.tenDonVi || item.ten || item.maDonVi])), [units]);
  const templateMap = useMemo(() => new Map(templates.map((item) => [item.id, item])), [templates]);
  const tabs = [
    { id: "thong-tin", label: "Thông tin cơ bản" },
    { id: "mau-phieu", label: `Mẫu phiếu (${templates.length})` },
    { id: "don-vi", label: `Đơn vị khảo sát (${subjects.length})` },
  ];

  const groupOptions = [{ value: "", label: "Chọn nhóm đối tượng" }, ...groups.map((item) => ({ value: item.id, label: `${item.maNhom} · ${item.tenNhom}` }))];
  const unitOptions = [{ value: "", label: "Chọn đơn vị khảo sát" }, ...units.map((item) => ({ value: item.id, label: item.tenDonVi || item.ten || item.maDonVi }))];
  const statusOptions = [{ value: "", label: "Chọn trạng thái" }, ...statuses.map((item) => ({ value: item.id, label: item.tenTrangThai || item.maTrangThai }))];
  const subjectTemplateOptions = [{ value: "", label: "Tự chọn mẫu mới nhất theo nhóm" }, ...templates.filter((item) => !subjectForm.nhomDoiTuongKhaoSatId || item.nhomDoiTuongKhaoSatId === subjectForm.nhomDoiTuongKhaoSatId).map((item) => ({ value: item.id, label: `${groupMap.get(item.nhomDoiTuongKhaoSatId)?.maNhom || "-"} · ${item.tenFile} · V${item.phienBan}` }))];

  const updateGroup = (value) => {
    setGroupId(value);
    const latest = templates.filter((item) => item.nhomDoiTuongKhaoSatId === value).reduce((max, item) => Math.max(max, item.phienBan || 0), 0);
    setVersion(latest + 1 || 1);
  };

  const openSubject = () => {
    const statusId = statuses.find((item) => item.maTrangThai === "CHUA_NOP")?.id || "";
    const firstGroupId = groups[0]?.id || "";
    const firstTemplateId = templates.find((item) => item.nhomDoiTuongKhaoSatId === firstGroupId)?.id || "";
    setSubjectForm({ ...emptySubject(), nhomDoiTuongKhaoSatId: firstGroupId, mauPhieuKhaoSatId: firstTemplateId, trangThaiId: statusId });
    setError("");
    setSuccess("");
    setSubjectOpen(true);
  };

  const updateSubjectGroup = (value) => {
    const latestTemplateId = templates.filter((item) => item.nhomDoiTuongKhaoSatId === value).sort((a, b) => (b.phienBan || 0) - (a.phienBan || 0))[0]?.id || "";
    setSubjectForm((current) => ({ ...current, nhomDoiTuongKhaoSatId: value, mauPhieuKhaoSatId: latestTemplateId }));
  };

  const createGroup = async () => {
    if (!groupForm.maNhom.trim() || !groupForm.tenNhom.trim()) {
      setError("Nhập mã nhóm và tên nhóm đối tượng.");
      return;
    }
    try {
      setSaving(true);
      setError("");
      const result = await createNhomDoiTuongKhaoSat(id, { maNhom: groupForm.maNhom.trim(), tenNhom: groupForm.tenNhom.trim(), thuTu: Number(groupForm.thuTu) || 1 });
      setGroupOpen(false);
      setSuccess("Đã thêm nhóm đối tượng khảo sát.");
      await load();
      if (result?.id) updateGroup(result.id);
    } catch (requestError) {
      setError(errorMessage(requestError));
    } finally {
      setSaving(false);
    }
  };

  const createSubject = async () => {
    if (!subjectForm.nhomDoiTuongKhaoSatId || !subjectForm.donViId || !subjectForm.hanNop || !subjectForm.trangThaiId) {
      setError("Chọn nhóm đối tượng, đơn vị khảo sát, hạn nộp và trạng thái.");
      return;
    }
    try {
      setSaving(true);
      setError("");
      await createDoiTuongKhaoSat(id, {
        nhomDoiTuongKhaoSatId: subjectForm.nhomDoiTuongKhaoSatId,
        mauPhieuKhaoSatId: subjectForm.mauPhieuKhaoSatId || null,
        donViId: subjectForm.donViId,
        canBoId: subjectForm.canBoId || null,
        hanNop: subjectForm.hanNop,
        trangThaiId: subjectForm.trangThaiId,
      });
      setSubjectOpen(false);
      setSuccess("Đã phân công đơn vị khảo sát.");
      await load();
    } catch (requestError) {
      setError(errorMessage(requestError));
    } finally {
      setSaving(false);
    }
  };

  const upload = async () => {
    if (!groupId || !file) {
      setError("Chọn nhóm đối tượng và file Word mẫu phiếu.");
      return;
    }
    if (!file.name.toLowerCase().endsWith(".docx")) {
      setError("Chỉ nhận file Word .docx.");
      return;
    }
    try {
      setUploading(true);
      setError("");
      setSuccess("");
      const result = await uploadMauPhieu({ cuocKhaoSatId: id, nhomDoiTuongId: groupId, phienBan: Number(version), file });
      setSuccess(`Đã lưu file và đọc ${result.soCauHoi || 0} câu hỏi (${result.soDongDocDuoc || 0} dòng).`);
      setFile(null);
      await load();
      setSelectedTemplateId(result.id);
    } catch (requestError) {
      setError(errorMessage(requestError));
    } finally {
      setUploading(false);
    }
  };

  return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Chi tiết cuộc khảo sát</h1><p className="mt-1 text-sm text-gray-500">{survey ? `${survey.maCuocKhaoSat} · ${survey.tenCuocKhaoSat}` : "Quản lý thông tin, mẫu phiếu và đơn vị khảo sát."}</p></div><button type="button" onClick={() => navigate("/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat")} className="rounded-lg border px-4 py-2 text-sm font-medium">Danh sách cuộc khảo sát</button></div>{error && <Alert variant="error" title="Có lỗi" message={error} />}{success && <Alert variant="success" title="Hoàn tất" message={success} />}{loading ? <div className="py-16 text-center text-sm text-gray-500">Đang tải dữ liệu...</div> : data && <><div className="flex flex-wrap gap-2 border-b border-gray-200 dark:border-white/[0.05]">{tabs.map((item) => <button type="button" key={item.id} onClick={() => setTab(item.id)} className={`border-b-2 px-4 py-3 text-sm font-medium ${tab === item.id ? "border-brand-500 text-brand-500" : "border-transparent text-gray-500 hover:text-gray-800"}`}>{item.label}</button>)}</div>{tab === "thong-tin" && <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="grid gap-5 sm:grid-cols-2"><Info label="Mã cuộc khảo sát" value={survey.maCuocKhaoSat} /><Info label="Đơn vị chủ trì" value={survey.donViChuTriId} /><Info label="Thời gian" value={`${formatDate(survey.tuNgay)} - ${formatDate(survey.denNgay)}`} /><Info label="Phạm vi" value={survey.phamVi} /><Info label="Mục đích" value={survey.mucDich} wide /></div></div>}{tab === "mau-phieu" && <div className="space-y-5"><div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="mb-4 flex flex-wrap items-start justify-between gap-3"><div><h2 className="font-semibold text-gray-800 dark:text-white/90">Tải file mẫu phiếu</h2><p className="mt-1 text-sm text-gray-500">File .docx sẽ được đọc để sinh câu hỏi và các lựa chọn trả lời.</p></div><button type="button" onClick={() => { setGroupForm(emptyGroup()); setGroupOpen(true); }} className="rounded-lg border px-4 py-2 text-sm font-medium">+ Nhóm đối tượng</button></div><div className="grid gap-4 lg:grid-cols-[minmax(0,1fr)_160px_minmax(0,1fr)]"><div><Label>Nhóm đối tượng *</Label><Select value={groupId} options={groupOptions} onChange={updateGroup} /></div><div><Label>Phiên bản *</Label><Input type="number" min="1" value={version} onChange={(event) => setVersion(event.target.value)} /></div><div><Label>File Word (.docx) *</Label><input type="file" accept=".docx,application/vnd.openxmlformats-officedocument.wordprocessingml.document" onChange={(event) => setFile(event.target.files?.[0] || null)} className="block h-11 w-full rounded-lg border border-[var(--admin-border)] px-3 py-2 text-sm" /></div></div><div className="mt-4 flex justify-end"><button type="button" onClick={upload} disabled={uploading || !groupId || !file} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">{uploading ? "Đang đọc và lưu file..." : "Tải file và đọc dữ liệu"}</button></div></div><div className="grid gap-5 xl:grid-cols-[minmax(320px,0.85fr)_minmax(0,1.4fr)]"><div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="border-b px-5 py-4"><h2 className="font-semibold">Các file đã lưu</h2></div><div className="divide-y">{!templates.length ? <div className="p-8 text-center text-sm text-gray-500">Chưa có mẫu phiếu được tải lên.</div> : templates.map((item) => <button type="button" key={item.id} onClick={() => setSelectedTemplateId(item.id)} className={`w-full px-5 py-4 text-left hover:bg-gray-50 dark:hover:bg-white/[0.02] ${selectedTemplateId === item.id ? "bg-brand-50/60 dark:bg-white/[0.04]" : ""}`}><div className="font-medium text-gray-800 dark:text-white/90">{item.tenFile}</div><div className="mt-1 text-xs text-gray-500">{groupMap.get(item.nhomDoiTuongKhaoSatId)?.maNhom || "-"} · {groupMap.get(item.nhomDoiTuongKhaoSatId)?.tenNhom || "Chưa xác định nhóm"}</div><div className="mt-2 flex items-center justify-between"><Badge size="sm" color={item.daPhatHanh ? "success" : "warning"}>{item.daPhatHanh ? "Đã phát hành" : item.trangThaiMauPhieu || "Đã đọc dữ liệu"}</Badge><span className="text-xs text-gray-500">V{item.phienBan} · {formatDate(item.createdAt)}</span></div></button>)}</div></div><div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="border-b px-5 py-4"><h2 className="font-semibold">Câu hỏi và câu trả lời đã đọc</h2><p className="mt-1 text-sm text-gray-500">Dữ liệu này được sinh tự động từ file mẫu Word vừa tải lên.</p></div><div className="max-w-full overflow-x-auto"><table className="w-full"><thead><tr className="border-b border-gray-100 dark:border-white/[0.05]"><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Mã / nội dung câu hỏi</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Cấu hình</th><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Câu trả lời</th></tr></thead><tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">{!selectedTemplateId ? <tr><td colSpan="3" className="px-5 py-10 text-center text-sm text-gray-500">Tải file mẫu hoặc chọn một file đã lưu để xem câu hỏi.</td></tr> : !questions.length ? <tr><td colSpan="3" className="px-5 py-10 text-center text-sm text-gray-500">Không đọc được câu hỏi nào từ file.</td></tr> : questions.map((item) => <tr key={item.id} className="align-top"><td className="px-5 py-4"><div className="font-medium text-gray-800 dark:text-white/90">{item.maCauHoi}</div><div className="mt-1 text-sm text-gray-600 dark:text-gray-300">{item.noiDung}</div></td><td className="px-5 py-4 text-center text-xs text-gray-500"><div>{item.loaiCauHoi}</div><div className="mt-1">{item.batBuoc ? "Bắt buộc" : "Không bắt buộc"} · {item.choPhepNhieuLuaChon ? "Nhiều lựa chọn" : "Một lựa chọn"}</div></td><td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.luaChon?.length ? item.luaChon.map((option) => <div key={option.maLuaChon} className="mb-1"><span className="font-medium">{option.maLuaChon}:</span> {option.noiDung}</div>) : "Không có câu trả lời cố định"}</td></tr>)}</tbody></table></div></div></div></div>}{tab === "don-vi" && <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="flex flex-wrap items-center justify-between gap-3 border-b px-5 py-4"><div><h2 className="font-semibold">Đơn vị khảo sát</h2><p className="mt-1 text-sm text-gray-500">Phân công đơn vị nhận mẫu phiếu và hạn nộp kết quả.</p></div><button type="button" onClick={openSubject} disabled={!groups.length || !templates.length} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">+ Phân công đơn vị</button></div><div className="max-w-full overflow-x-auto"><table className="w-full"><thead><tr className="border-b border-gray-100 dark:border-white/[0.05]"><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Đơn vị / nhóm đối tượng</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Hạn nộp</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Kết quả</th><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">File gần nhất</th></tr></thead><tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">{!subjects.length ? <tr><td colSpan="4" className="px-5 py-10 text-center text-sm text-gray-500">{!groups.length ? "Chưa có nhóm đối tượng. Hãy tạo nhóm và tải mẫu phiếu trước." : !templates.length ? "Chưa có mẫu phiếu. Hãy tải mẫu phiếu trước khi phân công đơn vị." : "Chưa phân công đơn vị khảo sát."}</td></tr> : subjects.map((item) => <tr key={item.id}><td className="px-5 py-4"><div className="font-medium text-gray-800 dark:text-white/90">{unitMap.get(item.donViId) || `Đơn vị: ${item.donViId}`}</div><div className="mt-1 text-xs text-gray-500">{groupMap.get(item.nhomDoiTuongKhaoSatId)?.tenNhom || "Chưa xác định nhóm"}{item.mauPhieuKhaoSatId ? ` · ${templateMap.get(item.mauPhieuKhaoSatId)?.tenFile || "Mẫu phiếu"}` : ""}</div></td><td className="px-5 py-4 text-center text-sm">{formatDate(item.hanNop)}</td><td className="px-5 py-4 text-center"><Badge size="sm" color={item.phieuGanNhat?.soLoi > 0 ? "error" : item.phieuGanNhat ? "success" : "light"}>{item.phieuGanNhat ? item.phieuGanNhat.soLoi > 0 ? `${item.phieuGanNhat.soLoi} lỗi` : "Đã nộp" : "Chưa nộp"}</Badge></td><td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.phieuGanNhat ? <><div>{item.phieuGanNhat.tenFile}</div><div className="mt-1 text-xs text-gray-500">{formatDate(item.phieuGanNhat.ngayImport)}</div></> : "-"}</td></tr>)}</tbody></table></div></div>}</>}<Modal isOpen={groupOpen} onClose={() => !saving && setGroupOpen(false)} className="max-w-lg p-6"><div className="space-y-4"><div><h2 className="text-lg font-semibold">Thêm nhóm đối tượng</h2><p className="mt-1 text-sm text-gray-500">Mẫu phiếu cần gắn với một nhóm đối tượng khảo sát.</p></div><div className="grid gap-4 sm:grid-cols-2"><div><Label>Mã nhóm *</Label><Input value={groupForm.maNhom} onChange={(event) => setGroupForm({ ...groupForm, maNhom: event.target.value })} /></div><div><Label>Thứ tự</Label><Input type="number" min="1" value={groupForm.thuTu} onChange={(event) => setGroupForm({ ...groupForm, thuTu: event.target.value })} /></div><div className="sm:col-span-2"><Label>Tên nhóm *</Label><Input value={groupForm.tenNhom} onChange={(event) => setGroupForm({ ...groupForm, tenNhom: event.target.value })} /></div></div><div className="flex justify-end gap-3"><button type="button" onClick={() => setGroupOpen(false)} disabled={saving} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Hủy</button><button type="button" onClick={createGroup} disabled={saving || !groupForm.maNhom.trim() || !groupForm.tenNhom.trim()} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">{saving ? "Đang lưu..." : "Lưu nhóm"}</button></div></div></Modal><Modal isOpen={subjectOpen} onClose={() => !saving && setSubjectOpen(false)} className="max-w-2xl p-6"><div className="space-y-4"><div><h2 className="text-lg font-semibold">Phân công đơn vị khảo sát</h2><p className="mt-1 text-sm text-gray-500">Mỗi đơn vị chỉ được phân công một lần trong cùng cuộc khảo sát.</p></div><div className="grid gap-4 sm:grid-cols-2"><div><Label>Nhóm đối tượng *</Label><Select value={subjectForm.nhomDoiTuongKhaoSatId} options={groupOptions} onChange={updateSubjectGroup} /></div><div><Label>Mẫu phiếu</Label><Select value={subjectForm.mauPhieuKhaoSatId} options={subjectTemplateOptions} onChange={(value) => setSubjectForm({ ...subjectForm, mauPhieuKhaoSatId: value })} /></div><div className="sm:col-span-2"><Label>Đơn vị khảo sát *</Label><Select value={subjectForm.donViId} options={unitOptions} onChange={(value) => setSubjectForm({ ...subjectForm, donViId: value })} /></div><div><Label>Hạn nộp *</Label><Input type="date" value={subjectForm.hanNop} onChange={(event) => setSubjectForm({ ...subjectForm, hanNop: event.target.value })} /></div><div><Label>Trạng thái *</Label><Select value={subjectForm.trangThaiId} options={statusOptions} onChange={(value) => setSubjectForm({ ...subjectForm, trangThaiId: value })} /></div><div className="sm:col-span-2"><Label>Cán bộ phụ trách</Label><Input value={subjectForm.canBoId} onChange={(event) => setSubjectForm({ ...subjectForm, canBoId: event.target.value })} placeholder="Nhập GUID cán bộ nếu cần" /></div></div><div className="flex justify-end gap-3"><button type="button" onClick={() => setSubjectOpen(false)} disabled={saving} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Hủy</button><button type="button" onClick={createSubject} disabled={saving || !subjectForm.nhomDoiTuongKhaoSatId || !subjectForm.donViId || !subjectForm.hanNop || !subjectForm.trangThaiId} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">{saving ? "Đang lưu..." : "Lưu phân công"}</button></div></div></Modal></div>;
}

function Info({ label, value, wide = false }) {
  return <div className={wide ? "sm:col-span-2" : ""}><div className="text-xs text-gray-500">{label}</div><div className="mt-1 whitespace-pre-line text-sm font-medium text-gray-800 dark:text-white/90">{value || "-"}</div></div>;
}
