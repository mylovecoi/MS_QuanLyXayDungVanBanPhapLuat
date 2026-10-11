import { useEffect, useMemo, useState } from "react";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import TextArea from "../../../app/components/forms/input/TextArea";
import { Modal } from "../../../app/components/ui/modal";
import { createTieuChiDiem, deleteTieuChiDiem, getTieuChiDiems, updateTieuChiDiem } from "../api/tieuChiDiemApi";

const KIEU_GIA_TRI_OPTIONS = [
  { value: "SO_LAN", label: "Số lần" },
  { value: "TY_LE", label: "Tỷ lệ" },
  { value: "SO_NGAY", label: "Số ngày" },
  { value: "SO_LUONG", label: "Số lượng" },
  { value: "DIEM_DANH_GIA", label: "Điểm đánh giá" },
];

const DON_VI_OPTIONS = [
  { value: "LAN", label: "Lần" },
  { value: "PERCENT", label: "%" },
  { value: "NGAY", label: "Ngày" },
  { value: "FILE", label: "File" },
  { value: "DIEM", label: "Điểm" },
];

const LOAI_TIEU_CHI_OPTIONS = [
  { value: "CHAT_LUONG_HO_SO", label: "Chất lượng hồ sơ" },
  { value: "TIEN_DO_THUC_HIEN", label: "Tiến độ thực hiện" },
  { value: "THANH_PHAN_TAI_LIEU", label: "Thành phần tài liệu" },
  { value: "TUAN_THU_QUY_TRINH", label: "Tuân thủ quy trình" },
  { value: "KET_QUA_XU_LY", label: "Kết quả xử lý" },
];

const level = (order = 1) => ({
  id: null,
  tuGiaTri: "",
  denGiaTri: "",
  baoGomTuGiaTri: true,
  baoGomDenGiaTri: true,
  diem: 0,
  nhanHienThi: "",
  thuTuSapXep: order,
  trangThai: true,
  ghiChu: "",
});

const blank = () => ({
  id: "",
  maTieuChi: "",
  tenTieuChi: "",
  loaiTieuChi: "CHAT_LUONG_HO_SO",
  kieuGiaTri: "SO_LAN",
  donViGiaTri: "LAN",
  thuTuSapXep: 1,
  diemToiDa: 10,
  trangThai: true,
  moTa: "",
  ghiChu: "",
  mucs: [level()],
});

const message = (error) => error?.response?.data?.message || error?.message || "Không thể xử lý yêu cầu.";
const numberOrNull = (value) => value === "" || value === null || value === undefined ? null : Number(value);
const labelOf = (options, value) => options.find((item) => item.value === value)?.label || value || "-";

function levelText(item) {
  const from = item.tuGiaTri === null || item.tuGiaTri === undefined ? "-∞" : `${item.baoGomTuGiaTri ? "[" : "("}${item.tuGiaTri}`;
  const to = item.denGiaTri === null || item.denGiaTri === undefined ? "+∞" : `${item.denGiaTri}${item.baoGomDenGiaTri ? "]" : ")"}`;
  return `${from} - ${to}: ${item.diem} điểm`;
}

export default function TieuChiDiemPage() {
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState("");
  const [pageSize, setPageSize] = useState(10);
  const [pageCurrent, setPageCurrent] = useState(1);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState(blank());
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const totalPages = Math.max(1, Math.ceil(total / pageSize));
  const totalPoint = useMemo(() => items.reduce((sum, item) => item.trangThai ? sum + Number(item.diemToiDa || 0) : sum, 0), [items]);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const result = await getTieuChiDiems({ search, pageSize, pageCurrent });
      setItems(result.data || []);
      setTotal(result.totalCount || 0);
    } catch (e) {
      setError(message(e));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => void load(), 250);
    return () => clearTimeout(timer);
  }, [search, pageSize, pageCurrent]);

  const edit = (item) => {
    setForm({
      ...blank(),
      ...item,
      mucs: (item.mucs?.length ? item.mucs : [level()]).map((row, index) => ({
        ...level(index + 1),
        ...row,
        tuGiaTri: row.tuGiaTri ?? "",
        denGiaTri: row.denGiaTri ?? "",
        ghiChu: row.ghiChu ?? "",
        nhanHienThi: row.nhanHienThi ?? "",
      })),
    });
    setOpen(true);
  };

  const updateForm = (key, value) => setForm((current) => ({ ...current, [key]: value }));
  const updateLevel = (index, key, value) => setForm((current) => ({
    ...current,
    mucs: current.mucs.map((item, rowIndex) => rowIndex === index ? { ...item, [key]: value } : item),
  }));

  const addLevel = () => setForm((current) => ({ ...current, mucs: [...current.mucs, level(current.mucs.length + 1)] }));
  const removeLevel = (index) => setForm((current) => ({ ...current, mucs: current.mucs.filter((_, rowIndex) => rowIndex !== index).map((item, rowIndex) => ({ ...item, thuTuSapXep: rowIndex + 1 })) }));

  const submit = async (event) => {
    event.preventDefault();
    if (!form.maTieuChi.trim() || !form.tenTieuChi.trim() || !form.loaiTieuChi.trim()) {
      setError("Vui lòng nhập mã, tên và loại tiêu chí.");
      return;
    }
    if (!form.mucs.length) {
      setError("Phải khai báo ít nhất một mức điểm.");
      return;
    }

    try {
      setSaving(true);
      setError("");
      const payload = {
        ...form,
        maTieuChi: form.maTieuChi.trim().toUpperCase(),
        tenTieuChi: form.tenTieuChi.trim(),
        loaiTieuChi: form.loaiTieuChi.trim().toUpperCase(),
        kieuGiaTri: form.kieuGiaTri.trim().toUpperCase(),
        donViGiaTri: form.donViGiaTri.trim().toUpperCase(),
        thuTuSapXep: Number(form.thuTuSapXep) || 1,
        diemToiDa: Number(form.diemToiDa) || 0,
        moTa: form.moTa?.trim() || null,
        ghiChu: form.ghiChu?.trim() || null,
        mucs: form.mucs.map((item, index) => ({
          ...item,
          tuGiaTri: numberOrNull(item.tuGiaTri),
          denGiaTri: numberOrNull(item.denGiaTri),
          diem: Number(item.diem) || 0,
          thuTuSapXep: Number(item.thuTuSapXep) || index + 1,
          nhanHienThi: item.nhanHienThi?.trim() || null,
          ghiChu: item.ghiChu?.trim() || null,
        })),
      };

      if (form.id) await updateTieuChiDiem(form.id, payload);
      else await createTieuChiDiem(payload);
      setSuccess(form.id ? "Đã cập nhật tiêu chí điểm." : "Đã thêm tiêu chí điểm.");
      setOpen(false);
      await load();
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  const remove = async (item) => {
    if (!window.confirm(`Xóa tiêu chí "${item.tenTieuChi}"?`)) return;
    try {
      setSaving(true);
      await deleteTieuChiDiem(item.id);
      setSuccess("Đã xóa tiêu chí điểm.");
      await load();
    } catch (e) {
      setError(message(e));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Tiêu chí chấm điểm</h1>
          <p className="mt-1 text-sm text-gray-500">Cấu hình tiêu chí, thang giá trị và điểm dùng để đánh giá hồ sơ xây dựng văn bản.</p>
        </div>
        <button type="button" onClick={() => { setForm(blank()); setOpen(true); }} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white hover:bg-brand-600">
          Thêm tiêu chí
        </button>
      </div>

      {error && <Alert variant="error" title="Không thể xử lý" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      <div className="grid gap-4 sm:grid-cols-3">
        <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
          <div className="text-sm text-gray-500">Tổng tiêu chí</div>
          <div className="mt-2 text-2xl font-semibold text-gray-800 dark:text-white/90">{total}</div>
        </div>
        <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
          <div className="text-sm text-gray-500">Đang áp dụng trên trang</div>
          <div className="mt-2 text-2xl font-semibold text-gray-800 dark:text-white/90">{items.filter((item) => item.trangThai).length}</div>
        </div>
        <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
          <div className="text-sm text-gray-500">Tổng điểm tối đa trên trang</div>
          <div className="mt-2 text-2xl font-semibold text-gray-800 dark:text-white/90">{totalPoint}</div>
        </div>
      </div>

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="grid gap-4 border-b border-gray-100 p-5 sm:grid-cols-12 dark:border-white/[0.05]">
          <div className="sm:col-span-2">
            <Label>Hiển thị</Label>
            <Select value={pageSize} onChange={(value) => { setPageSize(Number(value)); setPageCurrent(1); }} options={[5, 10, 20, 50].map((value) => ({ value, label: `${value} tiêu chí` }))} />
          </div>
          <div className="sm:col-span-10">
            <Label>Tìm kiếm</Label>
            <Input value={search} onChange={(event) => { setSearch(event.target.value); setPageCurrent(1); }} placeholder="Tìm theo mã, tên, loại tiêu chí..." />
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Tiêu chí</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Cấu hình</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Thang điểm</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Điểm tối đa</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Trạng thái</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <tr><td colSpan="6" className="p-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr>
              ) : items.length ? items.map((item) => (
                <tr key={item.id} className="align-top hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                  <td className="px-5 py-4">
                    <div className="font-medium text-gray-800 dark:text-white/90">{item.tenTieuChi}</div>
                    <div className="mt-1 text-xs text-gray-500">{item.maTieuChi}</div>
                    {item.moTa && <div className="mt-2 max-w-sm text-xs text-gray-500">{item.moTa}</div>}
                  </td>
                  <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">
                    <div>{labelOf(LOAI_TIEU_CHI_OPTIONS, item.loaiTieuChi)}</div>
                    <div className="mt-1 text-xs text-gray-500">{labelOf(KIEU_GIA_TRI_OPTIONS, item.kieuGiaTri)} · {labelOf(DON_VI_OPTIONS, item.donViGiaTri)}</div>
                    <div className="mt-1 text-xs text-gray-500">Thứ tự {item.thuTuSapXep}</div>
                  </td>
                  <td className="px-5 py-4">
                    <div className="space-y-1">
                      {(item.mucs || []).slice(0, 4).map((row) => (
                        <div key={row.id || `${row.thuTuSapXep}-${row.diem}`} className="text-xs text-gray-600 dark:text-gray-300">
                          {row.nhanHienThi || levelText(row)}
                        </div>
                      ))}
                      {(item.mucs?.length || 0) > 4 && <div className="text-xs text-gray-500">+{item.mucs.length - 4} mức khác</div>}
                    </div>
                  </td>
                  <td className="px-5 py-4 text-center text-sm font-medium text-gray-800 dark:text-white/90">{item.diemToiDa}</td>
                  <td className="px-5 py-4 text-center">
                    <Badge size="sm" color={item.trangThai ? "success" : "light"}>{item.trangThai ? "Đang dùng" : "Tạm dừng"}</Badge>
                  </td>
                  <td className="px-5 py-4">
                    <div className="flex justify-center gap-1">
                      <button type="button" onClick={() => edit(item)} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-brand-50">Sửa</button>
                      <button type="button" onClick={() => remove(item)} disabled={saving} className="rounded-lg px-3 py-2 text-xs font-medium text-error-600 hover:bg-error-50 disabled:opacity-50">Xóa</button>
                    </div>
                  </td>
                </tr>
              )) : (
                <tr><td colSpan="6" className="p-10 text-center text-sm text-gray-500">Không có dữ liệu tiêu chí điểm.</td></tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="flex items-center justify-between border-t border-gray-100 px-5 py-4 text-sm text-gray-500">
          <span>{total} tiêu chí</span>
          <div className="flex items-center gap-2">
            <button type="button" disabled={pageCurrent === 1 || loading} onClick={() => setPageCurrent((value) => value - 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Trước</button>
            <span className="px-3 py-2">{pageCurrent}/{totalPages}</span>
            <button type="button" disabled={pageCurrent >= totalPages || loading} onClick={() => setPageCurrent((value) => value + 1)} className="rounded-lg border px-3 py-2 disabled:opacity-50">Sau</button>
          </div>
        </div>
      </div>

      <Modal isOpen={open} onClose={() => !saving && setOpen(false)} className="max-w-6xl p-6">
        <form onSubmit={submit} className="space-y-5">
          <div>
            <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">{form.id ? "Cập nhật tiêu chí" : "Thêm mới tiêu chí"}</h4>
            <p className="mt-1 text-sm text-gray-500">Khoảng giá trị không nhập cận dưới hoặc cận trên sẽ được hiểu là không giới hạn.</p>
          </div>

          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <div>
              <Label>Mã tiêu chí *</Label>
              <Input value={form.maTieuChi} onChange={(event) => updateForm("maTieuChi", event.target.value)} placeholder="VD: CHAT_LUONG_HO_SO" />
            </div>
            <div className="lg:col-span-2">
              <Label>Tên tiêu chí *</Label>
              <Input value={form.tenTieuChi} onChange={(event) => updateForm("tenTieuChi", event.target.value)} />
            </div>
            <div>
              <Label>Loại tiêu chí *</Label>
              <Select value={form.loaiTieuChi} onChange={(value) => updateForm("loaiTieuChi", value)} options={LOAI_TIEU_CHI_OPTIONS} />
            </div>
            <div>
              <Label>Kiểu giá trị</Label>
              <Select value={form.kieuGiaTri} onChange={(value) => updateForm("kieuGiaTri", value)} options={KIEU_GIA_TRI_OPTIONS} />
            </div>
            <div>
              <Label>Đơn vị giá trị</Label>
              <Select value={form.donViGiaTri} onChange={(value) => updateForm("donViGiaTri", value)} options={DON_VI_OPTIONS} />
            </div>
            <div>
              <Label>Thứ tự</Label>
              <Input type="number" min="1" value={form.thuTuSapXep} onChange={(event) => updateForm("thuTuSapXep", event.target.value)} />
            </div>
            <div>
              <Label>Điểm tối đa</Label>
              <Input type="number" min="0" step="0.01" value={form.diemToiDa} onChange={(event) => updateForm("diemToiDa", event.target.value)} />
            </div>
            <label className="mt-7 flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300">
              <input type="checkbox" checked={form.trangThai} onChange={(event) => updateForm("trangThai", event.target.checked)} />
              Đang áp dụng
            </label>
            <div className="sm:col-span-2 lg:col-span-3">
              <Label>Mô tả</Label>
              <TextArea rows={2} value={form.moTa || ""} onChange={(value) => updateForm("moTa", value)} />
            </div>
            <div className="sm:col-span-2 lg:col-span-3">
              <Label>Ghi chú</Label>
              <TextArea rows={2} value={form.ghiChu || ""} onChange={(value) => updateForm("ghiChu", value)} />
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 p-4 dark:border-white/[0.08]">
            <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
              <div>
                <h5 className="font-medium text-gray-800 dark:text-white/90">Các mức điểm</h5>
                <p className="text-xs text-gray-500">Thiết lập khoảng giá trị, điểm tương ứng và nhãn hiển thị.</p>
              </div>
              <button type="button" onClick={addLevel} className="rounded-lg border border-brand-200 px-3 py-2 text-sm font-medium text-brand-500 hover:bg-brand-50">Thêm mức</button>
            </div>
            <div className="space-y-3">
              {form.mucs.map((row, index) => (
                <div key={index} className="grid items-end gap-3 rounded-lg bg-gray-50 p-3 sm:grid-cols-12 dark:bg-white/[0.04]">
                  <div className="sm:col-span-1">
                    <Label>TT</Label>
                    <Input type="number" min="1" value={row.thuTuSapXep} onChange={(event) => updateLevel(index, "thuTuSapXep", event.target.value)} />
                  </div>
                  <div className="sm:col-span-2">
                    <Label>Từ</Label>
                    <Input type="number" step="0.01" value={row.tuGiaTri} onChange={(event) => updateLevel(index, "tuGiaTri", event.target.value)} />
                    <label className="mt-2 flex items-center gap-2 text-xs text-gray-500">
                      <input type="checkbox" checked={row.baoGomTuGiaTri} onChange={(event) => updateLevel(index, "baoGomTuGiaTri", event.target.checked)} />
                      Bao gồm
                    </label>
                  </div>
                  <div className="sm:col-span-2">
                    <Label>Đến</Label>
                    <Input type="number" step="0.01" value={row.denGiaTri} onChange={(event) => updateLevel(index, "denGiaTri", event.target.value)} />
                    <label className="mt-2 flex items-center gap-2 text-xs text-gray-500">
                      <input type="checkbox" checked={row.baoGomDenGiaTri} onChange={(event) => updateLevel(index, "baoGomDenGiaTri", event.target.checked)} />
                      Bao gồm
                    </label>
                  </div>
                  <div className="sm:col-span-2">
                    <Label>Điểm</Label>
                    <Input type="number" min="0" step="0.01" value={row.diem} onChange={(event) => updateLevel(index, "diem", event.target.value)} />
                  </div>
                  <div className="sm:col-span-3">
                    <Label>Nhãn hiển thị</Label>
                    <Input value={row.nhanHienThi || ""} onChange={(event) => updateLevel(index, "nhanHienThi", event.target.value)} />
                  </div>
                  <div className="sm:col-span-1">
                    <label className="mb-2 flex items-center gap-2 text-xs text-gray-500">
                      <input type="checkbox" checked={row.trangThai} onChange={(event) => updateLevel(index, "trangThai", event.target.checked)} />
                      Dùng
                    </label>
                    <button type="button" disabled={form.mucs.length === 1} onClick={() => removeLevel(index)} className="w-full rounded-lg px-3 py-2 text-sm text-error-600 hover:bg-error-50 disabled:opacity-40">Xóa</button>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="flex justify-end gap-3 border-t pt-5">
            <button type="button" onClick={() => setOpen(false)} disabled={saving} className="rounded-lg border px-4 py-2.5 text-sm font-medium disabled:opacity-50">Hủy</button>
            <button type="submit" disabled={saving} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">{saving ? "Đang lưu..." : "Lưu tiêu chí"}</button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
