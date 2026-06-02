import { test, expect, Page, BrowserContext } from '@playwright/test';

const ADMIN_USER = process.env.UMBRACO_ADMIN_USER || 'admin@splatdev.com';
const ADMIN_PASS = process.env.UMBRACO_ADMIN_PASS || 'SplatDev123!';
const BASE_URL = process.env.UMBRACO_URL || 'http://localhost:5000';

// --- Login helper (shared session via storageState) ---

let authedContext: BrowserContext | null = null;

async function getAuthedPage(page: Page): Promise<void> {
  await page.goto(`${BASE_URL}/umbraco`);
  await page.waitForLoadState('networkidle');

  const usernameInput = page.locator('input[name="username"],input[type="email"]');
  const isLoginPage = await usernameInput.isVisible({ timeout: 5000 }).catch(() => false);

  if (isLoginPage) {
    await usernameInput.fill(ADMIN_USER);
    await page.fill('input[name="password"],input[type="password"]', ADMIN_PASS);
    await page.click('button[type="submit"]');
    await page.waitForURL(/umbraco[#/]/, { timeout: 20000 });
    await page.waitForLoadState('networkidle');
  }
}

async function navigateToDashboard(page: Page, pathname: string, screenshotName: string): Promise<void> {
  const errors: string[] = [];
  page.on('console', msg => {
    if (msg.type() === 'error') errors.push(msg.text());
  });

  await page.goto(`${BASE_URL}/umbraco/settings/${pathname}`, { waitUntil: 'networkidle', timeout: 20000 });
  await page.waitForTimeout(2000);

  const safeErrors = errors.filter(e =>
    !e.includes('favicon') &&
    !e.includes('404') &&        // expected for unconfigured API endpoints
    !e.includes('ERR_EMPTY_RESPONSE') &&
    !e.includes('ResizeObserver')
  );

  await page.screenshot({
    path: `/output/screenshots/${screenshotName}.png`,
    fullPage: true,
  });

  expect(safeErrors, `Console errors on ${pathname}: ${safeErrors.join('; ')}`).toHaveLength(0);
}

// ---------------------------------------------------------------------------
// All plugins with dashboards in Umb.Section.Settings
// ---------------------------------------------------------------------------

const PLUGINS = [
  { name: 'Analytics',              pathname: 'analytics' },
  { name: 'AdminBar',               pathname: 'admin-bar' },
  { name: 'Backups',                pathname: 'backups' },
  { name: 'Blog',                   pathname: 'blog' },
  { name: 'CacheManager',           pathname: 'cache-manager' },
  { name: 'CharLimit',              pathname: 'char-limit' },
  { name: 'CustomLogin',            pathname: 'customlogin' },
  { name: 'D4Sign',                 pathname: 'd4sign' },
  { name: 'DictionaryManager',      pathname: 'dictionary-manager' },
  { name: 'ENotAssina',             pathname: 'enotassina' },
  { name: 'ExamineExtensions',      pathname: 'examine-extensions' },
  { name: 'ExceptionManager',       pathname: 'exception-manager' },
  { name: 'Faqs',                   pathname: 'faqs' },
  { name: 'FormBuilder',            pathname: 'formbuilder-dashboard' },
  { name: 'Gdrp',                   pathname: 'gdpr-compliance' },
  { name: 'HiddenContent',          pathname: 'hiddencontent' },
  { name: 'ImageProcessor',         pathname: 'image-processor' },
  { name: 'LiveVideo',              pathname: 'live-video' },
  { name: 'Mailer',                 pathname: 'mailer' },
  { name: 'MemberGroups',           pathname: 'membergroups' },
  { name: 'MemberLogin',            pathname: 'memberlogin' },
  { name: 'MemberRegistration',     pathname: 'memberregistration' },
  { name: 'MemberTypes',            pathname: 'membertypes' },
  { name: 'MostViewed',             pathname: 'most-viewed' },
  { name: 'NewsTicker',             pathname: 'news-ticker' },
  { name: 'Newsletters',            pathname: 'newsletters' },
  { name: 'OnOff',                  pathname: 'feature-toggles' },
  { name: 'PasswordSettings',       pathname: 'passwordsettings' },
  { name: 'PropertiesReport',       pathname: 'properties-report' },
  { name: 'QuickPoll',              pathname: 'quick-poll' },
  { name: 'RdpManager',             pathname: 'rdp-manager' },
  { name: 'RedirectManager',        pathname: 'redirect-manager' },
  { name: 'Restricted',             pathname: 'restricted' },
  { name: 'Rsvp',                   pathname: 'rsvp' },
  { name: 'SEO',                    pathname: 'seo' },
  { name: 'Schema2Yaml',            pathname: 'schema-export' },
  { name: 'Settings',               pathname: 'site-settings' },
  { name: 'ShopCart',               pathname: 'shop-cart' },
  { name: 'ShortUrls',              pathname: 'short-urls' },
  { name: 'Smtp',                   pathname: 'smtp-settings' },
  { name: 'SocialMedia.Channels',   pathname: 'social-channels' },
  { name: 'SocialMedia.Login',      pathname: 'social-login' },
  { name: 'SocialMedia.Share',      pathname: 'social-share' },
  { name: 'StarRatings',            pathname: 'star-ratings' },
  { name: 'Surveys',                pathname: 'surveys' },
  { name: 'SvgViewer',             pathname: 'svg-viewer' },
  { name: 'ToastNotifications',     pathname: 'toast-notifications' },
  { name: 'Tweets',                 pathname: 'tweets' },
  { name: 'VideoPreview',           pathname: 'video-preview' },
  { name: 'VisitorCounter',         pathname: 'visitor-counter' },
  { name: 'WhatsApp',              pathname: 'whatsapp' },
  { name: 'Yaml2Schema',            pathname: 'yaml-import' },
  { name: 'BancoInter',             pathname: 'banco-inter' },
  { name: 'MercadoPago',            pathname: 'mercadopago' },
  { name: 'PagSeguro',              pathname: 'pagseguro' },
];

test.describe('Plugin Dashboard Navigation — All Plugins', () => {
  test.beforeAll(async ({ browser }) => {
    // Warm up login once
    const page = await browser.newPage();
    await getAuthedPage(page);
    await page.close();
  });

  test.beforeEach(async ({ page }) => {
    await getAuthedPage(page);
  });

  for (const plugin of PLUGINS) {
    test(`${plugin.name} dashboard loads without errors`, async ({ page }) => {
      await navigateToDashboard(page, plugin.pathname, `plugin-${plugin.name.toLowerCase().replace(/[\.\s]/g, '-')}`);

      // Verify the page did not navigate to a 404 or error page
      const url = page.url();
      const bodyText = await page.locator('body').innerText().catch(() => '');

      const is404 = url.includes('404') || bodyText.toLowerCase().includes('page not found') || bodyText.toLowerCase().includes('error 404');
      expect(is404, `${plugin.name}: navigated to 404`).toBe(false);
    });
  }
});

// ---------------------------------------------------------------------------
// Settings section is reachable and shows plugin list
// ---------------------------------------------------------------------------

test.describe('Settings Section Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await getAuthedPage(page);
  });

  test('Settings section accessible', async ({ page }) => {
    await page.goto(`${BASE_URL}/umbraco/settings`, { waitUntil: 'networkidle', timeout: 20000 });
    await page.waitForTimeout(2000);
    await page.screenshot({ path: '/output/screenshots/settings-section.png', fullPage: true });

    const url = page.url();
    expect(url).toContain('settings');
  });

  test('Backoffice login page renders', async ({ page }) => {
    await page.goto(`${BASE_URL}/umbraco`);
    await expect(page).toHaveTitle(/Umbraco/);
    await page.screenshot({ path: '/output/screenshots/00-login.png', fullPage: true });
  });

  test('Backoffice dashboard accessible after login', async ({ page }) => {
    await getAuthedPage(page);
    await page.screenshot({ path: '/output/screenshots/01-dashboard.png', fullPage: true });
    const url = page.url();
    expect(url).toContain('umbraco');
  });
});
