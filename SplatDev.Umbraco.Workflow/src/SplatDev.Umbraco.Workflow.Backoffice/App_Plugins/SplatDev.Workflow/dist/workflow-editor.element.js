import { t as e } from "./decorate-BapTsssJ.js";
import { LitElement as t, css as n, html as r, nothing as i } from "@umbraco-cms/backoffice/external/lit";
import { customElement as a, state as o } from "@umbraco-cms/backoffice/external/lit/decorators.js";
import { UmbElementMixin as s } from "@umbraco-cms/backoffice/element-api";
//#region src/workflow-editor.element.ts
var c = "/umbraco/backoffice/SplatDevWorkflow", l = class extends s(t) {
	constructor(...e) {
		super(...e), this._definitions = [], this._loading = !1, this._error = "", this._success = "", this._view = "list", this._editKey = "", this._editLabel = "", this._editVersion = 0, this._editJson = "", this._editIsActive = !1, this._saving = !1, this._jsonError = "";
	}
	connectedCallback() {
		super.connectedCallback(), this._loadDefinitions();
	}
	async _loadDefinitions() {
		this._loading = !0, this._error = "";
		try {
			let e = await fetch(`${c}/WorkflowDefinitions/List`);
			if (!e.ok) throw Error(`HTTP ${e.status}`);
			this._definitions = await e.json();
		} catch (e) {
			this._error = e instanceof Error ? e.message : "Failed to load definitions";
		} finally {
			this._loading = !1;
		}
	}
	async _selectDefinition(e) {
		this._error = "", this._success = "";
		try {
			let t = await fetch(`${c}/WorkflowDefinitions/Get/${encodeURIComponent(e)}`);
			if (!t.ok) throw Error(`HTTP ${t.status}`);
			let n = await t.json();
			this._editKey = n.key, this._editLabel = n.label, this._editVersion = n.version, this._editIsActive = n.isActive, this._editJson = this._formatJson(n.definitionJson), this._jsonError = "", this._view = "edit";
		} catch (e) {
			this._error = e instanceof Error ? e.message : "Failed to load definition";
		}
	}
	_startCreate() {
		this._editKey = "", this._editLabel = "", this._editVersion = 0, this._editIsActive = !1, this._editJson = this._formatJson(JSON.stringify({ steps: [
			{
				key: "start",
				label: "Start",
				actions: [{
					key: "submit",
					label: "Submit",
					targetStepKey: "review"
				}]
			},
			{
				key: "review",
				label: "Review",
				actions: [{
					key: "approve",
					label: "Approve",
					targetStepKey: "done"
				}, {
					key: "reject",
					label: "Reject",
					targetStepKey: "start"
				}]
			},
			{
				key: "done",
				label: "Done",
				actions: []
			}
		] })), this._jsonError = "", this._error = "", this._success = "", this._view = "create";
	}
	async _save() {
		this._jsonError = "";
		try {
			JSON.parse(this._editJson);
		} catch {
			this._jsonError = "Invalid JSON — please fix before saving";
			return;
		}
		if (!this._editKey.trim() || !this._editLabel.trim()) {
			this._error = "Key and label are required";
			return;
		}
		this._saving = !0, this._error = "", this._success = "";
		try {
			let e = await fetch(`${c}/WorkflowDefinitions/Save`, {
				method: "POST",
				headers: { "Content-Type": "application/json" },
				body: JSON.stringify({
					key: this._editKey,
					label: this._editLabel,
					version: this._editVersion,
					isActive: this._editIsActive,
					definitionJson: this._editJson,
					createdAt: (/* @__PURE__ */ new Date()).toISOString()
				})
			});
			if (!e.ok) throw Error(`HTTP ${e.status}`);
			let t = await e.json();
			this._editVersion = t.version, this._success = `Saved as version ${t.version}`, await this._loadDefinitions(), this._view === "create" && (this._view = "edit");
		} catch (e) {
			this._error = e instanceof Error ? e.message : "Save failed";
		} finally {
			this._saving = !1;
		}
	}
	async _activate() {
		this._error = "", this._success = "";
		try {
			let e = await fetch(`${c}/WorkflowDefinitions/Activate/${encodeURIComponent(this._editKey)}?version=${this._editVersion}`, { method: "PUT" });
			if (!e.ok) throw Error(`HTTP ${e.status}`);
			this._editIsActive = !0, this._success = `Version ${this._editVersion} activated`, await this._loadDefinitions();
		} catch (e) {
			this._error = e instanceof Error ? e.message : "Activation failed";
		}
	}
	_formatJson(e) {
		try {
			return JSON.stringify(JSON.parse(e), null, 2);
		} catch {
			return e;
		}
	}
	_validateJsonLive() {
		try {
			JSON.parse(this._editJson), this._jsonError = "";
		} catch (e) {
			this._jsonError = e instanceof Error ? e.message : "Invalid JSON";
		}
	}
	static {
		this.styles = n`
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
	}
	render() {
		return r`
      <uui-box headline="Workflow Definitions">
        ${this._error ? r`<div class="error-bar">${this._error}</div>` : i}
        ${this._success ? r`<div class="success-bar">${this._success}</div>` : i}
        ${this._view === "list" ? this._renderList() : i}
        ${this._view === "edit" || this._view === "create" ? this._renderEditor() : i}
      </uui-box>
    `;
	}
	_renderList() {
		return r`
      <div class="toolbar">
        <span>${this._definitions.length} definition${this._definitions.length === 1 ? "" : "s"}</span>
        <uui-button label="New Definition" look="primary" @click=${() => this._startCreate()}>
          <uui-icon name="icon-add"></uui-icon> New Definition
        </uui-button>
      </div>
      ${this._loading ? r`<uui-loader-bar></uui-loader-bar>` : this._definitions.length === 0 ? r`<div class="empty-state">
              <uui-icon name="icon-nodes" style="font-size:48px;opacity:0.3"></uui-icon>
              <p>No workflow definitions yet</p>
              <uui-button label="Create one" look="primary" @click=${() => this._startCreate()}>Create your first workflow</uui-button>
            </div>` : r`<div class="def-list">
              ${this._definitions.map((e) => r`
                <div class="def-card" @click=${() => this._selectDefinition(e.key)}>
                  <h3>${e.label}</h3>
                  <div class="meta">
                    <span>Key: ${e.key}</span>
                    <span>v${e.version}</span>
                    <span class="active-badge ${e.isActive ? "yes" : "no"}">${e.isActive ? "Active" : "Draft"}</span>
                  </div>
                </div>
              `)}
            </div>`}
    `;
	}
	_renderEditor() {
		let e = this._view === "create";
		return r`
      <div class="toolbar">
        <uui-button label="Back" look="secondary" @click=${() => {
			this._view = "list", this._success = "", this._error = "";
		}}>
          ← Back to list
        </uui-button>
        <span>${e ? "New Workflow Definition" : `Editing: ${this._editLabel}`}</span>
      </div>
      <div class="editor-form">
        <div class="form-row">
          <div class="form-group">
            <label>Key</label>
            <input type="text" placeholder="e.g. content-approval" .value=${this._editKey}
              ?disabled=${!e} @input=${(e) => {
			this._editKey = e.target.value;
		}} />
          </div>
          <div class="form-group">
            <label>Label</label>
            <input type="text" placeholder="e.g. Content Approval" .value=${this._editLabel}
              @input=${(e) => {
			this._editLabel = e.target.value;
		}} />
          </div>
        </div>
        <div class="json-editor">
          <label style="font-size:var(--uui-type-small-size);color:var(--uui-color-text-alt);font-weight:600;display:block;margin-bottom:4px">
            Definition JSON (steps, actions, transitions)
          </label>
          <textarea class=${this._jsonError ? "invalid" : ""} .value=${this._editJson}
            @input=${(e) => {
			this._editJson = e.target.value, this._validateJsonLive();
		}}
            spellcheck="false"></textarea>
          ${this._jsonError ? r`<div class="json-error">${this._jsonError}</div>` : i}
        </div>
        <div class="editor-actions">
          <uui-button label="Save" look="primary" ?disabled=${this._saving || !!this._jsonError} @click=${() => this._save()}>
            ${this._saving ? "Saving…" : e ? "Create" : "Save New Version"}
          </uui-button>
          ${!e && !this._editIsActive ? r`<uui-button label="Activate" look="secondary" color="positive" @click=${() => this._activate()}>
                Activate v${this._editVersion}
              </uui-button>` : i}
          ${e ? i : r`<span class="version-info">
                Version ${this._editVersion} · ${this._editIsActive ? "Active" : "Draft"} · Created ${new Date(this._definitions.find((e) => e.key === this._editKey)?.createdAt ?? "").toLocaleDateString()}
              </span>`}
        </div>
      </div>
    `;
	}
};
e([o()], l.prototype, "_definitions", void 0), e([o()], l.prototype, "_loading", void 0), e([o()], l.prototype, "_error", void 0), e([o()], l.prototype, "_success", void 0), e([o()], l.prototype, "_view", void 0), e([o()], l.prototype, "_editKey", void 0), e([o()], l.prototype, "_editLabel", void 0), e([o()], l.prototype, "_editVersion", void 0), e([o()], l.prototype, "_editJson", void 0), e([o()], l.prototype, "_editIsActive", void 0), e([o()], l.prototype, "_saving", void 0), e([o()], l.prototype, "_jsonError", void 0), l = e([a("workflow-editor-dashboard")], l);
//#endregion
export { l as WorkflowEditorDashboard };
