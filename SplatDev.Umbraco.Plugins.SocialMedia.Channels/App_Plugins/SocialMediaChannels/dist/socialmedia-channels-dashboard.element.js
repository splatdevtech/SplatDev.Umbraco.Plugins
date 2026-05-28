import { LitElement as u, nothing as c, html as e, css as p, state as l, customElement as g } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as m } from "@umbraco-cms/backoffice/element-api";
var b = Object.defineProperty, _ = Object.getOwnPropertyDescriptor, n = (t, a, d, o) => {
  for (var s = o > 1 ? void 0 : o ? _(a, d) : a, h = t.length - 1, r; h >= 0; h--)
    (r = t[h]) && (s = (o ? r(a, d, s) : r(s)) || s);
  return o && s && b(a, d, s), s;
};
let i = class extends m(u) {
  constructor() {
    super(...arguments), this._channels = [], this._posts = [], this._editingChannel = null, this._editingPost = null, this._loading = !1, this._message = null, this._api = "/umbraco/api/SocialChannelsApi";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadChannels(), this._loadPosts();
  }
  async _loadChannels() {
    try {
      const t = await fetch(`${this._api}/GetChannels`);
      this._channels = await t.json();
    } catch {
      this._channels = [];
    }
  }
  async _loadPosts() {
    try {
      const t = await fetch(`${this._api}/GetPosts`);
      this._posts = await t.json();
    } catch {
      this._posts = [];
    }
  }
  async _addChannel() {
    if (this._editingChannel) {
      this._loading = !0;
      try {
        await fetch(`${this._api}/AddChannel`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(this._editingChannel)
        }), this._editingChannel = null, this._message = { type: "success", text: "Channel added." }, await this._loadChannels();
      } catch {
        this._message = { type: "error", text: "Failed to add channel." };
      }
      this._loading = !1;
    }
  }
  async _removeChannel(t) {
    if (confirm("Remove this channel?"))
      try {
        await fetch(`${this._api}/RemoveChannel?id=${t}`, { method: "DELETE" }), await this._loadChannels();
      } catch {
        this._message = { type: "error", text: "Remove failed." };
      }
  }
  async _schedulePost() {
    if (this._editingPost) {
      this._loading = !0;
      try {
        await fetch(`${this._api}/SchedulePost`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(this._editingPost)
        }), this._editingPost = null, this._message = { type: "success", text: "Post scheduled." }, await this._loadPosts();
      } catch {
        this._message = { type: "error", text: "Schedule failed." };
      }
      this._loading = !1;
    }
  }
  async _deletePost(t) {
    try {
      await fetch(`${this._api}/DeletePost?id=${t}`, { method: "DELETE" }), await this._loadPosts();
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
  }
  render() {
    return e`
      <h1>Social Media Channels</h1>
      <p class="description">Manage social media channels and schedule posts.</p>

      ${this._message ? e`<div class="msg ${this._message.type}">${this._message.text}</div>` : c}

      <uui-box headline="Channels">
        ${this._editingChannel ? e`
          <div class="edit-form">
            <div class="fields">
              <div class="field"><label>Platform</label><uui-input .value=${this._editingChannel.platform} @input=${(t) => {
      this._editingChannel = { ...this._editingChannel, platform: t.target.value };
    }}></uui-input></div>
              <div class="field"><label>Name</label><uui-input .value=${this._editingChannel.name} @input=${(t) => {
      this._editingChannel = { ...this._editingChannel, name: t.target.value };
    }}></uui-input></div>
              <div class="field"><label>URL</label><uui-input .value=${this._editingChannel.url} @input=${(t) => {
      this._editingChannel = { ...this._editingChannel, url: t.target.value };
    }}></uui-input></div>
            </div>
            <div class="buttons">
              <uui-button look="primary" @click=${this._addChannel}>Add</uui-button>
              <uui-button look="secondary" @click=${() => this._editingChannel = null}>Cancel</uui-button>
            </div>
          </div>
        ` : e`<uui-button look="primary" @click=${() => this._editingChannel = { platform: "", name: "", url: "" }} style="margin-bottom:12px;">+ Add Channel</uui-button>`}

        ${this._channels.length === 0 ? e`<div class="empty">No channels configured.</div>` : e`
          <table>
            <thead><tr><th>Platform</th><th>Name</th><th>URL</th><th></th></tr></thead>
            <tbody>
              ${this._channels.map((t) => e`
                <tr><td>${t.platform}</td><td>${t.name}</td><td>${t.url}</td>
                <td><uui-button look="danger" compact @click=${() => this._removeChannel(t.id)}>Remove</uui-button></td></tr>
              `)}
            </tbody>
          </table>
        `}
      </uui-box>

      <uui-box headline="Scheduled Posts" style="margin-top:16px;">
        ${this._editingPost ? e`
          <div class="edit-form">
            <div class="fields">
              <div class="field"><label>Channel</label>
                <uui-select .options=${this._channels.map((t) => {
      var a;
      return { name: `${t.platform} — ${t.name}`, value: String(t.id), selected: t.id === ((a = this._editingPost) == null ? void 0 : a.channelId) };
    })}
                  @change=${(t) => {
      this._editingPost = { ...this._editingPost, channelId: Number(t.target.value) };
    }}></uui-select></div>
              <div class="field"><label>Scheduled At</label>
                <input type="datetime-local" style="padding:6px;border:1px solid #d1d5db;border-radius:4px;" @input=${(t) => {
      this._editingPost = { ...this._editingPost, scheduledAt: t.target.value };
    }} /></div>
              <div class="field" style="grid-column:1/-1;"><label>Content</label>
                <uui-input .value=${this._editingPost.content} @input=${(t) => {
      this._editingPost = { ...this._editingPost, content: t.target.value };
    }}></uui-input></div>
            </div>
            <div class="buttons">
              <uui-button look="primary" @click=${this._schedulePost}>Schedule</uui-button>
              <uui-button look="secondary" @click=${() => this._editingPost = null}>Cancel</uui-button>
            </div>
          </div>
        ` : e`<uui-button look="primary" @click=${() => this._editingPost = { channelId: 0, content: "", scheduledAt: "" }} style="margin-bottom:12px;">+ Schedule Post</uui-button>`}

        ${this._posts.length === 0 ? e`<div class="empty">No scheduled posts.</div>` : e`
          <table>
            <thead><tr><th>Content</th><th>Scheduled</th><th></th></tr></thead>
            <tbody>
              ${this._posts.map((t) => e`
                <tr><td>${t.content}</td><td>${t.scheduledAt}</td>
                <td><uui-button look="danger" compact @click=${() => this._deletePost(t.id)}>Delete</uui-button></td></tr>
              `)}
            </tbody>
          </table>
        `}
      </uui-box>
    `;
  }
};
i.styles = p`
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
n([
  l()
], i.prototype, "_channels", 2);
n([
  l()
], i.prototype, "_posts", 2);
n([
  l()
], i.prototype, "_editingChannel", 2);
n([
  l()
], i.prototype, "_editingPost", 2);
n([
  l()
], i.prototype, "_loading", 2);
n([
  l()
], i.prototype, "_message", 2);
i = n([
  g("socialmedia-channels-dashboard")
], i);
const y = i;
export {
  i as SocialMediaChannelsDashboardElement,
  y as default
};
