import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import svgr from 'vite-plugin-svgr';
import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import tailwindcss from "@tailwindcss/vite";

function readInternalApiKey() {
  if (process.env.QTHT_INTERNAL_API_KEY) {
    return process.env.QTHT_INTERNAL_API_KEY;
  }

  try {
    const settingsPath = resolve(
        import.meta.dirname,
        '../BE/services/QuanTriHeThongService/appsettings.Development.json'
    );

    const settings = JSON.parse(
        readFileSync(settingsPath, 'utf-8')
    );

    return settings.InternalApi?.ApiKey;
  } catch {
    return undefined;
  }
}

const internalApiKey = readInternalApiKey();

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
    svgr(),
  ],

  server: {
    port: 5173,

    proxy: {
      '/api/qtht': {
        target: 'http://localhost:5131',
        changeOrigin: true,

        configure: (proxy) => {
          proxy.on('proxyReq', (proxyReq) => {
            if (internalApiKey) {
              proxyReq.setHeader(
                  'X-Internal-Api-Key',
                  internalApiKey
              );
            }
          });
        },

        rewrite: (path) =>
            path.replace(/^\/api\/qtht/, '/api')
      },

      '/api/quan-tri-he-thong': {
        target: 'https://localhost:7101',
        changeOrigin: true,
        secure: false
      },

      '/api/danh-muc': {
        target: 'http://localhost:5132',
        changeOrigin: true,
      },

      '/api/khai-thac-du-lieu': {
        target: 'http://localhost:5138',
        changeOrigin: true,
      },

      '/api/xay-dung-van-ban': {
        target: process.env.XDVB_SERVICE_URL || 'http://localhost:50048',
        changeOrigin: true,
      }
    }
  }
});
