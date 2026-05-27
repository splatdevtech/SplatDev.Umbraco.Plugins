import { LitElement as c, html as a, css as p, state as l, customElement as b } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as v } from "@umbraco-cms/backoffice/element-api";
var g = Object.defineProperty, h = Object.getOwnPropertyDescriptor, t = (e, r, o, u) => {
  for (var s = u > 1 ? void 0 : u ? h(r, o) : r, d = e.length - 1, n; d >= 0; d--)
    (n = e[d]) && (s = (u ? n(r, o, s) : n(s)) || s);
  return u && s && g(r, o, s), s;
};
let i = class extends v(c) {
  constructor() {
    super(...arguments), this._activeTab = "status", this._cronExpression = "0 2 * * *", this._scheduleEnabled = !0, this._runningBackup = !1, this._providers = [
      { id: "gcs", name: "Google Cloud", initials: "GC", color: "#4285f4", enabled: !1 },
      { id: "box", name: "Box", initials: "BX", color: "#0061d5", enabled: !1 },
      { id: "dropbox", name: "Dropbox", initials: "DB", color: "#0061fe", enabled: !1 },
      { id: "onedrive", name: "OneDrive", initials: "OD", color: "#0078d4", enabled: !1 },
      { id: "s3", name: "Amazon S3", initials: "S3", color: "#ff9900", enabled: !1 },
      { id: "azure", name: "Azure Blob", initials: "AZ", color: "#008ad7", enabled: !1 }
    ], this._historyData = [
      { date: "2026-04-22 02:00", size: "1.2 GB", status: "Success", provider: "Google Cloud" },
      { date: "2026-04-21 02:00", size: "1.1 GB", status: "Success", provider: "Google Cloud" },
      { date: "2026-04-20 02:00", size: "1.1 GB", status: "Failed", provider: "Amazon S3" },
      { date: "2026-04-19 02:00", size: "1.0 GB", status: "Success", provider: "Google Cloud" },
      { date: "2026-04-18 02:00", size: "980 MB", status: "Success", provider: "Azure Blob" }
    ];
  }
  async _runBackupNow() {
    this._runningBackup = !0, await new Promise((e) => setTimeout(e, 2e3)), this._runningBackup = !1;
  }
  _renderStatusTab() {
    return a`
      <div class="notice">
        Phase 3 BE APIs are pending. Status data shown below is placeholder.
      </div>
      <uui-box headline="Backup Status">
        <div class="status-grid">
          <div>
            <p class="stat-label">Last Backup</p>
            <p class="stat-value">2026-04-22 02:00</p>
          </div>
          <div>
            <p class="stat-label">Backup Size</p>
            <p class="stat-value">1.2 GB</p>
          </div>
          <div>
            <p class="stat-label">Next Scheduled</p>
            <p class="stat-value">2026-04-23 02:00</p>
          </div>
          <div>
            <p class="stat-label">Current Status</p>
            <div class="status-indicator">
              <span class="dot success"></span>
              <span>Healthy</span>
            </div>
          </div>
        </div>
        <div style="margin-top: 16px;">
          <uui-button
            look="primary"
            label="Run Backup Now"
            ?disabled=${this._runningBackup}
            @click=${this._runBackupNow}
          >
            ${this._runningBackup ? "Running..." : "Run Backup Now"}
          </uui-button>
        </div>
      </uui-box>
    `;
  }
  _renderScheduleTab() {
    return a`
      <div class="notice">
        Phase 3 BE APIs are pending. Schedule configuration is not yet persisted.
      </div>
      <uui-box headline="Backup Schedule">
        <div class="form-row">
          <label for="cron-input">Cron Expression</label>
          <uui-input
            id="cron-input"
            .value=${this._cronExpression}
            placeholder="0 2 * * *"
            @input=${(e) => {
      this._cronExpression = e.target.value;
    }}
          ></uui-input>
          <small style="color: var(--uui-color-text-alt);">
            Current: <strong>${this._cronExpression}</strong> — runs daily at 02:00 UTC
          </small>
        </div>
        <div class="toggle-row">
          <uui-toggle
            label="Enable scheduled backups"
            ?checked=${this._scheduleEnabled}
            @change=${(e) => {
      this._scheduleEnabled = e.target.checked;
    }}
          ></uui-toggle>
          <span>${this._scheduleEnabled ? "Enabled" : "Disabled"}</span>
        </div>
        <uui-button look="primary" label="Save Schedule">
          Save Schedule
        </uui-button>
      </uui-box>
    `;
  }
  _renderProvidersTab() {
    return a`
      <div class="notice">
        Phase 3 BE APIs are pending. Provider configuration is not yet persisted.
      </div>
      <div class="providers-grid">
        ${this._providers.map(
      (e) => a`
            <div class="provider-card">
              <div class="provider-icon" style="background: ${e.color};">${e.initials}</div>
              <span class="provider-name">${e.name}</span>
              <div class="provider-actions">
                <uui-button look="secondary" label="Configure ${e.name}">Configure</uui-button>
                <uui-button
                  look="${e.enabled ? "outline" : "primary"}"
                  label="${e.enabled ? "Disable" : "Enable"} ${e.name}"
                >
                  ${e.enabled ? "Disable" : "Enable"}
                </uui-button>
              </div>
            </div>
          `
    )}
      </div>
    `;
  }
  _renderHistoryTab() {
    return a`
      <div class="notice">
        Phase 3 BE APIs are pending. History data shown below is placeholder.
      </div>
      <uui-table>
        <uui-table-head>
          <uui-table-head-cell>Date</uui-table-head-cell>
          <uui-table-head-cell>Size</uui-table-head-cell>
          <uui-table-head-cell>Status</uui-table-head-cell>
          <uui-table-head-cell>Provider</uui-table-head-cell>
        </uui-table-head>
        ${this._historyData.map(
      (e) => a`
            <uui-table-row>
              <uui-table-cell>${e.date}</uui-table-cell>
              <uui-table-cell>${e.size}</uui-table-cell>
              <uui-table-cell>
                <span class="badge ${e.status.toLowerCase()}">
                  ${e.status}
                </span>
              </uui-table-cell>
              <uui-table-cell>${e.provider}</uui-table-cell>
            </uui-table-row>
          `
    )}
      </uui-table>
    `;
  }
  render() {
    return a`
      <h1>Backup Manager</h1>
      <p class="description">
        Monitor, schedule, and manage backups of your Umbraco site across
        multiple cloud storage providers.
      </p>

      <uui-tab-group>
        ${["status", "schedule", "providers", "history"].map(
      (e) => a`
            <uui-tab
              label=${e.charAt(0).toUpperCase() + e.slice(1)}
              ?active=${this._activeTab === e}
              @click=${() => this._activeTab = e}
            >
              ${{
        status: "Status",
        schedule: "Schedule",
        providers: "Cloud Providers",
        history: "History"
      }[e]}
            </uui-tab>
          `
    )}
      </uui-tab-group>

      <div class="tab-content">
        ${this._activeTab === "status" ? this._renderStatusTab() : this._activeTab === "schedule" ? this._renderScheduleTab() : this._activeTab === "providers" ? this._renderProvidersTab() : this._renderHistoryTab()}
      </div>
    `;
  }
};
i.styles = p`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
    }

    h1 {
      font-size: 1.5rem;
      font-weight: 600;
      margin: 0 0 var(--uui-size-space-3, 8px);
    }

    p.description {
      color: var(--uui-color-text-alt, #6b7280);
      margin: 0 0 var(--uui-size-space-6, 24px);
    }

    uui-tab-group {
      margin-bottom: var(--uui-size-space-5, 16px);
    }

    .tab-content {
      margin-top: var(--uui-size-space-5, 16px);
    }

    .notice {
      background: #fef9c3;
      border: 1px solid #fde047;
      border-radius: var(--uui-border-radius, 4px);
      padding: var(--uui-size-space-3, 8px) var(--uui-size-space-4, 12px);
      font-size: 0.875rem;
      color: #713f12;
      margin-bottom: var(--uui-size-space-5, 16px);
    }

    .status-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--uui-size-space-4, 12px);
    }

    .stat-label {
      font-size: 0.75rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--uui-color-text-alt, #6b7280);
      margin: 0 0 4px;
    }

    .stat-value {
      font-size: 1.25rem;
      font-weight: 600;
      margin: 0;
    }

    .status-indicator {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      font-size: 0.875rem;
    }

    .dot {
      width: 10px;
      height: 10px;
      border-radius: 50%;
    }

    .dot.success { background: #22c55e; }
    .dot.warning { background: #f59e0b; }
    .dot.running { background: #3b82f6; }

    .form-row {
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-space-3, 8px);
      max-width: 480px;
      margin-bottom: var(--uui-size-space-4, 12px);
    }

    .form-row label {
      font-weight: 500;
      font-size: 0.875rem;
    }

    .toggle-row {
      display: flex;
      align-items: center;
      gap: var(--uui-size-space-3, 8px);
      margin-bottom: var(--uui-size-space-4, 12px);
    }

    .providers-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
      gap: var(--uui-size-space-4, 12px);
    }

    .provider-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--uui-size-space-3, 8px);
      padding: var(--uui-size-space-5, 16px);
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: var(--uui-border-radius, 4px);
      background: var(--uui-color-surface, #fff);
    }

    .provider-icon {
      width: 56px;
      height: 56px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
      font-size: 1rem;
      color: #fff;
    }

    .provider-name {
      font-weight: 600;
      font-size: 0.9rem;
    }

    .provider-actions {
      display: flex;
      gap: var(--uui-size-space-2, 6px);
    }

    .badge {
      display: inline-block;
      padding: 2px 8px;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 600;
    }

    .badge.success { background: #d1fae5; color: #065f46; }
    .badge.failed { background: #fee2e2; color: #991b1b; }
    .badge.running { background: #dbeafe; color: #1e40af; }

    uui-table { width: 100%; }
  `;
t([
  l()
], i.prototype, "_activeTab", 2);
t([
  l()
], i.prototype, "_cronExpression", 2);
t([
  l()
], i.prototype, "_scheduleEnabled", 2);
t([
  l()
], i.prototype, "_runningBackup", 2);
i = t([
  b("backup-manager-dashboard")
], i);
const x = i;
export {
  i as BackupManagerDashboardElement,
  x as default
};
