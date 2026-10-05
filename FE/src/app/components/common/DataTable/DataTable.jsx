const DataTable = ({
                       columns = [],
                       data = [],
                       loading = false,
                       emptyText = "Không có dữ liệu",
                   }) => {
    return (
        <div className="overflow-x-auto">
            <table className="w-full">
                <thead>
                <tr className="border-b border-gray-200">
                    {columns.map((column) => (
                        <th
                            key={column.key}
                            className={`px-4 py-3 text-left text-sm font-medium text-gray-500 ${
                                column.className || ""
                            }`}
                        >
                            {column.title}
                        </th>
                    ))}
                </tr>
                </thead>

                <tbody>
                {loading ? (
                    <tr>
                        <td
                            colSpan={columns.length}
                            className="px-4 py-10 text-center text-sm text-gray-500"
                        >
                            Đang tải dữ liệu...
                        </td>
                    </tr>
                ) : data.length > 0 ? (
                    data.map((row, index) => (
                        <tr
                            key={row.id || index}
                            className="border-b border-gray-100 transition hover:bg-gray-50"
                        >
                            {columns.map((column) => (
                                <td
                                    key={column.key}
                                    className={`px-4 py-4 text-sm text-gray-700 ${
                                        column.cellClassName || ""
                                    }`}
                                >
                                    {column.render
                                        ? column.render(row, index)
                                        : row[column.key]}
                                </td>
                            ))}
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td
                            colSpan={columns.length}
                            className="px-4 py-10 text-center text-sm text-gray-500"
                        >
                            {emptyText}
                        </td>
                    </tr>
                )}
                </tbody>
            </table>
        </div>
    );
};

export default DataTable;