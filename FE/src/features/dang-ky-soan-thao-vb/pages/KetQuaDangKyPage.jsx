import Badge from "../../../app/components/ui/badge/Badge.jsx";
import BasicTableTwo from "../../../app/components/tables/BasicTables/BasicTableTwo.jsx";
import {useMemo, useState} from "react";

export default function KetQuaDangKyPage() {
    const [data, setData] = useState([]);

    const columns = useMemo(
        () => [
            {
                key: "stt",
                header: "#",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400",
                render: (item, rowIndex) => rowIndex + 1,
            },
            {
                key: "maHoSo",
                header: "Mã hồ sơ",
                render: (item) => (
                    <div className="font-medium text-gray-800 dark:text-white/90">
                        {item.maHoSo || "-"}
                    </div>
                ),
            },
            {
                key: "tenHoSo",
                header: "Tên hồ sơ",
                render: (item) => (
                    <div className="max-w-[280px] font-medium text-gray-800 dark:text-white/90">
                        {item.tenHoSo || "-"}
                    </div>
                ),
            },
            {
                key: "tenDuThaoVanBan",
                header: "Tên dự thảo văn bản",
                render: (item) => (
                    <div className="max-w-[320px] text-gray-700 dark:text-gray-300">
                        {item.tenDuThaoVanBan || "-"}
                    </div>
                ),
            },
            {
                key: "namXayDung",
                header: "Năm xây dựng",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center text-sm text-gray-500 dark:text-gray-400",
                render: (item) => (
                    <span>{item.namXayDung || "-"}</span>
                ),
            },
            {
                key: "ketQua",
                header: "Kết quả",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center",
                render: (item) => (
                    <Badge
                        size="sm"
                        color={
                            item.ketQua === "Được phê duyệt"
                                ? "success"
                                : item.ketQua === "Không được phê duyệt"
                                    ? "error"
                                    : "warning"
                        }
                    >
                        {item.ketQua || "-"}
                    </Badge>
                ),
            },
            {
                key: "ngayKetQua",
                header: "Ngày có kết quả",
                render: (item) => (
                    <span className="text-sm text-gray-600 dark:text-gray-300">
                        {item.ngayKetQua
                            ? new Date(item.ngayKetQua).toLocaleDateString("vi-VN")
                            : "-"}
                    </span>
                ),
            },
            {
                key: "actions",
                header: "Thao tác",
                headerClassName:
                    "px-5 py-3 text-center font-medium text-gray-500 text-theme-xs dark:text-gray-400",
                cellClassName:
                    "px-5 py-4 text-center",
                render: (item) => (
                    <div className="flex items-center justify-center gap-2">
                        <button
                            type="button"
                            className="rounded-lg px-3 py-2 text-sm font-medium text-brand-500 transition hover:bg-brand-50 dark:hover:bg-white/[0.05]"
                        >
                            Xem
                        </button>
                    </div>
                ),
            },
        ],
        []
    );

    return (
        <div>
            <div className="mb-5">
                <h3 className="text-xl font-semibold text-gray-800 dark:text-white/90">
                    Kết quả đăng ký
                </h3>

                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                    Theo dõi kết quả đăng ký xây dựng văn bản
                </p>
            </div>

            <BasicTableTwo
                data={data}
                columns={columns}
                searchPlaceholder="Tìm kiếm kết quả..."
                searchFields={[
                    (item) => item.maHoSo,
                    (item) => item.tenHoSo,
                    (item) => item.tenDuThaoVanBan,
                    (item) => item.namXayDung,
                    (item) => item.ketQua,
                ]}
            />
        </div>
    );
}