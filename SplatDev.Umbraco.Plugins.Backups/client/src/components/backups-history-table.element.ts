import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
  BackupsContext,
  BACKUPS_CONTEXT_TOKEN,
  type BackupInfo,
} from "../context/backups.context.js";
import "./backups-provider-badge.element.js";

@customElement("backups-history-table")
export class BackupsHistoryTableElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host {
      display: block;
    }
    .empty-state {
      text-align: center;
      padding: var(--uui-size-space-6, 24px);
      color: var(--uui-color-text-alt, #6b7280);
    }
    .actions {
      display: flex;
      gap: 4px;
    }
    .extension-badge {
      display: inline-block;
      padding: 2px 6px;
      border-radius: 3px;
      font-size: 0.7rem;
      font-weight: 600;
      background: var(--uui-color-surface-alt, #f3f4f6);
      color: var(--uui-color-text-alt, #6b7280);
    }
    uui-table {
      width: 100%;
    }
  `;

  @state() private _backups: BackupInfo[] = [];
  @state() private _loading = false;

  #context?: BackupsContext;

  override connectedCallback(): void {
    super.connectedCallback();
    this.consumeContext(BACKUPS_CONTEXT_TOKEN, (ctx) => {
      this.#context = ctx;
      this.#loadBackups();
    });
  }

  async #loadBackups(): Promise<void> {
    if (!this.#context) return;
    this._loading = true;
    try {
      this._backups = await this.#context.getBackups();
    } catch {
      this._backups = [];
    }
    this._loading = false;
  }

  public async refresh(): Promise<void> {
    await this.#loadBackups();
  }

  async #handleDelete(name: string): Promise<void> {
    if (!this.#context) return;
    if (!confirm(`Delete backup '${name}'? This cannot be undone.`)) return;

    try {
      await this.#context.deleteBackup(name);
      this.dispatchEvent(
        new CustomEvent("backup-deleted", { detail: { name }, bubbles: true, composed: true }),
      );
      await this.#loadBackups();
    } catch {
      this.dispatchEvent(
        new CustomEvent("backup-error", {
          detail: { message: "Failed to delete backup" },
          bubbles: true,
          composed: true,
        }),
      );
    }
  }

  #formatDate(iso: string): string {
    try {
      return new Date(iso).toLocaleString();
    } catch {
      return iso;
    }
  }

  override render() {
    if (this._loading) {
      return html`<uui-loader-bar></uui-loader-bar>`;
    }

    if (this._backups.length === 0) {
      return html`
        <div class="empty-state">
          <p>No backups found. Create your first backup above.</p>
        </div>
      `;
    }

    return html`
      <uui-table>
        <uui-table-head>
          <uui-table-head-cell>Name</uui-table-head-cell>
          <uui-table-head-cell>Date</uui-table-head-cell>
          <uui-table-head-cell>Size</uui-table-head-cell>
          <uui-table-head-cell>Type</uui-table-head-cell>
          <uui-table-head-cell>Actions</uui-table-head-cell>
        </uui-table-head>
        ${this._backups.map(
          (b) => html`
            <uui-table-row>
              <uui-table-cell>${b.name}</uui-table-cell>
              <uui-table-cell>${this.#formatDate(b.createdAt)}</uui-table-cell>
              <uui-table-cell>${this.#context?.formatSize(b.sizeBytes) ?? b.sizeBytes}</uui-table-cell>
              <uui-table-cell>
                <span class="extension-badge">${b.extension || "zip"}</span>
                ${b.isCompressed ? html`<uui-tag look="secondary" size="small">Compressed</uui-tag>` : nothing}
              </uui-table-cell>
              <uui-table-cell>
                <div class="actions">
                  <uui-button
                    look="secondary"
                    compact
                    label="Delete"
                    @click=${() => this.#handleDelete(b.name)}
                  >
                    <uui-icon name="icon-delete"></uui-icon>
                  </uui-button>
                </div>
              </uui-table-cell>
            </uui-table-row>
          `,
        )}
      </uui-table>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "backups-history-table": BackupsHistoryTableElement;
  }
}
