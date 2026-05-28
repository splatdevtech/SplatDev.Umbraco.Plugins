import { t as e } from "./decorate-BapTsssJ.js";
import { LitElement as t, css as n, html as r, nothing as i } from "@umbraco-cms/backoffice/external/lit";
import { customElement as a, state as o } from "@umbraco-cms/backoffice/external/lit/decorators.js";
import { UmbElementMixin as s } from "@umbraco-cms/backoffice/element-api";
//#region src/workflow-queue.element.ts
var c = "/umbraco/backoffice/SplatDevWorkflow", l = {
	0: "Open",
	1: "Completed",
	2: "Cancelled"
}, u = class extends s(t) {
	constructor(...e) {
		super(...e), this._rows = [], this._total = 0, this._page = 1, this._pageSize = 50, this._loading = !1, this._error = "", this._filterWorkflowKey = "", this._filterStatus = "", this._filterAssignedToMe = !1, this._filterFreeText = "", this._definitions = [], this._selectedInstanceId = null, this._instanceDetail = null, this._instanceTasks = [], this._detailLoading = !1, this._transitionAction = "", this._transitioning = !1;
	}
	connectedCallback() {
		super.connectedCallback(), this._loadDefinitions(), this._loadQueue();
	}
	async _loadDefinitions() {
		try {
			let e = await fetch(`${c}/WorkflowDefinitions/List`);
			e.ok && (this._definitions = await e.json());
		} catch {}
	}
	async _loadQueue() {
		this._loading = !0, this._error = "";
		try {
			let e = new URLSearchParams();
			e.set("page", String(this._page)), e.set("pageSize", String(this._pageSize)), this._filterWorkflowKey && e.set("workflowKey", this._filterWorkflowKey), this._filterStatus !== "" && e.set("status", this._filterStatus), this._filterAssignedToMe && e.set("assignedToMe", "true"), this._filterFreeText && e.set("freeText", this._filterFreeText);
			let t = await fetch(`${c}/WorkflowInstances/List?${e}`);
			if (!t.ok) throw Error(`HTTP ${t.status}`);
			let n = await t.json();
			this._rows = n.items, this._total = n.totalCount;
		} catch (e) {
			this._error = e instanceof Error ? e.message : "Failed to load queue";
		} finally {
			this._loading = !1;
		}
	}
	async _selectInstance(e) {
		if (this._selectedInstanceId === e) {
			this._selectedInstanceId = null, this._instanceDetail = null, this._instanceTasks = [];
			return;
		}
		this._selectedInstanceId = e, this._detailLoading = !0, this._transitionAction = "";
		try {
			let [t, n] = await Promise.all([fetch(`${c}/WorkflowInstances/Get/${e}`), fetch(`${c}/WorkflowTasks/List/${e}`)]);
			this._instanceDetail = t.ok ? await t.json() : null, this._instanceTasks = n.ok ? await n.json() : [];
		} catch {
			this._instanceDetail = null, this._instanceTasks = [];
		} finally {
			this._detailLoading = !1;
		}
	}
	async _doTransition() {
		if (!(!this._selectedInstanceId || !this._transitionAction)) {
			this._transitioning = !0;
			try {
				let e = await fetch(`${c}/WorkflowInstances/Transition/${this._selectedInstanceId}`, {
					method: "POST",
					headers: { "Content-Type": "application/json" },
					body: JSON.stringify({ actionKey: this._transitionAction })
				});
				if (!e.ok) {
					let t = await e.json().catch(() => null);
					throw Error(t?.detail ?? `HTTP ${e.status}`);
				}
				this._selectedInstanceId = null, this._instanceDetail = null, this._instanceTasks = [], await this._loadQueue();
			} catch (e) {
				this._error = e instanceof Error ? e.message : "Transition failed";
			} finally {
				this._transitioning = !1;
			}
		}
	}
	async _toggleTask(e) {
		if (this._selectedInstanceId) try {
			await fetch(`${c}/WorkflowTasks/SetCompletion/${this._selectedInstanceId}`, {
				method: "POST",
				headers: { "Content-Type": "application/json" },
				body: JSON.stringify({ entries: [{
					taskId: e.id,
					isCompleted: !e.isCompleted
				}] })
			}), e.isCompleted = !e.isCompleted, this._instanceTasks = [...this._instanceTasks];
		} catch {}
	}
	_timeAgo(e) {
		let t = Date.now() - new Date(e).getTime(), n = Math.floor(t / 6e4);
		if (n < 1) return "just now";
		if (n < 60) return `${n}m ago`;
		let r = Math.floor(n / 60);
		return r < 24 ? `${r}h ago` : `${Math.floor(r / 24)}d ago`;
	}
	get _totalPages() {
		return Math.max(1, Math.ceil(this._total / this._pageSize));
	}
	static {
		this.styles = n`
    :host {
      display: block;
      padding: var(--uui-size-layout-1);
    }
    .filters {
      display: flex;
      gap: var(--uui-size-space-3);
      flex-wrap: wrap;
      align-items: end;
      margin-bottom: var(--uui-size-space-5);
    }
    .filter-group {
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-space-1);
    }
    .filter-group label {
      font-size: var(--uui-type-small-size);
      color: var(--uui-color-text-alt);
      font-weight: 600;
    }
    .filter-group select,
    .filter-group input[type="text"] {
      padding: 6px 10px;
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      font-size: var(--uui-type-small-size);
      background: var(--uui-color-surface);
      color: var(--uui-color-text);
      min-width: 160px;
    }
    .assigned-toggle {
      display: flex;
      align-items: center;
      gap: var(--uui-size-space-2);
      padding-bottom: 4px;
    }
    .assigned-toggle input[type="checkbox"] { margin: 0; }

    .queue-table {
      width: 100%;
      border-collapse: collapse;
    }
    .queue-table th,
    .queue-table td {
      text-align: left;
      padding: var(--uui-size-space-3) var(--uui-size-space-4);
      border-bottom: 1px solid var(--uui-color-border);
    }
    .queue-table th {
      font-size: var(--uui-type-small-size);
      color: var(--uui-color-text-alt);
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .queue-table tr.clickable { cursor: pointer; }
    .queue-table tr.clickable:hover { background: var(--uui-color-surface-emphasis); }
    .queue-table tr.selected { background: var(--uui-color-surface-emphasis); }

    .status-badge {
      display: inline-block;
      padding: 2px 8px;
      border-radius: 10px;
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
    }
    .status-badge.open { background: var(--uui-color-positive); color: #fff; }
    .status-badge.completed { background: var(--uui-color-default); color: var(--uui-color-text); }
    .status-badge.cancelled { background: var(--uui-color-danger); color: #fff; }

    .detail-panel {
      background: var(--uui-color-surface-alt);
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      padding: var(--uui-size-space-5);
      margin-bottom: var(--uui-size-space-3);
    }
    .detail-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: var(--uui-size-space-3);
      margin-bottom: var(--uui-size-space-5);
    }
    .detail-item label {
      font-size: 11px;
      color: var(--uui-color-text-alt);
      text-transform: uppercase;
      font-weight: 600;
    }
    .detail-item p { margin: 4px 0 0 0; font-size: 14px; }

    .tasks-section { margin-top: var(--uui-size-space-5); }
    .tasks-section h4 { margin: 0 0 var(--uui-size-space-3) 0; font-size: 14px; }
    .task-row {
      display: flex;
      align-items: center;
      gap: var(--uui-size-space-3);
      padding: var(--uui-size-space-2) 0;
      border-bottom: 1px solid var(--uui-color-border);
    }
    .task-row input[type="checkbox"] { margin: 0; }
    .task-row .task-name { font-weight: 500; }
    .task-row .task-desc { font-size: var(--uui-type-small-size); color: var(--uui-color-text-alt); }
    .task-row.completed .task-name { text-decoration: line-through; opacity: 0.6; }

    .transition-bar {
      display: flex;
      gap: var(--uui-size-space-3);
      align-items: center;
      margin-top: var(--uui-size-space-5);
      padding-top: var(--uui-size-space-4);
      border-top: 1px solid var(--uui-color-border);
    }
    .transition-bar input {
      flex: 1;
      padding: 6px 10px;
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      font-size: 14px;
      background: var(--uui-color-surface);
      color: var(--uui-color-text);
    }

    .pagination {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-top: var(--uui-size-space-4);
      font-size: var(--uui-type-small-size);
    }
    .pagination-buttons { display: flex; gap: var(--uui-size-space-2); }
    .empty-state {
      text-align: center;
      padding: var(--uui-size-layout-2);
      color: var(--uui-color-text-alt);
    }
    .error-bar {
      background: var(--uui-color-danger);
      color: #fff;
      padding: var(--uui-size-space-3) var(--uui-size-space-4);
      border-radius: var(--uui-border-radius);
      margin-bottom: var(--uui-size-space-4);
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .metadata-block {
      margin-top: var(--uui-size-space-4);
      background: var(--uui-color-surface);
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      padding: var(--uui-size-space-3);
      font-family: monospace;
      font-size: 12px;
      white-space: pre-wrap;
      word-break: break-all;
      max-height: 200px;
      overflow-y: auto;
    }
  `;
	}
	render() {
		return r`
      <uui-box headline="Workflow Queue">
        ${this._error ? r`<div class="error-bar">
              <span>${this._error}</span>
              <uui-button label="Dismiss" look="secondary" @click=${() => this._error = ""}>&#x2715;</uui-button>
            </div>` : i}

        <div class="filters">
          <div class="filter-group">
            <label>Workflow</label>
            <select @change=${(e) => {
			this._filterWorkflowKey = e.target.value;
		}}>
              <option value="">All Workflows</option>
              ${this._definitions.map((e) => r`<option value=${e.key} ?selected=${this._filterWorkflowKey === e.key}>${e.label}</option>`)}
            </select>
          </div>

          <div class="filter-group">
            <label>Status</label>
            <select @change=${(e) => {
			this._filterStatus = e.target.value;
		}}>
              <option value="">All</option>
              <option value="0" ?selected=${this._filterStatus === "0"}>Open</option>
              <option value="1" ?selected=${this._filterStatus === "1"}>Completed</option>
              <option value="2" ?selected=${this._filterStatus === "2"}>Cancelled</option>
            </select>
          </div>

          <div class="filter-group">
            <label>Search</label>
            <input type="text" placeholder="Free text..." .value=${this._filterFreeText}
              @input=${(e) => {
			this._filterFreeText = e.target.value;
		}} />
          </div>

          <div class="assigned-toggle">
            <input type="checkbox" id="assignedToMe" .checked=${this._filterAssignedToMe}
              @change=${(e) => {
			this._filterAssignedToMe = e.target.checked;
		}} />
            <label for="assignedToMe">Assigned to me</label>
          </div>

          <uui-button label="Search" look="primary" @click=${() => {
			this._page = 1, this._loadQueue();
		}}>
            <uui-icon name="icon-search"></uui-icon> Search
          </uui-button>
          <uui-button label="Refresh" look="secondary" @click=${() => this._loadQueue()}>
            <uui-icon name="icon-refresh"></uui-icon>
          </uui-button>
        </div>

        ${this._loading ? r`<uui-loader-bar></uui-loader-bar>` : this._rows.length === 0 ? r`<div class="empty-state">
                <uui-icon name="icon-inbox" style="font-size:48px;opacity:0.3"></uui-icon>
                <p>No workflow instances found</p>
              </div>` : r`
                <table class="queue-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Workflow</th>
                      <th>Step</th>
                      <th>Status</th>
                      <th>Created</th>
                      <th>Created By</th>
                    </tr>
                  </thead>
                  <tbody>
                    ${this._rows.map((e) => r`
                      <tr class="clickable ${this._selectedInstanceId === e.instanceId ? "selected" : ""}"
                        @click=${() => this._selectInstance(e.instanceId)}>
                        <td>${e.instanceId}</td>
                        <td>${e.values?.workflowKey ?? "—"}</td>
                        <td>${e.values?.currentStepKey ?? "—"}</td>
                        <td>
                          <span class="status-badge ${(l[Number(e.values?.status)] ?? "open").toLowerCase()}">
                            ${l[Number(e.values?.status)] ?? "Unknown"}
                          </span>
                        </td>
                        <td>${e.values?.createdAt ? this._timeAgo(String(e.values.createdAt)) : "—"}</td>
                        <td>${e.values?.createdBy ?? "—"}</td>
                      </tr>
                      ${this._selectedInstanceId === e.instanceId ? this._renderDetailPanel() : i}
                    `)}
                  </tbody>
                </table>

                <div class="pagination">
                  <span>Showing ${(this._page - 1) * this._pageSize + 1}–${Math.min(this._page * this._pageSize, this._total)} of ${this._total}</span>
                  <div class="pagination-buttons">
                    <uui-button label="Previous" look="secondary" ?disabled=${this._page <= 1}
                      @click=${() => {
			this._page--, this._loadQueue();
		}}>← Prev</uui-button>
                    <uui-button label="Next" look="secondary" ?disabled=${this._page >= this._totalPages}
                      @click=${() => {
			this._page++, this._loadQueue();
		}}>Next →</uui-button>
                  </div>
                </div>
              `}
      </uui-box>
    `;
	}
	_renderDetailPanel() {
		if (this._detailLoading) return r`<tr><td colspan="6"><div class="detail-panel"><uui-loader-bar></uui-loader-bar></div></td></tr>`;
		let e = this._instanceDetail;
		return e ? r`
      <tr>
        <td colspan="6">
          <div class="detail-panel">
            <div class="detail-grid">
              <div class="detail-item">
                <label>Instance ID</label><p>${e.id}</p>
              </div>
              <div class="detail-item">
                <label>Workflow</label><p>${e.workflowKey} (v${e.workflowVersion})</p>
              </div>
              <div class="detail-item">
                <label>Current Step</label><p>${e.currentStepKey}</p>
              </div>
              <div class="detail-item">
                <label>Status</label>
                <p><span class="status-badge ${(l[e.status] ?? "open").toLowerCase()}">${l[e.status] ?? "Unknown"}</span></p>
              </div>
              <div class="detail-item">
                <label>Created</label><p>${new Date(e.createdAt).toLocaleString()} by ${e.createdBy}</p>
              </div>
              <div class="detail-item">
                <label>Updated</label><p>${new Date(e.updatedAt).toLocaleString()} by ${e.updatedBy}</p>
              </div>
            </div>

            ${e.metadataJson ? r`<div class="metadata-block">${e.metadataJson}</div>` : i}

            ${this._instanceTasks.length > 0 ? r`
                  <div class="tasks-section">
                    <h4>Tasks (${this._instanceTasks.filter((e) => e.isCompleted).length}/${this._instanceTasks.length} completed)</h4>
                    ${this._instanceTasks.map((e) => r`
                      <div class="task-row ${e.isCompleted ? "completed" : ""}">
                        <input type="checkbox" .checked=${e.isCompleted}
                          @click=${(t) => {
			t.stopPropagation(), this._toggleTask(e);
		}} />
                        <span class="task-name">${e.name}</span>
                        ${e.description ? r`<span class="task-desc">— ${e.description}</span>` : i}
                      </div>
                    `)}
                  </div>` : i}

            ${e.status === 0 ? r`
                  <div class="transition-bar">
                    <input type="text" placeholder="Action key (e.g. approve, reject, escalate)"
                      .value=${this._transitionAction}
                      @input=${(e) => {
			this._transitionAction = e.target.value;
		}} />
                    <uui-button label="Execute Transition" look="primary" color="positive"
                      ?disabled=${!this._transitionAction || this._transitioning}
                      @click=${() => this._doTransition()}>
                      ${this._transitioning ? "Processing…" : "Execute"}
                    </uui-button>
                  </div>` : i}
          </div>
        </td>
      </tr>
    ` : i;
	}
};
e([o()], u.prototype, "_rows", void 0), e([o()], u.prototype, "_total", void 0), e([o()], u.prototype, "_page", void 0), e([o()], u.prototype, "_pageSize", void 0), e([o()], u.prototype, "_loading", void 0), e([o()], u.prototype, "_error", void 0), e([o()], u.prototype, "_filterWorkflowKey", void 0), e([o()], u.prototype, "_filterStatus", void 0), e([o()], u.prototype, "_filterAssignedToMe", void 0), e([o()], u.prototype, "_filterFreeText", void 0), e([o()], u.prototype, "_definitions", void 0), e([o()], u.prototype, "_selectedInstanceId", void 0), e([o()], u.prototype, "_instanceDetail", void 0), e([o()], u.prototype, "_instanceTasks", void 0), e([o()], u.prototype, "_detailLoading", void 0), e([o()], u.prototype, "_transitionAction", void 0), e([o()], u.prototype, "_transitioning", void 0), u = e([a("workflow-queue-dashboard")], u);
//#endregion
export { u as WorkflowQueueDashboard };
