import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/propertiesreport";

class PropertiesReportDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #7c3aed; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 18px; }
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
    .filter-row { display: flex; gap: 10px; align-items: flex-end; margin-bottom: 16px; flex-wrap: wrap; }
    .filter-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 160px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text] { border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px; font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a); }
    input:focus { outline: none; border-color: #7c3aed; box-shadow: 0 0 0 2px rgba(124,58,237,0.15); }
    .ct-section { margin-bottom: 20px; }
    .ct-header { background: var(--uui-color-surface-alt, #f9fafb); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px 8px 0 0; padding: 12px 16px; cursor: pointer; display: flex; align-items: center; justify-content: space-between; }
    .ct-header:hover { background: #f3e8ff; }
    .ct-name { font-weight: 700; font-size: 0.9375rem; }
    .ct-alias { font-family: monospace; font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); }
    .ct-count { font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; border: 1px solid var(--uui-color-border, #e5e7eb); border-top: none; border-radius: 0 0 8px 8px; overflow: hidden; }
    th { text-align: left; padding: 10px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 1px solid var(--uui-color-border, #e5e7eb); font-weight: 600; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #f0f0f0); }
    tr:last-child td { border-bottom: none; }
    .alias-text { font-family: monospace; font-size: 0.8125rem; }
    .type-badge { display: inline-block; padding: 1px 8px; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; background: #f3e8ff; color: #7c3aed; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; }
    .chevron { transition: transform 0.2s; display: inline-block; }
    .chevron.open { transform: rotate(90deg); }
  `;

  static properties = {
    _status: { state: true },
    _report: { state: true },
    _error: { state: true },
    _filter: { state: true },
    _filterAlias: { state: true },
    _filterLoading: { state: true },
    _filteredData: { state: true },
    _expanded: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._report = [];
    this._error = "";
    this._filter = "";
    this._filterAlias = "";
    this._filterLoading = false;
    this._filteredData = null;
    this._expanded = new Set();
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    this._error = "";
    try {
      const r = await fetch(`${API}/GetReport`);
      if (r.ok) {
        this._report = await r.json();
        this._status = "ok";
        this._expanded = new Set(this._report.slice(0, 3).map(ct => ct.alias));
      } else {
        this._status = "err";
        this._error = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._status = "err";
      this._error = e instanceof Error ? e.message : String(e);
    }
  }

  async _filterByAlias() {
    if (!this._filterAlias.trim()) return;
    this._filterLoading = true;
    this._filteredData = null;
    try {
      const r = await fetch(`${API}/GetByContentType?alias=${encodeURIComponent(this._filterAlias.trim())}`);
      if (r.ok) {
        this._filteredData = await r.json();
      }
    } catch (_) {}
    this._filterLoading = false;
  }

  _toggle(alias) {
    if (this._expanded.has(alias)) {
      this._expanded = new Set([...this._expanded].filter(a => a !== alias));
    } else {
      this._expanded = new Set([...this._expanded, alias]);
    }
  }

  _filtered() {
    if (!this._filter.trim()) return this._report;
    const q = this._filter.toLowerCase();
    return this._report.filter(ct =>
      (ct.name || "").toLowerCase().includes(q) ||
      (ct.alias || "").toLowerCase().includes(q) ||
      (ct.properties || []).some(p => (p.alias || "").toLowerCase().includes(q))
    );
  }

  _renderCt(ct) {
    const open = this._expanded.has(ct.alias);
    return html`
      <div class="ct-section">
        <div class="ct-header" @click=${() => this._toggle(ct.alias)}>
          <div>
            <span class="ct-name">${ct.name || ct.alias}</span>
            <span class="ct-alias"> (${ct.alias})</span>
          </div>
          <div style="display:flex;align-items:center;gap:10px">
            <span class="ct-count">${(ct.properties || []).length} properties</span>
            <span class="chevron ${open ? 'open' : ''}">▶</span>
          </div>
        </div>
        ${open ? html`
          <table>
            <thead><tr><th>Name</th><th>Alias</th><th>Property Editor</th><th>Tab</th></tr></thead>
            <tbody>
              ${(ct.properties || []).map(p => html`
                <tr>
                  <td>${p.name || "—"}</td>
                  <td class="alias-text">${p.alias}</td>
                  <td><span class="type-badge">${p.propertyEditorAlias || p.type || "—"}</span></td>
                  <td>${p.tab || "—"}</td>
                </tr>
              `)}
            </tbody>
          </table>
        ` : ""}
      </div>
    `;
  }

  render() {
    const filtered = this._filtered();
    return html`
      <div class="header">
        <div class="logo"><span>📋</span></div>
        <div>
          <h1>Properties Report</h1>
          <p>Inspect all document type properties across the Umbraco content tree</p>
        </div>
        <span class="pill ${this._status === 'ok' ? 'pill-ok' : this._status === 'loading' ? 'pill-checking' : 'pill-err'}">
          <span class="dot"></span>
          ${this._status === 'ok' ? `${this._report.length} content type(s)` : this._status === 'loading' ? 'Loading…' : 'Error'}
        </span>
      </div>

      ${this._status === "err" ? html`<div class="notice err"><strong>Error:</strong> ${this._error}</div>` : ""}

      ${this._status === "ok" ? html`
        <div class="filter-row">
          <div class="filter-col">
            <span class="field-label">Filter by name or alias</span>
            <input type="text" .value=${this._filter} @input=${e => this._filter = e.target.value} placeholder="Search content types…" />
          </div>
          <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
        </div>

        ${filtered.length === 0 ? html`<div class="empty">No content types match the filter.</div>` : filtered.map(ct => this._renderCt(ct))}
      ` : ""}
    `;
  }
}

customElements.define("properties-report-dashboard", PropertiesReportDashboard);
