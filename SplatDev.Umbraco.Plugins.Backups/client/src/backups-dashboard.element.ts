import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { BackupsContext, BACKUPS_CONTEXT_TOKEN } from "./context/backups.context.js";
import "./components/backups-create-modal.element.js";
import "./components/backups-history-table.element.js";
import type { BackupsHistoryTableElement } from "./components/backups-history-table.element.js";

@customElement("backups-dashboard")
export class BackupsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
    }
    h2 {
      font-size: 1.5rem;
      font-weight: 600;
      margin: 0 0 var(--uui-size-space-2, 4px);
    }
    .description {
      color: var(--uui-color-text-alt, #6b7280);
      margin: 0 0 var(--uui-size-space-6, 24px);
    }
    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: var(--uui-size-space-5, 16px);
    }
    .section-title {
      font-size: 1rem;
      font-weight: 600;
      margin: 0;
    }
  `;

  @state() private _notificationMessage = "";
  @state() private _notificationType: "positive" | "danger" | "" = "";

  #context!: BackupsContext;

  override connectedCallback(): void {
    super.connectedCallback();
    this.#context = new BackupsContext(this);
    this.provideContext(BACKUPS_CONTEXT_TOKEN, this.#context);
  }

  #getHistoryTable(): BackupsHistoryTableElement | null {
    return this.renderRoot.querySelector("backups-history-table");
  }

  #handleBackupCreated(): void {
    this.#showNotification("Backup started successfully.", "positive");
    this.#getHistoryTable()?.refresh();
  }

  #handleBackupDeleted(): void {
    this.#showNotification("Backup deleted.", "positive");
  }

  #handleBackupError(e: CustomEvent<{ message: string }>): void {
    this.#showNotification(e.detail.message, "danger");
  }

  #showNotification(message: string, type: "positive" | "danger"): void {
    this._notificationMessage = message;
    this._notificationType = type;
    setTimeout(() => {
      this._notificationMessage = "";
      this._notificationType = "";
    }, 4000);
  }

  override render() {
    return html`
      <h2>Backups</h2>
      <p class="description">
        Create and manage quick backups from the backoffice.
      </p>

      ${this._notificationMessage
        ? html`
            <uui-toast-notification
              color=${this._notificationType}
              auto-close="4000"
            >
              <uui-toast-notification-layout headline="Backups">
                ${this._notificationMessage}
              </uui-toast-notification-layout>
            </uui-toast-notification>
          `
        : ""}

      <backups-create-modal
        @backup-created=${this.#handleBackupCreated}
        @backup-error=${this.#handleBackupError}
      ></backups-create-modal>

      <div style="margin-top: var(--uui-size-space-6, 24px);">
        <div class="section-header">
          <h3 class="section-title">Backup History</h3>
        </div>
        <backups-history-table
          @backup-deleted=${this.#handleBackupDeleted}
          @backup-error=${this.#handleBackupError}
        ></backups-history-table>
      </div>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "backups-dashboard": BackupsDashboardElement;
  }
}
