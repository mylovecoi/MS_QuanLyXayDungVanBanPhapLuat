import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';

function readInternalApiKey() {
  if (process.env.QTHT_INTERNAL_API_KEY) {
    return process.env.QTHT_INTERNAL_API_KEY;
  }

  try {
    const settingsPath = resolve(
        __dirname,
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
  plugins: [react()],

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
        target: 'https://localhost:7102',
        changeOrigin: true,
        secure: false
      }
    }
  }
});