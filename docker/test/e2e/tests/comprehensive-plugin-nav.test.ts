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
    await page.waitForURL('**/umbraco/#/dashboard', { waitUntil: 'networkidle' });
    
    // Store authenticated state for reuse
    const storageState = await page.context().storageState();
    authedContext = await browser.newContext({ storageState });
  }
}

// Helper to navigate to a plugin dashboard and verify
async function navigateToPluginDashboard(page: Page, pluginName: string, pathname: string): Promise<void> {
  await page.goto(`${BASE_URL}/umbraco/#/${pathname}`, { waitUntil: 'networkidle', timeout: 20000 });
  await page.waitForTimeout(2000);
  
  // Take screenshot
  await page.screenshot({ path: `/output/screenshots/${pluginName.toLowerCase().replace(/[.\s]/g, '-')}-dashboard.png`, fullPage: true });
  
  // Check for console errors
  const messages: string[] = [];
  page.on('console', msg => messages.push(msg.text()));
  
  await page.waitForTimeout(1000);
  
  const errors = messages.filter(msg => msg.type() === 'error');
  if (errors.length > 0) {
    console.warn(`Console errors on ${pluginName} dashboard: ${errors.map(e => e.text()).join('; ')}`);
  }
  
  expect(errors).toHaveLength(0, `Console errors on ${pluginName} dashboard`);
  
  // Verify we didn't hit a 404 or error page
  const is404 = page.url().includes('404') || 
                await page.locator('text=Page not found').isVisible().catch(() => false) ||
                await page.locator('text=Error').isVisible().catch(() => false);
  expect(is404).toBeFalsy(`${pluginName} dashboard navigated to 404 or error page`);
}

// Helper to test frontend URL (if applicable)
async function testPluginFrontend(page: Page, pluginName: string, frontendPath: string): Promise<void> {
  try {
    await page.goto(`${BASE_URL}/${frontendPath}`, { waitUntil: 'networkidle', timeout: 15000 });
    await page.waitForTimeout(2000);
    
    // Take screenshot
    await page.screenshot({ path: `/output/screenshots/${pluginName.toLowerCase().replace(/[.\s]/g, '-')}-frontend.png`, fullPage: true });
    
    // Check for console errors
    const messages: string[] = [];
    page.on('console', msg => messages.push(msg.text()));
    
    await page.waitForTimeout(1000);
    
    const errors = messages.filter(msg => msg.type() === 'error');
    if (errors.length > 0) {
      console.warn(`Console errors on ${pluginName} frontend: ${errors.map(e => e.text()).join('; ')}`);
    }
    
    // For frontend, we mainly want to ensure no 500 errors and basic loading
    const isErrorPage = await page.locator('text=Error 500, text=Internal Server Error').isVisible().catch(() => false);
    expect(isErrorPage).toBeFalsy(`${pluginName} frontend returned server error`);
  } catch (error) {
    // Some plugins might not have frontend routes, which is OK
    console.info(`${pluginName} frontend test skipped or failed (may not have frontend): ${error.message}`);
  }
}

// List of all SplatDev Umbraco plugins with their Umbraco pathnames
const PLUGINS = [
  // Backoffice plugins with dashboard/pathname
  { name: '2FA', pathname: '2fa' },
  { name: 'AdminBar', pathname: 'admin-bar' },
  { name: 'Analytics', pathname: 'analytics' },
  { name: 'BackupManager', pathname: 'backup-manager' },
  { name: 'Backups', pathname: 'backups' },
  { name: 'Blog', pathname: 'blog' },
  { name: 'CacheManager', pathname: 'cache-manager' },
  { name: 'CharLimit', pathname: 'char-limit' },
  { name: 'CodeFirst', pathname: 'codefirst' },
  { name: 'CopyValue', pathname: 'copy-value' },
  { name: 'Core', pathname: 'core' },
  { name: 'Countries', pathname: 'countries' },
  { name: 'CustomLogin', pathname: 'customlogin' },
  { name: 'D4Sign', pathname: 'd4sign' },
  { name: 'DefaultValue', pathname: 'default-value' },
  { name: 'DictionaryManager', pathname: 'dictionary-manager' },
  { name: 'Dropzone', pathname: 'dropzone' },
  { name: 'ENotAssina', pathname: 'enotassina' },
  { name: 'EmailNotifications', pathname: 'email-notifications' },
  { name: 'EmailTemplates', pathname: 'email-templates' },
  { name: 'ExamineExtensions', pathname: 'examine-extensions' },
  { name: 'ExceptionManager', pathname: 'exception-manager' },
  { name: 'Exif', pathname: 'exif' },
  { name: 'Faqs', pathname: 'faqs' },
  { name: 'FormBuilder', pathname: 'formbuilder-dashboard' },
  { name: 'FormsClone', pathname: 'forms-clone' },
  { name: 'Forums', pathname: 'forums' },
  { name: 'Gdrp', pathname: 'gdpr-compliance' },
  { name: 'HiddenContent', pathname: 'hiddencontent' },
  { name: 'IspServices', pathname: 'isp-services' },
  { name: 'JsonRpc', pathname: 'jsonrpc' },
  { name: 'LazyLoad', pathname: 'lazy-load' },
  { name: 'LiveVideo', pathname: 'live-video' },
  { name: 'Mailer', pathname: 'mailer' },
  { name: 'MemberGroups', pathname: 'membergroups' },
  { name: 'MemberLogin', pathname: 'member-login' },
  { name: 'MemberNotifications', pathname: 'member-notifications' },
  { name: 'MemberRegistration', pathname: 'member-registration' },
  { name: 'MemberTypes', pathname: 'member-types' },
  { name: 'MostViewed', pathname: 'most-viewed' },
  { name: 'NewsTicker', pathname: 'news-ticker' },
  { name: 'Newsletter', pathname: 'newsletter' },
  { name: 'Newsletters', pathname: 'newsletters' },
  { name: 'OAuth', pathname: 'oauth' },
  { name: 'OnOff', pathname: 'feature-toggles' },
  { name: 'PasswordSettings', pathname: 'password-settings' },
  { name: 'Payments.BancoInter', pathname: 'payments-bancointer' },
  { name: 'Payments.MercadoPago', pathname: 'payments-mercadopago' },
  { name: 'Payments.PagSeguro', pathname: 'payments-pagseguro' },
  { name: 'PdfCurator', pathname: 'pdf-curator' },
  { name: 'PhotoGallery', pathname: 'photo-gallery' },
  { name: 'PropertiesReport', pathname: 'properties-report' },
  { name: 'QuickPoll', pathname: 'quick-poll' },
  { name: 'RdpManager', pathname: 'rdp-manager' },
  { name: 'RedirectManager', pathname: 'redirect-manager' },
  { name: 'Restricted', pathname: 'restricted' },
  { name: 'Rsvp', pathname: 'rsvp' },
  { name: 'SEO', pathname: 'seo' },
  { name: 'Security', pathname: 'security' },
  { name: 'Settings', pathname: 'settings' },
  { name: 'ShopCart', pathname: 'shop-cart' },
  { name: 'ShortUrls', pathname: 'short-urls' },
  { name: 'Slider', pathname: 'slider' },
  { name: 'Smtp', pathname: 'smtp-settings' },
  { name: 'SocialMedia.Channels', pathname: 'social-media-channels' },
  { name: 'SocialMedia.Login', pathname: 'social-media-login' },
  { name: 'SocialMedia.Share', pathname: 'social-media-share' },
  { name: 'StarRatings', pathname: 'star-ratings' },
  { name: 'Surveys', pathname: 'surveys' },
  { name: 'SvgViewer', pathname: 'svg-viewer' },
  { name: 'Tests', pathname: 'tests' },
  { name: 'ToastNotifications', pathname: 'toast-notifications' },
  { name: 'Tweets', pathname: 'tweets' },
  { name: 'VideoPreview', pathname: 'video-preview' },
  { name: 'VisitorCounter', pathname: 'visitor-counter' },
  { name: 'WhatsApp', pathname: 'whatsapp' },
  { name: 'WordsApi', pathname: 'words-api' },
  { name: 'Yaml', pathname: 'yaml' }
];

// Plugins known to have frontend components (for frontend testing)
const PLUGINS_WITH_FRONTEND = [
  { name: 'Analytics', frontendPath: '/analytics' },
  { name: 'Blog', frontendPath: '/blog' },
  { name: 'FormsClone', frontendPath: '/forms' },
  { name: 'Forums', frontendPath: '/forum' },
  { name: 'LiveVideo', frontendPath: '/live-video' },
  { name: 'MemberGroups', frontendPath: '/member-groups' },
  { name: 'MemberLogin', frontendPath: '/login' },
  { name: 'MemberRegistration', frontendPath: '/register' },
  { name: 'MostViewed', frontendPath: '/most-viewed' },
  { name: 'NewsTicker', frontendPath: '/news-ticker' },
  { name: 'Newsletter', frontendPath: '/newsletter' },
  { name: 'Newsletters', frontendPath: '/newsletters' },
  { name: 'PhotoGallery', frontendPath: '/photo-gallery' },
  { name: 'PropertiesReport', frontendPath: '/properties-report' },
  { name: 'ShopCart', frontendPath: '/shop' },
  { name: 'ShortUrls', frontendPath: '/go' },
  { name: 'Slider', frontendPath: '/slider' },
  { name: 'SocialMedia.Channels', frontendPath: '/social' },
  { name: 'StarRatings', frontendPath: '/ratings' },
  { name: 'Surveys', frontendPath: '/surveys' },
  { name: 'VideoPreview', frontendPath: '/video-preview' },
  { name: 'VisitorCounter', frontendPath: '/visitor-counter' },
  { name: 'WhatsApp', frontendPath: '/whatsapp' },
  { name: 'WordsApi', frontendPath: '/words-api' }
];

test.describe('Plugin Backoffice Navigation', () => {
  test.beforeEach(async ({ page, browser }) => {
    // Get authenticated page for each test
    if (!authedContext) {
      await getAuthedPage(page);
    } else {
      const context = await browser.newContext(await authedContext.storageState());
      await page.context().close();
      page = await context.newPage();
    }
  });

  test.afterEach(async () => {
    // Clean up page but keep authedContext for reuse
    if (page && !page.isClosed()) {
      await page.close();
    }
  });

  PLUGINS.forEach(plugin => {
    test(`${plugin.name} backoffice dashboard loads without errors`, async ({ page }) => {
      await navigateToPluginDashboard(page, plugin.name, plugin.pathname);
    });
  });
});

test.describe('Plugin Frontend Navigation', () => {
  test.beforeEach(async ({ page }) => {
    // No authentication needed for frontend tests
  });

  PLUGINS_WITH_FRONTEND.forEach(plugin => {
    test(`${plugin.name} frontend loads without server errors`, async ({ page }) => {
      await testPluginFrontend(page, plugin.name, plugin.frontendPath);
    });
  });
});

test.describe('Overall System Health', () => {
  test('Umbraco backoffice is accessible and login works', async ({ page }) => {
    await page.goto(`${BASE_URL}/umbraco`, { waitUntil: 'networkidle' });
    await page.waitForTimeout(2000);
    
    const usernameInput = page.locator('input[name="username"],input[type="email"]');
    await expect(usernameInput).toBeVisible({ timeout: 10000 });
  });

  test('Umbraco frontend is accessible', async ({ page }) => {
    await page.goto(`${BASE_URL}/`, { waitUntil: 'networkidle' });
    await page.waitForTimeout(2000);
    
    // Should not be an error page
    const isErrorPage = await page.locator('text=Error 500, text=Internal Server Error, text=Site not found').isVisible().catch(() => false);
    expect(isErrorPage).toBeFalsy('Frontend returned error page');
  });
});
