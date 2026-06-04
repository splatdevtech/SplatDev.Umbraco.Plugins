/**
 * SPL-1926: Capture remaining missing U13 plugin screenshots
 * Plugins: BackupManager(settings), Payments.BancoInter/MercadoPago/PagSeguro(content), PdfCurator(settings), Core(settings)
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
        name: parts[5].trim(),
        value: parts[6].trim(),
        domain: 'localhost',
        path: parts[2].trim(),
        httpOnly: l.startsWith('#HttpOnly_'),
        secure: parts[3] === 'TRUE',
        sameSite: 'Lax',
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
  if (!existsSync(errLog)) {
    writeFileSync(errLog, errors.length > 0 ? `Console errors:\n${errors.join('\n')}` : 'No console errors detected');
  }
  return size;
}

// Login via curl
console.log('[U13] Logging in...');
let cookies = [];
try {
  const loginResult = execSync(
    `curl -s -X POST "${U13_BASE}/umbraco/backoffice/UmbracoApi/Authentication/PostLogin" ` +
    `-H "Content-Type: application/json" -c /tmp/u13-missing-cookies.txt ` +
    `-d '{"username":"admin@splatdev.com","password":"SplatDev123!"}'`,
    { encoding: 'utf-8' }
  );
  if (loginResult.includes('emailHash') || loginResult.includes('userId')) {
    const cookieText = execSync('cat /tmp/u13-missing-cookies.txt', { encoding: 'utf-8' });
    cookies = parseCookieFile(cookieText);
    console.log('  cookies:', cookies.map(c => c.name));
  } else {
    console.error('  Login failed:', loginResult.substring(0, 100));
  }
} catch (e) {
  console.error('  Login error:', e.message);
}

const browser = await chromium.launch({ executablePath: '/usr/bin/chromium-browser', args: BROWSER_ARGS });
const ctx = await browser.newContext({ viewport: { width: 1280, height: 900 } });
if (cookies.length > 0) await ctx.addCookies(cookies);
const page = await ctx.newPage();

const errors = [];
page.on('pageerror', e => errors.push(e.message.substring(0, 150)));
page.on('console', m => { if (m.type() === 'error') errors.push(m.text().substring(0, 150)); });

// Settings section plugins
const settingsPlugins = [
  { name: 'BackupManager', hash: 'Backups' },    // tab: Backups in settings
  { name: 'Core', hash: 'Core' },
  { name: 'PdfCurator', hash: 'PdfCurator' },
];

console.log('[U13] Settings section plugins...');
for (const { name, hash } of settingsPlugins) {
  const pluginDir = join(U13_E2E, name);
  if (statSync(join(pluginDir, 'dashboard.png')).size > 10000) {
    console.log(`  [${name}] already captured, skipping`);
    continue;
  }
  try {
    await page.goto(`${U13_BASE}/umbraco#/settings`, { timeout: 30000, waitUntil: 'networkidle' });
    await page.waitForTimeout(4000);
    console.log(`  [${name}] settings URL: ${page.url()}`);
    await saveShot(page, pluginDir, [...errors]);
  } catch (e) {
    console.error(`  [${name}] Error: ${e.message}`);
    await saveShot(page, pluginDir, [...errors]);
  }
}

// Content section plugins
const contentPlugins = [
  { name: 'Payments.BancoInter' },
  { name: 'Payments.MercadoPago' },
  { name: 'Payments.PagSeguro' },
];

console.log('[U13] Content section plugins...');
for (const { name } of contentPlugins) {
  const pluginDir = join(U13_E2E, name);
  const existing = statSync(join(pluginDir, 'dashboard.png')).size;
  if (existing > 10000) {
    console.log(`  [${name}] already captured (${existing}B), skipping`);
    continue;
  }
  try {
    await page.goto(`${U13_BASE}/umbraco#/content`, { timeout: 30000, waitUntil: 'networkidle' });
    await page.waitForTimeout(5000);
    console.log(`  [${name}] content URL: ${page.url()}`);
    await saveShot(page, pluginDir, [...errors]);
  } catch (e) {
    console.error(`  [${name}] Error: ${e.message}`);
    await saveShot(page, pluginDir, [...errors]);
  }
}

await page.close();
await ctx.close();
await browser.close();
console.log('=== Done ===');
