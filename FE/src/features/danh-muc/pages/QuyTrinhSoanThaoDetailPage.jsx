import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import Label from "../../../app/components/forms/Label";
import Select from "../../../app/components/forms/Select";
import { Modal } from "../../../app/components/ui/modal";
import { deleteBuocQuyTrinh, deleteChuyenBuocQuyTrinh, getQuyTrinhById, updateBuocQuyTrinh, updateChuyenBuocQuyTrinh } from "../api/quyTrinhSoanThaoApi";

const errorMessage = (error) => error?.response?.data?.message || error?.message || "Không thể tải thông tin quy trình.";
const boolText = (value) => value ? "Có" : "Không";
const toNumberOrNull = (value) => value === "" || value == null ? null : Number(value);
const stepFormFrom = (step) => ({
  ...step,
  soNgayXuLyTieuChuan: step.soNgayXuLyTieuChuan ?? "",
  soNgayCanhBaoSapHan: step.soNgayCanhBaoSapHan ?? "",
  soLuongPhanHoiToiThieu: step.soLuongPhanHoiToiThieu ?? "",
});
const transitionFormFrom = (transition) => ({
  ...transition,
  tuBuocMa: transition.tuBuocMa || "",
  denBuocMa: transition.denBuocMa || "",
});
const deadlineText = (step) => {
  const standardDays = step.soNgayXuLyTieuChuan;
  const warningDays = step.soNgayCanhBaoSapHan;
  if (standardDays == null && warningDays == null) return "-";
  return (
    <>
      {standardDays != null && <span>{standardDays} ngày xử lý</span>}
      {standardDays != null && warningDays != null && <br />}
      {warningDays != null && <span>Cảnh báo trước {warningDays} ngày</span>}
    </>
  );
};

function InfoItem({ label, value }) {
  return (
    <div>
      <div className="text-xs font-medium uppercase tracking-wide text-gray-500 dark:text-gray-400">{label}</div>
      <div className="mt-1 text-sm font-medium text-gray-800 dark:text-white/90">{value || "-"}</div>
    </div>
  );
}

function RowActions({ onEdit, onDelete }) {
  return (
    <div className="flex justify-center gap-1">
      <button
        type="button"
        onClick={onEdit}
        className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-brand-500 dark:text-gray-300 dark:hover:bg-white/[0.05]"
      >
        Sửa
      </button>
      <button
        type="button"
        onClick={onDelete}
        className="rounded-lg px-3 py-2 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-error-500 dark:text-gray-300 dark:hover:bg-white/[0.05]"
      >
        Xóa
      </button>
    </div>
  );
}

export default function QuyTrinhSoanThaoDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [actionLoading, setActionLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [editingStep, setEditingStep] = useState(null);
  const [editingTransition, setEditingTransition] = useState(null);

  const load = async (showLoading = true) => {
    try {
      if (showLoading) setLoading(true);
      setError("");
      const result = await getQuyTrinhById(id);
      setData(result);
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      if (showLoading) setLoading(false);
    }
  };

  useEffect(() => { load(); }, [id]);

  const stepsById = useMemo(() => {
    const map = new Map();
    (data?.buocQuyTrinhs || []).forEach((step) => map.set(step.id, step));
    return map;
  }, [data]);

  const closeStepModal = () => !actionLoading && setEditingStep(null);
  const closeTransitionModal = () => !actionLoading && setEditingTransition(null);

  const submitStep = async (event) => {
    event.preventDefault();
    if (!editingStep.maBuoc?.trim() || !editingStep.tenBuoc?.trim()) {
      setError("Vui lòng nhập mã bước và tên bước.");
      return;
    }
    try {
      setActionLoading(true);
      setError("");
      const payload = {
        ...editingStep,
        maBuoc: editingStep.maBuoc.trim(),
        tenBuoc: editingStep.tenBuoc.trim(),
        thuTuSapXep: Number(editingStep.thuTuSapXep) || 1,
        soLanTraLaiToiDa: Number(editingStep.soLanTraLaiToiDa) || 0,
        soNgayXuLyTieuChuan: toNumberOrNull(editingStep.soNgayXuLyTieuChuan),
        soNgayCanhBaoSapHan: toNumberOrNull(editingStep.soNgayCanhBaoSapHan),
        soLuongPhanHoiToiThieu: toNumberOrNull(editingStep.soLuongPhanHoiToiThieu),
      };
      await updateBuocQuyTrinh(id, editingStep.id, payload);
      setSuccess("Cập nhật bước quy trình thành công.");
      setEditingStep(null);
      await load(false);
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setActionLoading(false);
    }
  };

  const submitTransition = async (event) => {
    event.preventDefault();
    if (!editingTransition.tuBuocMa || !editingTransition.denBuocMa || !editingTransition.dieuKienKetQua?.trim()) {
      setError("Vui lòng nhập đầy đủ bước đi, bước đến và kết quả chuyển bước.");
      return;
    }
    try {
      setActionLoading(true);
      setError("");
      const payload = {
        ...editingTransition,
        dieuKienKetQua: editingTransition.dieuKienKetQua.trim(),
        loaiChuyenBuoc: editingTransition.loaiChuyenBuoc || "Forward",
      };
      await updateChuyenBuocQuyTrinh(id, editingTransition.id, payload);
      setSuccess("Cập nhật nhánh chuyển bước thành công.");
      setEditingTransition(null);
      await load(false);
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setActionLoading(false);
    }
  };

  const removeStep = async (step) => {
    if (!window.confirm(`Xóa bước "${step.tenBuoc}"? Các nhánh chuyển liên quan cũng sẽ được xóa.`)) return;
    try {
      setActionLoading(true);
      setError("");
      const response = await deleteBuocQuyTrinh(id, step.id);
      setSuccess(response.message || "Xóa bước quy trình thành công.");
      await load(false);
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setActionLoading(false);
    }
  };

  const removeTransition = async (transition) => {
    if (!window.confirm(`Xóa nhánh chuyển "${transition.dieuKienKetQua}"?`)) return;
    try {
      setActionLoading(true);
      setError("");
      const response = await deleteChuyenBuocQuyTrinh(id, transition.id);
      setSuccess(response.message || "Xóa nhánh chuyển bước thành công.");
      await load(false);
    } catch (e) {
      setError(errorMessage(e));
    } finally {
      setActionLoading(false);
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <button
            type="button"
            onClick={() => navigate("/admin/danh-muc/quy-trinh-soan-thao")}
            className="mb-3 text-sm font-medium text-brand-500 hover:text-brand-600"
          >
            Quay lại danh sách
          </button>
          <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Chi tiết quy trình soạn thảo</h1>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Xem cấu hình nghiệp vụ, bước xử lý và nhánh chuyển của quy trình.</p>
        </div>
        {data && (
          <Badge size="sm" color={data.trangThai ? "success" : "light"}>
            {data.trangThai ? "Đang dùng" : "Tạm dừng"}
          </Badge>
        )}
      </div>

      {error && <Alert variant="error" title="Không thể tải dữ liệu" message={error} />}
      {success && <Alert variant="success" title="Hoàn tất" message={success} />}

      {loading ? (
        <div className="rounded-xl border border-gray-200 bg-white p-10 text-center text-sm text-gray-500 dark:border-white/[0.05] dark:bg-white/[0.03] dark:text-gray-400">Đang tải thông tin quy trình...</div>
      ) : data ? (
        <>
          <div className="rounded-xl border border-gray-200 bg-white p-5 dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="flex flex-col gap-3 border-b border-gray-100 pb-5 dark:border-white/[0.05]">
              <div>
                <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">{data.tenQuyTrinh}</h2>
                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">{data.moTa || "Chưa có mô tả."}</p>
              </div>
              <div className="flex flex-wrap gap-2 text-xs text-gray-500">
                <span className="rounded-md bg-gray-100 px-2 py-1 dark:bg-white/[0.06]">{data.maQuyTrinh}</span>
                <span className="rounded-md bg-gray-100 px-2 py-1 dark:bg-white/[0.06]">Phiên bản {data.phienBan}</span>
              </div>
            </div>
            <div className="grid gap-5 pt-5 sm:grid-cols-2 lg:grid-cols-4">
              <InfoItem label="Loại quy trình" value={data.tenLoaiQuyTrinh || data.loaiQuyTrinh} />
              <InfoItem label="Cấp áp dụng" value={data.capApDung || data.capApDungs?.join(", ")} />
              <InfoItem label="Loại văn bản" value={data.tenLoaiVanBan} />
              <InfoItem label="Bước / nhánh" value={`${data.soBuoc ?? data.buocQuyTrinhs?.length ?? 0} / ${data.soNhanhChuyen ?? data.chuyenBuocs?.length ?? 0}`} />
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]">
              <h3 className="font-semibold text-gray-800 dark:text-white/90">Các bước xử lý</h3>
            </div>
            <div className="max-w-full overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Thứ tự</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Bước xử lý</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Loại bước</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Thời hạn xử lý</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Ràng buộc</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Mô tả</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                  {(data.buocQuyTrinhs || []).length ? data.buocQuyTrinhs.map((step) => (
                    <tr key={step.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{step.thuTuSapXep}</td>
                      <td className="px-5 py-4">
                        <div className="font-medium text-gray-800 dark:text-white/90">{step.tenBuoc}</div>
                        <div className="mt-1 text-xs text-gray-500">{step.maBuoc}</div>
                      </td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{step.loaiBuoc}</td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{deadlineText(step)}</td>
                      <td className="px-5 py-4 text-center text-xs text-gray-500">
                        Bắt buộc: {boolText(step.batBuoc)}<br />
                        Quay lui: {boolText(step.choPhepQuayLui)}<br />
                        File: {boolText(step.yeuCauFileDinhKem)}
                      </td>
                      <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{step.moTa || "-"}</td>
                      <td className="px-5 py-4">
                        <RowActions
                          onEdit={() => setEditingStep(stepFormFrom(step))}
                          onDelete={() => !actionLoading && removeStep(step)}
                        />
                      </td>
                    </tr>
                  )) : (
                    <tr><td colSpan="7" className="px-5 py-10 text-center text-sm text-gray-500">Chưa khai báo bước xử lý.</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          <div className="rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
            <div className="border-b border-gray-100 px-5 py-4 dark:border-white/[0.05]">
              <h3 className="font-semibold text-gray-800 dark:text-white/90">Nhánh chuyển bước</h3>
            </div>
            <div className="max-w-full overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Từ bước</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Đến bước</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Kết quả</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Cấu hình</th>
                    <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Mô tả</th>
                    <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
                  {(data.chuyenBuocs || []).length ? data.chuyenBuocs.map((transition) => {
                    const fromStep = stepsById.get(transition.tuBuocId);
                    const toStep = stepsById.get(transition.denBuocId);
                    return (
                      <tr key={transition.id} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                        <td className="px-5 py-4 text-sm text-gray-700 dark:text-gray-300">{fromStep?.tenBuoc || transition.tuBuocMa || "-"}</td>
                        <td className="px-5 py-4 text-sm text-gray-700 dark:text-gray-300">{toStep?.tenBuoc || transition.denBuocMa || "-"}</td>
                        <td className="px-5 py-4">
                          <div className="font-medium text-gray-800 dark:text-white/90">{transition.dieuKienKetQua}</div>
                          <div className="mt-1 text-xs text-gray-500">{transition.loaiChuyenBuoc}</div>
                        </td>
                        <td className="px-5 py-4 text-center text-xs text-gray-500">
                          Mặc định: {boolText(transition.laNhanhMacDinh)}<br />
                          Lý do: {boolText(transition.yeuCauNhapLyDo)}<br />
                          Kết thúc: {boolText(transition.isKetThuc)}
                        </td>
                        <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{transition.moTa || "-"}</td>
                        <td className="px-5 py-4">
                          <RowActions
                            onEdit={() => setEditingTransition(transitionFormFrom(transition))}
                            onDelete={() => !actionLoading && removeTransition(transition)}
                          />
                        </td>
                      </tr>
                    );
                  }) : (
                    <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Chưa khai báo nhánh chuyển bước.</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          <Modal isOpen={Boolean(editingStep)} onClose={closeStepModal} className="max-w-3xl p-6">
            {editingStep && (
              <form onSubmit={submitStep} className="space-y-5">
                <div>
                  <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">Sửa bước xử lý</h4>
                  <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Cập nhật thông tin cấu hình của bước trong quy trình.</p>
                </div>
                <div className="grid gap-4 sm:grid-cols-2">
                  <div>
                    <Label>Mã bước *</Label>
                    <Input value={editingStep.maBuoc} onChange={(e) => setEditingStep({ ...editingStep, maBuoc: e.target.value })} disabled={actionLoading} />
                  </div>
                  <div>
                    <Label>Tên bước *</Label>
                    <Input value={editingStep.tenBuoc} onChange={(e) => setEditingStep({ ...editingStep, tenBuoc: e.target.value })} disabled={actionLoading} />
                  </div>
                  <div>
                    <Label>Thứ tự</Label>
                    <Input type="number" min="1" value={editingStep.thuTuSapXep} onChange={(e) => setEditingStep({ ...editingStep, thuTuSapXep: e.target.value })} disabled={actionLoading} />
                  </div>
                  <div>
                    <Label>Loại bước</Label>
                    <Select value={editingStep.loaiBuoc} onChange={(value) => setEditingStep({ ...editingStep, loaiBuoc: value })} options={[{ value: "NhapLieu", label: "Nhập liệu" }, { value: "XuLy", label: "Xử lý" }, { value: "Forward", label: "Chuyển tiếp" }, { value: "ThamDinh", label: "Thẩm định" }, { value: "ThamTra", label: "Thẩm tra" }, { value: "LayYKien", label: "Lấy ý kiến" }, { value: "DanhGia", label: "Đánh giá" }, { value: "PheDuyet", label: "Phê duyệt" }, { value: "KetThuc", label: "Kết thúc" }]} />
                  </div>
                  <div>
                    <Label>Số ngày xử lý tiêu chuẩn</Label>
                    <Input type="number" min="1" value={editingStep.soNgayXuLyTieuChuan} onChange={(e) => setEditingStep({ ...editingStep, soNgayXuLyTieuChuan: e.target.value })} disabled={actionLoading} />
                  </div>
                  <div>
                    <Label>Cảnh báo sắp hạn trước số ngày</Label>
                    <Input type="number" min="0" value={editingStep.soNgayCanhBaoSapHan} onChange={(e) => setEditingStep({ ...editingStep, soNgayCanhBaoSapHan: e.target.value })} disabled={actionLoading} />
                  </div>
                  <div>
                    <Label>Số lần trả lại tối đa</Label>
                    <Input type="number" min="0" value={editingStep.soLanTraLaiToiDa ?? 0} onChange={(e) => setEditingStep({ ...editingStep, soLanTraLaiToiDa: e.target.value })} disabled={actionLoading} />
                  </div>
                  <div>
                    <Label>Số phản hồi tối thiểu</Label>
                    <Input type="number" min="0" value={editingStep.soLuongPhanHoiToiThieu} onChange={(e) => setEditingStep({ ...editingStep, soLuongPhanHoiToiThieu: e.target.value })} disabled={actionLoading} />
                  </div>
                </div>
                <div className="grid gap-3 sm:grid-cols-2">
                  <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300"><input type="checkbox" checked={Boolean(editingStep.batBuoc)} onChange={(e) => setEditingStep({ ...editingStep, batBuoc: e.target.checked })} /> Bắt buộc</label>
                  <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300"><input type="checkbox" checked={Boolean(editingStep.choPhepBoQua)} onChange={(e) => setEditingStep({ ...editingStep, choPhepBoQua: e.target.checked })} /> Cho phép bỏ qua</label>
                  <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300"><input type="checkbox" checked={Boolean(editingStep.choPhepQuayLui)} onChange={(e) => setEditingStep({ ...editingStep, choPhepQuayLui: e.target.checked })} /> Cho phép quay lui</label>
                  <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300"><input type="checkbox" checked={Boolean(editingStep.yeuCauFileDinhKem)} onChange={(e) => setEditingStep({ ...editingStep, yeuCauFileDinhKem: e.target.checked })} /> Yêu cầu file đính kèm</label>
                </div>
                <div>
                  <Label>Mô tả</Label>
                  <Input value={editingStep.moTa || ""} onChange={(e) => setEditingStep({ ...editingStep, moTa: e.target.value })} disabled={actionLoading} />
                </div>
                <div className="flex justify-end gap-3 border-t border-gray-100 pt-5 dark:border-white/[0.05]">
                  <button type="button" onClick={closeStepModal} disabled={actionLoading} className="rounded-lg border px-4 py-2.5 text-sm font-medium disabled:opacity-50">Hủy</button>
                  <button disabled={actionLoading} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">{actionLoading ? "Đang lưu..." : "Lưu bước"}</button>
                </div>
              </form>
            )}
          </Modal>

          <Modal isOpen={Boolean(editingTransition)} onClose={closeTransitionModal} className="max-w-3xl p-6">
            {editingTransition && (
              <form onSubmit={submitTransition} className="space-y-5">
                <div>
                  <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90">Sửa nhánh chuyển bước</h4>
                  <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Cập nhật điều kiện và hướng chuyển trong quy trình.</p>
                </div>
                <div className="grid gap-4 sm:grid-cols-2">
                  <div>
                    <Label>Từ bước *</Label>
                    <Select value={editingTransition.tuBuocMa} onChange={(value) => setEditingTransition({ ...editingTransition, tuBuocMa: value })} options={(data.buocQuyTrinhs || []).map((step) => ({ value: step.maBuoc, label: step.tenBuoc }))} />
                  </div>
                  <div>
                    <Label>Đến bước *</Label>
                    <Select value={editingTransition.denBuocMa} onChange={(value) => setEditingTransition({ ...editingTransition, denBuocMa: value })} options={(data.buocQuyTrinhs || []).map((step) => ({ value: step.maBuoc, label: step.tenBuoc }))} />
                  </div>
                  <div>
                    <Label>Điều kiện kết quả *</Label>
                    <Input value={editingTransition.dieuKienKetQua} onChange={(e) => setEditingTransition({ ...editingTransition, dieuKienKetQua: e.target.value })} disabled={actionLoading} />
                  </div>
                  <div>
                    <Label>Loại chuyển bước</Label>
                    <Select value={editingTransition.loaiChuyenBuoc} onChange={(value) => setEditingTransition({ ...editingTransition, loaiChuyenBuoc: value })} options={[{ value: "Forward", label: "Chuyển tiếp" }, { value: "Approve", label: "Phê duyệt" }, { value: "Return", label: "Trả lại" }, { value: "Reject", label: "Từ chối" }]} />
                  </div>
                </div>
                <div className="grid gap-3 sm:grid-cols-3">
                  <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300"><input type="checkbox" checked={Boolean(editingTransition.laNhanhMacDinh)} onChange={(e) => setEditingTransition({ ...editingTransition, laNhanhMacDinh: e.target.checked })} /> Nhánh mặc định</label>
                  <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300"><input type="checkbox" checked={Boolean(editingTransition.yeuCauNhapLyDo)} onChange={(e) => setEditingTransition({ ...editingTransition, yeuCauNhapLyDo: e.target.checked })} /> Yêu cầu lý do</label>
                  <label className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300"><input type="checkbox" checked={Boolean(editingTransition.isKetThuc)} onChange={(e) => setEditingTransition({ ...editingTransition, isKetThuc: e.target.checked })} /> Kết thúc</label>
                </div>
                <div>
                  <Label>Mô tả</Label>
                  <Input value={editingTransition.moTa || ""} onChange={(e) => setEditingTransition({ ...editingTransition, moTa: e.target.value })} disabled={actionLoading} />
                </div>
                <div className="flex justify-end gap-3 border-t border-gray-100 pt-5 dark:border-white/[0.05]">
                  <button type="button" onClick={closeTransitionModal} disabled={actionLoading} className="rounded-lg border px-4 py-2.5 text-sm font-medium disabled:opacity-50">Hủy</button>
                  <button disabled={actionLoading} className="rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white disabled:opacity-50">{actionLoading ? "Đang lưu..." : "Lưu nhánh"}</button>
                </div>
              </form>
            )}
          </Modal>
        </>
      ) : null}
    </div>
  );
}
