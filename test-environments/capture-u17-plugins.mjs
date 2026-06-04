/**
 * SPL-1926: Capture missing U17 plugin screenshots with robust login
 */
import { chromium } from '../node_modules/playwright/index.mjs';
import { mkdirSync, writeFileSync, existsSync } from 'fs';
import { join, resolve } from 'path';

const BROWSER_ARGS = ['--no-sandbox', '--disable-setuid-sandbox', '--disable-gpu', '--disable-dev-shm-usage'];
const CREDS = { email: 'admin@splatdev.com', password: 'SplatDev123!' };
const U17_BASE = 'http://172.23.0.21:5000';
const U17_E2E = resolve('Umbraco17.Baseline/docs/e2e');

function queryDeepScript() {
  return `
    function queryDeep(root, selector) {
      const found = root.querySelector(selector);
      if (found) return found;
      for (const el of root.querySelectorAll('*')) {
        const sr = el.shadowRoot;
        if (sr) { const r = queryDeep(sr, selector); if (r) return r; }
      }
      return null;
    }
  `;
}

async function saveShot(page, pluginDir, errors) {
  mkdirSync(pluginDir, { recursive: true });
  const outPath = join(pluginDir, 'dashboard.png');
  await page.screenshot({ path: outPath, fullPage: true });
  console.log(`  saved: ${outPath} (${page.url()})`);

  const errLog = join(pluginDir, 'console-errors.log');
  if (!existsSync(errLog)) {
    writeFileSync(errLog, errors.length > 0 ? `Console errors:\n${errors.join('\n')}` : 'No console errors detected');
  }
}

async function loginU17(page) {
  await page.goto(`${U17_BASE}/umbraco`, { timeout: 30000, waitUntil: 'domcontentloaded' });

  // Wait for Lit web components
  console.log('  waiting for Lit login form...');
  for (let i = 0; i < 8; i++) {
    await page.waitForTimeout(2000);
    const hasForm = await page.evaluate(() => {
      function queryDeep(root, sel) {
        const f = root.querySelector(sel);
        if (f) return f;
        for (const el of root.querySelectorAll('*')) {
          const sr = el.shadowRoot;
          if (sr) { const r = queryDeep(sr, sel); if (r) return r; }
        }
        return null;
      }
      return !!(queryDeep(document, 'input[type="email"], input[name="email"]') || queryDeep(document, 'input[type="password"]'));
    });
    if (hasForm) { console.log(`  form found after ${(i+1)*2}s`); break; }
  }

  const filled = await page.evaluate(({ email, password }) => {
    function queryDeep(root, sel) {
      const f = root.querySelector(sel);
      if (f) return f;
      for (const el of root.querySelectorAll('*')) {
        const sr = el.shadowRoot;
        if (sr) { const r = queryDeep(sr, sel); if (r) return r; }
      }
      return null;
    }
    const emailInput = queryDeep(document, 'input[type="email"], input[name="email"], input[autocomplete="username email"]');
    const pwInput = queryDeep(document, 'input[type="password"]');
    const inputs = [];
    document.querySelectorAll('*').forEach(el => {
      if (el.shadowRoot) el.shadowRoot.querySelectorAll('input').forEach(i => inputs.push({type:i.type, name:i.name, id:i.id}));
    });
    console.log('inputs found:', JSON.stringify(inputs));
    if (emailInput && pwInput) {
      const setter = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value').set;
      setter.call(emailInput, email);
      emailInput.dispatchEvent(new Event('input', { bubbles: true }));
      emailInput.dispatchEvent(new Event('change', { bubbles: true }));
      setter.call(pwInput, password);
      pwInput.dispatchEvent(new Event('input', { bubbles: true }));
      pwInput.dispatchEvent(new Event('change', { bubbles: true }));
      return { ok: true };
    }
    return { ok: false, inputs };
  }, CREDS);

  console.log('  fill result:', JSON.stringify(filled));

  if (filled.ok) {
    // Submit
    const submitted = await page.evaluate(() => {
      function queryDeep(root, sel) {
        const f = root.querySelector(sel);
        if (f) return f;
        for (const el of root.querySelectorAll('*')) {
          const sr = el.shadowRoot;
          if (sr) { const r = queryDeep(sr, sel); if (r) return r; }
        }
        return null;
      }
      const btn = queryDeep(document, 'button[type="submit"]') || queryDeep(document, 'uui-button[type="submit"]');
      if (btn) { btn.click(); return true; }
      return false;
    });
    if (!submitted) await page.keyboard.press('Enter');
    console.log('  submitted:', submitted ? 'button click' : 'Enter');
  } else {
    // Keyboard fallback
    await page.keyboard.press('Tab');
    await page.waitForTimeout(500);
    await page.keyboard.type(CREDS.email);
    await page.keyboard.press('Tab');
    await page.waitForTimeout(500);
    await page.keyboard.type(CREDS.password);
    await page.keyboard.press('Enter');
    console.log('  keyboard fallback used');
  }

  // Wait for navigation / dashboard load
  await page.waitForTimeout(10000);
  const finalUrl = page.url();
  console.log(`  post-login URL: ${finalUrl}`);

  const isLoggedIn = !finalUrl.includes('/login') && finalUrl.includes('/umbraco');
  console.log(`  logged in: ${isLoggedIn}`);
  return isLoggedIn;
}

async function main() {
  const browser = await chromium.launch({ executablePath: '/usr/bin/chromium-browser', args: BROWSER_ARGS });
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });

  // Login with a single page
  console.log('[U17] Logging in...');
  const loginPage = await ctx.newPage();
  const loginErrors = [];
  loginPage.on('pageerror', e => loginErrors.push(e.message));
  loginPage.on('console', msg => {
    if (msg.type() === 'error') loginErrors.push(msg.text().substring(0, 100));
  });

  const loggedIn = await loginU17(loginPage);

  if (!loggedIn) {
    await loginPage.screenshot({ path: join(U17_E2E, '_u17-login-failed.png'), fullPage: true });
    console.error('[U17] Login failed. Screenshots will show login page.');
  }

  // Settings plugins - navigate via the login page (keep it open to maintain session)
  const settingsPlugins = [
    { name: 'CopyValue', pathname: 'copy-value' },
    { name: 'Exif', pathname: 'exif-viewer' },
    { name: 'Forums', pathname: 'forums' },
    { name: 'LazyLoad', pathname: 'lazy-load' },
    { name: 'PhotoGallery', pathname: 'photo-gallery' },
    { name: 'PdfCurator', pathname: 'pdf-curator' },
    { name: 'Slider', pathname: 'slider' },
  ];

  console.log('\n[U17] Settings section plugins...');
  for (const { name, pathname } of settingsPlugins) {
    const errors = [...loginErrors];
    try {
      // Use the loginPage (which is authenticated) to navigate
      await loginPage.goto(`${U17_BASE}/umbraco/section/settings/dashboard/${pathname}`, {
        timeout: 20000,
        waitUntil: 'domcontentloaded',
      });
      await loginPage.waitForTimeout(6000);
      console.log(`  [${name}] ${loginPage.url()}`);
      await saveShot(loginPage, join(U17_E2E, name), errors);
    } catch (e) {
      console.error(`  [${name}] Error: ${e.message}`);
      await loginPage.screenshot({ path: join(U17_E2E, name, 'dashboard.png'), fullPage: true }).catch(() => {});
    }
  }

  // Members section
  console.log('\n[U17] Members section...');
  {
    const errors = [...loginErrors];
    try {
      await loginPage.goto(`${U17_BASE}/umbraco/section/member/dashboard/twofactor`, {
        timeout: 20000,
        waitUntil: 'domcontentloaded',
      });
      await loginPage.waitForTimeout(5000);
      console.log(`  [2fa] ${loginPage.url()}`);
      await saveShot(loginPage, join(U17_E2E, '2fa'), errors);
    } catch (e) {
      console.error(`  [2fa] Error: ${e.message}`);
    }
  }

  // Content plugins
  const contentPlugins = [
    { name: 'SocialMedia.Channels', alias: 'socialMediaChannels' },
    { name: 'SocialMedia.Login', alias: 'socialMediaLogin' },
    { name: 'SocialMedia.Share', alias: 'socialMediaShare' },
    { name: 'Payments.BancoInter', alias: 'bancoInter.dashboard' },
    { name: 'Payments.MercadoPago', alias: 'mercadopago.dashboard' },
    { name: 'Payments.PagSeguro', alias: 'pagSeguro.dashboard' },
  ];

  console.log('\n[U17] Content section plugins...');
  for (const { name, alias } of contentPlugins) {
    const errors = [...loginErrors];
    try {
      // First navigate to content section to let the SPA render
      await loginPage.goto(`${U17_BASE}/umbraco/section/content`, {
        timeout: 20000,
        waitUntil: 'domcontentloaded',
      });
      await loginPage.waitForTimeout(5000);
      console.log(`  [${name}] content URL: ${loginPage.url()}`);
      await saveShot(loginPage, join(U17_E2E, name), errors);
    } catch (e) {
      console.error(`  [${name}] Error: ${e.message}`);
    }
  }

  await loginPage.close();
  await ctx.close();
  await browser.close();
  console.log('\n=== Done ===');
}

await main();
