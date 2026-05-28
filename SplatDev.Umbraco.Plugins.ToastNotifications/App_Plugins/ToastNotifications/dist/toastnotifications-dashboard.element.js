import { LitElement as p, nothing as u, html as o, css as g, state as n, customElement as h } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as _ } from "@umbraco-cms/backoffice/element-api";
var m = Object.defineProperty, f = Object.getOwnPropertyDescriptor, r = (t, s, e, l) => {
  for (var a = l > 1 ? void 0 : l ? f(s, e) : s, d = t.length - 1, c; d >= 0; d--)
    (c = t[d]) && (a = (l ? c(s, e, a) : c(a)) || a);
  return l && a && m(s, e, a), a;
};
let i = class extends _(p) {
  constructor() {
    super(...arguments), this._toasts = [], this._editing = null, this._creating = !1, this._loading = !1, this._message = null, this._api = "/umbraco/api/toastnotifications";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadToasts();
  }
  async _loadToasts() {
    this._loading = !0;
    try {
      const t = await fetch(`${this._api}/GetActive`);
      this._toasts = await t.json();
    } catch {
      this._toasts = [];
    }
    this._loading = !1;
  }
  _startCreate() {
    this._editing = { title: "", message: "", type: "info", isActive: !0 }, this._creating = !0;
  }
  _startEdit(t) {
    this._editing = { ...t }, this._creating = !1;
  }
  _cancelEdit() {
    this._editing = null, this._creating = !1;
  }
  async _save() {
    if (this._editing) {
      this._loading = !0;
      try {
        const t = this._creating ? "POST" : "PUT", s = this._creating ? `${this._api}/Create` : `${this._api}/Update?id=${this._editing.id}`;
        if (!(await fetch(s, {
          method: t,
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(this._editing)
        })).ok) throw new Error();
        this._message = { type: "success", text: `Toast ${this._creating ? "created" : "updated"}.` }, this._editing = null, this._creating = !1, await this._loadToasts();
      } catch {
        this._message = { type: "error", text: "Save failed." };
      }
      this._loading = !1;
    }
  }
  async _delete(t) {
    if (confirm("Delete this toast?")) {
      this._loading = !0;
      try {
        await fetch(`${this._api}/Delete?id=${t}`, { method: "DELETE" }), this._message = { type: "success", text: "Toast deleted." }, await this._loadToasts();
      } catch {
        this._message = { type: "error", text: "Delete failed." };
      }
      this._loading = !1;
    }
  }
  render() {
    return o`
      <h1>Toast Notifications</h1>
      <p class="description">Manage toast notification messages shown to site visitors.</p>

      ${this._message ? o`<div class="msg ${this._message.type}">${this._message.text}</div>` : u}
      ${this._editing ? this._renderEditor() : u}

      <div class="toolbar">
        <uui-button look="primary" @click=${this._startCreate}>+ Create Toast</uui-button>
      </div>

      ${this._loading ? o`<uui-loader-bar></uui-loader-bar>` : u}

      ${this._toasts.length === 0 && !this._loading ? o`<div class="empty">No active toasts.</div>` : this._toasts.map((t) => o`
            <div class="toast-card">
              <div class="toast-info">
                <h4>${t.title}</h4>
                <p>${t.message} — <em>${t.type}</em></p>
              </div>
              <div style="display:flex;gap:6px;">
                <uui-button look="secondary" compact @click=${() => this._startEdit(t)}>Edit</uui-button>
                <uui-button look="danger" compact @click=${() => this._delete(t.id)}>Delete</uui-button>
              </div>
            </div>
          `)}
    `;
  }
  _renderEditor() {
    const t = this._editing, s = [
      { name: "Info", value: "info", selected: t.type === "info" },
      { name: "Success", value: "success", selected: t.type === "success" },
      { name: "Warning", value: "warning", selected: t.type === "warning" },
      { name: "Error", value: "error", selected: t.type === "error" }
    ];
    return o`
      <div class="edit-form">
        <h3 style="margin:0 0 12px;">${this._creating ? "Create Toast" : "Edit Toast"}</h3>
        <div class="fields">
          <div class="field">
            <label>Title</label>
            <uui-input .value=${t.title} @input=${(e) => {
      this._editing = { ...t, title: e.target.value };
    }}></uui-input>
          </div>
          <div class="field">
            <label>Type</label>
            <uui-select .options=${s} @change=${(e) => {
      this._editing = { ...t, type: e.target.value };
    }}></uui-select>
          </div>
          <div class="field" style="grid-column: 1 / -1;">
            <label>Message</label>
            <uui-input .value=${t.message} @input=${(e) => {
      this._editing = { ...t, message: e.target.value };
    }}></uui-input>
          </div>
        </div>
        <div class="buttons">
          <uui-button look="primary" @click=${this._save}>${this._creating ? "Create" : "Save"}</uui-button>
          <uui-button look="secondary" @click=${this._cancelEdit}>Cancel</uui-button>
        </div>
      </div>
    `;
  }
};
i.styles = g`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 10px; margin-bottom: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .toast-card { padding: 12px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 8px; display: flex; justify-content: space-between; align-items: center; }
    .toast-info h4 { margin: 0 0 4px; font-size: 0.95rem; }
    .toast-info p { margin: 0; font-size: 0.8rem; color: var(--uui-color-text-alt, #6b7280); }
    .edit-form { padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 16px; background: var(--uui-color-surface-alt, #f9fafb); }
    .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 12px; }
    .field { display: flex; flex-direction: column; gap: 4px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .buttons { display: flex; gap: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;
r([
  n()
], i.prototype, "_toasts", 2);
r([
  n()
], i.prototype, "_editing", 2);
r([
  n()
], i.prototype, "_creating", 2);
r([
  n()
], i.prototype, "_loading", 2);
r([
  n()
], i.prototype, "_message", 2);
i = r([
  h("toastnotifications-dashboard")
], i);
const y = i;
export {
  i as ToastNotificationsDashboardElement,
  y as default
};
