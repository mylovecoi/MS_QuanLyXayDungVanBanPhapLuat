import { useState } from 'react';
import { FrontendMenuItem, MenuLayout } from '../shared/api/systemApi';
import { useAuth } from '../shared/auth/AuthContext';
import { findFirstNavigableUrl, hasActiveChild, normalizeUrl } from '../shared/menu/menuUtils';

type MainLayoutProps = {
  activePath: string;
  children: React.ReactNode;
  menuError: string;
  menuItems: FrontendMenuItem[];
  menuLayout: MenuLayout;
  onNavigate: (path: string) => void;
};

function MenuItemButton({
  activePath,
  expandedMenuIds,
  item,
  onNavigate,
  onToggle
}: {
  activePath: string;
  expandedMenuIds: Set<string>;
  item: FrontendMenuItem;
  onNavigate: (path: string) => void;
  onToggle: (id: string) => void;
}) {
  const ownUrl = normalizeUrl(item.url);
  const targetUrl = ownUrl ?? findFirstNavigableUrl(item);
  const hasChildren = item.children.length > 0;
  const isExpanded = expandedMenuIds.has(item.roleActionId);
  const isActive = targetUrl === activePath || hasActiveChild(item, activePath);

  const handleClick = () => {
    if (hasChildren) {
      onToggle(item.roleActionId);
      return;
    }

    if (targetUrl) {
      onNavigate(targetUrl);
    }
  };

  return (
    <div className={`menu-node ${isExpanded ? 'expanded' : ''}`}>
      <button
        className={isActive ? 'active' : ''}
        disabled={!targetUrl && !hasChildren}
        onClick={handleClick}
        aria-expanded={hasChildren ? isExpanded : undefined}
        type="button"
      >
        <span>{item.title}</span>
        {hasChildren ? <small>{isExpanded ? 'Thu gọn' : `${item.children.length} chức năng`}</small> : null}
      </button>

      {hasChildren ? (
        <div className="submenu">
          {item.children.map((child) => (
            <MenuItemButton
              activePath={activePath}
              expandedMenuIds={expandedMenuIds}
              item={child}
              key={child.roleActionId}
              onNavigate={onNavigate}
              onToggle={onToggle}
            />
          ))}
        </div>
      ) : null}
    </div>
  );
}

export function MainLayout({ activePath, children, menuError, menuItems, menuLayout, onNavigate }: MainLayoutProps) {
  const { logout, user } = useAuth();
  const [expandedMenuIds, setExpandedMenuIds] = useState<Set<string>>(new Set());

  const handleToggleMenu = (id: string) => {
    setExpandedMenuIds((current) => {
      const next = new Set(current);

      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }

      return next;
    });
  };

  return (
    <main className={`app-shell ${menuLayout === 'horizontal' ? 'horizontal-menu' : 'vertical-menu'}`}>
      <aside className="sidebar">
        <div className="sidebar-brand">
          <div className="brand-mark">MS</div>
          <div>
            <strong>XDVB</strong>
            <span>Microservice FE</span>
          </div>
        </div>

        <nav className="sidebar-nav" aria-label="Điều hướng chính">
          {menuItems.map((item) => (
            <MenuItemButton
              activePath={activePath}
              expandedMenuIds={expandedMenuIds}
              item={item}
              key={item.roleActionId}
              onNavigate={onNavigate}
              onToggle={handleToggleMenu}
            />
          ))}
        </nav>
      </aside>

      <section className="workspace">
        <header className="topbar">
          <div>
            <p className="eyebrow">MS_QuanLyXayDungVanBanPhapLuat</p>
            <h1>Trang chủ</h1>
          </div>
          <div className="user-menu">
            <span className="layout-pill">{menuLayout === 'horizontal' ? 'Menu ngang' : 'Menu dọc'}</span>
            <span>{user?.displayName}</span>
            <button onClick={logout} type="button">
              Đăng xuất
            </button>
          </div>
        </header>

        {menuError ? <p className="menu-error">{menuError}</p> : null}
        {children}
      </section>
    </main>
  );
}
