import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface ToastMessage {
  id?: number;
  title: string;
  message: string;
  type: string;
  isActive: boolean;
}

@customElement("toastnotifications-dashboard")
export class ToastNotificationsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 10px; margin-bottom: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .toast-card { padding: 12px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 8px; display: flex; justify-content: space-between; align-items: center; }
    .toast-info h4 { margin: 0 0 4px; font-size: 0.95rem; }
    .toast-info p { margin: 0; font-size: 0.8rem; color: var(--uui-color-text-alt, #6b7280); }
    .edit-form { padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 16px; background: var(--uui-color-surface-alt, #f9fafb); }
    .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 12px; }
    .field { display: flex; flex-direction: column; gap: 4px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .buttons { display: flex; gap: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _toasts: ToastMessage[] = [];
  @state() private _editing: ToastMessage | null = null;
  @state() private _creating = false;
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/toastnotifications";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadToasts();
  }

  private async _loadToasts(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetActive`);
      this._toasts = await r.json();
    } catch {
      this._toasts = [];
    }
    this._loading = false;
  }

  private _startCreate(): void {
    this._editing = { title: "", message: "", type: "info", isActive: true };
    this._creating = true;
  }

  private _startEdit(t: ToastMessage): void {
    this._editing = { ...t };
    this._creating = false;
  }

  private _cancelEdit(): void {
    this._editing = null;
    this._creating = false;
  }

  private async _save(): Promise<void> {
    if (!this._editing) return;
    this._loading = true;
    try {
      const method = this._creating ? "POST" : "PUT";
      const url = this._creating ? `${this._api}/Create` : `${this._api}/Update?id=${this._editing.id}`;
      const r = await fetch(url, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._editing),
      });
      if (!r.ok) throw new Error();
      this._message = { type: "success", text: `Toast ${this._creating ? "created" : "updated"}.` };
      this._editing = null;
      this._creating = false;
      await this._loadToasts();
    } catch {
      this._message = { type: "error", text: "Save failed." };
    }
    this._loading = false;
  }

  private async _delete(id: number): Promise<void> {
    if (!confirm("Delete this toast?")) return;
    this._loading = true;
    try {
      await fetch(`${this._api}/Delete?id=${id}`, { method: "DELETE" });
      this._message = { type: "success", text: "Toast deleted." };
      await this._loadToasts();
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
    this._loading = false;
  }

  override render() {
    return html`
      <h1>Toast Notifications</h1>
      <p class="description">Manage toast notification messages shown to site visitors.</p>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}
      ${this._editing ? this._renderEditor() : nothing}

      <div class="toolbar">
        <uui-button look="primary" @click=${this._startCreate}>+ Create Toast</uui-button>
      </div>

      ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}

      ${this._toasts.length === 0 && !this._loading
        ? html`<div class="empty">No active toasts.</div>`
        : this._toasts.map((t) => html`
            <div class="toast-card">
              <div class="toast-info">
                <h4>${t.title}</h4>
                <p>${t.message} — <em>${t.type}</em></p>
              </div>
              <div style="display:flex;gap:6px;">
                <uui-button look="secondary" compact @click=${() => this._startEdit(t)}>Edit</uui-button>
                <uui-button look="danger" compact @click=${() => this._delete(t.id!)}>Delete</uui-button>
              </div>
            </div>
          `)}
    `;
  }

  private _renderEditor() {
    const t = this._editing!;
    const typeOptions = [
      { name: "Info", value: "info", selected: t.type === "info" },
      { name: "Success", value: "success", selected: t.type === "success" },
      { name: "Warning", value: "warning", selected: t.type === "warning" },
      { name: "Error", value: "error", selected: t.type === "error" },
    ];
    return html`
      <div class="edit-form">
        <h3 style="margin:0 0 12px;">${this._creating ? "Create Toast" : "Edit Toast"}</h3>
        <div class="fields">
          <div class="field">
            <label>Title</label>
            <uui-input .value=${t.title} @input=${(e: InputEvent) => { this._editing = { ...t, title: (e.target as HTMLInputElement).value }; }}></uui-input>
          </div>
          <div class="field">
            <label>Type</label>
            <uui-select .options=${typeOptions} @change=${(e: Event) => { this._editing = { ...t, type: (e.target as HTMLSelectElement).value }; }}></uui-select>
          </div>
          <div class="field" style="grid-column: 1 / -1;">
            <label>Message</label>
            <uui-input .value=${t.message} @input=${(e: InputEvent) => { this._editing = { ...t, message: (e.target as HTMLInputElement).value }; }}></uui-input>
          </div>
        </div>
        <div class="buttons">
          <uui-button look="primary" @click=${this._save}>${this._creating ? "Create" : "Save"}</uui-button>
          <uui-button look="secondary" @click=${this._cancelEdit}>Cancel</uui-button>
        </div>
      </div>
    `;
  }
}

export default ToastNotificationsDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "toastnotifications-dashboard": ToastNotificationsDashboardElement;
  }
}
