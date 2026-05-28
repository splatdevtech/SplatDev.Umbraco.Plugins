import { LitElement as h, nothing as u, html as t, css as c, state as r, customElement as g } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as b } from "@umbraco-cms/backoffice/element-api";
var _ = Object.defineProperty, f = Object.getOwnPropertyDescriptor, o = (s, e, d, n) => {
  for (var a = n > 1 ? void 0 : n ? f(e, d) : e, l = s.length - 1, p; l >= 0; l--)
    (p = s[l]) && (a = (n ? p(e, d, a) : p(a)) || a);
  return n && a && _(e, d, a), a;
};
let i = class extends b(h) {
  constructor() {
    super(...arguments), this._nodeId = "", this._page = null, this._loading = !1, this._message = null, this._api = "/umbraco/api/adminbar";
  }
  async _lookup() {
    if (this._nodeId) {
      this._loading = !0, this._message = null, this._page = null;
      try {
        const s = await fetch(`${this._api}/GetCurrentPageInfo?nodeId=${this._nodeId}`);
        if (!s.ok) throw new Error(`${s.status}`);
        this._page = await s.json();
      } catch {
        this._message = { type: "error", text: "Page not found or request failed." };
      }
      this._loading = !1;
    }
  }
  async _publish() {
    this._loading = !0;
    try {
      const s = await fetch(`${this._api}/PublishPage?nodeId=${this._nodeId}`, { method: "POST" }), e = await s.json();
      this._message = { type: s.ok ? "success" : "error", text: e.message }, s.ok && await this._lookup();
    } catch {
      this._message = { type: "error", text: "Publish failed." };
    }
    this._loading = !1;
  }
  async _unpublish() {
    if (confirm("Unpublish this page?")) {
      this._loading = !0;
      try {
        const s = await fetch(`${this._api}/UnpublishPage?nodeId=${this._nodeId}`, { method: "POST" }), e = await s.json();
        this._message = { type: s.ok ? "success" : "error", text: e.message }, s.ok && await this._lookup();
      } catch {
        this._message = { type: "error", text: "Unpublish failed." };
      }
      this._loading = !1;
    }
  }
  render() {
    return t`
      <h1>Admin Bar</h1>
      <p class="description">Lookup and manage content pages by node ID.</p>

      <uui-box headline="Page Lookup">
        <div class="input-row">
          <input type="number" .value=${this._nodeId}
            @input=${(s) => this._nodeId = s.target.value}
            placeholder="Node ID" style="width:140px;padding:8px;border:1px solid #d1d5db;border-radius:4px;" />
          <uui-button look="secondary" ?disabled=${!this._nodeId || this._loading}
            @click=${this._lookup}>Lookup</uui-button>
        </div>
      </uui-box>

      ${this._message ? t`<div class="msg ${this._message.type}">${this._message.text}</div>` : u}

      ${this._page ? t`
        <div class="info-card">
          <div class="info-row">
            <span class="info-label">ID</span>
            <span class="info-value">${this._page.id}</span>
          </div>
          <div class="info-row">
            <span class="info-label">Name</span>
            <span class="info-value">${this._page.name}</span>
          </div>
          <div class="info-row">
            <span class="info-label">Status</span>
            <span class="status-badge ${this._page.published ? "published" : "unpublished"}">
              ${this._page.published ? "Published" : "Unpublished"}
            </span>
          </div>
          ${this._page.url ? t`
            <div class="info-row">
              <span class="info-label">URL</span>
              <span class="info-value">${this._page.url}</span>
            </div>
          ` : u}

          <div class="action-row">
            ${this._page.published ? t`
              <uui-button look="danger" ?disabled=${this._loading} @click=${this._unpublish}>Unpublish</uui-button>
            ` : t`
              <uui-button look="primary" ?disabled=${this._loading} @click=${this._publish}>Publish</uui-button>
            `}
          </div>
        </div>
      ` : u}
    `;
  }
};
i.styles = c`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .input-row { display: flex; gap: 10px; align-items: center; margin-bottom: 20px; }
    .info-card {
      padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 6px; margin-top: 16px;
    }
    .info-row { display: flex; gap: 8px; align-items: center; margin-bottom: 8px; }
    .info-label { font-weight: 600; min-width: 80px; font-size: 0.85rem; }
    .info-value { font-size: 0.85rem; }
    .status-badge { display: inline-block; padding: 2px 10px; border-radius: 9999px; font-size: 0.8rem; font-weight: 600; }
    .status-badge.published { background: #d1fae5; color: #065f46; }
    .status-badge.unpublished { background: #fef3c7; color: #92400e; }
    .action-row { display: flex; gap: 10px; margin-top: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-top: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
  `;
o([
  r()
], i.prototype, "_nodeId", 2);
o([
  r()
], i.prototype, "_page", 2);
o([
  r()
], i.prototype, "_loading", 2);
o([
  r()
], i.prototype, "_message", 2);
i = o([
  g("adminbar-dashboard")
], i);
const v = i;
export {
  i as AdminBarDashboardElement,
  v as default
};
