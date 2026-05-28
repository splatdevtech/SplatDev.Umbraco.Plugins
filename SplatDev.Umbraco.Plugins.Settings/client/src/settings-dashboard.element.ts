import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface SettingGroup {
  id: number;
  name: string;
}

interface Setting {
  id: number;
  key: string;
  value: string;
  groupId: number;
}

@customElement("settings-dashboard")
export class SettingsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .layout { display: flex; gap: 24px; }
    .groups { min-width: 180px; }
    .group-item { padding: 8px 12px; cursor: pointer; border-radius: 4px; font-size: 0.9rem; }
    .group-item:hover { background: var(--uui-color-surface-alt, #f9fafb); }
    .group-item.active { background: var(--uui-color-surface-emphasis, #e5e7eb); font-weight: 600; }
    .settings-panel { flex: 1; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
    th { text-align: left; padding: 8px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
    .edit-row { display: flex; gap: 8px; align-items: center; margin-top: 16px; }
    .edit-row uui-input { flex: 1; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _groups: SettingGroup[] = [];
  @state() private _selectedGroup: SettingGroup | null = null;
  @state() private _settings: Setting[] = [];
  @state() private _editKey = "";
  @state() private _editValue = "";
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/SettingsApi";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadGroups();
  }

  private async _loadGroups(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetGroups`);
      this._groups = await r.json();
      if (this._groups.length > 0) await this._selectGroup(this._groups[0]);
    } catch { this._groups = []; }
    this._loading = false;
  }

  private async _selectGroup(group: SettingGroup): Promise<void> {
    this._selectedGroup = group;
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetByGroup?groupId=${group.id}`);
      this._settings = await r.json();
    } catch { this._settings = []; }
    this._loading = false;
  }

  private async _setSetting(): Promise<void> {
    if (!this._editKey) return;
    this._loading = true;
    try {
      await fetch(`${this._api}/Set`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ key: this._editKey, value: this._editValue }),
      });
      this._message = { type: "success", text: `Setting '${this._editKey}' saved.` };
      this._editKey = "";
      this._editValue = "";
      if (this._selectedGroup) await this._selectGroup(this._selectedGroup);
    } catch {
      this._message = { type: "error", text: "Save failed." };
    }
    this._loading = false;
  }

  private async _delete(id: number): Promise<void> {
    if (!confirm("Delete this setting?")) return;
    this._loading = true;
    try {
      await fetch(`${this._api}/Delete?id=${id}`, { method: "DELETE" });
      this._message = { type: "success", text: "Setting deleted." };
      if (this._selectedGroup) await this._selectGroup(this._selectedGroup);
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
    this._loading = false;
  }

  override render() {
    return html`
      <h1>Settings</h1>
      <p class="description">Manage application settings grouped by category.</p>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}
      ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}

      <div class="layout">
        <div class="groups">
          ${this._groups.map((g) => html`
            <div class="group-item ${g.id === this._selectedGroup?.id ? "active" : ""}" @click=${() => this._selectGroup(g)}>
              ${g.name}
            </div>
          `)}
        </div>
        <div class="settings-panel">
          ${this._settings.length === 0 && !this._loading
            ? html`<div class="empty">No settings in this group.</div>`
            : html`
                <table>
                  <thead><tr><th>Key</th><th>Value</th><th></th></tr></thead>
                  <tbody>
                    ${this._settings.map((s) => html`
                      <tr>
                        <td>${s.key}</td>
                        <td>${s.value}</td>
                        <td><uui-button look="danger" compact @click=${() => this._delete(s.id)}>Delete</uui-button></td>
                      </tr>
                    `)}
                  </tbody>
                </table>
              `}
          <div class="edit-row">
            <uui-input placeholder="Key" .value=${this._editKey} @input=${(e: InputEvent) => (this._editKey = (e.target as HTMLInputElement).value)}></uui-input>
            <uui-input placeholder="Value" .value=${this._editValue} @input=${(e: InputEvent) => (this._editValue = (e.target as HTMLInputElement).value)}></uui-input>
            <uui-button look="primary" @click=${this._setSetting} ?disabled=${!this._editKey}>Set</uui-button>
          </div>
        </div>
      </div>
    `;
  }
}

export default SettingsDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "settings-dashboard": SettingsDashboardElement;
  }
}
