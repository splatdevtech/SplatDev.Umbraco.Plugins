import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface DictionaryItem {
  key: string;
  parentKey: string | null;
  value: string;
  languageCode: string;
  translations: Record<string, string>;
}

@customElement("dictionarymanager-dashboard")
export class DictionaryManagerDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-bottom: 16px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
    th { text-align: left; padding: 8px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
    tr:hover td { background: var(--uui-color-surface-alt, #f9fafb); }
    .translations { font-size: 0.75rem; color: var(--uui-color-text-alt, #6b7280); }
    .edit-form { padding: 16px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; margin-bottom: 16px; background: var(--uui-color-surface-alt, #f9fafb); }
    .edit-form .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 12px; }
    .edit-form .field { display: flex; flex-direction: column; gap: 4px; }
    .edit-form .field label { font-size: 0.8rem; font-weight: 500; }
    .edit-form .buttons { display: flex; gap: 8px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _items: DictionaryItem[] = [];
  @state() private _filter = "";
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;
  @state() private _editing: DictionaryItem | null = null;
  @state() private _creating = false;

  private readonly _api = "/umbraco/api/dictionarymanager";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadAll();
  }

  private async _loadAll(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetAll`);
      this._items = await r.json();
    } catch {
      this._items = [];
      this._message = { type: "error", text: "Failed to load dictionary items." };
    }
    this._loading = false;
  }

  private _startCreate(): void {
    this._editing = { key: "", parentKey: null, value: "", languageCode: "", translations: {} };
    this._creating = true;
  }

  private _startEdit(item: DictionaryItem): void {
    this._editing = { ...item, translations: { ...item.translations } };
    this._creating = false;
  }

  private _cancelEdit(): void {
    this._editing = null;
    this._creating = false;
  }

  private async _save(): Promise<void> {
    if (!this._editing) return;
    this._loading = true;
    this._message = null;
    try {
      const method = this._creating ? "POST" : "PUT";
      const action = this._creating ? "Create" : "Update";
      const r = await fetch(`${this._api}/${action}`, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._editing),
      });
      if (!r.ok) {
        const err = await r.text();
        throw new Error(err);
      }
      this._message = { type: "success", text: `Item ${this._creating ? "created" : "updated"} successfully.` };
      this._editing = null;
      this._creating = false;
      await this._loadAll();
    } catch (e) {
      this._message = { type: "error", text: `Save failed: ${(e as Error).message}` };
    }
    this._loading = false;
  }

  private async _delete(key: string): Promise<void> {
    if (!confirm(`Delete dictionary item '${key}'?`)) return;
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/Delete?key=${encodeURIComponent(key)}`, { method: "DELETE" });
      if (!r.ok) throw new Error();
      this._message = { type: "success", text: `'${key}' deleted.` };
      await this._loadAll();
    } catch {
      this._message = { type: "error", text: "Delete failed." };
    }
    this._loading = false;
  }

  private async _export(): Promise<void> {
    window.open(`${this._api}/Export`, "_blank");
  }

  private async _import(): Promise<void> {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = ".json";
    input.onchange = async () => {
      const file = input.files?.[0];
      if (!file) return;
      this._loading = true;
      try {
        const text = await file.text();
        const items = JSON.parse(text);
        const r = await fetch(`${this._api}/Import?overrideExisting=true`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(items),
        });
        if (!r.ok) throw new Error();
        const results = await r.json();
        const count = Array.isArray(results) ? results.filter((x: { success: boolean }) => x.success).length : 0;
        this._message = { type: "success", text: `Imported ${count} item(s).` };
        await this._loadAll();
      } catch {
        this._message = { type: "error", text: "Import failed." };
      }
      this._loading = false;
    };
    input.click();
  }

  private get _filtered(): DictionaryItem[] {
    if (!this._filter) return this._items;
    const q = this._filter.toLowerCase();
    return this._items.filter(
      (i) => i.key.toLowerCase().includes(q) || i.value.toLowerCase().includes(q),
    );
  }

  override render() {
    return html`
      <h1>Dictionary Manager</h1>
      <p class="description">Import, export and manage Umbraco dictionary items.</p>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}

      ${this._editing ? this._renderEditor() : nothing}

      <div class="toolbar">
        <uui-input placeholder="Filter by key or value…" style="flex:1;min-width:200px;"
          @input=${(e: InputEvent) => (this._filter = (e.target as HTMLInputElement).value)}></uui-input>
        <uui-button look="primary" @click=${this._startCreate} ?disabled=${this._loading}>+ Create</uui-button>
        <uui-button look="secondary" @click=${this._export} ?disabled=${this._loading}>Export JSON</uui-button>
        <uui-button look="secondary" @click=${this._import} ?disabled=${this._loading}>Import JSON</uui-button>
      </div>

      ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}

      ${this._filtered.length === 0 && !this._loading
        ? html`<div class="empty">No dictionary items found.</div>`
        : html`
            <table>
              <thead>
                <tr><th>Key</th><th>Value</th><th>Language</th><th>Translations</th><th></th></tr>
              </thead>
              <tbody>
                ${this._filtered.map(
                  (item) => html`
                    <tr>
                      <td>${item.key}</td>
                      <td>${item.value}</td>
                      <td>${item.languageCode}</td>
                      <td class="translations">${Object.keys(item.translations).join(", ") || "—"}</td>
                      <td style="white-space:nowrap;">
                        <uui-button look="secondary" compact @click=${() => this._startEdit(item)}>Edit</uui-button>
                        <uui-button look="danger" compact @click=${() => this._delete(item.key)}>Delete</uui-button>
                      </td>
                    </tr>
                  `,
                )}
              </tbody>
            </table>
          `}
    `;
  }

  private _renderEditor() {
    const item = this._editing!;
    return html`
      <div class="edit-form">
        <h3 style="margin:0 0 12px;">${this._creating ? "Create Item" : `Edit: ${item.key}`}</h3>
        <div class="fields">
          <div class="field">
            <label>Key</label>
            <uui-input .value=${item.key} ?disabled=${!this._creating}
              @input=${(e: InputEvent) => { this._editing = { ...item, key: (e.target as HTMLInputElement).value }; }}></uui-input>
          </div>
          <div class="field">
            <label>Value</label>
            <uui-input .value=${item.value}
              @input=${(e: InputEvent) => { this._editing = { ...item, value: (e.target as HTMLInputElement).value }; }}></uui-input>
          </div>
          <div class="field">
            <label>Language Code</label>
            <uui-input .value=${item.languageCode}
              @input=${(e: InputEvent) => { this._editing = { ...item, languageCode: (e.target as HTMLInputElement).value }; }}></uui-input>
          </div>
          <div class="field">
            <label>Parent Key</label>
            <uui-input .value=${item.parentKey ?? ""}
              @input=${(e: InputEvent) => { this._editing = { ...item, parentKey: (e.target as HTMLInputElement).value || null }; }}></uui-input>
          </div>
        </div>
        <div class="buttons">
          <uui-button look="primary" @click=${this._save} ?disabled=${this._loading}>
            ${this._creating ? "Create" : "Save"}
          </uui-button>
          <uui-button look="secondary" @click=${this._cancelEdit}>Cancel</uui-button>
        </div>
      </div>
    `;
  }
}

export default DictionaryManagerDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "dictionarymanager-dashboard": DictionaryManagerDashboardElement;
  }
}
