import { LitElement, html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface FormSummary {
  id: number;
  name: string;
  category: string;
  createdDate: string;
  fieldCount: number;
}

const API = "/umbraco/api/formbuilder";

@customElement("formbuilder-form-list")
export class FormListDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    .desc { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 12px; margin-bottom: 16px; }
    .table-wrap { overflow-x: auto; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 8px 12px; border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); vertical-align: middle; }
    .empty { text-align: center; padding: 48px; color: var(--uui-color-text-alt, #6b7280); }
    .empty-icon { font-size: 3rem; margin-bottom: 12px; }
    .actions { display: flex; gap: 6px; }
  `;

  @state() private _forms: FormSummary[] = [];
  @state() private _loading = false;

  override connectedCallback(): void {
    super.connectedCallback();
    this._load();
  }

  private async _load(): Promise<void> {
    this._loading = true;
    try {
      const res = await fetch(`${API}/GetAllForms`);
      if (res.ok) this._forms = (await res.json()) as FormSummary[];
    } finally {
      this._loading = false;
    }
  }

  private async _createForm(): Promise<void> {
    const name = prompt("Form name:");
    if (!name?.trim()) return;
    await fetch(`${API}/CreateForm`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name: name.trim() }),
    });
    await this._load();
  }

  private async _duplicateForm(id: number): Promise<void> {
    if (!confirm("Duplicate this form?")) return;
    await fetch(`${API}/DuplicateForm?id=${id}`, { method: "POST" });
    await this._load();
  }

  private async _deleteForm(id: number, name: string): Promise<void> {
    if (!confirm(`Delete "${name}"? This cannot be undone.`)) return;
    const res = await fetch(`${API}/DeleteForm?id=${id}`, { method: "DELETE" });
    if (!res.ok) {
      const err = await res.text();
      alert(`Delete failed: ${err}`);
    }
    await this._load();
  }

  override render() {
    return html`
      <h1>Form Builder</h1>
      <p class="desc">Create and manage forms for your website.</p>

      <div class="toolbar">
        <uui-button look="primary" color="positive" label="New Form" @click=${this._createForm}>
          New Form
        </uui-button>
        <uui-button look="default" label="Refresh" @click=${this._load}>Refresh</uui-button>
      </div>

      <uui-box headline="Forms">
        ${this._loading
          ? html`<uui-loader></uui-loader>`
          : this._forms.length === 0
          ? html`<div class="empty"><div class="empty-icon">📋</div>No forms yet. Create your first form.</div>`
          : html`
              <div class="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Category</th>
                      <th>Fields</th>
                      <th>Created</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    ${this._forms.map(
                      (f) => html`
                        <tr>
                          <td><strong>${f.name}</strong></td>
                          <td>${f.category || "\u2014"}</td>
                          <td>${f.fieldCount}</td>
                          <td>${new Date(f.createdDate).toLocaleDateString()}</td>
                          <td>
                            <div class="actions">
                              <uui-button compact look="primary" label="Edit" @click=${() => this._navigate(f.id)}>Edit</uui-button>
                              <uui-button compact look="default" label="Duplicate" @click=${() => this._duplicateForm(f.id)}>Copy</uui-button>
                              <uui-button compact look="danger" label="Delete" @click=${() => this._deleteForm(f.id, f.name)}>Delete</uui-button>
                            </div>
                          </td>
                        </tr>
                      `
                    )}
                  </tbody>
                </table>
              </div>
            `}
      </uui-box>
    `;
  }

  private _navigate(id: number): void {
    window.location.hash = `#/formbuilder/editor/${id}`;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "formbuilder-form-list": FormListDashboardElement;
  }
}
