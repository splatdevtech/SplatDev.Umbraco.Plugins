import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface SocialChannel {
  id?: number;
  platform: string;
  name: string;
  url: string;
}

interface ScheduledPost {
  id?: number;
  channelId: number;
  content: string;
  scheduledAt: string;
}

@customElement("socialmedia-channels-dashboard")
export class SocialMediaChannelsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 10px; margin-bottom: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; margin-bottom: 24px; }
    th { text-align: left; padding: 8px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
    .edit-form { padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 16px; background: var(--uui-color-surface-alt, #f9fafb); }
    .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 12px; }
    .field { display: flex; flex-direction: column; gap: 4px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .buttons { display: flex; gap: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _channels: SocialChannel[] = [];
  @state() private _posts: ScheduledPost[] = [];
  @state() private _editingChannel: SocialChannel | null = null;
  @state() private _editingPost: ScheduledPost | null = null;
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/SocialChannelsApi";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadChannels();
    this._loadPosts();
  }

  private async _loadChannels(): Promise<void> {
    try {
      const r = await fetch(`${this._api}/GetChannels`);
      this._channels = await r.json();
    } catch { this._channels = []; }
  }

  private async _loadPosts(): Promise<void> {
    try {
      const r = await fetch(`${this._api}/GetPosts`);
      this._posts = await r.json();
    } catch { this._posts = []; }
  }

  private async _addChannel(): Promise<void> {
    if (!this._editingChannel) return;
    this._loading = true;
    try {
      await fetch(`${this._api}/AddChannel`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._editingChannel),
      });
      this._editingChannel = null;
      this._message = { type: "success", text: "Channel added." };
      await this._loadChannels();
    } catch {
      this._message = { type: "error", text: "Failed to add channel." };
    }
    this._loading = false;
  }

  private async _removeChannel(id: number): Promise<void> {
    if (!confirm("Remove this channel?")) return;
    try {
      await fetch(`${this._api}/RemoveChannel?id=${id}`, { method: "DELETE" });
      await this._loadChannels();
    } catch {
      this._message = { type: "error", text: "Remove failed." };
    }
  }

  private async _schedulePost(): Promise<void> {
    if (!this._editingPost) return;
    this._loading = true;
    try {
      await fetch(`${this._api}/SchedulePost`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._editingPost),
      });
      this._editingPost = null;
      this._message = { type: "success", text: "Post scheduled." };
      await this._loadPosts();
    } catch {
      this._message = { type: "error", text: "Schedule failed." };
    }
    this._loading = false;
  }

  private async _deletePost(id: number): Promise<void> {
    try {
      await fetch(`${this._api}/DeletePost?id=${id}`, { method: "DELETE" });
      await this._loadPosts();
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
  }

  override render() {
    return html`
      <h1>Social Media Channels</h1>
      <p class="description">Manage social media channels and schedule posts.</p>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}

      <uui-box headline="Channels">
        ${this._editingChannel ? html`
          <div class="edit-form">
            <div class="fields">
              <div class="field"><label>Platform</label><uui-input .value=${this._editingChannel.platform} @input=${(e: InputEvent) => { this._editingChannel = { ...this._editingChannel!, platform: (e.target as HTMLInputElement).value }; }}></uui-input></div>
              <div class="field"><label>Name</label><uui-input .value=${this._editingChannel.name} @input=${(e: InputEvent) => { this._editingChannel = { ...this._editingChannel!, name: (e.target as HTMLInputElement).value }; }}></uui-input></div>
              <div class="field"><label>URL</label><uui-input .value=${this._editingChannel.url} @input=${(e: InputEvent) => { this._editingChannel = { ...this._editingChannel!, url: (e.target as HTMLInputElement).value }; }}></uui-input></div>
            </div>
            <div class="buttons">
              <uui-button look="primary" @click=${this._addChannel}>Add</uui-button>
              <uui-button look="secondary" @click=${() => (this._editingChannel = null)}>Cancel</uui-button>
            </div>
          </div>
        ` : html`<uui-button look="primary" @click=${() => (this._editingChannel = { platform: "", name: "", url: "" })} style="margin-bottom:12px;">+ Add Channel</uui-button>`}

        ${this._channels.length === 0 ? html`<div class="empty">No channels configured.</div>` : html`
          <table>
            <thead><tr><th>Platform</th><th>Name</th><th>URL</th><th></th></tr></thead>
            <tbody>
              ${this._channels.map((c) => html`
                <tr><td>${c.platform}</td><td>${c.name}</td><td>${c.url}</td>
                <td><uui-button look="danger" compact @click=${() => this._removeChannel(c.id!)}>Remove</uui-button></td></tr>
              `)}
            </tbody>
          </table>
        `}
      </uui-box>

      <uui-box headline="Scheduled Posts" style="margin-top:16px;">
        ${this._editingPost ? html`
          <div class="edit-form">
            <div class="fields">
              <div class="field"><label>Channel</label>
                <uui-select .options=${this._channels.map((c) => ({ name: `${c.platform} — ${c.name}`, value: String(c.id), selected: c.id === this._editingPost?.channelId }))}
                  @change=${(e: Event) => { this._editingPost = { ...this._editingPost!, channelId: Number((e.target as HTMLSelectElement).value) }; }}></uui-select></div>
              <div class="field"><label>Scheduled At</label>
                <input type="datetime-local" style="padding:6px;border:1px solid #d1d5db;border-radius:4px;" @input=${(e: InputEvent) => { this._editingPost = { ...this._editingPost!, scheduledAt: (e.target as HTMLInputElement).value }; }} /></div>
              <div class="field" style="grid-column:1/-1;"><label>Content</label>
                <uui-input .value=${this._editingPost.content} @input=${(e: InputEvent) => { this._editingPost = { ...this._editingPost!, content: (e.target as HTMLInputElement).value }; }}></uui-input></div>
            </div>
            <div class="buttons">
              <uui-button look="primary" @click=${this._schedulePost}>Schedule</uui-button>
              <uui-button look="secondary" @click=${() => (this._editingPost = null)}>Cancel</uui-button>
            </div>
          </div>
        ` : html`<uui-button look="primary" @click=${() => (this._editingPost = { channelId: 0, content: "", scheduledAt: "" })} style="margin-bottom:12px;">+ Schedule Post</uui-button>`}

        ${this._posts.length === 0 ? html`<div class="empty">No scheduled posts.</div>` : html`
          <table>
            <thead><tr><th>Content</th><th>Scheduled</th><th></th></tr></thead>
            <tbody>
              ${this._posts.map((p) => html`
                <tr><td>${p.content}</td><td>${p.scheduledAt}</td>
                <td><uui-button look="danger" compact @click=${() => this._deletePost(p.id!)}>Delete</uui-button></td></tr>
              `)}
            </tbody>
          </table>
        `}
      </uui-box>
    `;
  }
}

export default SocialMediaChannelsDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "socialmedia-channels-dashboard": SocialMediaChannelsDashboardElement;
  }
}
