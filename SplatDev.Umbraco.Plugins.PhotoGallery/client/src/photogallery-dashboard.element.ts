import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface Album {
  id?: number;
  title: string;
  description: string;
}

interface Photo {
  id?: number;
  albumId: number;
  mediaUrl: string;
  caption: string;
  sortOrder: number;
}

@customElement("photogallery-dashboard")
export class PhotoGalleryDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
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

  @state() private _albums: Album[] = [];
  @state() private _selectedAlbum: Album | null = null;
  @state() private _photos: Photo[] = [];
  @state() private _editingAlbum: Album | null = null;
  @state() private _creatingAlbum = false;
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/photogallery";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadAlbums();
  }

  private async _loadAlbums(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetAlbums`);
      this._albums = await r.json();
    } catch { this._albums = []; }
    this._loading = false;
  }

  private async _selectAlbum(album: Album): Promise<void> {
    this._selectedAlbum = album;
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetPhotos?albumId=${album.id}`);
      this._photos = await r.json();
    } catch { this._photos = []; }
    this._loading = false;
  }

  private _backToAlbums(): void {
    this._selectedAlbum = null;
    this._photos = [];
  }

  private _startCreateAlbum(): void {
    this._editingAlbum = { title: "", description: "" };
    this._creatingAlbum = true;
  }

  private async _saveAlbum(): Promise<void> {
    if (!this._editingAlbum) return;
    this._loading = true;
    try {
      const method = this._creatingAlbum ? "POST" : "PUT";
      const action = this._creatingAlbum ? "CreateAlbum" : "UpdateAlbum";
      const r = await fetch(`${this._api}/${action}`, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._editingAlbum),
      });
      if (!r.ok) throw new Error();
      this._message = { type: "success", text: `Album ${this._creatingAlbum ? "created" : "updated"}.` };
      this._editingAlbum = null;
      this._creatingAlbum = false;
      await this._loadAlbums();
    } catch {
      this._message = { type: "error", text: "Save failed." };
    }
    this._loading = false;
  }

  private async _deleteAlbum(id: number): Promise<void> {
    if (!confirm("Delete this album and all its photos?")) return;
    this._loading = true;
    try {
      await fetch(`${this._api}/DeleteAlbum?id=${id}`, { method: "DELETE" });
      this._message = { type: "success", text: "Album deleted." };
      await this._loadAlbums();
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
    this._loading = false;
  }

  private async _deletePhoto(id: number): Promise<void> {
    this._loading = true;
    try {
      await fetch(`${this._api}/DeletePhoto?id=${id}`, { method: "DELETE" });
      this._photos = this._photos.filter((p) => p.id !== id);
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
    this._loading = false;
  }

  override render() {
    return html`
      <h1>Photo Gallery</h1>
      <p class="description">Manage photo albums and images.</p>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}
      ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}

      ${this._editingAlbum ? html`
        <div class="edit-form">
          <h3 style="margin:0 0 12px;">${this._creatingAlbum ? "Create Album" : "Edit Album"}</h3>
          <div class="fields">
            <div class="field">
              <label>Title</label>
              <uui-input .value=${this._editingAlbum.title} @input=${(e: InputEvent) => { this._editingAlbum = { ...this._editingAlbum!, title: (e.target as HTMLInputElement).value }; }}></uui-input>
            </div>
            <div class="field">
              <label>Description</label>
              <uui-input .value=${this._editingAlbum.description} @input=${(e: InputEvent) => { this._editingAlbum = { ...this._editingAlbum!, description: (e.target as HTMLInputElement).value }; }}></uui-input>
            </div>
          </div>
          <div class="buttons">
            <uui-button look="primary" @click=${this._saveAlbum}>${this._creatingAlbum ? "Create" : "Save"}</uui-button>
            <uui-button look="secondary" @click=${() => { this._editingAlbum = null; this._creatingAlbum = false; }}>Cancel</uui-button>
          </div>
        </div>
      ` : nothing}

      ${this._selectedAlbum ? this._renderPhotos() : this._renderAlbums()}
    `;
  }

  private _renderAlbums() {
    return html`
      <div class="toolbar">
        <uui-button look="primary" @click=${this._startCreateAlbum}>+ Create Album</uui-button>
      </div>
      ${this._albums.length === 0 && !this._loading
        ? html`<div class="empty">No albums yet.</div>`
        : html`
            <div class="album-grid">
              ${this._albums.map((a) => html`
                <div class="album-card" @click=${() => this._selectAlbum(a)}>
                  <h3>${a.title}</h3>
                  <p>${a.description}</p>
                  <div style="margin-top:8px;display:flex;gap:4px;">
                    <uui-button look="secondary" compact @click=${(e: Event) => { e.stopPropagation(); this._editingAlbum = { ...a }; this._creatingAlbum = false; }}>Edit</uui-button>
                    <uui-button look="danger" compact @click=${(e: Event) => { e.stopPropagation(); this._deleteAlbum(a.id!); }}>Delete</uui-button>
                  </div>
                </div>
              `)}
            </div>
          `}
    `;
  }

  private _renderPhotos() {
    const album = this._selectedAlbum!;
    return html`
      <uui-button class="back-btn" look="secondary" @click=${this._backToAlbums}>← Back to Albums</uui-button>
      <h2 style="margin:0 0 12px;">${album.title}</h2>
      ${this._photos.length === 0
        ? html`<div class="empty">No photos in this album.</div>`
        : html`
            <div class="photo-grid">
              ${this._photos.map((p) => html`
                <div class="photo-item">
                  <img src="${p.mediaUrl}" alt="${p.caption}" />
                  <div class="caption">${p.caption}</div>
                  <uui-button class="remove" look="danger" compact @click=${() => this._deletePhoto(p.id!)}>
                    <uui-icon name="icon-delete"></uui-icon>
                  </uui-button>
                </div>
              `)}
            </div>
          `}
    `;
  }
}

export default PhotoGalleryDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "photogallery-dashboard": PhotoGalleryDashboardElement;
  }
}
