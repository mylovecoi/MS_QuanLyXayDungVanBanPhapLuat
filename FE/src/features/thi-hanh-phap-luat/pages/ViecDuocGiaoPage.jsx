import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert";
import Badge from "../../../app/components/ui/badge/Badge";
import Input from "../../../app/components/forms/input/InputField";
import { getViecDuocGiao } from "../api/thiHanhPhapLuatApi";

const formatDate = (value) => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "-";
const getErrorMessage = (error) => error?.response?.data?.message || error?.response?.data || error?.message || "Không thể tải danh sách công việc.";

function progressColor(value) {
  if (value >= 100) return "success";
  if (value > 0) return "warning";
  return "light";
}

export default function ViecDuocGiaoPage() {
  const navigate = useNavigate();
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState("");
  const [onlyOverdue, setOnlyOverdue] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      setItems(await getViecDuocGiao({ chiQuaHan: onlyOverdue }));
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void load(); }, [onlyOverdue]);

  const filteredItems = useMemo(() => {
    const keyword = search.trim().toLocaleLowerCase("vi-VN");
    if (!keyword) return items;
    return items.filter((item) => [item.maKeHoach, item.tenKeHoach, item.maNoiDung, item.tenNoiDung]
      .some((value) => value?.toLocaleLowerCase("vi-VN").includes(keyword)));
  }, [items, search]);

  return (
    <div className="space-y-5">
      <div>
        <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">Việc được giao</h1>
        <p className="mt-1 text-sm text-gray-500">Theo dõi các đầu việc được phân công và cập nhật kết quả thực hiện theo kỳ.</p>
      </div>

      {error && <Alert variant="error" title="Không thể tải dữ liệu" message={error} />}

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="flex flex-col gap-3 border-b border-gray-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-white/[0.05]">
          <div className="w-full max-w-xl"><Input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Tìm kế hoạch hoặc đầu việc..." /></div>
          <label className="flex shrink-0 cursor-pointer items-center gap-2 text-sm text-gray-600 dark:text-gray-300">
            <input type="checkbox" checked={onlyOverdue} onChange={(event) => setOnlyOverdue(event.target.checked)} />
            Chỉ việc quá hạn
          </label>
        </div>
        <div className="max-w-full overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-100 dark:border-white/[0.05]">
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Kế hoạch / đầu việc</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Vai trò</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Hạn thực hiện</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Tiến độ</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Báo cáo gần nhất</th>
                <th className="px-5 py-3 text-center text-xs font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Đang tải dữ liệu...</td></tr> : null}
              {!loading && !filteredItems.length ? <tr><td colSpan="6" className="px-5 py-10 text-center text-sm text-gray-500">Chưa có đầu việc được phân công.</td></tr> : null}
              {!loading && filteredItems.map((item) => (
                <tr key={item.phanCongId} className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                  <td className="px-5 py-4">
                    <div className="font-medium text-gray-800 dark:text-white/90">{item.tenNoiDung}</div>
                    <div className="mt-1 text-xs text-gray-500">{item.maNoiDung} · {item.maKeHoach} · {item.tenKeHoach}</div>
                  </td>
                  <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">{item.vaiTro || "-"}</td>
                  <td className="px-5 py-4 text-center text-sm text-gray-600 dark:text-gray-300">{formatDate(item.hanThucHien)}</td>
                  <td className="px-5 py-4 text-center"><Badge size="sm" color={progressColor(item.tyLeHoanThanh)}>{item.tyLeHoanThanh ?? 0}%</Badge></td>
                  <td className="px-5 py-4 text-sm text-gray-600 dark:text-gray-300">
                    {item.baoCaoGanNhat ? <><div>{item.baoCaoGanNhat.kyBaoCao} · {item.baoCaoGanNhat.tyLeHoanThanh}%</div><div className="mt-1 text-xs text-gray-500">{formatDate(item.baoCaoGanNhat.ngayBaoCao)}</div></> : "Chưa có báo cáo"}
                  </td>
                  <td className="px-5 py-4 text-center"><button type="button" onClick={() => navigate(`/thi-hanh-phap-luat/viec-duoc-giao/${item.phanCongId}${item.baoCaoGanNhat?.id ? `?baoCaoId=${item.baoCaoGanNhat.id}` : ""}`)} className="rounded-lg px-3 py-2 text-xs font-medium text-brand-500 hover:bg-gray-100">{item.baoCaoGanNhat ? "Cập nhật báo cáo" : "Lập báo cáo"}</button></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
