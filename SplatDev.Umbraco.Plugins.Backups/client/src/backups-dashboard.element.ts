import {
  LitElement,
  html,
  css,
  customElement,
  state,
  nothing,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface BackupInfo {
  name: string;
  createdAt: string;
  sizeBytes: number;
  extension: string;
  isCompressed: boolean;
  isEncrypted: boolean;
}

interface CloudProviderConfig {
  id: string;
  providerType: string;
  enabled: boolean;
}

interface RestoreResult {
  success: boolean;
  restoredContentCount: number;
  restoredMediaCount: number;
  errors: string[];
}

type TabName = "backups" | "restore" | "schedule" | "providers";
type SortField = "name" | "createdAt" | "sizeBytes";

const SCOPES = [
  { value: 1, label: "Content" },
  { value: 2, label: "Media" },
  { value: 4, label: "Database" },
  { value: 3, label: "Content + Media" },
  { value: 7, label: "Full (Content + Media + Database)" },
] as const;

const CRON_PRESETS = [
  { label: "Every day at 2 AM", value: "0 2 * * *" },
  { label: "Every 6 hours", value: "0 */6 * * *" },
  { label: "Every Sunday at midnight", value: "0 0 * * 0" },
  { label: "First of month at 3 AM", value: "0 3 1 * *" },
  { label: "Custom", value: "" },
] as const;

const API = "/umbraco/api/backups";

@customElement("backups-dashboard")
export class BackupsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host {
      display: block;
      padding: var(--uui-size-layout-1);
    }

    .dashboard-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      gap: var(--uui-size-4);
      margin-bottom: var(--uui-size-layout-1);
    }

    .dashboard-header h1 {
      margin: 0 0 var(--uui-size-2) 0;
      font-size: var(--uui-size-10);
      font-weight: 700;
      color: var(--uui-color-text);
    }

    .dashboard-header p {
      margin: 0;
      color: var(--uui-color-text-alt);
      font-size: var(--uui-size-5);
    }

    .tabs {
      display: flex;
      gap: 0;
      border-bottom: 2px solid var(--uui-color-border);
      margin-bottom: var(--uui-size-layout-2);
    }

    .tab-button {
      display: inline-flex;
      align-items: center;
      gap: var(--uui-size-2);
      padding: var(--uui-size-3) var(--uui-size-5);
      font-size: var(--uui-size-5);
      font-weight: 600;
      color: var(--uui-color-text-alt);
      background: none;
      border: none;
      border-bottom: 2px solid transparent;
      margin-bottom: -2px;
      cursor: pointer;
      transition: color 0.15s, border-color 0.15s;
    }

    .tab-button:hover {
      color: var(--uui-color-interactive-emphasis);
    }

    .tab-button[aria-selected="true"] {
      color: var(--uui-color-text);
      border-bottom-color: var(--uui-color-interactive-emphasis);
    }

    .section {
      margin-bottom: var(--uui-size-layout-2);
    }

    .toolbar {
      display: flex;
      gap: var(--uui-size-4);
      align-items: center;
      margin-bottom: var(--uui-size-4);
      flex-wrap: wrap;
    }

    .toolbar uui-input {
      flex: 1;
      min-width: 200px;
    }

    uui-table {
      width: 100%;
    }

    uui-table-head-cell {
      font-weight: 600;
    }

    .sortable {
      cursor: pointer;
      user-select: none;
    }

    .sortable:hover {
      color: var(--uui-color-interactive-emphasis);
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: var(--uui-size-layout-3) var(--uui-size-layout-1);
      color: var(--uui-color-text-alt);
      gap: var(--uui-size-4);
    }

    .empty-state uui-icon {
      font-size: 3rem;
      opacity: 0.4;
    }

    .form-row {
      margin-bottom: var(--uui-size-4);
    }

    .form-row label {
      display: block;
      font-size: var(--uui-size-5);
      font-weight: 600;
      margin-bottom: var(--uui-size-2);
      color: var(--uui-color-text);
    }

    .form-row uui-input,
    .form-row uui-select,
    .form-row select {
      width: 100%;
      max-width: 400px;
    }

    .form-row select {
      padding: 8px;
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      font-size: var(--uui-size-5);
      background: var(--uui-color-surface);
    }

    .checkbox-row {
      display: flex;
      gap: var(--uui-size-6);
      flex-wrap: wrap;
      margin-bottom: var(--uui-size-4);
    }

    .checkbox-row label {
      display: inline-flex;
      align-items: center;
      gap: var(--uui-size-2);
      font-size: var(--uui-size-5);
      cursor: pointer;
    }

    .form-actions {
      display: flex;
      gap: var(--uui-size-3);
      margin-top: var(--uui-size-4);
      padding-top: var(--uui-size-4);
      border-top: 1px solid var(--uui-color-border);
    }

    .help-text {
      display: block;
      margin-top: var(--uui-size-1);
      font-size: 0.8em;
      color: var(--uui-color-text-alt);
    }

    .badge {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      padding: 2px 8px;
      border-radius: var(--uui-border-radius);
      font-size: 0.8em;
      font-weight: 600;
      white-space: nowrap;
    }

    .badge--compressed {
      background: var(--uui-color-positive-standalone);
      color: var(--uui-color-positive-contrast);
    }

    .badge--encrypted {
      background: var(--uui-color-warning-standalone);
      color: var(--uui-color-warning-contrast);
    }

    .badge--raw {
      background: var(--uui-color-border);
      color: var(--uui-color-text-alt);
    }

    .badge--success {
      background: var(--uui-color-positive-standalone);
      color: var(--uui-color-positive-contrast);
    }

    .badge--enabled {
      background: var(--uui-color-positive-standalone);
      color: var(--uui-color-positive-contrast);
    }

    .badge--disabled {
      background: var(--uui-color-border);
      color: var(--uui-color-text-alt);
    }

    .warning-box {
      display: flex;
      align-items: flex-start;
      gap: var(--uui-size-3);
      background: var(--uui-color-warning-standalone);
      border: 1px solid var(--uui-color-warning);
      border-radius: var(--uui-border-radius);
      padding: var(--uui-size-4);
      margin-bottom: var(--uui-size-layout-1);
      font-size: var(--uui-size-5);
      color: var(--uui-color-warning-contrast);
    }

    .info-box {
      display: flex;
      align-items: flex-start;
      gap: var(--uui-size-3);
      background: var(--uui-color-surface-alt);
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      padding: var(--uui-size-4);
      margin-bottom: var(--uui-size-layout-1);
      font-size: var(--uui-size-5);
      color: var(--uui-color-text-alt);
    }

    .info-box code {
      background: rgba(0, 0, 0, 0.06);
      padding: 1px 5px;
      border-radius: 3px;
      font-size: 0.9em;
    }

    .provider-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: var(--uui-size-4);
    }

    .provider-card {
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      overflow: hidden;
      background: var(--uui-color-surface);
      transition: box-shadow 0.15s;
    }

    .provider-card:hover {
      box-shadow: var(--uui-shadow-depth-1);
    }

    .provider-card__header {
      display: flex;
      align-items: center;
      gap: var(--uui-size-3);
      padding: var(--uui-size-4);
      background: var(--uui-color-surface-alt);
      border-bottom: 1px solid var(--uui-color-border);
    }

    .provider-card__body {
      padding: var(--uui-size-4);
      font-size: var(--uui-size-4);
      color: var(--uui-color-text-alt);
    }

    .provider-card__footer {
      display: flex;
      align-items: center;
      gap: var(--uui-size-3);
      padding: var(--uui-size-4);
      border-top: 1px solid var(--uui-color-border);
    }

    .test-pass {
      color: var(--uui-color-positive);
      font-size: 0.85em;
      font-weight: 600;
    }

    .test-fail {
      color: var(--uui-color-danger);
      font-size: 0.85em;
      font-weight: 600;
    }

    .summary-bar {
      display: flex;
      gap: var(--uui-size-3);
      align-items: center;
      padding: var(--uui-size-3) var(--uui-size-4);
      background: var(--uui-color-surface-alt);
      border-radius: var(--uui-border-radius);
      font-size: 0.85em;
      color: var(--uui-color-text-alt);
      margin-top: var(--uui-size-3);
    }

    .config-preview {
      margin-top: var(--uui-size-layout-1);
    }

    .config-preview pre {
      margin: 0;
      padding: var(--uui-size-4);
      background: #282c34;
      color: #abb2bf;
      font-family: "Consolas", "Monaco", monospace;
      font-size: 0.85em;
      line-height: 1.5;
      overflow-x: auto;
      white-space: pre;
      border-radius: 0 0 var(--uui-border-radius) var(--uui-border-radius);
    }

    .actions-cell {
      white-space: nowrap;
    }

    .checkbox-warning label {
      color: var(--uui-color-danger);
    }

    .provider-checkboxes {
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-2);
      padding: var(--uui-size-3);
      background: var(--uui-color-surface);
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      max-width: 400px;
    }
  `;

  @state() private _activeTab: TabName = "backups";
  @state() private _loading = false;
  @state() private _creating = false;
  @state() private _restoring = false;
  @state() private _showCreateForm = false;
  @state() private _backups: BackupInfo[] = [];
  @state() private _providers: CloudProviderConfig[] = [];
  @state() private _filter = "";
  @state() private _sortField: SortField = "createdAt";
  @state() private _sortReverse = true;

  @state() private _formName = "";
  @state() private _formScope = 7;
  @state() private _formIncludeMedia = true;
  @state() private _formCompress = true;
  @state() private _formEncrypt = false;
  @state() private _formEncryptionKey = "";
  @state() private _formCloudProviderIds: string[] = [];

  @state() private _restoreBackupName = "";
  @state() private _restoreScope = 7;
  @state() private _restoreOverwrite = false;
  @state() private _restoreDecryptionKey = "";

  @state() private _scheduleEnabled = false;
  @state() private _scheduleCron = "0 2 * * *";
  @state() private _scheduleScope = 7;
  @state() private _scheduleCompress = true;
  @state() private _scheduleKeepLocal = true;

  @state() private _providerTestResults: Record<
    string,
    { testing: boolean; valid: boolean; tested: boolean }
  > = {};

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadProviders();
    this._loadBackups();
  }

  private async _loadBackups(): Promise<void> {
    this._loading = true;
    try {
      const resp = await fetch(`${API}/GetAll`);
      if (resp.ok) {
        this._backups = (await resp.json()) as BackupInfo[];
      }
    } catch {
      // handled by empty state
    } finally {
      this._loading = false;
    }
  }

  private async _loadProviders(): Promise<void> {
    try {
      const resp = await fetch(`${API}/GetCloudProviders`);
      if (resp.ok) {
        this._providers = (await resp.json()) as CloudProviderConfig[];
      }
    } catch {
      this._providers = [];
    }
  }

  private async _createBackup(): Promise<void> {
    if (!this._formName.trim()) return;
    if (this._formEncrypt && !this._formEncryptionKey) return;

    this._creating = true;
    try {
      const resp = await fetch(`${API}/Create`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: this._formName,
          includeMedia: this._formIncludeMedia,
          scope: this._formScope,
          compress: this._formCompress,
          encrypt: this._formEncrypt,
          encryptionKey: this._formEncryptionKey,
          cloudProviderIds: this._formCloudProviderIds,
        }),
      });

      if (resp.ok) {
        this._formName = "";
        this._formEncryptionKey = "";
        this._showCreateForm = false;
        await this._loadBackups();
      }
    } finally {
      this._creating = false;
    }
  }

  private async _deleteBackup(name: string): Promise<void> {
    if (!confirm(`Delete backup '${name}'? This cannot be undone.`)) return;
    const resp = await fetch(
      `${API}/Delete?name=${encodeURIComponent(name)}`,
      { method: "DELETE" }
    );
    if (resp.ok) {
      await this._loadBackups();
    }
  }

  private async _restoreBackup(): Promise<void> {
    if (!this._restoreBackupName) return;

    let msg = `Restore backup '${this._restoreBackupName}'?`;
    if (this._restoreOverwrite) {
      msg += "\n\nWARNING: This will overwrite existing content!";
    }
    if (!confirm(msg)) return;

    this._restoring = true;
    try {
      const resp = await fetch(
        `${API}/Restore?backupPath=${encodeURIComponent(this._restoreBackupName)}`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            scope: this._restoreScope,
            overwriteExisting: this._restoreOverwrite,
            decryptionKey: this._restoreDecryptionKey,
          }),
        }
      );

      if (resp.ok) {
        const result = (await resp.json()) as RestoreResult;
        if (result.success) {
          alert(
            `Restore complete. Content: ${result.restoredContentCount}, Media: ${result.restoredMediaCount}`
          );
        } else {
          alert(`Restore completed with errors:\n${result.errors.join("\n")}`);
        }
        this._restoreBackupName = "";
        this._restoreDecryptionKey = "";
      }
    } finally {
      this._restoring = false;
    }
  }

  private async _testProvider(id: string): Promise<void> {
    this._providerTestResults = {
      ...this._providerTestResults,
      [id]: { testing: true, valid: false, tested: false },
    };
    try {
      const resp = await fetch(
        `${API}/TestProvider?providerId=${encodeURIComponent(id)}`,
        { method: "POST" }
      );
      const data = resp.ok
        ? ((await resp.json()) as { valid: boolean })
        : { valid: false };
      this._providerTestResults = {
        ...this._providerTestResults,
        [id]: { testing: false, valid: data.valid, tested: true },
      };
    } catch {
      this._providerTestResults = {
        ...this._providerTestResults,
        [id]: { testing: false, valid: false, tested: true },
      };
    }
  }

  private _toggleProviderSelection(list: string[], id: string): string[] {
    const idx = list.indexOf(id);
    if (idx > -1) {
      return [...list.slice(0, idx), ...list.slice(idx + 1)];
    }
    return [...list, id];
  }

  private _formatSize(bytes: number): string {
    if (!bytes) return "0 B";
    const units = ["B", "KB", "MB", "GB", "TB"];
    const i = Math.min(
      Math.floor(Math.log(bytes) / Math.log(1024)),
      units.length - 1
    );
    return `${(bytes / Math.pow(1024, i)).toFixed(i === 0 ? 0 : 1)} ${units[i]}`;
  }

  private _timeAgo(dateStr: string): string {
    if (!dateStr) return "";
    const diff = Math.floor((Date.now() - new Date(dateStr).getTime()) / 1000);
    if (diff < 60) return "just now";
    if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
    if (diff < 604800) return `${Math.floor(diff / 86400)}d ago`;
    return new Date(dateStr).toLocaleDateString();
  }

  private get _filteredBackups(): BackupInfo[] {
    let list = this._backups;
    if (this._filter.trim()) {
      const q = this._filter.toLowerCase();
      list = list.filter(
        (b) =>
          b.name.toLowerCase().includes(q) ||
          (b.extension && b.extension.toLowerCase().includes(q))
      );
    }
    const field = this._sortField;
    const dir = this._sortReverse ? -1 : 1;
    return [...list].sort((a, b) => {
      const aVal = a[field];
      const bVal = b[field];
      if (typeof aVal === "string" && typeof bVal === "string") {
        return aVal.localeCompare(bVal) * dir;
      }
      return ((aVal as number) - (bVal as number)) * dir;
    });
  }

  private get _totalSize(): number {
    return this._backups.reduce((sum, b) => sum + (b.sizeBytes || 0), 0);
  }

  private _isSelectedBackupEncrypted(): boolean {
    const b = this._backups.find((x) => x.name === this._restoreBackupName);
    return b?.isEncrypted ?? false;
  }

  private _toggleSort(field: SortField): void {
    if (this._sortField === field) {
      this._sortReverse = !this._sortReverse;
    } else {
      this._sortField = field;
      this._sortReverse = false;
    }
  }

  override render() {
    return html`
      <div class="dashboard-header">
        <div>
          <h1>Backups</h1>
          <p>Create, restore, and schedule backups from the backoffice.</p>
        </div>
        <uui-button
          look="primary"
          label="Create Backup"
          ?disabled=${this._creating}
          @click=${() => (this._showCreateForm = !this._showCreateForm)}
        >
          <uui-icon name="cloud-upload"></uui-icon>
          Create Backup
        </uui-button>
      </div>

      <div class="tabs" role="tablist">
        ${this._renderTabButton("backups", "Backups", "box")}
        ${this._renderTabButton("restore", "Restore", "undo")}
        ${this._renderTabButton("schedule", "Schedule", "timer")}
        ${this._renderTabButton("providers", "Cloud Providers", "cloud")}
      </div>

      ${this._showCreateForm ? this._renderCreateForm() : nothing}
      ${this._activeTab === "backups" ? this._renderBackupsTab() : nothing}
      ${this._activeTab === "restore" ? this._renderRestoreTab() : nothing}
      ${this._activeTab === "schedule" ? this._renderScheduleTab() : nothing}
      ${this._activeTab === "providers" ? this._renderProvidersTab() : nothing}
    `;
  }

  private _renderTabButton(tab: TabName, label: string, icon: string) {
    return html`
      <button
        class="tab-button"
        role="tab"
        aria-selected=${this._activeTab === tab}
        @click=${() => (this._activeTab = tab)}
      >
        <uui-icon name=${icon}></uui-icon>
        ${label}
      </button>
    `;
  }

  private _renderCreateForm() {
    return html`
      <uui-box headline="New Backup" class="section">
        <div class="form-row">
          <label>Backup Name</label>
          <uui-input
            placeholder="e.g. pre-deploy-2026-05-28"
            label="Backup name"
            .value=${this._formName}
            @input=${(e: Event) =>
              (this._formName = (e.target as HTMLInputElement).value)}
          ></uui-input>
        </div>

        <div class="form-row">
          <label>Scope</label>
          <select
            @change=${(e: Event) =>
              (this._formScope = Number(
                (e.target as HTMLSelectElement).value
              ))}
          >
            ${SCOPES.map(
              (s) =>
                html`<option value=${s.value} ?selected=${this._formScope === s.value}>
                  ${s.label}
                </option>`
            )}
          </select>
        </div>

        <div class="checkbox-row">
          <label>
            <input
              type="checkbox"
              .checked=${this._formIncludeMedia}
              @change=${(e: Event) =>
                (this._formIncludeMedia = (e.target as HTMLInputElement).checked)}
            />
            Include Media Files
          </label>
          <label>
            <input
              type="checkbox"
              .checked=${this._formCompress}
              @change=${(e: Event) =>
                (this._formCompress = (e.target as HTMLInputElement).checked)}
            />
            Compress (ZIP)
          </label>
          <label>
            <input
              type="checkbox"
              .checked=${this._formEncrypt}
              @change=${(e: Event) =>
                (this._formEncrypt = (e.target as HTMLInputElement).checked)}
            />
            Encrypt
          </label>
        </div>

        ${this._formEncrypt
          ? html`
              <div class="form-row">
                <label>Encryption Key</label>
                <uui-input
                  type="password"
                  placeholder="Enter encryption key"
                  label="Encryption key"
                  .value=${this._formEncryptionKey}
                  @input=${(e: Event) =>
                    (this._formEncryptionKey = (
                      e.target as HTMLInputElement
                    ).value)}
                ></uui-input>
              </div>
            `
          : nothing}
        ${this._providers.length > 0
          ? html`
              <div class="form-row">
                <label>Upload to Cloud (optional)</label>
                <div class="provider-checkboxes">
                  ${this._providers.map(
                    (p) => html`
                      <label>
                        <input
                          type="checkbox"
                          .checked=${this._formCloudProviderIds.includes(p.id)}
                          @change=${() =>
                            (this._formCloudProviderIds =
                              this._toggleProviderSelection(
                                this._formCloudProviderIds,
                                p.id
                              ))}
                        />
                        ${p.providerType}
                        ${p.enabled
                          ? html`<span class="badge badge--enabled"
                              >Active</span
                            >`
                          : nothing}
                      </label>
                    `
                  )}
                </div>
              </div>
            `
          : nothing}

        <div class="form-actions">
          <uui-button
            look="primary"
            color="positive"
            label="Run Backup Now"
            ?disabled=${this._creating || !this._formName.trim()}
            @click=${this._createBackup}
          >
            ${this._creating ? "Creating..." : "Run Backup Now"}
          </uui-button>
          <uui-button
            look="secondary"
            label="Cancel"
            @click=${() => (this._showCreateForm = false)}
          >
            Cancel
          </uui-button>
        </div>
      </uui-box>
    `;
  }

  private _renderBackupsTab() {
    const filtered = this._filteredBackups;

    return html`
      <div class="section">
        <uui-box headline="Backup History">
          <div class="toolbar">
            <uui-input
              type="search"
              placeholder="Filter backups..."
              label="Filter backups"
              .value=${this._filter}
              @input=${(e: Event) =>
                (this._filter = (e.target as HTMLInputElement).value)}
            ></uui-input>
            <uui-button
              look="secondary"
              label="Refresh"
              ?disabled=${this._loading}
              @click=${this._loadBackups}
            >
              ${this._loading ? "Loading..." : "Refresh"}
            </uui-button>
          </div>

          ${filtered.length > 0
            ? html`
                <uui-table>
                  <uui-table-head>
                    <uui-table-head-cell
                      class="sortable"
                      @click=${() => this._toggleSort("name")}
                    >
                      Name ${this._sortField === "name" ? (this._sortReverse ? "▼" : "▲") : ""}
                    </uui-table-head-cell>
                    <uui-table-head-cell
                      class="sortable"
                      @click=${() => this._toggleSort("createdAt")}
                    >
                      Created ${this._sortField === "createdAt" ? (this._sortReverse ? "▼" : "▲") : ""}
                    </uui-table-head-cell>
                    <uui-table-head-cell
                      class="sortable"
                      @click=${() => this._toggleSort("sizeBytes")}
                    >
                      Size ${this._sortField === "sizeBytes" ? (this._sortReverse ? "▼" : "▲") : ""}
                    </uui-table-head-cell>
                    <uui-table-head-cell>Type</uui-table-head-cell>
                    <uui-table-head-cell>Actions</uui-table-head-cell>
                  </uui-table-head>
                  ${filtered.map(
                    (b) => html`
                      <uui-table-row>
                        <uui-table-cell>
                          <strong>${b.name}</strong>
                          ${b.extension
                            ? html`<span style="opacity:0.5">.${b.extension}</span>`
                            : nothing}
                        </uui-table-cell>
                        <uui-table-cell title=${new Date(b.createdAt).toLocaleString()}>
                          ${this._timeAgo(b.createdAt)}
                        </uui-table-cell>
                        <uui-table-cell>
                          ${this._formatSize(b.sizeBytes)}
                        </uui-table-cell>
                        <uui-table-cell>
                          ${b.isCompressed
                            ? html`<span class="badge badge--compressed"
                                >ZIP</span
                              >`
                            : nothing}
                          ${b.isEncrypted
                            ? html`<span class="badge badge--encrypted"
                                >Encrypted</span
                              >`
                            : nothing}
                          ${!b.isCompressed && !b.isEncrypted
                            ? html`<span class="badge badge--raw">Raw</span>`
                            : nothing}
                        </uui-table-cell>
                        <uui-table-cell class="actions-cell">
                          <uui-button
                            look="secondary"
                            compact
                            label="Restore"
                            @click=${() => {
                              this._restoreBackupName = b.name;
                              this._activeTab = "restore";
                            }}
                          >
                            <uui-icon name="undo"></uui-icon>
                          </uui-button>
                          <uui-button
                            look="secondary"
                            color="danger"
                            compact
                            label="Delete"
                            @click=${() => this._deleteBackup(b.name)}
                          >
                            <uui-icon name="delete"></uui-icon>
                          </uui-button>
                        </uui-table-cell>
                      </uui-table-row>
                    `
                  )}
                </uui-table>
                <div class="summary-bar">
                  <span>${this._backups.length} backup(s)</span>
                  <span>|</span>
                  <span>Total: ${this._formatSize(this._totalSize)}</span>
                </div>
              `
            : html`
                <div class="empty-state">
                  <uui-icon name="box"></uui-icon>
                  <p>No backups found.</p>
                  <uui-button
                    look="primary"
                    label="Create your first backup"
                    @click=${() => (this._showCreateForm = true)}
                  >
                    Create your first backup
                  </uui-button>
                </div>
              `}
        </uui-box>
      </div>
    `;
  }

  private _renderRestoreTab() {
    return html`
      <div class="section">
        <div class="warning-box">
          <uui-icon name="alert"></uui-icon>
          <span>
            Restoring a backup will modify your site's content and/or database.
            Make sure you have a current backup before proceeding.
          </span>
        </div>

        <uui-box headline="Restore from Backup">
          <div class="form-row">
            <label>Select Backup</label>
            <select
              @change=${(e: Event) =>
                (this._restoreBackupName = (
                  e.target as HTMLSelectElement
                ).value)}
            >
              <option value="">-- Select a backup --</option>
              ${this._backups.map(
                (b) =>
                  html`<option
                    value=${b.name}
                    ?selected=${this._restoreBackupName === b.name}
                  >
                    ${b.name} (${this._formatSize(b.sizeBytes)} —
                    ${new Date(b.createdAt).toLocaleDateString()})
                  </option>`
              )}
            </select>
          </div>

          <div class="form-row">
            <label>Restore Scope</label>
            <select
              @change=${(e: Event) =>
                (this._restoreScope = Number(
                  (e.target as HTMLSelectElement).value
                ))}
            >
              ${SCOPES.map(
                (s) =>
                  html`<option
                    value=${s.value}
                    ?selected=${this._restoreScope === s.value}
                  >
                    ${s.label}
                  </option>`
              )}
            </select>
          </div>

          <div class="checkbox-row checkbox-warning">
            <label>
              <input
                type="checkbox"
                .checked=${this._restoreOverwrite}
                @change=${(e: Event) =>
                  (this._restoreOverwrite = (
                    e.target as HTMLInputElement
                  ).checked)}
              />
              Overwrite existing content
            </label>
          </div>

          ${this._restoreBackupName && this._isSelectedBackupEncrypted()
            ? html`
                <div class="form-row">
                  <label>Decryption Key</label>
                  <uui-input
                    type="password"
                    placeholder="Enter decryption key"
                    label="Decryption key"
                    .value=${this._restoreDecryptionKey}
                    @input=${(e: Event) =>
                      (this._restoreDecryptionKey = (
                        e.target as HTMLInputElement
                      ).value)}
                  ></uui-input>
                </div>
              `
            : nothing}

          <div class="form-actions">
            <uui-button
              look="primary"
              color="warning"
              label="Restore Backup"
              ?disabled=${this._restoring || !this._restoreBackupName}
              @click=${this._restoreBackup}
            >
              ${this._restoring ? "Restoring..." : "Restore Backup"}
            </uui-button>
          </div>
        </uui-box>
      </div>
    `;
  }

  private _renderScheduleTab() {
    return html`
      <div class="section">
        <div class="info-box">
          <uui-icon name="info"></uui-icon>
          <span>
            Backup scheduling is configured via
            <code>appsettings.json</code> under
            <code>SplatDev:Backups:Schedule</code>. The controls below generate
            the configuration snippet for you.
          </span>
        </div>

        <uui-box headline="Backup Schedule">
          <div class="checkbox-row">
            <label>
              <input
                type="checkbox"
                .checked=${this._scheduleEnabled}
                @change=${(e: Event) =>
                  (this._scheduleEnabled = (
                    e.target as HTMLInputElement
                  ).checked)}
              />
              Enable Scheduled Backups
            </label>
          </div>

          ${this._scheduleEnabled
            ? html`
                <div class="form-row">
                  <label>Frequency</label>
                  <select
                    @change=${(e: Event) => {
                      const val = (e.target as HTMLSelectElement).value;
                      if (val) this._scheduleCron = val;
                    }}
                  >
                    ${CRON_PRESETS.map(
                      (p) =>
                        html`<option value=${p.value}>${p.label}</option>`
                    )}
                  </select>
                </div>

                <div class="form-row">
                  <label>Cron Expression</label>
                  <uui-input
                    placeholder="0 2 * * *"
                    label="Cron expression"
                    .value=${this._scheduleCron}
                    @input=${(e: Event) =>
                      (this._scheduleCron = (
                        e.target as HTMLInputElement
                      ).value)}
                  ></uui-input>
                  <span class="help-text">
                    Format: minute hour day-of-month month day-of-week
                  </span>
                </div>

                <div class="form-row">
                  <label>Scope</label>
                  <select
                    @change=${(e: Event) =>
                      (this._scheduleScope = Number(
                        (e.target as HTMLSelectElement).value
                      ))}
                  >
                    ${SCOPES.map(
                      (s) =>
                        html`<option
                          value=${s.value}
                          ?selected=${this._scheduleScope === s.value}
                        >
                          ${s.label}
                        </option>`
                    )}
                  </select>
                </div>

                <div class="checkbox-row">
                  <label>
                    <input
                      type="checkbox"
                      .checked=${this._scheduleCompress}
                      @change=${(e: Event) =>
                        (this._scheduleCompress = (
                          e.target as HTMLInputElement
                        ).checked)}
                    />
                    Compress
                  </label>
                  <label>
                    <input
                      type="checkbox"
                      .checked=${this._scheduleKeepLocal}
                      @change=${(e: Event) =>
                        (this._scheduleKeepLocal = (
                          e.target as HTMLInputElement
                        ).checked)}
                    />
                    Keep Local Copy
                  </label>
                </div>

                <div class="config-preview">
                  <uui-box headline="appsettings.json snippet">
                    <pre>${JSON.stringify(
                      {
                        SplatDev: {
                          Backups: {
                            Schedule: {
                              Enabled: this._scheduleEnabled,
                              CronExpression: this._scheduleCron,
                              Scope: this._scheduleScope,
                              Compress: this._scheduleCompress,
                              KeepLocal: this._scheduleKeepLocal,
                            },
                          },
                        },
                      },
                      null,
                      2
                    )}</pre>
                  </uui-box>
                </div>
              `
            : nothing}
        </uui-box>
      </div>
    `;
  }

  private _renderProvidersTab() {
    return html`
      <div class="section">
        ${this._providers.length === 0
          ? html`
              <uui-box>
                <div class="empty-state">
                  <uui-icon name="cloud"></uui-icon>
                  <p>No cloud providers configured.</p>
                  <p style="font-size:0.85em">
                    Add providers in <code>appsettings.json</code> under
                    <code>SplatDev:Backups:Providers</code>.
                  </p>
                </div>
              </uui-box>
            `
          : html`
              <div class="provider-grid">
                ${this._providers.map((p) => {
                  const testResult = this._providerTestResults[p.id];
                  return html`
                    <div class="provider-card">
                      <div class="provider-card__header">
                        <uui-icon name="cloud"></uui-icon>
                        <strong>${p.providerType}</strong>
                        <span
                          class="badge ${p.enabled
                            ? "badge--enabled"
                            : "badge--disabled"}"
                        >
                          ${p.enabled ? "Enabled" : "Disabled"}
                        </span>
                      </div>
                      <div class="provider-card__body">ID: ${p.id}</div>
                      <div class="provider-card__footer">
                        <uui-button
                          look="secondary"
                          compact
                          label="Test Connection"
                          ?disabled=${testResult?.testing}
                          @click=${() => this._testProvider(p.id)}
                        >
                          ${testResult?.testing
                            ? "Testing..."
                            : "Test Connection"}
                        </uui-button>
                        ${testResult?.tested
                          ? testResult.valid
                            ? html`<span class="test-pass"
                                >Connected</span
                              >`
                            : html`<span class="test-fail">Failed</span>`
                          : nothing}
                      </div>
                    </div>
                  `;
                })}
              </div>
            `}
      </div>
    `;
  }
}

export default BackupsDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "backups-dashboard": BackupsDashboardElement;
  }
}
