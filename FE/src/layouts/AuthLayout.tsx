export function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <main className="auth-shell">
      <section className="auth-hero" aria-label="Gioi thieu he thong">
        <div className="brand-mark">MS</div>
        <div>
          <p className="eyebrow">Quản lý xây dựng văn bản pháp luật</p>
          <h1>Hệ thống điều hành nghiệp vụ tập trung</h1>
          <p className="hero-copy">
            Nền tảng frontend được khởi tạo theo hướng tách module, sẵn sàng kết nối các
            microservice Quản trị hệ thống và Danh mục.
          </p>
        </div>
      </section>
      <section className="auth-panel" aria-label="Đăng nhập">
        {children}
      </section>
    </main>
  );
}
