import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit/decorators.js";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface WorkflowDisplayRow {
  instanceId: number;
  values: Record<string, unknown>;
}

interface PagedResult {
  items: WorkflowDisplayRow[];
  totalCount: number;
  page: number;
  pageSize: number;
}

interface WorkflowInstanceDetail {
  id: number;
  workflowKey: string;
  workflowVersion: number;
  currentStepKey: string;
  status: number;
  metadataJson: string | null;
  createdAt: string;
  createdBy: string;
  updatedAt: string;
  updatedBy: string;
}

interface WorkflowTask {
  id: number;
  instanceId: number;
  alias: string;
  name: string;
  description: string | null;
  isCompleted: boolean;
  completedAt: string | null;
  completedBy: string | null;
  departmentId: number | null;
}

interface WorkflowDefinitionSummary {
  key: string;
  label: string;
  version: number;
  isActive: boolean;
  definitionJson: string;
  createdAt: string;
}

const API = "/umbraco/backoffice/SplatDevWorkflow";

const STATUS_LABELS: Record<number, string> = {
  0: "Open",
  1: "Completed",
  2: "Cancelled",
};

@customElement("workflow-queue-dashboard")
export class WorkflowQueueDashboard extends UmbElementMixin(LitElement) {
  @state() private _rows: WorkflowDisplayRow[] = [];
  @state() private _total = 0;
  @state() private _page = 1;
  @state() private _pageSize = 50;
  @state() private _loading = false;
  @state() private _error = "";

  @state() private _filterWorkflowKey = "";
  @state() private _filterStatus: string = "";
  @state() private _filterAssignedToMe = false;
  @state() private _filterFreeText = "";

  @state() private _definitions: WorkflowDefinitionSummary[] = [];

  @state() private _selectedInstanceId: number | null = null;
  @state() private _instanceDetail: WorkflowInstanceDetail | null = null;
  @state() private _instanceTasks: WorkflowTask[] = [];
  @state() private _detailLoading = false;

  @state() private _transitionAction = "";
  @state() private _transitioning = false;

  connectedCallback(): void {
    super.connectedCallback();
    this._loadDefinitions();
    this._loadQueue();
  }

  private async _loadDefinitions(): Promise<void> {
    try {
      const resp = await fetch(`${API}/WorkflowDefinitions/List`);
      if (resp.ok) this._definitions = await resp.json();
    } catch {
      // non-critical
    }
  }

  private async _loadQueue(): Promise<void> {
    this._loading = true;
    this._error = "";
    try {
      const params = new URLSearchParams();
      params.set("page", String(this._page));
      params.set("pageSize", String(this._pageSize));
      if (this._filterWorkflowKey) params.set("workflowKey", this._filterWorkflowKey);
      if (this._filterStatus !== "") params.set("status", this._filterStatus);
      if (this._filterAssignedToMe) params.set("assignedToMe", "true");
      if (this._filterFreeText) params.set("freeText", this._filterFreeText);

      const resp = await fetch(`${API}/WorkflowInstances/List?${params}`);
      if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
      const data: PagedResult = await resp.json();
      this._rows = data.items;
      this._total = data.totalCount;
    } catch (e: unknown) {
      this._error = e instanceof Error ? e.message : "Failed to load queue";
    } finally {
      this._loading = false;
    }
  }

  private async _selectInstance(id: number): Promise<void> {
    if (this._selectedInstanceId === id) {
      this._selectedInstanceId = null;
      this._instanceDetail = null;
      this._instanceTasks = [];
      return;
    }
    this._selectedInstanceId = id;
    this._detailLoading = true;
    this._transitionAction = "";
    try {
      const [instResp, tasksResp] = await Promise.all([
        fetch(`${API}/WorkflowInstances/Get/${id}`),
        fetch(`${API}/WorkflowTasks/List/${id}`),
      ]);
      this._instanceDetail = instResp.ok ? await instResp.json() : null;
      this._instanceTasks = tasksResp.ok ? await tasksResp.json() : [];
    } catch {
      this._instanceDetail = null;
      this._instanceTasks = [];
    } finally {
      this._detailLoading = false;
    }
  }

  private async _doTransition(): Promise<void> {
    if (!this._selectedInstanceId || !this._transitionAction) return;
    this._transitioning = true;
    try {
      const resp = await fetch(`${API}/WorkflowInstances/Transition/${this._selectedInstanceId}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ actionKey: this._transitionAction }),
      });
      if (!resp.ok) {
        const problem = await resp.json().catch(() => null);
        throw new Error(problem?.detail ?? `HTTP ${resp.status}`);
      }
      this._selectedInstanceId = null;
      this._instanceDetail = null;
      this._instanceTasks = [];
      await this._loadQueue();
    } catch (e: unknown) {
      this._error = e instanceof Error ? e.message : "Transition failed";
    } finally {
      this._transitioning = false;
    }
  }

  private async _toggleTask(task: WorkflowTask): Promise<void> {
    if (!this._selectedInstanceId) return;
    try {
      await fetch(`${API}/WorkflowTasks/SetCompletion/${this._selectedInstanceId}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ entries: [{ taskId: task.id, isCompleted: !task.isCompleted }] }),
      });
      task.isCompleted = !task.isCompleted;
      this._instanceTasks = [...this._instanceTasks];
    } catch {
      // silent
    }
  }

  private _timeAgo(dateStr: string): string {
    const diff = Date.now() - new Date(dateStr).getTime();
    const mins = Math.floor(diff / 60000);
    if (mins < 1) return "just now";
    if (mins < 60) return `${mins}m ago`;
    const hrs = Math.floor(mins / 60);
    if (hrs < 24) return `${hrs}h ago`;
    const days = Math.floor(hrs / 24);
    return `${days}d ago`;
  }

  private get _totalPages(): number {
    return Math.max(1, Math.ceil(this._total / this._pageSize));
  }

  static styles = css`
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

  render() {
    return html`
      <uui-box headline="Workflow Queue">
        ${this._error
          ? html`<div class="error-bar">
              <span>${this._error}</span>
              <uui-button label="Dismiss" look="secondary" @click=${() => (this._error = "")}>&#x2715;</uui-button>
            </div>`
          : nothing}

        <div class="filters">
          <div class="filter-group">
            <label>Workflow</label>
            <select @change=${(e: Event) => { this._filterWorkflowKey = (e.target as HTMLSelectElement).value; }}>
              <option value="">All Workflows</option>
              ${this._definitions.map(
                (d) => html`<option value=${d.key} ?selected=${this._filterWorkflowKey === d.key}>${d.label}</option>`
              )}
            </select>
          </div>

          <div class="filter-group">
            <label>Status</label>
            <select @change=${(e: Event) => { this._filterStatus = (e.target as HTMLSelectElement).value; }}>
              <option value="">All</option>
              <option value="0" ?selected=${this._filterStatus === "0"}>Open</option>
              <option value="1" ?selected=${this._filterStatus === "1"}>Completed</option>
              <option value="2" ?selected=${this._filterStatus === "2"}>Cancelled</option>
            </select>
          </div>

          <div class="filter-group">
            <label>Search</label>
            <input type="text" placeholder="Free text..." .value=${this._filterFreeText}
              @input=${(e: Event) => { this._filterFreeText = (e.target as HTMLInputElement).value; }} />
          </div>

          <div class="assigned-toggle">
            <input type="checkbox" id="assignedToMe" .checked=${this._filterAssignedToMe}
              @change=${(e: Event) => { this._filterAssignedToMe = (e.target as HTMLInputElement).checked; }} />
            <label for="assignedToMe">Assigned to me</label>
          </div>

          <uui-button label="Search" look="primary" @click=${() => { this._page = 1; this._loadQueue(); }}>
            <uui-icon name="icon-search"></uui-icon> Search
          </uui-button>
          <uui-button label="Refresh" look="secondary" @click=${() => this._loadQueue()}>
            <uui-icon name="icon-refresh"></uui-icon>
          </uui-button>
        </div>

        ${this._loading
          ? html`<uui-loader-bar></uui-loader-bar>`
          : this._rows.length === 0
            ? html`<div class="empty-state">
                <uui-icon name="icon-inbox" style="font-size:48px;opacity:0.3"></uui-icon>
                <p>No workflow instances found</p>
              </div>`
            : html`
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
                    ${this._rows.map((row) => html`
                      <tr class="clickable ${this._selectedInstanceId === row.instanceId ? "selected" : ""}"
                        @click=${() => this._selectInstance(row.instanceId)}>
                        <td>${row.instanceId}</td>
                        <td>${row.values?.["workflowKey"] ?? "—"}</td>
                        <td>${row.values?.["currentStepKey"] ?? "—"}</td>
                        <td>
                          <span class="status-badge ${(STATUS_LABELS[Number(row.values?.["status"])] ?? "open").toLowerCase()}">
                            ${STATUS_LABELS[Number(row.values?.["status"])] ?? "Unknown"}
                          </span>
                        </td>
                        <td>${row.values?.["createdAt"] ? this._timeAgo(String(row.values["createdAt"])) : "—"}</td>
                        <td>${row.values?.["createdBy"] ?? "—"}</td>
                      </tr>
                      ${this._selectedInstanceId === row.instanceId ? this._renderDetailPanel() : nothing}
                    `)}
                  </tbody>
                </table>

                <div class="pagination">
                  <span>Showing ${(this._page - 1) * this._pageSize + 1}–${Math.min(this._page * this._pageSize, this._total)} of ${this._total}</span>
                  <div class="pagination-buttons">
                    <uui-button label="Previous" look="secondary" ?disabled=${this._page <= 1}
                      @click=${() => { this._page--; this._loadQueue(); }}>← Prev</uui-button>
                    <uui-button label="Next" look="secondary" ?disabled=${this._page >= this._totalPages}
                      @click=${() => { this._page++; this._loadQueue(); }}>Next →</uui-button>
                  </div>
                </div>
              `}
      </uui-box>
    `;
  }

  private _renderDetailPanel() {
    if (this._detailLoading) {
      return html`<tr><td colspan="6"><div class="detail-panel"><uui-loader-bar></uui-loader-bar></div></td></tr>`;
    }
    const d = this._instanceDetail;
    if (!d) return nothing;

    return html`
      <tr>
        <td colspan="6">
          <div class="detail-panel">
            <div class="detail-grid">
              <div class="detail-item">
                <label>Instance ID</label><p>${d.id}</p>
              </div>
              <div class="detail-item">
                <label>Workflow</label><p>${d.workflowKey} (v${d.workflowVersion})</p>
              </div>
              <div class="detail-item">
                <label>Current Step</label><p>${d.currentStepKey}</p>
              </div>
              <div class="detail-item">
                <label>Status</label>
                <p><span class="status-badge ${(STATUS_LABELS[d.status] ?? "open").toLowerCase()}">${STATUS_LABELS[d.status] ?? "Unknown"}</span></p>
              </div>
              <div class="detail-item">
                <label>Created</label><p>${new Date(d.createdAt).toLocaleString()} by ${d.createdBy}</p>
              </div>
              <div class="detail-item">
                <label>Updated</label><p>${new Date(d.updatedAt).toLocaleString()} by ${d.updatedBy}</p>
              </div>
            </div>

            ${d.metadataJson ? html`<div class="metadata-block">${d.metadataJson}</div>` : nothing}

            ${this._instanceTasks.length > 0
              ? html`
                  <div class="tasks-section">
                    <h4>Tasks (${this._instanceTasks.filter((t) => t.isCompleted).length}/${this._instanceTasks.length} completed)</h4>
                    ${this._instanceTasks.map((task) => html`
                      <div class="task-row ${task.isCompleted ? "completed" : ""}">
                        <input type="checkbox" .checked=${task.isCompleted}
                          @click=${(e: Event) => { e.stopPropagation(); this._toggleTask(task); }} />
                        <span class="task-name">${task.name}</span>
                        ${task.description ? html`<span class="task-desc">— ${task.description}</span>` : nothing}
                      </div>
                    `)}
                  </div>`
              : nothing}

            ${d.status === 0
              ? html`
                  <div class="transition-bar">
                    <input type="text" placeholder="Action key (e.g. approve, reject, escalate)"
                      .value=${this._transitionAction}
                      @input=${(e: Event) => { this._transitionAction = (e.target as HTMLInputElement).value; }} />
                    <uui-button label="Execute Transition" look="primary" color="positive"
                      ?disabled=${!this._transitionAction || this._transitioning}
                      @click=${() => this._doTransition()}>
                      ${this._transitioning ? "Processing…" : "Execute"}
                    </uui-button>
                  </div>`
              : nothing}
          </div>
        </td>
      </tr>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "workflow-queue-dashboard": WorkflowQueueDashboard;
  }
}
