import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api/quan-tri-he-thong': {
        target: 'https://localhost:7101',
        changeOrigin: true,
        secure: false
      },
      '/api/danh-muc': {
        target: 'https://localhost:7102',
        changeOrigin: true,
        secure: false
      }
    }
  }
});
