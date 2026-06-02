import { defineConfig, devices } from '@playwright/test';

/**
 * Shared Playwright config for SplatDev Umbraco plugin evidence capture.
 * Used inside both U17 (port 5000) and U13 (port 5001) test containers.
 *
 * Environment variables:
 *   UMBRACO_URL          - base URL (default: http://localhost:5000)
 *   PLAYWRIGHT_OUTPUT_DIR - artifacts root (default: /output/test-results)
 */
export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: 1,
  reporter: [
    ['html', { outputFolder: '/output/playwright-report', open: 'never' }],
    ['list'],
    ['json', { outputFile: '/output/playwright-results.json' }],
  ],
  outputDir: process.env.PLAYWRIGHT_OUTPUT_DIR || '/output/test-results',
  use: {
    baseURL: process.env.UMBRACO_URL || 'http://localhost:5000',
    screenshot: 'on',
    video: 'on',
    trace: 'on-first-retry',
    viewport: { width: 1280, height: 800 },
    deviceScaleFactor: 2,
    launchOptions: {
      args: ['--no-sandbox', '--disable-setuid-sandbox'],
    },
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
