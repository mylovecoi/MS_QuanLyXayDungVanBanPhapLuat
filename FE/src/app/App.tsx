import { useEffect, useMemo, useState } from 'react';
import { AuthProvider, useAuth } from '../shared/auth/AuthContext';
import { AuthLayout } from '../layouts/AuthLayout';
import { MainLayout } from '../layouts/MainLayout';
import { HomePage } from './home/HomePage';
import { LoginPage } from '../features/quan-tri-he-thong/auth/pages/LoginPage';
import { getFrontendMenu, getSystemInfo, FrontendMenuItem, MenuLayout } from '../shared/api/systemApi';
import { homeMenuItem } from '../shared/menu/menuUtils';
import { appFeatureRoutes } from './routes';

const normalizePath = (path: string) => {
  if (!path || path === '/') {
    return '/';
  }

  return path.replace(/\/+$/, '') || '/';
};

function useBrowserPath() {
  const [path, setPath] = useState(() => normalizePath(window.location.pathname));

  useEffect(() => {
    const handlePopState = () => setPath(normalizePath(window.location.pathname));
    window.addEventListener('popstate', handlePopState);

    return () => window.removeEventListener('popstate', handlePopState);
  }, []);

  return useMemo(
    () => ({
      path,
      navigate(nextPath: string, replace = false) {
        const normalizedPath = normalizePath(nextPath);

        if (normalizedPath === path) {
          return;
        }

        if (replace) {
          window.history.replaceState(null, '', normalizedPath);
        } else {
          window.history.pushState(null, '', normalizedPath);
        }

        setPath(normalizedPath);
      }
    }),
    [path]
  );
}

function AppRoutes() {
  const { isAuthenticated } = useAuth();
  const { path, navigate } = useBrowserPath();
  const [menuLayout, setMenuLayout] = useState<MenuLayout>('vertical');
  const [menuItems, setMenuItems] = useState<FrontendMenuItem[]>([homeMenuItem]);
  const [menuError, setMenuError] = useState('');

  useEffect(() => {
    if (!isAuthenticated && path !== '/login') {
      navigate('/login', true);
      return;
    }

    if (isAuthenticated && path === '/login') {
      navigate('/', true);
    }
  }, [isAuthenticated, navigate, path]);

  useEffect(() => {
    let isMounted = true;

    async function loadShellData() {
      if (!isAuthenticated) {
        setMenuItems([homeMenuItem]);
        setMenuLayout('vertical');
        return;
      }

      setMenuError('');

      try {
        const systemInfo = await getSystemInfo();

        if (isMounted) {
          setMenuLayout(systemInfo.menuLayout || 'vertical');
        }
      } catch {
        if (isMounted) {
          setMenuLayout('vertical');
        }
      }

      try {
        const menu = await getFrontendMenu();

        if (isMounted) {
          setMenuItems([homeMenuItem, ...(menu.items ?? [])]);
        }
      } catch (error) {
        if (isMounted) {
          setMenuError(error instanceof Error ? error.message : 'Không tải được menu hệ thống.');
          setMenuItems([homeMenuItem]);
        }
      }
    }

    loadShellData();

    return () => {
      isMounted = false;
    };
  }, [isAuthenticated]);

  if (!isAuthenticated) {
    return (
      <AuthLayout>
        <LoginPage onLoginSuccess={() => navigate('/', true)} />
      </AuthLayout>
    );
  }

  const matchedRoute = appFeatureRoutes.find((route) => route.path === path);
  const page = matchedRoute?.element ?? <HomePage menuItems={menuItems} onNavigate={navigate} />;

  return (
    <MainLayout
      activePath={path}
      menuError={menuError}
      menuItems={menuItems}
      menuLayout={menuLayout}
      onNavigate={navigate}
    >
      {page}
    </MainLayout>
  );
}

export function App() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  );
}
