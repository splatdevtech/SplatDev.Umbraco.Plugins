import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/ShareApi";

class SocialShareDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #0ea5e9; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 10px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 200px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], input[type=url] { border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px; font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a); }
    input:focus { outline: none; border-color: #0ea5e9; box-shadow: 0 0 0 2px rgba(14,165,233,0.15); }
    .link-list { display: flex; flex-direction: column; gap: 8px; margin-top: 16px; }
    .link-row { display: flex; align-items: center; gap: 12px; background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 12px 14px; }
    .link-platform { font-weight: 700; font-size: 0.875rem; min-width: 90px; }
    .link-url { flex: 1; font-family: monospace; font-size: 0.8125rem; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .open-btn { padding: 5px 12px; border-radius: 4px; border: 1px solid var(--uui-color-border, #d1d5db); background: var(--uui-color-surface, #fff); cursor: pointer; font-size: 0.8125rem; text-decoration: none; color: var(--uui-color-text, #1a1a1a); }
    .open-btn:hover { background: var(--uui-color-surface-alt, #f9fafb); }
    uui-box { margin-bottom: 20px; }
    .platform-icon { font-size: 1.25rem; margin-right: 6px; }
  `;

  static properties = {
    _pageUrl: { state: true },
    _pageTitle: { state: true },
    _loading: { state: true },
    _links: { state: true },
    _error: { state: true },
  };

  constructor() {
    super();
    this._pageUrl = "";
    this._pageTitle = "";
    this._loading = false;
    this._links = null;
    this._error = "";
  }

  async _getLinks() {
    if (!this._pageUrl.trim() || !this._pageTitle.trim()) return;
    this._loading = true;
    this._links = null;
    this._error = "";
    try {
      const params = new URLSearchParams({ pageUrl: this._pageUrl.trim(), pageTitle: this._pageTitle.trim() });
      const r = await fetch(`${API}/GetShareLinks?${params}`);
      if (r.ok) {
        this._links = await r.json();
      } else {
        const text = await r.text();
        this._error = `HTTP ${r.status}: ${text}`;
      }
    } catch (e) {
      this._error = e instanceof Error ? e.message : String(e);
    } finally {
      this._loading = false;
    }
  }

  _icon(key) {
    const map = {
      facebook: "📘", twitter: "🐦", x: "🐦", linkedin: "💼",
      whatsapp: "💬", telegram: "✈️", reddit: "🟠", email: "✉️",
    };
    return map[(key || "").toLowerCase()] || "🔗";
  }

  render() {
    const entries = this._links ? Object.entries(this._links) : [];
    return html`
      <div class="header">
        <div class="logo"><span>↗</span></div>
        <div>
          <h1>Social Share</h1>
          <p>Generate social media share links for any page URL</p>
        </div>
      </div>

      <uui-box headline="Generate Share Links">
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Page URL *</span>
            <input type="url" .value=${this._pageUrl} @input=${e => this._pageUrl = e.target.value} placeholder="https://example.com/my-article" />
          </div>
          <div class="form-col">
            <span class="field-label">Page Title *</span>
            <input type="text" .value=${this._pageTitle} @input=${e => this._pageTitle = e.target.value} placeholder="My Amazing Article" />
          </div>
        </div>
        <uui-button
          look="primary"
          label="Generate Links"
          ?disabled=${!this._pageUrl.trim() || !this._pageTitle.trim() || this._loading}
          @click=${() => this._getLinks()}
          style="--uui-button-background-color:#0ea5e9;--uui-button-background-color-hover:#0284c7"
        >${this._loading ? "Generating…" : "Generate Share Links"}</uui-button>

        ${this._error ? html`<div class="notice err" style="margin-top:12px">${this._error}</div>` : ""}

        ${entries.length > 0 ? html`
          <div class="link-list">
            ${entries.map(([platform, url]) => html`
              <div class="link-row">
                <span class="link-platform"><span class="platform-icon">${this._icon(platform)}</span>${platform}</span>
                <span class="link-url" title="${url}">${url}</span>
                <a class="open-btn" href="${url}" target="_blank" rel="noopener">Open ↗</a>
              </div>
            `)}
          </div>
        ` : ""}
      </uui-box>

      <uui-box headline="About Social Share">
        <div class="notice info">
          The Social Share plugin generates one-click share URLs for major social platforms. Embed these links in templates using the provided API endpoint: <code>GET /umbraco/api/ShareApi/GetShareLinks?pageUrl=...&amp;pageTitle=...</code>
        </div>
      </uui-box>
    `;
  }
}

customElements.define("social-share-dashboard", SocialShareDashboard);
