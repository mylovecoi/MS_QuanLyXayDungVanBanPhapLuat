import {useMemo, useState, useEffect} from "react";
import {getDonViOptions} from "../../danh-muc/api/donViApi";
import Label from "../../../app/components/forms/Label.jsx";
import Select from "../../../app/components/forms/Select.jsx";
import Alert from "../../../app/components/ui/alert/Alert.jsx";
import BasicTableTwo from "../../../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {searchTraCuu} from "../api/traCuuApi";
import DatePicker from "../../../app/components/forms/date-picker.jsx";
import Badge from "../../../app/components/ui/badge/Badge.jsx";

const getTrangThaiBadge = (tinhTrang = "") => {
    const status = tinhTrang.toLowerCase().trim();

    if (
        status.includes("hoàn thành") ||
        status.includes("đã hoàn tất")
    ) {
        return {color: "green", label: tinhTrang};
    }

    if (
        status.includes("trả lại") ||
        status.includes("từ chối") ||
        status.includes("quá hạn")
    ) {
        return {color: "red", label: tinhTrang};
    }

    if (
        status.includes("phê duyệt") ||
        status.includes("đã trình")
    ) {
        return {color: "blue", label: tinhTrang};
    }

    if (
        status.includes("chờ") ||
        status.includes("đang")
    ) {
        return {color: "orange", label: tinhTrang};
    }

    return {color: "gray", label: tinhTrang || "-"};
};

const badgeColor = (color) => ({
    blue: "primary",
    green: "success",
    red: "error",
    orange: "warning",
    cyan: "info",
    purple: "primary",
    gray: "light",
}[color] || "primary");

const configs = {
    "tong-hop": {
        title: "Tra cứu tổng hợp",
        description: "Tra cứu xuyên suốt các nguồn dữ liệu nghiệp vụ.",
    },
    "dang-ky-xay-dung-van-ban": {
        title: "Tra cứu đăng ký xây dựng văn bản",
        description: "Tra cứu hồ sơ đăng ký xây dựng văn bản pháp luật.",
    },
    "xay-dung-van-ban": {
        title: "Tra cứu xây dựng văn bản",
        description: "Tra cứu tiến độ và hồ sơ xây dựng văn bản.",
    },
    "thi-hanh-phap-luat": {
        title: "Tra cứu thi hành pháp luật",
        description: "Tra cứu hoạt động thi hành pháp luật.",
    },
};

const createEmptyFilters = () => ({
    search: "",
    donViId: "",
    nguoiPhuTrachId: "",
    loaiVanBanId: "",
    trangThaiId: "",
    tuNgay: "",
    denNgay: "",
    nam: "",
});

const toError = (error) =>
    error?.response?.data?.message ||
    error?.response?.data?.title ||
    (typeof error?.response?.data === "string"
        ? error.response.data
        : null) ||
    error?.message ||
    "Không thể tải dữ liệu tra cứu.";

export default function TraCuuPage({source}) {
    const config = configs[source] || configs["tong-hop"];

    // Bộ lọc tra cứu phía trên
    const [filters, setFilters] = useState(createEmptyFilters);

    // Dữ liệu kết quả
    const [items, setItems] = useState([]);
    const [total, setTotal] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [hasSearched, setHasSearched] = useState(false);
    // Phân trang phía server
    const [pageCurrent, setPageCurrent] = useState(1);
    const [pageSize, setPageSize] = useState(20);

    const [donViOptions, setDonViOptions] = useState([]);
    const [loadingDanhMuc, setLoadingDanhMuc] = useState(false);

    useEffect(() => {
        const loadDanhMuc = async () => {
            try {
                setLoadingDanhMuc(true);
                const result = await getDonViOptions();
                setDonViOptions(
                    Array.isArray(result) ? result : []
                );
            } catch (error) {
                console.error("Không thể tải danh sách đơn vị:", error);
            } finally {
                setLoadingDanhMuc(false);
            }
        };

        void loadDanhMuc();
    }, []);

    const columns = useMemo(
        () => [
            {
                key: "stt",
                header: "#",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400",
                render: (item, rowIndex, currentPage, pageSize) =>
                    (currentPage - 1) * pageSize + rowIndex + 1,
            },
            {
                key: "tenDoiTuong",
                header: "Đối tượng",
                render: (item) => (
                    <div className="max-w-[320px]">
                        <div className="font-medium text-gray-800 dark:text-white/90">
                            {item.tenDoiTuong || "-"}
                        </div>
                        <div className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                            {item.maDoiTuong || "-"}
                        </div>
                    </div>
                ),
            },
            {
                key: "nguonDuLieu",
                header: "Nguồn dữ liệu",
                render: (item) => (
                    <span className="text-gray-700 dark:text-gray-300">
                        {item.nguonDuLieu || "-"}
                    </span>
                ),
            },
            {
                key: "tinhTrang",
                header: "Tình trạng",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName: "px-5 py-4 text-center",
                render: (item) => {
                    const badge = getTrangThaiBadge(item.tinhTrang);

                    return (
                        <Badge size="sm" color={badgeColor(badge.color)}>
                            {badge.label}
                        </Badge>
                    );
                },
            },
            {
                key: "ngayBatDau",
                header: "Ngày bắt đầu",
                render: (item) => (
                    <span className="text-sm text-gray-600 dark:text-gray-300">
                        {item.ngayBatDau
                            ? new Date(item.ngayBatDau).toLocaleDateString("vi-VN")
                            : "-"}
                    </span>
                ),
            },
            {
                key: "hanHoanThanh",
                header: "Hạn hoàn thành",
                render: (item) => (
                    <span className="text-sm text-gray-600 dark:text-gray-300">
                        {item.hanHoanThanh
                            ? new Date(item.hanHoanThanh).toLocaleDateString("vi-VN")
                            : "-"}
                    </span>
                ),
            },
        ],
        []
    );

    const searchFields = useMemo(
        () => [
            (item) => item.tenDoiTuong,
            (item) => item.maDoiTuong,
            (item) => item.nguonDuLieu,
            (item) => item.tinhTrang,
        ],
        []
    );

    const handleFilterChange = (key, value) => {
        setFilters((current) => ({
            ...current,
            [key]: value,
        }));
    };

    useEffect(() => {
        setFilters(createEmptyFilters());
        setItems([]);
        setTotal(0);
        setPageCurrent(1);
        setPageSize(20);
        setError("");
        setHasSearched(false);
        setLoading(false);
    }, [source]);

    // Chỉ gọi API khi nhấn Tìm kiếm hoặc Xóa bộ lọc
    const loadData = async (
        filterValues = filters,
        currentPage = pageCurrent,
        currentPageSize = pageSize
    ) => {
        try {
            setLoading(true);
            setError("");
            setHasSearched(true);

            const params = {
                Search: filterValues.search.trim() || undefined,
                DonViId: filterValues.donViId || undefined,
                NguoiPhuTrachId:
                    filterValues.nguoiPhuTrachId || undefined,
                LoaiVanBanId:
                    filterValues.loaiVanBanId || undefined,
                TrangThaiId: filterValues.trangThaiId || undefined,
                TuNgay: filterValues.tuNgay || undefined,
                DenNgay: filterValues.denNgay || undefined,
                Nam: filterValues.nam
                    ? Number(filterValues.nam)
                    : undefined,
                PageSize: currentPageSize,
                PageCurrent: currentPage,
            };

            const result = await searchTraCuu(source, params);

            // Đọc dữ liệu phân trang từ API
            const resultItems = Array.isArray(result?.items)
                ? result.items
                : [];

            const resultTotal = Number(result?.totalCount ?? 0);
            const resultPageSize = Number(
                result?.pageSize ?? currentPageSize
            );
            const resultPageCurrent = Number(
                result?.pageCurrent ?? currentPage
            );

            setItems(resultItems);
            setTotal(resultTotal);
            setPageSize(resultPageSize);
            setPageCurrent(resultPageCurrent);
        } catch (requestError) {
            setItems([]);
            setTotal(0);
            setError(toError(requestError));
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        setPageCurrent(1);
        void loadData(filters, 1, pageSize);
    };

    const handleClear = () => {
        const emptyFilters = createEmptyFilters();
        setFilters(emptyFilters);
        setPageCurrent(1);
        void loadData(emptyFilters, 1, pageSize);
    };

    return (
        <div>
            {/* Tiêu đề trang */}
            <div className="mb-5">
                <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
                    {config.title}
                </h1>

                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                    {config.description}
                </p>
            </div>

            {/* Phần 1: Bộ lọc tra cứu */}
            <div
                className="mb-6 rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
                <form onSubmit={handleSubmit}>
                    <div className="grid grid-cols-1 gap-4 p-5 sm:grid-cols-2 xl:grid-cols-5">
                        {/* Đơn vị */}
                        <div>
                            <Label>Đơn vị</Label>
                            <Select
                                value={filters.donViId}
                                onChange={(value) =>
                                    handleFilterChange("donViId", value)
                                }
                                options={[
                                    {value: "", label: "Tất cả đơn vị"},
                                    ...donViOptions.map((item) => ({
                                        value: item.id,
                                        label: item.tenDonVi,
                                    })),
                                ]}
                            />
                        </div>

                        {/* Người phụ trách */}
                        <div>
                            <Label>Người phụ trách</Label>
                            <Select
                                value={filters.nguoiPhuTrachId}
                                onChange={(value) =>
                                    handleFilterChange("nguoiPhuTrachId", value)
                                }
                                options={[
                                    {value: "", label: "Tất cả người phụ trách"},
                                ]}
                            />
                        </div>

                        {/* Loại văn bản */}
                        <div>
                            <Label>Loại văn bản</Label>
                            <Select
                                value={filters.loaiVanBanId}
                                onChange={(value) =>
                                    handleFilterChange("loaiVanBanId", value)
                                }
                                options={[
                                    {value: "", label: "Tất cả loại văn bản"},
                                ]}
                            />
                        </div>

                        {/* Trạng thái */}
                        <div>
                            <Label>Trạng thái</Label>
                            <Select
                                value={filters.trangThaiId}
                                onChange={(value) =>
                                    handleFilterChange("trangThaiId", value)
                                }
                                options={[
                                    {value: "", label: "Tất cả trạng thái"},
                                ]}
                            />
                        </div>

                        <div>
                            <DatePicker
                                id="tuNgay"
                                key={`${source}-tuNgay`}
                                label="Từ ngày"
                                placeholder="Chọn ngày bắt đầu"
                                defaultDate={filters.tuNgay || undefined}
                                onChange={(selectedDates, dateStr) => {
                                    setFilters((current) =>
                                        ({
                                            ...current, tuNgay: dateStr,
                                        }));
                                }}/>
                        </div>
                        <div>
                            <DatePicker
                                id="denNgay"
                                key={`${source}-denNgay`}
                                label="Đến ngày"
                                placeholder="Chọn ngày kết thúc"
                                defaultDate={filters.denNgay || undefined}
                                onChange={(selectedDates, dateStr) => {
                                    setFilters((current) =>
                                        ({
                                            ...current, denNgay: dateStr,
                                        }));
                                }}/>
                        </div>

                        {/* Năm */}
                        <div>
                            <Label>Năm</Label>
                            <Select
                                value={filters.nam}
                                onChange={(value) =>
                                    handleFilterChange("nam", value)
                                }
                                options={[
                                    {value: "", label: "Tất cả các năm"},
                                    {value: "2024", label: "Năm 2024"},
                                    {value: "2025", label: "Năm 2025"},
                                    {value: "2026", label: "Năm 2026"},
                                    {value: "2027", label: "Năm 2027"},
                                ]}
                            />
                        </div>
                    </div>

                    {/* Nút Xóa và Tìm kiếm */}
                    <div
                        className="flex flex-wrap justify-center gap-3 border-t border-gray-100 px-5 py-4 dark:border-white/[0.05]">
                        <button
                            type="button"
                            onClick={handleClear}
                            disabled={loading}
                            className="inline-flex items-center gap-2 rounded-lg border border-gray-300 px-4 py-2.5 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:opacity-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                        >
                            <svg
                                width="16"
                                height="16"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="2"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                            >
                                <path d="M3 6h18"/>
                                <path d="M8 6V4h8v2"/>
                                <path d="M19 6v14H5V6"/>
                            </svg>
                            Xóa bộ lọc
                        </button>

                        <button
                            type="submit"
                            disabled={loading}
                            className="inline-flex items-center gap-2 rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-brand-600 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                            {loading ? (
                                <>
                                    <svg
                                        className="animate-spin"
                                        width="16"
                                        height="16"
                                        viewBox="0 0 24 24"
                                        fill="none"
                                    >
                                        <circle
                                            cx="12"
                                            cy="12"
                                            r="10"
                                            stroke="currentColor"
                                            strokeWidth="4"
                                            opacity="0.25"
                                        />
                                        <path
                                            d="M22 12a10 10 0 0 0-10-10"
                                            stroke="currentColor"
                                            strokeWidth="4"
                                            strokeLinecap="round"
                                        />
                                    </svg>
                                    Đang tìm...
                                </>
                            ) : (
                                <>
                                    <svg
                                        width="16"
                                        height="16"
                                        viewBox="0 0 24 24"
                                        fill="none"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    >
                                        <circle cx="11" cy="11" r="8"/>
                                        <path d="m21 21-4.35-4.35"/>
                                    </svg>
                                    Tìm kiếm
                                </>
                            )}
                        </button>
                    </div>
                </form>
            </div>

            {/* Lỗi API */}
            {error && (
                <div className="mb-5">
                    <Alert
                        variant="error"
                        title="Không thể tra cứu"
                        message={error}
                    />
                </div>
            )}

            {/* Phần 2: Bảng kết quả */}
            <div>
                <div className="mb-4 flex flex-col gap-1 sm:flex-row sm:items-end sm:justify-between">
                    <div>
                        <h2 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                            Kết quả tra cứu
                        </h2>

                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            {hasSearched
                                ? `Tổng số ${total} kết quả phù hợp.`
                                : "Nhấn Tìm kiếm để tải dữ liệu."}
                        </p>
                    </div>
                </div>

                {loading ? (
                    <div
                        className="rounded-xl border border-gray-200 bg-white px-5 py-12 text-center text-sm text-gray-500 dark:border-white/[0.05] dark:bg-white/[0.03] dark:text-gray-400">
                        Đang tải dữ liệu tra cứu...
                    </div>
                ) : (
                    <BasicTableTwo
                        data={items}
                        columns={columns}
                        searchPlaceholder="Tìm kiếm trong kết quả..."
                        searchFields={searchFields}
                        pageSizeOptions={[5, 10, 20, 100]}
                        emptyText="Không tìm thấy dữ liệu phù hợp."
                        serverPagination
                        totalCount={total}
                        pageCurrent={pageCurrent}
                        pageSize={pageSize}
                        onPageChange={(nextPage) => {
                            void loadData(filters, nextPage, pageSize);
                        }}
                        onPageSizeChange={(nextPageSize) => {
                            setPageCurrent(1);
                            setPageSize(nextPageSize);

                            void loadData(filters, 1, nextPageSize);
                        }}
                    />
                )}
            </div>
        </div>
    );
}

