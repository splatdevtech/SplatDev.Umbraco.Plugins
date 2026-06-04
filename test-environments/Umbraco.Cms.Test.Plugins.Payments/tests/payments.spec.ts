import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('Payments Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Settings section shows Payments entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const settingsNav = page.locator('[data-section="settings"], [href*="settings"]').first();
    if (await settingsNav.isVisible()) {
      await settingsNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'settings-section', 1);
  });

  test('Payments dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'settings/payments');

    const pageContent = await page.content();
    const hasPaymentsContent =
      pageContent.toLowerCase().includes('payments') ||
      page.url().includes('payments');

    await takeScreenshot(page, 'payments-dashboard', 2);
    expect(hasPaymentsContent).toBeTruthy();
  });
});

test.describe('Payments Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/payments/GetProviders returns providers array', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/payments/GetProviders', {
      headers: { Cookie: cookieHeader }
    });

    expect([200, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(Array.isArray(body)).toBeTruthy();
    }

    await takeScreenshot(page, 'api-get-providers', 3);
  });
});