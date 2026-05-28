import { LitElement as h, nothing as n, html as o, css as c, state as r, customElement as g } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as _ } from "@umbraco-cms/backoffice/element-api";
var m = Object.defineProperty, y = Object.getOwnPropertyDescriptor, s = (t, i, d, l) => {
  for (var a = l > 1 ? void 0 : l ? y(i, d) : i, p = t.length - 1, u; p >= 0; p--)
    (u = t[p]) && (a = (l ? u(i, d, a) : u(a)) || a);
  return l && a && m(i, d, a), a;
};
let e = class extends _(h) {
  constructor() {
    super(...arguments), this._groups = [], this._selectedGroup = null, this._settings = [], this._editKey = "", this._editValue = "", this._loading = !1, this._message = null, this._api = "/umbraco/api/SettingsApi";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadGroups();
  }
  async _loadGroups() {
    this._loading = !0;
    try {
      const t = await fetch(`${this._api}/GetGroups`);
      this._groups = await t.json(), this._groups.length > 0 && await this._selectGroup(this._groups[0]);
    } catch {
      this._groups = [];
    }
    this._loading = !1;
  }
  async _selectGroup(t) {
    this._selectedGroup = t, this._loading = !0;
    try {
      const i = await fetch(`${this._api}/GetByGroup?groupId=${t.id}`);
      this._settings = await i.json();
    } catch {
      this._settings = [];
    }
    this._loading = !1;
  }
  async _setSetting() {
    if (this._editKey) {
      this._loading = !0;
      try {
        await fetch(`${this._api}/Set`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ key: this._editKey, value: this._editValue })
        }), this._message = { type: "success", text: `Setting '${this._editKey}' saved.` }, this._editKey = "", this._editValue = "", this._selectedGroup && await this._selectGroup(this._selectedGroup);
      } catch {
        this._message = { type: "error", text: "Save failed." };
      }
      this._loading = !1;
    }
  }
  async _delete(t) {
    if (confirm("Delete this setting?")) {
      this._loading = !0;
      try {
        await fetch(`${this._api}/Delete?id=${t}`, { method: "DELETE" }), this._message = { type: "success", text: "Setting deleted." }, this._selectedGroup && await this._selectGroup(this._selectedGroup);
      } catch {
        this._message = { type: "error", text: "Delete failed." };
      }
      this._loading = !1;
    }
  }
  render() {
    return o`
      <h1>Settings</h1>
      <p class="description">Manage application settings grouped by category.</p>

      ${this._message ? o`<div class="msg ${this._message.type}">${this._message.text}</div>` : n}
      ${this._loading ? o`<uui-loader-bar></uui-loader-bar>` : n}

      <div class="layout">
        <div class="groups">
          ${this._groups.map((t) => {
      var i;
      return o`
            <div class="group-item ${t.id === ((i = this._selectedGroup) == null ? void 0 : i.id) ? "active" : ""}" @click=${() => this._selectGroup(t)}>
              ${t.name}
            </div>
          `;
    })}
        </div>
        <div class="settings-panel">
          ${this._settings.length === 0 && !this._loading ? o`<div class="empty">No settings in this group.</div>` : o`
                <table>
                  <thead><tr><th>Key</th><th>Value</th><th></th></tr></thead>
                  <tbody>
                    ${this._settings.map((t) => o`
                      <tr>
                        <td>${t.key}</td>
                        <td>${t.value}</td>
                        <td><uui-button look="danger" compact @click=${() => this._delete(t.id)}>Delete</uui-button></td>
                      </tr>
                    `)}
                  </tbody>
                </table>
              `}
          <div class="edit-row">
            <uui-input placeholder="Key" .value=${this._editKey} @input=${(t) => this._editKey = t.target.value}></uui-input>
            <uui-input placeholder="Value" .value=${this._editValue} @input=${(t) => this._editValue = t.target.value}></uui-input>
            <uui-button look="primary" @click=${this._setSetting} ?disabled=${!this._editKey}>Set</uui-button>
          </div>
        </div>
      </div>
    `;
  }
};
e.styles = c`
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
s([
  r()
], e.prototype, "_groups", 2);
s([
  r()
], e.prototype, "_selectedGroup", 2);
s([
  r()
], e.prototype, "_settings", 2);
s([
  r()
], e.prototype, "_editKey", 2);
s([
  r()
], e.prototype, "_editValue", 2);
s([
  r()
], e.prototype, "_loading", 2);
s([
  r()
], e.prototype, "_message", 2);
e = s([
  g("settings-dashboard")
], e);
const v = e;
export {
  e as SettingsDashboardElement,
  v as default
};
