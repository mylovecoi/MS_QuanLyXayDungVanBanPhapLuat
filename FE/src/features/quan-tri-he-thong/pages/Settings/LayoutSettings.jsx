import {useLayout} from "../../context/LayoutContext";

const LayoutSettings = () => {
    const {layoutMode, setLayoutMode} = useLayout();

    const handleChangeLayout = (mode) => {
        setLayoutMode(mode);
    };

    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-semibold text-gray-800 dark:text-white">
                    Cài đặt giao diện
                </h1>

                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                    Lựa chọn kiểu hiển thị menu cho hệ thống.
                </p>
            </div>

            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
                {/* Sidebar */}
                <button
                    type="button"
                    onClick={() => handleChangeLayout("sidebar")}
                    className={`rounded-xl border-2 bg-white p-5 text-left transition-all dark:bg-gray-900 ${
                        layoutMode === "sidebar"
                            ? "border-brand-500 shadow-md"
                            : "border-gray-200 hover:border-brand-300 dark:border-gray-800"
                    }`}
                >
                    <div className="mb-4 flex items-center justify-between">
                        <h2 className="text-lg font-semibold text-gray-800 dark:text-white">
                            Sidebar
                        </h2>

                        {layoutMode === "sidebar" && (
                            <span className="rounded-full bg-brand-50 px-3 py-1 text-xs font-medium text-brand-500 dark:bg-brand-500/10 dark:text-brand-400">
                                Đang sử dụng
                            </span>
                        )}
                    </div>

                    {/* Preview */}
                    <div className="flex h-48 overflow-hidden rounded-lg border border-gray-200 bg-gray-50 dark:border-gray-700 dark:bg-gray-800">
                        <div className="w-16 border-r border-gray-200 bg-white p-2 dark:border-gray-700 dark:bg-gray-900">
                            <div className="mb-4 h-5 rounded bg-gray-200 dark:bg-gray-700"/>
                            <div className="space-y-2">
                                <div className="h-3 rounded bg-brand-500"/>
                                <div className="h-3 rounded bg-gray-200 dark:bg-gray-700"/>
                                <div className="h-3 rounded bg-gray-200 dark:bg-gray-700"/>
                                <div className="h-3 rounded bg-gray-200 dark:bg-gray-700"/>
                            </div>
                        </div>

                        <div className="flex-1 p-3">
                            <div className="mb-3 h-8 rounded bg-white shadow-sm dark:bg-gray-900"/>
                            <div className="grid grid-cols-2 gap-3">
                                <div className="h-20 rounded bg-white dark:bg-gray-900"/>
                                <div className="h-20 rounded bg-white dark:bg-gray-900"/>
                            </div>
                        </div>
                    </div>

                    <p className="mt-4 text-sm text-gray-500 dark:text-gray-400">
                        Menu chính được hiển thị cố định ở bên trái màn hình.
                    </p>
                </button>

                {/* Topbar */}
                <button
                    type="button"
                    onClick={() => handleChangeLayout("topbar")}
                    className={`rounded-xl border-2 bg-white p-5 text-left transition-all dark:bg-gray-900 ${
                        layoutMode === "topbar"
                            ? "border-brand-500 shadow-md"
                            : "border-gray-200 hover:border-brand-300 dark:border-gray-800"
                    }`}
                >
                    <div className="mb-4 flex items-center justify-between">
                        <h2 className="text-lg font-semibold text-gray-800 dark:text-white">
                            Topbar
                        </h2>

                        {layoutMode === "topbar" && (
                            <span className="rounded-full bg-brand-50 px-3 py-1 text-xs font-medium text-brand-500 dark:bg-brand-500/10 dark:text-brand-400">
                                Đang sử dụng
                            </span>
                        )}
                    </div>

                    {/* Preview */}
                    <div className="h-48 overflow-hidden rounded-lg border border-gray-200 bg-gray-50 dark:border-gray-700 dark:bg-gray-800">
                        <div className="h-10 border-b border-gray-200 bg-white p-2 dark:border-gray-700 dark:bg-gray-900">
                            <div className="flex items-center gap-2">
                                <div className="h-4 w-14 rounded bg-gray-200 dark:bg-gray-700"/>
                                <div className="h-4 w-16 rounded bg-brand-500"/>
                                <div className="h-4 w-16 rounded bg-gray-200 dark:bg-gray-700"/>
                                <div className="h-4 w-16 rounded bg-gray-200 dark:bg-gray-700"/>
                            </div>
                        </div>

                        <div className="p-3">
                            <div className="mb-3 h-8 rounded bg-white shadow-sm dark:bg-gray-900"/>
                            <div className="grid grid-cols-2 gap-3">
                                <div className="h-20 rounded bg-white dark:bg-gray-900"/>
                                <div className="h-20 rounded bg-white dark:bg-gray-900"/>
                            </div>
                        </div>
                    </div>

                    <p className="mt-4 text-sm text-gray-500 dark:text-gray-400">
                        Menu chính được hiển thị theo chiều ngang phía trên.
                    </p>
                </button>
            </div>
        </div>
    );
};

export default LayoutSettings;