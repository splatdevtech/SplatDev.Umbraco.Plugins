# SplatDev.Umbraco.Plugins.Backups — v13 Design

**Status:** Draft for review
**Date:** 2026-05-27
**Target:** Umbraco 13.x · .NET 8 · AngularJS backoffice

---

## 1. Goals & Non-Goals

### Goals
- Provide a **dashboard in the Settings section** that lets editors trigger backups without server access.
- Support **files backup** (zip of `wwwroot` / `App_Data`) and **database backup** (both MSSQL and SQLite/SQLCE).
- Support **10 cloud storage destinations** selectable per backup job: Local File System, Azure Blob, Google Drive, Dropbox, Box.net, OneDrive, Mega, Seafile, Amazon S3, SFTP.
- Allow **scheduled backups** (background hosted service, configurable via `appsettings.json`).
- Show **backup history** in the dashboard (last N entries from a local log).
- Allow **download** of local backups directly from the dashboard.
- **Delete** old backups via dashboard.
- Ship as a NuGet package (`SplatDev.Umbraco.Plugins.Backups`, `Version = 3.x.y`), target `net8.0`.

### Non-Goals (v13)
- Restore functionality (restore from backup into a running site is out of scope — risk too high without an offline mode).
- Multi-site/multi-tenant isolation.
- Backup encryption at rest (cloud providers handle this).
- Umbraco 14+ / Bellissima UI (covered in `2026-05-27-backup-v17-design.md`).

---

## 2. Current Implementation State

The **C# backend** (`SplatDev.Umbraco.Plugins.Backups`, multi-target) is **complete** for net8.0:

| Layer | Folder | Status |
|---|---|---|
| Engine | `Engine/BackupEngine.cs`, `IBackupEngine.cs` | ✅ Complete |
| Cloud Providers | `Providers/` (10 providers) | ✅ Complete |
| API Controller | `Controllers/BackupsApiController.cs` | ✅ Complete |
| Scheduling | `Scheduling/` | ✅ Complete |
| Configuration | `Configuration/` | ✅ Complete |
| Composers | `Composers/` | ✅ Complete |

**Missing: AngularJS frontend.**

The `package.manifest` at `App_Plugins/Backups/package.manifest` declares:
- `javascript`: `/App_Plugins/Backups/angular/backups.controller.js` — **FILE DOES NOT EXIST**
- Dashboard view: `/App_Plugins/Backups/angular/backups.html` — **FILE DOES NOT EXIST**

These two files must be created to complete the Umbraco 13 plugin.

---

## 3. Architecture

### Backend (existing)

```
SplatDev.Umbraco.Plugins.Backups/
├── Composers/          # Umbraco composer (registers services + migration plan)
├── Configuration/      # BackupSettings (provider configs, schedule)
├── Controllers/        # BackupsApiController — REST API for the dashboard
├── Engine/             # BackupEngine — orchestrates zip + upload
├── Models/             # BackupJob, BackupHistoryEntry, CloudProviderConfig
├── Providers/          # 10 x ICloudStorageProvider impls
├── Scheduling/         # BackupScheduledService (IHostedService)
├── Services/           # IBackupHistoryService (reads/writes backup log)
└── App_Plugins/Backups/
    ├── package.manifest     # AngularJS registration
    └── umbraco-package.json # Bellissima registration (for v17)
```

### Frontend (missing — to be created)

```
App_Plugins/Backups/angular/
├── backups.html             # Dashboard view (AngularJS template)
└── backups.controller.js    # AngularJS controller + $http service
```

---

## 4. API Surface

All endpoints are on `BackupsApiController` (Umbraco `UmbracoAuthorizedApiController`), URL prefix `/umbraco/api/Backups/`.

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/history` | Returns list of `BackupHistoryEntry` (date, size, status, provider, filename) |
| `POST` | `/create` | Triggers a backup: body `{includeFiles, includeDatabase, provider}` |
| `DELETE` | `/delete/{filename}` | Deletes a local backup file |
| `GET` | `/download/{filename}` | Streams local backup file for download |
| `GET` | `/providers` | Returns list of configured providers |
| `GET` | `/config` | Returns current `BackupSettings` (sanitized, no secrets) |

---

## 5. AngularJS Dashboard Spec

### Dashboard view: `backups.html`

**Layout (3 sections):**

1. **Header** — title "Backups", description line, "Create Backup" button.

2. **Create Backup Form** (collapsible / modal):
   - Checkbox: "Include Files"
   - Checkbox: "Include Database"
   - Dropdown: "Cloud Provider" (populated from `/providers`)
   - Submit button: "Run Backup Now" (disabled while backup is running)

3. **Backup History Table**:
   - Columns: Date/Time, Size, Provider, Status (badge: Success / Failed / Running), Actions
   - Actions per row: Download (local only), Delete
   - Empty state message when no backups exist
   - Loading spinner while fetching

### Controller: `backups.controller.js`

```js
// Key behaviour
// - On load: GET /providers → populate dropdown, GET /history → populate table
// - "Run Backup Now": POST /create, disable button, show progress, reload history on complete
// - "Download": GET /download/{filename} → trigger browser download
// - "Delete": confirm dialog, DELETE /delete/{filename}, reload history
// - Poll status every 5s while a backup has status "Running"
// - Use Umbraco's $http service wrapper with bearer token (UmbracoBackofficeApiController)
```

AngularJS module: `umbraco.controllers` (standard Umbraco 13 controller namespace).
Controller name: `Backups.DashboardController`.

---

## 6. Configuration (`appsettings.json`)

```json
{
  "SplatDev": {
    "Backups": {
      "Schedule": {
        "Enabled": false,
        "CronExpression": "0 2 * * *",
        "IncludeFiles": true,
        "IncludeDatabase": true,
        "Provider": "local"
      },
      "LocalPath": "~/App_Data/Backups",
      "MaxLocalBackupsToKeep": 10,
      "Providers": {
        "Azure": { "ConnectionString": "", "ContainerName": "" },
        "GoogleDrive": { "ClientId": "", "ClientSecret": "", "FolderId": "" },
        "Dropbox": { "AccessToken": "" },
        "Box": { "ClientId": "", "ClientSecret": "" },
        "OneDrive": { "ClientId": "", "ClientSecret": "" },
        "Mega": { "Email": "", "Password": "" },
        "Seafile": { "ServerUrl": "", "Username": "", "Password": "", "LibraryId": "" },
        "S3": { "AccessKey": "", "SecretKey": "", "BucketName": "", "Region": "" },
        "Sftp": { "Host": "", "Port": 22, "Username": "", "Password": "", "RemotePath": "" }
      }
    }
  }
}
```

---

## 7. Testing Strategy

| Type | Scope | Status |
|---|---|---|
| Unit tests | BackupEngine encryption, cloud provider config | ✅ Exists (`Backups.Tests`) |
| Integration tests | Each cloud provider round-trips a small file | ❌ Missing (needs cloud credentials) |
| UI tests | AngularJS dashboard: create, history, delete | ❌ Missing |

**For v13 DoD:**
- All existing tests pass.
- Add 3 dashboard smoke tests (Playwright against a test Umbraco 13 site): create backup, verify history entry, delete backup.

---

## 8. Deliverables (v13 missing pieces)

1. `App_Plugins/Backups/angular/backups.html` — dashboard template
2. `App_Plugins/Backups/angular/backups.controller.js` — AngularJS controller
3. Playwright test: `test-environments/Umbraco.Cms.Test.Plugins.Backups/` — 3 E2E scenarios

---

## 9. Open Questions

- Should the backup history be persisted in the DB or as a JSON sidecar file? (Current assumption: sidecar JSON in `App_Data/Backups/history.json`.)
- Should the Settings section tab be admin-only or also available to editors with specific permissions?
