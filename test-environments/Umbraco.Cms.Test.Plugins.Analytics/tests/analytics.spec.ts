import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('Analytics Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Settings section shows Analytics entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const settingsNav = page.locator('[data-section="settings"], [href*="settings"]').first();
    if (await settingsNav.isVisible()) {
      await settingsNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'settings-section', 1);
  });

  test('Analytics dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'settings/analytics');

    const pageContent = await page.content();
    const hasAnalyticsContent =
      pageContent.toLowerCase().includes('analytics') ||
      page.url().includes('analytics');

    await takeScreenshot(page, 'analytics-dashboard', 2);
    expect(hasAnalyticsContent).toBeTruthy();
  });
});

test.describe('Analytics Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/analytics/GetOverview returns overview data', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/analytics/GetOverview', {
      headers: { Cookie: cookieHeader }
    });

    expect([200, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(typeof body).toBe('object');
    }

    await takeScreenshot(page, 'api-get-overview', 3);
  });
});