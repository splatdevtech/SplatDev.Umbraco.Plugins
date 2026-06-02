import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/SocialLoginApi";

class SocialLoginDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #3b82f6; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
    .pill-none { background: #f3f4f6; color: #374151; }
    .pill-err { background: #fee2e2; color: #dc2626; }
    .dot { width: 8px; height: 8px; border-radius: 50%; background: currentColor; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .provider-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 14px; margin-bottom: 16px; }
    .provider-card { background: var(--uui-color-surface, #fff); border: 2px solid var(--uui-color-border, #e5e7eb); border-radius: 10px; padding: 20px; text-align: center; }
    .provider-card.active { border-color: #3b82f6; box-shadow: 0 0 0 1px #3b82f6; }
    .provider-icon { font-size: 2rem; margin-bottom: 8px; }
    .provider-name { font-weight: 700; font-size: 0.9375rem; margin-bottom: 4px; }
    .provider-status { font-size: 0.8125rem; }
    .badge-active { color: #15803d; font-weight: 600; }
    .badge-inactive { color: #6b7280; }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
  `;

  static properties = {
    _status: { state: true },
    _providers: { state: true },
    _error: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._providers = [];
    this._error = "";
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    this._error = "";
    try {
      const r = await fetch(`${API}/GetProviders`);
      if (r.ok) {
        this._providers = await r.json();
        this._status = "ok";
      } else {
        this._status = "err";
        this._error = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._status = "err";
      this._error = e instanceof Error ? e.message : String(e);
    }
  }

  _providerIcon(name) {
    const n = (name || "").toLowerCase();
    if (n.includes("google")) return "🔵";
    if (n.includes("facebook")) return "🔷";
    if (n.includes("twitter") || n.includes("x")) return "🐦";
    if (n.includes("microsoft")) return "🪟";
    if (n.includes("github")) return "⚫";
    if (n.includes("linkedin")) return "🔗";
    if (n.includes("apple")) return "🍎";
    return "🔑";
  }

  render() {
    const active = this._providers.filter(p => p.isEnabled !== false);
    return html`
      <div class="header">
        <div class="logo"><span>🔑</span></div>
        <div>
          <h1>Social Login</h1>
          <p>OAuth providers configured for member social login</p>
        </div>
        <span class="pill ${this._status === 'ok' ? (active.length > 0 ? 'pill-ok' : 'pill-none') : this._status === 'loading' ? 'pill-checking' : 'pill-err'}">
          <span class="dot"></span>
          ${this._status === 'ok' ? `${active.length} active provider(s)` : this._status === 'loading' ? 'Loading…' : 'Error'}
        </span>
      </div>

      ${this._status === "err" ? html`<div class="notice err"><strong>Error:</strong> ${this._error}</div>` : ""}

      <uui-box headline="OAuth Providers">
        ${this._status === "loading" ? html`<div class="notice info">Loading providers…</div>` : ""}
        ${this._status === "ok" && this._providers.length === 0 ? html`
          <div class="empty">No social login providers configured. Add provider credentials to <code>appsettings.json</code> under <code>SocialLogin</code>.</div>
        ` : ""}
        ${this._status === "ok" && this._providers.length > 0 ? html`
          <div class="provider-grid">
            ${this._providers.map(p => html`
              <div class="provider-card ${p.isEnabled !== false ? 'active' : ''}">
                <div class="provider-icon">${this._providerIcon(p.name || p.provider)}</div>
                <div class="provider-name">${p.name || p.provider}</div>
                <div class="provider-status ${p.isEnabled !== false ? 'badge-active' : 'badge-inactive'}">
                  ${p.isEnabled !== false ? "✓ Enabled" : "○ Disabled"}
                </div>
                ${p.clientId ? html`<div style="font-size:0.75rem;color:#9ca3af;margin-top:4px;font-family:monospace">${p.clientId.slice(0, 8)}…</div>` : ""}
              </div>
            `)}
          </div>
        ` : ""}
        <div class="actions">
          <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
        </div>
      </uui-box>

      <uui-box headline="Configuration">
        <div class="notice info">
          Social login providers are configured in <code>appsettings.json</code> under the <code>SocialLogin</code> section.
          Each provider requires a <code>ClientId</code> and <code>ClientSecret</code> from the respective OAuth application dashboard.
        </div>
      </uui-box>
    `;
  }
}

customElements.define("social-login-dashboard", SocialLoginDashboard);
