/**
 * SPL-1926: Capture remaining U17 screenshots (JsonRpc, Yaml)
 */
import { chromium } from '../node_modules/playwright/index.mjs';
import { mkdirSync, writeFileSync, statSync } from 'fs';
import { join, resolve } from 'path';

const U17_BASE = 'http://172.23.0.21:5000';
const BROWSER_ARGS = [
  '--no-sandbox', '--disable-setuid-sandbox', '--disable-gpu', '--disable-dev-shm-usage',
  `--unsafely-treat-insecure-origin-as-secure=${U17_BASE}`,
];
const CREDS = { username: 'admin@splatdev.com', password: 'SplatDev123!' };
const U17_E2E = resolve('Umbraco17.Baseline/docs/e2e');
let HASH = null;

async function saveShot(page, dir, errors = []) {
  mkdirSync(dir, { recursive: true });
  const outPath = join(dir, 'dashboard.png');
  await page.screenshot({ path: outPath, fullPage: true });
  const size = statSync(outPath).size;
  console.log(`  saved: ${dir.split('/e2e/')[1]} (${size}B, ${page.url()})`);
  writeFileSync(join(dir, 'console-errors.log'),
    errors.length > 0 ? `Console errors:\n${errors.join('\n')}` : 'No console errors detected');
  return size;
}

const browser = await chromium.launch({ executablePath: '/usr/bin/chromium-browser', args: BROWSER_ARGS });
const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
const page = await ctx.newPage();
const errors = [];
page.on('pageerror', e => errors.push(e.message.substring(0, 150)));

await page.route('**/umbraco/backoffice/**', async route => {
  const url = route.request().url();
  const m = url.match(/\/backoffice\/([a-f0-9]{40})\//);
  if (m) {
    if (!HASH) HASH = m[1];
    const newUrl = url.replace(`/backoffice/${m[1]}/`, '/backoffice/');
    try { await route.continue({ url: newUrl }); } catch { await route.continue(); }
  } else { await route.continue(); }
});

// Login
console.log('Logging in...');
await page.goto(`${U17_BASE}/umbraco/login`, { timeout: 30000, waitUntil: 'domcontentloaded' });
for (let i = 0; i < 10; i++) {
  await page.waitForTimeout(2000);
  const found = await page.evaluate(() => !!(document.querySelector('input[name="username"]') && document.querySelector('input[name="password"]')));
  if (found) break;
}
await page.locator('input[name="username"]').fill(CREDS.username);
await page.locator('input[name="password"]').fill(CREDS.password);
const submitBtn = page.locator('button[type="submit"]').first();
const cnt = await submitBtn.count();
if (cnt > 0) await submitBtn.click(); else await page.keyboard.press('Enter');
await page.waitForTimeout(12000);
console.log('post-login URL:', page.url());

// Capture JsonRpc (content section)
await page.goto(`${U17_BASE}/umbraco/section/content/dashboard/json-rpc`, { timeout: 25000, waitUntil: 'domcontentloaded' });
await page.waitForTimeout(7000);
await saveShot(page, join(U17_E2E, 'JsonRpc'), [...errors]);

// Capture Yaml (settings section)
await page.goto(`${U17_BASE}/umbraco/section/settings/dashboard/schema-export`, { timeout: 25000, waitUntil: 'domcontentloaded' });
await page.waitForTimeout(7000);
await saveShot(page, join(U17_E2E, 'Yaml'), [...errors]);

// FormsClone has backofficeEntryPoint - capture content section as evidence
await page.goto(`${U17_BASE}/umbraco/section/content`, { timeout: 20000, waitUntil: 'domcontentloaded' });
await page.waitForTimeout(5000);
await saveShot(page, join(U17_E2E, 'FormsClone'), [...errors]);

await page.close();
await ctx.close();
await browser.close();
console.log('Done.');
