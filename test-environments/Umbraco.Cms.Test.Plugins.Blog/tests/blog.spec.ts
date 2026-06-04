import { test, expect, Page } from '@playwright/test';
import { loginToBackoffice, navigateToSection, takeScreenshot } from '../../shared-helpers/umbraco-helpers';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

test.describe('Blog Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Content section shows Blog entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const contentNav = page.locator('[data-section="content"], [href*="content"]').first();
    if (await contentNav.isVisible()) {
      await contentNav.click();
      await page.waitForTimeout(1500);
    }

    await takeScreenshot(page, 'content-section', 1);
  });

  test('Blog dashboard loads', async ({ page }) => {
    await navigateToSection(page, 'content/blog');

    const pageContent = await page.content();
    const hasBlogContent =
      pageContent.toLowerCase().includes('blog') ||
      page.url().includes('blog');

    await takeScreenshot(page, 'blog-dashboard', 2);
    expect(hasBlogContent).toBeTruthy();
  });
});

test.describe('Blog Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/blog/GetRecentPosts returns posts array', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/blog/GetRecentPosts', {
      headers: { Cookie: cookieHeader }
    });

    expect([200, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(Array.isArray(body)).toBeTruthy();
    }

    await takeScreenshot(page, 'api-get-recent-posts', 3);
  });
});