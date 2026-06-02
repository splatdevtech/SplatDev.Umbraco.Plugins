import { test, expect } from '@playwright/test';

test.describe('Smoke Tests', () => {
  test('should load the login page', async ({ page }) => {
    await page.goto('http://localhost:5000/umbraco');
    await expect(page).toHaveTitle(/Umbraco/);
    await expect(page.locator('h1')).toContainText('Sign in to the backoffice');
  });

  test('should be able to login with default credentials', async ({ page }) => {
    await page.goto('http://localhost:5000/umbraco');
    
    // Fill in login form
    await page.fill('input[name="username"]', 'admin@splatdev.local');
    await page.fill('input[name="password"]', 'SplatDev2024!');
    
    // Submit form
    await page.click('button[type="submit"]');
    
    // Wait for dashboard to load
    await page.waitForTimeout(5000);
    
    // Check if we're logged in (look for user menu or dashboard)
    await expect(page.locator('.umb-header__user-name')).toBeVisible();
  });
});
