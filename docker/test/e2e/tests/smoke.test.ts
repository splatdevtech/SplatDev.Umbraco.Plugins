import { test, expect } from '@playwright/test';

const ADMIN_USER = process.env.UMBRACO_ADMIN_USER || 'admin@splatdev.local';
const ADMIN_PASS = process.env.UMBRACO_ADMIN_PASS || 'SplatDev2024!';

test.describe('Smoke Tests', () => {
  test('should load the backoffice login or dashboard', async ({ page }) => {
    await page.goto('/umbraco');
    await expect(page).toHaveTitle(/Umbraco/);

    const title = page.locator('h1');
    const loginVisible = await title.isVisible().catch(() => false);

    if (loginVisible) {
      await expect(title).toContainText(/Sign in|Welcome/);
    }
  });

  test('should be able to login with default credentials', async ({ page }) => {
    await page.goto('/umbraco');

    const usernameInput = page.locator('input[name="username"],input[type="email"]');
    const isLoginPage = await usernameInput.isVisible().catch(() => false);

    if (!isLoginPage) {
      return;
    }

    await usernameInput.fill(ADMIN_USER);
    await page.fill('input[name="password"],input[type="password"]', ADMIN_PASS);
    await page.click('button[type="submit"]');

    await page.waitForURL('**/umbraco#/**', { timeout: 15000 });

    await expect(page.locator('body')).toBeVisible();
  });
});
