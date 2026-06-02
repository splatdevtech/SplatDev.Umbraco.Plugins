import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/shopcart";

class ShopCartDashboard extends UmbElementMixin(LitElement) {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); font-family: var(--uui-font-family, sans-serif); color: var(--uui-color-text, #1a1a1a); }
    .header { display: flex; align-items: center; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .logo { width: 44px; height: 44px; border-radius: 10px; background: #16a34a; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .logo span { color: #fff; font-weight: 900; font-size: 20px; }
    .header h1 { margin: 0 0 4px; font-size: 1.5rem; font-weight: 700; }
    .header p { margin: 0; color: var(--uui-color-text-alt, #6b7280); font-size: 0.875rem; }
    .notice { padding: 12px 16px; border-radius: 6px; font-size: 0.875rem; margin-bottom: 16px; }
    .info  { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .err   { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .ok-notice { background: #dcfce7; color: #15803d; border-left: 3px solid #22c55e; }
    .feature-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 14px; margin-bottom: 20px; }
    .feature-card { background: var(--uui-color-surface, #fff); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 16px; }
    .feature-card h3 { margin: 0 0 6px; font-size: 0.9375rem; font-weight: 700; }
    .feature-card p { margin: 0; font-size: 0.8125rem; color: var(--uui-color-text-alt, #6b7280); line-height: 1.5; }
    .form-row { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 10px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 140px; }
    .field-label { font-size: 0.75rem; font-weight: 600; color: var(--uui-color-text-alt, #6b7280); }
    input[type=text] { border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; padding: 8px 10px; font-size: 0.875rem; background: var(--uui-color-surface, #fff); color: var(--uui-color-text, #1a1a1a); }
    input:focus { outline: none; border-color: #16a34a; box-shadow: 0 0 0 2px rgba(22,163,74,0.15); }
    .result-box { background: #1e293b; color: #e2e8f0; padding: 12px 14px; border-radius: 6px; font-family: monospace; font-size: 0.8125rem; white-space: pre-wrap; word-break: break-all; margin-top: 12px; max-height: 300px; overflow-y: auto; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 10px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; color: var(--uui-color-text-alt, #6b7280); }
    td { padding: 10px 12px; border-bottom: 1px solid var(--uui-color-border, #f0f0f0); }
    uui-box { margin-bottom: 20px; }
    .actions { display: flex; gap: 10px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
    .total-badge { font-size: 1.125rem; font-weight: 700; color: #16a34a; }
  `;

  static properties = {
    _sessionId: { state: true },
    _loading: { state: true },
    _cart: { state: true },
    _total: { state: true },
    _error: { state: true },
  };

  constructor() {
    super();
    this._sessionId = "";
    this._loading = false;
    this._cart = null;
    this._total = null;
    this._error = "";
  }

  async _lookupCart() {
    if (!this._sessionId.trim()) return;
    this._loading = true;
    this._cart = null;
    this._total = null;
    this._error = "";
    try {
      const [cartR, totalR] = await Promise.all([
        fetch(`${API}/GetCart?sessionId=${encodeURIComponent(this._sessionId.trim())}`),
        fetch(`${API}/GetTotal?sessionId=${encodeURIComponent(this._sessionId.trim())}`),
      ]);
      if (cartR.ok) this._cart = await cartR.json();
      if (totalR.ok) this._total = await totalR.json();
    } catch (e) {
      this._error = e instanceof Error ? e.message : String(e);
    } finally {
      this._loading = false;
    }
  }

  render() {
    return html`
      <div class="header">
        <div class="logo"><span>🛒</span></div>
        <div>
          <h1>Shop Cart</h1>
          <p>Session-based shopping cart — inspect cart contents by session ID</p>
        </div>
      </div>

      <div class="notice info">The ShopCart plugin provides a session-based cart API. Cart data is stored per visitor session and can be accessed by session ID for debugging or support purposes.</div>

      <uui-box headline="Plugin Capabilities">
        <div class="feature-grid">
          <div class="feature-card">
            <h3>GetCart</h3>
            <p>Retrieve all items in a session's cart by session ID.</p>
          </div>
          <div class="feature-card">
            <h3>AddItem</h3>
            <p>Add a product to a session cart with quantity and price.</p>
          </div>
          <div class="feature-card">
            <h3>UpdateQuantity</h3>
            <p>Change the quantity of an existing cart item.</p>
          </div>
          <div class="feature-card">
            <h3>RemoveItem</h3>
            <p>Remove a specific item from the cart by item ID.</p>
          </div>
          <div class="feature-card">
            <h3>ClearCart</h3>
            <p>Empty all items from a session's cart.</p>
          </div>
          <div class="feature-card">
            <h3>GetTotal</h3>
            <p>Calculate the total price for all items in a session cart.</p>
          </div>
        </div>
      </uui-box>

      <uui-box headline="Inspect Cart by Session ID">
        <div class="notice info">Enter a visitor session ID to view their current cart contents. Session IDs are typically stored in browser cookies.</div>
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Session ID</span>
            <input type="text" .value=${this._sessionId} @input=${e => this._sessionId = e.target.value} placeholder="Enter session ID…" />
          </div>
          <div class="form-col" style="flex:0;min-width:0;justify-content:flex-end;padding-top:18px">
            <uui-button
              look="primary"
              label="Look Up Cart"
              ?disabled=${!this._sessionId.trim() || this._loading}
              @click=${() => this._lookupCart()}
              style="--uui-button-background-color:#16a34a;--uui-button-background-color-hover:#15803d"
            >${this._loading ? "Loading…" : "Look Up Cart"}</uui-button>
          </div>
        </div>
        ${this._error ? html`<div class="notice err">${this._error}</div>` : ""}
        ${this._cart !== null ? html`
          ${this._total ? html`<div class="notice ok-notice">Total: <span class="total-badge">${typeof this._total === 'number' ? this._total.toFixed(2) : JSON.stringify(this._total)}</span></div>` : ""}
          ${this._cart.length === 0 ? html`<div class="notice info">Cart is empty for this session.</div>` : html`
            <table>
              <thead><tr><th>#</th><th>Product ID</th><th>Name</th><th>Qty</th><th>Price</th></tr></thead>
              <tbody>
                ${this._cart.map(item => html`
                  <tr>
                    <td>${item.id}</td>
                    <td>${item.productId}</td>
                    <td>${item.productName || "—"}</td>
                    <td>${item.quantity}</td>
                    <td>${item.price != null ? item.price.toFixed(2) : "—"}</td>
                  </tr>
                `)}
              </tbody>
            </table>
          `}
        ` : ""}
      </uui-box>
    `;
  }
}

customElements.define("shop-cart-dashboard", ShopCartDashboard);
