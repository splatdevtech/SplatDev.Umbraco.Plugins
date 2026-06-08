import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

class JsonRpcDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; max-width: 600px; line-height: 1.6; }
    .status-card { display: flex; align-items: center; gap: 12px; background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; padding: 16px; margin-bottom: 16px; }
    .icon { width: 48px; height: 48px; border-radius: 10px; background: #3b82f6; display: flex; align-items: center; justify-content: center; font-size: 1.5rem; flex-shrink: 0; }
    .text h2 { margin: 0 0 4px; font-size: 1rem; font-weight: 600; }
    .text p { margin: 0; font-size: 0.875rem; color: var(--uui-color-text-alt, #6b7280); }
    .badge { display: inline-block; padding: 2px 8px; border-radius: 9999px; font-size: 0.7rem; font-weight: 700; text-transform: uppercase; background: #d1fae5; color: #065f46; }
    .endpoints { background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; padding: 20px; margin-top: 16px; }
    .endpoints h3 { margin: 0 0 12px; font-size: 1rem; font-weight: 600; }
    .endpoint-item { display: flex; align-items: center; gap: 8px; padding: 8px 0; border-bottom: 1px solid var(--uui-color-border, #f3f4f6); font-size: 0.875rem; font-family: monospace; }
    .method { padding: 2px 6px; border-radius: 3px; font-size: 0.75rem; font-weight: 700; background: #dbeafe; color: #1d4ed8; }
  `;

  render() {
    return html`
      <h1>JSON-RPC</h1>
      <p class="description">JSON-RPC 2.0 endpoint management for Umbraco. Expose and manage remote procedure call APIs with authentication and rate limiting.</p>
      <div class="status-card">
        <div class="icon">🔌</div>
        <div class="text">
          <h2>JSON-RPC API <span class="badge">Active</span></h2>
          <p>JSON-RPC 2.0 service is running and accepting requests.</p>
        </div>
      </div>
      <div class="endpoints">
        <h3>API Endpoints</h3>
        <div class="endpoint-item"><span class="method">POST</span> /umbraco/api/jsonrpc</div>
        <div class="endpoint-item"><span class="method">GET</span> /umbraco/api/jsonrpc/schema</div>
        <div class="endpoint-item"><span class="method">POST</span> /umbraco/api/jsonrpc/batch</div>
      </div>
    `;
  }
}

customElements.define("jsonrpc-dashboard", JsonRpcDashboard);
export default JsonRpcDashboard;
