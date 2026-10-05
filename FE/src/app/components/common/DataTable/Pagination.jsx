const Pagination = ({
                        pageCurrent,
                        totalPages,
                        onPageChange,
                    }) => {
    if (!totalPages || totalPages <= 1) {
        return null;
    }

    const pages = Array.from(
        { length: totalPages },
        (_, index) => index + 1
    );

    return (
        <div className="flex flex-wrap items-center justify-center gap-2">
            <button
                type="button"
                disabled={pageCurrent === 1}
                onClick={() => onPageChange(pageCurrent - 1)}
                className="flex h-9 min-w-9 items-center justify-center rounded-lg border border-gray-300 px-3 text-sm transition hover:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50"
            >
                Trước
            </button>

            {pages.map((page) => (
                <button
                    key={page}
                    type="button"
                    onClick={() => onPageChange(page)}
                    className={`flex h-9 min-w-9 items-center justify-center rounded-lg px-3 text-sm transition ${
                        page === pageCurrent
                            ? "bg-brand-500 text-white"
                            : "border border-gray-300 hover:bg-gray-100"
                    }`}
                >
                    {page}
                </button>
            ))}

            <button
                type="button"
                disabled={pageCurrent === totalPages}
                onClick={() => onPageChange(pageCurrent + 1)}
                className="flex h-9 min-w-9 items-center justify-center rounded-lg border border-gray-300 px-3 text-sm transition hover:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50"
            >
                Sau
            </button>
        </div>
    );
};

export default Pagination;