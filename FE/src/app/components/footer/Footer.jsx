const Footer = () => {
    const currentYear = new Date().getFullYear();

    return (
        <footer className="border-t border-gray-200 bg-white py-4 dark:border-gray-800 dark:bg-gray-900">
            <div className="flex flex-col items-center justify-between gap-3 px-4 md:flex-row md:px-6">
                {/* Navigation */}
                <nav className="order-1 flex items-center gap-5 md:order-2">
                    <a
                        href="https://phanmemcuocsong.com/gioi-thieu/"
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-gray-600 transition hover:text-brand-500 dark:text-gray-400"
                    >
                        Về chúng tôi
                    </a>

                    <a
                        href="https://phanmemcuocsong.com/lien-he/"
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-gray-600 transition hover:text-brand-500 dark:text-gray-400"
                    >
                        Liên hệ
                    </a>
                </nav>

                {/* Copyright */}
                <div className="order-2 text-center text-sm md:order-1 md:text-left">
                    <span className="mr-2 text-gray-500">
                        Copyright © 2012-{currentYear}
                    </span>

                    <a
                        href="https://phanmemcuocsong.com/"
                        target="_blank"
                        rel="noopener noreferrer"
                        className="font-medium text-gray-800 transition hover:text-brand-500 dark:text-white/90"
                    >
                        LifeSoft
                    </a>

                    <span className="ml-2 text-gray-500">
                        Tiện ích hơn - Hiệu quả hơn
                    </span>
                </div>
            </div>
        </footer>
    );
};

export default Footer;