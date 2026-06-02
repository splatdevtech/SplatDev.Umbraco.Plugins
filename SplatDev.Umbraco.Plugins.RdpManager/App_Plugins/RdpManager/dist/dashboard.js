import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/RdpManagerApi";

class RdpManagerDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #0078d4; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 13px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
    .pill-err { background: #fee2e2; color: #dc2626; }
    .dot { width: 8px; height: 8px; border-radius: 50%; background: currentColor; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .warn  { background: #fffbeb; color: #92400e; border-left: 3px solid #f59e0b; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .conn-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 14px; margin-bottom: 16px; }
    .conn-card { background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 16px; }
    .conn-card:hover { border-color: #0078d4; box-shadow: 0 0 0 1px #0078d4; }
    .conn-name { font-weight: 700; font-size: 1rem; margin-bottom: 4px; }
    .conn-host { font-family: monospace; font-size: 0.875rem; color: var(--uui-color-text-alt, #6b7280); margin-bottom: 8px; }
    .conn-meta { font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); margin-bottom: 10px; line-height: 1.5; }
    .conn-actions { display: flex; gap: 8px; }
    .dl-btn { padding: 6px 12px; background: #0078d4; color: #fff; border: none; border-radius: 4px; cursor: pointer; font-size: 0.8125rem; font-weight: 600; }
    .dl-btn:hover { background: #005fa3; }
    .del-btn { padding: 6px 12px; background: #fee2e2; color: #dc2626; border: 1px solid #fca5a5; border-radius: 4px; cursor: pointer; font-size: 0.8125rem; font-weight: 600; }
    .del-btn:hover { background: #fecaca; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .form-section { border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 16px; margin-bottom: 16px; background: var(--uui-color-surface-alt, #f9fafb); }
    .form-section h3 { margin: 0 0 12px; font-size: 1rem; font-weight: 700; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 10px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 130px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], input[type=number] {
      border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px;
      font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a);
    }
    input:focus { outline: none; border-color: #0078d4; box-shadow: 0 0 0 2px rgba(0,120,212,0.15); }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
  `;

  static properties = {
    _status: { state: true },
    _connections: { state: true },
    _error: { state: true },
    _name: { state: true },
    _host: { state: true },
    _port: { state: true },
    _username: { state: true },
    _domain: { state: true },
    _notes: { state: true },
    _creating: { state: true },
    _showForm: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._connections = [];
    this._error = "";
    this._name = "";
    this._host = "";
    this._port = "3389";
    this._username = "";
    this._domain = "";
    this._notes = "";
    this._creating = false;
    this._showForm = false;
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    this._error = "";
    try {
      const r = await fetch(`${API}/GetAll`);
      if (r.ok) {
        this._connections = await r.json();
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

  async _create() {
    if (!this._name.trim() || !this._host.trim()) return;
    this._creating = true;
    try {
      const r = await fetch(`${API}/Create`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: this._name.trim(),
          host: this._host.trim(),
          port: parseInt(this._port) || 3389,
          username: this._username.trim() || null,
          domain: this._domain.trim() || null,
          notes: this._notes.trim() || null,
        }),
      });
      if (r.ok) {
        this._name = "";
        this._host = "";
        this._port = "3389";
        this._username = "";
        this._domain = "";
        this._notes = "";
        this._showForm = false;
        await this._load();
      }
    } catch (_) {}
    this._creating = false;
  }

  async _delete(id) {
    if (!confirm("Delete this RDP connection?")) return;
    await fetch(`${API}/Delete?id=${id}`, { method: "DELETE" });
    await this._load();
  }

  _download(id, name) {
    const link = document.createElement("a");
    link.href = `${API}/DownloadRdpFile?id=${id}`;
    link.download = `${name.replace(/\s+/g, "_")}.rdp`;
    link.click();
  }

  render() {
    return html`
      <div class="header">
        <div class="logo"><span>RDP</span></div>
        <div>
          <h1>RDP Manager</h1>
          <p>Manage Remote Desktop connections and download .rdp files</p>
        </div>
        <span class="pill ${this._status === 'ok' ? 'pill-ok' : this._status === 'loading' ? 'pill-checking' : 'pill-err'}">
          <span class="dot"></span>
          ${this._status === 'ok' ? `${this._connections.length} connection(s)` : this._status === 'loading' ? 'Loading…' : 'Error'}
        </span>
      </div>

      ${this._status === "err" ? html`<div class="notice err"><strong>Error:</strong> ${this._error}</div>` : ""}
      ${this._status === "ok" ? html`<div class="notice warn">RDP files contain server credentials. Ensure access to this dashboard is restricted to administrators.</div>` : ""}

      <uui-box headline="Connections">
        ${this._status === "loading" ? html`<div class="notice info">Loading connections…</div>` : ""}
        ${this._status === "ok" && this._connections.length === 0 ? html`<div class="empty">No RDP connections configured. Click "Add Connection" below.</div>` : ""}
        ${this._status === "ok" && this._connections.length > 0 ? html`
          <div class="conn-grid">
            ${this._connections.map(c => html`
              <div class="conn-card">
                <div class="conn-name">${c.name}</div>
                <div class="conn-host">${c.host}:${c.port}</div>
                <div class="conn-meta">
                  ${c.username ? html`User: ${c.domain ? c.domain + '\\' : ''}${c.username}<br>` : ""}
                  ${c.notes ? html`${c.notes}` : ""}
                </div>
                <div class="conn-actions">
                  <button class="dl-btn" @click=${() => this._download(c.id, c.name)}>⬇ Download .rdp</button>
                  <button class="del-btn" @click=${() => this._delete(c.id)}>✕</button>
                </div>
              </div>
            `)}
          </div>
        ` : ""}
        <div class="actions">
          <uui-button look="primary" label="Add Connection" @click=${() => this._showForm = !this._showForm}
            style="--uui-button-background-color:#0078d4;--uui-button-background-color-hover:#005fa3"
          >${this._showForm ? "Cancel" : "+ Add Connection"}</uui-button>
          <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
        </div>
      </uui-box>

      ${this._showForm ? html`
        <uui-box headline="Add New Connection">
          <div class="form-row">
            <div class="form-col">
              <span class="field-label">Name *</span>
              <input type="text" .value=${this._name} @input=${e => this._name = e.target.value} placeholder="Production Server" />
            </div>
            <div class="form-col">
              <span class="field-label">Host *</span>
              <input type="text" .value=${this._host} @input=${e => this._host = e.target.value} placeholder="192.168.1.10" />
            </div>
            <div class="form-col" style="max-width:100px">
              <span class="field-label">Port</span>
              <input type="number" .value=${this._port} @input=${e => this._port = e.target.value} />
            </div>
          </div>
          <div class="form-row">
            <div class="form-col">
              <span class="field-label">Username</span>
              <input type="text" .value=${this._username} @input=${e => this._username = e.target.value} placeholder="administrator" />
            </div>
            <div class="form-col">
              <span class="field-label">Domain</span>
              <input type="text" .value=${this._domain} @input=${e => this._domain = e.target.value} placeholder="CORP" />
            </div>
          </div>
          <div class="form-row">
            <div class="form-col">
              <span class="field-label">Notes</span>
              <input type="text" .value=${this._notes} @input=${e => this._notes = e.target.value} placeholder="Optional notes…" />
            </div>
          </div>
          <uui-button
            look="primary"
            label="Create"
            ?disabled=${!this._name.trim() || !this._host.trim() || this._creating}
            @click=${() => this._create()}
            style="--uui-button-background-color:#0078d4;--uui-button-background-color-hover:#005fa3"
          >${this._creating ? "Creating…" : "Create Connection"}</uui-button>
        </uui-box>
      ` : ""}
    `;
  }
}

customElements.define("rdp-manager-dashboard", RdpManagerDashboard);
