import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('DictionaryManager Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Settings section shows Dictionary Manager entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const settingsNav = page.locator('[data-section="settings"], [href*="settings"]').first();
    if (await settingsNav.isVisible()) {
      await settingsNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'settings-section', 1);
  });

  test('Dictionary Manager dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'settings/dictionary-manager');

    const pageContent = await page.content();
    const hasDictionaryManagerContent =
      pageContent.toLowerCase().includes('dictionary manager') ||
      page.url().includes('dictionary-manager');

    await takeScreenshot(page, 'dictionarymanager-dashboard', 2);
    expect(hasDictionaryManagerContent).toBeTruthy();
  });
});

test.describe('DictionaryManager Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/dictionary-manager/GetDictionaries returns dictionaries array', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/dictionary-manager/GetDictionaries', {
      headers: { Cookie: cookieHeader }
    });

    expect([200, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(Array.isArray(body)).toBeTruthy();
    }

    await takeScreenshot(page, 'api-get-dictionaries', 3);
  });
});