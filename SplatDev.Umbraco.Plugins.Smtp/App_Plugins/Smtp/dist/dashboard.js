import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/smtp";
const COLOR = "#6366f1";

class SmtpDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #6366f1; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 16px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
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
    .ok-notice { background: #dcfce7; color: #15803d; border-left: 3px solid #22c55e; }
    .form-row { display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 12px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 150px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], input[type=email], input[type=number], input[type=password] {
      border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px;
      font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a);
    }
    input:focus { outline: none; border-color: #6366f1; box-shadow: 0 0 0 2px rgba(99,102,241,0.15); }
    .result-box { background: #1e293b; color: #e2e8f0; padding: 12px 14px; border-radius: 6px; font-family: monospace; font-size: 0.8125rem; white-space: pre-wrap; word-break: break-all; margin-top: 12px; }
    uui-box { margin-bottom: 20px; }
    .badge { display: inline-block; padding: 2px 10px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; }
    .badge-yes { background: #dcfce7; color: #15803d; }
    .badge-no  { background: #fee2e2; color: #dc2626; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
  `;

  static properties = {
    _status: { state: true },
    _settings: { state: true },
    _error: { state: true },
    _testHost: { state: true },
    _testPort: { state: true },
    _testUser: { state: true },
    _testPass: { state: true },
    _testSsl: { state: true },
    _testFrom: { state: true },
    _testFromName: { state: true },
    _testing: { state: true },
    _testResult: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._settings = null;
    this._error = "";
    this._testHost = "";
    this._testPort = "587";
    this._testUser = "";
    this._testPass = "";
    this._testSsl = true;
    this._testFrom = "";
    this._testFromName = "";
    this._testing = false;
    this._testResult = "";
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    this._error = "";
    try {
      const r = await fetch(`${API}/GetSettings`);
      if (r.ok) {
        this._settings = await r.json();
        this._status = "ok";
        this._testHost = this._settings.host || "";
        this._testPort = String(this._settings.port || 587);
        this._testUser = this._settings.username || "";
        this._testSsl = this._settings.enableSsl !== false;
        this._testFrom = this._settings.fromEmail || "";
        this._testFromName = this._settings.fromName || "";
      } else {
        this._status = "err";
        this._error = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._status = "err";
      this._error = e instanceof Error ? e.message : String(e);
    }
  }

  async _testConnection() {
    this._testing = true;
    this._testResult = "";
    try {
      const r = await fetch(`${API}/TestConnection`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          host: this._testHost,
          port: parseInt(this._testPort),
          username: this._testUser,
          password: this._testPass,
          enableSsl: this._testSsl,
          fromEmail: this._testFrom,
          fromName: this._testFromName,
        }),
      });
      const body = await r.json();
      this._testResult = JSON.stringify(body, null, 2);
    } catch (e) {
      this._testResult = e instanceof Error ? e.message : String(e);
    } finally {
      this._testing = false;
    }
  }

  _pillClass() {
    if (this._status === "loading") return "pill-checking";
    if (this._status === "ok") return "pill-ok";
    return "pill-err";
  }

  _pillLabel() {
    if (this._status === "loading") return "Checking…";
    if (this._status === "ok") return "Configured";
    return "Error";
  }

  _renderSettings() {
    if (this._status === "loading") return html`<div class="notice info">Loading SMTP settings…</div>`;
    if (this._status === "err") return html`<div class="notice err"><strong>Failed to load settings:</strong> ${this._error}</div>`;
    const s = this._settings;
    return html`
      <uui-box headline="Current Configuration">
        <div class="grid">
          <div class="card"><div class="card-label">Host</div><div class="card-value">${s.host || "—"}</div></div>
          <div class="card"><div class="card-label">Port</div><div class="card-value">${s.port}</div></div>
          <div class="card"><div class="card-label">Username</div><div class="card-value">${s.username || "—"}</div></div>
          <div class="card"><div class="card-label">SSL/TLS</div><div class="card-value"><span class="${s.enableSsl ? 'badge badge-yes' : 'badge badge-no'}">${s.enableSsl ? "Enabled" : "Disabled"}</span></div></div>
          <div class="card"><div class="card-label">From Email</div><div class="card-value">${s.fromEmail || "—"}</div></div>
          <div class="card"><div class="card-label">From Name</div><div class="card-value">${s.fromName || "—"}</div></div>
        </div>
        <div class="notice info">Password is masked. To update SMTP settings, edit <code>appsettings.json</code> under the <code>Smtp</code> section.</div>
        <div class="actions">
          <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
        </div>
      </uui-box>
    `;
  }

  _renderTest() {
    return html`
      <uui-box headline="Test SMTP Connection">
        <div class="notice warn">Sends a test connection using the values below. Password is required for testing.</div>
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Host *</span>
            <input type="text" .value=${this._testHost} @input=${e => this._testHost = e.target.value} placeholder="smtp.example.com" />
          </div>
          <div class="form-col" style="max-width:100px">
            <span class="field-label">Port *</span>
            <input type="number" .value=${this._testPort} @input=${e => this._testPort = e.target.value} />
          </div>
        </div>
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Username</span>
            <input type="text" .value=${this._testUser} @input=${e => this._testUser = e.target.value} />
          </div>
          <div class="form-col">
            <span class="field-label">Password</span>
            <input type="password" .value=${this._testPass} @input=${e => this._testPass = e.target.value} placeholder="Enter password to test" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">From Email *</span>
            <input type="email" .value=${this._testFrom} @input=${e => this._testFrom = e.target.value} />
          </div>
          <div class="form-col">
            <span class="field-label">From Name</span>
            <input type="text" .value=${this._testFromName} @input=${e => this._testFromName = e.target.value} />
          </div>
        </div>
        <div style="display:flex;align-items:center;gap:12px;margin-bottom:12px">
          <label style="display:flex;align-items:center;gap:6px;font-size:0.875rem;cursor:pointer">
            <input type="checkbox" ?checked=${this._testSsl} @change=${e => this._testSsl = e.target.checked} />
            Enable SSL/TLS
          </label>
        </div>
        <uui-button
          look="primary"
          label="Test Connection"
          ?disabled=${!this._testHost.trim() || this._testing}
          @click=${() => this._testConnection()}
          style="--uui-button-background-color:#6366f1;--uui-button-background-color-hover:#4f46e5"
        >${this._testing ? "Testing…" : "Test Connection"}</uui-button>
        ${this._testResult ? html`<div class="result-box">${this._testResult}</div>` : ""}
      </uui-box>
    `;
  }

  render() {
    return html`
      <div class="header">
        <div class="logo"><span>✉</span></div>
        <div>
          <h1>SMTP Settings</h1>
          <p>Email server configuration and connection test</p>
        </div>
        <span class="pill ${this._pillClass()}">
          <span class="dot"></span>${this._pillLabel()}
        </span>
      </div>
      ${this._renderSettings()}
      ${this._renderTest()}
    `;
  }
}

customElements.define("smtp-dashboard", SmtpDashboard);
