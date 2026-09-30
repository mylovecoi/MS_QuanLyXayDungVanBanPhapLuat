import { createContext, useContext, useMemo, useState } from 'react';
import { loginApi } from '../api/authApi';
import { CurrentUserHeaders } from '../api/systemApi';

type AuthUser = {
  userId: string;
  username: string;
  displayName: string;
  donViId?: string;
  groupPermissionId: string;
  isSSA: boolean;
};

type AuthState = {
  user: AuthUser;
};

type LoginInput = {
  username: string;
  password: string;
};

type AuthContextValue = {
  isAuthenticated: boolean;
  user: AuthUser | null;
  currentUserHeaders: CurrentUserHeaders | null;
  login: (input: LoginInput) => Promise<void>;
  logout: () => void;
};

const AUTH_STORAGE_KEY = 'ms_xdvb_auth';

const AuthContext = createContext<AuthContextValue | null>(null);

function readStoredAuth(): AuthState | null {
  const rawValue = window.localStorage.getItem(AUTH_STORAGE_KEY);

  if (!rawValue) {
    return null;
  }

  try {
    const parsedValue = JSON.parse(rawValue) as AuthState;

    if (!parsedValue.user?.userId || !parsedValue.user?.username || !parsedValue.user?.groupPermissionId) {
      return null;
    }

    return parsedValue;
  } catch {
    window.localStorage.removeItem(AUTH_STORAGE_KEY);
    return null;
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [authState, setAuthState] = useState<AuthState | null>(() => readStoredAuth());

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: Boolean(authState?.user?.userId),
      user: authState?.user ?? null,
      currentUserHeaders: authState?.user
        ? {
            userId: authState.user.userId,
            username: authState.user.username,
            donViId: authState.user.donViId,
            groupPermissionId: authState.user.groupPermissionId,
            isSSA: authState.user.isSSA
          }
        : null,
      async login(input) {
        const username = input.username.trim();
        const password = input.password.trim();

        if (!username || !password) {
          throw new Error('Vui long nhap day du tai khoan va mat khau.');
        }

        const loginResult = await loginApi(username, password);
        const nextAuthState: AuthState = {
          user: {
            userId: loginResult.userId,
            username: loginResult.username,
            displayName: loginResult.displayName || loginResult.username,
            donViId: loginResult.donViId,
            groupPermissionId: loginResult.groupPermissionId,
            isSSA: loginResult.isSSA
          }
        };

        window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(nextAuthState));
        setAuthState(nextAuthState);
      },
      logout() {
        window.localStorage.removeItem(AUTH_STORAGE_KEY);
        setAuthState(null);
      }
    }),
    [authState]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider.');
  }

  return context;
}
