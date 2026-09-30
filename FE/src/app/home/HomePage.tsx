import { FrontendMenuItem } from '../../shared/api/systemApi';
import { findFirstNavigableUrl, flattenMenuItems } from '../../shared/menu/menuUtils';

type HomePageProps = {
  menuItems: FrontendMenuItem[];
  onNavigate: (path: string) => void;
};

const serviceHints: Record<string, string> = {
  Systems: 'QuanTriHeThongService',
  DanhMuc: 'DanhMucService'
};

function resolveServiceName(item: FrontendMenuItem) {
  const roleRoot = item.role.split('.')[0];
  return serviceHints[roleRoot] ?? roleRoot;
}

function countNavigableItems(item: FrontendMenuItem) {
  return flattenMenuItems([item]).filter((menuItem) => findFirstNavigableUrl(menuItem)).length;
}

export function HomePage({ menuItems, onNavigate }: HomePageProps) {
  const rootMenus = menuItems.filter((item) => item.roleActionId !== 'home');

  return (
    <section className="dashboard">
      <div className="summary-band">
        <p className="eyebrow">Tổng quan</p>
        <h2>Trang chủ kế thừa từ menu chức năng</h2>
        <p>
          Các khối bên dưới được sinh từ menu động theo quyền người dùng. Khi cấu hình
          RoleActions thay đổi, trang chủ và thanh menu sẽ cùng kế thừa một nguồn dữ liệu.
        </p>
      </div>

      <div className="module-grid">
        {rootMenus.map((item) => {
          const targetUrl = findFirstNavigableUrl(item);
          const childrenWithUrl = flattenMenuItems(item.children).filter((child) => findFirstNavigableUrl(child));

          return (
            <article className="module-card home-module-card" key={item.roleActionId}>
              <div>
                <h3>{item.title}</h3>
                <span>{resolveServiceName(item)}</span>
              </div>
              <p>{countNavigableItems(item)} chức năng có thể truy cập trong nhóm này.</p>
              <div className="quick-links">
                {childrenWithUrl.slice(0, 4).map((child) => (
                  <button
                    key={child.roleActionId}
                    onClick={() => {
                      const childUrl = findFirstNavigableUrl(child);
                      if (childUrl) {
                        onNavigate(childUrl);
                      }
                    }}
                    type="button"
                  >
                    {child.title}
                  </button>
                ))}
              </div>
              {targetUrl ? (
                <button className="secondary-button" onClick={() => onNavigate(targetUrl)} type="button">
                  Mở nhóm chức năng
                </button>
              ) : null}
            </article>
          );
        })}

        {rootMenus.length === 0 ? (
          <article className="module-card">
            <div>
              <h3>Chưa có menu chức năng</h3>
              <span>Đang chờ cấu hình</span>
            </div>
            <p>Hãy kiểm tra RoleActions hoặc quyền người dùng để hiển thị menu trên trang chủ.</p>
          </article>
        ) : null}
      </div>
    </section>
  );
}
