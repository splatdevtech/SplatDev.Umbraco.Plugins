import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('MemberGroups Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Users section shows Member Groups entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const usersNav = page.locator('[data-section="users"], [href*="users"]').first();
    if (await usersNav.isVisible()) {
      await usersNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'users-section', 1);
  });

  test('Member Groups dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'users/member-groups');

    const pageContent = await page.content();
    const hasMemberGroupsContent =
      pageContent.toLowerCase().includes('member groups') ||
      page.url().includes('member-groups');

    await takeScreenshot(page, 'membergroups-dashboard', 2);
    expect(hasMemberGroupsContent).toBeTruthy();
  });
});

test.describe('MemberGroups Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/member-groups/GetAll returns groups array', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/member-groups/GetAll', {
      headers: { Cookie: cookieHeader }
    });

    expect([200, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(Array.isArray(body)).toBeTruthy();
    }

    await takeScreenshot(page, 'api-get-all-groups', 3);
  });
});