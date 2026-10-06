import PageMeta from "../../../app/components/common/PageMeta.jsx";
import Supports from "./Supports.jsx";
import icon1 from "../../../assets/icons/icon_1.svg";
import bg9 from "../../../assets/media/bg/bg-9.jpg";

export default function TrangChu() {
    const appName = "LIFESOFT FRAMEWORK";
    const copyright = "2012-2026 © LifeSoft";

    return (
        <>
            <PageMeta
                title={`Trang chủ | LifeSoft`}
                description="Trang chủ | LifeSoft"
            />
            <div
                className="relative bg-top bg-no-repeat"
                style={{
                    backgroundImage: `url(${bg9})`,
                }}
            >
                <div className="mx-auto max-w-(--breakpoint-2xl) px-4 md:px-6 h-60">
                    <div className="flex h-full items-center  justify-around gap-30 pb-10  pt-0">
                        {/* Thông tin hệ thống */}
                        <div>
                            <h3 className="mb-0 mr-10 text-xl text-gray-900">
                                {appName}
                            </h3>

                            <p className="mt-1 text-sm text-gray-600">
                                {copyright}
                            </p>
                        </div>

                        <div className="hidden flex-col items-start justify-center opacity-30 lg:flex lg:opacity-100">
                            <img
                                src={icon1}
                                alt="Dashboard illustration"
                                className="ml-30 h-auto w-70 items-end"
                            />
                        </div>
                    </div>
                </div>
            </div>

            {/* Nội dung dashboard */}
            <div className="relative z-10 -mt-10 px-4 md:px-6">
                <Supports />
            </div>
        </>
    );
}
