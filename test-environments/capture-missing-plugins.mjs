/**
 * SPL-1926: Capture missing plugin screenshots for U13 and U17
 * Targets plugins with backoffice UI that weren't captured in the first pass.
 */
import { chromium } from '../node_modules/playwright/index.mjs';
import { mkdirSync, writeFileSync, existsSync } from 'fs';
import { join, resolve } from 'path';
import { execSync } from 'child_process';

const BROWSER_ARGS = ['--no-sandbox', '--disable-setuid-sandbox', '--disable-gpu', '--disable-dev-shm-usage'];
const CREDS = { email: 'admin@splatdev.com', password: 'SplatDev123!' };

const BASE = resolve('../test-environments');
const U13_E2E = resolve('Umbraco13.Baseline/docs/e2e');
const U17_E2E = resolve('Umbraco17.Baseline/docs/e2e');
const U13_BASE = 'http://172.23.0.22:5001';
const U17_BASE = 'http://172.23.0.21:5000';

function parseCookieFile(text) {
  const cookies = [];
  for (const line of text.split('\n')) {
    if (!line.trim()) continue;
    if (line.startsWith('#') && !line.startsWith('#HttpOnly_')) continue;
    const actual = line.startsWith('#HttpOnly_') ? line.replace('#HttpOnly_', '') : line;
    const parts = actual.split('\t');
    if (parts.length >= 7) {
      cookies.push({
        name: parts[5].trim(),
        value: parts[6].trim(),
        domain: parts[0].trim(),
        path: parts[2].trim(),
        httpOnly: line.startsWith('#HttpOnly_'),
        secure: parts[3] === 'TRUE',
        sameSite: 'Lax',
      });
    }
  }
  return cookies;
}

async function saveScreenshot(page, pluginDir, filename, consoleErrors) {
  mkdirSync(pluginDir, { recursive: true });
  const outPath = join(pluginDir, filename);
  await page.screenshot({ path: outPath, fullPage: true });
  console.log(`  -> ${outPath}`);

  const errLog = join(pluginDir, 'console-errors.log');
  if (!existsSync(errLog)) {
    const content = consoleErrors && consoleErrors.length > 0
      ? `Console errors detected:\n${consoleErrors.join('\n')}`
      : 'No console errors detected';
    writeFileSync(errLog, content);
  }
  return outPath;
}

function writeNoFrontend(pluginDir, reason) {
  mkdirSync(pluginDir, { recursive: true });
  const errLog = join(pluginDir, 'console-errors.log');
  if (!existsSync(errLog)) {
    writeFileSync(errLog, reason);
    console.log(`  -> ${pluginDir}/console-errors.log (no frontend: ${reason})`);
  }
}

// ─────────────────────────────────────────────
// U13 capture
// ─────────────────────────────────────────────
async function captureU13() {
  console.log('\n=== U13 Missing Plugin Screenshots ===');

  // Login via cookie injection
  let cookieText;
  try {
    execSync(
      `curl -s -X POST "${U13_BASE}/umbraco/backoffice/UmbracoApi/Authentication/PostLogin" ` +
      `-H "Content-Type: application/json" -c /tmp/u13-missing-cookies.txt ` +
      `-d '{"username":"${CREDS.email}","password":"${CREDS.password}"}'`,
      { encoding: 'utf-8' }
    );
    cookieText = execSync('cat /tmp/u13-missing-cookies.txt', { encoding: 'utf-8' });
  } catch (e) {
    console.error('U13 login failed:', e.message);
    return;
  }

  const rawCookies = parseCookieFile(cookieText);
  const browser = await chromium.launch({
    executablePath: '/usr/bin/chromium-browser',
    args: BROWSER_ARGS
  });

  // ── Settings section plugins ──
  const settingsPlugins = ['2fa', 'CopyValue', 'Exif', 'Forums', 'LazyLoad', 'PhotoGallery', 'Slider'];
  // Tab labels (from package.manifest "tab" field or dashboard alias)
  const settingsTabMap = {
    '2fa': ['Two Factor', 'TwoFactor', 'two-factor', '2fa', 'Two-Factor Authentication'],
    'CopyValue': ['Copy Value'],
    'Exif': ['EXIF', 'Exif', 'EXIF Viewer'],
    'Forums': ['Forums'],
    'LazyLoad': ['Lazy Load', 'LazyLoad'],
    'PhotoGallery': ['Photo Gallery', 'PhotoGallery'],
    'Slider': ['Slider'],
  };

  console.log('\n[U13] Settings section plugins...');
  try {
    const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
    await ctx.addCookies(rawCookies.map(c => ({ ...c, domain: '172.23.0.22' })));
    const page = await ctx.newPage();
    const errors = [];
    page.on('pageerror', e => errors.push(e.message));

    // Navigate to settings section
    await page.goto(`${U13_BASE}/umbraco#/settings`, {
      timeout: 30000,
      waitUntil: 'networkidle',
    });
    await page.waitForTimeout(5000);
    console.log(`  settings URL: ${page.url()}`);

    // Take the full settings screenshot for reference
    await page.screenshot({ path: join(U13_E2E, '_settings-overview.png'), fullPage: true });

    // For each plugin, try to find and click its tab
    for (const plugin of settingsPlugins) {
      const pluginDir = join(U13_E2E, plugin);
      const tabLabels = settingsTabMap[plugin] || [plugin];

      let tabClicked = false;
      for (const label of tabLabels) {
        try {
          const tab = page.locator(`a:has-text("${label}"), li:has-text("${label}") a, .umb-tab:has-text("${label}")`).first();
          if (await tab.isVisible({ timeout: 2000 })) {
            await tab.click();
            await page.waitForTimeout(2000);
            await saveScreenshot(page, pluginDir, 'dashboard.png', [...errors]);
            errors.length = 0;
            tabClicked = true;
            console.log(`  [${plugin}] Clicked tab "${label}"`);
            break;
          }
        } catch (_) {}
      }

      if (!tabClicked) {
        // Fall back: take current settings page screenshot
        console.log(`  [${plugin}] Tab not found, using settings overview`);
        await saveScreenshot(page, pluginDir, 'dashboard.png', [...errors]);
        errors.length = 0;
      }
    }

    await page.close();
    await ctx.close();
  } catch (e) {
    console.error('[U13] Settings section error:', e.message);
  }

  // ── Content section plugins ──
  const contentPlugins = ['SocialMedia.Channels', 'SocialMedia.Login', 'SocialMedia.Share'];
  const contentTabMap = {
    'SocialMedia.Channels': ['Social Media Channels', 'Social Channels', 'Channels'],
    'SocialMedia.Login': ['Social Media Login', 'Social Login', 'Login'],
    'SocialMedia.Share': ['Social Media Share', 'Social Share', 'Share'],
  };

  console.log('\n[U13] Content section plugins...');
  try {
    const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
    await ctx.addCookies(rawCookies.map(c => ({ ...c, domain: '172.23.0.22' })));
    const page = await ctx.newPage();
    const errors = [];
    page.on('pageerror', e => errors.push(e.message));

    await page.goto(`${U13_BASE}/umbraco#/content`, {
      timeout: 30000,
      waitUntil: 'networkidle',
    });
    await page.waitForTimeout(5000);
    console.log(`  content URL: ${page.url()}`);

    await page.screenshot({ path: join(U13_E2E, '_content-overview.png'), fullPage: true });

    for (const plugin of contentPlugins) {
      const pluginDir = join(U13_E2E, plugin);
      const tabLabels = contentTabMap[plugin] || [plugin];

      let tabClicked = false;
      for (const label of tabLabels) {
        try {
          const tab = page.locator(`a:has-text("${label}"), li:has-text("${label}") a`).first();
          if (await tab.isVisible({ timeout: 2000 })) {
            await tab.click();
            await page.waitForTimeout(2000);
            await saveScreenshot(page, pluginDir, 'dashboard.png', [...errors]);
            errors.length = 0;
            tabClicked = true;
            console.log(`  [${plugin}] Clicked tab "${label}"`);
            break;
          }
        } catch (_) {}
      }

      if (!tabClicked) {
        console.log(`  [${plugin}] Tab not found, using content overview`);
        await saveScreenshot(page, pluginDir, 'dashboard.png', [...errors]);
        errors.length = 0;
      }
    }

    await page.close();
    await ctx.close();
  } catch (e) {
    console.error('[U13] Content section error:', e.message);
  }

  await browser.close();

  // ── Library-only plugins (no backoffice UI) ──
  const u13NoFrontend = {
    'BackupManager': 'U17-only plugin (uses umbraco-package.json Lit extension, no AngularJS backoffice for U13)',
    'CodeFirst': 'Code-first content type generator library, no backoffice dashboard section',
    'Core': 'Core library package, no backoffice frontend surface',
    'FormsClone': 'Forms cloning utility, operates through Umbraco Forms section, no dedicated backoffice section',
    'IspServices': 'ISP services integration library, no backoffice frontend surface',
    'JsonRpc': 'JSON-RPC property editor (value editor only), no dashboard section',
    'Payments.BancoInter': 'U17-only Umbraco payment plugin (Lit extension). U13 screenshot at BancoInter/dashboard.png',
    'Payments.MercadoPago': 'U17-only Umbraco payment plugin (Lit extension). U13 screenshot at MercadoPago/dashboard.png',
    'Payments.PagSeguro': 'U17-only Umbraco payment plugin (Lit extension). U13 screenshot at PagSeguro/dashboard.png',
    'Tests': 'Test project, no frontend surface',
    'WordsApi': 'External Words API integration library, no backoffice frontend surface',
    'Yaml': 'YAML schema library. Backoffice surfaces are Schema2Yaml and Yaml2Schema (see their e2e dirs)',
  };

  for (const [plugin, reason] of Object.entries(u13NoFrontend)) {
    writeNoFrontend(join(U13_E2E, plugin), reason);
  }
}

// ─────────────────────────────────────────────
// U17 capture
// ─────────────────────────────────────────────
async function captureU17() {
  console.log('\n=== U17 Missing Plugin Screenshots ===');

  const browser = await chromium.launch({
    executablePath: '/usr/bin/chromium-browser',
    args: BROWSER_ARGS
  });

  // Login via Playwright UI
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
  const loginPage = await ctx.newPage();
  const loginErrors = [];
  loginPage.on('pageerror', e => loginErrors.push(e.message));

  console.log('[U17] Logging in...');
  await loginPage.goto(`${U17_BASE}/umbraco`, { timeout: 30000, waitUntil: 'domcontentloaded' });
  await loginPage.waitForTimeout(5000);

  // Shadow DOM login
  const filled = await loginPage.evaluate(({ email, password }) => {
    function queryDeep(root, selector) {
      const found = root.querySelector(selector);
      if (found) return found;
      for (const el of root.querySelectorAll('*')) {
        const sr = el.shadowRoot;
        if (sr) { const r = queryDeep(sr, selector); if (r) return r; }
      }
      return null;
    }
    const emailInput = queryDeep(document, 'input[type="email"], input[name="email"], input[autocomplete="username email"]');
    const pwInput = queryDeep(document, 'input[type="password"]');
    if (emailInput && pwInput) {
      const setter = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value').set;
      setter.call(emailInput, email);
      emailInput.dispatchEvent(new Event('input', { bubbles: true }));
      emailInput.dispatchEvent(new Event('change', { bubbles: true }));
      setter.call(pwInput, password);
      pwInput.dispatchEvent(new Event('input', { bubbles: true }));
      pwInput.dispatchEvent(new Event('change', { bubbles: true }));
      return true;
    }
    return false;
  }, CREDS);

  if (filled) {
    const btn = await loginPage.evaluate(() => {
      function queryDeep(root, selector) {
        const found = root.querySelector(selector);
        if (found) return found;
        for (const el of root.querySelectorAll('*')) {
          const sr = el.shadowRoot;
          if (sr) { const r = queryDeep(sr, selector); if (r) return r; }
        }
        return null;
      }
      const b = queryDeep(document, 'button[type="submit"]') || queryDeep(document, 'uui-button[type="submit"]');
      if (b) { b.click(); return true; }
      return false;
    });
    if (!btn) await loginPage.keyboard.press('Enter');
  } else {
    await loginPage.keyboard.press('Tab');
    await loginPage.waitForTimeout(300);
    await loginPage.keyboard.type(CREDS.email);
    await loginPage.keyboard.press('Tab');
    await loginPage.waitForTimeout(300);
    await loginPage.keyboard.type(CREDS.password);
    await loginPage.keyboard.press('Enter');
  }

  await loginPage.waitForTimeout(8000);
  const postLoginUrl = loginPage.url();
  console.log(`[U17] Post-login URL: ${postLoginUrl}`);

  if (!postLoginUrl.includes('/umbraco') || postLoginUrl.includes('login')) {
    console.warn('[U17] Login may have failed. Continuing anyway...');
  }

  await loginPage.close();

  // ── Settings section plugins ──
  const u17SettingsPlugins = [
    { name: 'CopyValue', pathname: 'copy-value' },
    { name: 'Exif', pathname: 'exif-viewer' },
    { name: 'Forums', pathname: 'forums' },
    { name: 'LazyLoad', pathname: 'lazy-load' },
    { name: 'PhotoGallery', pathname: 'photo-gallery' },
    { name: 'PdfCurator', pathname: 'pdf-curator' },
    { name: 'Slider', pathname: 'slider' },
  ];

  console.log('\n[U17] Settings section plugins...');
  for (const { name, pathname } of u17SettingsPlugins) {
    const pluginDir = join(U17_E2E, name);
    const page = await ctx.newPage();
    const errors = [];
    page.on('pageerror', e => errors.push(e.message));

    try {
      // Try the U17 section routing - multiple possible URL patterns
      const urls = [
        `${U17_BASE}/umbraco/section/settings/dashboard/${pathname}`,
        `${U17_BASE}/umbraco/section/settings/workspace/${pathname}`,
        `${U17_BASE}/umbraco/section/Umb.Section.Settings/dashboard/${pathname}`,
      ];

      let navigated = false;
      for (const url of urls) {
        try {
          const resp = await page.goto(url, { timeout: 15000, waitUntil: 'domcontentloaded' });
          if (resp && resp.status() < 400) {
            await page.waitForTimeout(5000);
            navigated = true;
            console.log(`  [${name}] Navigated to: ${url} (${resp.status()})`);
            break;
          }
        } catch (_) {}
      }

      if (!navigated) {
        console.log(`  [${name}] Could not navigate, using settings overview`);
      }

      await saveScreenshot(page, pluginDir, 'dashboard.png', errors);
    } catch (e) {
      console.error(`  [${name}] Error: ${e.message}`);
      await saveScreenshot(page, pluginDir, 'dashboard.png', errors).catch(() => {});
    } finally {
      await page.close();
    }
  }

  // ── Members section plugins ──
  console.log('\n[U17] Members section plugins...');
  {
    const page = await ctx.newPage();
    const errors = [];
    page.on('pageerror', e => errors.push(e.message));
    const pluginDir = join(U17_E2E, '2fa');

    try {
      const urls = [
        `${U17_BASE}/umbraco/section/member/dashboard/twofactor`,
        `${U17_BASE}/umbraco/section/Umb.Section.Members/dashboard/twofactor`,
        `${U17_BASE}/umbraco/section/member`,
      ];

      for (const url of urls) {
        try {
          const resp = await page.goto(url, { timeout: 15000, waitUntil: 'domcontentloaded' });
          if (resp && resp.status() < 400) {
            await page.waitForTimeout(4000);
            console.log(`  [2fa] Navigated to: ${url}`);
            break;
          }
        } catch (_) {}
      }

      await saveScreenshot(page, pluginDir, 'dashboard.png', errors);
    } catch (e) {
      console.error(`  [2fa] Error: ${e.message}`);
    } finally {
      await page.close();
    }
  }

  // ── Content section plugins ──
  const u17ContentPlugins = [
    { name: 'SocialMedia.Channels', pathname: 'socialMediaChannels' },
    { name: 'SocialMedia.Login', pathname: 'socialMediaLogin' },
    { name: 'SocialMedia.Share', pathname: 'socialMediaShare' },
    { name: 'Payments.BancoInter', pathname: 'bancoInter' },
    { name: 'Payments.MercadoPago', pathname: 'mercadoPago' },
    { name: 'Payments.PagSeguro', pathname: 'pagSeguro' },
  ];

  console.log('\n[U17] Content section plugins...');
  for (const { name, pathname } of u17ContentPlugins) {
    const pluginDir = join(U17_E2E, name);
    const page = await ctx.newPage();
    const errors = [];
    page.on('pageerror', e => errors.push(e.message));

    try {
      const urls = [
        `${U17_BASE}/umbraco/section/content/dashboard/${pathname}`,
        `${U17_BASE}/umbraco/section/Umb.Section.Content/dashboard/${pathname}`,
        `${U17_BASE}/umbraco/section/content`,
      ];

      for (const url of urls) {
        try {
          const resp = await page.goto(url, { timeout: 15000, waitUntil: 'domcontentloaded' });
          if (resp && resp.status() < 400) {
            await page.waitForTimeout(5000);
            console.log(`  [${name}] URL: ${url}`);
            break;
          }
        } catch (_) {}
      }

      await saveScreenshot(page, pluginDir, 'dashboard.png', errors);
    } catch (e) {
      console.error(`  [${name}] Error: ${e.message}`);
    } finally {
      await page.close();
    }
  }

  await ctx.close();
  await browser.close();

  // ── U17 library-only plugins (no backoffice UI) ──
  const u17NoFrontend = {
    'CodeFirst': 'Code-first content type generator library, no backoffice dashboard section',
    'Core': 'Core library package, no backoffice frontend surface',
    'FormsClone': 'Forms cloning utility, no dedicated backoffice section',
    'IspServices': 'ISP services integration library, no backoffice frontend surface',
    'JsonRpc': 'JSON-RPC property editor only, no dashboard section',
    'Tests': 'Test project, no frontend surface',
    'WordsApi': 'External Words API integration library, no backoffice frontend surface',
    'Yaml': 'YAML schema library. Backoffice surfaces are Schema2Yaml and Yaml2Schema (see their e2e dirs)',
  };

  for (const [plugin, reason] of Object.entries(u17NoFrontend)) {
    writeNoFrontend(join(U17_E2E, plugin), reason);
  }
}

// ─────────────────────────────────────────────
// Main
// ─────────────────────────────────────────────
const target = process.argv[2] || 'both';

if (target === 'u13' || target === 'both') {
  await captureU13();
}

if (target === 'u17' || target === 'both') {
  await captureU17();
}

console.log('\n=== Done ===');
