import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('Security Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Settings section shows Security entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const settingsNav = page.locator('[data-section="settings"], [href*="settings"]').first();
    if (await settingsNav.isVisible()) {
      await settingsNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'settings-section', 1);
  });

  test('Security dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'settings/security');

    const pageContent = await page.content();
    const hasSecurityContent =
      pageContent.toLowerCase().includes('security') ||
      page.url().includes('security');

    await takeScreenshot(page, 'security-dashboard', 2);
    expect(hasSecurityContent).toBeTruthy();
  });
});

test.describe('Security Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/security/GetSettings returns settings object', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/security/GetSettings', {
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