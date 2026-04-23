import { LitElement as n, html as l, nothing as p, css as b, state as u, customElement as h } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as _ } from "@umbraco-cms/backoffice/element-api";
var g = Object.defineProperty, f = Object.getOwnPropertyDescriptor, s = (e, t, i, r) => {
  for (var o = r > 1 ? void 0 : r ? f(t, i) : t, c = e.length - 1, d; c >= 0; c--)
    (d = e[c]) && (o = (r ? d(t, i, o) : d(o)) || o);
  return r && o && g(t, i, o), o;
};
let a = class extends _(n) {
  constructor() {
    super(...arguments), this._activeTab = "library", this._books = [], this._loadingBooks = !1, this._selectedFile = null, this._uploading = !1, this._uploadMessage = "", this._uploadSuccess = !1;
  }
  connectedCallback() {
    super.connectedCallback(), this._fetchBooks();
  }
  async _fetchBooks() {
    this._loadingBooks = !0;
    try {
      const e = await fetch("/umbraco/api/ManagerApi/GetAllAsync");
      if (!e.ok) throw new Error(`HTTP ${e.status}`);
      this._books = await e.json();
    } catch {
      this._books = [];
    } finally {
      this._loadingBooks = !1;
    }
  }
  async _deleteBook(e) {
    if (confirm("Are you sure you want to delete this entry?"))
      try {
        (await fetch(`/umbraco/api/ManagerApi/Delete/${e}`, { method: "DELETE" })).ok && (this._books = this._books.filter((i) => i.id !== e));
      } catch {
      }
  }
  _onFileChange(e) {
    var i;
    const t = e.target;
    this._selectedFile = ((i = t.files) == null ? void 0 : i[0]) ?? null, this._uploadMessage = "";
  }
  async _upload() {
    if (this._selectedFile) {
      this._uploading = !0, this._uploadMessage = "";
      try {
        const e = new FormData();
        e.append("file", this._selectedFile);
        const t = await fetch("/umbraco/api/UploadApi/UploadFiles", {
          method: "POST",
          body: e
        });
        if (!t.ok) throw new Error(`HTTP ${t.status}`);
        this._uploadSuccess = !0, this._uploadMessage = "File uploaded successfully.", this._selectedFile = null, await this._fetchBooks();
      } catch (e) {
        this._uploadSuccess = !1, this._uploadMessage = e instanceof Error ? e.message : "Upload failed.";
      } finally {
        this._uploading = !1;
      }
    }
  }
  _renderLibraryTab() {
    return this._loadingBooks ? l`
        <div class="loading">
          <uui-loader-circle></uui-loader-circle>
          <span>Loading library...</span>
        </div>
      ` : this._books.length === 0 ? l`
        <div class="empty-state">
          <p>No PDFs in the library yet. Upload one to get started.</p>
        </div>
      ` : l`
      <uui-table>
        <uui-table-head>
          <uui-table-head-cell>ID</uui-table-head-cell>
          <uui-table-head-cell>Title</uui-table-head-cell>
          <uui-table-head-cell>Author</uui-table-head-cell>
          <uui-table-head-cell>Actions</uui-table-head-cell>
        </uui-table-head>
        ${this._books.map(
      (e) => l`
            <uui-table-row>
              <uui-table-cell>${e.id}</uui-table-cell>
              <uui-table-cell>${e.title}</uui-table-cell>
              <uui-table-cell>${e.author}</uui-table-cell>
              <uui-table-cell>
                <uui-button
                  look="secondary"
                  color="danger"
                  label="Delete"
                  @click=${() => this._deleteBook(e.id)}
                >
                  Delete
                </uui-button>
              </uui-table-cell>
            </uui-table-row>
          `
    )}
      </uui-table>
    `;
  }
  _renderUploadTab() {
    return l`
      <uui-box headline="Upload PDF">
        <div class="upload-area">
          <label for="pdf-file-input">Select a PDF file</label>
          <input
            id="pdf-file-input"
            type="file"
            accept=".pdf"
            @change=${this._onFileChange}
          />
          ${this._selectedFile ? l`<p>Selected: <strong>${this._selectedFile.name}</strong></p>` : p}
          <uui-button
            look="primary"
            label="Upload"
            ?disabled=${!this._selectedFile || this._uploading}
            @click=${this._upload}
          >
            ${this._uploading ? "Uploading..." : "Upload"}
          </uui-button>
          ${this._uploadMessage ? l`
                <div class="message ${this._uploadSuccess ? "success" : "error"}">
                  ${this._uploadMessage}
                </div>
              ` : p}
        </div>
      </uui-box>
    `;
  }
  render() {
    return l`
      <h1>PDF Curator</h1>
      <p class="description">
        Manage your digital book and PDF library. Upload, browse, and organise
        your PDF collection from the Umbraco backoffice.
      </p>

      <uui-tab-group>
        <uui-tab
          label="Library"
          ?active=${this._activeTab === "library"}
          @click=${() => this._activeTab = "library"}
        >
          Library
        </uui-tab>
        <uui-tab
          label="Upload"
          ?active=${this._activeTab === "upload"}
          @click=${() => this._activeTab = "upload"}
        >
          Upload
        </uui-tab>
      </uui-tab-group>

      <div class="tab-content">
        ${this._activeTab === "library" ? this._renderLibraryTab() : this._renderUploadTab()}
      </div>
    `;
  }
};
a.styles = b`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
    }

    h1 {
      font-size: 1.5rem;
      font-weight: 600;
      margin: 0 0 var(--uui-size-space-3, 8px);
    }

    p.description {
      color: var(--uui-color-text-alt, #6b7280);
      margin: 0 0 var(--uui-size-space-6, 24px);
    }

    uui-tab-group {
      margin-bottom: var(--uui-size-space-5, 16px);
    }

    .tab-content {
      margin-top: var(--uui-size-space-5, 16px);
    }

    .empty-state {
      text-align: center;
      padding: var(--uui-size-space-10, 48px);
      color: var(--uui-color-text-alt, #6b7280);
    }

    .upload-area {
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-space-4, 12px);
      max-width: 480px;
    }

    .upload-area label {
      font-weight: 500;
    }

    .message {
      padding: var(--uui-size-space-3, 8px) var(--uui-size-space-4, 12px);
      border-radius: var(--uui-border-radius, 4px);
      font-size: 0.875rem;
    }

    .message.success {
      background: #d1fae5;
      color: #065f46;
      border: 1px solid #6ee7b7;
    }

    .message.error {
      background: #fee2e2;
      color: #991b1b;
      border: 1px solid #fca5a5;
    }

    .loading {
      display: flex;
      align-items: center;
      gap: var(--uui-size-space-3, 8px);
      color: var(--uui-color-text-alt, #6b7280);
      padding: var(--uui-size-space-5, 16px) 0;
    }

    uui-table {
      width: 100%;
    }

    uui-table-head-cell,
    uui-table-cell {
      padding: var(--uui-size-space-3, 8px) var(--uui-size-space-4, 12px);
    }
  `;
s([
  u()
], a.prototype, "_activeTab", 2);
s([
  u()
], a.prototype, "_books", 2);
s([
  u()
], a.prototype, "_loadingBooks", 2);
s([
  u()
], a.prototype, "_selectedFile", 2);
s([
  u()
], a.prototype, "_uploading", 2);
s([
  u()
], a.prototype, "_uploadMessage", 2);
s([
  u()
], a.prototype, "_uploadSuccess", 2);
a = s([
  h("pdf-curator-dashboard")
], a);
const v = a;
export {
  a as PdfCuratorDashboardElement,
  v as default
};
