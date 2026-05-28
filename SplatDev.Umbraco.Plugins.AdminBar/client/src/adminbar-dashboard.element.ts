import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface PageInfo {
  id: number;
  name: string;
  published: boolean;
  url: string | null;
}

@customElement("adminbar-dashboard")
export class AdminBarDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .input-row { display: flex; gap: 10px; align-items: center; margin-bottom: 20px; }
    .info-card {
      padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 6px; margin-top: 16px;
    }
    .info-row { display: flex; gap: 8px; align-items: center; margin-bottom: 8px; }
    .info-label { font-weight: 600; min-width: 80px; font-size: 0.85rem; }
    .info-value { font-size: 0.85rem; }
    .status-badge { display: inline-block; padding: 2px 10px; border-radius: 9999px; font-size: 0.8rem; font-weight: 600; }
    .status-badge.published { background: #d1fae5; color: #065f46; }
    .status-badge.unpublished { background: #fef3c7; color: #92400e; }
    .action-row { display: flex; gap: 10px; margin-top: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-top: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
  `;

  @state() private _nodeId = "";
  @state() private _page: PageInfo | null = null;
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/adminbar";

  private async _lookup(): Promise<void> {
    if (!this._nodeId) return;
    this._loading = true;
    this._message = null;
    this._page = null;
    try {
      const r = await fetch(`${this._api}/GetCurrentPageInfo?nodeId=${this._nodeId}`);
      if (!r.ok) throw new Error(`${r.status}`);
      this._page = await r.json();
    } catch {
      this._message = { type: "error", text: "Page not found or request failed." };
    }
    this._loading = false;
  }

  private async _publish(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/PublishPage?nodeId=${this._nodeId}`, { method: "POST" });
      const d = await r.json();
      this._message = { type: r.ok ? "success" : "error", text: d.message };
      if (r.ok) await this._lookup();
    } catch {
      this._message = { type: "error", text: "Publish failed." };
    }
    this._loading = false;
  }

  private async _unpublish(): Promise<void> {
    if (!confirm("Unpublish this page?")) return;
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/UnpublishPage?nodeId=${this._nodeId}`, { method: "POST" });
      const d = await r.json();
      this._message = { type: r.ok ? "success" : "error", text: d.message };
      if (r.ok) await this._lookup();
    } catch {
      this._message = { type: "error", text: "Unpublish failed." };
    }
    this._loading = false;
  }

  override render() {
    return html`
      <h1>Admin Bar</h1>
      <p class="description">Lookup and manage content pages by node ID.</p>

      <uui-box headline="Page Lookup">
        <div class="input-row">
          <input type="number" .value=${this._nodeId}
            @input=${(e: InputEvent) => (this._nodeId = (e.target as HTMLInputElement).value)}
            placeholder="Node ID" style="width:140px;padding:8px;border:1px solid #d1d5db;border-radius:4px;" />
          <uui-button look="secondary" ?disabled=${!this._nodeId || this._loading}
            @click=${this._lookup}>Lookup</uui-button>
        </div>
      </uui-box>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}

      ${this._page ? html`
        <div class="info-card">
          <div class="info-row">
            <span class="info-label">ID</span>
            <span class="info-value">${this._page.id}</span>
          </div>
          <div class="info-row">
            <span class="info-label">Name</span>
            <span class="info-value">${this._page.name}</span>
          </div>
          <div class="info-row">
            <span class="info-label">Status</span>
            <span class="status-badge ${this._page.published ? "published" : "unpublished"}">
              ${this._page.published ? "Published" : "Unpublished"}
            </span>
          </div>
          ${this._page.url ? html`
            <div class="info-row">
              <span class="info-label">URL</span>
              <span class="info-value">${this._page.url}</span>
            </div>
          ` : nothing}

          <div class="action-row">
            ${!this._page.published ? html`
              <uui-button look="primary" ?disabled=${this._loading} @click=${this._publish}>Publish</uui-button>
            ` : html`
              <uui-button look="danger" ?disabled=${this._loading} @click=${this._unpublish}>Unpublish</uui-button>
            `}
          </div>
        </div>
      ` : nothing}
    `;
  }
}

export default AdminBarDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "adminbar-dashboard": AdminBarDashboardElement;
  }
}
