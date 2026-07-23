import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface EmailStyle {
  id: number;
  headerHtml?: string;
  footerHtml?: string;
  globalCss?: string;
  logoUrl?: string;
  primaryColor?: string;
  companyName?: string;
  updatedAt?: string;
}

@customElement("emailstyles-dashboard")
export class EmailStylesDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
    }

    h2 {
      font-size: 1.25rem;
      font-weight: 600;
      margin: 0 0 8px;
    }

    .description {
      color: var(--uui-color-text-alt, #6b7280);
      margin: 0 0 24px;
      max-width: 600px;
      line-height: 1.6;
      font-size: 0.875rem;
    }

    .form-card {
      background: var(--uui-color-surface, #fff);
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 8px;
      padding: 24px;
      max-width: 720px;
    }

    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
    }

    .form-field {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .form-field.span-2 {
      grid-column: span 2;
    }

    .form-field label {
      font-size: 0.8rem;
      font-weight: 600;
      color: var(--uui-color-text, #374151);
    }

    .form-field input,
    .form-field textarea {
      padding: 8px 12px;
      border: 1px solid var(--uui-color-border, #d1d5db);
      border-radius: 6px;
      font-size: 0.875rem;
      font-family: inherit;
      background: var(--uui-color-surface, #fff);
      color: var(--uui-color-text, #111827);
    }

    .form-field textarea {
      min-height: 100px;
      resize: vertical;
      font-family: monospace;
    }

    .form-field textarea.tall {
      min-height: 200px;
    }

    .form-field input[type="color"] {
      padding: 4px;
      height: 40px;
      cursor: pointer;
    }

    .form-actions {
      display: flex;
      gap: 8px;
      margin-top: 20px;
    }

    .feedback {
      padding: 12px 16px;
      border-radius: 6px;
      margin-bottom: 16px;
      font-size: 0.875rem;
    }

    .feedback-success {
      background: #d1fae5;
      color: #065f46;
      border: 1px solid #a7f3d0;
    }

    .feedback-error {
      background: #fee2e2;
      color: #991b1b;
      border: 1px solid #fecaca;
    }

    .last-updated {
      font-size: 0.75rem;
      color: var(--uui-color-text-alt, #9ca3af);
      margin-top: 12px;
    }
  `;

  @state() private _loading = true;
  @state() private _saving = false;
  @state() private _style: EmailStyle = { id: 1 };
  @state() private _message: { type: "success" | "error"; text: string } | null = null;
  private readonly _apiBase = "/umbraco/management/api/v1/email-style";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadStyle();
  }

  private async _loadStyle(): Promise<void> {
    this._loading = true;
    try {
      const res = await fetch(this._apiBase, { credentials: "include" });
      if (res.ok) {
        this._style = await res.json();
      }
    } catch {
      this._message = { type: "error", text: "Failed to load style settings." };
    } finally {
      this._loading = false;
    }
  }

  private async _saveStyle(): Promise<void> {
    this._saving = true;
    this._message = null;
    try {
      const res = await fetch(this._apiBase, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(this._style),
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const saved = await res.json();
      this._style = saved;
      this._message = { type: "success", text: "Style settings saved." };
    } catch {
      this._message = { type: "error", text: "Failed to save style settings." };
    } finally {
      this._saving = false;
    }
  }

  private _formatDate(iso?: string): string {
    if (!iso) return "Never";
    return new Date(iso).toLocaleString(undefined, {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  }

  override render(): unknown {
    if (this._loading) {
      return html`<p>Loading style settings...</p>`;
    }

    return html`
      <h2>Style Settings</h2>
      <p class="description">
        Configure the global email style that is applied to all template previews: header and footer HTML,
        CSS styles, brand colors, company name, and logo.
      </p>

      ${this._message
        ? html`<div class="feedback feedback-${this._message.type}">${this._message.text}</div>`
        : ""}

      <div class="form-card">
        <div class="form-grid">
          <div class="form-field">
            <label for="style-company">Company Name</label>
            <input
              id="style-company"
              type="text"
              .value=${this._style.companyName || ""}
              @input=${(e: InputEvent) => {
                this._style.companyName = (e.target as HTMLInputElement).value;
              }}
              placeholder="My Company"
            />
          </div>
          <div class="form-field">
            <label for="style-logo">Logo URL</label>
            <input
              id="style-logo"
              type="text"
              .value=${this._style.logoUrl || ""}
              @input=${(e: InputEvent) => {
                this._style.logoUrl = (e.target as HTMLInputElement).value;
              }}
              placeholder="https://example.com/logo.png"
            />
          </div>
          <div class="form-field">
            <label for="style-color">Primary Color</label>
            <input
              id="style-color"
              type="text"
              .value=${this._style.primaryColor || ""}
              @input=${(e: InputEvent) => {
                this._style.primaryColor = (e.target as HTMLInputElement).value;
              }}
              placeholder="#333333"
            />
          </div>
          <div class="form-field">
            <label for="style-color-picker">&nbsp;</label>
            <input
              id="style-color-picker"
              type="color"
              .value=${this._style.primaryColor || "#333333"}
              @input=${(e: InputEvent) => {
                this._style.primaryColor = (e.target as HTMLInputElement).value;
              }}
            />
          </div>
          <div class="form-field span-2">
            <label for="style-header">Header HTML</label>
            <textarea
              id="style-header"
              .value=${this._style.headerHtml || ""}
              @input=${(e: InputEvent) => {
                this._style.headerHtml = (e.target as HTMLTextAreaElement).value;
              }}
              placeholder="<div style='...'>Header content</div>"
            ></textarea>
          </div>
          <div class="form-field span-2">
            <label for="style-footer">Footer HTML</label>
            <textarea
              id="style-footer"
              .value=${this._style.footerHtml || ""}
              @input=${(e: InputEvent) => {
                this._style.footerHtml = (e.target as HTMLTextAreaElement).value;
              }}
              placeholder="<div style='...'>Footer content</div>"
            ></textarea>
          </div>
          <div class="form-field span-2">
            <label for="style-css">Global CSS</label>
            <textarea
              id="style-css"
              class="tall"
              .value=${this._style.globalCss || ""}
              @input=${(e: InputEvent) => {
                this._style.globalCss = (e.target as HTMLTextAreaElement).value;
              }}
              placeholder="a { color: #0066cc; }"
            ></textarea>
          </div>
        </div>
        <div class="form-actions">
          <uui-button
            look="primary"
            label="Save Style Settings"
            ?disabled=${this._saving}
            @click=${this._saveStyle}
          >
            ${this._saving ? "Saving..." : "Save Style Settings"}
          </uui-button>
        </div>
        <p class="last-updated">Last updated: ${this._formatDate(this._style.updatedAt)}</p>
      </div>
    `;
  }
}

export default EmailStylesDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "emailstyles-dashboard": EmailStylesDashboardElement;
  }
}
