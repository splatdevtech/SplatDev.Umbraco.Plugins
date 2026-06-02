#!/usr/bin/env node
/**
 * Copies captured screenshots from the Docker output into each plugin's
 * docs/screenshots/ directory, then updates the plugin README.md with
 * image references.
 *
 * Usage:
 *   node install-screenshots.mjs <screenshots-dir> <plugins-root>
 *
 * Example:
 *   node install-screenshots.mjs /output/screenshots /app/plugins
 */

import { readdirSync, copyFileSync, mkdirSync, existsSync, readFileSync, writeFileSync } from 'fs';
import { join, resolve } from 'path';

const SCREENSHOTS_DIR = resolve(process.argv[2] || '/output/screenshots');
const PLUGINS_ROOT = resolve(process.argv[3] || '/app');

if (!existsSync(SCREENSHOTS_DIR)) {
  console.error(`Screenshots directory not found: ${SCREENSHOTS_DIR}`);
  process.exit(1);
}

const files = readdirSync(SCREENSHOTS_DIR).filter(f => f.endsWith('.png'));

const pluginMap = {};

for (const file of files) {
  // filename format: <plugin-name>-<type>.png
  const match = file.match(/^(.+?)-(backoffice|frontend)\.png$/);
  if (!match) continue;
  const [, pluginName, type] = match;

  if (!pluginMap[pluginName]) {
    pluginMap[pluginName] = {};
  }
  pluginMap[pluginName][type] = file;
}

console.log(`Found ${Object.keys(pluginMap).length} plugins with screenshots\n`);

for (const [pluginName, screenshots] of Object.entries(pluginMap)) {
  const pluginDir = join(PLUGINS_ROOT, `SplatDev.Umbraco.Plugins.${pluginName}`);
  if (!existsSync(pluginDir)) {
    console.log(`⚠  Plugin directory not found: ${pluginName} (SplatDev.Umbraco.Plugins.${pluginName})`);
    continue;
  }

  const screenshotsDir = join(pluginDir, 'docs', 'screenshots');
  mkdirSync(screenshotsDir, { recursive: true });

  const readmePath = join(pluginDir, 'README.md');
  let readme = existsSync(readmePath) ? readFileSync(readmePath, 'utf-8') : '';

  // Copy screenshots
  for (const type of ['backoffice', 'frontend']) {
    if (screenshots[type]) {
      const src = join(SCREENSHOTS_DIR, screenshots[type]);
      const dest = join(screenshotsDir, screenshots[type]);
      copyFileSync(src, dest);
      console.log(`  📷 ${pluginName}/${type}`);
    }
  }

  // Add images section to README if not present
  if (!readme.includes('## Screenshots') && !readme.includes('## 📸 Screenshots')) {
    const imgSection = ['', '## 📸 Screenshots', ''];

    if (screenshots.backoffice) {
      imgSection.push(`![${pluginName} Backoffice](docs/screenshots/${screenshots.backoffice})`);
    }
    if (screenshots.frontend) {
      imgSection.push(`![${pluginName} Frontend](docs/screenshots/${screenshots.frontend})`);
    }
    imgSection.push('');

    if (screenshots.backoffice || screenshots.frontend) {
      // Add before License or at end
      if (readme.includes('## License')) {
        readme = readme.replace('## License', `${imgSection.join('\n')}\n## License`);
      } else {
        readme = readme.trimEnd() + '\n' + imgSection.join('\n');
      }
      writeFileSync(readmePath, readme);
      console.log(`  📝 ${pluginName}/README.md updated`);
    }
  } else {
    console.log(`  ⏭️  ${pluginName}/README.md already has screenshots section`);
  }
}

console.log('\nDone!');
