import { LitElement as n, html as a, css as m, state as s, customElement as _ } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as p } from "@umbraco-cms/backoffice/element-api";
var f = Object.defineProperty, b = Object.getOwnPropertyDescriptor, r = (e, i, u, o) => {
  for (var l = o > 1 ? void 0 : o ? b(i, u) : i, c = e.length - 1, h; c >= 0; c--)
    (h = e[c]) && (l = (o ? h(i, u, l) : h(l)) || l);
  return o && l && f(i, u, l), l;
};
const d = "/umbraco/backoffice/api/RedirectManager";
let t = class extends p(n) {
  constructor() {
    super(...arguments), this._redirects = [], this._loading = !1, this._message = "", this._showForm = !1, this._editItem = null, this._formUrl = "", this._formRedirectTo = "", this._filter = "";
  }
  connectedCallback() {
    super.connectedCallback(), this._load();
  }
  async _load() {
    this._loading = !0;
    try {
      const e = await fetch(`${d}/GetAll`);
      e.ok && (this._redirects = await e.json());
    } catch {
      this._redirects = [];
    }
    this._loading = !1;
  }
  _openCreate() {
    this._editItem = null, this._formUrl = "", this._formRedirectTo = "", this._showForm = !0, this._message = "";
  }
  _openEdit(e) {
    this._editItem = e, this._formUrl = e.url, this._formRedirectTo = e.redirectToUrl, this._showForm = !0, this._message = "";
  }
  _cancelForm() {
    this._showForm = !1, this._editItem = null, this._formUrl = "", this._formRedirectTo = "";
  }
  async _save() {
    var i;
    if (!this._formUrl.trim() || !this._formRedirectTo.trim()) {
      this._message = "Both URL and Redirect To are required.";
      return;
    }
    const e = {
      id: ((i = this._editItem) == null ? void 0 : i.id) ?? 0,
      url: this._formUrl.trim(),
      redirectToUrl: this._formRedirectTo.trim()
    };
    try {
      this._editItem ? (await fetch(`${d}/Put`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(e)
      }), this._message = "Redirect updated.") : (await fetch(`${d}/Post`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(e)
      }), this._message = "Redirect created."), this._showForm = !1, this._editItem = null, await this._load();
    } catch {
      this._message = "Error saving redirect.";
    }
  }
  async _delete(e) {
    if (confirm("Delete this redirect?"))
      try {
        await fetch(`${d}/Delete?id=${e}`, { method: "DELETE" }), await this._load(), this._message = "Redirect deleted.";
      } catch {
        this._message = "Error deleting redirect.";
      }
  }
  get _filtered() {
    if (!this._filter) return this._redirects;
    const e = this._filter.toLowerCase();
    return this._redirects.filter(
      (i) => i.url.toLowerCase().includes(e) || i.redirectToUrl.toLowerCase().includes(e)
    );
  }
  _renderForm() {
    return a`
      <uui-box headline=${this._editItem ? "Edit Redirect" : "New Redirect"}>
        <div class="form">
          <div class="field">
            <label>From URL</label>
            <uui-input
              .value=${this._formUrl}
              placeholder="/old-path"
              @input=${(e) => this._formUrl = e.target.value}
            ></uui-input>
          </div>
          <div class="field">
            <label>Redirect To</label>
            <uui-input
              .value=${this._formRedirectTo}
              placeholder="/new-path"
              @input=${(e) => this._formRedirectTo = e.target.value}
            ></uui-input>
          </div>
          ${this._message ? a`<div class="message error">${this._message}</div>` : ""}
          <div class="form-actions">
            <uui-button look="primary" label="Save" @click=${this._save}>Save</uui-button>
            <uui-button look="secondary" label="Cancel" @click=${this._cancelForm}>Cancel</uui-button>
          </div>
        </div>
      </uui-box>
    `;
  }
  render() {
    return a`
      <div class="dashboard">
        <div class="header">
          <div>
            <h1>Redirect Manager</h1>
            <p>Manage URL redirects for your Umbraco site.</p>
          </div>
          <uui-button
            look="primary"
            label="Add Redirect"
            @click=${this._openCreate}
          >+ Add Redirect</uui-button>
        </div>

        ${this._message && !this._showForm ? a`<div class="message success">${this._message}</div>` : ""}

        ${this._showForm ? this._renderForm() : ""}

        <uui-box headline="Redirects (${this._filtered.length})">
          <div slot="header" class="search-wrap">
            <uui-input
              placeholder="Search..."
              .value=${this._filter}
              @input=${(e) => this._filter = e.target.value}
            ></uui-input>
          </div>

          ${this._loading ? a`<div class="loading">Loading...</div>` : this._filtered.length === 0 ? a`<p class="empty">No redirects found.</p>` : a`
              <uui-table>
                <uui-table-head>
                  <uui-table-head-cell>From URL</uui-table-head-cell>
                  <uui-table-head-cell>Redirect To</uui-table-head-cell>
                  <uui-table-head-cell>Created</uui-table-head-cell>
                  <uui-table-head-cell></uui-table-head-cell>
                </uui-table-head>
                ${this._filtered.map(
      (e) => a`
                    <uui-table-row>
                      <uui-table-cell>${e.url}</uui-table-cell>
                      <uui-table-cell>${e.redirectToUrl}</uui-table-cell>
                      <uui-table-cell>
                        ${e.createdOn ? new Date(e.createdOn).toLocaleDateString() : "—"}
                      </uui-table-cell>
                      <uui-table-cell class="actions-cell">
                        <uui-button
                          compact
                          look="secondary"
                          label="Edit"
                          @click=${() => this._openEdit(e)}
                        >Edit</uui-button>
                        <uui-button
                          compact
                          look="danger"
                          label="Delete"
                          @click=${() => this._delete(e.id)}
                        >Delete</uui-button>
                      </uui-table-cell>
                    </uui-table-row>
                  `
    )}
              </uui-table>
            `}
        </uui-box>
      </div>
    `;
  }
};
t.styles = m`
    :host {
      display: block;
      padding: var(--uui-size-layout-1);
    }
    .dashboard {
      max-width: 1200px;
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-space-5);
    }
    .header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      gap: var(--uui-size-space-3);
    }
    .header h1 {
      margin: 0 0 var(--uui-size-2) 0;
      font-size: 1.5rem;
    }
    .header p {
      margin: 0;
      color: var(--uui-color-text-alt);
    }
    .form {
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-space-3);
      max-width: 600px;
    }
    .field {
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-2);
    }
    .field label {
      font-weight: 600;
      font-size: 0.875rem;
    }
    .field uui-input {
      width: 100%;
    }
    .form-actions {
      display: flex;
      gap: var(--uui-size-space-3);
    }
    .message {
      padding: var(--uui-size-space-3);
      border-radius: var(--uui-border-radius);
    }
    .message.success {
      background: var(--uui-color-positive-standalone);
      color: white;
    }
    .message.error {
      background: var(--uui-color-danger-standalone);
      color: white;
    }
    .search-wrap {
      margin-bottom: var(--uui-size-space-3);
    }
    .loading,
    .empty {
      color: var(--uui-color-text-alt);
      font-style: italic;
    }
    .actions-cell {
      display: flex;
      gap: var(--uui-size-2);
    }
    uui-table {
      width: 100%;
    }
  `;
r([
  s()
], t.prototype, "_redirects", 2);
r([
  s()
], t.prototype, "_loading", 2);
r([
  s()
], t.prototype, "_message", 2);
r([
  s()
], t.prototype, "_showForm", 2);
r([
  s()
], t.prototype, "_editItem", 2);
r([
  s()
], t.prototype, "_formUrl", 2);
r([
  s()
], t.prototype, "_formRedirectTo", 2);
r([
  s()
], t.prototype, "_filter", 2);
t = r([
  _("redirect-manager-dashboard")
], t);
export {
  t as RedirectManagerDashboardElement
};
//# sourceMappingURL=redirect-manager.js.map
