# SplatDev.Umbraco.Plugins.Backups — v17 Implementation Plan

> **Architecture:** See `docs/specs/2026-05-27-backup-v17-design.md`
> **Goal:** Complete the Lit 3 Bellissima frontend for the Umbraco 17 Backup plugin. The C# backend (`net10.0`) is already complete.

**Tech Stack:** .NET 10 · Umbraco 17.x · Lit 3 · TypeScript · Vite

---

## Phase 1 — Lit 3 Client Setup

### Task 1: Move and configure the Lit 3 client

**Current state:** A Lit 3 skeleton exists at `SplatDev.Umbraco.Plugins.BackupManager/client/src/backup-manager-dashboard.element.ts`.
**Target:** A clean client at `SplatDev.Umbraco.Plugins.Backups/client/`.

- [ ] **Step 1: Create the client folder structure**

```
SplatDev.Umbraco.Plugins.Backups/client/
├── package.json
├── tsconfig.json
├── vite.config.ts
└── src/
    ├── index.ts                           # barrel exports
    ├── backups-dashboard.element.ts
    ├── components/
    │   ├── backups-create-modal.element.ts
    │   ├── backups-history-table.element.ts
    │   └── backups-provider-badge.element.ts
    └── context/
        └── backups.context.ts
```

- [ ] **Step 2: `package.json`**

```json
{
  "name": "splatdev-umbraco-backups-client",
  "version": "3.1.0",
  "type": "module",
  "scripts": {
    "build": "vite build",
    "dev": "vite build --watch"
  },
  "devDependencies": {
    "@umbraco-cms/backoffice": "^17.0.0",
    "typescript": "^5.5.0",
    "vite": "^5.4.0"
  }
}
```

- [ ] **Step 3: `vite.config.ts`**

```typescript
import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    lib: {
      entry: 'src/index.ts',
      formats: ['es'],
      fileName: () => 'backups-dashboard.element.js',
    },
    outDir: '../App_Plugins/Backups/dist',
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
```

- [ ] **Step 4: `tsconfig.json`**

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "ES2020",
    "moduleResolution": "bundler",
    "experimentalDecorators": true,
    "useDefineForClassFields": false,
    "strict": true,
    "lib": ["ES2020", "dom"]
  },
  "include": ["src"]
}
```

---

## Phase 2 — Context and API Client

### Task 2: Implement `backups.context.ts`

**File:** `client/src/context/backups.context.ts`

- [ ] **Step 1: Write the context**

```typescript
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export interface BackupHistoryEntry {
  date: string;
  size: string;
  status: 'Success' | 'Failed' | 'Running';
  provider: string;
  filename: string;
}

export interface CloudProviderInfo {
  id: string;
  name: string;
}

export interface CreateBackupOptions {
  includeFiles: boolean;
  includeDatabase: boolean;
  provider: string;
}

export const BACKUPS_CONTEXT_TOKEN = new UmbContextToken<BackupsContext>('SplatDev.Backups.Context');

export class BackupsContext extends UmbContextBase {
  #baseUrl = '/umbraco/api/Backups';

  constructor(host: UmbControllerHost) {
    super(host, BACKUPS_CONTEXT_TOKEN);
  }

  async getHistory(): Promise<BackupHistoryEntry[]> {
    const resp = await fetch(`${this.#baseUrl}/history`);
    if (!resp.ok) throw new Error('Failed to load backup history');
    return resp.json();
  }

  async getProviders(): Promise<CloudProviderInfo[]> {
    const resp = await fetch(`${this.#baseUrl}/providers`);
    if (!resp.ok) throw new Error('Failed to load providers');
    return resp.json();
  }

  async createBackup(opts: CreateBackupOptions): Promise<void> {
    const resp = await fetch(`${this.#baseUrl}/create`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(opts),
    });
    if (!resp.ok) throw new Error('Failed to create backup');
  }

  async deleteBackup(filename: string): Promise<void> {
    const resp = await fetch(`${this.#baseUrl}/delete/${encodeURIComponent(filename)}`, { method: 'DELETE' });
    if (!resp.ok) throw new Error('Failed to delete backup');
  }

  getDownloadUrl(filename: string): string {
    return `${this.#baseUrl}/download/${encodeURIComponent(filename)}`;
  }
}
```

---

## Phase 3 — Lit 3 Components

### Task 3: Implement `backups-provider-badge.element.ts`

- [ ] **Step 1: Write the badge component**

```typescript
import { LitElement, html, css } from '@umbraco-cms/backoffice/external/lit';
import { customElement, property } from '@umbraco-cms/backoffice/external/lit';

const PROVIDER_INITIALS: Record<string, { initials: string; color: string }> = {
  local:       { initials: 'FS', color: '#64748b' },
  azure:       { initials: 'AZ', color: '#0078d4' },
  googledrive: { initials: 'GD', color: '#4285f4' },
  dropbox:     { initials: 'DB', color: '#0061ff' },
  box:         { initials: 'BX', color: '#0061d5' },
  onedrive:    { initials: 'OD', color: '#0f3cc9' },
  mega:        { initials: 'MG', color: '#d9272e' },
  seafile:     { initials: 'SF', color: '#0d8f75' },
  s3:          { initials: 'S3', color: '#ff9900' },
  sftp:        { initials: 'FTP', color: '#4a4a4a' },
};

@customElement('backups-provider-badge')
export class BackupsProviderBadgeElement extends LitElement {
  static override styles = css`
    :host { display: inline-flex; align-items: center; gap: 4px; }
    .badge {
      display: inline-flex; align-items: center; justify-content: center;
      width: 28px; height: 28px; border-radius: 4px;
      color: #fff; font-size: 0.6rem; font-weight: 700; letter-spacing: 0.5px;
    }
  `;

  @property({ type: String }) provider = 'local';

  override render() {
    const info = PROVIDER_INITIALS[this.provider] ?? { initials: '?', color: '#999' };
    return html`<span class="badge" style="background:${info.color}">${info.initials}</span>`;
  }
}
```

---

### Task 4: Implement `backups-history-table.element.ts`

- [ ] **Step 1: Write the history table component**

Key UUI components: `uui-table`, `uui-table-head`, `uui-table-row`, `uui-table-cell`, `uui-badge`, `uui-button`, `uui-loader-bar`.

State: `@state() entries: BackupHistoryEntry[] = []`, `@state() loading = false`.

```typescript
// Responsibilities:
// - Consume BACKUPS_CONTEXT_TOKEN via UmbContextConsumerController
// - On connectedCallback: load history
// - Emit 'backup-deleted' and 'backup-downloaded' events
// - Render UUI table with columns: Date, Provider, Size, Status, Actions
// - Badge colors: Success=positive, Failed=danger, Running=warning
// - Download: window.location.href = context.getDownloadUrl(filename)
// - Delete: UUI confirm → context.deleteBackup → emit 'backup-deleted'
```

---

### Task 5: Implement `backups-create-modal.element.ts`

- [ ] **Step 1: Write the create modal component**

Uses `UmbModalBaseElement` (or a simple `uui-dialog` overlay triggered from the parent dashboard).

Fields:
- `uui-toggle` — Include Files (default: on)
- `uui-toggle` — Include Database (default: on)
- `uui-select` — Storage Destination (populated from context)
- `uui-button` — "Run Backup Now" (disabled while creating)
- `uui-button` variant="secondary" — Cancel

On submit: `context.createBackup(opts)` → emit `'backup-created'` to parent.

---

### Task 6: Implement `backups-dashboard.element.ts` (root)

- [ ] **Step 1: Write root dashboard component**

```typescript
// Responsibilities:
// - Provide BackupsContext
// - Listen for 'backup-created' from modal → reload history table
// - Listen for 'backup-deleted' from table → show success notification
// - Poll context.getHistory() every 10s while any entry has status 'Running'
// - Show uui-loader-bar while loading
// - Render: header + "Create Backup" button, backups-create-modal, backups-history-table
```

- [ ] **Step 2: Build the Lit 3 assets**

```bash
cd SplatDev.Umbraco.Plugins.Backups/client
npm ci
npm run build
```

Expected: `App_Plugins/Backups/dist/backups-dashboard.element.js` created.

- [ ] **Step 3: Verify in Umbraco 17 dev site**

Open Umbraco 17 backoffice → Settings → Backups:
- Dashboard element loads (no 404 for dist JS)
- UUI components render correctly
- Create backup flow works end-to-end

---

## Phase 4 — Tests

### Task 7: Playwright E2E tests for Umbraco 17

**File:** `test-environments/Umbraco.Cms.Test.Plugins.Backups/Tests/BackupDashboardV17Tests.cs`

- [ ] **Step 1: Write 3 E2E tests**

```csharp
[Fact]
public async Task V17_Dashboard_Loads_WithEmptyState() { /* ... */ }

[Fact]
public async Task V17_CreateBackup_LocalProvider_AppearsInHistory() { /* ... */ }

[Fact]
public async Task V17_DeleteBackup_RemovesFromHistory() { /* ... */ }
```

- [ ] **Step 2: Run tests**

```bash
dotnet test test-environments/Umbraco.Cms.Test.Plugins.Backups/
```

---

## Phase 5 — CI/CD Integration

### Task 8: Update GitHub Actions workflow

**File:** `.github/workflows/build.yml`

- [ ] **Step 1: Add npm build step before dotnet build**

```yaml
- name: Build Backups Lit 3 client
  working-directory: SplatDev.Umbraco.Plugins.Backups/client
  run: |
    npm ci
    npm run build
```

---

## Acceptance Criteria

- [ ] Umbraco 17 backoffice → Settings → Backups tab loads without console errors.
- [ ] All UUI components render correctly in the Bellissima shell.
- [ ] Create Backup flow works for local provider.
- [ ] History table shows entries with correct status badges and provider icons.
- [ ] Download link works for local backups.
- [ ] Delete removes entry from history.
- [ ] `npm run build` completes with 0 TypeScript errors.
- [ ] `dotnet build` with `net10.0` target produces 0 warnings, 0 errors.
- [ ] 3 Playwright E2E tests pass against a real Umbraco 17 instance.
