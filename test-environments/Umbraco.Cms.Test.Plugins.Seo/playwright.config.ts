import { defineConfig, devices } from '@playwright/test';
import { BASE_CONFIG } from '../shared-helpers/playwright.base.config';

export default defineConfig({
  ...BASE_CONFIG,
  testDir: './tests',
  use: {
    ...BASE_CONFIG.use,
    baseURL: process.env.UMBRACO_URL || 'http://localhost:5100',
  },
});