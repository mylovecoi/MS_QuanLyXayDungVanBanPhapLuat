import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import { Modal } from "../../../app/components/ui/modal";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { createCuocKhaoSat, getCuocKhaoSats } from "../api/khaoSatThiHanhPhapLuatApi";

const today = () => new Date().toISOString().slice(0, 10);
const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const errorMessage = (error) => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể xử lý yêu cầu.";
const emptyForm = () => ({
  maCuocKhaoSat: "",
  tenCuocKhaoSat: "",
  mucDich: "",
  phamVi: "",
  donViChuTriId: "",
  tuNgay: today(),
  denNgay: today(),
  trangThaiId: "",
  linhVucId: null,
  vanBanId: null,
});

export default function CuocKhaoSatListPage() {
  const navigate = useNavigate();
  const [items, setItems] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [units, setUnits] = useState([]);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(emptyForm());

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const data = await getCuocKhaoSats({ keyword });
      setItems(data.items || []);
    } catch (requestError) {
      setError(errorMessage(requestError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
    void Promise.all([
      getTrangThais({ nhomTrangThai: "CUOC_KHAO_SAT_THI_HANH_PHAP_LUAT", pageSize: 100, pageCurrent: 1 }),
      getDonViOptions(),
    ]).then(([statusResult, unitResult]) => {
      setStatuses(statusResult?.data || []);
      setUnits(unitResult || []);
    }).catch((requestError) => setError(errorMessage(requestError)));
  }, []);

  const shown = useMemo(() => {
    const q = keyword.trim().toLocaleLowerCase("vi-VN");
    return q ? items.filter((item) => [item.maCuocKhaoSat, item.tenCuocKhaoSat].some((value) => value?.toLocaleLowerCase("vi-VN").includes(q))) : items;
  }, [items, keyword]);

  const openCreate = () => {
    const draftStatusId = statuses.find((item) => item.maTrangThai === "NHAP")?.id || "";
    setForm({ ...emptyForm(), trangThaiId: draftStatusId });
    setSuccess("");
    setError("");
    setOpen(true);
  };

  const update = (field, value) => setForm((current) => ({ ...current, [field]: value }));

  const create = async () => {
    if (form.tuNgay > form.denNgay) {
      setError("Thời gian khảo sát không hợp lệ.");
      return;
    }

    try {
      setSaving(true);
      setError("");
      const result = await createCuocKhaoSat({
        ...form,
        maCuocKhaoSat: form.maCuocKhaoSat.trim(),
        tenCuocKhaoSat: form.tenCuocKhaoSat.trim(),
        mucDich: form.mucDich.trim() || null,
        phamVi: form.phamVi.trim() || null,
        linhVucId: null,
        vanBanId: null,
      });
      setOpen(false);
      setSuccess("Đã tạo cuộc khảo sát.");
      await load();
      if (result?.id) navigate(`/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat/${result.id}`);
    } catch (requestError) {
      setError(errorMessage(requestError));
    } finally {
      setSaving(false);
    }
  };

  const statusOptions = [{ value: "", label: "Chọn trạng thái" }, ...statuses.map((item) => ({ value: item.id, label: item.tenTrangThai || item.maTrangThai }))];
  const unitOptions = [{ value: "", label: "Chọn đơn vị chủ trì" }, ...units.map((item) => ({ value: item.id, label: item.tenDonVi || item.ten || item.maDonVi }))];
  const canSave = form.maCuocKhaoSat.trim() && form.tenCuocKhaoSat.trim() && form.donViChuTriId && form.trangThaiId && form.tuNgay && form.denNgay && !saving;

  return <div className="space-y-5"><div className="flex flex-wrap items-start justify-between gap-3"><div><h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Cuộc khảo sát</h1><p className="mt-1 text-sm text-gray-500">Quản lý hồ sơ cuộc khảo sát, mẫu phiếu và đơn vị được khảo sát.</p></div><div className="flex flex-wrap gap-2"><button type="button" onClick={load} className="rounded-lg border px-4 py-2 text-sm font-medium">Tải lại</button><button type="button" onClick={openCreate} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white">+ Thêm mới</button></div></div>{error && <Alert variant="error" title="Có lỗi" message={error} />}{success && <Alert variant="success" title="Hoàn tất" message={success} />}<div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]"><div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]"><div className="max-w-xl"><Input value={keyword} onChange={(event) => setKeyword(event.target.value)} onKeyDown={(event) => event.key === "Enter" && load()} placeholder="Tìm mã hoặc tên cuộc khảo sát..." /></div></div><div className="max-w-full overflow-x-auto"><table className="w-full"><thead><tr className="border-b border-gray-100 dark:border-white/[0.05]"><th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Cuộc khảo sát</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thời gian</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Tiến độ nộp</th><th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th></tr></thead><tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">{loading ? <tr><td colSpan="4" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr> : !shown.length ? <tr><td colSpan="4" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có cuộc khảo sát.</td></tr> : shown.map((item) => <tr key={item.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]"><td className="px-5 py-4"><div className="font-medium text-gray-800 dark:text-white/90">{item.tenCuocKhaoSat}</div><div className="mt-1 text-xs text-gray-500">{item.maCuocKhaoSat}</div></td><td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{formatDate(item.tuNgay)} - {formatDate(item.denNgay)}</td><td className="px-5 py-4 text-center"><Badge size="sm" color={item.soDaNop >= item.soDoiTuong && item.soDoiTuong > 0 ? "success" : "warning"}>{item.soDaNop}/{item.soDoiTuong} đơn vị đã nộp</Badge></td><td className="px-5 py-4 text-center"><button type="button" onClick={() => navigate(`/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat/${item.id}`)} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50">Xem chi tiết</button></td></tr>)}</tbody></table></div></div><Modal isOpen={open} onClose={() => !saving && setOpen(false)} className="max-w-2xl p-6"><div className="space-y-4"><div><h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">Thêm mới cuộc khảo sát</h2><p className="mt-1 text-sm text-gray-500">Sau khi lưu, bạn có thể cấu hình nhóm đối tượng, mẫu phiếu và đơn vị khảo sát ở màn hình chi tiết.</p></div><div className="grid gap-4 sm:grid-cols-2"><div><Label>Mã cuộc khảo sát *</Label><Input value={form.maCuocKhaoSat} onChange={(event) => update("maCuocKhaoSat", event.target.value)} /></div><div><Label>Trạng thái *</Label><Select value={form.trangThaiId} options={statusOptions} onChange={(value) => update("trangThaiId", value)} /></div><div className="sm:col-span-2"><Label>Tên cuộc khảo sát *</Label><Input value={form.tenCuocKhaoSat} onChange={(event) => update("tenCuocKhaoSat", event.target.value)} /></div><div><Label>Từ ngày *</Label><Input type="date" value={form.tuNgay} onChange={(event) => update("tuNgay", event.target.value)} /></div><div><Label>Đến ngày *</Label><Input type="date" value={form.denNgay} onChange={(event) => update("denNgay", event.target.value)} /></div><div className="sm:col-span-2"><Label>Đơn vị chủ trì *</Label><Select value={form.donViChuTriId} options={unitOptions} onChange={(value) => update("donViChuTriId", value)} /></div><div className="sm:col-span-2"><Label>Phạm vi</Label><Input value={form.phamVi} onChange={(event) => update("phamVi", event.target.value)} /></div><div className="sm:col-span-2"><Label>Mục đích</Label><TextArea rows={3} value={form.mucDich} onChange={(value) => update("mucDich", value)} /></div></div><div className="flex justify-end gap-3"><button type="button" onClick={() => setOpen(false)} disabled={saving} className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-50">Hủy</button><button type="button" onClick={create} disabled={!canSave} className="rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white disabled:opacity-50">{saving ? "Đang lưu..." : "Lưu cuộc khảo sát"}</button></div></div></Modal></div>;
}
