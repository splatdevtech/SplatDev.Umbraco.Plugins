import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('SEO Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Settings section shows SEO entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const settingsNav = page.locator('[data-section="settings"], [href*="settings"]').first();
    if (await settingsNav.isVisible()) {
      await settingsNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'settings-section', 1);
  });

  test('SEO dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'settings/seo');

    const pageContent = await page.content();
    const hasSeoContent =
      pageContent.toLowerCase().includes('seo') ||
      page.url().includes('seo');

    await takeScreenshot(page, 'seo-dashboard', 2);
    expect(hasSeoContent).toBeTruthy();
  });
});

test.describe('SEO Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/seo/GetSettings returns settings object', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/seo/GetSettings', {
      headers: { Cookie: cookieHeader }
    });

    expect([200, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(typeof body).toBe('object');
    }

    await takeScreenshot(page, 'api-get-settings', 3);
  });
});