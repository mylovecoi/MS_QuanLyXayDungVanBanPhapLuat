import React, {useEffect, useState} from "react";
import GridShape from "../../app/components/common/GridShape";
import {Link} from "react-router";
import ThemeTogglerTwo from "../../app/components/common/ThemeTogglerTwo";
import {getSystemInfo} from "../../shared/api/systemApi.js";

export default function AuthLayout({children}) {
    const [appName, setAppName] = useState("");

    useEffect(() => {
        const fetchSystemInfo = async () => {
            try {
                const response = await getSystemInfo();

                // Điều chỉnh theo cấu trúc response thực tế của request wrapper.
                const data = response?.data?.data ?? response?.data;

                if (data?.appName) {
                    setAppName(data.appName);
                }
            } catch (error) {
                console.error("Không thể lấy cấu hình hệ thống:", error);
            }
        };

        void fetchSystemInfo();
    }, []);

    return (
        <div className="relative z-1 bg-white p-6 dark:bg-gray-900 sm:p-0">
            <div className="relative flex h-screen w-full flex-col justify-center dark:bg-gray-900 sm:p-0 lg:flex-row">
                {children}

                <div className="hidden h-full w-full items-center bg-brand-950 lg:grid lg:w-1/2 dark:bg-white/5">
                    <div className="relative z-1 flex items-center justify-center">
                        <GridShape/>

                        <div className="flex max-w-xl flex-col items-center">
                            <Link to="/" className="mb-4 block">
                                <img
                                    width={231}
                                    height={48}
                                    src="/images/logo/logo-Life.png"
                                    alt="Logo"
                                />
                            </Link>

                            <h1 className="max-w-xl text-center text-xl font-bold leading-relaxed tracking-wide text-white drop-shadow-md sm:text-2xl dark:text-white">
                                {appName || "PHẦN MỀM QUẢN LÝ XÂY DỰNG VĂN BẢN PHÁP LUẬT"}
                            </h1>
                        </div>
                    </div>
                </div>

                <div className="fixed bottom-6 right-6 z-50 hidden sm:block">
                    <ThemeTogglerTwo/>
                </div>
            </div>
        </div>
    );
}