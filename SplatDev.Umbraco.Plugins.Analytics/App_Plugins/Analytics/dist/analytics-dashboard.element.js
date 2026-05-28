import { LitElement as p, html as a, nothing as c, css as u, state as n, customElement as h } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as m } from "@umbraco-cms/backoffice/element-api";
var b = Object.defineProperty, _ = Object.getOwnPropertyDescriptor, o = (t, i, l, r) => {
  for (var s = r > 1 ? void 0 : r ? _(i, l) : i, d = t.length - 1, g; d >= 0; d--)
    (g = t[d]) && (s = (r ? g(i, l, s) : g(s)) || s);
  return r && s && b(i, l, s), s;
};
let e = class extends m(p) {
  constructor() {
    super(...arguments), this._settings = { measurementId: "", trackingEnabled: !1 }, this._pageViews = [], this._loading = !1, this._message = null, this._api = "/umbraco/api/analytics";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadSettings();
  }
  async _loadSettings() {
    this._loading = !0;
    try {
      const t = await fetch(`${this._api}/GetSettings`);
      this._settings = await t.json(), this._settings.measurementId && await this._loadPageViews();
    } catch {
      this._message = { type: "error", text: "Failed to load settings." };
    }
    this._loading = !1;
  }
  async _saveSettings() {
    this._loading = !0;
    try {
      const t = await fetch(`${this._api}/SaveSettings`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._settings)
      }), i = await t.json();
      this._message = { type: t.ok ? "success" : "error", text: i.message ?? "Saved." };
    } catch {
      this._message = { type: "error", text: "Save failed." };
    }
    this._loading = !1;
  }
  async _loadPageViews() {
    try {
      const t = await fetch(`${this._api}/GetPageViews`);
      t.ok && (this._pageViews = await t.json());
    } catch {
    }
  }
  render() {
    return this._loading ? a`<uui-loader-bar></uui-loader-bar>` : a`
      <h1>Analytics</h1>
      <p class="description">Configure Google Analytics tracking and view page statistics.</p>

      ${this._message ? a`<div class="msg ${this._message.type}">${this._message.text}</div>` : c}

      <uui-box headline="Settings">
        <div class="fields">
          <div class="field">
            <label>Measurement ID</label>
            <uui-input .value=${this._settings.measurementId}
              @input=${(t) => {
      this._settings = { ...this._settings, measurementId: t.target.value };
    }}></uui-input>
          </div>
          <div class="field">
            <label>Tracking</label>
            <uui-toggle label="Enable tracking" ?checked=${this._settings.trackingEnabled}
              @change=${(t) => {
      this._settings = { ...this._settings, trackingEnabled: t.target.checked };
    }}></uui-toggle>
          </div>
        </div>
        <uui-button look="primary" @click=${this._saveSettings}>Save Settings</uui-button>
      </uui-box>

      ${this._pageViews.length > 0 ? a`
        <uui-box headline="Page Views" class="views-list">
          <table>
            <thead><tr><th>Page</th><th>Views</th></tr></thead>
            <tbody>
              ${this._pageViews.map((t) => a`<tr><td>${t.page}</td><td>${t.views}</td></tr>`)}
            </tbody>
          </table>
        </uui-box>
      ` : c}
    `;
  }
};
e.styles = u`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; margin-bottom: 16px; }
    .field { display: flex; flex-direction: column; gap: 4px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .views-list { margin-top: 24px; }
    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
    th { text-align: left; padding: 8px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
  `;
o([
  n()
], e.prototype, "_settings", 2);
o([
  n()
], e.prototype, "_pageViews", 2);
o([
  n()
], e.prototype, "_loading", 2);
o([
  n()
], e.prototype, "_message", 2);
e = o([
  h("analytics-dashboard")
], e);
const y = e;
export {
  e as AnalyticsDashboardElement,
  y as default
};
