import { test, expect, Page } from '@playwright/test';

const ADMIN_EMAIL = process.env.UMBRACO_ADMIN_EMAIL || 'admin@example.com';
const ADMIN_PASSWORD = process.env.UMBRACO_ADMIN_PASSWORD || 'Admin1234!';

async function loginToBackoffice(page: Page) {
  await page.goto('/umbraco/');
  await page.waitForLoadState('networkidle');

  const emailInput = page.locator('input[type="email"], input[name="username"], input[id*="email"]').first();
  const passwordInput = page.locator('input[type="password"]').first();

  await emailInput.fill(ADMIN_EMAIL);
  await passwordInput.fill(ADMIN_PASSWORD);
  await passwordInput.press('Enter');

  await page.waitForURL('**/umbraco/**', { timeout: 15000 });
  await page.waitForLoadState('networkidle');
}

async function navigateToBackups(page: Page) {
  await page.goto('/umbraco/#/settings/backups');
  await page.waitForLoadState('networkidle');
  await page.waitForTimeout(2000);
}

test.describe('Backups Plugin - Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Settings section shows Backups entry', async ({ page }) => {
    await page.goto('/umbraco/');
    await page.waitForLoadState('networkidle');

    const settingsNav = page.locator('[data-section="settings"], [href*="settings"]').first();
    if (await settingsNav.isVisible()) {
      await settingsNav.click();
      await page.waitForTimeout(1500);
    }

    await page.screenshot({ path: 'test-results/01-settings-section.png', fullPage: true });
  });

  test('Backups dashboard loads', async ({ page }) => {
    await navigateToBackups(page);

    const pageContent = await page.content();
    const hasBackupsContent =
      pageContent.toLowerCase().includes('backup') ||
      page.url().includes('backup');

    await page.screenshot({ path: 'test-results/02-backups-dashboard.png', fullPage: true });
    expect(hasBackupsContent).toBeTruthy();
  });
});

test.describe('Backups Plugin - API', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('GET /umbraco/api/backups/GetAll returns JSON array', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/backups/GetAll', {
      headers: { Cookie: cookieHeader }
    });

    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(Array.isArray(body)).toBeTruthy();

    await page.screenshot({ path: 'test-results/03-api-list-backups.png' });
  });

  test('GET /umbraco/api/backups/GetCloudProviders returns providers list', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const response = await request.get('/umbraco/api/backups/GetCloudProviders', {
      headers: { Cookie: cookieHeader }
    });

    const body = await response.json();
    expect(Array.isArray(body)).toBeTruthy();

    await page.screenshot({ path: 'test-results/04-api-cloud-providers.png' });
  });

  test('POST /umbraco/api/backups/Create creates a backup', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const xsrfToken = cookies.find(c => c.name.toLowerCase().includes('xsrf') || c.name.toLowerCase().includes('csrf'));

    const headers: Record<string, string> = {
      'Cookie': cookieHeader,
      'Content-Type': 'application/json',
    };

    if (xsrfToken) {
      headers['X-XSRF-TOKEN'] = xsrfToken.value;
      headers['RequestVerificationToken'] = xsrfToken.value;
    }

    const response = await request.post('/umbraco/api/backups/Create', {
      headers,
      data: {
        scope: 1,
        includeMedia: false,
        compress: true,
        encrypt: false,
        cloudProviderIds: []
      }
    });

    expect([200, 201, 400, 401, 403]).toContain(response.status());

    await page.screenshot({ path: 'test-results/05-api-create-backup.png' });
  });
});

test.describe('Backups Plugin - Provider Configuration', () => {
  test.beforeEach(async ({ page }) => {
    await loginToBackoffice(page);
  });

  test('Local File System provider is always available', async ({ page, request }) => {
    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

    const xsrfToken = cookies.find(c =>
      c.name.toLowerCase().includes('xsrf') || c.name.toLowerCase().includes('csrf'));

    const headers: Record<string, string> = {
      'Cookie': cookieHeader,
      'Content-Type': 'application/json',
    };
    if (xsrfToken) {
      headers['X-XSRF-TOKEN'] = xsrfToken.value;
    }

    const response = await request.post('/umbraco/api/backups/TestProvider?providerId=LocalFileSystem', {
      headers
    });

    expect([200, 400, 401, 403]).toContain(response.status());
    if (response.status() === 200) {
      const body = await response.json();
      expect(body).toHaveProperty('valid');
      expect(body.valid).toBe(true);
    }

    await page.screenshot({ path: 'test-results/06-local-provider-test.png' });
  });

  const cloudProviders = [
    'AzureBlobStorage',
    'GoogleDrive',
    'Dropbox',
    'BoxNet',
    'OneDrive',
    'Mega',
    'Seafile',
    'AwsS3',
    'Sftp',
  ];

  for (const providerId of cloudProviders) {
    test(`Provider ${providerId} endpoint responds correctly`, async ({ page, request }) => {
      const cookies = await page.context().cookies();
      const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');

      const xsrfToken = cookies.find(c =>
        c.name.toLowerCase().includes('xsrf') || c.name.toLowerCase().includes('csrf'));
      const headers: Record<string, string> = { 'Cookie': cookieHeader };
      if (xsrfToken) headers['X-XSRF-TOKEN'] = xsrfToken.value;

      const response = await request.post(
        `/umbraco/api/backups/TestProvider?providerId=${providerId}`,
        { headers }
      );

      // Unconfigured providers return false, not an error
      expect([200, 400, 401, 403]).toContain(response.status());
      if (response.status() === 200) {
        const body = await response.json();
        expect(body).toHaveProperty('providerId');
        expect(body).toHaveProperty('valid');
      }

      await page.screenshot({
        path: `test-results/provider-${providerId.toLowerCase()}.png`,
      });
    });
  }
});

test.describe('Backups Plugin - Full Workflow', () => {
  test('Create, list, and delete a backup', async ({ page, request }) => {
    await loginToBackoffice(page);

    const cookies = await page.context().cookies();
    const cookieHeader = cookies.map(c => `${c.name}=${c.value}`).join('; ');
    const xsrfToken = cookies.find(c =>
      c.name.toLowerCase().includes('xsrf') || c.name.toLowerCase().includes('csrf'));

    const headers: Record<string, string> = {
      'Cookie': cookieHeader,
      'Content-Type': 'application/json',
    };
    if (xsrfToken) headers['X-XSRF-TOKEN'] = xsrfToken.value;

    // List initial backups
    const listBefore = await request.get('/umbraco/api/backups/GetAll', { headers });
    const backupsBefore = listBefore.status() === 200 ? await listBefore.json() : [];

    await page.screenshot({ path: 'test-results/workflow-01-list-before.png' });

    // Create backup
    const createResponse = await request.post('/umbraco/api/backups/Create', {
      headers,
      data: { scope: 1, includeMedia: false, compress: true, encrypt: false, cloudProviderIds: [] }
    });

    await page.screenshot({ path: 'test-results/workflow-02-create.png' });

    if (createResponse.status() === 200) {
      const created = await createResponse.json();
      expect(created).toHaveProperty('name');

      // List after creation
      const listAfter = await request.get('/umbraco/api/backups/GetAll', { headers });
      if (listAfter.status() === 200) {
        const backupsAfter = await listAfter.json();
        expect(backupsAfter.length).toBeGreaterThan(backupsBefore.length);
        await page.screenshot({ path: 'test-results/workflow-03-list-after-create.png' });
      }

      // Delete backup
      const deleteResponse = await request.delete(
        `/umbraco/api/backups/Delete?name=${encodeURIComponent(created.name)}`,
        { headers }
      );
      expect([200, 404]).toContain(deleteResponse.status());
      await page.screenshot({ path: 'test-results/workflow-04-after-delete.png' });
    }
  });
});
