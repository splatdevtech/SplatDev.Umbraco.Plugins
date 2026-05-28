import { LitElement as c, nothing as p, html as e, css as m, state as o, customElement as h } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as b } from "@umbraco-cms/backoffice/element-api";
var g = Object.defineProperty, _ = Object.getOwnPropertyDescriptor, l = (t, i, r, u) => {
  for (var a = u > 1 ? void 0 : u ? _(i, r) : i, d = t.length - 1, n; d >= 0; d--)
    (n = t[d]) && (a = (u ? n(i, r, a) : n(a)) || a);
  return u && a && g(i, r, a), a;
};
let s = class extends b(c) {
  constructor() {
    super(...arguments), this._albums = [], this._selectedAlbum = null, this._photos = [], this._editingAlbum = null, this._creatingAlbum = !1, this._loading = !1, this._message = null, this._api = "/umbraco/api/photogallery";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadAlbums();
  }
  async _loadAlbums() {
    this._loading = !0;
    try {
      const t = await fetch(`${this._api}/GetAlbums`);
      this._albums = await t.json();
    } catch {
      this._albums = [];
    }
    this._loading = !1;
  }
  async _selectAlbum(t) {
    this._selectedAlbum = t, this._loading = !0;
    try {
      const i = await fetch(`${this._api}/GetPhotos?albumId=${t.id}`);
      this._photos = await i.json();
    } catch {
      this._photos = [];
    }
    this._loading = !1;
  }
  _backToAlbums() {
    this._selectedAlbum = null, this._photos = [];
  }
  _startCreateAlbum() {
    this._editingAlbum = { title: "", description: "" }, this._creatingAlbum = !0;
  }
  async _saveAlbum() {
    if (this._editingAlbum) {
      this._loading = !0;
      try {
        const t = this._creatingAlbum ? "POST" : "PUT", i = this._creatingAlbum ? "CreateAlbum" : "UpdateAlbum";
        if (!(await fetch(`${this._api}/${i}`, {
          method: t,
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(this._editingAlbum)
        })).ok) throw new Error();
        this._message = { type: "success", text: `Album ${this._creatingAlbum ? "created" : "updated"}.` }, this._editingAlbum = null, this._creatingAlbum = !1, await this._loadAlbums();
      } catch {
        this._message = { type: "error", text: "Save failed." };
      }
      this._loading = !1;
    }
  }
  async _deleteAlbum(t) {
    if (confirm("Delete this album and all its photos?")) {
      this._loading = !0;
      try {
        await fetch(`${this._api}/DeleteAlbum?id=${t}`, { method: "DELETE" }), this._message = { type: "success", text: "Album deleted." }, await this._loadAlbums();
      } catch {
        this._message = { type: "error", text: "Delete failed." };
      }
      this._loading = !1;
    }
  }
  async _deletePhoto(t) {
    this._loading = !0;
    try {
      await fetch(`${this._api}/DeletePhoto?id=${t}`, { method: "DELETE" }), this._photos = this._photos.filter((i) => i.id !== t);
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
    this._loading = !1;
  }
  render() {
    return e`
      <h1>Photo Gallery</h1>
      <p class="description">Manage photo albums and images.</p>

      ${this._message ? e`<div class="msg ${this._message.type}">${this._message.text}</div>` : p}
      ${this._loading ? e`<uui-loader-bar></uui-loader-bar>` : p}

      ${this._editingAlbum ? e`
        <div class="edit-form">
          <h3 style="margin:0 0 12px;">${this._creatingAlbum ? "Create Album" : "Edit Album"}</h3>
          <div class="fields">
            <div class="field">
              <label>Title</label>
              <uui-input .value=${this._editingAlbum.title} @input=${(t) => {
      this._editingAlbum = { ...this._editingAlbum, title: t.target.value };
    }}></uui-input>
            </div>
            <div class="field">
              <label>Description</label>
              <uui-input .value=${this._editingAlbum.description} @input=${(t) => {
      this._editingAlbum = { ...this._editingAlbum, description: t.target.value };
    }}></uui-input>
            </div>
          </div>
          <div class="buttons">
            <uui-button look="primary" @click=${this._saveAlbum}>${this._creatingAlbum ? "Create" : "Save"}</uui-button>
            <uui-button look="secondary" @click=${() => {
      this._editingAlbum = null, this._creatingAlbum = !1;
    }}>Cancel</uui-button>
          </div>
        </div>
      ` : p}

      ${this._selectedAlbum ? this._renderPhotos() : this._renderAlbums()}
    `;
  }
  _renderAlbums() {
    return e`
      <div class="toolbar">
        <uui-button look="primary" @click=${this._startCreateAlbum}>+ Create Album</uui-button>
      </div>
      ${this._albums.length === 0 && !this._loading ? e`<div class="empty">No albums yet.</div>` : e`
            <div class="album-grid">
              ${this._albums.map((t) => e`
                <div class="album-card" @click=${() => this._selectAlbum(t)}>
                  <h3>${t.title}</h3>
                  <p>${t.description}</p>
                  <div style="margin-top:8px;display:flex;gap:4px;">
                    <uui-button look="secondary" compact @click=${(i) => {
      i.stopPropagation(), this._editingAlbum = { ...t }, this._creatingAlbum = !1;
    }}>Edit</uui-button>
                    <uui-button look="danger" compact @click=${(i) => {
      i.stopPropagation(), this._deleteAlbum(t.id);
    }}>Delete</uui-button>
                  </div>
                </div>
              `)}
            </div>
          `}
    `;
  }
  _renderPhotos() {
    const t = this._selectedAlbum;
    return e`
      <uui-button class="back-btn" look="secondary" @click=${this._backToAlbums}>← Back to Albums</uui-button>
      <h2 style="margin:0 0 12px;">${t.title}</h2>
      ${this._photos.length === 0 ? e`<div class="empty">No photos in this album.</div>` : e`
            <div class="photo-grid">
              ${this._photos.map((i) => e`
                <div class="photo-item">
                  <img src="${i.mediaUrl}" alt="${i.caption}" />
                  <div class="caption">${i.caption}</div>
                  <uui-button class="remove" look="danger" compact @click=${() => this._deletePhoto(i.id)}>
                    <uui-icon name="icon-delete"></uui-icon>
                  </uui-button>
                </div>
              `)}
            </div>
          `}
    `;
  }
};
s.styles = m`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 10px; margin-bottom: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .album-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 12px; }
    .album-card { padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; cursor: pointer; }
    .album-card:hover { background: var(--uui-color-surface-alt, #f9fafb); }
    .album-card h3 { margin: 0 0 4px; font-size: 0.95rem; }
    .album-card p { margin: 0; font-size: 0.8rem; color: var(--uui-color-text-alt, #6b7280); }
    .photo-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); gap: 8px; margin-top: 16px; }
    .photo-item { border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 4px; overflow: hidden; position: relative; }
    .photo-item img { width: 100%; height: 120px; object-fit: cover; display: block; }
    .photo-item .caption { padding: 4px 8px; font-size: 0.75rem; }
    .photo-item .remove { position: absolute; top: 4px; right: 4px; }
    .edit-form { padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 16px; background: var(--uui-color-surface-alt, #f9fafb); }
    .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 12px; }
    .field { display: flex; flex-direction: column; gap: 4px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .buttons { display: flex; gap: 8px; }
    .back-btn { margin-bottom: 16px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;
l([
  o()
], s.prototype, "_albums", 2);
l([
  o()
], s.prototype, "_selectedAlbum", 2);
l([
  o()
], s.prototype, "_photos", 2);
l([
  o()
], s.prototype, "_editingAlbum", 2);
l([
  o()
], s.prototype, "_creatingAlbum", 2);
l([
  o()
], s.prototype, "_loading", 2);
l([
  o()
], s.prototype, "_message", 2);
s = l([
  h("photogallery-dashboard")
], s);
const x = s;
export {
  s as PhotoGalleryDashboardElement,
  x as default
};
