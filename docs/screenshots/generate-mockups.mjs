#!/usr/bin/env node
/**
 * Generates professional SVG mockup screenshots for plugin documentation.
 * Run: node docs/screenshots/generate-mockups.mjs
 */
import { writeFileSync, mkdirSync } from 'fs';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));

const PLUGINS = [
  {
    id: 'blog',
    name: 'Blog',
    section: 'Content',
    icon: 'icon-edit',
    desc: 'Full-featured blog engine',
    panels: [
      { type: 'table', title: 'Blog Posts', cols: ['Title', 'Author', 'Category', 'Published', 'Views'], rows: [
        ['Getting Started with Umbraco', 'admin', 'Tutorials', '2024-01-15', '1,247'],
        ['Building Custom Plugins', 'editor', 'Development', '2024-01-12', '892'],
        ['Best Practices for CMS Content', 'admin', 'Tips', '2024-01-10', '654'],
        ['Umbraco 13 New Features', 'admin', 'News', '2024-01-08', '1,531'],
      ]},
    ],
    hasActions: ['New Post', 'Categories', 'Tags'],
  },
  {
    id: 'backups',
    name: 'Backups',
    section: 'Settings',
    icon: 'icon-box',
    desc: 'Cloud backup management',
    panels: [
      { type: 'cards', title: 'Storage Providers', items: [
        { name: 'Azure Blob', status: 'Connected', color: '#0078D4' },
        { name: 'AWS S3', status: 'Connected', color: '#FF9900' },
        { name: 'Google Drive', status: 'Not configured', color: '#4285F4' },
        { name: 'Local Storage', status: 'Active', color: '#28A745' },
      ]},
      { type: 'table', title: 'Recent Backups', cols: ['Date', 'Provider', 'Size', 'Status'], rows: [
        ['2024-01-15 03:00', 'Azure Blob', '245 MB', 'Completed'],
        ['2024-01-14 03:00', 'AWS S3', '243 MB', 'Completed'],
        ['2024-01-13 03:00', 'Azure Blob', '241 MB', 'Completed'],
      ]},
    ],
    hasActions: ['Create Backup', 'Schedule', 'Restore'],
  },
  {
    id: 'cache-manager',
    name: 'Cache Manager',
    section: 'Settings',
    icon: 'icon-server',
    desc: 'Cache management and warming',
    panels: [
      { type: 'stats', items: [
        { label: 'Cache Entries', value: '2,847', color: '#4ECDC4' },
        { label: 'Hit Rate', value: '94.2%', color: '#28A745' },
        { label: 'Memory Used', value: '128 MB', color: '#FF6B6B' },
        { label: 'Last Warmed', value: '5m ago', color: '#6C757D' },
      ]},
      { type: 'table', title: 'Cache Regions', cols: ['Region', 'Entries', 'Hit Rate', 'Size'], rows: [
        ['Content Cache', '1,245', '96.8%', '64 MB'],
        ['Media Cache', '892', '91.4%', '45 MB'],
        ['Template Cache', '421', '98.1%', '12 MB'],
        ['Runtime Cache', '289', '89.3%', '7 MB'],
      ]},
    ],
    hasActions: ['Clear All', 'Warm Cache', 'Settings'],
  },
  {
    id: 'analytics',
    name: 'Analytics',
    section: 'Analytics',
    icon: 'icon-bar-chart',
    desc: 'Site analytics dashboard',
    panels: [
      { type: 'stats', items: [
        { label: 'Page Views', value: '45,892', color: '#4ECDC4' },
        { label: 'Unique Visitors', value: '12,456', color: '#FF6B6B' },
        { label: 'Avg. Duration', value: '2m 34s', color: '#FFD700' },
        { label: 'Bounce Rate', value: '34.2%', color: '#6C757D' },
      ]},
      { type: 'chart', title: 'Page Views (Last 7 Days)' },
    ],
    hasActions: ['Export', 'Date Range'],
  },
  {
    id: 'newsletters',
    name: 'Newsletters',
    section: 'Content',
    icon: 'icon-mail',
    desc: 'Newsletter subscription management',
    panels: [
      { type: 'stats', items: [
        { label: 'Subscribers', value: '3,456', color: '#4ECDC4' },
        { label: 'Campaigns', value: '24', color: '#FF6B6B' },
        { label: 'Open Rate', value: '42.1%', color: '#28A745' },
        { label: 'Click Rate', value: '12.8%', color: '#FFD700' },
      ]},
      { type: 'table', title: 'Recent Campaigns', cols: ['Campaign', 'Sent', 'Opens', 'Clicks', 'Status'], rows: [
        ['January Newsletter', '3,200', '1,347', '410', 'Sent'],
        ['Holiday Special', '3,150', '1,890', '623', 'Sent'],
        ['Product Update', '3,100', '1,205', '389', 'Sent'],
      ]},
    ],
    hasActions: ['New Campaign', 'Subscribers', 'Templates'],
  },
  {
    id: 'surveys',
    name: 'Surveys',
    section: 'Content',
    icon: 'icon-poll',
    desc: 'Survey and form builder',
    panels: [
      { type: 'table', title: 'Active Surveys', cols: ['Survey', 'Responses', 'Created', 'Status'], rows: [
        ['Customer Satisfaction', '234', '2024-01-10', 'Active'],
        ['Product Feedback', '156', '2024-01-05', 'Active'],
        ['Website Usability', '89', '2023-12-20', 'Closed'],
        ['Event Registration', '412', '2023-12-15', 'Active'],
      ]},
    ],
    hasActions: ['New Survey', 'Results', 'Export'],
  },
  {
    id: 'custom-login',
    name: 'Custom Login',
    section: 'Settings',
    icon: 'icon-lock',
    desc: 'Custom login page configuration',
    panels: [
      { type: 'form', title: 'Login Page Settings', fields: [
        { label: 'Background Image', value: '/media/login-bg.jpg' },
        { label: 'Logo URL', value: '/media/logo.png' },
        { label: 'Title', value: 'Welcome to Our Portal' },
        { label: 'Enable Social Login', value: 'Yes' },
        { label: 'Remember Me', value: 'Enabled' },
      ]},
    ],
    hasActions: ['Preview', 'Save'],
  },
  {
    id: 'faqs',
    name: 'FAQs',
    section: 'Content',
    icon: 'icon-help',
    desc: 'FAQ content management',
    panels: [
      { type: 'table', title: 'FAQ Categories', cols: ['Category', 'Questions', 'Published', 'Order'], rows: [
        ['General', '12', 'Yes', '1'],
        ['Account', '8', 'Yes', '2'],
        ['Billing', '6', 'Yes', '3'],
        ['Technical Support', '15', 'Yes', '4'],
      ]},
    ],
    hasActions: ['New Category', 'New Question', 'Reorder'],
  },
  {
    id: 'forums',
    name: 'Forums',
    section: 'Forums',
    icon: 'icon-chat',
    desc: 'Discussion forum plugin',
    panels: [
      { type: 'stats', items: [
        { label: 'Topics', value: '1,234', color: '#4ECDC4' },
        { label: 'Posts', value: '8,567', color: '#FF6B6B' },
        { label: 'Members', value: '456', color: '#28A745' },
        { label: 'Online', value: '23', color: '#FFD700' },
      ]},
      { type: 'table', title: 'Recent Topics', cols: ['Topic', 'Author', 'Replies', 'Last Post'], rows: [
        ['How to customize themes?', 'john_doe', '12', '2h ago'],
        ['Plugin compatibility Q', 'sarah_dev', '5', '4h ago'],
        ['Performance tips', 'admin', '28', '1d ago'],
      ]},
    ],
    hasActions: ['New Forum', 'Moderation', 'Settings'],
  },
  {
    id: 'star-ratings',
    name: 'Star Ratings',
    section: 'Settings',
    icon: 'icon-star',
    desc: 'Content star rating plugin',
    panels: [
      { type: 'stats', items: [
        { label: 'Total Ratings', value: '5,678', color: '#FFD700' },
        { label: 'Avg. Rating', value: '4.2/5', color: '#28A745' },
        { label: 'Rated Items', value: '234', color: '#4ECDC4' },
        { label: 'Today', value: '45', color: '#FF6B6B' },
      ]},
      { type: 'table', title: 'Top Rated Content', cols: ['Content', 'Avg Rating', 'Total', 'Page'], rows: [
        ['Getting Started Guide', '4.8', '456', '/docs/getting-started'],
        ['Video Tutorials', '4.6', '312', '/tutorials'],
        ['Product Overview', '4.5', '289', '/products'],
        ['About Us', '4.2', '156', '/about'],
      ]},
    ],
    hasActions: ['Settings', 'Export', 'Reset'],
  },
  {
    id: 'redirect-manager',
    name: 'Redirect Manager',
    section: 'Settings',
    icon: 'icon-redirect',
    desc: 'URL redirect management',
    panels: [
      { type: 'table', title: 'URL Redirects', cols: ['From URL', 'To URL', 'Type', 'Hits', 'Status'], rows: [
        ['/old-page', '/new-page', '301', '1,234', 'Active'],
        ['/blog/2023/*', '/blog/archive', '301', '567', 'Active'],
        ['/products/legacy', '/shop', '302', '234', 'Active'],
        ['/contact-us', '/contact', '301', '890', 'Active'],
      ]},
    ],
    hasActions: ['New Redirect', 'Import', 'Export'],
  },
  {
    id: 'exception-manager',
    name: 'Exception Manager',
    section: 'Settings',
    icon: 'icon-bug',
    desc: 'Exception management and logging',
    panels: [
      { type: 'stats', items: [
        { label: 'Total Errors', value: '127', color: '#FF6B6B' },
        { label: 'Last 24h', value: '3', color: '#28A745' },
        { label: 'Unresolved', value: '12', color: '#FFD700' },
        { label: 'Resolved', value: '115', color: '#4ECDC4' },
      ]},
      { type: 'table', title: 'Recent Exceptions', cols: ['Exception', 'Count', 'Last Seen', 'Status'], rows: [
        ['NullReferenceException', '5', '2h ago', 'New'],
        ['TimeoutException', '2', '1d ago', 'Investigating'],
        ['SqlException', '1', '3d ago', 'Resolved'],
      ]},
    ],
    hasActions: ['Clear All', 'Export', 'Settings'],
  },
  {
    id: 'mailer',
    name: 'Mailer',
    section: 'Settings',
    icon: 'icon-mail',
    desc: 'Email sending plugin',
    panels: [
      { type: 'form', title: 'SMTP Configuration', fields: [
        { label: 'SMTP Host', value: 'smtp.sendgrid.net' },
        { label: 'Port', value: '587' },
        { label: 'Username', value: 'apikey' },
        { label: 'From Address', value: 'noreply@example.com' },
        { label: 'Enable SSL', value: 'Yes' },
      ]},
    ],
    hasActions: ['Test Email', 'Save'],
  },
  {
    id: 'seo',
    name: 'SEO',
    section: 'Settings',
    icon: 'icon-search',
    desc: 'SEO optimization plugin',
    panels: [
      { type: 'stats', items: [
        { label: 'Pages Indexed', value: '234', color: '#28A745' },
        { label: 'Issues Found', value: '12', color: '#FF6B6B' },
        { label: 'Meta Score', value: '87/100', color: '#4ECDC4' },
        { label: 'Sitemap URLs', value: '234', color: '#FFD700' },
      ]},
      { type: 'table', title: 'SEO Issues', cols: ['Page', 'Issue', 'Priority'], rows: [
        ['/products', 'Missing meta description', 'High'],
        ['/about', 'Title too long (72 chars)', 'Medium'],
        ['/blog/post-3', 'Missing alt text on images', 'Medium'],
        ['/contact', 'No H1 tag found', 'High'],
      ]},
    ],
    hasActions: ['Run Audit', 'Sitemap', 'Robots.txt'],
  },
  {
    id: 'visitor-counter',
    name: 'Visitor Counter',
    section: 'Settings',
    icon: 'icon-bar-chart',
    desc: 'Visitor counter and analytics',
    panels: [
      { type: 'stats', items: [
        { label: 'Today', value: '1,234', color: '#4ECDC4' },
        { label: 'This Week', value: '8,567', color: '#FF6B6B' },
        { label: 'This Month', value: '34,890', color: '#28A745' },
        { label: 'Total', value: '456,789', color: '#FFD700' },
      ]},
      { type: 'chart', title: 'Visitors (Last 30 Days)' },
    ],
    hasActions: ['Export', 'Reset'],
  },
];

function escapeXml(str) {
  return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

function renderStats(items, y) {
  const W = 200;
  const gap = 16;
  const startX = 236 + gap;
  let svg = '';
  items.forEach((item, i) => {
    const x = startX + i * (W + gap);
    svg += `
    <rect x="${x}" y="${y}" width="${W}" height="72" rx="6" fill="#2B2D42" stroke="#3D3F5A" stroke-width="1"/>
    <text x="${x + W/2}" y="${y + 30}" font-family="Inter, sans-serif" font-size="22" font-weight="700" fill="${item.color}" text-anchor="middle">${escapeXml(item.value)}</text>
    <text x="${x + W/2}" y="${y + 52}" font-family="Inter, sans-serif" font-size="11" fill="#8D90A8" text-anchor="middle">${escapeXml(item.label)}</text>`;
  });
  return svg;
}

function renderTable(panel, y) {
  const startX = 252;
  const colW = (1280 - startX - 40) / panel.cols.length;
  let svg = `
    <text x="${startX}" y="${y}" font-family="Inter, sans-serif" font-size="14" font-weight="600" fill="#E0E2F0">${escapeXml(panel.title)}</text>`;

  // Header row
  y += 18;
  svg += `<rect x="${startX}" y="${y}" width="${1280 - startX - 24}" height="28" rx="4" fill="#2B2D42"/>`;
  panel.cols.forEach((col, i) => {
    svg += `<text x="${startX + 12 + i * colW}" y="${y + 18}" font-family="Inter, sans-serif" font-size="11" font-weight="600" fill="#8D90A8">${escapeXml(col)}</text>`;
  });

  // Data rows
  y += 30;
  panel.rows.forEach((row, ri) => {
    if (ri % 2 === 0) {
      svg += `<rect x="${startX}" y="${y}" width="${1280 - startX - 24}" height="26" rx="2" fill="#1E2036" opacity="0.5"/>`;
    }
    row.forEach((cell, ci) => {
      let fill = '#C0C4D8';
      if (cell === 'Completed' || cell === 'Active' || cell === 'Connected' || cell === 'Sent' || cell === 'Yes' || cell === 'Resolved') fill = '#4ECDC4';
      if (cell === 'New' || cell === 'High' || cell === 'Not configured') fill = '#FF6B6B';
      if (cell === 'Investigating' || cell === 'Medium' || cell === 'Closed') fill = '#FFD700';
      svg += `<text x="${startX + 12 + ci * colW}" y="${y + 17}" font-family="Inter, sans-serif" font-size="11" fill="${fill}">${escapeXml(cell)}</text>`;
    });
    y += 26;
  });
  return { svg, endY: y + 8 };
}

function renderForm(panel, y) {
  const startX = 252;
  let svg = `
    <text x="${startX}" y="${y}" font-family="Inter, sans-serif" font-size="14" font-weight="600" fill="#E0E2F0">${escapeXml(panel.title)}</text>`;
  y += 20;

  panel.fields.forEach((field) => {
    svg += `
    <text x="${startX}" y="${y + 14}" font-family="Inter, sans-serif" font-size="11" fill="#8D90A8">${escapeXml(field.label)}</text>
    <rect x="${startX + 160}" y="${y}" width="400" height="24" rx="4" fill="#2B2D42" stroke="#3D3F5A" stroke-width="1"/>
    <text x="${startX + 170}" y="${y + 16}" font-family="Inter, sans-serif" font-size="11" fill="#C0C4D8">${escapeXml(field.value)}</text>`;
    y += 34;
  });
  return { svg, endY: y };
}

function renderCards(panel, y) {
  const startX = 252;
  const cardW = 200;
  const gap = 16;
  let svg = `
    <text x="${startX}" y="${y}" font-family="Inter, sans-serif" font-size="14" font-weight="600" fill="#E0E2F0">${escapeXml(panel.title)}</text>`;
  y += 14;

  panel.items.forEach((item, i) => {
    const x = startX + i * (cardW + gap);
    svg += `
    <rect x="${x}" y="${y}" width="${cardW}" height="56" rx="6" fill="#2B2D42" stroke="#3D3F5A" stroke-width="1"/>
    <circle cx="${x + 20}" cy="${y + 28}" r="8" fill="${item.color}"/>
    <text x="${x + 36}" y="${y + 24}" font-family="Inter, sans-serif" font-size="12" font-weight="600" fill="#E0E2F0">${escapeXml(item.name)}</text>
    <text x="${x + 36}" y="${y + 40}" font-family="Inter, sans-serif" font-size="10" fill="${item.status === 'Connected' || item.status === 'Active' ? '#4ECDC4' : '#8D90A8'}">${escapeXml(item.status)}</text>`;
  });
  return { svg, endY: y + 70 };
}

function renderChart(panel, y) {
  const startX = 252;
  const chartW = 1280 - startX - 40;
  const chartH = 120;
  let svg = `
    <text x="${startX}" y="${y}" font-family="Inter, sans-serif" font-size="14" font-weight="600" fill="#E0E2F0">${escapeXml(panel.title)}</text>`;
  y += 14;

  svg += `<rect x="${startX}" y="${y}" width="${chartW}" height="${chartH}" rx="6" fill="#2B2D42" stroke="#3D3F5A" stroke-width="1"/>`;

  // Bar chart
  const bars = [65, 78, 45, 89, 92, 73, 85];
  const barW = (chartW - 60) / bars.length;
  const labels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  bars.forEach((val, i) => {
    const bx = startX + 30 + i * barW;
    const bh = (val / 100) * (chartH - 40);
    const by = y + chartH - 20 - bh;
    svg += `<rect x="${bx + 4}" y="${by}" width="${barW - 8}" height="${bh}" rx="3" fill="#4ECDC4" opacity="0.7"/>`;
    svg += `<text x="${bx + barW/2}" y="${y + chartH - 6}" font-family="Inter, sans-serif" font-size="9" fill="#6D7090" text-anchor="middle">${labels[i]}</text>`;
  });

  return { svg, endY: y + chartH + 12 };
}

function generateBackofficeSvg(plugin) {
  const W = 1280;
  const H = 800;

  let panelSvg = '';
  let currentY = 90;

  // Stats panels
  for (const panel of plugin.panels) {
    if (panel.type === 'stats') {
      panelSvg += renderStats(panel.items, currentY);
      currentY += 96;
    } else if (panel.type === 'cards') {
      const r = renderCards(panel, currentY);
      panelSvg += r.svg;
      currentY = r.endY + 8;
    } else if (panel.type === 'table') {
      const r = renderTable(panel, currentY);
      panelSvg += r.svg;
      currentY = r.endY + 8;
    } else if (panel.type === 'form') {
      const r = renderForm(panel, currentY);
      panelSvg += r.svg;
      currentY = r.endY + 8;
    } else if (panel.type === 'chart') {
      const r = renderChart(panel, currentY);
      panelSvg += r.svg;
      currentY = r.endY + 8;
    }
  }

  // Action buttons
  let actionsSvg = '';
  if (plugin.hasActions) {
    const actionsX = 252;
    plugin.hasActions.forEach((action, i) => {
      const bx = actionsX + i * 120;
      const isPrimary = i === 0;
      actionsSvg += `
      <rect x="${bx}" y="${currentY}" width="110" height="30" rx="4" fill="${isPrimary ? '#4ECDC4' : '#2B2D42'}" stroke="${isPrimary ? '#4ECDC4' : '#3D3F5A'}" stroke-width="1"/>
      <text x="${bx + 55}" y="${currentY + 19}" font-family="Inter, sans-serif" font-size="11" font-weight="${isPrimary ? '600' : '400'}" fill="${isPrimary ? '#1A1B2E' : '#C0C4D8'}" text-anchor="middle">${escapeXml(action)}</text>`;
    });
  }

  // Sidebar nav items
  const sidebarItems = ['Content', 'Media', 'Settings', 'Members', 'Forms', 'Packages'];
  let sidebarSvg = '';
  sidebarItems.forEach((item, i) => {
    const isActive = item === plugin.section;
    const iy = 60 + i * 38;
    if (isActive) {
      sidebarSvg += `<rect x="0" y="${iy}" width="220" height="36" rx="0" fill="#2B2D42"/>`;
      sidebarSvg += `<rect x="0" y="${iy}" width="3" height="36" fill="#4ECDC4"/>`;
    }
    sidebarSvg += `<text x="42" y="${iy + 23}" font-family="Inter, sans-serif" font-size="13" fill="${isActive ? '#E0E2F0' : '#6D7090'}">${escapeXml(item)}</text>`;
  });

  return `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${W} ${H}" width="${W}" height="${H}">
  <defs>
    <linearGradient id="sidebarBg" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" style="stop-color:#1A1B2E"/>
      <stop offset="100%" style="stop-color:#16172B"/>
    </linearGradient>
    <clipPath id="rounded"><rect width="${W}" height="${H}" rx="8"/></clipPath>
  </defs>

  <g clip-path="url(#rounded)">
    <!-- Browser chrome -->
    <rect width="${W}" height="${H}" fill="#1A1B2E"/>

    <!-- Top bar -->
    <rect width="${W}" height="48" fill="#14152A"/>
    <circle cx="24" cy="24" r="6" fill="#FF5F57"/>
    <circle cx="44" cy="24" r="6" fill="#FEBC2E"/>
    <circle cx="64" cy="24" r="6" fill="#28C840"/>
    <rect x="90" y="12" width="500" height="24" rx="12" fill="#1E2036"/>
    <text x="340" y="28" font-family="Inter, sans-serif" font-size="11" fill="#6D7090" text-anchor="middle">localhost:5001/umbraco#/${plugin.id}</text>

    <!-- Sidebar -->
    <rect x="0" y="48" width="220" height="${H - 48}" fill="url(#sidebarBg)"/>
    <rect x="0" y="48" width="220" height="1" fill="#2B2D42"/>

    <!-- Umbraco logo area -->
    <text x="24" y="36" font-family="Inter, sans-serif" font-size="13" font-weight="700" fill="#4ECDC4">Umbraco</text>

    ${sidebarSvg}

    <!-- Main content area -->
    <rect x="220" y="48" width="${W - 220}" height="${H - 48}" fill="#1E2036"/>
    <rect x="220" y="48" width="${W - 220}" height="1" fill="#2B2D42"/>

    <!-- Page header -->
    <text x="252" y="${82}" font-family="Inter, sans-serif" font-size="20" font-weight="700" fill="#E0E2F0">${escapeXml(plugin.name)}</text>
    <text x="252" y="${82 + 4}" font-family="Inter, sans-serif" font-size="0" fill="#8D90A8">${escapeXml(plugin.desc)}</text>

    <!-- Panels -->
    ${panelSvg}

    <!-- Actions -->
    ${actionsSvg}

    <!-- Watermark -->
    <text x="${W - 16}" y="${H - 10}" font-family="Inter, sans-serif" font-size="9" fill="#3D3F5A" text-anchor="end">SplatDev.Umbraco.Plugins</text>
  </g>
</svg>`;
}

// Generate all mockups
for (const plugin of PLUGINS) {
  const filename = `${plugin.id}-backoffice.svg`;
  const filepath = join(__dirname, filename);
  writeFileSync(filepath, generateBackofficeSvg(plugin));
  console.log(`Generated ${filename}`);
}

console.log(`\nDone! Generated ${PLUGINS.length} backoffice mockup screenshots.`);
