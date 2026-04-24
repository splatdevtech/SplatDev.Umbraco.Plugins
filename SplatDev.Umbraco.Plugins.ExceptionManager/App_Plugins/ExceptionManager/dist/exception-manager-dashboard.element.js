import { LitElement as d, html as s, css as h, state as c, customElement as p } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as b } from "@umbraco-cms/backoffice/element-api";
var g = Object.defineProperty, f = Object.getOwnPropertyDescriptor, o = (t, e, l, u) => {
  for (var i = u > 1 ? void 0 : u ? f(e, l) : e, n = t.length - 1, r; n >= 0; n--)
    (r = t[n]) && (i = (u ? r(e, l, i) : r(i)) || i);
  return u && i && g(e, l, i), i;
};
let a = class extends b(d) {
  constructor() {
    super(...arguments), this._filter = "", this._loading = !1, this._exceptions = [], this._apiAvailable = !1, this._apiBase = "/umbraco/management/api/v1/exception-manager";
  }
  _handleFilterInput(t) {
    const e = t.target;
    this._filter = e.value;
  }
  async _refresh() {
    if (this._apiAvailable) {
      this._loading = !0;
      try {
        const t = this._filter ? `${this._apiBase}?filter=${encodeURIComponent(this._filter)}` : this._apiBase, e = await fetch(t, {
          headers: { "Content-Type": "application/json" }
        });
        if (e.ok) {
          const l = await e.json();
          this._exceptions = l;
        }
      } catch {
      } finally {
        this._loading = !1;
      }
    }
  }
  get _filteredExceptions() {
    if (!this._filter.trim()) return this._exceptions;
    const t = this._filter.toLowerCase();
    return this._exceptions.filter(
      (e) => e.url.toLowerCase().includes(t) || e.ip.includes(t) || e.message.toLowerCase().includes(t) || String(e.statusCode).includes(t)
    );
  }
  render() {
    const t = this._filteredExceptions;
    return s`
      <div class="dashboard-header">
        <h1>Exception Manager</h1>
        <p>
          View and filter exceptions logged by the application. Drill into
          individual records to inspect request details.
        </p>
      </div>

      <div class="section">
        <uui-box headline="API Status">
          <div class="pending-notice">
            <uui-icon name="alert"></uui-icon>
            <span>
              The exception log API
              (<code>/umbraco/management/api/v1/exception-manager</code>) will
              be available once the <strong>Phase 3 backend</strong> is
              deployed. The table below is currently showing a placeholder
              empty state.
            </span>
          </div>
        </uui-box>
      </div>

      <div class="section">
        <uui-box headline="Exception Log">
          <div class="toolbar">
            <uui-input
              type="search"
              placeholder="Filter by URL, IP, status code, or message…"
              label="Filter exceptions"
              .value=${this._filter}
              @input=${this._handleFilterInput}
            ></uui-input>
            <uui-button
              look="secondary"
              label="Refresh"
              ?disabled=${this._loading || !this._apiAvailable}
              @click=${this._refresh}
            >
              ${this._loading ? "Loading…" : "Refresh"}
            </uui-button>
          </div>

          ${t.length > 0 ? s`
                <uui-table>
                  <uui-table-head>
                    <uui-table-head-cell>URL</uui-table-head-cell>
                    <uui-table-head-cell>IP</uui-table-head-cell>
                    <uui-table-head-cell>Status Code</uui-table-head-cell>
                    <uui-table-head-cell>Date</uui-table-head-cell>
                    <uui-table-head-cell>Message</uui-table-head-cell>
                  </uui-table-head>
                  ${t.map(
      (e) => s`
                      <uui-table-row>
                        <uui-table-cell>${e.url}</uui-table-cell>
                        <uui-table-cell>${e.ip}</uui-table-cell>
                        <uui-table-cell>${e.statusCode}</uui-table-cell>
                        <uui-table-cell>${e.date}</uui-table-cell>
                        <uui-table-cell>${e.message}</uui-table-cell>
                      </uui-table-row>
                    `
    )}
                </uui-table>
              ` : s`
                <uui-table>
                  <uui-table-head>
                    <uui-table-head-cell>URL</uui-table-head-cell>
                    <uui-table-head-cell>IP</uui-table-head-cell>
                    <uui-table-head-cell>Status Code</uui-table-head-cell>
                    <uui-table-head-cell>Date</uui-table-head-cell>
                    <uui-table-head-cell>Message</uui-table-head-cell>
                  </uui-table-head>
                </uui-table>
                <div class="empty-state">
                  <uui-icon name="info"></uui-icon>
                  <p>No exceptions logged.</p>
                </div>
              `}
        </uui-box>
      </div>
    `;
  }
};
a.styles = h`
    :host {
      display: block;
      padding: var(--uui-size-layout-1);
    }

    .dashboard-header {
      margin-bottom: var(--uui-size-layout-2);
    }

    .dashboard-header h1 {
      margin: 0 0 var(--uui-size-4) 0;
      font-size: var(--uui-size-10);
      font-weight: 700;
      color: var(--uui-color-text);
    }

    .dashboard-header p {
      margin: 0;
      color: var(--uui-color-text-alt);
      font-size: var(--uui-size-5);
    }

    .section {
      margin-bottom: var(--uui-size-layout-2);
    }

    .toolbar {
      display: flex;
      gap: var(--uui-size-4);
      align-items: center;
      margin-bottom: var(--uui-size-4);
      flex-wrap: wrap;
    }

    .toolbar uui-input {
      flex: 1;
      min-width: 240px;
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: var(--uui-size-layout-3) var(--uui-size-layout-1);
      color: var(--uui-color-text-alt);
      gap: var(--uui-size-4);
    }

    .empty-state uui-icon {
      font-size: 3rem;
      opacity: 0.4;
    }

    .empty-state p {
      margin: 0;
      font-size: var(--uui-size-5);
    }

    uui-table {
      width: 100%;
    }

    uui-table-head-cell {
      font-weight: 600;
    }

    .pending-notice {
      display: flex;
      align-items: flex-start;
      gap: var(--uui-size-3);
      font-size: var(--uui-size-5);
      color: var(--uui-color-text-alt);
      line-height: 1.6;
    }

    .pending-notice uui-icon {
      flex-shrink: 0;
      margin-top: 2px;
    }
  `;
o([
  c()
], a.prototype, "_filter", 2);
o([
  c()
], a.prototype, "_loading", 2);
o([
  c()
], a.prototype, "_exceptions", 2);
a = o([
  p("exception-manager-dashboard")
], a);
const _ = a;
export {
  a as ExceptionManagerDashboardElement,
  _ as default
};
