import { LitElement as c, html as r, css as u, state as p, customElement as b } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as g } from "@umbraco-cms/backoffice/element-api";
var v = Object.defineProperty, f = Object.getOwnPropertyDescriptor, n = (e, o, t, s) => {
  for (var i = s > 1 ? void 0 : s ? f(o, t) : o, d = e.length - 1, l; d >= 0; d--)
    (l = e[d]) && (i = (s ? l(o, t, i) : l(i)) || i);
  return s && i && v(o, t, i), i;
};
let a = class extends g(c) {
  constructor() {
    super(...arguments), this._providers = [], this._loading = !1, this._api = "/umbraco/api/SocialLoginApi";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadProviders();
  }
  async _loadProviders() {
    this._loading = !0;
    try {
      const e = await fetch(`${this._api}/GetProviders`);
      this._providers = await e.json();
    } catch {
      this._providers = [];
    }
    this._loading = !1;
  }
  render() {
    return this._loading ? r`<uui-loader-bar></uui-loader-bar>` : r`
      <h1>Social Media Login</h1>
      <p class="description">View configured social login providers and their status.</p>

      ${this._providers.length === 0 ? r`<div class="empty">No social login providers configured.</div>` : r`
            <div class="provider-list">
              ${this._providers.map((e) => r`
                <div class="provider-card">
                  <span class="provider-name">${e.name}</span>
                  <span class="status-badge ${e.enabled ? "enabled" : "disabled"}">
                    ${e.enabled ? "Enabled" : "Disabled"}
                  </span>
                </div>
              `)}
            </div>
          `}
    `;
  }
};
a.styles = u`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .provider-list { display: flex; flex-direction: column; gap: 8px; }
    .provider-card { display: flex; justify-content: space-between; align-items: center; padding: 12px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; }
    .provider-name { font-weight: 600; font-size: 0.95rem; text-transform: capitalize; }
    .status-badge { padding: 2px 10px; border-radius: 9999px; font-size: 0.8rem; font-weight: 600; }
    .status-badge.enabled { background: #d1fae5; color: #065f46; }
    .status-badge.disabled { background: #fee2e2; color: #991b1b; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;
n([
  p()
], a.prototype, "_providers", 2);
n([
  p()
], a.prototype, "_loading", 2);
a = n([
  b("socialmedia-login-dashboard")
], a);
const _ = a;
export {
  a as SocialMediaLoginDashboardElement,
  _ as default
};
