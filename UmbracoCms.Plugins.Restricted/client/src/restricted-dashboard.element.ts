import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface RestrictNodeRequest {
  nodeId: number;
  loginPageNodeId: string;
  errorPageNodeId: string;
  memberGroups: string[];
}

@customElement("restricted-dashboard")
export class RestrictedDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .form-grid { display: grid; gap: 14px; max-width: 480px; margin-bottom: 20px; }
    .field label { display: block; font-weight: 600; font-size: 0.875rem; margin-bottom: 4px; }
    .actions { margin-top: 12px; display: flex; gap: 12px; align-items: center; }
    .status { font-size: 0.875rem; color: #065f46; }
    .err-msg { font-size: 0.875rem; color: #b91c1c; }
    uui-table { width: 100%; }
    .empty { color: var(--uui-color-text-alt, #6b7280); padding: 12px 0; }
  `;

  @state() private _restrictedNodes: number[] = [];
  @state() private _loading = false;
  @state() private _nodeId = "";
  @state() private _loginPageNodeId = "";
  @state() private _errorPageNodeId = "";
  @state() private _memberGroups = "";
  @state() private _statusMsg = "";
  @state() private _errorMsg = "";

  private readonly _apiBase = "/umbraco/api/restricted";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadRestrictedNodes();
  }

  private async _loadRestrictedNodes(): Promise<void> {
    this._loading = true;
    try {
      const res = await fetch(`${this._apiBase}/GetRestrictedNodes`);
      if (res.ok) this._restrictedNodes = await res.json();
    } finally {
      this._loading = false;
    }
  }

  private async _restrictNode(): Promise<void> {
    if (!this._nodeId) return;
    this._statusMsg = "";
    this._errorMsg = "";
    const groups = this._memberGroups.split(",").map((g) => g.trim()).filter(Boolean);
    const payload: RestrictNodeRequest = {
      nodeId: parseInt(this._nodeId, 10),
      loginPageNodeId: this._loginPageNodeId,
      errorPageNodeId: this._errorPageNodeId,
      memberGroups: groups,
    };
    const res = await fetch(`${this._apiBase}/RestrictNode`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
    if (res.ok) {
      const data = await res.json();
      this._statusMsg = data.message;
      this._loadRestrictedNodes();
    } else {
      this._errorMsg = "Failed to restrict node.";
    }
  }

  private async _unrestrictNode(nodeId: number): Promise<void> {
    this._statusMsg = "";
    this._errorMsg = "";
    const res = await fetch(`${this._apiBase}/UnrestrictNode?nodeId=${nodeId}`, { method: "DELETE" });
    if (res.ok) {
      this._statusMsg = `Node ${nodeId} unrestricted.`;
      this._loadRestrictedNodes();
    } else {
      this._errorMsg = `Failed to unrestrict node ${nodeId}.`;
    }
  }

  override render() {
    return html`
      <h1>Restricted Content</h1>
      <p class="description">Manage member-only content gates, role-based access and paywall restrictions.</p>

      ${this._statusMsg ? html`<p class="status">${this._statusMsg}</p>` : nothing}
      ${this._errorMsg ? html`<p class="err-msg">${this._errorMsg}</p>` : nothing}

      <uui-box headline="Restrict a Node">
        <div class="form-grid">
          <div class="field">
            <label>Content Node ID</label>
            <uui-input placeholder="e.g. 1234" .value=${this._nodeId}
              @input=${(e: InputEvent) => (this._nodeId = (e.target as HTMLInputElement).value)}
            ></uui-input>
          </div>
          <div class="field">
            <label>Login Page Node ID</label>
            <uui-input placeholder="e.g. 1050" .value=${this._loginPageNodeId}
              @input=${(e: InputEvent) => (this._loginPageNodeId = (e.target as HTMLInputElement).value)}
            ></uui-input>
          </div>
          <div class="field">
            <label>Error Page Node ID</label>
            <uui-input placeholder="e.g. 1051" .value=${this._errorPageNodeId}
              @input=${(e: InputEvent) => (this._errorPageNodeId = (e.target as HTMLInputElement).value)}
            ></uui-input>
          </div>
          <div class="field">
            <label>Member Groups (comma-separated)</label>
            <uui-input placeholder="e.g. Members, Premium" .value=${this._memberGroups}
              @input=${(e: InputEvent) => (this._memberGroups = (e.target as HTMLInputElement).value)}
            ></uui-input>
          </div>
        </div>
        <uui-button look="primary" label="Apply Restriction" @click=${this._restrictNode}>Apply Restriction</uui-button>
      </uui-box>

      <uui-box headline="Currently Restricted Nodes" style="margin-top:20px">
        ${this._loading
          ? html`<p>Loading...</p>`
          : this._restrictedNodes.length === 0
          ? html`<p class="empty">No restricted nodes found.</p>`
          : html`
              <uui-table>
                <uui-table-head>
                  <uui-table-head-cell>Node ID</uui-table-head-cell>
                  <uui-table-head-cell>Actions</uui-table-head-cell>
                </uui-table-head>
                ${this._restrictedNodes.map(
                  (nodeId) => html`
                    <uui-table-row>
                      <uui-table-cell><strong>${nodeId}</strong></uui-table-cell>
                      <uui-table-cell>
                        <uui-button
                          look="secondary"
                          label="Remove Restriction"
                          @click=${() => this._unrestrictNode(nodeId)}
                        >Remove Restriction</uui-button>
                      </uui-table-cell>
                    </uui-table-row>
                  `
                )}
              </uui-table>
            `}
      </uui-box>
    `;
  }
}

export default RestrictedDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "restricted-dashboard": RestrictedDashboardElement;
  }
}
