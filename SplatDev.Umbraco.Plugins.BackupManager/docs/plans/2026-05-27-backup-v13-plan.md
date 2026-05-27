# SplatDev.Umbraco.Plugins.Backups — v13 Implementation Plan

> **Architecture:** See `docs/specs/2026-05-27-backup-v13-design.md`
> **Goal:** Complete the AngularJS frontend for the Umbraco 13 Backup plugin. The C# backend is already done.

**Tech Stack:** .NET 8 · Umbraco 13.x · AngularJS (backoffice constraint)

---

## Phase 1 — AngularJS Dashboard

### Task 1: Create `backups.html` dashboard view

**File:** `App_Plugins/Backups/angular/backups.html`

- [ ] **Step 1: Create the HTML template**

```html
<div ng-controller="Backups.DashboardController as vm" class="umb-dashboard backups-dashboard">

  <!-- Header -->
  <umb-load-indicator ng-if="vm.loading"></umb-load-indicator>

  <div class="umb-dashboard-content" ng-hide="vm.loading">

    <div class="backups-header">
      <h1>Backups</h1>
      <p class="muted">Create and manage quick backups from the backoffice.</p>
      <button type="button" class="btn btn-action" ng-click="vm.showCreateForm = !vm.showCreateForm"
        ng-disabled="vm.creating">
        <i class="icon icon-cloud-upload"></i> Create Backup
      </button>
    </div>

    <!-- Create backup form -->
    <div class="backups-create-form umb-property" ng-show="vm.showCreateForm">
      <div class="umb-panel-group__group-label">New Backup</div>
      <div class="umb-el-wrap">
        <label><input type="checkbox" ng-model="vm.form.includeFiles" /> Include Files</label>
        <label><input type="checkbox" ng-model="vm.form.includeDatabase" /> Include Database</label>
        <div>
          <label for="backup-provider">Storage Destination</label>
          <select id="backup-provider" ng-model="vm.form.provider"
            ng-options="p.id as p.name for p in vm.providers">
          </select>
        </div>
        <button type="button" class="btn btn-success" ng-click="vm.createBackup()" ng-disabled="vm.creating">
          <i class="icon icon-load" ng-show="vm.creating"></i>
          <span ng-hide="vm.creating">Run Backup Now</span>
          <span ng-show="vm.creating">Creating…</span>
        </button>
        <button type="button" class="btn btn-link" ng-click="vm.showCreateForm = false">Cancel</button>
      </div>
    </div>

    <!-- History table -->
    <div class="backups-history">
      <div class="umb-panel-group__group-label">Backup History</div>
      <div ng-if="vm.history.length === 0 && !vm.creating" class="text-muted backups-empty">
        No backups yet.
      </div>
      <table class="table" ng-if="vm.history.length > 0">
        <thead>
          <tr>
            <th>Date/Time</th>
            <th>Provider</th>
            <th>Size</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr ng-repeat="entry in vm.history">
            <td>{{ entry.date | date:'medium' }}</td>
            <td>{{ entry.provider }}</td>
            <td>{{ entry.size }}</td>
            <td>
              <span class="badge"
                ng-class="{'label-success': entry.status==='Success', 'label-danger': entry.status==='Failed', 'label-warning': entry.status==='Running'}">
                {{ entry.status }}
              </span>
            </td>
            <td>
              <button class="btn btn-link" ng-if="entry.provider === 'local'"
                ng-click="vm.download(entry.filename)">
                <i class="icon icon-download-alt"></i>
              </button>
              <button class="btn btn-link text-danger" ng-click="vm.delete(entry.filename)">
                <i class="icon icon-delete"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

  </div>
</div>
```

- [ ] **Step 2: Add minimal CSS** (inline styles or a `backups.css` added to `package.manifest`)

```css
/* App_Plugins/Backups/angular/backups.css */
.backups-dashboard .backups-header { display: flex; align-items: center; gap: 12px; margin-bottom: 16px; }
.backups-dashboard .backups-header h1 { flex: 1; margin: 0; }
.backups-dashboard .backups-create-form { background: #f9f9f9; padding: 16px; border-radius: 4px; margin-bottom: 16px; }
.backups-dashboard .backups-empty { padding: 24px 0; text-align: center; }
```

- [ ] **Step 3: Update `package.manifest` to include CSS**

```json
{
  "javascript": ["/App_Plugins/Backups/angular/backups.controller.js"],
  "css": ["/App_Plugins/Backups/angular/backups.css"],
  "dashboards": [
    {
      "alias": "Backups.Dashboard",
      "view": "/App_Plugins/Backups/angular/backups.html",
      "sections": ["settings"],
      "tab": "Backups",
      "access": [{"grant": "admin"}]
    }
  ]
}
```

---

### Task 2: Create `backups.controller.js`

**File:** `App_Plugins/Backups/angular/backups.controller.js`

- [ ] **Step 1: Write the AngularJS controller**

```javascript
angular.module("umbraco").controller("Backups.DashboardController", [
    "$http", "$interval", "notificationsService",
    function ($http, $interval, notificationsService) {
        var vm = this;

        // State
        vm.loading = true;
        vm.creating = false;
        vm.showCreateForm = false;
        vm.history = [];
        vm.providers = [];
        vm.form = { includeFiles: true, includeDatabase: true, provider: "local" };

        var pollingInterval = null;

        // Init
        vm.$onInit = function () {
            loadProviders();
            loadHistory();
        };

        function loadProviders() {
            $http.get("/umbraco/api/Backups/providers").then(function (resp) {
                vm.providers = resp.data;
                if (vm.providers.length > 0 && !vm.form.provider) {
                    vm.form.provider = vm.providers[0].id;
                }
            });
        }

        function loadHistory() {
            vm.loading = true;
            $http.get("/umbraco/api/Backups/history").then(function (resp) {
                vm.history = resp.data;
                vm.loading = false;
                checkForRunning();
            }, function () {
                vm.loading = false;
                notificationsService.error("Backups", "Failed to load backup history.");
            });
        }

        function checkForRunning() {
            var hasRunning = vm.history.some(function (e) { return e.status === "Running"; });
            if (hasRunning && !pollingInterval) {
                pollingInterval = $interval(loadHistory, 10000);
            } else if (!hasRunning && pollingInterval) {
                $interval.cancel(pollingInterval);
                pollingInterval = null;
            }
        }

        vm.createBackup = function () {
            vm.creating = true;
            $http.post("/umbraco/api/Backups/create", vm.form).then(function () {
                vm.creating = false;
                vm.showCreateForm = false;
                notificationsService.success("Backups", "Backup started.");
                loadHistory();
            }, function () {
                vm.creating = false;
                notificationsService.error("Backups", "Failed to start backup.");
            });
        };

        vm.download = function (filename) {
            window.location.href = "/umbraco/api/Backups/download/" + encodeURIComponent(filename);
        };

        vm.delete = function (filename) {
            if (!confirm("Delete this backup? This action cannot be undone.")) return;
            $http.delete("/umbraco/api/Backups/delete/" + encodeURIComponent(filename)).then(function () {
                notificationsService.success("Backups", "Backup deleted.");
                loadHistory();
            }, function () {
                notificationsService.error("Backups", "Failed to delete backup.");
            });
        };

        // Cleanup interval on scope destroy
        var scope = angular.element(document).injector().get("$rootScope").$new();
        scope.$on("$destroy", function () {
            if (pollingInterval) $interval.cancel(pollingInterval);
        });
    }
]);
```

- [ ] **Step 2: Build and manually verify**

Open Umbraco 13 backoffice → Settings → Backups dashboard:
- Dashboard loads with empty history message
- Create Backup form appears when button clicked
- Providers dropdown populated
- Create backup triggers POST, success notification appears
- History updates after backup completes

---

## Phase 2 — Playwright E2E Tests

### Task 3: E2E tests for Umbraco 13 dashboard

**File:** `test-environments/Umbraco.Cms.Test.Plugins.Backups/Tests/BackupDashboardTests.cs`

- [ ] **Step 1: Write Playwright tests**

```csharp
[Fact]
public async Task Dashboard_Loads_WithEmptyState()
{
    // Login to Umbraco 13 backoffice, navigate to Settings > Backups
    // Assert: "No backups yet." message visible
}

[Fact]
public async Task CreateBackup_LocalProvider_AppearsInHistory()
{
    // Click "Create Backup", select Local, click "Run Backup Now"
    // Wait for status = Success
    // Assert: 1 entry in history table with status badge "Success"
}

[Fact]
public async Task DeleteBackup_RemovesFromHistory()
{
    // Pre-condition: at least 1 backup in history
    // Click Delete on first row, confirm dialog
    // Assert: history table row count decreases by 1
}
```

- [ ] **Step 2: Run tests**

```bash
dotnet test test-environments/Umbraco.Cms.Test.Plugins.Backups/
```

Expected: 3 tests PASS.

---

## Phase 3 — NuGet Package & Documentation

### Task 4: Verify package builds and is publishable

- [ ] **Step 1: Build the NuGet package**

```bash
dotnet pack SplatDev.Umbraco.Plugins.Backups/SplatDev.Umbraco.Plugins.Backups.csproj \
  -c Release -f net8.0 --output nupkg/
```

- [ ] **Step 2: Verify package contents**

Confirm `App_Plugins/Backups/angular/backups.html` and `backups.controller.js` are embedded.

### Task 5: Update README

- [ ] Update `SplatDev.Umbraco.Plugins.BackupManager/README.md` and `SplatDev.Umbraco.Plugins.Backups/README.md` with:
  - Installation instructions (`dotnet add package SplatDev.Umbraco.Plugins.Backups`)
  - `appsettings.json` configuration snippet
  - Screenshot of dashboard (after implementation)

---

## Acceptance Criteria

- [ ] Umbraco 13 backoffice → Settings → Backups tab is visible and loads.
- [ ] Create Backup form works for all configured providers.
- [ ] History table shows past backups with correct status badges.
- [ ] Download works for local provider backups.
- [ ] Delete removes backup from history and local storage.
- [ ] All existing unit tests pass.
- [ ] 3 Playwright E2E tests pass.
- [ ] `dotnet build` with `net8.0` target produces 0 warnings, 0 errors.
- [ ] NuGet package embeds the AngularJS assets.
