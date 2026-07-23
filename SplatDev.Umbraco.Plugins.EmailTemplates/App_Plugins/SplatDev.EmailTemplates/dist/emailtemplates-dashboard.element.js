import { LitElement as m, html as a, nothing as n, css as u, state as d, customElement as f } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as h } from "@umbraco-cms/backoffice/element-api";
var b = Object.defineProperty, g = Object.getOwnPropertyDescriptor, i = (e, t, o, l) => {
  for (var s = l > 1 ? void 0 : l ? g(t, o) : t, p = e.length - 1, c; p >= 0; p--)
    (c = e[p]) && (s = (l ? c(t, o, s) : c(s)) || s);
  return l && s && b(t, o, s), s;
};
let r = class extends h(m) {
  constructor() {
    super(...arguments), this._mode = "list", this._templates = [], this._loading = !1, this._message = null, this._form = {
      id: 0,
      name: "",
      subject: "",
      htmlBody: "",
      textBody: "",
      variables: "",
      category: "transactional",
      createdAt: ""
    }, this._previewHtml = null, this._editingId = null, this._apiBase = "/umbraco/management/api/v1/email-templates";
  }
  connectedCallback() {
    super.connectedCallback(), this._loadTemplates();
  }
  async _loadTemplates() {
    this._loading = !0;
    try {
      const e = await fetch(this._apiBase, { credentials: "include" });
      if (!e.ok) throw new Error(`HTTP ${e.status}`);
      this._templates = await e.json();
    } catch {
      this._message = { type: "error", text: "Failed to load templates." };
    } finally {
      this._loading = !1;
    }
  }
  _startCreate() {
    this._editingId = null, this._form = {
      id: 0,
      name: "",
      subject: "",
      htmlBody: "",
      textBody: "",
      variables: "",
      category: "transactional",
      createdAt: ""
    }, this._previewHtml = null, this._message = null, this._mode = "create";
  }
  _startEdit(e) {
    this._editingId = e.id, this._form = { ...e }, this._previewHtml = null, this._message = null, this._mode = "edit";
  }
  _cancelForm() {
    this._mode = "list", this._message = null, this._previewHtml = null;
  }
  async _submitForm() {
    this._message = null;
    const e = this._editingId ? `${this._apiBase}/${this._editingId}` : this._apiBase, t = this._editingId ? "PUT" : "POST", o = {
      name: this._form.name,
      subject: this._form.subject,
      htmlBody: this._form.htmlBody,
      textBody: this._form.textBody || null,
      variables: this._form.variables || null,
      category: this._form.category
    };
    try {
      const l = await fetch(e, {
        method: t,
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(o)
      });
      if (!l.ok) throw new Error(`HTTP ${l.status}`);
      this._message = { type: "success", text: this._editingId ? "Template updated." : "Template created." }, await this._loadTemplates(), this._mode = "list";
    } catch {
      this._message = { type: "error", text: "Failed to save template." };
    }
  }
  async _deleteTemplate(e) {
    if (confirm("Delete this template?"))
      try {
        const t = await fetch(`${this._apiBase}/${e}`, {
          method: "DELETE",
          credentials: "include"
        });
        if (!t.ok) throw new Error(`HTTP ${t.status}`);
        this._message = { type: "success", text: "Template deleted." }, await this._loadTemplates();
      } catch {
        this._message = { type: "error", text: "Failed to delete template." };
      }
  }
  async _previewTemplate(e) {
    try {
      const t = await fetch(`${this._apiBase}/${e}/preview`, { credentials: "include" });
      if (!t.ok) throw new Error(`HTTP ${t.status}`);
      this._previewHtml = await t.text();
    } catch {
      this._message = { type: "error", text: "Failed to load preview." };
    }
  }
  async _loadVariables(e) {
    try {
      const t = await fetch(`${this._apiBase}/${e}/variables`, { credentials: "include" });
      if (!t.ok) throw new Error(`HTTP ${t.status}`);
      const o = await t.json();
      this._form.variables = o.join(", ");
    } catch {
      this._message = { type: "error", text: "Failed to extract variables." };
    }
  }
  _formatDate(e) {
    return e ? new Date(e).toLocaleDateString(void 0, {
      year: "numeric",
      month: "short",
      day: "numeric"
    }) : "";
  }
  _renderList() {
    return this._loading ? a`<div class="empty-state">Loading templates...</div>` : this._templates.length === 0 ? a`
        <div class="empty-state">
          <p><strong>No email templates yet.</strong></p>
          <p>Create your first template to get started.</p>
        </div>
      ` : a`
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
      (e) => a`
              <tr>
                <td><strong>${e.name}</strong></td>
                <td>${e.subject}</td>
                <td>
                  <span class="badge badge-${e.category === "transactional" ? "transactional" : "marketing"}">
                    ${e.category}
                  </span>
                </td>
                <td>${this._formatDate(e.createdAt)}</td>
                <td>
                  <div class="actions-cell">
                    <uui-button
                      look="secondary"
                      label="Preview"
                      color="positive"
                      @click=${() => this._previewTemplate(e.id)}
                    >
                      Preview
                    </uui-button>
                    <uui-button look="secondary" label="Edit" @click=${() => this._startEdit(e)}>
                      Edit
                    </uui-button>
                    <uui-button
                      look="secondary"
                      label="Delete"
                      color="danger"
                      @click=${() => this._deleteTemplate(e.id)}
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
  _renderForm() {
    const e = this._mode === "edit";
    return a`
      <div class="form-card">
        <h3 style="margin: 0 0 16px; font-size: 1rem; font-weight: 600;">
          ${e ? "Edit Template" : "Create Template"}
        </h3>
        <div class="form-grid">
          <div class="form-field">
            <label for="tpl-name">Name *</label>
            <input
              id="tpl-name"
              type="text"
              .value=${this._form.name}
              @input=${(t) => {
      this._form.name = t.target.value;
    }}
              required
            />
          </div>
          <div class="form-field">
            <label for="tpl-category">Category</label>
            <select
              id="tpl-category"
              .value=${this._form.category}
              @change=${(t) => {
      this._form.category = t.target.value;
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
              @input=${(t) => {
      this._form.subject = t.target.value;
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
              @input=${(t) => {
      this._form.htmlBody = t.target.value;
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
              @input=${(t) => {
      this._form.textBody = t.target.value;
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
              @input=${(t) => {
      this._form.variables = t.target.value;
    }}
              placeholder="name, email, company (comma-separated)"
            />
            ${e ? a`
              <uui-button
                look="secondary"
                label="Auto-extract"
                style="margin-top: 8px; align-self: flex-start;"
                @click=${() => this._loadVariables(this._editingId)}
              >
                Auto-extract from template
              </uui-button>
            ` : n}
          </div>
        </div>
        <div class="form-actions">
          <uui-button look="primary" label="Save" @click=${this._submitForm}>
            ${e ? "Update Template" : "Create Template"}
          </uui-button>
          <uui-button look="secondary" label="Cancel" @click=${this._cancelForm}>
            Cancel
          </uui-button>
        </div>
      </div>
    `;
  }
  render() {
    return a`
      <div class="header">
        <h2>Templates</h2>
        ${this._mode === "list" ? a`
              <uui-button look="primary" label="Create Template" @click=${this._startCreate}>
                Create Template
              </uui-button>
            ` : n}
      </div>

      ${this._message ? a`<div class="feedback feedback-${this._message.type}">${this._message.text}</div>` : n}

      ${this._previewHtml ? a`
            <div class="form-card">
              <h3 style="margin: 0 0 12px; font-size: 1rem; font-weight: 600;">Preview</h3>
              <iframe class="preview-frame" srcdoc=${this._previewHtml} title="Email preview"></iframe>
              <div class="form-actions" style="margin-top: 12px;">
                <uui-button look="secondary" label="Close Preview" @click=${() => {
      this._previewHtml = null;
    }}>
                  Close Preview
                </uui-button>
              </div>
            </div>
          ` : n}

      ${this._mode === "list" ? a`<div class="table-wrapper">${this._renderList()}</div>` : this._renderForm()}
    `;
  }
};
r.styles = u`
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
i([
  d()
], r.prototype, "_mode", 2);
i([
  d()
], r.prototype, "_templates", 2);
i([
  d()
], r.prototype, "_loading", 2);
i([
  d()
], r.prototype, "_message", 2);
i([
  d()
], r.prototype, "_form", 2);
i([
  d()
], r.prototype, "_previewHtml", 2);
i([
  d()
], r.prototype, "_editingId", 2);
r = i([
  f("emailtemplates-dashboard")
], r);
const x = r;
export {
  r as EmailTemplatesDashboardElement,
  x as default
};
