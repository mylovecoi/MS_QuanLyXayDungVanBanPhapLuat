import { Fragment, useEffect, useMemo, useState } from "react";
import { useParams, useSearchParams } from "react-router";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import { getBaoCao } from "../api/baoCaoApi";

const message = (error) =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  error?.response?.data ||
  error?.message ||
  "Không thể tải báo cáo.";

function formatDate(value) {
  if (!value) return "-";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "-";
  return date.toLocaleDateString("vi-VN");
}

function formatDateTime(value) {
  if (!value) return "-";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "-";
  return date.toLocaleString("vi-VN");
}

function escapeHtml(value) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function fileSafeName(value) {
  return String(value || "bao-cao")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-zA-Z0-9_-]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase();
}

function csvValue(value) {
  const text = value == null ? "" : String(value);
  return `"${text.replace(/"/g, '""')}"`;
}

function dateRangeText(filters) {
  if (filters.tuNgay || filters.denNgay) {
    return `Từ ngày: ${formatDate(filters.tuNgay)} đến ngày: ${formatDate(filters.denNgay)}`;
  }
  if (filters.nam) return `Báo cáo năm ${filters.nam}`;
  return "Báo cáo toàn kỳ";
}

function getUnitName(item) {
  return item.tenDonVi || item.donViId || "Chưa xác định đơn vị";
}

function getStatusName(item) {
  return item.trangThai || "Chưa xác định trạng thái";
}

function groupByUnitAndStatus(rows = []) {
  return rows.reduce((groups, item) => {
    const unit = getUnitName(item);
    const status = getStatusName(item);
    if (!groups[unit]) groups[unit] = {};
    if (!groups[unit][status]) groups[unit][status] = [];
    groups[unit][status].push(item);
    return groups;
  }, {});
}

function downloadBlob(blob, fileName) {
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

function exportCsv(report) {
  const rows = report?.chiTiet || [];
  const header = [
    "STT",
    "Đơn vị chủ trì",
    "Trạng thái",
    "Mã đối tượng",
    "Tên đối tượng",
    "Nội dung",
    "Ngày phát sinh",
    "Hạn hoàn thành",
    "Ghi chú",
  ];
  const content = [
    header.map(csvValue).join(","),
    ...rows.map((item, index) => [
      index + 1,
      getUnitName(item),
      getStatusName(item),
      item.maDoiTuong,
      item.tenDoiTuong,
      item.noiDung,
      formatDate(item.ngayPhatSinh),
      formatDate(item.hanHoanThanh),
      item.ghiChu,
    ].map(csvValue).join(",")),
  ].join("\r\n");
  downloadBlob(
    new Blob([`\uFEFF${content}`], { type: "text/csv;charset=utf-8" }),
    `${fileSafeName(report?.loaiBaoCao)}.csv`,
  );
}

function buildReportHtml(report, filters) {
  const grouped = groupByUnitAndStatus(report?.chiTiet || []);
  const rowsHtml = Object.entries(grouped).map(([unit, statuses]) => (
    `<tr><td colspan="9" class="group-row">Đơn vị chủ trì: ${escapeHtml(unit)}</td></tr>` +
    Object.entries(statuses).map(([status, rows]) => (
      `<tr><td colspan="9" class="status-row">Trạng thái: ${escapeHtml(status)} (${rows.length})</td></tr>` +
      rows.map((item, index) => `
        <tr>
          <td class="center">${index + 1}</td>
          <td>${escapeHtml(item.maDoiTuong || "")}</td>
          <td>${escapeHtml(item.tenDoiTuong || "")}</td>
          <td>${escapeHtml(item.noiDung || "")}</td>
          <td>${escapeHtml(formatDate(item.ngayPhatSinh))}</td>
          <td>${escapeHtml(formatDate(item.hanHoanThanh))}</td>
          <td>${escapeHtml(getUnitName(item))}</td>
          <td>${escapeHtml(status)}</td>
          <td>${escapeHtml(item.ghiChu || "")}</td>
        </tr>
      `).join("")
    )).join("")
  )).join("");

  return `
    <!doctype html>
    <html>
      <head>
        <meta charset="utf-8" />
        <style>
          body { font-family: "Times New Roman", serif; font-size: 13px; color: #111827; }
          .meta { margin: 18px 0 8px 34px; line-height: 1.7; }
          h1 { margin: 22px 0 8px; text-align: center; font-size: 18px; text-transform: uppercase; }
          .subtitle { text-align: center; font-style: italic; margin-bottom: 22px; }
          table { width: 100%; border-collapse: collapse; }
          th, td { border: 1px solid #222; padding: 7px 8px; vertical-align: middle; }
          th { text-align: center; font-weight: 700; }
          .center { text-align: center; }
          .group-row { font-weight: 700; background: #f3f4f6; }
          .status-row { font-style: italic; background: #fafafa; }
        </style>
      </head>
      <body>
        <div class="meta">
          <div><strong>Đơn vị:</strong> UBND tỉnh Khánh Hòa</div>
          <div><strong>Mã đơn vị SDNS:</strong></div>
        </div>
        <h1>${escapeHtml(report?.tenBaoCao || "Báo cáo thống kê")}</h1>
        <div class="subtitle">${escapeHtml(dateRangeText(filters))}</div>
        <table>
          <thead>
            <tr>
              <th>STT</th>
              <th>Mã</th>
              <th>Tên đối tượng</th>
              <th>Nội dung</th>
              <th>Ngày phát sinh</th>
              <th>Hạn hoàn thành</th>
              <th>Đơn vị chủ trì</th>
              <th>Trạng thái</th>
              <th>Ghi chú</th>
            </tr>
          </thead>
          <tbody>${rowsHtml || '<tr><td colspan="9" class="center">Không có dữ liệu thỏa mãn điều kiện.</td></tr>'}</tbody>
        </table>
      </body>
    </html>
  `;
}

function exportWord(report, filters) {
  downloadBlob(
    new Blob([`\uFEFF${buildReportHtml(report, filters)}`], { type: "application/msword;charset=utf-8" }),
    `${fileSafeName(report?.loaiBaoCao)}.doc`,
  );
}

function exportExcel(report, filters) {
  downloadBlob(
    new Blob([`\uFEFF${buildReportHtml(report, filters)}`], { type: "application/vnd.ms-excel;charset=utf-8" }),
    `${fileSafeName(report?.loaiBaoCao)}.xls`,
  );
}

function SummaryLine({ label, value }) {
  return (
    <span className="inline-flex items-center gap-2">
      <span className="text-gray-500">{label}</span>
      <strong className="font-semibold text-gray-800">{value}</strong>
    </span>
  );
}

function ReportTable({ rows }) {
  const grouped = groupByUnitAndStatus(rows);
  const unitEntries = Object.entries(grouped);
  let order = 0;

  return (
    <div className="overflow-x-auto">
      <table className="report-table min-w-[1180px]">
        <thead>
          <tr>
            <th className="w-14">STT</th>
            <th className="w-36">Mã</th>
            <th className="min-w-[260px]">Tên đối tượng</th>
            <th className="min-w-[320px]">Nội dung</th>
            <th className="w-32">Ngày phát sinh</th>
            <th className="w-32">Hạn hoàn thành</th>
            <th className="w-56">Đơn vị chủ trì</th>
            <th className="w-44">Trạng thái</th>
            <th className="min-w-[220px]">Ghi chú</th>
          </tr>
        </thead>
        <tbody>
          {!unitEntries.length ? (
            <tr>
              <td colSpan="9" className="h-24 text-center">Không có dữ liệu thỏa mãn điều kiện.</td>
            </tr>
          ) : unitEntries.map(([unit, statuses]) => (
            <Fragment key={unit}>
              <tr className="report-group-row">
                <td colSpan="9">Đơn vị chủ trì: {unit}</td>
              </tr>
              {Object.entries(statuses).map(([status, statusRows]) => (
                <Fragment key={`${unit}-${status}`}>
                  <tr className="report-status-row">
                    <td colSpan="9">Trạng thái: {status} ({statusRows.length})</td>
                  </tr>
                  {statusRows.map((item) => {
                    order += 1;
                    return (
                      <tr key={item.id || `${unit}-${status}-${order}`}>
                        <td className="text-center">{order}</td>
                        <td>{item.maDoiTuong || "-"}</td>
                        <td>{item.tenDoiTuong || "-"}</td>
                        <td>{item.noiDung || "-"}</td>
                        <td className="text-center">{formatDate(item.ngayPhatSinh)}</td>
                        <td className="text-center">{formatDate(item.hanHoanThanh)}</td>
                        <td>{getUnitName(item)}</td>
                        <td className="text-center">{status}</td>
                        <td>{item.ghiChu || "-"}</td>
                      </tr>
                    );
                  })}
                </Fragment>
              ))}
            </Fragment>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default function BaoCaoKetQuaPage() {
  const { maBaoCao } = useParams();
  const [searchParams] = useSearchParams();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const filters = useMemo(() => ({
    nam: searchParams.get("nam") || "",
    donViId: searchParams.get("donViId") || "",
    trangThaiId: searchParams.get("trangThaiId") || "",
    tuNgay: searchParams.get("tuNgay") || "",
    denNgay: searchParams.get("denNgay") || "",
  }), [searchParams]);

  const load = async () => {
    try {
      setLoading(true);
      setError("");
      setData(await getBaoCao(maBaoCao, filters));
    } catch (requestError) {
      setError(message(requestError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, [maBaoCao, filters]);

  const totalRows = data?.chiTiet?.length || 0;

  return (
    <div className="report-result-page">
      <style>{`
        .report-result-page {
          margin: -8px -8px 0;
          color: #111827;
          font-family: Arial, sans-serif;
        }
        .report-toolbar {
          position: sticky;
          top: 0;
          z-index: 20;
          display: flex;
          flex-wrap: wrap;
          gap: 10px;
          padding: 10px 12px;
          border-bottom: 1px solid #cbd5e1;
          background: #f8fafc;
        }
        .report-toolbar button {
          border: 1px solid #d1d5db;
          border-radius: 6px;
          padding: 9px 14px;
          font-size: 13px;
          font-weight: 600;
          background: #ffffff;
          color: #111827;
        }
        .report-toolbar button.primary {
          border-color: #2563eb;
          background: #2563eb;
          color: #ffffff;
        }
        .report-toolbar button.success {
          border-color: #0f766e;
          background: #0f766e;
          color: #ffffff;
        }
        .report-toolbar button:disabled {
          cursor: not-allowed;
          opacity: 0.55;
        }
        .report-paper {
          min-height: calc(100vh - 88px);
          padding: 18px 5vw 40px;
          background: #eef2f8;
        }
        .report-sheet {
          max-width: 1720px;
          margin: 0 auto;
          padding: 18px 28px 32px;
          background: #eef2f8;
        }
        .report-meta {
          margin: 0 0 12px 6px;
          font-family: "Times New Roman", serif;
          font-size: 14px;
          line-height: 1.8;
        }
        .report-title {
          margin-top: 14px;
          text-align: center;
          font-family: "Times New Roman", serif;
        }
        .report-title h1 {
          margin: 0;
          font-size: 21px;
          font-weight: 700;
          text-transform: uppercase;
        }
        .report-title p {
          margin-top: 10px;
          font-size: 14px;
          font-style: italic;
        }
        .report-summary {
          display: flex;
          flex-wrap: wrap;
          gap: 12px 26px;
          margin: 22px 0 12px;
          font-size: 13px;
        }
        .report-table {
          width: 100%;
          border-collapse: collapse;
          font-family: "Times New Roman", serif;
          font-size: 13px;
          background: transparent;
        }
        .report-table th,
        .report-table td {
          border: 1px solid #1f2937;
          padding: 8px 8px;
          vertical-align: middle;
        }
        .report-table th {
          text-align: center;
          font-weight: 700;
        }
        .report-group-row td {
          background: #dbeafe;
          font-weight: 700;
        }
        .report-status-row td {
          background: #f8fafc;
          font-style: italic;
          font-weight: 600;
        }
        @page {
          size: A4 landscape;
          margin: 12mm;
        }
        @media print {
          body {
            background: #ffffff !important;
          }
          .report-toolbar,
          .print\\:hidden,
          aside,
          header,
          nav {
            display: none !important;
          }
          .report-result-page {
            margin: 0 !important;
          }
          .report-paper,
          .report-sheet {
            min-height: auto !important;
            max-width: none !important;
            margin: 0 !important;
            padding: 0 !important;
            background: #ffffff !important;
          }
          .report-summary {
            break-inside: avoid;
          }
          .report-table {
            min-width: 0 !important;
            font-size: 11px;
          }
          .report-table th,
          .report-table td {
            padding: 5px 6px;
          }
          .report-group-row td,
          .report-status-row td {
            background: #ffffff !important;
          }
        }
      `}</style>

      <div className="report-toolbar">
        <button type="button" onClick={() => window.print()} disabled={loading || !data} className="success">In dữ liệu</button>
        <button type="button" onClick={() => exportWord(data, filters)} disabled={loading || !data}>Kết xuất file Doc</button>
        <button type="button" onClick={() => exportExcel(data, filters)} disabled={loading || !data} className="primary">Export Excel</button>
        <button type="button" onClick={() => exportCsv(data)} disabled={loading || !totalRows}>Xuất CSV</button>
        <button type="button" onClick={load} disabled={loading}>Tải lại</button>
      </div>

      {error && <div className="mx-4 mt-4 print:hidden"><Alert variant="error" title="Có lỗi" message={error} /></div>}

      <div className="report-paper">
        <div className="report-sheet">
          {loading ? (
            <div className="py-20 text-center text-sm text-gray-500">Đang tải báo cáo...</div>
          ) : data ? (
            <>
              <div className="report-meta">
                <div><strong>Đơn vị:</strong> UBND tỉnh Khánh Hòa</div>
                <div><strong>Mã đơn vị SDNS:</strong></div>
              </div>

              <div className="report-title">
                <h1>{data.tenBaoCao || "Báo cáo thống kê"}</h1>
                <p>{dateRangeText(filters)}</p>
              </div>

              <div className="report-summary">
                <SummaryLine label="Mã báo cáo:" value={data.loaiBaoCao || maBaoCao} />
                <SummaryLine label="Tổng số dòng:" value={totalRows} />
                <SummaryLine label="Tổng hợp lúc:" value={formatDateTime(data.ngayTongHop)} />
                {filters.donViId && <SummaryLine label="Đơn vị lọc:" value={filters.donViId} />}
                {filters.trangThaiId && <SummaryLine label="Trạng thái lọc:" value={filters.trangThaiId} />}
              </div>

              <ReportTable rows={data.chiTiet || []} />
            </>
          ) : (
            <div className="py-20 text-center text-sm text-gray-500">Không có dữ liệu báo cáo.</div>
          )}
        </div>
      </div>
    </div>
  );
}
