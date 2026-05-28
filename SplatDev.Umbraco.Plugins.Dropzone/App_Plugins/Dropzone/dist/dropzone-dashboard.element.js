import { LitElement as m, nothing as c, html as r, css as u, state as l, customElement as h } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as g } from "@umbraco-cms/backoffice/element-api";
var _ = Object.defineProperty, f = Object.getOwnPropertyDescriptor, d = (e, a, t, o) => {
  for (var s = o > 1 ? void 0 : o ? f(a, t) : a, n = e.length - 1, p; n >= 0; n--)
    (p = e[n]) && (s = (o ? p(a, t, s) : p(s)) || s);
  return o && s && _(a, t, s), s;
};
let i = class extends g(m) {
  constructor() {
    super(...arguments), this._items = [], this._loading = !1, this._uploading = !1, this._dragOver = !1, this._folderName = "", this._message = null, this._api = "/umbraco/api/dropzone";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadMedia();
  }
  async _loadMedia() {
    this._loading = !0;
    try {
      const e = await fetch(`${this._api}/GetMedia`);
      this._items = await e.json();
    } catch {
      this._items = [];
    }
    this._loading = !1;
  }
  _handleDragOver(e) {
    e.preventDefault(), this._dragOver = !0;
  }
  _handleDragLeave() {
    this._dragOver = !1;
  }
  _handleDrop(e) {
    var t;
    e.preventDefault(), this._dragOver = !1;
    const a = (t = e.dataTransfer) == null ? void 0 : t.files;
    a != null && a.length && this._uploadFile(a[0]);
  }
  _handleFileInput() {
    const e = document.createElement("input");
    e.type = "file", e.onchange = () => {
      var a;
      (a = e.files) != null && a.length && this._uploadFile(e.files[0]);
    }, e.click();
  }
  async _uploadFile(e) {
    this._uploading = !0, this._message = null;
    try {
      const a = new FormData();
      a.append("file", e), this._folderName && a.append("folderName", this._folderName);
      const t = await fetch(`${this._api}/Upload`, { method: "POST", body: a }), o = await t.json();
      t.ok && o.success ? (this._message = { type: "success", text: `'${e.name}' uploaded.` }, await this._loadMedia()) : this._message = { type: "error", text: o.message ?? "Upload failed." };
    } catch {
      this._message = { type: "error", text: "Upload failed." };
    }
    this._uploading = !1;
  }
  async _deleteMedia(e) {
    if (confirm("Delete this media item?"))
      try {
        (await fetch(`${this._api}/Delete?mediaKey=${e}`, { method: "DELETE" })).ok && (this._items = this._items.filter((t) => t.key !== e), this._message = { type: "success", text: "Media deleted." });
      } catch {
        this._message = { type: "error", text: "Delete failed." };
      }
  }
  render() {
    return r`
      <h1>Dropzone</h1>
      <p class="description">Upload and manage media files via drag & drop.</p>

      ${this._message ? r`<div class="msg ${this._message.type}">${this._message.text}</div>` : c}

      <div class="upload-zone ${this._dragOver ? "drag-over" : ""}"
        @dragover=${this._handleDragOver}
        @dragleave=${this._handleDragLeave}
        @drop=${this._handleDrop}
        @click=${this._handleFileInput}>
        ${this._uploading ? r`<uui-loader-bar></uui-loader-bar>` : r`<p>Drop files here or click to upload</p>`}
      </div>

      <div class="toolbar">
        <uui-input placeholder="Folder name (optional)" .value=${this._folderName}
          @input=${(e) => this._folderName = e.target.value}></uui-input>
      </div>

      ${this._loading ? r`<uui-loader-bar></uui-loader-bar>` : c}

      ${this._items.length === 0 && !this._loading ? r`<div class="empty">No media items.</div>` : r`
            <div class="media-grid">
              ${this._items.map((e) => r`
                <div class="media-card">
                  <div class="name">${e.name}</div>
                  <div class="type">${e.contentType}</div>
                  <uui-button look="danger" compact @click=${() => this._deleteMedia(e.key)}>Delete</uui-button>
                </div>
              `)}
            </div>
          `}
    `;
  }
};
i.styles = u`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .upload-zone {
      border: 2px dashed var(--uui-color-border, #d1d5db);
      border-radius: 8px; padding: 32px; text-align: center;
      cursor: pointer; margin-bottom: 24px;
      transition: border-color 0.2s, background 0.2s;
    }
    .upload-zone:hover, .upload-zone.drag-over {
      border-color: var(--uui-color-interactive, #3b82f6);
      background: rgba(59, 130, 246, 0.04);
    }
    .upload-zone p { margin: 0; color: var(--uui-color-text-alt, #6b7280); }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .toolbar { display: flex; gap: 10px; align-items: center; margin-bottom: 16px; }
    .media-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 12px; }
    .media-card { padding: 12px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; text-align: center; }
    .media-card .name { font-weight: 500; font-size: 0.85rem; margin-bottom: 4px; word-break: break-word; }
    .media-card .type { font-size: 0.75rem; color: var(--uui-color-text-alt, #6b7280); margin-bottom: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;
d([
  l()
], i.prototype, "_items", 2);
d([
  l()
], i.prototype, "_loading", 2);
d([
  l()
], i.prototype, "_uploading", 2);
d([
  l()
], i.prototype, "_dragOver", 2);
d([
  l()
], i.prototype, "_folderName", 2);
d([
  l()
], i.prototype, "_message", 2);
i = d([
  h("dropzone-dashboard")
], i);
const x = i;
export {
  i as DropzoneDashboardElement,
  x as default
};
