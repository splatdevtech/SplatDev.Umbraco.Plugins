import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/gdrp";

class GdrpDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #4b5563; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 14px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
    .pill-warn { background: #fef9c3; color: #854d0e; }
    .pill-err { background: #fee2e2; color: #dc2626; }
    .dot { width: 8px; height: 8px; border-radius: 50%; background: currentColor; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .warn  { background: #fffbeb; color: #92400e; border-left: 3px solid #f59e0b; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 10px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); }
    td { padding: 10px 12px; border-bottom: 1px solid var(--uui-color-border, #f0f0f0); vertical-align: middle; }
    tr:hover td { background: var(--uui-color-surface-alt, #f9fafb); }
    .badge { display: inline-block; padding: 2px 10px; border-radius: 9999px; font-size: 0.75rem; font-weight: 700; }
    .badge-export  { background: #dbeafe; color: #1e40af; }
    .badge-erasure { background: #fef9c3; color: #854d0e; }
    .badge-pending   { background: #fef9c3; color: #854d0e; }
    .badge-completed { background: #dcfce7; color: #15803d; }
    .complete-btn { color: #15803d; cursor: pointer; background: #dcfce7; border: 1px solid #86efac; font-size: 0.8125rem; padding: 4px 12px; border-radius: 4px; font-weight: 600; }
    .complete-btn:hover { background: #bbf7d0; }
    .complete-btn:disabled { opacity: 0.5; cursor: not-allowed; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .date-text { font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
  `;

  static properties = {
    _status: { state: true },
    _requests: { state: true },
    _error: { state: true },
    _completing: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._requests = [];
    this._error = "";
    this._completing = new Set();
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    this._error = "";
    try {
      const r = await fetch(`${API}/GetRequests`);
      if (r.ok) {
        this._requests = await r.json();
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

  async _completeRequest(id) {
    this._completing = new Set([...this._completing, id]);
    this.requestUpdate();
    try {
      await fetch(`${API}/CompleteRequest`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id }),
      });
      await this._load();
    } catch (_) {}
    this._completing = new Set([...this._completing].filter(x => x !== id));
  }

  _fmt(dt) {
    if (!dt) return "—";
    try { return new Date(dt).toLocaleString(); } catch { return dt; }
  }

  _pending() { return this._requests.filter(r => !r.completedAt); }
  _completed() { return this._requests.filter(r => r.completedAt); }

  render() {
    const pending = this._pending();
    return html`
      <div class="header">
        <div class="logo"><span>GDPR</span></div>
        <div>
          <h1>GDPR Compliance</h1>
          <p>Manage data export and erasure requests from site users</p>
        </div>
        <span class="pill ${pending.length > 0 ? 'pill-warn' : this._status === 'loading' ? 'pill-checking' : this._status === 'ok' ? 'pill-ok' : 'pill-err'}">
          <span class="dot"></span>
          ${this._status === 'loading' ? 'Loading…' : this._status === 'ok' ? `${pending.length} pending` : 'Error'}
        </span>
      </div>

      ${this._status === "err" ? html`<div class="notice err"><strong>Error:</strong> ${this._error}</div>` : ""}
      ${this._status === "ok" && pending.length > 0 ? html`<div class="notice warn"><strong>${pending.length} pending request(s)</strong> require action. Review and complete them below.</div>` : ""}

      <uui-box headline="Pending Requests">
        ${this._status === "loading" ? html`<div class="notice info">Loading data requests…</div>` : ""}
        ${this._status === "ok" && pending.length === 0 ? html`<div class="empty">No pending GDPR requests. All caught up!</div>` : ""}
        ${this._status === "ok" && pending.length > 0 ? html`
          <table>
            <thead><tr><th>#</th><th>Email</th><th>Type</th><th>Submitted</th><th>Action</th></tr></thead>
            <tbody>
              ${pending.map(r => html`
                <tr>
                  <td>${r.id}</td>
                  <td>${r.email}</td>
                  <td><span class="badge badge-${r.requestType}">${r.requestType}</span></td>
                  <td class="date-text">${this._fmt(r.createdAt)}</td>
                  <td>
                    <button
                      class="complete-btn"
                      ?disabled=${this._completing.has(r.id)}
                      @click=${() => this._completeRequest(r.id)}
                    >${this._completing.has(r.id) ? "Processing…" : "Mark Complete"}</button>
                  </td>
                </tr>
              `)}
            </tbody>
          </table>
        ` : ""}
        <div class="actions">
          <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
        </div>
      </uui-box>

      ${this._status === "ok" && this._completed().length > 0 ? html`
        <uui-box headline="Completed Requests">
          <table>
            <thead><tr><th>#</th><th>Email</th><th>Type</th><th>Submitted</th><th>Completed</th></tr></thead>
            <tbody>
              ${this._completed().slice(0, 10).map(r => html`
                <tr>
                  <td>${r.id}</td>
                  <td>${r.email}</td>
                  <td><span class="badge badge-${r.requestType}">${r.requestType}</span></td>
                  <td class="date-text">${this._fmt(r.createdAt)}</td>
                  <td class="date-text">${this._fmt(r.completedAt)}</td>
                </tr>
              `)}
            </tbody>
          </table>
          ${this._completed().length > 10 ? html`<div class="notice info">Showing latest 10 of ${this._completed().length} completed requests.</div>` : ""}
        </uui-box>
      ` : ""}
    `;
  }
}

customElements.define("gdrp-dashboard", GdrpDashboard);
