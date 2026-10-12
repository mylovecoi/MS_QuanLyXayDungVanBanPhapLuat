import {useEffect, useMemo, useState} from "react";
import {useNavigate} from "react-router";
import Chart from "react-apexcharts";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import {getDonViOptions} from "../../danh-muc/api/donViApi";
import {getQuyTrinhs} from "../../danh-muc/api/quyTrinhSoanThaoApi";
import {getTrangThais} from "../../danh-muc/api/trangThaiApi";
import {getDashboardLayout} from "../api/dashboardApi";

const currentYear = new Date().getFullYear();
const initialFilters = {nam: String(currentYear), donViId: "", quyTrinhSoanThaoId: "", trangThaiId: ""};
const itemsOf = (response) => response?.data || response?.items || [];
const errorMessage = (error) => error?.response?.data?.message || error?.response?.data?.title || error?.message || "Không thể tải dashboard.";

function userInfo() {
    try {
        return JSON.parse(localStorage.getItem("userInfo") || "{}");
    } catch {
        return {};
    }
}

function Icon({name}) {
    const icons = {
        documents: <>
            <path d="M7 3.75h7.3L18 7.45v12.8H7z"/>
            <path d="M14 3.75v3.7h4"/>
            <path d="M10 11h5M10 14.5h5"/>
        </>,
        clock: <>
            <circle cx="12" cy="12" r="8.25"/>
            <path d="M12 7.8v4.5l3 1.8"/>
        </>,
        check: <>
            <circle cx="12" cy="12" r="8.25"/>
            <path d="m8.5 12 2.25 2.25 4.75-5"/>
        </>,
        warning: <>
            <path
                d="M10.2 4.5 3.7 16a1.35 1.35 0 0 0 1.18 2h14.24A1.35 1.35 0 0 0 20.3 16L13.8 4.5a2.07 2.07 0 0 0-3.6 0Z"/>
            <path d="M12 8.5v4M12 15.5h.01"/>
        </>,
        bell: <>
            <path d="M18 10a6 6 0 0 0-12 0c0 7-3 7-3 8.5h18C21 17 18 17 18 10Z"/>
            <path d="M10 21h4"/>
        </>,
    };
    return <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round"
                strokeLinejoin="round" aria-hidden="true">{icons[name]}</svg>;
}

function MetricCard({label, value, hint, tone, icon}) {
    const tones = {
        brand: "text-brand-600 dark:text-brand-400",
        blue: "text-blue-light-600 dark:text-blue-light-400",
        success: "text-success-600 dark:text-success-400",
        warning: "text-warning-600 dark:text-warning-400",
        error: "text-error-600 dark:text-error-400",
    };

    return (
        <article
            className="rounded-2xl border border-[var(--admin-border)] bg-[var(--admin-surface)] p-5 shadow-[var(--admin-shadow-soft)]">
            <div className="flex items-start justify-between gap-3">
                <div>
                    <p className="text-sm font-medium text-gray-500 dark:text-gray-400">
                        {label}
                    </p>
                    <p className="mt-2 text-3xl font-bold tracking-tight text-gray-900 dark:text-white">
                        {value}
                    </p>
                </div>

                <span className={`flex h-11 w-11 shrink-0 items-center justify-center ${tones[tone] || tones.brand}`}>
                    <Icon name={icon}/>
                </span>
            </div>

            <p className="mt-3 text-xs text-gray-500 dark:text-gray-400">
                {hint}
            </p>
        </article>
    );
}

function ChartCard({title, subtitle, action, children}) {
    return <section
        className="rounded-2xl border border-[var(--admin-border)] bg-[var(--admin-surface)] p-5 shadow-[var(--admin-shadow-soft)]">
        <div className="mb-4 flex items-start justify-between gap-3">
            <div><h2 className="font-semibold text-gray-900 dark:text-white">{title}</h2><p
                className="mt-1 text-sm text-gray-500 dark:text-gray-400">{subtitle}</p></div>
            {action}</div>
        {children}</section>;
}

function EmptyChart() {
    return <div
        className="flex h-64 items-center justify-center rounded-xl bg-gray-50 text-center text-sm text-gray-500 dark:bg-white/[0.03]">Chưa
        có dữ liệu trong phạm vi đã chọn.</div>;
}

function ProgressList({rows, colors = ["bg-brand-500", "bg-success-500", "bg-warning-500", "bg-error-500"]}) {
    const max = Math.max(...rows.map((item) => item.soLuong || 0), 1);
    return <div className="space-y-4">{rows.map((item, index) => <div key={item.nhom}>
        <div className="mb-1.5 flex items-center justify-between gap-3 text-sm"><span
            className="truncate text-gray-600 dark:text-gray-300">{item.nhom}</span><span
            className="font-semibold text-gray-900 dark:text-white">{item.soLuong}</span></div>
        <div className="h-2 overflow-hidden rounded-full bg-gray-100 dark:bg-white/10">
            <div className={`h-full rounded-full ${colors[index % colors.length]}`}
                 style={{width: `${Math.max(3, Math.round((item.soLuong || 0) * 100 / max))}%`}}/>
        </div>
    </div>)}</div>;
}

export default function DashboardKhaiThacDuLieuPage() {
    const navigate = useNavigate();
    const [layout, setLayout] = useState(null);
    const [filters, setFilters] = useState(initialFilters);
    const [options, setOptions] = useState({donVis: [], quyTrinhs: [], trangThais: []});
    const [loading, setLoading] = useState(true);
    const [loadingOptions, setLoadingOptions] = useState(true);
    const [error, setError] = useState("");
    const isSSA = useMemo(() => Boolean(userInfo().isSSA), []);
    const queryFilters = useMemo(() => ({
        nam: filters.nam ? Number(filters.nam) : undefined,
        donViId: filters.donViId || undefined,
        quyTrinhSoanThaoId: filters.quyTrinhSoanThaoId || undefined,
        trangThaiId: filters.trangThaiId || undefined
    }), [filters]);
    const byCode = useMemo(() => Object.fromEntries((layout?.widgets || []).map((item) => [item.maWidget, item.duLieu || {
        giaTri: 0,
        nhomSoLieu: []
    }])), [layout]);
    const data = (code) => byCode[code] || {giaTri: 0, nhomSoLieu: []};
    const countOf = (rows, name) => rows.find((item) => item.nhom === name)?.soLuong || 0;
    const construction = data("TIEN_DO_XAY_DUNG_VAN_BAN");
    const tasks = data("TO_CHUC_THI_HANH_PHAP_LUAT");
    const alerts = data("TONG_QUAN_DIEU_HANH");
    const stages = data("HO_SO_SAP_HAN");
    const units = data("TY_LE_BAN_HANH_THEO_DON_VI");

    const load = async () => {
        try {
            setLoading(true);
            setError("");
            setLayout(await getDashboardLayout(queryFilters));
        } catch (requestError) {
            setError(errorMessage(requestError));
        } finally {
            setLoading(false);
        }
    };
    useEffect(() => {
        void load();
    }, [queryFilters]);
    useEffect(() => {
        const loadOptions = async () => {
            try {
                setLoadingOptions(true);
                const [donVi, quyTrinh, trangThai] = await Promise.allSettled([getDonViOptions(), getQuyTrinhs({
                    pageSize: 200,
                    pageCurrent: 1
                }), getTrangThais({pageSize: 200, pageCurrent: 1})]);
                setOptions({
                    donVis: donVi.status === "fulfilled" ? donVi.value || [] : [],
                    quyTrinhs: quyTrinh.status === "fulfilled" ? itemsOf(quyTrinh.value) : [],
                    trangThais: trangThai.status === "fulfilled" ? itemsOf(trangThai.value) : []
                });
            } finally {
                setLoadingOptions(false);
            }
        };
        void loadOptions();
    }, []);
    const updateFilter = (field, value) => setFilters((current) => ({...current, [field]: value}));
    const donutOptions = {
        chart: {type: "donut", fontFamily: "Inter Variable, sans-serif"},
        labels: construction.nhomSoLieu.map((item) => item.nhom),
        colors: ["#465fff", "#12b76a", "#f79009", "#f04438"],
        legend: {position: "bottom", fontSize: "12px"},
        dataLabels: {enabled: false},
        stroke: {colors: ["#ffffff"]},
        plotOptions: {
            pie: {
                donut: {
                    size: "68%",
                    labels: {
                        show: true,
                        total: {show: true, label: "Hồ sơ", formatter: () => String(construction.giaTri || 0)}
                    }
                }
            }
        }
    };
    const horizontalOptions = (rows, color, unit) => ({
        chart: {
            type: "bar",
            toolbar: {show: false},
            fontFamily: "Inter Variable, sans-serif"
        },
        colors: [color],
        plotOptions: {bar: {horizontal: true, borderRadius: 5, barHeight: "48%"}},
        dataLabels: {enabled: false},
        xaxis: {
            categories: rows.map((item) => item.nhom.length > 30 ? `${item.nhom.slice(0, 30)}…` : item.nhom),
            labels: {style: {colors: "#667085"}}
        },
        yaxis: {labels: {style: {colors: "#667085", fontSize: "11px"}}},
        grid: {borderColor: "#eaecf0"},
        tooltip: {y: {formatter: (value) => `${value} ${unit}`}}
    });

    return (
        <div className="space-y-6">
            <section
                className="rounded-2xl bg-gradient-to-r from-brand-950 via-brand-800 to-brand-600 px-5 py-6 text-white shadow-[var(--admin-shadow-panel)] md:px-7">
                <div className="flex flex-wrap items-start justify-between gap-4">
                    <div>
                        <p className="text-sm font-semibold tracking-[.15em] text-brand-200">
                            TRUNG TÂM ĐIỀU HÀNH
                        </p>
                        <h1 className="mt-2 text-2xl !text-white font-semibold md:text-3xl">
                            Dashboard quản lý xây dựng văn bản pháp luật
                        </h1>
                        <p className="mt-2 max-w-2xl text-sm leading-6 text-brand-100">
                            Tổng hợp số liệu xây dựng văn bản, tiến độ xử lý và tổ chức thi hành pháp luật.
                        </p>
                    </div>
                    <div className="rounded-xl border border-white/15 bg-white/10 px-4 py-3 text-sm backdrop-blur"><span
                        className="block text-brand-200">Phạm vi hiển thị</span><strong>{isSSA ? "Tất cả đơn vị" : "Đơn vị được phân quyền"}</strong>
                    </div>
                </div>
            </section>
            {error && <Alert variant="error" title="Có lỗi" message={error}/>}
            <section
                className="rounded-2xl border border-[var(--admin-border)] bg-[var(--admin-surface)] p-5 shadow-[var(--admin-shadow-soft)]">
                <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
                    <div>
                        <h2 className="font-semibold text-gray-900 dark:text-white">
                            Phạm vi báo cáo
                        </h2>
                        <p
                            className="mt-1 text-sm text-gray-500">Thay đổi bộ lọc để cập nhật chỉ số và biểu đồ.
                        </p>
                    </div>
                    <button type="button" onClick={() => setFilters(initialFilters)}
                            className="text-sm font-semibold text-brand-600 hover:text-brand-700">Đặt lại bộ lọc
                    </button>
                </div>
                <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-4">
                    <label
                        className="text-sm font-medium text-gray-700 dark:text-gray-300">
                        Năm
                        <input type="number"
                               value={filters.nam}
                               onChange={(event) => updateFilter("nam", event.target.value)}
                               className="mt-1.5 h-11 w-full rounded-xl border border-gray-200 bg-white px-3 text-sm outline-none focus:border-brand-400 dark:border-white/10 dark:bg-white/[0.03]"/>
                    </label>
                    {isSSA &&
                        <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Đơn vị<select
                            value={filters.donViId} onChange={(event) => updateFilter("donViId", event.target.value)}
                            disabled={loadingOptions}
                            className="mt-1.5 h-11 w-full rounded-xl border border-gray-200 bg-white px-3 text-sm outline-none focus:border-brand-400 dark:border-white/10 dark:bg-white/[0.03]">
                            <option value="">Tất cả đơn vị</option>
                            {options.donVis.map((item) => <option key={item.id}
                                                                  value={item.id}>{item.tenDonVi || item.ten || item.maDonVi}</option>)}
                        </select></label>}<label className="text-sm font-medium text-gray-700 dark:text-gray-300">Quy
                    trình<select value={filters.quyTrinhSoanThaoId}
                                 onChange={(event) => updateFilter("quyTrinhSoanThaoId", event.target.value)}
                                 disabled={loadingOptions}
                                 className="mt-1.5 h-11 w-full rounded-xl border border-gray-200 bg-white px-3 text-sm outline-none focus:border-brand-400 dark:border-white/10 dark:bg-white/[0.03]">
                        <option value="">Tất cả quy trình</option>
                        {options.quyTrinhs.map((item) => <option key={item.id}
                                                                 value={item.id}>{item.tenQuyTrinh || item.ten || item.maQuyTrinh}</option>)}
                    </select></label><label className="text-sm font-medium text-gray-700 dark:text-gray-300">Trạng
                    thái<select value={filters.trangThaiId}
                                onChange={(event) => updateFilter("trangThaiId", event.target.value)}
                                disabled={loadingOptions}
                                className="mt-1.5 h-11 w-full rounded-xl border border-gray-200 bg-white px-3 text-sm outline-none focus:border-brand-400 dark:border-white/10 dark:bg-white/[0.03]">
                        <option value="">Tất cả trạng thái</option>
                        {options.trangThais.map((item) => <option key={item.id}
                                                                  value={item.id}>{item.tenTrangThai || item.maTrangThai}</option>)}
                    </select></label></div>
            </section>
            {loading ? <div
                className="rounded-2xl border border-[var(--admin-border)] bg-[var(--admin-surface)] p-14 text-center text-sm text-gray-500">Đang
                tổng hợp số liệu điều hành...</div> : <>
                <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-5"><MetricCard label="Tổng hồ sơ"
                                                                                          value={construction.giaTri || 0}
                                                                                          hint="Hồ sơ xây dựng văn bản"
                                                                                          tone="brand"
                                                                                          icon="documents"/><MetricCard
                    label="Đang xử lý" value={countOf(construction.nhomSoLieu, "Đang xử lý")}
                    hint="Cần tiếp tục theo dõi"
                    tone="blue" icon="clock"/><MetricCard label="Đã ban hành"
                                                          value={countOf(construction.nhomSoLieu, "Đã ban hành")}
                                                          hint="Hoàn tất quy trình" tone="success"
                                                          icon="check"/><MetricCard
                    label="Quá hạn" value={countOf(construction.nhomSoLieu, "Quá hạn")} hint="Cần ưu tiên xử lý"
                    tone="error" icon="warning"/><MetricCard label="Cảnh báo mới"
                                                             value={countOf(alerts.nhomSoLieu, "Mới")}
                                                             hint={`${alerts.giaTri || 0} cảnh báo trong hệ thống`}
                                                             tone="warning" icon="bell"/></section>
                <section className="grid gap-5 xl:grid-cols-2"><ChartCard title="Tiến độ hồ sơ xây dựng văn bản"
                                                                          subtitle="Tỷ trọng hồ sơ theo tình trạng xử lý">{construction.nhomSoLieu.length ?
                    <Chart options={donutOptions} series={construction.nhomSoLieu.map((item) => item.soLuong)}
                           type="donut"
                           height={290}/> : <EmptyChart/>}</ChartCard><ChartCard title="Tổ chức thi hành pháp luật"
                                                                                 subtitle="Khối lượng nhiệm vụ theo tiến độ">{tasks.nhomSoLieu.length ?
                    <Chart options={horizontalOptions(tasks.nhomSoLieu, "#465fff", "nhiệm vụ")}
                           series={[{name: "Nhiệm vụ", data: tasks.nhomSoLieu.map((item) => item.soLuong)}]} type="bar"
                           height={290}/> : <EmptyChart/>}</ChartCard></section>
                <section className="grid gap-5 xl:grid-cols-[1.35fr_.65fr]"><ChartCard
                    title="Tiến độ theo bước nghiệp vụ"
                    subtitle="Số hồ sơ đang có tại từng bước trong quy trình"
                    action={<button type="button"
                                    onClick={() => navigate("/xay-dung-van-ban/danh-sach")}
                                    className="text-sm font-semibold text-brand-600 hover:text-brand-700">Xem
                        hồ
                        sơ</button>}>{stages.nhomSoLieu.length ?
                    <ProgressList rows={stages.nhomSoLieu}/> : <EmptyChart/>}</ChartCard><ChartCard
                    title="Cảnh báo điều hành" subtitle="Trạng thái xử lý cảnh báo">{alerts.nhomSoLieu.length ?
                    <ProgressList rows={alerts.nhomSoLieu}
                                  colors={["bg-warning-500", "bg-brand-500", "bg-success-500"]}/> :
                    <EmptyChart/>}</ChartCard></section>
                <ChartCard title={isSSA ? "Khối lượng hồ sơ theo đơn vị" : "Khối lượng hồ sơ thuộc đơn vị"}
                           subtitle={isSSA ? "10 đơn vị có số hồ sơ xây dựng văn bản nhiều nhất" : "Dữ liệu thuộc phạm vi đơn vị được phân quyền"}>{units.nhomSoLieu.length ?
                    <Chart options={horizontalOptions(units.nhomSoLieu, "#0ba5ec", "hồ sơ")}
                           series={[{name: "Hồ sơ", data: units.nhomSoLieu.map((item) => item.soLuong)}]} type="bar"
                           height={Math.max(250, units.nhomSoLieu.length * 45)}/> : <EmptyChart/>}</ChartCard>
            </>}</div>
    );
}
