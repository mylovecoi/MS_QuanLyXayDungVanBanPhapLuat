import { FormEvent, useState } from 'react';
import { useAuth } from '../../../../shared/auth/AuthContext';

type LoginPageProps = {
  onLoginSuccess: () => void;
};

export function LoginPage({ onLoginSuccess }: LoginPageProps) {
  const { login } = useAuth();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setErrorMessage('');
    setIsSubmitting(true);

    try {
      await login({ username, password });
      onLoginSuccess();
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : 'Đăng nhập không thành công.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form className="login-card" onSubmit={handleSubmit}>
      <div className="form-heading">
        <p className="eyebrow">Đăng nhập</p>
        <h2>Chào mừng trở lại</h2>
      </div>

      <label>
        <span>Tài khoản</span>
        <input
          autoComplete="username"
          autoFocus
          onChange={(event) => setUsername(event.target.value)}
          placeholder="Nhập tài khoản"
          type="text"
          value={username}
        />
      </label>

      <label>
        <span>Mật khẩu</span>
        <input
          autoComplete="current-password"
          onChange={(event) => setPassword(event.target.value)}
          placeholder="Nhập mật khẩu"
          type="password"
          value={password}
        />
      </label>

      {errorMessage ? <p className="form-error">{errorMessage}</p> : null}

      <button className="primary-button" disabled={isSubmitting} type="submit">
        {isSubmitting ? 'Đang xử lý...' : 'Đăng nhập'}
      </button>
    </form>
  );
}
