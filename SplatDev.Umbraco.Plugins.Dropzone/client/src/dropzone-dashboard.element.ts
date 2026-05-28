import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface MediaItem {
  id: number;
  key: string;
  name: string;
  contentType: string;
}

@customElement("dropzone-dashboard")
export class DropzoneDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
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

  @state() private _items: MediaItem[] = [];
  @state() private _loading = false;
  @state() private _uploading = false;
  @state() private _dragOver = false;
  @state() private _folderName = "";
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/dropzone";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadMedia();
  }

  private async _loadMedia(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetMedia`);
      this._items = await r.json();
    } catch { this._items = []; }
    this._loading = false;
  }

  private _handleDragOver(e: DragEvent): void {
    e.preventDefault();
    this._dragOver = true;
  }

  private _handleDragLeave(): void {
    this._dragOver = false;
  }

  private _handleDrop(e: DragEvent): void {
    e.preventDefault();
    this._dragOver = false;
    const files = e.dataTransfer?.files;
    if (files?.length) this._uploadFile(files[0]);
  }

  private _handleFileInput(): void {
    const input = document.createElement("input");
    input.type = "file";
    input.onchange = () => {
      if (input.files?.length) this._uploadFile(input.files[0]);
    };
    input.click();
  }

  private async _uploadFile(file: File): Promise<void> {
    this._uploading = true;
    this._message = null;
    try {
      const form = new FormData();
      form.append("file", file);
      if (this._folderName) form.append("folderName", this._folderName);

      const r = await fetch(`${this._api}/Upload`, { method: "POST", body: form });
      const d = await r.json();
      if (r.ok && d.success) {
        this._message = { type: "success", text: `'${file.name}' uploaded.` };
        await this._loadMedia();
      } else {
        this._message = { type: "error", text: d.message ?? "Upload failed." };
      }
    } catch {
      this._message = { type: "error", text: "Upload failed." };
    }
    this._uploading = false;
  }

  private async _deleteMedia(key: string): Promise<void> {
    if (!confirm("Delete this media item?")) return;
    try {
      const r = await fetch(`${this._api}/Delete?mediaKey=${key}`, { method: "DELETE" });
      if (r.ok) {
        this._items = this._items.filter((i) => i.key !== key);
        this._message = { type: "success", text: "Media deleted." };
      }
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
  }

  override render() {
    return html`
      <h1>Dropzone</h1>
      <p class="description">Upload and manage media files via drag & drop.</p>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}

      <div class="upload-zone ${this._dragOver ? "drag-over" : ""}"
        @dragover=${this._handleDragOver}
        @dragleave=${this._handleDragLeave}
        @drop=${this._handleDrop}
        @click=${this._handleFileInput}>
        ${this._uploading
          ? html`<uui-loader-bar></uui-loader-bar>`
          : html`<p>Drop files here or click to upload</p>`}
      </div>

      <div class="toolbar">
        <uui-input placeholder="Folder name (optional)" .value=${this._folderName}
          @input=${(e: InputEvent) => (this._folderName = (e.target as HTMLInputElement).value)}></uui-input>
      </div>

      ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}

      ${this._items.length === 0 && !this._loading
        ? html`<div class="empty">No media items.</div>`
        : html`
            <div class="media-grid">
              ${this._items.map((item) => html`
                <div class="media-card">
                  <div class="name">${item.name}</div>
                  <div class="type">${item.contentType}</div>
                  <uui-button look="danger" compact @click=${() => this._deleteMedia(item.key)}>Delete</uui-button>
                </div>
              `)}
            </div>
          `}
    `;
  }
}

export default DropzoneDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "dropzone-dashboard": DropzoneDashboardElement;
  }
}
