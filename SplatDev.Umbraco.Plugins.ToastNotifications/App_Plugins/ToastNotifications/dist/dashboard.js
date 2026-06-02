import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/toastnotifications";

class ToastNotificationsDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #f59e0b; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
    .pill-err { background: #fee2e2; color: #dc2626; }
    .dot { width: 8px; height: 8px; border-radius: 50%; background: currentColor; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 10px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); }
    td { padding: 10px 12px; border-bottom: 1px solid var(--uui-color-border, #f0f0f0); vertical-align: middle; }
    tr:hover td { background: var(--uui-color-surface-alt, #f9fafb); }
    .type-badge { display: inline-block; padding: 2px 10px; border-radius: 9999px; font-size: 0.75rem; font-weight: 700; }
    .type-info    { background: #dbeafe; color: #1e40af; }
    .type-success { background: #dcfce7; color: #15803d; }
    .type-warning { background: #fef9c3; color: #854d0e; }
    .type-error   { background: #fee2e2; color: #991b1b; }
    .delete-btn { color: #dc2626; cursor: pointer; background: none; border: none; font-size: 0.875rem; padding: 4px 8px; border-radius: 4px; }
    .delete-btn:hover { background: #fee2e2; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 10px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 140px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], input[type=datetime-local], select, textarea {
      border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px;
      font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a);
      font-family: inherit;
    }
    textarea { resize: vertical; min-height: 70px; }
    input:focus, select:focus, textarea:focus { outline: none; border-color: #f59e0b; box-shadow: 0 0 0 2px rgba(245,158,11,0.15); }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
    .date-text { font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); }
  `;

  static properties = {
    _status: { state: true },
    _toasts: { state: true },
    _error: { state: true },
    _title: { state: true },
    _body: { state: true },
    _type: { state: true },
    _startDate: { state: true },
    _endDate: { state: true },
    _creating: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._toasts = [];
    this._error = "";
    this._title = "";
    this._body = "";
    this._type = "info";
    this._startDate = "";
    this._endDate = "";
    this._creating = false;
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    this._error = "";
    try {
      const r = await fetch(`${API}/GetActive`);
      if (r.ok) {
        this._toasts = await r.json();
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

  async _createToast() {
    if (!this._title.trim()) return;
    this._creating = true;
    try {
      const r = await fetch(`${API}/Create`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          title: this._title.trim(),
          body: this._body.trim(),
          type: this._type,
          isActive: true,
          startDate: this._startDate || null,
          endDate: this._endDate || null,
        }),
      });
      if (r.ok) {
        this._title = "";
        this._body = "";
        this._type = "info";
        this._startDate = "";
        this._endDate = "";
        await this._load();
      }
    } catch (_) {}
    this._creating = false;
  }

  async _deleteToast(id) {
    if (!confirm("Delete this toast notification?")) return;
    await fetch(`${API}/Delete?id=${id}`, { method: "DELETE" });
    await this._load();
  }

  _typeBadge(type) {
    const map = { info: "type-info", success: "type-success", warning: "type-warning", error: "type-error" };
    return `type-badge ${map[type] || 'type-info'}`;
  }

  _fmt(dt) {
    if (!dt) return "—";
    try { return new Date(dt).toLocaleString(); } catch { return dt; }
  }

  render() {
    return html`
      <div class="header">
        <div class="logo"><span>🔔</span></div>
        <div>
          <h1>Toast Notifications</h1>
          <p>Manage active site notifications shown to visitors</p>
        </div>
        <span class="pill ${this._status === 'ok' ? 'pill-ok' : this._status === 'loading' ? 'pill-checking' : 'pill-err'}">
          <span class="dot"></span>
          ${this._status === 'ok' ? `${this._toasts.length} active` : this._status === 'loading' ? 'Loading…' : 'Error'}
        </span>
      </div>

      ${this._status === "err" ? html`<div class="notice err"><strong>Error:</strong> ${this._error}</div>` : ""}

      <uui-box headline="Active Toasts">
        ${this._status === "loading" ? html`<div class="notice info">Loading…</div>` : ""}
        ${this._status === "ok" && this._toasts.length === 0 ? html`<div class="empty">No active toast notifications.</div>` : ""}
        ${this._status === "ok" && this._toasts.length > 0 ? html`
          <table>
            <thead><tr><th>Title</th><th>Message</th><th>Type</th><th>Start</th><th>End</th><th></th></tr></thead>
            <tbody>
              ${this._toasts.map(t => html`
                <tr>
                  <td><strong>${t.title}</strong></td>
                  <td>${t.body}</td>
                  <td><span class="${this._typeBadge(t.type)}">${t.type}</span></td>
                  <td class="date-text">${this._fmt(t.startDate)}</td>
                  <td class="date-text">${this._fmt(t.endDate)}</td>
                  <td><button class="delete-btn" @click=${() => this._deleteToast(t.id)}>✕</button></td>
                </tr>
              `)}
            </tbody>
          </table>
        ` : ""}
        <div class="actions">
          <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
        </div>
      </uui-box>

      <uui-box headline="Create Toast Notification">
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Title *</span>
            <input type="text" .value=${this._title} @input=${e => this._title = e.target.value} placeholder="Maintenance window tonight" />
          </div>
          <div class="form-col" style="max-width:130px">
            <span class="field-label">Type</span>
            <select .value=${this._type} @change=${e => this._type = e.target.value}>
              <option value="info">Info</option>
              <option value="success">Success</option>
              <option value="warning">Warning</option>
              <option value="error">Error</option>
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Message</span>
            <textarea .value=${this._body} @input=${e => this._body = e.target.value} placeholder="We'll be performing maintenance from 02:00–04:00 UTC."></textarea>
          </div>
        </div>
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Start Date (optional)</span>
            <input type="datetime-local" .value=${this._startDate} @input=${e => this._startDate = e.target.value} />
          </div>
          <div class="form-col">
            <span class="field-label">End Date (optional)</span>
            <input type="datetime-local" .value=${this._endDate} @input=${e => this._endDate = e.target.value} />
          </div>
        </div>
        <uui-button
          look="primary"
          label="Create"
          ?disabled=${!this._title.trim() || this._creating}
          @click=${() => this._createToast()}
          style="--uui-button-background-color:#f59e0b;--uui-button-background-color-hover:#d97706"
        >${this._creating ? "Creating…" : "Create Notification"}</uui-button>
      </uui-box>
    `;
  }
}

customElements.define("toast-notifications-dashboard", ToastNotificationsDashboard);
