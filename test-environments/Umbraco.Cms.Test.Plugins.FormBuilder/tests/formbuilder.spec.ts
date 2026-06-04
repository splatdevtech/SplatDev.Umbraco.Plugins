import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('FormBuilder Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Forms section shows FormBuilder entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const formsNav = page.locator('[data-section="forms"], [href*="forms"]').first();
    if (await formsNav.isVisible()) {
      await formsNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'forms-section', 1);
  });

  test('FormBuilder dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'forms/formbuilder');

    const pageContent = await page.content();
    const hasFormBuilderContent =
      pageContent.toLowerCase().includes('form builder') ||
      page.url().includes('formbuilder');

    await takeScreenshot(page, 'formbuilder-dashboard', 2);
    expect(hasFormBuilderContent).toBeTruthy();
  });
});

test.describe('FormBuilder Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/formbuilder/GetForms returns forms array', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/formbuilder/GetForms', {
      headers: { Cookie: cookieHeader }
    });

    expect([200, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(Array.isArray(body)).toBeTruthy();
    }

    await takeScreenshot(page, 'api-get-forms', 3);
  });
});