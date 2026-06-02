import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/SocialChannelsApi";

class SocialChannelsDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: linear-gradient(135deg,#f09433,#e6683c,#dc2743,#cc2366,#bc1888); display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .pill { display: inline-flex; align-items: center; gap: 6px; padding: 4px 14px; border-radius: 9999px; font-size: 0.8125rem; font-weight: 600; margin-left: auto; white-space: nowrap; }
    .pill-checking { background: #eff6ff; color: #1d4ed8; }
    .pill-ok { background: #dcfce7; color: #15803d; }
    .pill-err { background: #fee2e2; color: #dc2626; }
    .dot { width: 8px; height: 8px; border-radius: 50%; background: currentColor; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .channel-list { display: flex; flex-direction: column; gap: 8px; margin-bottom: 16px; }
    .channel-row { display: flex; align-items: center; gap: 12px; background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 12px 14px; }
    .channel-platform { font-size: 0.75rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.05em; color: #6b7280; min-width: 80px; }
    .channel-name { font-weight: 600; flex: 1; }
    .channel-url { font-size: 0.8125rem; color: #3b82f6; text-decoration: none; flex: 2; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .channel-url:hover { text-decoration: underline; }
    .del-btn { color: #dc2626; cursor: pointer; background: none; border: none; font-size: 0.875rem; padding: 4px 8px; border-radius: 4px; flex-shrink: 0; }
    .del-btn:hover { background: #fee2e2; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 10px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); }
    td { padding: 10px 12px; border-bottom: 1px solid var(--uui-color-border, #f0f0f0); vertical-align: middle; }
    tr:hover td { background: var(--uui-color-surface-alt, #f9fafb); }
    .empty { text-align: center; padding: 24px; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 10px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 130px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text], input[type=url], select {
      border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px;
      font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a);
    }
    input:focus, select:focus { outline: none; border-color: #e6683c; box-shadow: 0 0 0 2px rgba(230,104,60,0.15); }
    .date-text { font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
  `;

  static properties = {
    _status: { state: true },
    _channels: { state: true },
    _posts: { state: true },
    _error: { state: true },
    _chName: { state: true },
    _chPlatform: { state: true },
    _chUrl: { state: true },
    _adding: { state: true },
  };

  constructor() {
    super();
    this._status = "loading";
    this._channels = [];
    this._posts = [];
    this._error = "";
    this._chName = "";
    this._chPlatform = "instagram";
    this._chUrl = "";
    this._adding = false;
  }

  connectedCallback() {
    super.connectedCallback();
    this._load();
  }

  async _load() {
    this._status = "loading";
    this._error = "";
    try {
      const [chR, poR] = await Promise.all([
        fetch(`${API}/GetChannels`),
        fetch(`${API}/GetPosts`),
      ]);
      if (chR.ok) this._channels = await chR.json();
      if (poR.ok) this._posts = await poR.json();
      this._status = "ok";
    } catch (e) {
      this._status = "err";
      this._error = e instanceof Error ? e.message : String(e);
    }
  }

  async _addChannel() {
    if (!this._chName.trim() || !this._chUrl.trim()) return;
    this._adding = true;
    try {
      const r = await fetch(`${API}/AddChannel`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name: this._chName.trim(), platform: this._chPlatform, url: this._chUrl.trim() }),
      });
      if (r.ok) {
        this._chName = "";
        this._chUrl = "";
        await this._load();
      }
    } catch (_) {}
    this._adding = false;
  }

  async _removeChannel(id) {
    if (!confirm("Remove this channel?")) return;
    await fetch(`${API}/RemoveChannel?id=${id}`, { method: "DELETE" });
    await this._load();
  }

  async _deletePost(id) {
    if (!confirm("Delete this scheduled post?")) return;
    await fetch(`${API}/DeletePost?id=${id}`, { method: "DELETE" });
    await this._load();
  }

  _fmt(dt) {
    if (!dt) return "—";
    try { return new Date(dt).toLocaleString(); } catch { return dt; }
  }

  render() {
    return html`
      <div class="header">
        <div class="logo"><span>📡</span></div>
        <div>
          <h1>Social Channels</h1>
          <p>Manage social media channels and scheduled posts</p>
        </div>
        <span class="pill ${this._status === 'ok' ? 'pill-ok' : this._status === 'loading' ? 'pill-checking' : 'pill-err'}">
          <span class="dot"></span>
          ${this._status === 'ok' ? `${this._channels.length} channel(s)` : this._status === 'loading' ? 'Loading…' : 'Error'}
        </span>
      </div>

      ${this._status === "err" ? html`<div class="notice err"><strong>Error:</strong> ${this._error}</div>` : ""}

      <uui-box headline="Channels">
        ${this._status === "loading" ? html`<div class="notice info">Loading…</div>` : ""}
        ${this._status === "ok" && this._channels.length === 0 ? html`<div class="empty">No social channels configured.</div>` : ""}
        ${this._status === "ok" && this._channels.length > 0 ? html`
          <div class="channel-list">
            ${this._channels.map(c => html`
              <div class="channel-row">
                <span class="channel-platform">${c.platform}</span>
                <span class="channel-name">${c.name}</span>
                ${c.url ? html`<a class="channel-url" href="${c.url}" target="_blank" rel="noopener">${c.url}</a>` : html`<span class="channel-url">—</span>`}
                <button class="del-btn" @click=${() => this._removeChannel(c.id)}>✕</button>
              </div>
            `)}
          </div>
        ` : ""}
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Name *</span>
            <input type="text" .value=${this._chName} @input=${e => this._chName = e.target.value} placeholder="My Instagram" />
          </div>
          <div class="form-col" style="max-width:150px">
            <span class="field-label">Platform</span>
            <select .value=${this._chPlatform} @change=${e => this._chPlatform = e.target.value}>
              <option value="instagram">Instagram</option>
              <option value="facebook">Facebook</option>
              <option value="twitter">X / Twitter</option>
              <option value="linkedin">LinkedIn</option>
              <option value="youtube">YouTube</option>
              <option value="tiktok">TikTok</option>
              <option value="other">Other</option>
            </select>
          </div>
          <div class="form-col">
            <span class="field-label">URL</span>
            <input type="url" .value=${this._chUrl} @input=${e => this._chUrl = e.target.value} placeholder="https://instagram.com/..." />
          </div>
          <div class="form-col" style="flex:0;min-width:0;justify-content:flex-end;padding-top:18px">
            <uui-button
              look="primary"
              label="Add"
              ?disabled=${!this._chName.trim() || !this._chUrl.trim() || this._adding}
              @click=${() => this._addChannel()}
              style="--uui-button-background-color:#e6683c;--uui-button-background-color-hover:#cc2366"
            >${this._adding ? "Adding…" : "Add Channel"}</uui-button>
          </div>
        </div>
        <div class="actions">
          <uui-button look="secondary" label="Refresh" @click=${() => this._load()}>↻ Refresh</uui-button>
        </div>
      </uui-box>

      <uui-box headline="Scheduled Posts (${this._posts.length})">
        ${this._posts.length === 0 ? html`<div class="empty">No scheduled posts.</div>` : html`
          <table>
            <thead><tr><th>Content</th><th>Channel</th><th>Scheduled</th><th></th></tr></thead>
            <tbody>
              ${this._posts.map(p => html`
                <tr>
                  <td>${(p.content || "").slice(0, 80)}${(p.content || "").length > 80 ? "…" : ""}</td>
                  <td>${p.channelId || "—"}</td>
                  <td class="date-text">${this._fmt(p.scheduledAt)}</td>
                  <td><button class="del-btn" @click=${() => this._deletePost(p.id)}>✕</button></td>
                </tr>
              `)}
            </tbody>
          </table>
        `}
      </uui-box>
    `;
  }
}

customElements.define("social-channels-dashboard", SocialChannelsDashboard);
