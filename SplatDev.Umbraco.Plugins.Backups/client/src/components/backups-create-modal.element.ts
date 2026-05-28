import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
  BackupsContext,
  BACKUPS_CONTEXT_TOKEN,
  type CloudProviderInfo,
} from "../context/backups.context.js";

const SCOPE_OPTIONS = [
  { value: 1, label: "Content" },
  { value: 2, label: "Media" },
  { value: 4, label: "Database" },
  { value: 3, label: "Content + Media" },
  { value: 7, label: "Full (Content + Media + Database)" },
];

@customElement("backups-create-modal")
export class BackupsCreateModalElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host {
      display: block;
    }
    .form-container {
      display: none;
      margin-top: var(--uui-size-space-4, 12px);
    }
    :host([open]) .form-container {
      display: block;
    }
    .form-row {
      display: flex;
      flex-direction: column;
      gap: 4px;
      margin-bottom: var(--uui-size-space-4, 12px);
      max-width: 480px;
    }
    .form-row label {
      font-weight: 500;
      font-size: 0.875rem;
    }
    .toggle-row {
      display: flex;
      align-items: center;
      gap: var(--uui-size-space-3, 8px);
      margin-bottom: var(--uui-size-space-3, 8px);
    }
    .button-row {
      display: flex;
      gap: var(--uui-size-space-3, 8px);
      margin-top: var(--uui-size-space-5, 16px);
    }
  `;

  @state() private _open = false;
  @state() private _creating = false;
  @state() private _providers: CloudProviderInfo[] = [];

  @state() private _name = "";
  @state() private _includeMedia = true;
  @state() private _scope = 7;
  @state() private _compress = true;
  @state() private _selectedProviders: string[] = [];

  #context?: BackupsContext;

  override connectedCallback(): void {
    super.connectedCallback();
    this.consumeContext(BACKUPS_CONTEXT_TOKEN, (ctx) => {
      this.#context = ctx;
      this.#loadProviders();
    });
  }

  async #loadProviders(): Promise<void> {
    if (!this.#context) return;
    try {
      this._providers = await this.#context.getProviders();
    } catch {
      this._providers = [];
    }
  }

  #toggle(): void {
    this._open = !this._open;
    if (this._open) {
      this.setAttribute("open", "");
    } else {
      this.removeAttribute("open");
    }
  }

  async #handleSubmit(): Promise<void> {
    if (!this.#context) return;
    if (!this._name.trim()) return;

    this._creating = true;
    try {
      await this.#context.createBackup({
        name: this._name.trim(),
        includeMedia: this._includeMedia,
        scope: this._scope,
        compress: this._compress,
        encrypt: false,
        encryptionKey: "",
        cloudProviderIds: this._selectedProviders,
      });
      this._name = "";
      this._open = false;
      this.removeAttribute("open");
      this.dispatchEvent(
        new CustomEvent("backup-created", { bubbles: true, composed: true }),
      );
    } catch {
      this.dispatchEvent(
        new CustomEvent("backup-error", {
          detail: { message: "Failed to create backup" },
          bubbles: true,
          composed: true,
        }),
      );
    }
    this._creating = false;
  }

  override render() {
    const enabledProviders = this._providers.filter((p) => p.enabled);
    const scopeSelectOptions = SCOPE_OPTIONS.map((s) => ({
      name: s.label,
      value: String(s.value),
      selected: s.value === this._scope,
    }));

    return html`
      <uui-button
        look="primary"
        label=${this._creating ? "Creating…" : "Create Backup"}
        ?disabled=${this._creating}
        @click=${this.#toggle}
      >
        ${this._creating ? "Creating…" : "Create Backup"}
      </uui-button>

      <div class="form-container">
        <uui-box headline="New Backup">
          <div class="form-row">
            <label for="backup-name">Backup Name</label>
            <uui-input
              id="backup-name"
              placeholder="e.g. pre-deploy-2026-05-28"
              .value=${this._name}
              @input=${(e: InputEvent) => {
                this._name = (e.target as HTMLInputElement).value;
              }}
            ></uui-input>
          </div>

          <div class="form-row">
            <label>Scope</label>
            <uui-select
              .options=${scopeSelectOptions}
              @change=${(e: Event) => {
                this._scope = Number((e.target as HTMLSelectElement).value);
              }}
            ></uui-select>
          </div>

          <div class="toggle-row">
            <uui-toggle
              label="Include Media"
              ?checked=${this._includeMedia}
              @change=${(e: Event) => {
                this._includeMedia = (e.target as HTMLInputElement).checked;
              }}
            ></uui-toggle>
          </div>

          <div class="toggle-row">
            <uui-toggle
              label="Compress"
              ?checked=${this._compress}
              @change=${(e: Event) => {
                this._compress = (e.target as HTMLInputElement).checked;
              }}
            ></uui-toggle>
          </div>

          ${enabledProviders.length > 0
            ? html`
                <div class="form-row">
                  <label>Cloud Storage Destinations</label>
                  ${enabledProviders.map(
                    (p) => html`
                      <div class="toggle-row">
                        <uui-checkbox
                          label=${p.providerType}
                          ?checked=${this._selectedProviders.includes(p.id)}
                          @change=${(e: Event) => {
                            const checked = (e.target as HTMLInputElement).checked;
                            if (checked) {
                              this._selectedProviders = [...this._selectedProviders, p.id];
                            } else {
                              this._selectedProviders = this._selectedProviders.filter(
                                (id) => id !== p.id,
                              );
                            }
                          }}
                        ></uui-checkbox>
                      </div>
                    `,
                  )}
                </div>
              `
            : ""}

          <div class="button-row">
            <uui-button
              look="primary"
              label="Run Backup Now"
              ?disabled=${this._creating || !this._name.trim()}
              @click=${this.#handleSubmit}
            >
              ${this._creating ? "Running…" : "Run Backup Now"}
            </uui-button>
            <uui-button
              look="secondary"
              label="Cancel"
              @click=${this.#toggle}
            >
              Cancel
            </uui-button>
          </div>
        </uui-box>
      </div>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "backups-create-modal": BackupsCreateModalElement;
  }
}
