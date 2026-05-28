import { LitElement as g, nothing as c, html as o, css as _, state as u, customElement as f } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as m } from "@umbraco-cms/backoffice/element-api";
var b = Object.defineProperty, y = Object.getOwnPropertyDescriptor, l = (t, e, a, r) => {
  for (var s = r > 1 ? void 0 : r ? y(e, a) : e, n = t.length - 1, d; n >= 0; n--)
    (d = t[n]) && (s = (r ? d(e, a, s) : d(s)) || s);
  return r && s && b(e, a, s), s;
};
let i = class extends m(g) {
  constructor() {
    super(...arguments), this._items = [], this._filter = "", this._loading = !1, this._message = null, this._editing = null, this._creating = !1, this._api = "/umbraco/api/dictionarymanager";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadAll();
  }
  async _loadAll() {
    this._loading = !0;
    try {
      const t = await fetch(`${this._api}/GetAll`);
      this._items = await t.json();
    } catch {
      this._items = [], this._message = { type: "error", text: "Failed to load dictionary items." };
    }
    this._loading = !1;
  }
  _startCreate() {
    this._editing = { key: "", parentKey: null, value: "", languageCode: "", translations: {} }, this._creating = !0;
  }
  _startEdit(t) {
    this._editing = { ...t, translations: { ...t.translations } }, this._creating = !1;
  }
  _cancelEdit() {
    this._editing = null, this._creating = !1;
  }
  async _save() {
    if (this._editing) {
      this._loading = !0, this._message = null;
      try {
        const t = this._creating ? "POST" : "PUT", e = this._creating ? "Create" : "Update", a = await fetch(`${this._api}/${e}`, {
          method: t,
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(this._editing)
        });
        if (!a.ok) {
          const r = await a.text();
          throw new Error(r);
        }
        this._message = { type: "success", text: `Item ${this._creating ? "created" : "updated"} successfully.` }, this._editing = null, this._creating = !1, await this._loadAll();
      } catch (t) {
        this._message = { type: "error", text: `Save failed: ${t.message}` };
      }
      this._loading = !1;
    }
  }
  async _delete(t) {
    if (confirm(`Delete dictionary item '${t}'?`)) {
      this._loading = !0;
      try {
        if (!(await fetch(`${this._api}/Delete?key=${encodeURIComponent(t)}`, { method: "DELETE" })).ok) throw new Error();
        this._message = { type: "success", text: `'${t}' deleted.` }, await this._loadAll();
      } catch {
        this._message = { type: "error", text: "Delete failed." };
      }
      this._loading = !1;
    }
  }
  async _export() {
    window.open(`${this._api}/Export`, "_blank");
  }
  async _import() {
    const t = document.createElement("input");
    t.type = "file", t.accept = ".json", t.onchange = async () => {
      var a;
      const e = (a = t.files) == null ? void 0 : a[0];
      if (e) {
        this._loading = !0;
        try {
          const r = await e.text(), s = JSON.parse(r), n = await fetch(`${this._api}/Import?overrideExisting=true`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(s)
          });
          if (!n.ok) throw new Error();
          const d = await n.json(), p = Array.isArray(d) ? d.filter((h) => h.success).length : 0;
          this._message = { type: "success", text: `Imported ${p} item(s).` }, await this._loadAll();
        } catch {
          this._message = { type: "error", text: "Import failed." };
        }
        this._loading = !1;
      }
    }, t.click();
  }
  get _filtered() {
    if (!this._filter) return this._items;
    const t = this._filter.toLowerCase();
    return this._items.filter(
      (e) => e.key.toLowerCase().includes(t) || e.value.toLowerCase().includes(t)
    );
  }
  render() {
    return o`
      <h1>Dictionary Manager</h1>
      <p class="description">Import, export and manage Umbraco dictionary items.</p>

      ${this._message ? o`<div class="msg ${this._message.type}">${this._message.text}</div>` : c}

      ${this._editing ? this._renderEditor() : c}

      <div class="toolbar">
        <uui-input placeholder="Filter by key or value…" style="flex:1;min-width:200px;"
          @input=${(t) => this._filter = t.target.value}></uui-input>
        <uui-button look="primary" @click=${this._startCreate} ?disabled=${this._loading}>+ Create</uui-button>
        <uui-button look="secondary" @click=${this._export} ?disabled=${this._loading}>Export JSON</uui-button>
        <uui-button look="secondary" @click=${this._import} ?disabled=${this._loading}>Import JSON</uui-button>
      </div>

      ${this._loading ? o`<uui-loader-bar></uui-loader-bar>` : c}

      ${this._filtered.length === 0 && !this._loading ? o`<div class="empty">No dictionary items found.</div>` : o`
            <table>
              <thead>
                <tr><th>Key</th><th>Value</th><th>Language</th><th>Translations</th><th></th></tr>
              </thead>
              <tbody>
                ${this._filtered.map(
      (t) => o`
                    <tr>
                      <td>${t.key}</td>
                      <td>${t.value}</td>
                      <td>${t.languageCode}</td>
                      <td class="translations">${Object.keys(t.translations).join(", ") || "—"}</td>
                      <td style="white-space:nowrap;">
                        <uui-button look="secondary" compact @click=${() => this._startEdit(t)}>Edit</uui-button>
                        <uui-button look="danger" compact @click=${() => this._delete(t.key)}>Delete</uui-button>
                      </td>
                    </tr>
                  `
    )}
              </tbody>
            </table>
          `}
    `;
  }
  _renderEditor() {
    const t = this._editing;
    return o`
      <div class="edit-form">
        <h3 style="margin:0 0 12px;">${this._creating ? "Create Item" : `Edit: ${t.key}`}</h3>
        <div class="fields">
          <div class="field">
            <label>Key</label>
            <uui-input .value=${t.key} ?disabled=${!this._creating}
              @input=${(e) => {
      this._editing = { ...t, key: e.target.value };
    }}></uui-input>
          </div>
          <div class="field">
            <label>Value</label>
            <uui-input .value=${t.value}
              @input=${(e) => {
      this._editing = { ...t, value: e.target.value };
    }}></uui-input>
          </div>
          <div class="field">
            <label>Language Code</label>
            <uui-input .value=${t.languageCode}
              @input=${(e) => {
      this._editing = { ...t, languageCode: e.target.value };
    }}></uui-input>
          </div>
          <div class="field">
            <label>Parent Key</label>
            <uui-input .value=${t.parentKey ?? ""}
              @input=${(e) => {
      this._editing = { ...t, parentKey: e.target.value || null };
    }}></uui-input>
          </div>
        </div>
        <div class="buttons">
          <uui-button look="primary" @click=${this._save} ?disabled=${this._loading}>
            ${this._creating ? "Create" : "Save"}
          </uui-button>
          <uui-button look="secondary" @click=${this._cancelEdit}>Cancel</uui-button>
        </div>
      </div>
    `;
  }
};
i.styles = _`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-bottom: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
    th { text-align: left; padding: 8px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
    tr:hover td { background: var(--uui-color-surface-alt, #f9fafb); }
    .translations { font-size: 0.75rem; color: var(--uui-color-text-alt, #6b7280); }
    .edit-form { padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 16px; background: var(--uui-color-surface-alt, #f9fafb); }
    .edit-form .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 12px; }
    .edit-form .field { display: flex; flex-direction: column; gap: 4px; }
    .edit-form .field label { font-size: 0.8rem; font-weight: 500; }
    .edit-form .buttons { display: flex; gap: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;
l([
  u()
], i.prototype, "_items", 2);
l([
  u()
], i.prototype, "_filter", 2);
l([
  u()
], i.prototype, "_loading", 2);
l([
  u()
], i.prototype, "_message", 2);
l([
  u()
], i.prototype, "_editing", 2);
l([
  u()
], i.prototype, "_creating", 2);
i = l([
  f("dictionarymanager-dashboard")
], i);
const $ = i;
export {
  i as DictionaryManagerDashboardElement,
  $ as default
};
