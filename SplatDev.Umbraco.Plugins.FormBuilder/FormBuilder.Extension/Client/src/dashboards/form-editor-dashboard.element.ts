import { LitElement, html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface FormDetail {
  id: number;
  name: string;
  category: string;
  fields: FormFieldDto[];
  workflowCount: number;
}

interface FormFieldDto {
  id: number;
  alias: string;
  label: string;
  type: string;
  isRequired: boolean;
  placeholder: string;
  regex: string;
  minLength: number;
  sortOrder: number;
  dropdownValues: { value: string }[];
}

interface FieldTypeOption {
  type: string;
  label: string;
  icon: string;
}

const API = "/umbraco/api/formbuilder";

@customElement("formbuilder-form-editor")
export class FormEditorDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    .desc { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 12px; margin-bottom: 16px; }
    .form-row { display: flex; gap: 12px; margin-bottom: 12px; align-items: flex-start; flex-wrap: wrap; }
    .form-row label { display: block; font-size: 0.8rem; font-weight: 600; margin-bottom: 4px; }
    .form-row input, .form-row select { padding: 6px 12px; border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 6px; font-size: 0.875rem; min-width: 180px; }
    .field-card { background: var(--uui-color-surface-alt, #f9fafb); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 16px; margin-bottom: 12px; }
    .field-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px; }
    .field-header h3 { margin: 0; font-size: 1rem; }
    .field-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 10px; }
    .required-toggle { display: flex; align-items: center; gap: 8px; margin-top: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _form: FormDetail | null = null;
  @state() private _fieldTypes: FieldTypeOption[] = [];
  @state() private _loading = false;
  @state() private _formId: number | null = null;

  override connectedCallback(): void {
    super.connectedCallback();
    const match = window.location.hash.match(/\/editor\/(\d+)/);
    this._formId = match ? Number(match[1]) : null;
    if (this._formId) this._load();
    this._loadFieldTypes();
  }

  private async _load(): Promise<void> {
    if (!this._formId) return;
    this._loading = true;
    try {
      const res = await fetch(`${API}/GetForm?id=${this._formId}`);
      if (res.ok) this._form = (await res.json()) as FormDetail;
    } finally {
      this._loading = false;
    }
  }

  private async _loadFieldTypes(): Promise<void> {
    const res = await fetch(`${API}/GetFieldTypes`);
    if (res.ok) this._fieldTypes = (await res.json()) as FieldTypeOption[];
  }

  private async _save(): Promise<void> {
    if (!this._form || !this._formId) return;
    const body = {
      name: this._form.name,
      category: this._form.category,
      fields: this._form.fields.map(f => ({
        alias: f.alias,
        label: f.label,
        type: f.type,
        required: f.isRequired,
        placeholder: f.placeholder || null,
        regex: f.regex || null,
        minLength: f.minLength || null,
        sortOrder: f.sortOrder,
      })),
    };
    const res = await fetch(`${API}/UpdateForm?id=${this._formId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });
    if (res.ok) {
      alert("Saved.");
      await this._load();
    } else {
      alert("Save failed: " + (await res.text()));
    }
  }

  private _addField(): void {
    if (!this._form) return;
    const alias = prompt("Field alias (camelCase):");
    if (!alias?.trim()) return;
    const newField: FormFieldDto = {
      id: 0,
      alias: alias.trim(),
      label: alias.trim(),
      type: "text",
      isRequired: false,
      placeholder: "",
      regex: "",
      minLength: 0,
      sortOrder: this._form.fields.length,
      dropdownValues: [],
    };
    this._form = { ...this._form, fields: [...this._form.fields, newField] };
  }

  private _removeField(index: number): void {
    if (!this._form) return;
    this._form = { ...this._form, fields: this._form.fields.filter((_, i) => i !== index) };
  }

  private _updateField(index: number, key: string, value: unknown): void {
    if (!this._form) return;
    const fields = [...this._form.fields];
    fields[index] = { ...fields[index], [key]: value };
    this._form = { ...this._form, fields };
  }

  override render() {
    if (this._loading && !this._form) return html`<uui-loader></uui-loader>`;
    if (!this._formId) return html`<div class="empty">No form selected.</div>`;
    if (!this._form) return html`<div class="empty">Loading form...</div>`;

    return html`
      <h1>${this._form.name}</h1>
      <p class="desc">Edit form fields and settings.</p>

      <div class="toolbar">
        <uui-button look="primary" color="positive" label="Save" @click=${this._save}>Save</uui-button>
        <a href="#/formbuilder/formbuilder-dashboard"><uui-button look="default" label="Back">Back</uui-button></a>
      </div>

      <uui-box headline="Form Settings">
        <div class="form-row">
          <div>
            <label>Form Name</label>
            <input type="text" .value=${this._form.name}
              @input=${(e: Event) => this._updateField(-1, "name", (e.target as HTMLInputElement).value)} />
          </div>
          <div>
            <label>Category</label>
            <input type="text" .value=${this._form.category} placeholder="Optional"
              @input=${(e: Event) => this._updateField(-1, "category", (e.target as HTMLInputElement).value)} />
          </div>
        </div>
      </uui-box>

      <uui-box headline="Fields (${this._form.fields.length})">
        <div class="toolbar">
          <uui-button look="primary" compact label="Add Field" @click=${this._addField}>Add Field</uui-button>
        </div>

        ${this._form.fields.length === 0
          ? html`<div class="empty">No fields yet. Add your first field.</div>`
          : this._form.fields.map((field, i) => html`
              <div class="field-card">
                <div class="field-header">
                  <h3>${field.label || field.alias}</h3>
                  <uui-button compact look="danger" label="Remove" @click=${() => this._removeField(i)}>Remove</uui-button>
                </div>
                <div class="field-grid">
                  <div>
                    <label>Alias</label>
                    <input type="text" .value=${field.alias}
                      @input=${(e: Event) => this._updateField(i, "alias", (e.target as HTMLInputElement).value)} />
                  </div>
                  <div>
                    <label>Label</label>
                    <input type="text" .value=${field.label}
                      @input=${(e: Event) => this._updateField(i, "label", (e.target as HTMLInputElement).value)} />
                  </div>
                  <div>
                    <label>Type</label>
                    <select .value=${field.type}
                      @change=${(e: Event) => this._updateField(i, "type", (e.target as HTMLSelectElement).value)}>
                      ${this._fieldTypes.map(ft => html`<option value=${ft.type} ?selected=${field.type === ft.type}>${ft.label}</option>`)}
                    </select>
                  </div>
                  <div>
                    <label>Placeholder</label>
                    <input type="text" .value=${field.placeholder}
                      @input=${(e: Event) => this._updateField(i, "placeholder", (e.target as HTMLInputElement).value)} />
                  </div>
                  <div>
                    <label>Regex</label>
                    <input type="text" .value=${field.regex}
                      @input=${(e: Event) => this._updateField(i, "regex", (e.target as HTMLInputElement).value)} />
                  </div>
                  <div>
                    <label>Min Length</label>
                    <input type="number" .value=${field.minLength.toString()}
                      @input=${(e: Event) => this._updateField(i, "minLength", Number((e.target as HTMLInputElement).value))} />
                  </div>
                </div>
                <div class="required-toggle">
                  <label><input type="checkbox" .checked=${field.isRequired}
                    @change=${(e: Event) => this._updateField(i, "isRequired", (e.target as HTMLInputElement).checked)} /> Required</label>
                </div>
              </div>
            `)}
      </uui-box>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "formbuilder-form-editor": FormEditorDashboardElement;
  }
}
