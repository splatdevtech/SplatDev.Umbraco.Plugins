import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/SettingsApi";

class SettingsDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #0ea5e9; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 18px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
    .pill-err { background: #fee2e2; color: #dc2626; }
    .dot { width: 8px; height: 8px; border-radius: 50%; background: currentColor; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .group-tabs { display: flex; gap: 4px; flex-wrap: wrap; margin-bottom: 16px; }
    .group-tab { padding: 6px 14px; border-radius: 6px; border: 1px solid var(--uui-color-border, #e5e7eb); background: var(--uui-color-surface, #fff); cursor: pointer; font-size: 0.875rem; }
    .group-tab.active { background: #0ea5e9; color: #fff; border-color: #0ea5e9; font-weight: 600; }
    .group-tab:hover:not(.active) { background: #f0f9ff; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 10px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); }
    td { padding: 10px 12px; border-bottom: 1px solid var(--uui-color-border, #f0f0f0); vertical-align: middle; }
    tr:hover td { background: var(--uui-color-surface-alt, #f9fafb); }
    .type-badge { display: inline-block; padding: 1px 8px; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; background: #eff6ff; color: #1d4ed8; }
    .key-text { font-family: monospace; font-size: 0.8125rem; }
    .value-text { max-width: 300px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .delete-btn { color: #dc2626; cursor: pointer; background: none; border: none; font-size: 0.875rem; padding: 4px 8px; border-radius: 4px; }
    .delete-btn:hover { background: #fee2e2; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-top: 12px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 140px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], select {
      border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px;
      font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a);
    }
    input:focus, select:focus { outline: none; border-color: #0ea5e9; box-shadow: 0 0 0 2px rgba(14,165,233,0.15); }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 12px; }
  `;

  static properties = {
    _groupsStatus: { state: true },
    _groups: { state: true },
    _groupsError: { state: true },
    _selectedGroup: { state: true },
    _settings: { state: true },
    _settingsLoading: { state: true },
    _newKey: { state: true },
    _newValue: { state: true },
    _saving: { state: true },
  };

  constructor() {
    super();
    this._groupsStatus = "loading";
    this._groups = [];
    this._groupsError = "";
    this._selectedGroup = null;
    this._settings = [];
    this._settingsLoading = false;
    this._newKey = "";
    this._newValue = "";
    this._saving = false;
  }

  connectedCallback() {
    super.connectedCallback();
    this._loadGroups();
  }

  async _loadGroups() {
    this._groupsStatus = "loading";
    this._groupsError = "";
    try {
      const r = await fetch(`${API}/GetGroups`);
      if (r.ok) {
        this._groups = await r.json();
        this._groupsStatus = "ok";
        if (this._groups.length > 0 && !this._selectedGroup) {
          this._selectGroup(this._groups[0]);
        }
      } else {
        this._groupsStatus = "err";
        this._groupsError = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._groupsStatus = "err";
      this._groupsError = e instanceof Error ? e.message : String(e);
    }
  }

  async _selectGroup(group) {
    this._selectedGroup = group;
    this._settingsLoading = true;
    this._settings = [];
    try {
      const r = await fetch(`${API}/GetByGroup?groupId=${group.id}`);
      if (r.ok) {
        this._settings = await r.json();
      }
    } catch (_) {}
    this._settingsLoading = false;
  }

  async _saveSetting() {
    if (!this._newKey.trim()) return;
    this._saving = true;
    try {
      const r = await fetch(`${API}/Set`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ key: this._newKey.trim(), value: this._newValue }),
      });
      if (r.ok) {
        this._newKey = "";
        this._newValue = "";
        if (this._selectedGroup) await this._selectGroup(this._selectedGroup);
      }
    } catch (_) {}
    this._saving = false;
  }

  async _deleteSetting(id) {
    if (!confirm("Delete this setting?")) return;
    await fetch(`${API}/Delete?id=${id}`, { method: "DELETE" });
    if (this._selectedGroup) await this._selectGroup(this._selectedGroup);
  }

  render() {
    return html`
      <div class="header">
        <div class="logo"><span>⚙</span></div>
        <div>
          <h1>Site Settings</h1>
          <p>Manage key-value configuration settings by group</p>
        </div>
        <span class="pill ${this._groupsStatus === 'ok' ? 'pill-ok' : this._groupsStatus === 'loading' ? 'pill-checking' : 'pill-err'}">
          <span class="dot"></span>
          ${this._groupsStatus === 'ok' ? `${this._groups.length} group(s)` : this._groupsStatus === 'loading' ? 'Loading…' : 'Error'}
        </span>
      </div>

      ${this._groupsStatus === "err" ? html`<div class="notice err"><strong>Failed to load groups:</strong> ${this._groupsError}</div>` : ""}

      ${this._groupsStatus === "ok" ? html`
        ${this._groups.length === 0 ? html`<div class="notice info">No setting groups configured. Add groups via the Settings API or appsettings.</div>` : ""}

        ${this._groups.length > 0 ? html`
          <div class="group-tabs">
            ${this._groups.map(g => html`
              <button
                class="group-tab ${this._selectedGroup?.id === g.id ? 'active' : ''}"
                @click=${() => this._selectGroup(g)}
              >${g.name}</button>
            `)}
          </div>

          <uui-box headline="${this._selectedGroup?.name || 'Settings'}">
            ${this._settingsLoading ? html`<div class="notice info">Loading settings…</div>` : ""}
            ${!this._settingsLoading ? html`
              ${this._settings.length === 0 ? html`<div class="empty">No settings in this group yet.</div>` : html`
                <table>
                  <thead>
                    <tr><th>Key</th><th>Value</th><th>Type</th><th></th></tr>
                  </thead>
                  <tbody>
                    ${this._settings.map(s => html`
                      <tr>
                        <td class="key-text">${s.key}</td>
                        <td class="value-text" title="${s.value || ''}">${s.value || <em>empty</em>}</td>
                        <td><span class="type-badge">${s.type || 'text'}</span></td>
                        <td><button class="delete-btn" @click=${() => this._deleteSetting(s.id)}>✕</button></td>
                      </tr>
                    `)}
                  </tbody>
                </table>
              `}
              <div class="form-row">
                <div class="form-col">
                  <span class="field-label">Key *</span>
                  <input type="text" .value=${this._newKey} @input=${e => this._newKey = e.target.value} placeholder="site.title" />
                </div>
                <div class="form-col">
                  <span class="field-label">Value</span>
                  <input type="text" .value=${this._newValue} @input=${e => this._newValue = e.target.value} placeholder="My Site" />
                </div>
                <div class="form-col" style="flex:0;min-width:0;justify-content:flex-end;padding-top:18px">
                  <uui-button
                    look="primary"
                    label="Save"
                    ?disabled=${!this._newKey.trim() || this._saving}
                    @click=${() => this._saveSetting()}
                    style="--uui-button-background-color:#0ea5e9;--uui-button-background-color-hover:#0284c7"
                  >${this._saving ? "Saving…" : "Save Setting"}</uui-button>
                </div>
              </div>
            ` : ""}
          </uui-box>
        ` : ""}

        <div class="actions">
          <uui-button look="secondary" label="Refresh" @click=${() => this._loadGroups()}>↻ Refresh Groups</uui-button>
        </div>
      ` : ""}
    `;
  }
}

customElements.define("settings-dashboard", SettingsDashboard);
