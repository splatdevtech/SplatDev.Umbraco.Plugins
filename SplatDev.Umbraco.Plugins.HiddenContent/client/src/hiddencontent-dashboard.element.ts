import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface CheckResult {
  nodeId: number;
  hidden: boolean;
}

@customElement("hiddencontent-dashboard")
export class HiddenContentDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .row { display: flex; gap: 10px; align-items: center; margin-bottom: 14px; flex-wrap: wrap; }
    .status { font-size: 0.875rem; color: #065f46; margin-bottom: 10px; }
    .err-msg { font-size: 0.875rem; color: #b91c1c; margin-bottom: 10px; }
    .check-result { background: var(--uui-color-surface-alt, #f3f4f6); padding: 10px 14px; border-radius: 4px; font-size: 0.9rem; margin-bottom: 16px; }
    uui-table { width: 100%; }
    .empty { color: var(--uui-color-text-alt, #6b7280); padding: 12px 0; }
  `;

  @state() private _hiddenNodes: number[] = [];
  @state() private _loading = false;
  @state() private _checkNodeId = "";
  @state() private _checkResult: CheckResult | null = null;
  @state() private _bulkIds = "";
  @state() private _statusMsg = "";
  @state() private _errorMsg = "";

  private readonly _apiBase = "/umbraco/api/hiddencontent";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadHiddenNodes();
  }

  private async _loadHiddenNodes(): Promise<void> {
    this._loading = true;
    try {
      const res = await fetch(`${this._apiBase}/GetHiddenNodes`);
      if (res.ok) this._hiddenNodes = await res.json();
    } finally {
      this._loading = false;
    }
  }

  private async _post(action: string, params?: string, body?: unknown): Promise<boolean> {
    this._statusMsg = "";
    this._errorMsg = "";
    const url = `${this._apiBase}/${action}${params ? `?${params}` : ""}`;
    const res = await fetch(url, {
      method: "POST",
      headers: body ? { "Content-Type": "application/json" } : {},
      body: body ? JSON.stringify(body) : undefined,
    });
    if (res.ok) {
      const data = await res.json();
      this._statusMsg = data.message ?? "Done.";
      return true;
    }
    this._errorMsg = `Request to ${action} failed.`;
    return false;
  }

  private async _hideNode(nodeId: number): Promise<void> {
    if (await this._post("HideNode", `nodeId=${nodeId}`)) this._loadHiddenNodes();
  }

  private async _showNode(nodeId: number): Promise<void> {
    if (await this._post("ShowNode", `nodeId=${nodeId}`)) this._loadHiddenNodes();
  }

  private async _checkNode(): Promise<void> {
    if (!this._checkNodeId) return;
    const res = await fetch(`${this._apiBase}/IsHidden?nodeId=${this._checkNodeId}`);
    if (res.ok) this._checkResult = await res.json();
  }

  private _parseBulkIds(): number[] {
    return this._bulkIds.split(",").map((s) => parseInt(s.trim(), 10)).filter((n) => !isNaN(n));
  }

  private async _bulkHide(): Promise<void> {
    const ids = this._parseBulkIds();
    if (!ids.length) return;
    if (await this._post("BulkHide", undefined, { nodeIds: ids })) this._loadHiddenNodes();
  }

  private async _bulkShow(): Promise<void> {
    const ids = this._parseBulkIds();
    if (!ids.length) return;
    if (await this._post("BulkShow", undefined, { nodeIds: ids })) this._loadHiddenNodes();
  }

  override render() {
    return html`
      <h1>Hidden Content</h1>
      <p class="description">Hide nodes from navigation and sitemaps while keeping them accessible by direct URL.</p>

      ${this._statusMsg ? html`<p class="status">${this._statusMsg}</p>` : nothing}
      ${this._errorMsg ? html`<p class="err-msg">${this._errorMsg}</p>` : nothing}

      <uui-box headline="Check Node Status">
        <div class="row">
          <uui-input
            placeholder="Node ID"
            type="number"
            .value=${this._checkNodeId}
            @input=${(e: InputEvent) => (this._checkNodeId = (e.target as HTMLInputElement).value)}
          ></uui-input>
          <uui-button look="secondary" label="Check" @click=${this._checkNode}>Check</uui-button>
        </div>
        ${this._checkResult
          ? html`
              <div class="check-result">
                Node <strong>${this._checkResult.nodeId}</strong> is
                <strong>${this._checkResult.hidden ? "hidden" : "visible"}</strong>.
                ${this._checkResult.hidden
                  ? html`<uui-button look="primary" label="Show" @click=${() => this._showNode(this._checkResult!.nodeId)}>Show in Nav</uui-button>`
                  : html`<uui-button look="secondary" label="Hide" @click=${() => this._hideNode(this._checkResult!.nodeId)}>Hide from Nav</uui-button>`}
              </div>
            `
          : nothing}
      </uui-box>

      <uui-box headline="Bulk Operations" style="margin-top:20px">
        <div class="row">
          <uui-input
            placeholder="Node IDs (comma-separated)"
            .value=${this._bulkIds}
            @input=${(e: InputEvent) => (this._bulkIds = (e.target as HTMLInputElement).value)}
            style="min-width:280px"
          ></uui-input>
          <uui-button look="secondary" label="Bulk Hide" @click=${this._bulkHide}>Bulk Hide</uui-button>
          <uui-button look="primary" label="Bulk Show" @click=${this._bulkShow}>Bulk Show</uui-button>
        </div>
      </uui-box>

      <uui-box headline="Currently Hidden Nodes" style="margin-top:20px">
        ${this._loading
          ? html`<p>Loading...</p>`
          : this._hiddenNodes.length === 0
          ? html`<p class="empty">No hidden nodes found.</p>`
          : html`
              <uui-table>
                <uui-table-head>
                  <uui-table-head-cell>Node ID</uui-table-head-cell>
                  <uui-table-head-cell>Actions</uui-table-head-cell>
                </uui-table-head>
                ${this._hiddenNodes.map(
                  (nodeId) => html`
                    <uui-table-row>
                      <uui-table-cell><strong>${nodeId}</strong></uui-table-cell>
                      <uui-table-cell>
                        <uui-button look="primary" label="Show in Nav" @click=${() => this._showNode(nodeId)}>Show in Nav</uui-button>
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

export default HiddenContentDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "hiddencontent-dashboard": HiddenContentDashboardElement;
  }
}
