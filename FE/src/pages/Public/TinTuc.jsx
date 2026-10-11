import { useMemo, useState } from "react";
import { Link } from "react-router";
import PageMeta from "../../app/components/common/PageMeta.jsx";

const newsItems = [
    {
        category: "Chỉ đạo điều hành",
        date: "11/10/2026",
        title: "Tăng cường chất lượng xây dựng, ban hành và tổ chức thi hành văn bản pháp luật",
        excerpt: "Các đơn vị chủ động rà soát tiến độ, bảo đảm hồ sơ được xử lý thống nhất, đúng quy trình và công khai thông tin cần thiết.",
        accent: "from-brand-600 to-brand-400",
        icon: "01",
    },
    {
        category: "Hoạt động chuyên môn",
        date: "09/10/2026",
        title: "Hướng dẫn cập nhật kế hoạch xây dựng văn bản quy phạm pháp luật năm 2027",
        excerpt: "Kế hoạch được cập nhật theo lĩnh vực, đơn vị chủ trì và mốc thời gian thực hiện trên hệ thống.",
        accent: "from-blue-light-600 to-blue-light-400",
        icon: "02",
    },
    {
        category: "Thông báo",
        date: "07/10/2026",
        title: "Lịch tiếp nhận hồ sơ, tài liệu phục vụ phiên họp thẩm định tháng 10",
        excerpt: "Hồ sơ gửi đúng hạn sẽ được phân công xử lý và thông báo trạng thái trực tuyến đến đơn vị lập hồ sơ.",
        accent: "from-theme-purple-500 to-brand-400",
        icon: "03",
    },
    {
        category: "Chuyển đổi số",
        date: "04/10/2026",
        title: "Chuẩn hóa biểu mẫu điện tử trong quy trình soạn thảo và lấy ý kiến",
        excerpt: "Biểu mẫu mới giúp theo dõi ý kiến góp ý, tổng hợp nội dung và lưu vết xử lý minh bạch hơn.",
        accent: "from-success-600 to-success-400",
        icon: "04",
    },
];

const categories = ["Tất cả", "Chỉ đạo điều hành", "Hoạt động chuyên môn", "Thông báo", "Chuyển đổi số"];

function SearchIcon({ className = "" }) {
    return <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="m21 21-4.35-4.35m2.35-5.15a7.5 7.5 0 1 1-15 0 7.5 7.5 0 0 1 15 0Z" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round"/></svg>;
}

export default function TinTuc() {
    const [activeCategory, setActiveCategory] = useState("Tất cả");
    const [keyword, setKeyword] = useState("");
    const visibleNews = useMemo(() => newsItems.filter((item) =>
        (activeCategory === "Tất cả" || item.category === activeCategory)
        && `${item.title} ${item.excerpt}`.toLocaleLowerCase().includes(keyword.toLocaleLowerCase())
    ), [activeCategory, keyword]);

    return (
        <>
            <PageMeta title="Tin tức | Hệ thống quản lý xây dựng văn bản pháp luật" description="Tin tức và thông báo mới nhất" />
            <main className="min-h-screen bg-[#f7f9ff] text-gray-800">
                <div className="border-b border-brand-100 bg-brand-950 text-brand-100">
                    <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-2 text-xs sm:px-6 lg:px-8">
                        <span>HỆ THỐNG THÔNG TIN QUẢN LÝ XÂY DỰNG VĂN BẢN PHÁP LUẬT</span>
                        <span className="hidden sm:inline">Cập nhật thông tin chính thống, kịp thời</span>
                    </div>
                </div>

                <header className="bg-white shadow-sm">
                    <div className="mx-auto flex max-w-7xl items-center justify-between gap-4 px-4 py-4 sm:px-6 lg:px-8">
                        <Link to="/" className="flex min-w-0 items-center gap-3">
                            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-brand-500 text-lg font-bold text-white shadow-lg shadow-brand-500/25">PL</div>
                            <div className="min-w-0">
                                <p className="truncate text-base font-bold tracking-tight text-gray-900 sm:text-lg">QUẢN LÝ XÂY DỰNG VĂN BẢN PHÁP LUẬT</p>
                                <p className="hidden text-xs text-gray-500 sm:block">Cổng thông tin, điều hành và công bố văn bản</p>
                            </div>
                        </Link>
                        <Link to="/signin" className="shrink-0 rounded-xl border border-brand-200 px-3 py-2 text-sm font-semibold text-brand-700 transition hover:bg-brand-50 sm:px-4">Đăng nhập</Link>
                    </div>
                    <nav className="border-t border-gray-100">
                        <div className="mx-auto flex max-w-7xl gap-1 overflow-x-auto px-4 sm:px-6 lg:px-8">
                            {categories.map((category) => <button key={category} onClick={() => setActiveCategory(category)} className={`whitespace-nowrap border-b-2 px-3 py-3 text-sm font-semibold transition ${activeCategory === category ? "border-brand-500 text-brand-600" : "border-transparent text-gray-500 hover:border-brand-200 hover:text-brand-600"}`}>{category}</button>)}
                        </div>
                    </nav>
                </header>

                <section className="relative overflow-hidden bg-brand-950">
                    <div className="absolute inset-0 opacity-30" style={{backgroundImage: "radial-gradient(circle at 8% 10%, #7592ff 0, transparent 25%), radial-gradient(circle at 90% 80%, #0ba5ec 0, transparent 26%)"}} />
                    <div className="relative mx-auto grid max-w-7xl gap-8 px-4 py-12 sm:px-6 lg:grid-cols-[1.2fr_.8fr] lg:px-8 lg:py-16">
                        <div>
                            <p className="mb-3 text-sm font-bold tracking-[.18em] text-brand-300">TIN TỨC &amp; THÔNG BÁO</p>
                            <h1 className="max-w-3xl text-3xl font-bold leading-tight text-white sm:text-4xl">Thông tin điều hành và hoạt động xây dựng văn bản pháp luật</h1>
                            <p className="mt-5 max-w-2xl text-base leading-7 text-brand-100">Theo dõi các chỉ đạo mới, kế hoạch triển khai, hướng dẫn nghiệp vụ và kết quả xử lý hồ sơ trên toàn hệ thống.</p>
                        </div>
                        <div className="self-end rounded-2xl border border-white/15 bg-white/10 p-5 backdrop-blur-sm">
                            <label htmlFor="news-search" className="text-sm font-semibold text-white">Tìm kiếm tin tức</label>
                            <div className="mt-3 flex items-center gap-2 rounded-xl bg-white px-3 py-1.5 text-gray-500 shadow-lg">
                                <SearchIcon className="h-5 w-5 shrink-0" />
                                <input id="news-search" value={keyword} onChange={(event) => setKeyword(event.target.value)} placeholder="Nhập từ khóa cần tìm..." className="w-full border-0 bg-transparent py-2 text-sm text-gray-800 outline-none placeholder:text-gray-400" />
                            </div>
                        </div>
                    </div>
                </section>

                <section className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
                    <div className="mb-5 flex items-end justify-between gap-4">
                        <div><p className="text-sm font-semibold text-brand-600">MỚI CẬP NHẬT</p><h2 className="mt-1 text-2xl font-bold text-gray-900">Tin nổi bật</h2></div>
                        <span className="text-sm text-gray-500">{visibleNews.length} bài viết</span>
                    </div>
                    <div className="grid gap-5 md:grid-cols-2">
                        {visibleNews.map((item) => <article key={item.icon} className="group overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm transition hover:-translate-y-0.5 hover:border-brand-200 hover:shadow-lg">
                            <div className={`flex h-24 items-end justify-between bg-gradient-to-br ${item.accent} p-5 text-white`}><span className="rounded-full bg-white/15 px-3 py-1 text-xs font-bold backdrop-blur">{item.category}</span><span className="text-3xl font-bold text-white/35">{item.icon}</span></div>
                            <div className="p-5"><p className="text-xs font-semibold text-gray-400">{item.date}</p><h3 className="mt-2 text-lg font-bold leading-6 text-gray-900 group-hover:text-brand-600">{item.title}</h3><p className="mt-3 text-sm leading-6 text-gray-600">{item.excerpt}</p><button className="mt-4 text-sm font-bold text-brand-600 hover:text-brand-700">Xem chi tiết <span aria-hidden="true">→</span></button></div>
                        </article>)}
                    </div>
                    {visibleNews.length === 0 && <div className="rounded-2xl border border-dashed border-gray-300 bg-white px-6 py-12 text-center text-gray-500">Không tìm thấy tin tức phù hợp. Hãy thử một từ khóa khác.</div>}
                </section>

                <footer className="border-t border-brand-100 bg-white"><div className="mx-auto grid max-w-7xl gap-6 px-4 py-8 text-sm sm:px-6 md:grid-cols-2 lg:px-8"><div><p className="font-bold text-gray-900">HỆ THỐNG QUẢN LÝ XÂY DỰNG VĂN BẢN PHÁP LUẬT</p><p className="mt-2 leading-6 text-gray-500">Kênh thông tin hỗ trợ tra cứu, theo dõi tiến độ và công bố kết quả xử lý văn bản.</p></div><div className="md:text-right"><p className="font-semibold text-gray-700">Hỗ trợ kỹ thuật</p><p className="mt-2 text-gray-500">Liên hệ quản trị hệ thống để được hướng dẫn.</p></div></div></footer>
            </main>
        </>
    );
}
