import { LitElement as h, nothing as d, html as l, css as c, state as s, customElement as g } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as m } from "@umbraco-cms/backoffice/element-api";
var _ = Object.defineProperty, f = Object.getOwnPropertyDescriptor, r = (e, a, n, o) => {
  for (var t = o > 1 ? void 0 : o ? f(a, n) : a, p = e.length - 1, u; p >= 0; p--)
    (u = e[p]) && (t = (o ? u(a, n, t) : u(t)) || t);
  return o && t && _(a, n, t), t;
};
let i = class extends m(h) {
  constructor() {
    super(...arguments), this._pageUrl = "", this._pageTitle = "", this._links = [], this._loading = !1, this._error = "", this._api = "/umbraco/api/ShareApi";
  }
  async _generateLinks() {
    if (!(!this._pageUrl || !this._pageTitle)) {
      this._loading = !0, this._error = "";
      try {
        const e = new URLSearchParams({ pageUrl: this._pageUrl, pageTitle: this._pageTitle }), a = await fetch(`${this._api}/GetShareLinks?${e}`);
        if (!a.ok) throw new Error();
        this._links = await a.json();
      } catch {
        this._error = "Failed to generate share links.", this._links = [];
      }
      this._loading = !1;
    }
  }
  render() {
    return l`
      <h1>Social Media Share</h1>
      <p class="description">Generate social sharing links for any page.</p>

      <uui-box headline="Generate Share Links">
        <div class="input-row">
          <div class="field">
            <label>Page URL</label>
            <uui-input placeholder="https://example.com/page" .value=${this._pageUrl}
              @input=${(e) => this._pageUrl = e.target.value}></uui-input>
          </div>
          <div class="field">
            <label>Page Title</label>
            <uui-input placeholder="My Page Title" .value=${this._pageTitle}
              @input=${(e) => this._pageTitle = e.target.value}></uui-input>
          </div>
          <uui-button look="primary" ?disabled=${!this._pageUrl || !this._pageTitle || this._loading}
            @click=${this._generateLinks}>Generate</uui-button>
        </div>
      </uui-box>

      ${this._error ? l`<div class="msg error">${this._error}</div>` : d}
      ${this._loading ? l`<uui-loader-bar></uui-loader-bar>` : d}

      ${this._links.length > 0 ? l`
        <uui-box headline="Share Links" style="margin-top:16px;">
          <div class="links-list">
            ${this._links.map((e) => l`
              <div class="link-card">
                <span class="link-platform">${e.platform}</span>
                <span class="link-url">${e.url}</span>
                <uui-button look="secondary" compact @click=${() => navigator.clipboard.writeText(e.url)}>Copy</uui-button>
              </div>
            `)}
          </div>
        </uui-box>
      ` : d}
    `;
  }
};
i.styles = c`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .input-row { display: flex; gap: 10px; align-items: flex-end; margin-bottom: 20px; flex-wrap: wrap; }
    .field { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 200px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .links-list { display: flex; flex-direction: column; gap: 8px; }
    .link-card { display: flex; justify-content: space-between; align-items: center; padding: 12px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; }
    .link-platform { font-weight: 600; text-transform: capitalize; min-width: 120px; }
    .link-url { font-size: 0.8rem; color: var(--uui-color-text-alt, #6b7280); word-break: break-all; flex: 1; margin: 0 12px; }
  `;
r([
  s()
], i.prototype, "_pageUrl", 2);
r([
  s()
], i.prototype, "_pageTitle", 2);
r([
  s()
], i.prototype, "_links", 2);
r([
  s()
], i.prototype, "_loading", 2);
r([
  s()
], i.prototype, "_error", 2);
i = r([
  g("socialmedia-share-dashboard")
], i);
const v = i;
export {
  i as SocialMediaShareDashboardElement,
  v as default
};
