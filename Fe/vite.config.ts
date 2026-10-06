import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api/qtht': {
        target: 'http://localhost:5131',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/qtht/, '/api')
      },
      '/api/quan-tri-he-thong': {
        target: 'https://localhost:7101',
        changeOrigin: true,
        secure: false
      },
      '/api/danh-muc': {
        target: 'http://localhost:5132',
        changeOrigin: true
      },
      '/api/xay-dung-van-ban': {
        target: 'http://localhost:50048',
        changeOrigin: true
      },
      '/api/dang-ky-xay-dung-van-ban': {
        target: 'http://localhost:60578',
        changeOrigin: true
      }
    }
  }
});
