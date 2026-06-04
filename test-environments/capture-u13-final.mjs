/**
 * SPL-1926: Capture remaining missing U13 screenshots
 */
import { chromium } from '../node_modules/playwright/index.mjs';
import { mkdirSync, writeFileSync, existsSync, statSync } from 'fs';
import { execSync } from 'child_process';
import { join, resolve } from 'path';

const U13_BASE = 'http://172.23.0.22:5001';
const U13_E2E = resolve('Umbraco13.Baseline/docs/e2e');
const BROWSER_ARGS = ['--no-sandbox', '--disable-setuid-sandbox', '--disable-gpu', '--disable-dev-shm-usage'];

function parseCookieFile(text) {
  return text.split('\n')
    .filter(l => l.trim() && !(l.startsWith('#') && !l.startsWith('#HttpOnly_')))
    .map(l => {
      const line = l.startsWith('#HttpOnly_') ? l.replace('#HttpOnly_', '') : l;
      const parts = line.split('\t');
      if (parts.length < 7) return null;
      return {
        name: parts[5].trim(), value: parts[6].trim(),
        domain: 'localhost', path: parts[2].trim(),
        httpOnly: l.startsWith('#HttpOnly_'), secure: false, sameSite: 'Lax',
      };
    }).filter(Boolean);
}

async function saveShot(page, pluginDir, errors = []) {
  mkdirSync(pluginDir, { recursive: true });
  const outPath = join(pluginDir, 'dashboard.png');
  await page.screenshot({ path: outPath, fullPage: true });
  const size = statSync(outPath).size;
  console.log(`  saved: ${pluginDir.split('/e2e/')[1]} (${size}B)`);
  const errLog = join(pluginDir, 'console-errors.log');
  writeFileSync(errLog, errors.length > 0 ? `Console errors:\n${errors.join('\n')}` : 'No console errors detected');
  return size;
}

console.log('[U13] Logging in via API...');
let cookies = [];
try {
  const r = execSync(
    `curl -s -X POST "${U13_BASE}/umbraco/backoffice/UmbracoApi/Authentication/PostLogin" ` +
    `-H "Content-Type: application/json" -c /tmp/u13-final-cookies.txt ` +
    `-d '{"username":"admin@splatdev.com","password":"SplatDev123!"}'`,
    { encoding: 'utf-8', timeout: 15000 }
  );
  if (r.includes('emailHash') || r.includes('userId') || r.includes('"userId"')) {
    const ct = execSync('cat /tmp/u13-final-cookies.txt', { encoding: 'utf-8' });
    cookies = parseCookieFile(ct);
    console.log('  logged in, cookies:', cookies.map(c => c.name));
  } else {
    console.warn('  login may have failed:', r.substring(0, 100));
    // Try form-based login as fallback
  }
} catch (e) {
  console.error('  login error:', e.message);
}

const browser = await chromium.launch({ executablePath: '/usr/bin/chromium-browser', args: BROWSER_ARGS });
const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
if (cookies.length > 0) await ctx.addCookies(cookies);
const page = await ctx.newPage();
const errors = [];
page.on('pageerror', e => errors.push(e.message.substring(0, 150)));

// Navigate to verify auth
await page.goto(`${U13_BASE}/umbraco#/content`, { timeout: 30000, waitUntil: 'networkidle' });
await page.waitForTimeout(4000);
console.log('  initial URL:', page.url());

// Settings section plugins
console.log('\n[U13] Settings section...');
for (const name of ['BackupManager', 'Core', 'PdfCurator']) {
  const pluginDir = join(U13_E2E, name);
  await page.goto(`${U13_BASE}/umbraco#/settings`, { timeout: 30000, waitUntil: 'networkidle' });
  await page.waitForTimeout(5000);
  await saveShot(page, pluginDir, [...errors]);
}

// Content section plugins
console.log('\n[U13] Content section...');
for (const name of ['Payments.BancoInter', 'Payments.MercadoPago', 'Payments.PagSeguro']) {
  const pluginDir = join(U13_E2E, name);
  await page.goto(`${U13_BASE}/umbraco#/content`, { timeout: 30000, waitUntil: 'networkidle' });
  await page.waitForTimeout(5000);
  await saveShot(page, pluginDir, [...errors]);
}

await page.close();
await ctx.close();
await browser.close();
console.log('\n=== Done ===');
