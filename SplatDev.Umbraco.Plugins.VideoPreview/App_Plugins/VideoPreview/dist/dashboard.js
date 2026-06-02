import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/videopreview";

class VideoPreviewDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #1e293b; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 10px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 200px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=url] { border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px; font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a); }
    input:focus { outline: none; border-color: #1e293b; box-shadow: 0 0 0 2px rgba(30,41,59,0.15); }
    .video-info { background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 10px; overflow: hidden; margin-top: 16px; }
    .video-thumb { width: 100%; aspect-ratio: 16/9; max-height: 240px; object-fit: cover; background: #1e293b; display: block; }
    .video-thumb-placeholder { width: 100%; height: 180px; background: #1e293b; display: flex; align-items: center; justify-content: center; color: #e2e8f0; font-size: 3rem; }
    .video-meta { padding: 16px; }
    .video-title { font-size: 1.0625rem; font-weight: 700; margin-bottom: 8px; }
    .video-desc { font-size: 0.875rem; color: var(--uui-color-text-alt, #6b7280); line-height: 1.5; margin-bottom: 12px; }
    .meta-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(140px, 1fr)); gap: 10px; }
    .meta-item .label { font-size: 0.75rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); margin-bottom: 2px; }
    .meta-item .value { font-size: 0.875rem; font-weight: 600; }
    uui-box { margin-bottom: 20px; }
  `;

  static properties = {
    _videoUrl: { state: true },
    _loading: { state: true },
    _info: { state: true },
    _error: { state: true },
  };

  constructor() {
    super();
    this._videoUrl = "";
    this._loading = false;
    this._info = null;
    this._error = "";
  }

  async _getInfo() {
    if (!this._videoUrl.trim()) return;
    this._loading = true;
    this._info = null;
    this._error = "";
    try {
      const r = await fetch(`${API}/GetVideoInfo?url=${encodeURIComponent(this._videoUrl.trim())}`);
      if (r.ok) {
        this._info = await r.json();
      } else if (r.status === 404) {
        this._error = "Could not extract video info from the provided URL. Ensure it is a valid YouTube or Vimeo URL.";
      } else {
        this._error = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._error = e instanceof Error ? e.message : String(e);
    } finally {
      this._loading = false;
    }
  }

  _fmt(secs) {
    if (!secs) return "—";
    const m = Math.floor(secs / 60);
    const s = secs % 60;
    return `${m}:${String(s).padStart(2, "0")}`;
  }

  render() {
    const info = this._info;
    return html`
      <div class="header">
        <div class="logo"><span>🎬</span></div>
        <div>
          <h1>Video Preview</h1>
          <p>Extract metadata and thumbnails from YouTube and Vimeo URLs</p>
        </div>
      </div>

      <uui-box headline="Look Up Video Info">
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Video URL *</span>
            <input
              type="url"
              .value=${this._videoUrl}
              @input=${e => this._videoUrl = e.target.value}
              @keydown=${e => e.key === "Enter" && this._getInfo()}
              placeholder="https://www.youtube.com/watch?v=..."
            />
          </div>
          <div class="form-col" style="flex:0;min-width:0;justify-content:flex-end;padding-top:18px">
            <uui-button
              look="primary"
              label="Get Info"
              ?disabled=${!this._videoUrl.trim() || this._loading}
              @click=${() => this._getInfo()}
              style="--uui-button-background-color:#1e293b;--uui-button-background-color-hover:#0f172a"
            >${this._loading ? "Loading…" : "Get Video Info"}</uui-button>
          </div>
        </div>

        ${this._error ? html`<div class="notice err">${this._error}</div>` : ""}

        ${info ? html`
          <div class="video-info">
            ${info.thumbnailUrl
              ? html`<img class="video-thumb" src="${info.thumbnailUrl}" alt="${info.title || 'Video thumbnail'}" />`
              : html`<div class="video-thumb-placeholder">🎬</div>`}
            <div class="video-meta">
              <div class="video-title">${info.title || "Untitled"}</div>
              ${info.description ? html`<div class="video-desc">${info.description.slice(0, 200)}${info.description.length > 200 ? "…" : ""}</div>` : ""}
              <div class="meta-grid">
                ${info.platform ? html`<div class="meta-item"><div class="label">Platform</div><div class="value">${info.platform}</div></div>` : ""}
                ${info.duration != null ? html`<div class="meta-item"><div class="label">Duration</div><div class="value">${this._fmt(info.duration)}</div></div>` : ""}
                ${info.videoId ? html`<div class="meta-item"><div class="label">Video ID</div><div class="value" style="font-family:monospace">${info.videoId}</div></div>` : ""}
                ${info.author ? html`<div class="meta-item"><div class="label">Author</div><div class="value">${info.author}</div></div>` : ""}
                ${info.embedUrl ? html`<div class="meta-item" style="grid-column:span 2"><div class="label">Embed URL</div><div class="value" style="font-family:monospace;font-size:0.8125rem;word-break:break-all">${info.embedUrl}</div></div>` : ""}
              </div>
            </div>
          </div>
        ` : ""}
      </uui-box>

      <uui-box headline="Supported Platforms">
        <div class="notice info">
          Video Preview extracts metadata (title, thumbnail, embed URL, duration) from YouTube and Vimeo video URLs.
          Use the API endpoint at <code>GET /umbraco/api/videopreview/GetVideoInfo?url=…</code> in templates to show video previews.
        </div>
      </uui-box>
    `;
  }
}

customElements.define("video-preview-dashboard", VideoPreviewDashboard);
