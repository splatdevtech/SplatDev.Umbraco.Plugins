import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/whatsapp";

class WhatsAppDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #25D366; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
    .pill-disabled { background: #f3f4f6; color: #374151; }
    .pill-err { background: #fee2e2; color: #dc2626; }
    .dot { width: 8px; height: 8px; border-radius: 50%; background: currentColor; }
    .grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 12px; margin-bottom: 16px; }
    .card { background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 14px; }
    .card-label { font-size: 0.75rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); margin-bottom: 4px; }
    .card-value { font-size: 0.9375rem; font-weight: 600; word-break: break-all; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .warn  { background: #fffbeb; color: #92400e; border-left: 3px solid #f59e0b; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .form-row { display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 12px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 160px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], input[type=tel] {
      border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px;
      font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a);
    }
    input:focus { outline: none; border-color: #25D366; box-shadow: 0 0 0 2px rgba(37,211,102,0.15); }
    .url-box { display: flex; align-items: center; gap: 8px; background: #f0fdf4; border: 1px solid #86efac; border-radius: 6px; padding: 10px 14px; font-family: monospace; font-size: 0.875rem; word-break: break-all; margin-top: 12px; }
    .copy-btn { cursor: pointer; padding: 4px 10px; border-radius: 4px; border: 1px solid #86efac; background: #dcfce7; color: #15803d; font-size: 0.75rem; font-weight: 600; flex-shrink: 0; }
    .copy-btn:hover { background: #bbf7d0; }
    .badge { display: inline-block; padding: 2px 10px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; }
    .badge-yes { background: #dcfce7; color: #15803d; }
    .badge-no  { background: #fee2e2; color: #dc2626; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
    uui-box { margin-bottom: 20px; }
  `;

  static properties = {
    _status: { state: true },
    _settings: { state: true },
    _error: { state: true },
    _phone: { state: true },
    _message: { state: true },
    _url: { state: true },
    _loading: { state: true },
    _copied: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._settings = null;
    this._error = "";
    this._phone = "";
    this._message = "";
    this._url = "";
    this._loading = false;
    this._copied = false;
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    try {
      const r = await fetch(`${API}/GetSettings`);
      if (r.ok) {
        this._settings = await r.json();
        this._phone = this._settings.phoneNumber || "";
        this._message = this._settings.welcomeMessage || "";
        this._status = this._settings.isEnabled ? "ok" : "disabled";
      } else {
        this._status = "err";
        this._error = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._status = "err";
      this._error = e instanceof Error ? e.message : String(e);
    }
  }

  async _generateUrl() {
    this._loading = true;
    this._url = "";
    try {
      const params = new URLSearchParams();
      if (this._phone) params.set("phoneNumber", this._phone);
      if (this._message) params.set("message", this._message);
      const r = await fetch(`${API}/GetWhatsAppUrl?${params}`);
      if (r.ok) {
        const data = await r.json();
        this._url = data.url;
      } else {
        this._url = `Error: HTTP ${r.status}`;
      }
    } catch (e) {
      this._url = e instanceof Error ? e.message : String(e);
    } finally {
      this._loading = false;
    }
  }

  async _copyUrl() {
    if (!this._url || this._url.startsWith("Error")) return;
    await navigator.clipboard.writeText(this._url);
    this._copied = true;
    setTimeout(() => { this._copied = false; }, 2000);
  }

  _pillClass() {
    if (this._status === "loading") return "pill-checking";
    if (this._status === "ok") return "pill-ok";
    if (this._status === "disabled") return "pill-disabled";
    return "pill-err";
  }

  _pillLabel() {
    if (this._status === "loading") return "Checking…";
    if (this._status === "ok") return "Enabled";
    if (this._status === "disabled") return "Disabled";
    return "Error";
  }

  render() {
    const s = this._settings;
    return html`
      <div class="header">
        <div class="logo"><span>💬</span></div>
        <div>
          <h1>WhatsApp Widget</h1>
          <p>Configure the WhatsApp floating button and generate chat links</p>
        </div>
        <span class="pill ${this._pillClass()}">
          <span class="dot"></span>${this._pillLabel()}
        </span>
      </div>

      ${this._status === "loading" ? html`<div class="notice info">Loading settings…</div>` : ""}
      ${this._status === "err" ? html`<div class="notice err"><strong>Error:</strong> ${this._error}</div>` : ""}

      ${s ? html`
        <uui-box headline="Configuration">
          <div class="grid">
            <div class="card"><div class="card-label">Phone Number</div><div class="card-value">${s.phoneNumber || "—"}</div></div>
            <div class="card"><div class="card-label">Status</div><div class="card-value"><span class="${s.isEnabled ? 'badge badge-yes' : 'badge badge-no'}">${s.isEnabled ? "Enabled" : "Disabled"}</span></div></div>
            <div class="card"><div class="card-label">Button Position</div><div class="card-value">${s.buttonPosition || "—"}</div></div>
            <div class="card" style="grid-column:span 2"><div class="card-label">Welcome Message</div><div class="card-value">${s.welcomeMessage || "—"}</div></div>
          </div>
          <div class="notice info">To change settings, update <code>appsettings.json</code> under the <code>WhatsApp</code> section.</div>
          <div class="actions">
            <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
          </div>
        </uui-box>

        <uui-box headline="Generate Chat Link">
          <div class="form-row">
            <div class="form-col">
              <span class="field-label">Phone Number</span>
              <input type="tel" .value=${this._phone} @input=${e => this._phone = e.target.value} placeholder="+55 11 99999-9999" />
            </div>
            <div class="form-col">
              <span class="field-label">Pre-filled Message</span>
              <input type="text" .value=${this._message} @input=${e => this._message = e.target.value} placeholder="Hello! I'd like more information…" />
            </div>
          </div>
          <uui-button
            look="primary"
            label="Generate Link"
            ?disabled=${!this._phone.trim() || this._loading}
            @click=${() => this._generateUrl()}
            style="--uui-button-background-color:#25D366;--uui-button-background-color-hover:#1ebe5d"
          >${this._loading ? "Generating…" : "Generate Link"}</uui-button>
          ${this._url ? html`
            <div class="url-box">
              <span style="flex:1">${this._url}</span>
              <button class="copy-btn" @click=${() => this._copyUrl()}>${this._copied ? "Copied!" : "Copy"}</button>
            </div>
            ${!this._url.startsWith("Error") ? html`
              <div style="margin-top:8px">
                <a href="${this._url}" target="_blank" rel="noopener" style="font-size:0.875rem;color:#25D366">Open link →</a>
              </div>
            ` : ""}
          ` : ""}
        </uui-box>
      ` : ""}
    `;
  }
}

customElements.define("whatsapp-dashboard", WhatsAppDashboard);
