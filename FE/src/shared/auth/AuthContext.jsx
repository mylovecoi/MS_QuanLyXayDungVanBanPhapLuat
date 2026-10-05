import { createContext, useContext, useMemo, useState } from 'react';
import { loginApi } from '../api/authApi';

const AUTH_STORAGE_KEY = 'ms_xdvb_auth';

const AuthContext = createContext(null);

function readStoredAuth() {
  const rawValue = window.localStorage.getItem(AUTH_STORAGE_KEY);

  if (!rawValue) {
    return null;
  }

  try {
    const parsedValue = JSON.parse(rawValue);

    if (
        !parsedValue.user?.userId ||
        !parsedValue.user?.username ||
        !parsedValue.user?.groupPermissionId
    ) {
      return null;
    }

    return parsedValue;
  } catch {
    window.localStorage.removeItem(AUTH_STORAGE_KEY);
    return null;
  }
}

export function AuthProvider({ children }) {
  const [authState, setAuthState] = useState(() =>
      readStoredAuth()
  );

  const value = useMemo(
      () => ({
        isAuthenticated: Boolean(authState?.user?.userId),

        user: authState?.user ?? null,

        currentUserHeaders: authState?.user
            ? {
              userId: authState.user.userId,
              username: authState.user.username,
              donViId: authState.user.donViId,
              groupPermissionId: authState.user.groupPermissionId,
              isSSA: authState.user.isSSA,
            }
            : null,

        async login(input) {
          const username = input.username.trim();
          const password = input.password.trim();

          if (!username || !password) {
            throw new Error(
                'Vui long nhap day du tai khoan va mat khau.'
            );
          }

          const loginResult = await loginApi(
              username,
              password
          );

          const nextAuthState = {
            user: {
              userId: loginResult.userId,
              username: loginResult.username,
              displayName:
                  loginResult.displayName ||
                  loginResult.username,
              donViId: loginResult.donViId,
              groupPermissionId:
              loginResult.groupPermissionId,
              isSSA: loginResult.isSSA,
            },
          };

          window.localStorage.setItem(
              AUTH_STORAGE_KEY,
              JSON.stringify(nextAuthState)
          );

          setAuthState(nextAuthState);
        },

        logout() {
          window.localStorage.removeItem(AUTH_STORAGE_KEY);
          setAuthState(null);
        },
      }),
      [authState]
  );

  return (
      <AuthContext.Provider value={value}>
        {children}
      </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error(
        'useAuth must be used inside AuthProvider.'
    );
  }

  return context;
}