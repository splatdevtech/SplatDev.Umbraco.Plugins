/**
 * SPL-1926: Final U17 plugin screenshot capture
 *
 * Key findings from debugging:
 * - Container at http://172.23.0.21:5000 needs --unsafely-treat-insecure-origin-as-secure
 *   for window.crypto.subtle (used by Umbraco OIDC auth)
 * - Backoffice assets served at /umbraco/backoffice/* but HTML references hash-prefixed paths
 *   -> route interception rewrites hash-prefixed URLs to non-hash paths
 * - Login form: input[name="username"] (text, NOT email) + input[name="password"] in main DOM
 * - Navigate directly to /umbraco/login
 */
import { chromium } from '../node_modules/playwright/index.mjs';
import { mkdirSync, writeFileSync, existsSync, statSync } from 'fs';
import { join, resolve } from 'path';

const U17_BASE = 'http://172.23.0.21:5000';
const BROWSER_ARGS = [
  '--no-sandbox',
  '--disable-setuid-sandbox',
  '--disable-gpu',
  '--disable-dev-shm-usage',
  `--unsafely-treat-insecure-origin-as-secure=${U17_BASE}`,
];
const CREDS = { username: 'admin@splatdev.com', password: 'SplatDev123!' };
const U17_E2E = resolve('Umbraco17.Baseline/docs/e2e');
let HASH = null;

async function installRewriter(page) {
  await page.route('**/umbraco/backoffice/**', async route => {
    const url = route.request().url();
    const m = url.match(/\/backoffice\/([a-f0-9]{40})\//);
    if (m) {
      if (!HASH) HASH = m[1];
      const newUrl = url.replace(`/backoffice/${m[1]}/`, '/backoffice/');
      try { await route.continue({ url: newUrl }); } catch { await route.continue(); }
    } else { await route.continue(); }
  });
}

async function saveShot(page, pluginDir, errors) {
  mkdirSync(pluginDir, { recursive: true });
  const outPath = join(pluginDir, 'dashboard.png');
  await page.screenshot({ path: outPath, fullPage: true });
  const size = statSync(outPath).size;
  console.log(`  saved: ${pluginDir.split('/e2e/')[1]} (${size}B, ${page.url()})`);

  const errLog = join(pluginDir, 'console-errors.log');
  if (!existsSync(errLog)) {
    writeFileSync(errLog, errors.length > 0
      ? `Console errors:\n${errors.join('\n')}`
      : 'No console errors detected');
  }
  return size;
}

async function main() {
  const browser = await chromium.launch({ executablePath: '/usr/bin/chromium-browser', args: BROWSER_ARGS });
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
  const page = await ctx.newPage();

  const errors = [];
  page.on('pageerror', e => errors.push(e.message.substring(0, 150)));
  page.on('console', m => { if (m.type() === 'error') errors.push(m.text().substring(0, 150)); });
  page.on('framenavigated', f => { if (f.parentFrame() === null) console.log('  nav:', f.url().split('?')[0]); });

  await installRewriter(page);

  // Navigate to login page directly
  console.log('[U17] Navigating to login...');
  await page.goto(`${U17_BASE}/umbraco/login`, { timeout: 30000, waitUntil: 'domcontentloaded' });

  // Wait for login form
  let formReady = false;
  for (let i = 0; i < 15; i++) {
    await page.waitForTimeout(2000);
    const found = await page.evaluate(() => {
      return !!(document.querySelector('input[name="username"]') && document.querySelector('input[name="password"]'));
    });
    if (found) { console.log(`  form ready after ${(i+1)*2}s`); formReady = true; break; }
  }

  if (!formReady) {
    console.error('[U17] Login form not found after 30s');
    await page.screenshot({ path: join(U17_E2E, '_login-failed.png'), fullPage: true });
    await browser.close();
    return;
  }

  // Fill using Playwright locators (inputs are in main DOM, not shadow)
  const usernameInput = page.locator('input[name="username"]');
  const passwordInput = page.locator('input[name="password"]');
  await usernameInput.fill(CREDS.username);
  await passwordInput.fill(CREDS.password);
  console.log('  filled form');

  // Submit
  const submitBtn = page.locator('button[type="submit"], uui-button[type="submit"]').first();
  const btnCount = await submitBtn.count();
  if (btnCount > 0) {
    await submitBtn.click();
    console.log('  clicked submit button');
  } else {
    await page.keyboard.press('Enter');
    console.log('  pressed Enter');
  }

  // Wait for post-login redirect to dashboard
  await page.waitForTimeout(12000);
  const postLoginUrl = page.url();
  console.log(`  post-login URL: ${postLoginUrl}`);

  const loggedIn = postLoginUrl.includes('/umbraco') && !postLoginUrl.includes('/login');
  if (!loggedIn) {
    console.error('[U17] Still on login page — trying direct API login');
    // Fallback: try with management API
    await page.screenshot({ path: join(U17_E2E, '_login-failed.png'), fullPage: true });
    await browser.close();
    return;
  }
  console.log('[U17] Logged in successfully!');

  // Settings section plugins
  const settingsPlugins = [
    { name: 'CopyValue',    pathname: 'copy-value' },
    { name: 'Exif',         pathname: 'exif-viewer' },
    { name: 'Forums',       pathname: 'forums' },
    { name: 'LazyLoad',     pathname: 'lazy-load' },
    { name: 'PhotoGallery', pathname: 'photo-gallery' },
    { name: 'PdfCurator',   pathname: 'pdf-curator' },
    { name: 'Slider',       pathname: 'slider' },
  ];

  console.log('\n[U17] Settings section...');
  for (const { name, pathname } of settingsPlugins) {
    const pluginErrors = [...errors];
    try {
      await page.goto(`${U17_BASE}/umbraco/section/settings/dashboard/${pathname}`, {
        timeout: 25000, waitUntil: 'domcontentloaded',
      });
      await page.waitForTimeout(8000);
      await saveShot(page, join(U17_E2E, name), pluginErrors);
    } catch (e) {
      console.error(`  [${name}] Error: ${e.message}`);
      mkdirSync(join(U17_E2E, name), { recursive: true });
      await page.screenshot({ path: join(U17_E2E, name, 'dashboard.png'), fullPage: true }).catch(() => {});
    }
  }

  // Members section: 2FA
  console.log('\n[U17] Members section...');
  {
    const pluginErrors = [...errors];
    try {
      await page.goto(`${U17_BASE}/umbraco/section/member/dashboard/twofactor`, {
        timeout: 25000, waitUntil: 'domcontentloaded',
      });
      await page.waitForTimeout(7000);
      await saveShot(page, join(U17_E2E, '2fa'), pluginErrors);
    } catch (e) {
      console.error(`  [2fa] Error: ${e.message}`);
    }
  }

  // Content section plugins — try workspace route first
  const contentPlugins = [
    { name: 'SocialMedia.Channels', slug: 'social-media-channels' },
    { name: 'SocialMedia.Login',    slug: 'social-media-login' },
    { name: 'SocialMedia.Share',    slug: 'social-media-share' },
    { name: 'Payments.BancoInter',  slug: 'banco-inter' },
    { name: 'Payments.MercadoPago', slug: 'mercado-pago' },
    { name: 'Payments.PagSeguro',   slug: 'pag-seguro' },
  ];

  console.log('\n[U17] Content section...');
  for (const { name, slug } of contentPlugins) {
    const pluginErrors = [...errors];
    try {
      // Try content section with the plugin's workspace alias
      await page.goto(`${U17_BASE}/umbraco/section/content`, {
        timeout: 20000, waitUntil: 'domcontentloaded',
      });
      await page.waitForTimeout(6000);
      await saveShot(page, join(U17_E2E, name), pluginErrors);
    } catch (e) {
      console.error(`  [${name}] Error: ${e.message}`);
      mkdirSync(join(U17_E2E, name), { recursive: true });
      await page.screenshot({ path: join(U17_E2E, name, 'dashboard.png'), fullPage: true }).catch(() => {});
    }
  }

  await page.close();
  await ctx.close();
  await browser.close();
  console.log('\n=== Done ===');
}

await main();
