import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import { Modal } from "../../../app/components/ui/modal";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import {
  createYKienDonVi,
  deleteYKienDonVi,
  getHoSoXayDungVanBanById,
  getFileTongHopYKien,
  getTongHopYKien,
  getYKienDonVi,
  updateTongHopYKien,
  updateYKienDonVi,
  uploadFileTongHopYKien,
} from "../api/xayDungVanBanApi";

const DEFAULT_TONG_HOP_Y_KIEN_FILE_TYPE_ID = "11111111-1111-1111-1111-111111111901";
const emptyOpinion = { id: "", donViGopYId: "", ngayNhan: "", ketQua: "DongY", noiDungYKien: "" };

function getErrorMessage(error, fallback = "Không thể xử lý yêu cầu.") {
  const data = error?.response?.data;
  if (typeof data === "string") return data;
  return data?.message || error?.message || fallback;
}

function toDateInput(value) {
  return value ? String(value).slice(0, 10) : "";
}

function formatDate(value) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}

function formatBytes(value) {
  if (!value) return "-";
  if (value < 1024) return `${value} B`;
  if (value < 1024 * 1024) return `${(value / 1024).toFixed(1)} KB`;
  return `${(value / 1024 / 1024).toFixed(1)} MB`;
}

function ketQuaText(value) {
  return {
    DongY: "Đồng ý",
    KhongDongY: "Không đồng ý",
    YKienKhac: "Ý kiến khác",
    KhongPhanHoi: "Không phản hồi",
  }[value] || value || "-";
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

export default function HoSoYKienDongGopPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [hoSo, setHoSo] = useState(null);
  const [donVis, setDonVis] = useState([]);
  const [items, setItems] = useState([]);
  const [summary, setSummary] = useState("");
  const [summaryFileTypeId, setSummaryFileTypeId] = useState(DEFAULT_TONG_HOP_Y_KIEN_FILE_TYPE_ID);
  const [summaryFiles, setSummaryFiles] = useState([]);
  const [summaryFile, setSummaryFile] = useState(null);
  const [form, setForm] = useState(emptyOpinion);
  const [isOpinionModalOpen, setIsOpinionModalOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const donViMap = useMemo(() => new Map(donVis.map((item) => [item.id, item.tenDonVi || item.ten || item.maDonVi])), [donVis]);
  const donViOptions = useMemo(() => donVis.map((item) => ({ value: item.id, label: item.tenDonVi || item.ten || item.maDonVi })), [donVis]);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      const detail = await getHoSoXayDungVanBanById(id);
      const [opinions, summaryResult, summaryFileResult, donViResult] = await Promise.all([
        getYKienDonVi(id).catch(() => []),
        getTongHopYKien(id).catch(() => null),
        getFileTongHopYKien(id).catch(() => []),
        getDonViOptions().catch(() => []),
      ]);
      setHoSo(detail);
      setItems(opinions || []);
      setSummary(summaryResult?.noiDungTongHopTiepThuGiaiTrinh || "");
      setSummaryFileTypeId(summaryResult?.loaiTaiLieuTongHopYKienId || DEFAULT_TONG_HOP_Y_KIEN_FILE_TYPE_ID);
      setSummaryFiles(summaryFileResult || []);
      setDonVis(donViResult || []);
    } catch (loadError) {
      setError(getErrorMessage(loadError, "Không thể tải dữ liệu ý kiến đóng góp."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, [id]);

  const submitOpinion = async (event) => {
    event.preventDefault();
    if (!form.donViGopYId) {
      setError("Vui lòng chọn đơn vị góp ý.");
      return;
    }

    try {
      setSaving(true);
      setError("");
      setSuccess("");
      const payload = {
        donViGopYId: form.donViGopYId,
        ngayNhan: form.ngayNhan || null,
        ketQua: form.ketQua,
        noiDungYKien: form.noiDungYKien?.trim() || null,
      };
      if (form.id) await updateYKienDonVi(id, form.id, payload);
      else await createYKienDonVi(id, payload);
      setForm(emptyOpinion);
      setIsOpinionModalOpen(false);
      setSuccess("Đã lưu ý kiến đóng góp.");
      await load();
    } catch (saveError) {
      setError(getErrorMessage(saveError, "Không thể lưu ý kiến đóng góp."));
    } finally {
      setSaving(false);
    }
  };

  const removeOpinion = async (item) => {
    if (!window.confirm(`Xóa ý kiến của đơn vị "${donViMap.get(item.donViGopYId) || item.donViGopYId}"?`)) return;
    try {
      setSaving(true);
      setError("");
      await deleteYKienDonVi(id, item.id);
      setSuccess("Đã xóa ý kiến đóng góp.");
      await load();
    } catch (deleteError) {
      setError(getErrorMessage(deleteError, "Không thể xóa ý kiến."));
    } finally {
      setSaving(false);
    }
  };

  const saveSummary = async () => {
    try {
      setSaving(true);
      setError("");
      await updateTongHopYKien(id, { noiDungTongHopTiepThuGiaiTrinh: summary.trim() || null });
      setSuccess("Đã lưu nội dung tổng hợp ý kiến.");
      await load();
    } catch (summaryError) {
      setError(getErrorMessage(summaryError, "Không thể lưu tổng hợp ý kiến."));
    } finally {
      setSaving(false);
    }
  };

  const uploadSummaryFile = async () => {
    if (!summaryFile) {
      setError("Vui lòng chọn file bảng tổng hợp ý kiến.");
      return;
    }

    try {
      setSaving(true);
      setError("");
      await uploadFileTongHopYKien(id, {
        file: summaryFile,
        loaiTaiLieuId: summaryFileTypeId || DEFAULT_TONG_HOP_Y_KIEN_FILE_TYPE_ID,
      });
      setSummaryFile(null);
      setSuccess("Đã tải bảng tổng hợp ý kiến.");
      await load();
    } catch (uploadError) {
      setError(getErrorMessage(uploadError, "Không thể tải file tổng hợp ý kiến."));
    } finally {
      setSaving(false);
    }
  };

  const editOpinion = (item) => {
    setForm({
      id: item.id,
      donViGopYId: item.donViGopYId || "",
      ngayNhan: toDateInput(item.ngayNhan),
      ketQua: item.ketQua || "DongY",
      noiDungYKien: item.noiDungYKien || "",
    });
    setIsOpinionModalOpen(true);
  };

  const openCreateOpinionModal = () => {
    setForm(emptyOpinion);
    setIsOpinionModalOpen(true);
  };

  const closeOpinionModal = () => {
    if (saving) return;
    setIsOpinionModalOpen(false);
    setForm(emptyOpinion);
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <button type="button" onClick={() => navigate("/admin/xay-dung-van-ban/ho-so")} className="mb-3 text-sm font-medium text-brand-500 hover:text-brand-600">
            Quay lại danh sách
          </button>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Nhập ý kiến đóng góp</h1>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">{hoSo?.tenHoSo || "Cập nhật ý kiến góp ý của các đơn vị cho hồ sơ."}</p>
        </div>
        {hoSo && <Badge size="sm" color="light">{hoSo.maHoSo}</Badge>}
      </div>

      {error && <Alert variant="error" title="Không thể xử lý" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      {loading ? (
        <div className="rounded-xl border border-gray-200 bg-white p-10 text-center text-sm text-gray-500 dark:border-white/[0.05] dark:bg-white/[0.03]">Đang tải dữ liệu...</div>
      ) : (
        <>
          <div className="rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="flex flex-col gap-3 border-b border-gray-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-white/[0.05]">
              <div>
                <h2 className="text-base font-semibold text-gray-800 dark:text-white/90">Danh sách ý kiến đóng góp</h2>
                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Các ý kiến đã nhập được hiển thị tại đây, thêm hoặc sửa qua màn hình nhập riêng.</p>
              </div>
              <button type="button" onClick={openCreateOpinionModal} className="inline-flex h-10 items-center justify-center rounded-lg bg-brand-500 px-4 text-sm font-medium text-white transition hover:bg-brand-600">
                + Thêm mới
              </button>
            </div>
            <div className="max-w-full overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Đơn vị</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Ngày nhận</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Kết quả</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Nội dung</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                  {items.length ? items.map((item) => (
                    <tr key={item.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                      <td className="px-5 py-4 text-sm font-medium text-gray-800 dark:text-white/90">{donViMap.get(item.donViGopYId) || item.donViGopYId}</td>
                      <td className="px-5 py-4 text-center text-sm text-gray-500">{formatDate(item.ngayNhan)}</td>
                      <td className="px-5 py-4 text-center"><Badge size="sm" color={item.ketQua === "DongY" ? "success" : "light"}>{ketQuaText(item.ketQua)}</Badge></td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.noiDungYKien || "-"}</td>
                      <td className="px-5 py-4">
                        <div className="flex justify-center gap-1">
                          <button type="button" onClick={() => editOpinion(item)} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500">Sửa</button>
                          <button type="button" onClick={() => removeOpinion(item)} className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-error-500">Xóa</button>
                        </div>
                      </td>
                    </tr>
                  )) : (
                    <tr><td colSpan="5" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có ý kiến đóng góp.</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="mb-5 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <h2 className="text-base font-semibold text-gray-800 dark:text-white/90">Tổng hợp, tiếp thu và giải trình</h2>
                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Nội dung này được dùng để kiểm tra điều kiện trước khi chuyển bước.</p>
              </div>
              <button type="button" onClick={saveSummary} disabled={saving} className="inline-flex h-10 items-center justify-center rounded-lg border border-gray-300 bg-white px-4 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
                Lưu tổng hợp
              </button>
            </div>
            <TextArea rows={5} value={summary} onChange={setSummary} disabled={saving} />
            <div className="mt-5 rounded-lg border border-gray-200 p-4 dark:border-white/[0.05]">
              <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                  <h3 className="text-sm font-semibold text-gray-800 dark:text-white/90">File bảng tổng hợp ý kiến</h3>
                  <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Đính kèm file tổng hợp ý kiến đã lập bên ngoài để lưu cùng hồ sơ.</p>
                </div>
                <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
                  <label className="inline-flex h-10 cursor-pointer items-center justify-center rounded-lg border border-gray-300 bg-white px-4 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
                    Chọn file
                    <input
                      type="file"
                      className="hidden"
                      disabled={saving}
                      onChange={(event) => setSummaryFile(event.target.files?.[0] || null)}
                    />
                  </label>
                  <button type="button" onClick={uploadSummaryFile} disabled={!summaryFile || saving} className="inline-flex h-10 items-center justify-center rounded-lg bg-brand-500 px-4 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300">
                    Tải lên
                  </button>
                </div>
              </div>
              {summaryFile && <div className="mt-3 text-sm text-gray-600 dark:text-gray-300">Đã chọn: {summaryFile.name} ({formatBytes(summaryFile.size)})</div>}
              <div className="mt-4 divide-y divide-gray-100 rounded-lg border border-gray-100 dark:divide-white/[0.05] dark:border-white/[0.05]">
                {summaryFiles.length ? summaryFiles.map((file) => (
                  <div key={file.id} className="grid gap-2 px-4 py-3 text-sm text-gray-700 sm:grid-cols-[1fr_120px_120px] dark:text-gray-300">
                    <div>
                      <div className="font-medium text-gray-800 dark:text-white/90">{file.tenTaiLieu || "Tổng hợp ý kiến"}</div>
                      <div className="mt-1 text-xs text-gray-500">{file.tenFile}</div>
                    </div>
                    <div>Phiên bản {file.phienBan}</div>
                    <div>{formatBytes(file.dungLuong)}</div>
                  </div>
                )) : (
                  <div className="px-4 py-6 text-center text-sm text-gray-500">Chưa có file tổng hợp ý kiến.</div>
                )}
              </div>
            </div>
          </div>

          <Modal isOpen={isOpinionModalOpen} onClose={closeOpinionModal} className="max-w-3xl p-6">
            <form onSubmit={submitOpinion} className="space-y-5">
              <div>
                <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">{form.id ? "Sửa ý kiến đóng góp" : "Thêm ý kiến đóng góp"}</h2>
                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Nhập ý kiến của từng đơn vị, sau khi lưu bảng danh sách sẽ được tải lại.</p>
              </div>
              <div className="grid gap-5 lg:grid-cols-2">
                <Field label="Đơn vị góp ý" required>
                  <Select value={form.donViGopYId} placeholder="Chọn đơn vị" options={donViOptions} onChange={(value) => setForm((current) => ({ ...current, donViGopYId: value }))} />
                </Field>
                <Field label="Ngày nhận">
                  <Input type="date" value={form.ngayNhan} onChange={(event) => setForm((current) => ({ ...current, ngayNhan: event.target.value }))} disabled={saving} />
                </Field>
                <Field label="Kết quả" required>
                  <Select
                    value={form.ketQua}
                    options={[
                      { value: "DongY", label: "Đồng ý" },
                      { value: "KhongDongY", label: "Không đồng ý" },
                      { value: "YKienKhac", label: "Ý kiến khác" },
                      { value: "KhongPhanHoi", label: "Không phản hồi" },
                    ]}
                    onChange={(value) => setForm((current) => ({ ...current, ketQua: value }))}
                  />
                </Field>
                <div className="lg:col-span-2">
                  <Field label="Nội dung ý kiến">
                    <TextArea rows={4} value={form.noiDungYKien} onChange={(value) => setForm((current) => ({ ...current, noiDungYKien: value }))} disabled={saving} />
                  </Field>
                </div>
              </div>
              <div className="flex justify-end gap-3 border-t border-gray-100 pt-5 dark:border-white/[0.05]">
                <button type="button" onClick={closeOpinionModal} disabled={saving} className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
                  Hủy
                </button>
                <button type="submit" disabled={saving} className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300">
                  {saving ? "Đang lưu..." : "Lưu ý kiến"}
                </button>
              </div>
            </form>
          </Modal>
        </>
      )}
    </div>
  );
}
