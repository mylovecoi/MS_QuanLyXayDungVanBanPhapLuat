import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Input from "../../../app/components/forms/input/InputField";
import TextArea from "../../../app/components/forms/input/TextArea";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import { getDonViOptions } from "../../danh-muc/api/donViApi";
import { getQuyTrinhById, getQuyTrinhs } from "../../danh-muc/api/quyTrinhSoanThaoApi";
import { getTrangThais } from "../../danh-muc/api/trangThaiApi";
import { getVanBans } from "../../danh-muc/api/vanBanApi";
import {
  createHoSoSoanThao,
  getHoSoSoanThaoById,
  getTaiLieuSoanThao,
  updateHoSoSoanThao,
  uploadTaiLieuSoanThao,
} from "../api/xayDungVanBanApi";

const CURRENT_YEAR = new Date().getFullYear();
const HO_SO_STATUS_GROUP = "HO_SO_XAY_DUNG_VAN_BAN";

const initialForm = {
  tenHoSo: "",
  tenDuThaoVanBan: "",
  danhMucVanBanId: "",
  quyTrinhSoanThaoId: "",
  buocHienTaiId: "",
  trangThaiHoSoId: "",
  donViChuTriSoanThaoId: "",
  nguoiPhuTrachId: "",
  namXayDung: CURRENT_YEAR,
  thoiGianDuKienBatDau: "",
  thoiGianDuKienHoanThanh: "",
  moTa: "",
  canCuXayDung: "",
  phamViDieuChinh: "",
  noiDungChinhSach: "",
};

function getErrorMessage(error, fallback = "Không thể xử lý yêu cầu.") {
  const data = error?.response?.data;
  if (typeof data === "string") return data;
  return data?.message || error?.message || fallback;
}

function toDateInput(value) {
  return value ? String(value).slice(0, 10) : "";
}

function emptyToNull(value) {
  return value?.trim() ? value.trim() : null;
}

function createId() {
  return crypto?.randomUUID?.() || `${Date.now()}-${Math.random()}`;
}

function formatBytes(value) {
  if (!value) return "-";
  if (value < 1024) return `${value} B`;
  if (value < 1024 * 1024) return `${(value / 1024).toFixed(1)} KB`;
  return `${(value / 1024 / 1024).toFixed(1)} MB`;
}

function asOptions(items, valueKey, labelGetter) {
  return (items || []).map((item) => ({
    value: item[valueKey],
    label: labelGetter(item),
  }));
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

export default function HoSoFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);
  const [form, setForm] = useState(initialForm);
  const [vanBans, setVanBans] = useState([]);
  const [quyTrinhs, setQuyTrinhs] = useState([]);
  const [donVis, setDonVis] = useState([]);
  const [trangThais, setTrangThais] = useState([]);
  const [selectedQuyTrinh, setSelectedQuyTrinh] = useState(null);
  const [existingFiles, setExistingFiles] = useState([]);
  const [pendingFiles, setPendingFiles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const updateForm = (key, value) => {
    setForm((current) => ({ ...current, [key]: value }));
  };

  const loadLookups = async () => {
    const [vanBanResponse, quyTrinhResponse, donViResponse, trangThaiResponse] = await Promise.all([
      getVanBans({ pageSize: 200, pageCurrent: 1 }),
      getQuyTrinhs({ pageSize: 200, pageCurrent: 1 }),
      getDonViOptions(),
      getTrangThais({ nhomTrangThai: HO_SO_STATUS_GROUP, pageSize: 100, pageCurrent: 1 }),
    ]);

    const statusItems = trangThaiResponse?.data || [];
    setVanBans(vanBanResponse?.data || []);
    setQuyTrinhs(quyTrinhResponse?.data || []);
    setDonVis(donViResponse || []);
    setTrangThais(statusItems);

    return {
      statuses: statusItems,
    };
  };

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        setError("");
        const lookups = await loadLookups();

        if (isEdit) {
          const [detail, files] = await Promise.all([
            getHoSoSoanThaoById(id),
            getTaiLieuSoanThao(id).catch(() => []),
          ]);
          setForm({
            tenHoSo: detail.tenHoSo || "",
            tenDuThaoVanBan: detail.tenDuThaoVanBan || "",
            danhMucVanBanId: detail.danhMucVanBanId || "",
            quyTrinhSoanThaoId: detail.quyTrinhSoanThaoId || "",
            buocHienTaiId: detail.buocHienTaiId || "",
            trangThaiHoSoId: detail.trangThaiHoSoId || "",
            donViChuTriSoanThaoId: detail.donViChuTriSoanThaoId || "",
            nguoiPhuTrachId: detail.nguoiPhuTrachId || "",
            namXayDung: detail.namXayDung || CURRENT_YEAR,
            thoiGianDuKienBatDau: toDateInput(detail.thoiGianDuKienBatDau),
            thoiGianDuKienHoanThanh: toDateInput(detail.thoiGianDuKienHoanThanh),
            moTa: detail.moTa || "",
            canCuXayDung: detail.canCuXayDung || "",
            phamViDieuChinh: detail.phamViDieuChinh || "",
            noiDungChinhSach: detail.noiDungChinhSach || "",
          });
          setExistingFiles(files || []);
        } else {
          const defaultStatus = lookups.statuses.find((item) => item.maTrangThai === "DANG_XU_LY") || lookups.statuses[0];
          setForm((current) => ({
            ...current,
            trangThaiHoSoId: defaultStatus?.id || "",
          }));
        }
      } catch (loadError) {
        setError(getErrorMessage(loadError, "Không thể tải dữ liệu nhập hồ sơ."));
      } finally {
        setLoading(false);
      }
    };

    void load();
  }, [id, isEdit]);

  useEffect(() => {
    const loadWorkflow = async () => {
      if (!form.quyTrinhSoanThaoId) {
        setSelectedQuyTrinh(null);
        return;
      }

      try {
        const detail = await getQuyTrinhById(form.quyTrinhSoanThaoId);
        setSelectedQuyTrinh(detail);

        if (!isEdit && !form.buocHienTaiId) {
          const firstStep = [...(detail.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0))[0];
          if (firstStep?.id) {
            setForm((current) => ({ ...current, buocHienTaiId: firstStep.id }));
          }
        }
      } catch (workflowError) {
        setSelectedQuyTrinh(null);
        setError(getErrorMessage(workflowError, "Không thể tải bước của quy trình."));
      }
    };

    void loadWorkflow();
  }, [form.quyTrinhSoanThaoId, form.buocHienTaiId, isEdit]);

  const filteredQuyTrinhOptions = useMemo(() => {
    const items = form.danhMucVanBanId
      ? quyTrinhs.filter((item) => {
          const ids = item.danhMucVanBanIds || [];
          return ids.length === 0 || ids.includes(form.danhMucVanBanId) || item.danhMucVanBanId === form.danhMucVanBanId;
        })
      : quyTrinhs;

    return asOptions(items, "id", (item) => `${item.tenQuyTrinh || item.maQuyTrinh} (${item.maQuyTrinh})`);
  }, [form.danhMucVanBanId, quyTrinhs]);

  const stepOptions = useMemo(() => {
    const steps = [...(selectedQuyTrinh?.buocQuyTrinhs || [])].sort((a, b) => (a.thuTuSapXep || 0) - (b.thuTuSapXep || 0));
    return asOptions(steps, "id", (item) => `${item.thuTuSapXep}. ${item.tenBuoc}`);
  }, [selectedQuyTrinh]);

  const statusOptions = useMemo(
    () => asOptions(trangThais, "id", (item) => `${item.tenTrangThai || item.maTrangThai} (${item.maTrangThai})`),
    [trangThais]
  );

  const validate = () => {
    if (!form.tenHoSo.trim()) return "Vui lòng nhập tên hồ sơ.";
    if (!form.tenDuThaoVanBan.trim()) return "Vui lòng nhập tên dự thảo văn bản.";
    if (!form.danhMucVanBanId) return "Vui lòng chọn loại văn bản.";
    if (!form.quyTrinhSoanThaoId) return "Vui lòng chọn quy trình soạn thảo.";
    if (!form.buocHienTaiId) return "Vui lòng chọn bước hiện tại.";
    if (!form.trangThaiHoSoId) return "Vui lòng chọn trạng thái hồ sơ.";
    if (!form.donViChuTriSoanThaoId) return "Vui lòng chọn đơn vị chủ trì.";
    if (!form.namXayDung || Number(form.namXayDung) < 2000 || Number(form.namXayDung) > 9999) return "Năm xây dựng không hợp lệ.";
    if (form.thoiGianDuKienBatDau && form.thoiGianDuKienHoanThanh && form.thoiGianDuKienBatDau > form.thoiGianDuKienHoanThanh) {
      return "Thời gian bắt đầu không được lớn hơn thời gian hoàn thành.";
    }
    if (pendingFiles.some((item) => !item.tenTaiLieu.trim() || !item.file)) return "Vui lòng nhập đủ tên tài liệu cho file đính kèm.";
    return "";
  };

  const addPendingFiles = (fileList) => {
    const selectedFiles = Array.from(fileList || []);
    if (!selectedFiles.length) return;

    setPendingFiles((current) => [
      ...current,
      ...selectedFiles.map((file) => ({
        id: createId(),
        loaiTaiLieuId: createId(),
        tenTaiLieu: file.name.replace(/\.[^/.]+$/, "") || file.name,
        file,
      })),
    ]);
  };

  const updatePendingFile = (id, value) => {
    setPendingFiles((current) => current.map((item) => (item.id === id ? { ...item, tenTaiLieu: value } : item)));
  };

  const removePendingFile = (id) => {
    setPendingFiles((current) => current.filter((item) => item.id !== id));
  };

  const uploadPendingFiles = async (hoSoId) => {
    for (const item of pendingFiles) {
      await uploadTaiLieuSoanThao(hoSoId, {
        file: item.file,
        loaiTaiLieuId: item.loaiTaiLieuId,
        tenTaiLieu: item.tenTaiLieu.trim(),
      });
    }
  };

  const buildPayload = () => {
    const shared = {
      tenHoSo: form.tenHoSo.trim(),
      tenDuThaoVanBan: form.tenDuThaoVanBan.trim(),
      nguoiPhuTrachId: emptyToNull(form.nguoiPhuTrachId),
      namXayDung: Number(form.namXayDung),
      thoiGianDuKienBatDau: form.thoiGianDuKienBatDau || null,
      thoiGianDuKienHoanThanh: form.thoiGianDuKienHoanThanh || null,
      moTa: emptyToNull(form.moTa),
      canCuXayDung: emptyToNull(form.canCuXayDung),
      phamViDieuChinh: emptyToNull(form.phamViDieuChinh),
      noiDungChinhSach: emptyToNull(form.noiDungChinhSach),
      danhMucVanBanId: form.danhMucVanBanId,
      quyTrinhSoanThaoId: form.quyTrinhSoanThaoId,
      buocHienTaiId: form.buocHienTaiId,
      trangThaiHoSoId: form.trangThaiHoSoId,
      donViChuTriSoanThaoId: form.donViChuTriSoanThaoId,
    };

    return shared;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    const validationMessage = validate();
    if (validationMessage) {
      setError(validationMessage);
      return;
    }

    try {
      setSaving(true);
      setError("");
      setSuccess("");
      const result = isEdit ? await updateHoSoSoanThao(id, buildPayload()) : await createHoSoSoanThao(buildPayload());
      const hoSoId = result?.hoSoId || id;
      if (pendingFiles.length) {
        await uploadPendingFiles(hoSoId);
      }
      setSuccess(isEdit ? "Cập nhật hồ sơ thành công." : "Tạo hồ sơ soạn thảo thành công.");
      setTimeout(() => navigate("/admin/xay-dung-van-ban/ho-so"), 300);
    } catch (saveError) {
      setError(getErrorMessage(saveError, "Không thể lưu hồ sơ."));
    } finally {
      setSaving(false);
    }
  };

  const vanBanOptions = asOptions(vanBans, "id", (item) => item.tenLoaiVanBan || item.ten || item.maLoaiVanBan || item.ma);
  const donViOptions = asOptions(donVis, "id", (item) => item.tenDonVi || item.ten || item.maDonVi);

  return (
    <form className="space-y-5" onSubmit={handleSubmit}>
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <button
            type="button"
            onClick={() => navigate("/admin/xay-dung-van-ban/ho-so")}
            className="mb-3 text-sm font-medium text-brand-500 hover:text-brand-600"
          >
            Quay lại danh sách
          </button>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
            {isEdit ? "Cập nhật hồ sơ xây dựng văn bản" : "Tạo hồ sơ xây dựng văn bản"}
          </h1>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Nhập thông tin khởi tạo hồ sơ soạn thảo và gắn vào quy trình nghiệp vụ.
          </p>
        </div>
        <div className="flex gap-2">
          <button
            type="button"
            onClick={() => navigate("/admin/xay-dung-van-ban/ho-so")}
            className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300"
            disabled={saving}
          >
            Hủy
          </button>
          <button
            type="submit"
            className="inline-flex h-11 items-center justify-center rounded-lg bg-brand-500 px-5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:bg-brand-300"
            disabled={loading || saving}
          >
            {saving ? "Đang lưu..." : "Lưu"}
          </button>
        </div>
      </div>

      {error && <Alert variant="error" title="Có lỗi" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
        {loading ? (
          <div className="py-12 text-center text-sm text-gray-500 dark:text-gray-400">Đang tải dữ liệu nhập liệu...</div>
        ) : (
          <div className="grid gap-5 lg:grid-cols-2">
            <Field label="Tên hồ sơ" required>
              <Input value={form.tenHoSo} onChange={(event) => updateForm("tenHoSo", event.target.value)} disabled={saving} />
            </Field>
            <Field label="Tên dự thảo văn bản" required>
              <Input value={form.tenDuThaoVanBan} onChange={(event) => updateForm("tenDuThaoVanBan", event.target.value)} disabled={saving} />
            </Field>

            <Field label="Loại văn bản" required>
              <Select
                value={form.danhMucVanBanId}
                placeholder="Chọn loại văn bản"
                options={vanBanOptions}
                onChange={(value) => setForm((current) => ({ ...current, danhMucVanBanId: value, quyTrinhSoanThaoId: "", buocHienTaiId: "" }))}
              />
            </Field>
            <Field label="Quy trình soạn thảo" required>
              <Select
                value={form.quyTrinhSoanThaoId}
                placeholder="Chọn quy trình"
                options={filteredQuyTrinhOptions}
                onChange={(value) => setForm((current) => ({ ...current, quyTrinhSoanThaoId: value, buocHienTaiId: "" }))}
              />
            </Field>

            <Field label="Bước hiện tại" required>
              <Select
                value={form.buocHienTaiId}
                placeholder="Chọn bước"
                options={stepOptions}
                onChange={(value) => updateForm("buocHienTaiId", value)}
              />
            </Field>
            <Field label="Trạng thái hồ sơ" required>
              <Select
                value={form.trangThaiHoSoId}
                placeholder="Chọn trạng thái"
                options={statusOptions}
                onChange={(value) => updateForm("trangThaiHoSoId", value)}
              />
            </Field>

            <Field label="Đơn vị chủ trì" required>
              <Select
                value={form.donViChuTriSoanThaoId}
                placeholder="Chọn đơn vị"
                options={donViOptions}
                onChange={(value) => updateForm("donViChuTriSoanThaoId", value)}
              />
            </Field>
            <Field label="Năm xây dựng" required>
              <Input type="number" min="2000" max="9999" value={form.namXayDung} onChange={(event) => updateForm("namXayDung", event.target.value)} disabled={saving} />
            </Field>

            <Field label="Thời gian dự kiến bắt đầu">
              <Input type="date" value={form.thoiGianDuKienBatDau} onChange={(event) => updateForm("thoiGianDuKienBatDau", event.target.value)} disabled={saving} />
            </Field>
            <Field label="Thời gian dự kiến hoàn thành">
              <Input type="date" value={form.thoiGianDuKienHoanThanh} onChange={(event) => updateForm("thoiGianDuKienHoanThanh", event.target.value)} disabled={saving} />
            </Field>

            <div className="lg:col-span-2">
              <Field label="Mô tả">
                <TextArea rows={3} value={form.moTa} onChange={(value) => updateForm("moTa", value)} disabled={saving} />
              </Field>
            </div>
          </div>
        )}
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
        <h2 className="text-base font-semibold text-gray-800 dark:text-white/90">Thông tin soạn thảo</h2>
        <div className="mt-5 grid gap-5">
          <Field label="Căn cứ xây dựng">
            <TextArea rows={4} value={form.canCuXayDung} onChange={(value) => updateForm("canCuXayDung", value)} disabled={loading || saving} />
          </Field>
          <Field label="Phạm vi điều chỉnh">
            <TextArea rows={4} value={form.phamViDieuChinh} onChange={(value) => updateForm("phamViDieuChinh", value)} disabled={loading || saving} />
          </Field>
          <Field label="Nội dung chính sách">
            <TextArea rows={5} value={form.noiDungChinhSach} onChange={(value) => updateForm("noiDungChinhSach", value)} disabled={loading || saving} />
          </Field>
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 className="text-base font-semibold text-gray-800 dark:text-white/90">File đính kèm</h2>
            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Chọn tài liệu dự thảo, căn cứ hoặc tài liệu liên quan để lưu cùng hồ sơ.</p>
          </div>
          <label className="inline-flex h-10 cursor-pointer items-center justify-center rounded-lg border border-gray-300 bg-white px-4 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300">
            Chọn file
            <input
              type="file"
              multiple
              className="hidden"
              disabled={loading || saving}
              onChange={(event) => {
                addPendingFiles(event.target.files);
                event.target.value = "";
              }}
            />
          </label>
        </div>

        {existingFiles.length > 0 && (
          <div className="mt-5 overflow-hidden rounded-lg border border-gray-200 dark:border-white/[0.05]">
            <div className="border-b border-gray-100 bg-gray-50 px-4 py-3 text-sm font-medium text-gray-700 dark:border-white/[0.05] dark:bg-white/[0.03] dark:text-gray-300">
              Tài liệu đã lưu
            </div>
            <div className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {existingFiles.map((item) => (
                <div key={item.id} className="grid gap-2 px-4 py-3 text-sm text-gray-700 sm:grid-cols-[1fr_120px_120px] dark:text-gray-300">
                  <div>
                    <div className="font-medium text-gray-800 dark:text-white/90">{item.tenTaiLieu || item.tenFile}</div>
                    <div className="mt-1 text-xs text-gray-500">{item.tenFile}</div>
                  </div>
                  <div>Phiên bản {item.phienBan}</div>
                  <div>{formatBytes(item.dungLuong)}</div>
                </div>
              ))}
            </div>
          </div>
        )}

        {pendingFiles.length > 0 ? (
          <div className="mt-5 space-y-3">
            {pendingFiles.map((item) => (
              <div key={item.id} className="grid gap-3 rounded-lg border border-gray-200 p-4 sm:grid-cols-[1fr_180px_auto] sm:items-end dark:border-white/[0.05]">
                <Field label="Tên tài liệu" required>
                  <Input value={item.tenTaiLieu} onChange={(event) => updatePendingFile(item.id, event.target.value)} disabled={saving} />
                </Field>
                <div className="text-sm text-gray-500 dark:text-gray-400">
                  <div className="font-medium text-gray-700 dark:text-gray-300">{item.file.name}</div>
                  <div className="mt-1">{formatBytes(item.file.size)}</div>
                </div>
                <button
                  type="button"
                  onClick={() => removePendingFile(item.id)}
                  className="inline-flex h-11 items-center justify-center rounded-lg border border-gray-300 bg-white px-4 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300"
                  disabled={saving}
                >
                  Gỡ
                </button>
              </div>
            ))}
          </div>
        ) : (
          <div className="mt-5 rounded-lg border border-dashed border-gray-300 px-4 py-8 text-center text-sm text-gray-500 dark:border-gray-700 dark:text-gray-400">
            Chưa chọn file đính kèm.
          </div>
        )}
      </div>
    </form>
  );
}
