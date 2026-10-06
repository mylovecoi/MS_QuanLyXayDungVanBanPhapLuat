const Supports = () => {
    const contactsChuyenVien = [
        {
            name: "Hoàng Ngọc Long",
            phone: "0985 365 683",
        },
        {
            name: "Ngô Thế Dương",
            phone: "0916 678 911",
        },
        {
            name: "Nguyễn Trần Huynh",
            phone: "0964 304 891",
        },
        {
            name: "Trịnh Minh Khải",
            phone: "0389 095 454",
        },
    ];

    return (
        <div className="w-full">
            <div className="relative overflow-hidden rounded-md bg-white shadow-theme-sm ">
                {/* Background diagonal */}
                <div
                    className="pointer-events-none absolute inset-0"
                    style={{
                        background: `
                            linear-gradient(
                                115deg,
                                #c6faf7 0%,
                                #c6faf7 50%,
                                transparent 30.1%,
                                transparent 100%
                            )
                        `,
                    }}
                />

                <div className="relative p-6 md:p-8">
                    <div className="flex items-center gap-5">
                        {/* Icon */}
                        <div className="shrink-0">
                            <div className="text-success-500">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                >
                                    <g
                                        stroke="none"
                                        strokeWidth="1"
                                        fill="none"
                                        fillRule="evenodd"
                                    >
                                        <rect x="0" y="0" width="24" height="24"/>

                                        <path
                                            d="M3,13.5 L19,12 L3,10.5 L3,3.7732928 C3,3.70255344 3.01501031,3.63261921 3.04403925,3.56811047 C3.15735832,3.3162903 3.45336217,3.20401298 3.70518234,3.31733205 L21.9867539,11.5440392 C22.098181,11.5941815 22.1873901,11.6833905 22.2375323,11.7948177 C22.3508514,12.0466378 22.2385741,12.3426417 21.9867539,12.4559608 L3.70518234,20.6826679 C3.64067359,20.7116969 3.57073936,20.7267072 3.5,20.7267072 C3.22385763,20.7267072 3,20.5028496 3,20.2267072 L3,13.5 Z"
                                            fill="currentColor"
                                        />
                                    </g>
                                </svg>
                            </div>
                        </div>

                        {/* Nội dung */}
                        <div className="flex min-w-0 flex-1 flex-col">
                            <h3 className="mb-4 text-xl mr-2 text-gray-900 ">
                                Lời cảm ơn!
                            </h3>
                            <div className="text-sm leading-6 text-gray-600">
                                <p className="mb-3">
                                    <span className="mr-2 inline-block h-2 w-2 rounded-full bg-error-500"/>
                                    Công ty TNHH phát triển phần mềm Cuộc sống
                                    (LifeSoft) chân thành cảm ơn quý khách hàng
                                    đã tin tưởng sử dụng phần mềm của công ty.
                                    Thay mặt toàn bộ cán bộ nhân viên trong công
                                    ty gửi đến khách hàng lời chúc sức khỏe -
                                    thành công.
                                </p>
                                <p className="mb-3">
                                    <span className="mr-2 inline-block h-2 w-2 rounded-full bg-error-500"/>
                                    Nhằm chăm sóc, hỗ trợ khách hàng nhanh chóng
                                    và tiện dụng nhất công ty xin cung cấp thông
                                    tin các cán bộ hỗ trợ khách hàng trong quá
                                    trình sử dụng. Mọi vấn đề khúc mắc khách
                                    hàng có thể liên hệ trực tiếp cho cán bộ để
                                    được hỗ trợ!
                                </p>
                                {/* Danh sách cán bộ */}
                                <div className="grid grid-cols-1 gap-x-8 gap-y-2 md:grid-cols-2">
                                    <p className="mb-0">
                                        <span className="mr-2 inline-block h-2 w-2 rounded-full bg-error-500"/>
                                        Phụ trách khối kỹ thuật:{"   "}
                                        <span className="text-brand-500">
                                            Giám đốc:
                                        </span>{" "}
                                        Trần Ngọc Hiếu{" "}
                                        <span className="text-brand-500">
                                            - Tel:
                                        </span>{" "}
                                        096 8206844
                                    </p>
                                    <p className="mb-0">
                                        <span className="mr-2 inline-block h-2 w-2 rounded-full bg-error-500"/>
                                        Phòng TKBT:{" "}
                                        <span className="text-brand-500">
                                            Trưởng phòng:
                                        </span>{" "}
                                        Nguyễn Xuân Trường{" "}
                                        <span className="text-brand-500">
                                            - Tel:
                                        </span>{" "}
                                        0917 737 456
                                    </p>


                                    {contactsChuyenVien.map((item, index) => (
                                        <p
                                            key={`${item.name}-${index}`}
                                            className="mb-0"
                                        >
                                            <span className="mr-2 inline-block h-2 w-2 rounded-full bg-error-500"/>

                                            Chuyên viên:{" "}
                                            <span className="text-brand-500">
                                                {item.name}
                                            </span>{" "}
                                            <span className="text-brand-500">
                                                - Tel:
                                            </span>{" "}
                                            {item.phone}
                                        </p>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Supports;

