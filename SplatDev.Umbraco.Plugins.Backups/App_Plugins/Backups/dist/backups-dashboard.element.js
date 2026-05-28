import { LitElement as e, css as t, customElement as n, html as r, nothing as i, state as a } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as o } from "@umbraco-cms/backoffice/element-api";
//#region \0@oxc-project+runtime@0.132.0/helpers/decorate.js
function s(e, t, n, r) {
	var i = arguments.length, a = i < 3 ? t : r === null ? r = Object.getOwnPropertyDescriptor(t, n) : r, o;
	if (typeof Reflect == "object" && typeof Reflect.decorate == "function") a = Reflect.decorate(e, t, n, r);
	else for (var s = e.length - 1; s >= 0; s--) (o = e[s]) && (a = (i < 3 ? o(a) : i > 3 ? o(t, n, a) : o(t, n)) || a);
	return i > 3 && a && Object.defineProperty(t, n, a), a;
}
//#endregion
//#region src/backups-dashboard.element.ts
var c = [
	{
		value: 1,
		label: "Content"
	},
	{
		value: 2,
		label: "Media"
	},
	{
		value: 4,
		label: "Database"
	},
	{
		value: 3,
		label: "Content + Media"
	},
	{
		value: 7,
		label: "Full (Content + Media + Database)"
	}
], l = [
	{
		label: "Every day at 2 AM",
		value: "0 2 * * *"
	},
	{
		label: "Every 6 hours",
		value: "0 */6 * * *"
	},
	{
		label: "Every Sunday at midnight",
		value: "0 0 * * 0"
	},
	{
		label: "First of month at 3 AM",
		value: "0 3 1 * *"
	},
	{
		label: "Custom",
		value: ""
	}
], u = "/umbraco/api/backups", d = class extends o(e) {
	constructor(...e) {
		super(...e), this._activeTab = "backups", this._loading = !1, this._creating = !1, this._restoring = !1, this._showCreateForm = !1, this._backups = [], this._providers = [], this._filter = "", this._sortField = "createdAt", this._sortReverse = !0, this._formName = "", this._formScope = 7, this._formIncludeMedia = !0, this._formCompress = !0, this._formEncrypt = !1, this._formEncryptionKey = "", this._formCloudProviderIds = [], this._restoreBackupName = "", this._restoreScope = 7, this._restoreOverwrite = !1, this._restoreDecryptionKey = "", this._scheduleEnabled = !1, this._scheduleCron = "0 2 * * *", this._scheduleScope = 7, this._scheduleCompress = !0, this._scheduleKeepLocal = !0, this._providerTestResults = {};
	}
	static {
		this.styles = t`
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
	}
	connectedCallback() {
		super.connectedCallback(), this._loadProviders(), this._loadBackups();
	}
	async _loadBackups() {
		this._loading = !0;
		try {
			let e = await fetch(`${u}/GetAll`);
			e.ok && (this._backups = await e.json());
		} catch {} finally {
			this._loading = !1;
		}
	}
	async _loadProviders() {
		try {
			let e = await fetch(`${u}/GetCloudProviders`);
			e.ok && (this._providers = await e.json());
		} catch {
			this._providers = [];
		}
	}
	async _createBackup() {
		if (this._formName.trim() && !(this._formEncrypt && !this._formEncryptionKey)) {
			this._creating = !0;
			try {
				(await fetch(`${u}/Create`, {
					method: "POST",
					headers: { "Content-Type": "application/json" },
					body: JSON.stringify({
						name: this._formName,
						includeMedia: this._formIncludeMedia,
						scope: this._formScope,
						compress: this._formCompress,
						encrypt: this._formEncrypt,
						encryptionKey: this._formEncryptionKey,
						cloudProviderIds: this._formCloudProviderIds
					})
				})).ok && (this._formName = "", this._formEncryptionKey = "", this._showCreateForm = !1, await this._loadBackups());
			} finally {
				this._creating = !1;
			}
		}
	}
	async _deleteBackup(e) {
		confirm(`Delete backup '${e}'? This cannot be undone.`) && (await fetch(`${u}/Delete?name=${encodeURIComponent(e)}`, { method: "DELETE" })).ok && await this._loadBackups();
	}
	async _restoreBackup() {
		if (!this._restoreBackupName) return;
		let e = `Restore backup '${this._restoreBackupName}'?`;
		if (this._restoreOverwrite && (e += "\n\nWARNING: This will overwrite existing content!"), confirm(e)) {
			this._restoring = !0;
			try {
				let e = await fetch(`${u}/Restore?backupPath=${encodeURIComponent(this._restoreBackupName)}`, {
					method: "POST",
					headers: { "Content-Type": "application/json" },
					body: JSON.stringify({
						scope: this._restoreScope,
						overwriteExisting: this._restoreOverwrite,
						decryptionKey: this._restoreDecryptionKey
					})
				});
				if (e.ok) {
					let t = await e.json();
					t.success ? alert(`Restore complete. Content: ${t.restoredContentCount}, Media: ${t.restoredMediaCount}`) : alert(`Restore completed with errors:\n${t.errors.join("\n")}`), this._restoreBackupName = "", this._restoreDecryptionKey = "";
				}
			} finally {
				this._restoring = !1;
			}
		}
	}
	async _testProvider(e) {
		this._providerTestResults = {
			...this._providerTestResults,
			[e]: {
				testing: !0,
				valid: !1,
				tested: !1
			}
		};
		try {
			let t = await fetch(`${u}/TestProvider?providerId=${encodeURIComponent(e)}`, { method: "POST" }), n = t.ok ? await t.json() : { valid: !1 };
			this._providerTestResults = {
				...this._providerTestResults,
				[e]: {
					testing: !1,
					valid: n.valid,
					tested: !0
				}
			};
		} catch {
			this._providerTestResults = {
				...this._providerTestResults,
				[e]: {
					testing: !1,
					valid: !1,
					tested: !0
				}
			};
		}
	}
	_toggleProviderSelection(e, t) {
		let n = e.indexOf(t);
		return n > -1 ? [...e.slice(0, n), ...e.slice(n + 1)] : [...e, t];
	}
	_formatSize(e) {
		if (!e) return "0 B";
		let t = [
			"B",
			"KB",
			"MB",
			"GB",
			"TB"
		], n = Math.min(Math.floor(Math.log(e) / Math.log(1024)), t.length - 1);
		return `${(e / 1024 ** n).toFixed(n === 0 ? 0 : 1)} ${t[n]}`;
	}
	_timeAgo(e) {
		if (!e) return "";
		let t = Math.floor((Date.now() - new Date(e).getTime()) / 1e3);
		return t < 60 ? "just now" : t < 3600 ? `${Math.floor(t / 60)}m ago` : t < 86400 ? `${Math.floor(t / 3600)}h ago` : t < 604800 ? `${Math.floor(t / 86400)}d ago` : new Date(e).toLocaleDateString();
	}
	get _filteredBackups() {
		let e = this._backups;
		if (this._filter.trim()) {
			let t = this._filter.toLowerCase();
			e = e.filter((e) => e.name.toLowerCase().includes(t) || e.extension && e.extension.toLowerCase().includes(t));
		}
		let t = this._sortField, n = this._sortReverse ? -1 : 1;
		return [...e].sort((e, r) => {
			let i = e[t], a = r[t];
			return typeof i == "string" && typeof a == "string" ? i.localeCompare(a) * n : (i - a) * n;
		});
	}
	get _totalSize() {
		return this._backups.reduce((e, t) => e + (t.sizeBytes || 0), 0);
	}
	_isSelectedBackupEncrypted() {
		return this._backups.find((e) => e.name === this._restoreBackupName)?.isEncrypted ?? !1;
	}
	_toggleSort(e) {
		this._sortField === e ? this._sortReverse = !this._sortReverse : (this._sortField = e, this._sortReverse = !1);
	}
	render() {
		return r`
      <div class="dashboard-header">
        <div>
          <h1>Backups</h1>
          <p>Create, restore, and schedule backups from the backoffice.</p>
        </div>
        <uui-button
          look="primary"
          label="Create Backup"
          ?disabled=${this._creating}
          @click=${() => this._showCreateForm = !this._showCreateForm}
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

      ${this._showCreateForm ? this._renderCreateForm() : i}
      ${this._activeTab === "backups" ? this._renderBackupsTab() : i}
      ${this._activeTab === "restore" ? this._renderRestoreTab() : i}
      ${this._activeTab === "schedule" ? this._renderScheduleTab() : i}
      ${this._activeTab === "providers" ? this._renderProvidersTab() : i}
    `;
	}
	_renderTabButton(e, t, n) {
		return r`
      <button
        class="tab-button"
        role="tab"
        aria-selected=${this._activeTab === e}
        @click=${() => this._activeTab = e}
      >
        <uui-icon name=${n}></uui-icon>
        ${t}
      </button>
    `;
	}
	_renderCreateForm() {
		return r`
      <uui-box headline="New Backup" class="section">
        <div class="form-row">
          <label>Backup Name</label>
          <uui-input
            placeholder="e.g. pre-deploy-2026-05-28"
            label="Backup name"
            .value=${this._formName}
            @input=${(e) => this._formName = e.target.value}
          ></uui-input>
        </div>

        <div class="form-row">
          <label>Scope</label>
          <select
            @change=${(e) => this._formScope = Number(e.target.value)}
          >
            ${c.map((e) => r`<option value=${e.value} ?selected=${this._formScope === e.value}>
                  ${e.label}
                </option>`)}
          </select>
        </div>

        <div class="checkbox-row">
          <label>
            <input
              type="checkbox"
              .checked=${this._formIncludeMedia}
              @change=${(e) => this._formIncludeMedia = e.target.checked}
            />
            Include Media Files
          </label>
          <label>
            <input
              type="checkbox"
              .checked=${this._formCompress}
              @change=${(e) => this._formCompress = e.target.checked}
            />
            Compress (ZIP)
          </label>
          <label>
            <input
              type="checkbox"
              .checked=${this._formEncrypt}
              @change=${(e) => this._formEncrypt = e.target.checked}
            />
            Encrypt
          </label>
        </div>

        ${this._formEncrypt ? r`
              <div class="form-row">
                <label>Encryption Key</label>
                <uui-input
                  type="password"
                  placeholder="Enter encryption key"
                  label="Encryption key"
                  .value=${this._formEncryptionKey}
                  @input=${(e) => this._formEncryptionKey = e.target.value}
                ></uui-input>
              </div>
            ` : i}
        ${this._providers.length > 0 ? r`
              <div class="form-row">
                <label>Upload to Cloud (optional)</label>
                <div class="provider-checkboxes">
                  ${this._providers.map((e) => r`
                      <label>
                        <input
                          type="checkbox"
                          .checked=${this._formCloudProviderIds.includes(e.id)}
                          @change=${() => this._formCloudProviderIds = this._toggleProviderSelection(this._formCloudProviderIds, e.id)}
                        />
                        ${e.providerType}
                        ${e.enabled ? r`<span class="badge badge--enabled"
                              >Active</span
                            >` : i}
                      </label>
                    `)}
                </div>
              </div>
            ` : i}

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
            @click=${() => this._showCreateForm = !1}
          >
            Cancel
          </uui-button>
        </div>
      </uui-box>
    `;
	}
	_renderBackupsTab() {
		let e = this._filteredBackups;
		return r`
      <div class="section">
        <uui-box headline="Backup History">
          <div class="toolbar">
            <uui-input
              type="search"
              placeholder="Filter backups..."
              label="Filter backups"
              .value=${this._filter}
              @input=${(e) => this._filter = e.target.value}
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

          ${e.length > 0 ? r`
                <uui-table>
                  <uui-table-head>
                    <uui-table-head-cell
                      class="sortable"
                      @click=${() => this._toggleSort("name")}
                    >
                      Name ${this._sortField === "name" ? this._sortReverse ? "▼" : "▲" : ""}
                    </uui-table-head-cell>
                    <uui-table-head-cell
                      class="sortable"
                      @click=${() => this._toggleSort("createdAt")}
                    >
                      Created ${this._sortField === "createdAt" ? this._sortReverse ? "▼" : "▲" : ""}
                    </uui-table-head-cell>
                    <uui-table-head-cell
                      class="sortable"
                      @click=${() => this._toggleSort("sizeBytes")}
                    >
                      Size ${this._sortField === "sizeBytes" ? this._sortReverse ? "▼" : "▲" : ""}
                    </uui-table-head-cell>
                    <uui-table-head-cell>Type</uui-table-head-cell>
                    <uui-table-head-cell>Actions</uui-table-head-cell>
                  </uui-table-head>
                  ${e.map((e) => r`
                      <uui-table-row>
                        <uui-table-cell>
                          <strong>${e.name}</strong>
                          ${e.extension ? r`<span style="opacity:0.5">.${e.extension}</span>` : i}
                        </uui-table-cell>
                        <uui-table-cell title=${new Date(e.createdAt).toLocaleString()}>
                          ${this._timeAgo(e.createdAt)}
                        </uui-table-cell>
                        <uui-table-cell>
                          ${this._formatSize(e.sizeBytes)}
                        </uui-table-cell>
                        <uui-table-cell>
                          ${e.isCompressed ? r`<span class="badge badge--compressed"
                                >ZIP</span
                              >` : i}
                          ${e.isEncrypted ? r`<span class="badge badge--encrypted"
                                >Encrypted</span
                              >` : i}
                          ${!e.isCompressed && !e.isEncrypted ? r`<span class="badge badge--raw">Raw</span>` : i}
                        </uui-table-cell>
                        <uui-table-cell class="actions-cell">
                          <uui-button
                            look="secondary"
                            compact
                            label="Restore"
                            @click=${() => {
			this._restoreBackupName = e.name, this._activeTab = "restore";
		}}
                          >
                            <uui-icon name="undo"></uui-icon>
                          </uui-button>
                          <uui-button
                            look="secondary"
                            color="danger"
                            compact
                            label="Delete"
                            @click=${() => this._deleteBackup(e.name)}
                          >
                            <uui-icon name="delete"></uui-icon>
                          </uui-button>
                        </uui-table-cell>
                      </uui-table-row>
                    `)}
                </uui-table>
                <div class="summary-bar">
                  <span>${this._backups.length} backup(s)</span>
                  <span>|</span>
                  <span>Total: ${this._formatSize(this._totalSize)}</span>
                </div>
              ` : r`
                <div class="empty-state">
                  <uui-icon name="box"></uui-icon>
                  <p>No backups found.</p>
                  <uui-button
                    look="primary"
                    label="Create your first backup"
                    @click=${() => this._showCreateForm = !0}
                  >
                    Create your first backup
                  </uui-button>
                </div>
              `}
        </uui-box>
      </div>
    `;
	}
	_renderRestoreTab() {
		return r`
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
              @change=${(e) => this._restoreBackupName = e.target.value}
            >
              <option value="">-- Select a backup --</option>
              ${this._backups.map((e) => r`<option
                    value=${e.name}
                    ?selected=${this._restoreBackupName === e.name}
                  >
                    ${e.name} (${this._formatSize(e.sizeBytes)} —
                    ${new Date(e.createdAt).toLocaleDateString()})
                  </option>`)}
            </select>
          </div>

          <div class="form-row">
            <label>Restore Scope</label>
            <select
              @change=${(e) => this._restoreScope = Number(e.target.value)}
            >
              ${c.map((e) => r`<option
                    value=${e.value}
                    ?selected=${this._restoreScope === e.value}
                  >
                    ${e.label}
                  </option>`)}
            </select>
          </div>

          <div class="checkbox-row checkbox-warning">
            <label>
              <input
                type="checkbox"
                .checked=${this._restoreOverwrite}
                @change=${(e) => this._restoreOverwrite = e.target.checked}
              />
              Overwrite existing content
            </label>
          </div>

          ${this._restoreBackupName && this._isSelectedBackupEncrypted() ? r`
                <div class="form-row">
                  <label>Decryption Key</label>
                  <uui-input
                    type="password"
                    placeholder="Enter decryption key"
                    label="Decryption key"
                    .value=${this._restoreDecryptionKey}
                    @input=${(e) => this._restoreDecryptionKey = e.target.value}
                  ></uui-input>
                </div>
              ` : i}

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
	_renderScheduleTab() {
		return r`
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
                @change=${(e) => this._scheduleEnabled = e.target.checked}
              />
              Enable Scheduled Backups
            </label>
          </div>

          ${this._scheduleEnabled ? r`
                <div class="form-row">
                  <label>Frequency</label>
                  <select
                    @change=${(e) => {
			let t = e.target.value;
			t && (this._scheduleCron = t);
		}}
                  >
                    ${l.map((e) => r`<option value=${e.value}>${e.label}</option>`)}
                  </select>
                </div>

                <div class="form-row">
                  <label>Cron Expression</label>
                  <uui-input
                    placeholder="0 2 * * *"
                    label="Cron expression"
                    .value=${this._scheduleCron}
                    @input=${(e) => this._scheduleCron = e.target.value}
                  ></uui-input>
                  <span class="help-text">
                    Format: minute hour day-of-month month day-of-week
                  </span>
                </div>

                <div class="form-row">
                  <label>Scope</label>
                  <select
                    @change=${(e) => this._scheduleScope = Number(e.target.value)}
                  >
                    ${c.map((e) => r`<option
                          value=${e.value}
                          ?selected=${this._scheduleScope === e.value}
                        >
                          ${e.label}
                        </option>`)}
                  </select>
                </div>

                <div class="checkbox-row">
                  <label>
                    <input
                      type="checkbox"
                      .checked=${this._scheduleCompress}
                      @change=${(e) => this._scheduleCompress = e.target.checked}
                    />
                    Compress
                  </label>
                  <label>
                    <input
                      type="checkbox"
                      .checked=${this._scheduleKeepLocal}
                      @change=${(e) => this._scheduleKeepLocal = e.target.checked}
                    />
                    Keep Local Copy
                  </label>
                </div>

                <div class="config-preview">
                  <uui-box headline="appsettings.json snippet">
                    <pre>${JSON.stringify({ SplatDev: { Backups: { Schedule: {
			Enabled: this._scheduleEnabled,
			CronExpression: this._scheduleCron,
			Scope: this._scheduleScope,
			Compress: this._scheduleCompress,
			KeepLocal: this._scheduleKeepLocal
		} } } }, null, 2)}</pre>
                  </uui-box>
                </div>
              ` : i}
        </uui-box>
      </div>
    `;
	}
	_renderProvidersTab() {
		return r`
      <div class="section">
        ${this._providers.length === 0 ? r`
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
            ` : r`
              <div class="provider-grid">
                ${this._providers.map((e) => {
			let t = this._providerTestResults[e.id];
			return r`
                    <div class="provider-card">
                      <div class="provider-card__header">
                        <uui-icon name="cloud"></uui-icon>
                        <strong>${e.providerType}</strong>
                        <span
                          class="badge ${e.enabled ? "badge--enabled" : "badge--disabled"}"
                        >
                          ${e.enabled ? "Enabled" : "Disabled"}
                        </span>
                      </div>
                      <div class="provider-card__body">ID: ${e.id}</div>
                      <div class="provider-card__footer">
                        <uui-button
                          look="secondary"
                          compact
                          label="Test Connection"
                          ?disabled=${t?.testing}
                          @click=${() => this._testProvider(e.id)}
                        >
                          ${t?.testing ? "Testing..." : "Test Connection"}
                        </uui-button>
                        ${t?.tested ? t.valid ? r`<span class="test-pass"
                                >Connected</span
                              >` : r`<span class="test-fail">Failed</span>` : i}
                      </div>
                    </div>
                  `;
		})}
              </div>
            `}
      </div>
    `;
	}
};
s([a()], d.prototype, "_activeTab", void 0), s([a()], d.prototype, "_loading", void 0), s([a()], d.prototype, "_creating", void 0), s([a()], d.prototype, "_restoring", void 0), s([a()], d.prototype, "_showCreateForm", void 0), s([a()], d.prototype, "_backups", void 0), s([a()], d.prototype, "_providers", void 0), s([a()], d.prototype, "_filter", void 0), s([a()], d.prototype, "_sortField", void 0), s([a()], d.prototype, "_sortReverse", void 0), s([a()], d.prototype, "_formName", void 0), s([a()], d.prototype, "_formScope", void 0), s([a()], d.prototype, "_formIncludeMedia", void 0), s([a()], d.prototype, "_formCompress", void 0), s([a()], d.prototype, "_formEncrypt", void 0), s([a()], d.prototype, "_formEncryptionKey", void 0), s([a()], d.prototype, "_formCloudProviderIds", void 0), s([a()], d.prototype, "_restoreBackupName", void 0), s([a()], d.prototype, "_restoreScope", void 0), s([a()], d.prototype, "_restoreOverwrite", void 0), s([a()], d.prototype, "_restoreDecryptionKey", void 0), s([a()], d.prototype, "_scheduleEnabled", void 0), s([a()], d.prototype, "_scheduleCron", void 0), s([a()], d.prototype, "_scheduleScope", void 0), s([a()], d.prototype, "_scheduleCompress", void 0), s([a()], d.prototype, "_scheduleKeepLocal", void 0), s([a()], d.prototype, "_providerTestResults", void 0), d = s([n("backups-dashboard")], d);
var f = d;
//#endregion
export { d as BackupsDashboardElement, f as default };
