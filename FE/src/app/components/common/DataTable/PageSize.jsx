const PageSize = ({ pageSize, onPageSizeChange }) => {
    const pageSizeOptions = [5, 10, 20, 50, 100];

    return (
        <div className="flex items-center gap-3">
            <span className="text-sm text-gray-500">
                Hiển thị
            </span>

            <select
                value={pageSize}
                onChange={(e) => onPageSizeChange(Number(e.target.value))}
                className="h-10 rounded-lg border border-gray-300 bg-white px-3 text-sm text-gray-700 outline-none transition focus:border-brand-500"
            >
                {pageSizeOptions.map((item) => (
                    <option key={item} value={item}>
                        {item}
                    </option>
                ))}
            </select>
        </div>
    );
};

export default PageSize;