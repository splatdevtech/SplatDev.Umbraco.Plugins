import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface EmailTemplate {
  id: number;
  name: string;
  subject: string;
  htmlBody: string;
  textBody?: string;
  variables?: string;
  category: string;
  createdAt: string;
  updatedAt?: string;
}

type FormMode = "list" | "create" | "edit";

@customElement("emailtemplates-dashboard")
export class EmailTemplatesDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
    }

    h2 {
      font-size: 1.25rem;
      font-weight: 600;
      margin: 0;
    }

    .table-wrapper {
      background: var(--uui-color-surface, #fff);
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 8px;
      overflow: hidden;
      margin-bottom: 24px;
    }

    .empty-state {
      text-align: center;
      padding: 48px 24px;
      color: var(--uui-color-text-alt, #6b7280);
    }

    table {
      width: 100%;
      border-collapse: collapse;
    }

    th {
      text-align: left;
      padding: 12px 16px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      color: var(--uui-color-text-alt, #6b7280);
      background: var(--uui-color-surface-alt, #f9fafb);
      border-bottom: 1px solid var(--uui-color-border, #e5e7eb);
    }

    td {
      padding: 12px 16px;
      border-bottom: 1px solid var(--uui-color-border, #e5e7eb);
      font-size: 0.875rem;
    }

    tr:last-child td {
      border-bottom: none;
    }

    tr:hover td {
      background: var(--uui-color-surface-alt, #f9fafb);
    }

    .badge {
      display: inline-block;
      padding: 2px 10px;
      border-radius: 9999px;
      font-size: 0.7rem;
      font-weight: 600;
      text-transform: uppercase;
    }

    .badge-transactional {
      background: #dbeafe;
      color: #1e40af;
    }

    .badge-marketing {
      background: #fef3c7;
      color: #92400e;
    }

    .actions-cell {
      display: flex;
      gap: 6px;
    }

    .form-card {
      background: var(--uui-color-surface, #fff);
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 8px;
      padding: 24px;
      margin-bottom: 24px;
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
    .form-field select,
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
      min-height: 120px;
      resize: vertical;
    }

    .form-field textarea.code {
      font-family: monospace;
      min-height: 250px;
    }

    .form-actions {
      display: flex;
      gap: 8px;
      margin-top: 16px;
    }

    .preview-frame {
      width: 100%;
      min-height: 400px;
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 6px;
    }

    .var-chips {
      display: flex;
      flex-wrap: wrap;
      gap: 6px;
      margin-top: 8px;
    }

    .var-chip {
      display: inline-block;
      padding: 2px 10px;
      border-radius: 4px;
      font-size: 0.75rem;
      font-family: monospace;
      background: var(--uui-color-surface-alt, #f3f4f6);
      border: 1px solid var(--uui-color-border, #e5e7eb);
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

    .flex-row {
      display: flex;
      gap: 16px;
      align-items: flex-start;
    }

    .flex-col {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
  `;

  @state() private _mode: FormMode = "list";
  @state() private _templates: EmailTemplate[] = [];
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  @state() private _form: EmailTemplate = {
    id: 0,
    name: "",
    subject: "",
    htmlBody: "",
    textBody: "",
    variables: "",
    category: "transactional",
    createdAt: "",
  };

  @state() private _previewHtml: string | null = null;
  @state() private _editingId: number | null = null;
  private readonly _apiBase = "/umbraco/management/api/v1/email-templates";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadTemplates();
  }

  private async _loadTemplates(): Promise<void> {
    this._loading = true;
    try {
      const res = await fetch(this._apiBase, { credentials: "include" });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      this._templates = await res.json();
    } catch {
      this._message = { type: "error", text: "Failed to load templates." };
    } finally {
      this._loading = false;
    }
  }

  private _startCreate(): void {
    this._editingId = null;
    this._form = {
      id: 0,
      name: "",
      subject: "",
      htmlBody: "",
      textBody: "",
      variables: "",
      category: "transactional",
      createdAt: "",
    };
    this._previewHtml = null;
    this._message = null;
    this._mode = "create";
  }

  private _startEdit(template: EmailTemplate): void {
    this._editingId = template.id;
    this._form = { ...template };
    this._previewHtml = null;
    this._message = null;
    this._mode = "edit";
  }

  private _cancelForm(): void {
    this._mode = "list";
    this._message = null;
    this._previewHtml = null;
  }

  private async _submitForm(): Promise<void> {
    this._message = null;
    const url = this._editingId ? `${this._apiBase}/${this._editingId}` : this._apiBase;
    const method = this._editingId ? "PUT" : "POST";

    const body: Record<string, unknown> = {
      name: this._form.name,
      subject: this._form.subject,
      htmlBody: this._form.htmlBody,
      textBody: this._form.textBody || null,
      variables: this._form.variables || null,
      category: this._form.category,
    };

    try {
      const res = await fetch(url, {
        method,
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(body),
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      this._message = { type: "success", text: this._editingId ? "Template updated." : "Template created." };
      await this._loadTemplates();
      this._mode = "list";
    } catch {
      this._message = { type: "error", text: "Failed to save template." };
    }
  }

  private async _deleteTemplate(id: number): Promise<void> {
    if (!confirm("Delete this template?")) return;
    try {
      const res = await fetch(`${this._apiBase}/${id}`, {
        method: "DELETE",
        credentials: "include",
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      this._message = { type: "success", text: "Template deleted." };
      await this._loadTemplates();
    } catch {
      this._message = { type: "error", text: "Failed to delete template." };
    }
  }

  private async _previewTemplate(id: number): Promise<void> {
    try {
      const res = await fetch(`${this._apiBase}/${id}/preview`, { credentials: "include" });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      this._previewHtml = await res.text();
    } catch {
      this._message = { type: "error", text: "Failed to load preview." };
    }
  }

  private async _loadVariables(id: number): Promise<void> {
    try {
      const res = await fetch(`${this._apiBase}/${id}/variables`, { credentials: "include" });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const vars: string[] = await res.json();
      this._form.variables = vars.join(", ");
    } catch {
      this._message = { type: "error", text: "Failed to extract variables." };
    }
  }

  private _formatDate(iso: string): string {
    if (!iso) return "";
    return new Date(iso).toLocaleDateString(undefined, {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  }

  private _renderList(): unknown {
    if (this._loading) {
      return html`<div class="empty-state">Loading templates...</div>`;
    }

    if (this._templates.length === 0) {
      return html`
        <div class="empty-state">
          <p><strong>No email templates yet.</strong></p>
          <p>Create your first template to get started.</p>
        </div>
      `;
    }

    return html`
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Subject</th>
            <th>Category</th>
            <th>Created</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          ${this._templates.map(
            (t) => html`
              <tr>
                <td><strong>${t.name}</strong></td>
                <td>${t.subject}</td>
                <td>
                  <span class="badge badge-${t.category === "transactional" ? "transactional" : "marketing"}">
                    ${t.category}
                  </span>
                </td>
                <td>${this._formatDate(t.createdAt)}</td>
                <td>
                  <div class="actions-cell">
                    <uui-button
                      look="secondary"
                      label="Preview"
                      color="positive"
                      @click=${() => this._previewTemplate(t.id)}
                    >
                      Preview
                    </uui-button>
                    <uui-button look="secondary" label="Edit" @click=${() => this._startEdit(t)}>
                      Edit
                    </uui-button>
                    <uui-button
                      look="secondary"
                      label="Delete"
                      color="danger"
                      @click=${() => this._deleteTemplate(t.id)}
                    >
                      Delete
                    </uui-button>
                  </div>
                </td>
              </tr>
            `
          )}
        </tbody>
      </table>
    `;
  }

  private _renderForm(): unknown {
    const isEdit = this._mode === "edit";

    return html`
      <div class="form-card">
        <h3 style="margin: 0 0 16px; font-size: 1rem; font-weight: 600;">
          ${isEdit ? "Edit Template" : "Create Template"}
        </h3>
        <div class="form-grid">
          <div class="form-field">
            <label for="tpl-name">Name *</label>
            <input
              id="tpl-name"
              type="text"
              .value=${this._form.name}
              @input=${(e: InputEvent) => {
                this._form.name = (e.target as HTMLInputElement).value;
              }}
              required
            />
          </div>
          <div class="form-field">
            <label for="tpl-category">Category</label>
            <select
              id="tpl-category"
              .value=${this._form.category}
              @change=${(e: Event) => {
                this._form.category = (e.target as HTMLSelectElement).value;
              }}
            >
              <option value="transactional">Transactional</option>
              <option value="marketing">Marketing</option>
              <option value="notification">Notification</option>
              <option value="other">Other</option>
            </select>
          </div>
          <div class="form-field span-2">
            <label for="tpl-subject">Subject *</label>
            <input
              id="tpl-subject"
              type="text"
              .value=${this._form.subject}
              @input=${(e: InputEvent) => {
                this._form.subject = (e.target as HTMLInputElement).value;
              }}
              required
            />
          </div>
          <div class="form-field span-2">
            <label for="tpl-body">HTML Body *</label>
            <textarea
              id="tpl-body"
              class="code"
              .value=${this._form.htmlBody}
              @input=${(e: InputEvent) => {
                this._form.htmlBody = (e.target as HTMLTextAreaElement).value;
              }}
              placeholder="<p>Hello {{name}},</p>"
              required
            ></textarea>
          </div>
          <div class="form-field span-2">
            <label for="tpl-textbody">Plain Text Body</label>
            <textarea
              id="tpl-textbody"
              .value=${this._form.textBody || ""}
              @input=${(e: InputEvent) => {
                this._form.textBody = (e.target as HTMLTextAreaElement).value;
              }}
              placeholder="Optional plain-text version"
            ></textarea>
          </div>
          <div class="form-field span-2">
            <label for="tpl-vars">Variables</label>
            <input
              id="tpl-vars"
              type="text"
              .value=${this._form.variables || ""}
              @input=${(e: InputEvent) => {
                this._form.variables = (e.target as HTMLInputElement).value;
              }}
              placeholder="name, email, company (comma-separated)"
            />
            ${isEdit ? html`
              <uui-button
                look="secondary"
                label="Auto-extract"
                style="margin-top: 8px; align-self: flex-start;"
                @click=${() => this._loadVariables(this._editingId!)}
              >
                Auto-extract from template
              </uui-button>
            ` : nothing}
          </div>
        </div>
        <div class="form-actions">
          <uui-button look="primary" label="Save" @click=${this._submitForm}>
            ${isEdit ? "Update Template" : "Create Template"}
          </uui-button>
          <uui-button look="secondary" label="Cancel" @click=${this._cancelForm}>
            Cancel
          </uui-button>
        </div>
      </div>
    `;
  }

  override render(): unknown {
    return html`
      <div class="header">
        <h2>Templates</h2>
        ${this._mode === "list"
          ? html`
              <uui-button look="primary" label="Create Template" @click=${this._startCreate}>
                Create Template
              </uui-button>
            `
          : nothing}
      </div>

      ${this._message
        ? html`<div class="feedback feedback-${this._message.type}">${this._message.text}</div>`
        : nothing}

      ${this._previewHtml
        ? html`
            <div class="form-card">
              <h3 style="margin: 0 0 12px; font-size: 1rem; font-weight: 600;">Preview</h3>
              <iframe class="preview-frame" srcdoc=${this._previewHtml} title="Email preview"></iframe>
              <div class="form-actions" style="margin-top: 12px;">
                <uui-button look="secondary" label="Close Preview" @click=${() => { this._previewHtml = null; }}>
                  Close Preview
                </uui-button>
              </div>
            </div>
          `
        : nothing}

      ${this._mode === "list"
        ? html`<div class="table-wrapper">${this._renderList()}</div>`
        : this._renderForm()}
    `;
  }
}

export default EmailTemplatesDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "emailtemplates-dashboard": EmailTemplatesDashboardElement;
  }
}
