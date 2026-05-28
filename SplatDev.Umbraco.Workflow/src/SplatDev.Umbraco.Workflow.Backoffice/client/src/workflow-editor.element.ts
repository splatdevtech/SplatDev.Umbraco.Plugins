import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit/decorators.js";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface WorkflowDefinition {
  key: string;
  label: string;
  version: number;
  isActive: boolean;
  definitionJson: string;
  createdAt: string;
}

const API = "/umbraco/backoffice/SplatDevWorkflow";

@customElement("workflow-editor-dashboard")
export class WorkflowEditorDashboard extends UmbElementMixin(LitElement) {
  @state() private _definitions: WorkflowDefinition[] = [];
  @state() private _loading = false;
  @state() private _error = "";
  @state() private _success = "";

  @state() private _view: "list" | "edit" | "create" = "list";

  @state() private _editKey = "";
  @state() private _editLabel = "";
  @state() private _editVersion = 0;
  @state() private _editJson = "";
  @state() private _editIsActive = false;
  @state() private _saving = false;
  @state() private _jsonError = "";

  connectedCallback(): void {
    super.connectedCallback();
    this._loadDefinitions();
  }

  private async _loadDefinitions(): Promise<void> {
    this._loading = true;
    this._error = "";
    try {
      const resp = await fetch(`${API}/WorkflowDefinitions/List`);
      if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
      this._definitions = await resp.json();
    } catch (e: unknown) {
      this._error = e instanceof Error ? e.message : "Failed to load definitions";
    } finally {
      this._loading = false;
    }
  }

  private async _selectDefinition(key: string): Promise<void> {
    this._error = "";
    this._success = "";
    try {
      const resp = await fetch(`${API}/WorkflowDefinitions/Get/${encodeURIComponent(key)}`);
      if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
      const def: WorkflowDefinition = await resp.json();
      this._editKey = def.key;
      this._editLabel = def.label;
      this._editVersion = def.version;
      this._editIsActive = def.isActive;
      this._editJson = this._formatJson(def.definitionJson);
      this._jsonError = "";
      this._view = "edit";
    } catch (e: unknown) {
      this._error = e instanceof Error ? e.message : "Failed to load definition";
    }
  }

  private _startCreate(): void {
    this._editKey = "";
    this._editLabel = "";
    this._editVersion = 0;
    this._editIsActive = false;
    this._editJson = this._formatJson(JSON.stringify({
      steps: [
        { key: "start", label: "Start", actions: [{ key: "submit", label: "Submit", targetStepKey: "review" }] },
        { key: "review", label: "Review", actions: [
          { key: "approve", label: "Approve", targetStepKey: "done" },
          { key: "reject", label: "Reject", targetStepKey: "start" }
        ]},
        { key: "done", label: "Done", actions: [] }
      ]
    }));
    this._jsonError = "";
    this._error = "";
    this._success = "";
    this._view = "create";
  }

  private async _save(): Promise<void> {
    this._jsonError = "";
    try { JSON.parse(this._editJson); } catch {
      this._jsonError = "Invalid JSON — please fix before saving";
      return;
    }
    if (!this._editKey.trim() || !this._editLabel.trim()) {
      this._error = "Key and label are required";
      return;
    }

    this._saving = true;
    this._error = "";
    this._success = "";
    try {
      const resp = await fetch(`${API}/WorkflowDefinitions/Save`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          key: this._editKey,
          label: this._editLabel,
          version: this._editVersion,
          isActive: this._editIsActive,
          definitionJson: this._editJson,
          createdAt: new Date().toISOString(),
        }),
      });
      if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
      const saved: WorkflowDefinition = await resp.json();
      this._editVersion = saved.version;
      this._success = `Saved as version ${saved.version}`;
      await this._loadDefinitions();
      if (this._view === "create") this._view = "edit";
    } catch (e: unknown) {
      this._error = e instanceof Error ? e.message : "Save failed";
    } finally {
      this._saving = false;
    }
  }

  private async _activate(): Promise<void> {
    this._error = "";
    this._success = "";
    try {
      const resp = await fetch(
        `${API}/WorkflowDefinitions/Activate/${encodeURIComponent(this._editKey)}?version=${this._editVersion}`,
        { method: "PUT" }
      );
      if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
      this._editIsActive = true;
      this._success = `Version ${this._editVersion} activated`;
      await this._loadDefinitions();
    } catch (e: unknown) {
      this._error = e instanceof Error ? e.message : "Activation failed";
    }
  }

  private _formatJson(raw: string): string {
    try { return JSON.stringify(JSON.parse(raw), null, 2); } catch { return raw; }
  }

  private _validateJsonLive(): void {
    try { JSON.parse(this._editJson); this._jsonError = ""; }
    catch (e: unknown) { this._jsonError = e instanceof Error ? e.message : "Invalid JSON"; }
  }

  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1); }

    .toolbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: var(--uui-size-space-5);
    }
    .def-list {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: var(--uui-size-space-4);
    }
    .def-card {
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      padding: var(--uui-size-space-4);
      cursor: pointer;
      transition: box-shadow 0.15s, border-color 0.15s;
      background: var(--uui-color-surface);
    }
    .def-card:hover {
      border-color: var(--uui-color-current);
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
    }
    .def-card h3 { margin: 0 0 var(--uui-size-space-2) 0; font-size: 16px; }
    .def-card .meta {
      font-size: var(--uui-type-small-size);
      color: var(--uui-color-text-alt);
      display: flex;
      gap: var(--uui-size-space-3);
    }
    .active-badge {
      display: inline-block;
      padding: 1px 6px;
      border-radius: 8px;
      font-size: 10px;
      font-weight: 700;
      text-transform: uppercase;
    }
    .active-badge.yes { background: var(--uui-color-positive); color: #fff; }
    .active-badge.no { background: var(--uui-color-border); color: var(--uui-color-text-alt); }

    .editor-form { display: flex; flex-direction: column; gap: var(--uui-size-space-4); }
    .form-row { display: flex; gap: var(--uui-size-space-4); }
    .form-group { display: flex; flex-direction: column; gap: var(--uui-size-space-1); flex: 1; }
    .form-group label {
      font-size: var(--uui-type-small-size);
      color: var(--uui-color-text-alt);
      font-weight: 600;
    }
    .form-group input {
      padding: 8px 12px;
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      font-size: 14px;
      background: var(--uui-color-surface);
      color: var(--uui-color-text);
    }

    .json-editor textarea {
      width: 100%;
      min-height: 400px;
      padding: var(--uui-size-space-3);
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      font-family: "Cascadia Code", "Fira Code", monospace;
      font-size: 13px;
      line-height: 1.5;
      background: var(--uui-color-surface);
      color: var(--uui-color-text);
      resize: vertical;
      tab-size: 2;
      box-sizing: border-box;
    }
    .json-editor textarea.invalid { border-color: var(--uui-color-danger); }
    .json-error { color: var(--uui-color-danger); font-size: 12px; margin-top: 4px; }

    .editor-actions {
      display: flex;
      gap: var(--uui-size-space-3);
      align-items: center;
    }
    .version-info {
      font-size: var(--uui-type-small-size);
      color: var(--uui-color-text-alt);
      margin-left: auto;
    }
    .error-bar {
      background: var(--uui-color-danger); color: #fff;
      padding: var(--uui-size-space-3) var(--uui-size-space-4);
      border-radius: var(--uui-border-radius);
      margin-bottom: var(--uui-size-space-4);
    }
    .success-bar {
      background: var(--uui-color-positive); color: #fff;
      padding: var(--uui-size-space-3) var(--uui-size-space-4);
      border-radius: var(--uui-border-radius);
      margin-bottom: var(--uui-size-space-4);
    }
    .empty-state {
      text-align: center;
      padding: var(--uui-size-layout-2);
      color: var(--uui-color-text-alt);
    }
  `;

  render() {
    return html`
      <uui-box headline="Workflow Definitions">
        ${this._error ? html`<div class="error-bar">${this._error}</div>` : nothing}
        ${this._success ? html`<div class="success-bar">${this._success}</div>` : nothing}
        ${this._view === "list" ? this._renderList() : nothing}
        ${this._view === "edit" || this._view === "create" ? this._renderEditor() : nothing}
      </uui-box>
    `;
  }

  private _renderList() {
    return html`
      <div class="toolbar">
        <span>${this._definitions.length} definition${this._definitions.length !== 1 ? "s" : ""}</span>
        <uui-button label="New Definition" look="primary" @click=${() => this._startCreate()}>
          <uui-icon name="icon-add"></uui-icon> New Definition
        </uui-button>
      </div>
      ${this._loading
        ? html`<uui-loader-bar></uui-loader-bar>`
        : this._definitions.length === 0
          ? html`<div class="empty-state">
              <uui-icon name="icon-nodes" style="font-size:48px;opacity:0.3"></uui-icon>
              <p>No workflow definitions yet</p>
              <uui-button label="Create one" look="primary" @click=${() => this._startCreate()}>Create your first workflow</uui-button>
            </div>`
          : html`<div class="def-list">
              ${this._definitions.map((def) => html`
                <div class="def-card" @click=${() => this._selectDefinition(def.key)}>
                  <h3>${def.label}</h3>
                  <div class="meta">
                    <span>Key: ${def.key}</span>
                    <span>v${def.version}</span>
                    <span class="active-badge ${def.isActive ? "yes" : "no"}">${def.isActive ? "Active" : "Draft"}</span>
                  </div>
                </div>
              `)}
            </div>`}
    `;
  }

  private _renderEditor() {
    const isNew = this._view === "create";
    return html`
      <div class="toolbar">
        <uui-button label="Back" look="secondary" @click=${() => { this._view = "list"; this._success = ""; this._error = ""; }}>
          ← Back to list
        </uui-button>
        <span>${isNew ? "New Workflow Definition" : `Editing: ${this._editLabel}`}</span>
      </div>
      <div class="editor-form">
        <div class="form-row">
          <div class="form-group">
            <label>Key</label>
            <input type="text" placeholder="e.g. content-approval" .value=${this._editKey}
              ?disabled=${!isNew} @input=${(e: Event) => { this._editKey = (e.target as HTMLInputElement).value; }} />
          </div>
          <div class="form-group">
            <label>Label</label>
            <input type="text" placeholder="e.g. Content Approval" .value=${this._editLabel}
              @input=${(e: Event) => { this._editLabel = (e.target as HTMLInputElement).value; }} />
          </div>
        </div>
        <div class="json-editor">
          <label style="font-size:var(--uui-type-small-size);color:var(--uui-color-text-alt);font-weight:600;display:block;margin-bottom:4px">
            Definition JSON (steps, actions, transitions)
          </label>
          <textarea class=${this._jsonError ? "invalid" : ""} .value=${this._editJson}
            @input=${(e: Event) => { this._editJson = (e.target as HTMLTextAreaElement).value; this._validateJsonLive(); }}
            spellcheck="false"></textarea>
          ${this._jsonError ? html`<div class="json-error">${this._jsonError}</div>` : nothing}
        </div>
        <div class="editor-actions">
          <uui-button label="Save" look="primary" ?disabled=${this._saving || !!this._jsonError} @click=${() => this._save()}>
            ${this._saving ? "Saving…" : isNew ? "Create" : "Save New Version"}
          </uui-button>
          ${!isNew && !this._editIsActive
            ? html`<uui-button label="Activate" look="secondary" color="positive" @click=${() => this._activate()}>
                Activate v${this._editVersion}
              </uui-button>`
            : nothing}
          ${!isNew
            ? html`<span class="version-info">
                Version ${this._editVersion} · ${this._editIsActive ? "Active" : "Draft"} · Created ${new Date(this._definitions.find((d) => d.key === this._editKey)?.createdAt ?? "").toLocaleDateString()}
              </span>`
            : nothing}
        </div>
      </div>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "workflow-editor-dashboard": WorkflowEditorDashboard;
  }
}
