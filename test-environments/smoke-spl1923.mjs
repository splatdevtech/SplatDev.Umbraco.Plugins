/**
 * SPL-1923 smoke test: first-boot config + runtime screenshot capture
 * Captures install page, login page, and dashboard for U13 and U17.
 */
import { chromium } from '../node_modules/playwright/index.mjs';
import { mkdirSync } from 'fs';
import { join } from 'path';

const SCREENSHOTS_DIR = './screenshots/spl1923';
mkdirSync(SCREENSHOTS_DIR, { recursive: true });

const BROWSER_ARGS = ['--no-sandbox', '--disable-setuid-sandbox', '--disable-gpu', '--disable-dev-shm-usage'];
const CREDS = { email: 'admin@splatdev.com', password: 'SplatDev123!' };

async function shot(page, name) {
  const file = join(SCREENSHOTS_DIR, name);
  await page.screenshot({ path: file, fullPage: true });
  console.log(`  -> ${file} (${page.url()})`);
}

async function captureU13(browser) {
  console.log('\n=== Umbraco 13 (port 5001) ===');
  const base = 'http://localhost:5001';

  // Root
  {
    const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
    await page.goto(base + '/', { timeout: 20000, waitUntil: 'domcontentloaded' });
    await page.waitForTimeout(1500);
    await shot(page, 'u13-01-root.png');
    await page.close();
  }

  // Install page (expect 404 because unattended install already ran)
  {
    const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
    try {
      const resp = await page.goto(base + '/umbraco/install', { timeout: 10000, waitUntil: 'domcontentloaded' });
      await page.waitForTimeout(1000);
      await shot(page, 'u13-02-install.png');
      console.log(`  install page status: ${resp?.status()} (404 expected - unattended install done)`);
    } catch(e) {
      console.log(`  install page error (expected): ${e.message}`);
    }
    await page.close();
  }

  // Login + Dashboard
  {
    const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
    await page.goto(base + '/umbraco', { timeout: 30000, waitUntil: 'networkidle' });
    await page.waitForTimeout(2000);
    await shot(page, 'u13-03-login.png');

    try {
      // U13 Angular login form
      await page.waitForSelector('input[name="username"], input[type="email"]', { timeout: 10000 });
      const emailInput = page.locator('input[name="username"], input[type="email"]').first();
      const pwInput = page.locator('input[type="password"]').first();
      await emailInput.fill(CREDS.email);
      await pwInput.fill(CREDS.password);
      await shot(page, 'u13-04-login-filled.png');
      await page.keyboard.press('Enter');
      await page.waitForNavigation({ timeout: 30000, waitUntil: 'networkidle' }).catch(() => {});
      await page.waitForTimeout(3000);
      await shot(page, 'u13-05-dashboard.png');
      console.log(`  dashboard URL: ${page.url()}`);
    } catch(e) {
      console.warn(`  login error: ${e.message}`);
      await shot(page, 'u13-04-login-error.png');
    }
    await page.close();
  }
}

async function captureU17(browser) {
  console.log('\n=== Umbraco 17 (port 5000) ===');
  const base = 'http://localhost:5000';

  // Root
  {
    const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
    await page.goto(base + '/', { timeout: 20000, waitUntil: 'domcontentloaded' });
    await page.waitForTimeout(1500);
    await shot(page, 'u17-01-root.png');
    await page.close();
  }

  // Install page (expect 404)
  {
    const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
    try {
      const resp = await page.goto(base + '/umbraco/install', { timeout: 10000, waitUntil: 'domcontentloaded' });
      await page.waitForTimeout(1000);
      await shot(page, 'u17-02-install.png');
      console.log(`  install page status: ${resp?.status()} (404 expected)`);
    } catch(e) {
      console.log(`  install page error (expected): ${e.message}`);
    }
    await page.close();
  }

  // Login + Dashboard - U17 uses Web Components (Lit)
  {
    const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
    page.on('console', msg => { if (msg.type() === 'error') console.log('  [console error]', msg.text().substring(0, 100)); });

    // Navigate to backoffice
    await page.goto(base + '/umbraco', { timeout: 30000, waitUntil: 'domcontentloaded' });

    // Wait for Web Components / custom elements to render (U17 new backoffice)
    // The login page uses umb-app-auth or similar custom elements
    console.log('  waiting for U17 backoffice UI...');
    try {
      // Wait for any custom element indicating the UI has loaded
      await page.waitForFunction(() => {
        return document.querySelector('umb-app, umb-app-auth, umb-login, [class*="uui-"], [class*="umb-"]') !== null
          || document.querySelector('input[type="email"], input[type="password"]') !== null;
      }, { timeout: 20000 });
    } catch(e) {
      console.log('  timeout waiting for UI elements, taking screenshot anyway');
    }
    await page.waitForTimeout(2000);
    await shot(page, 'u17-03-login.png');

    // Log body for debugging
    const bodyHTML = await page.evaluate(() => document.body.innerHTML.substring(0, 500));
    console.log('  body excerpt:', bodyHTML.replace(/\n/g, ' ').substring(0, 200));

    // Try to login
    try {
      // Try shadow DOM piercing with U17's Lit components
      // Method 1: direct input selectors
      let emailInput = null, pwInput = null;

      // Try regular selectors first
      const directEmail = await page.$('input[type="email"], input[autocomplete="username"]');
      const directPw = await page.$('input[type="password"]');

      if (directEmail && directPw) {
        emailInput = directEmail;
        pwInput = directPw;
        console.log('  found inputs via direct selector');
      } else {
        // Try evaluating inside shadow DOM
        const found = await page.evaluate(() => {
          function findInputsInShadow(root) {
            const inputs = root.querySelectorAll('input[type="email"], input[autocomplete="username"], input[type="password"]');
            if (inputs.length >= 2) return true;
            for (const el of root.querySelectorAll('*')) {
              if (el.shadowRoot && findInputsInShadow(el.shadowRoot)) return true;
            }
            return false;
          }
          return findInputsInShadow(document);
        });
        console.log('  shadow DOM inputs found:', found);
      }

      if (emailInput && pwInput) {
        await emailInput.fill(CREDS.email);
        await pwInput.fill(CREDS.password);
        await shot(page, 'u17-04-login-filled.png');
        await page.keyboard.press('Enter');
        await page.waitForTimeout(5000);
        await shot(page, 'u17-05-dashboard-attempt.png');
        console.log(`  post-login URL: ${page.url()}`);

        // If redirected to dashboard, wait more for it to fully render
        if (page.url().includes('/umbraco') && !page.url().includes('/login')) {
          await page.waitForFunction(() => {
            return document.querySelector('umb-backoffice, umb-dashboard, [class*="dashboard"]') !== null
              || document.querySelector('umb-app') !== null;
          }, { timeout: 15000 }).catch(() => {});
          await page.waitForTimeout(3000);
          await shot(page, 'u17-06-dashboard.png');
          console.log('  dashboard screenshot captured');
        }
      } else {
        console.warn('  could not find login inputs');
      }
    } catch(e) {
      console.warn('  login error:', e.message);
      await shot(page, 'u17-04-login-error.png');
    }
    await page.close();
  }
}

const browser = await chromium.launch({
  executablePath: '/usr/bin/chromium-browser',
  args: BROWSER_ARGS
});

await captureU13(browser);
await captureU17(browser);
await browser.close();

console.log('\nDone. Screenshots in ' + SCREENSHOTS_DIR);
