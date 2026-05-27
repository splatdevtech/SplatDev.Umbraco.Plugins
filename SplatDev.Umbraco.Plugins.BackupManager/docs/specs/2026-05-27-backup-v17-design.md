# SplatDev.Umbraco.Plugins.Backups — v17 Design

**Status:** Draft for review
**Date:** 2026-05-27
**Target:** Umbraco 17.x · .NET 10 · Bellissima (Lit 3) backoffice

---

## 1. Goals & Non-Goals

### Goals
- Provide a **Bellissima backoffice dashboard** built with Lit 3 web components inside the Settings section.
- Reuse the **exact same C# engine and providers** from the `net8.0` (Umbraco 13) target — no duplication.
- Support all 10 cloud providers (Local, Azure Blob, Google Drive, Dropbox, Box.net, OneDrive, Mega, Seafile, Amazon S3, SFTP).
- Support **scheduled backups** via the same `BackupScheduledService`.
- Provide **backup history**, **download**, and **delete** operations from the dashboard.
- Ship as the same NuGet package (`SplatDev.Umbraco.Plugins.Backups`), targeting `net10.0`.
- Follow Umbraco 17 extension API conventions: `umbraco-package.json`, `@umbraco-cms/backoffice` Lit 3 web components.

### Non-Goals (v17)
- Restore functionality.
- Multi-site/multi-tenant.
- Encryption at rest.
- Umbraco 13 AngularJS UI (covered in `2026-05-27-backup-v13-design.md`).

---

## 2. Current Implementation State

| Layer | Status | Notes |
|---|---|---|
| C# backend (net10.0) | ✅ Complete | Multi-target in `SplatDev.Umbraco.Plugins.Backups.csproj` |
| `App_Plugins/Backups/umbraco-package.json` | ✅ Shell exists | References `dist/backups-dashboard.element.js` |
| Lit 3 element skeleton | 🔶 Partial | `BackupManager/client/src/backup-manager-dashboard.element.ts` started |
| Built dist file | ❌ Missing | No `App_Plugins/Backups/dist/backups-dashboard.element.js` |
| E2E tests | ❌ Missing | |

**Primary gap:** The Lit 3 dashboard element must be completed and built into `App_Plugins/Backups/dist/`.

> Note on folder alignment: The existing Lit skeleton is at `SplatDev.Umbraco.Plugins.BackupManager/client/`. This must be migrated to live alongside the main plugin at `SplatDev.Umbraco.Plugins.Backups/client/` with vite output pointing to `../App_Plugins/Backups/dist/`.

---

## 3. Architecture

### Solution layout (Umbraco 17 frontend addition)

```
SplatDev.Umbraco.Plugins.Backups/
├── client/                                        # NEW — Lit 3 frontend build
│   ├── package.json                               # @umbraco-cms/backoffice ^17, vite, typescript
│   ├── tsconfig.json
│   ├── vite.config.ts                             # output → ../App_Plugins/Backups/dist/
│   └── src/
│       ├── backups-dashboard.element.ts            # Main dashboard web component
│       ├── components/
│       │   ├── backups-create-modal.element.ts     # Create backup modal
│       │   ├── backups-history-table.element.ts    # History table
│       │   └── backups-provider-badge.element.ts   # Provider icon/badge
│       └── context/
│           └── backups.context.ts                  # UmbContext — wraps API calls
└── App_Plugins/Backups/
    ├── umbraco-package.json                       # Dashboard + localization extension
    └── dist/
        └── backups-dashboard.element.js           # Vite build output (ES module)
```

### Component hierarchy

```
<backups-dashboard>               # Root, registered in umbraco-package.json
├── <uui-box>                     # Header + description
├── <backups-create-modal>        # Trigger button + UUI modal
│   ├── <uui-checkbox>            # Include Files
│   ├── <uui-checkbox>            # Include Database
│   └── <uui-select>              # Cloud provider dropdown
└── <backups-history-table>       # Paged table of BackupHistoryEntry
    └── <backups-provider-badge>  # Per-row provider icon
```

---

## 4. Umbraco 17 Extension Points

The `umbraco-package.json` registers:

| Extension type | Alias | Purpose |
|---|---|---|
| `dashboard` | `SplatDev.Backups.Dashboard` | The Lit 3 dashboard in Settings section |
| `localization` | `SplatDev.Backups.Localization.En` | English strings |
| `localization` | `SplatDev.Backups.Localization.PtBr` | Portuguese (Brazil) strings |

Dashboard placement: `Umb.Section.Settings`, weight 100.

---

## 5. Lit 3 Component Spec

### `backups-dashboard.element.ts`

```typescript
// Responsibilities:
// - On connectedCallback: fetch providers list + backup history via BackupsContext
// - Render header, create-backup section, history table
// - Handle create-backup form submission (disable button, show UUI notice "running…", reload history)
// - Poll history every 10s while a backup has status "Running"
// - Handle download (GET /download/{filename} → creates <a> with object URL)
// - Handle delete (UUI confirm dialog → DELETE /delete/{filename} → reload history)
```

Must use:
- `UmbElementMixin` from `@umbraco-cms/backoffice/element-api`
- `@customElement`, `@state`, `@property` decorators from `@umbraco-cms/backoffice/external/lit`
- `uui-button`, `uui-box`, `uui-checkbox`, `uui-select`, `uui-table`, `uui-badge`, `uui-loader-bar`
- `UMB_NOTIFICATION_CONTEXT` for success/error toasts
- `UMB_MODAL_MANAGER_CONTEXT` for the create-backup confirmation modal

### `backups.context.ts`

```typescript
// UmbContext token: SplatDev.Backups.Context
// Methods:
// - getHistory(): Promise<BackupHistoryEntry[]>
// - getProviders(): Promise<CloudProviderInfo[]>
// - createBackup(opts: CreateBackupOptions): Promise<void>
// - deleteBackup(filename: string): Promise<void>
// - getDownloadUrl(filename: string): string
```

Calls the same `/umbraco/api/Backups/*` endpoints as the Umbraco 13 AngularJS controller.

---

## 6. Build Setup

`vite.config.ts`:
```typescript
import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    lib: {
      entry: 'src/backups-dashboard.element.ts',
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

`package.json` scripts:
- `build`: `vite build`
- `dev`: `vite build --watch`

CI/CD: `npm ci && npm run build` must run before `dotnet build` for the Umbraco 17 target.

---

## 7. API Compatibility

The exact same `BackupsApiController` serves both Umbraco 13 and Umbraco 17. No new endpoints needed for v17. The Lit 3 context calls the same URLs as the AngularJS controller.

---

## 8. Testing Strategy

| Type | Tool | Scope |
|---|---|---|
| Unit (TypeScript) | Vitest | Context methods, provider badge rendering |
| E2E | Playwright | Against Umbraco 17 test host: create, history, delete |
| CI gate | GitHub Actions | `npm run build` + `dotnet build` + all tests pass |

---

## 9. Localization Strings (English)

```json
{
  "section": {
    "backups_heading": "Backups",
    "backups_description": "Create and manage quick backups from the backoffice",
    "backups_create": "Create Backup",
    "backups_creating": "Creating backup…",
    "backups_delete": "Delete",
    "backups_download": "Download",
    "backups_confirm_delete": "Delete this backup? This action cannot be undone.",
    "backups_includeMedia": "Include Media Files",
    "backups_includeDatabase": "Include Database",
    "backups_cloudProvider": "Storage Destination",
    "backups_provider_local": "Local File System",
    "backups_provider_azure": "Azure Blob Storage",
    "backups_provider_googledrive": "Google Drive",
    "backups_provider_dropbox": "Dropbox",
    "backups_provider_box": "Box.net",
    "backups_provider_onedrive": "OneDrive",
    "backups_provider_mega": "Mega",
    "backups_provider_seafile": "Seafile",
    "backups_provider_s3": "Amazon S3",
    "backups_provider_sftp": "SFTP",
    "backups_status_success": "Success",
    "backups_status_failed": "Failed",
    "backups_status_running": "Running"
  }
}
```

---

## 10. Deliverables

1. `client/` folder (move/replace the BackupManager/client skeleton)
2. `client/src/backups-dashboard.element.ts` — complete Lit 3 root component
3. `client/src/components/backups-create-modal.element.ts`
4. `client/src/components/backups-history-table.element.ts`
5. `client/src/components/backups-provider-badge.element.ts`
6. `client/src/context/backups.context.ts`
7. Built `App_Plugins/Backups/dist/backups-dashboard.element.js`
8. Playwright E2E tests against Umbraco 17 test host

---

## 11. Open Questions

- Should the Lit 3 client live in `SplatDev.Umbraco.Plugins.Backups/client/` or `SplatDev.Umbraco.Plugins.BackupManager/client/`? (Recommendation: move to the plugin's own folder for clarity.)
- Portuguese (Brazil) localization strings — need input from operator.
