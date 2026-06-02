import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/livevideo";

class LiveVideoDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #dc2626; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .warn  { background: #fffbeb; color: #92400e; border-left: 3px solid #f59e0b; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .ok-notice { background: #dcfce7; color: #15803d; border-left: 3px solid #22c55e; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 12px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 140px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], select {
      border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px;
      font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a);
    }
    input:focus, select:focus { outline: none; border-color: #dc2626; box-shadow: 0 0 0 2px rgba(220,38,38,0.15); }
    .embed-url-box { background: #f0fdf4; border: 1px solid #86efac; border-radius: 6px; padding: 10px 14px; font-family: monospace; font-size: 0.875rem; word-break: break-all; margin-top: 12px; }
    .embed-url-box a { color: #15803d; }
    .embed-preview { margin-top: 16px; aspect-ratio: 16/9; max-width: 640px; border-radius: 8px; overflow: hidden; border: 1px solid var(--uui-color-border, #e5e7eb); }
    .embed-preview iframe { width: 100%; height: 100%; border: none; display: block; }
    uui-box { margin-bottom: 20px; }
    .platform-guide { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 12px; }
    .platform-card { background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 14px; }
    .platform-card h3 { margin: 0 0 6px; font-size: 0.9375rem; }
    .platform-card p { margin: 0; font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); }
    code { background: #f3f4f6; padding: 1px 5px; border-radius: 3px; font-size: 0.8125rem; }
  `;

  static properties = {
    _platform: { state: true },
    _channelId: { state: true },
    _loading: { state: true },
    _embedUrl: { state: true },
    _error: { state: true },
  };

  constructor() {
    super();
    this._platform = "YouTube";
    this._channelId = "";
    this._loading = false;
    this._embedUrl = "";
    this._error = "";
  }

  async _getEmbed() {
    if (!this._channelId.trim()) return;
    this._loading = true;
    this._embedUrl = "";
    this._error = "";
    try {
      const r = await fetch(`${API}/GetEmbed?platform=${this._platform}&channelId=${encodeURIComponent(this._channelId.trim())}`);
      if (r.ok) {
        const data = await r.json();
        this._embedUrl = typeof data === "string" ? data : (data.url || data.embedUrl || JSON.stringify(data));
      } else {
        const text = await r.text();
        this._error = `HTTP ${r.status}: ${text}`;
      }
    } catch (e) {
      this._error = e instanceof Error ? e.message : String(e);
    } finally {
      this._loading = false;
    }
  }

  _isEmbeddable() {
    return this._embedUrl && !this._embedUrl.startsWith("Error");
  }

  render() {
    return html`
      <div class="header">
        <div class="logo"><span>▶</span></div>
        <div>
          <h1>Live Video</h1>
          <p>Generate embed URLs for live streaming on YouTube, Twitch, and Vimeo</p>
        </div>
      </div>

      <uui-box headline="Generate Embed URL">
        <div class="form-row">
          <div class="form-col" style="max-width:160px">
            <span class="field-label">Platform</span>
            <select .value=${this._platform} @change=${e => { this._platform = e.target.value; this._embedUrl = ""; this._error = ""; }}>
              <option value="YouTube">YouTube</option>
              <option value="Twitch">Twitch</option>
              <option value="Vimeo">Vimeo</option>
            </select>
          </div>
          <div class="form-col">
            <span class="field-label">Channel / Video ID *</span>
            <input
              type="text"
              .value=${this._channelId}
              @input=${e => this._channelId = e.target.value}
              placeholder="${this._platform === 'YouTube' ? 'UCxxxxxx or live video ID' : this._platform === 'Twitch' ? 'channel_name' : 'video_id'}"
            />
          </div>
        </div>
        <uui-button
          look="primary"
          label="Get Embed URL"
          ?disabled=${!this._channelId.trim() || this._loading}
          @click=${() => this._getEmbed()}
          style="--uui-button-background-color:#dc2626;--uui-button-background-color-hover:#b91c1c"
        >${this._loading ? "Getting URL…" : "Get Embed URL"}</uui-button>

        ${this._error ? html`<div class="notice err" style="margin-top:12px">${this._error}</div>` : ""}
        ${this._embedUrl ? html`
          <div class="embed-url-box">
            <a href="${this._embedUrl}" target="_blank" rel="noopener">${this._embedUrl}</a>
          </div>
          ${this._isEmbeddable() ? html`
            <div class="embed-preview">
              <iframe src="${this._embedUrl}" allowfullscreen></iframe>
            </div>
          ` : ""}
        ` : ""}
      </uui-box>

      <uui-box headline="Channel ID Reference">
        <div class="platform-guide">
          <div class="platform-card">
            <h3>YouTube</h3>
            <p>Use the channel ID (e.g. <code>UC...</code>) or a live stream video ID for a specific broadcast.</p>
          </div>
          <div class="platform-card">
            <h3>Twitch</h3>
            <p>Use the channel name exactly as it appears in the Twitch URL (e.g. <code>shroud</code>).</p>
          </div>
          <div class="platform-card">
            <h3>Vimeo</h3>
            <p>Use the numeric video or event ID from the Vimeo video URL (e.g. <code>123456789</code>).</p>
          </div>
        </div>
      </uui-box>
    `;
  }
}

customElements.define("live-video-dashboard", LiveVideoDashboard);
