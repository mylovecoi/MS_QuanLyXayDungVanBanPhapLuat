import {useMemo, useState} from "react";

import {
    Table,
    TableBody,
    TableCell,
    TableHeader,
    TableRow,
} from "../../ui/table";

import Select from "../../forms/Select.jsx";
import Input from "../../forms/input/InputField.jsx";

export default function BasicTableTwo({
                                          data = [],
                                          columns = [],
                                          searchPlaceholder = "Search...",
                                          searchFields = [],
                                          pageSizeOptions = [5, 10, 20, 100],
                                          emptyText = "No data found.",
                                      }) {
    const [search, setSearch] = useState("");
    const [pageCurrent, setPageCurrent] = useState(1);
    const [pageSize, setPageSize] = useState(pageSizeOptions[0] ?? 5);

    /*
     * Search
     *
     * searchFields:
     * [
     *   (item) => item.name,
     *   (item) => item.code
     * ]
     */
    const filteredData = useMemo(() => {
        const keyword = search.trim().toLowerCase();

        if (!keyword) {
            return data;
        }

        if (!searchFields.length) {
            return data;
        }

        return data.filter((item) =>
            searchFields.some((getValue) => {
                const value = getValue(item);

                return (
                    value !== null &&
                    value !== undefined &&
                    String(value)
                        .toLowerCase()
                        .includes(keyword)
                );
            })
        );
    }, [data, search, searchFields]);

    /*
     * Pagination
     */
    const totalRecord = filteredData.length;

    const totalPages = Math.max(
        1,
        Math.ceil(totalRecord / pageSize)
    );

    const currentPage = Math.min(
        pageCurrent,
        totalPages
    );

    const paginatedData = useMemo(() => {
        const startIndex =
            (currentPage - 1) * pageSize;

        return filteredData.slice(
            startIndex,
            startIndex + pageSize
        );
    }, [
        filteredData,
        currentPage,
        pageSize,
    ]);

    /*
     * Events
     */
    const handleSearch = (value) => {
        setSearch(value);
        setPageCurrent(1);
    };

    const handlePageSizeChange = (value) => {
        setPageSize(Number(value));
        setPageCurrent(1);
    };

    const handlePrevious = () => {
        if (currentPage > 1) {
            setPageCurrent((prev) => prev - 1);
        }
    };

    const handleNext = () => {
        if (currentPage < totalPages) {
            setPageCurrent((prev) => prev + 1);
        }
    };

    return (
        <div
            className="
                overflow-hidden
                rounded-xl
                border
                border-gray-200
                bg-white
                dark:border-white/[0.05]
                dark:bg-white/[0.03]
            "
        >
            {/* Filter */}
            <div
                className="
                    grid
                    grid-cols-1
                    gap-4
                    border-b
                    border-gray-100
                    px-5
                    py-4
                    sm:grid-cols-12
                    dark:border-white/[0.05]
                "
            >
                {/* Page size */}
                <div className="w-full sm:col-span-3">
                    <label
                        className="
                            mb-2
                            block
                            text-xs
                            font-medium
                            text-gray-500
                            dark:text-gray-400
                        "
                    >
                        Hiển Thị
                    </label>

                    <Select
                        value={pageSize}
                        onChange={handlePageSizeChange}
                        options={pageSizeOptions.map(
                            (value) => ({
                                value,
                                label: `${value} thông tin`,
                            })
                        )}
                    />
                </div>

                {/* Search */}
                <div className="w-full sm:col-span-9">
                    <label
                        className="
                            mb-2
                            block
                            text-xs
                            font-medium
                            text-gray-500
                            dark:text-gray-400
                        "
                    >
                        Tìm Kiếm
                    </label>

                    <Input
                        value={search}
                        onChange={(event) =>
                            handleSearch(
                                event.target.value
                            )
                        }
                        placeholder={searchPlaceholder}
                        prefix={
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                width="18"
                                height="18"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="2"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                            >
                                <circle
                                    cx="11"
                                    cy="11"
                                    r="8"
                                />
                                <path d="m21 21-4.3-4.3" />
                            </svg>
                        }
                        suffix={
                            search && (
                                <button
                                    type="button"
                                    onClick={() =>
                                        handleSearch("")
                                    }
                                    className="
                                        flex
                                        h-6
                                        w-6
                                        items-center
                                        justify-center
                                        rounded-full
                                        text-gray-400
                                        transition
                                        hover:bg-gray-100
                                        hover:text-gray-600
                                        dark:hover:bg-gray-800
                                        dark:hover:text-gray-200
                                    "
                                    aria-label="Clear search"
                                >
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        width="16"
                                        height="16"
                                        viewBox="0 0 24 24"
                                        fill="none"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    >
                                        <path d="M18 6 6 18" />
                                        <path d="m6 6 12 12" />
                                    </svg>
                                </button>
                            )
                        }
                    />
                </div>
            </div>

            {/* Table */}
            <div className="max-w-full overflow-x-auto">
                <Table>
                    <TableHeader
                        className="
                            border-b
                            border-gray-100
                            dark:border-white/[0.05]
                        "
                    >
                        <TableRow>
                            {columns.map(
                                (column) => (
                                    <TableCell
                                        key={column.key}
                                        isHeader
                                        className={
                                            column.headerClassName ||
                                            `
                                                px-5
                                                py-3
                                                font-medium
                                                text-gray-500
                                                text-start
                                                text-theme-xs
                                                dark:text-gray-400
                                            `
                                        }
                                    >
                                        {column.header}
                                    </TableCell>
                                )
                            )}
                        </TableRow>
                    </TableHeader>

                    <TableBody
                        className="
                            divide-y
                            divide-gray-100
                            dark:divide-white/[0.05]
                        "
                    >
                        {paginatedData.length > 0 ? (
                            paginatedData.map(
                                (item, rowIndex) => (
                                    <TableRow
                                        key={
                                            item.id ??
                                            rowIndex
                                        }
                                    >
                                        {columns.map(
                                            (
                                                column
                                            ) => (
                                                <TableCell
                                                    key={
                                                        column.key
                                                    }
                                                    className={
                                                        column.cellClassName ||
                                                        `
                                                            px-4
                                                            py-3
                                                            text-gray-500
                                                            text-start
                                                            text-theme-sm
                                                            dark:text-gray-400
                                                        `
                                                    }
                                                >
                                                    {column.render
                                                        ? column.render(
                                                            item,
                                                            rowIndex,
                                                            currentPage,
                                                            pageSize
                                                        )
                                                        : item[
                                                            column.key
                                                            ]}
                                                </TableCell>
                                            )
                                        )}
                                    </TableRow>
                                )
                            )
                        ) : (
                            <TableRow>
                                <TableCell
                                    colSpan={
                                        columns.length
                                    }
                                    className="
                                        px-5
                                        py-10
                                        text-center
                                        text-sm
                                        text-gray-500
                                        dark:text-gray-400
                                    "
                                >
                                    {emptyText}
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </div>

            {/* Pagination */}
            <div
                className="
                    flex
                    flex-col
                    gap-4
                    border-t
                    border-gray-100
                    px-5
                    py-4
                    sm:flex-row
                    sm:items-center
                    sm:justify-between
                    dark:border-white/[0.05]
                "
            >
                <div className="text-sm text-gray-500 dark:text-gray-400">
                    Showing{" "}
                    {totalRecord === 0
                        ? 0
                        : (currentPage - 1) *
                        pageSize +
                        1}{" "}
                    to{" "}
                    {Math.min(
                        currentPage * pageSize,
                        totalRecord
                    )}{" "}
                    of {totalRecord} entries
                </div>

                <div className="flex items-center gap-2">
                    <button
                        type="button"
                        onClick={handlePrevious}
                        disabled={currentPage === 1}
                        className="
                            rounded-lg
                            border
                            border-gray-300
                            px-3
                            py-2
                            text-sm
                            font-medium
                            text-gray-700
                            transition
                            hover:bg-gray-50
                            disabled:cursor-not-allowed
                            disabled:opacity-50
                            dark:border-gray-700
                            dark:text-gray-300
                            dark:hover:bg-white/[0.05]
                        "
                    >
                        Previous
                    </button>

                    <div className="flex items-center gap-1">
                        {Array.from(
                            {
                                length: totalPages,
                            },
                            (_, index) =>
                                index + 1
                        ).map((page) => (
                            <button
                                key={page}
                                type="button"
                                onClick={() =>
                                    setPageCurrent(
                                        page
                                    )
                                }
                                className={`
                                    h-9
                                    min-w-9
                                    rounded-lg
                                    px-3
                                    text-sm
                                    font-medium
                                    ${
                                    currentPage ===
                                    page
                                        ? "bg-brand-500 text-white"
                                        : "text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/[0.05]"
                                }
                                `}
                            >
                                {page}
                            </button>
                        ))}
                    </div>

                    <button
                        type="button"
                        onClick={handleNext}
                        disabled={
                            currentPage ===
                            totalPages
                        }
                        className="
                            rounded-lg
                            border
                            border-gray-300
                            px-3
                            py-2
                            text-sm
                            font-medium
                            text-gray-700
                            transition
                            hover:bg-gray-50
                            disabled:cursor-not-allowed
                            disabled:opacity-50
                            dark:border-gray-700
                            dark:text-gray-300
                            dark:hover:bg-white/[0.05]
                        "
                    >
                        Next
                    </button>
                </div>
            </div>
        </div>
    );
}