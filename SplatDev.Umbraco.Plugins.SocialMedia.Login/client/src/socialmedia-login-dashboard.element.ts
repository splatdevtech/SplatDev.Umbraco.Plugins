import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface SocialProvider {
  name: string;
  enabled: boolean;
}

@customElement("socialmedia-login-dashboard")
export class SocialMediaLoginDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .provider-list { display: flex; flex-direction: column; gap: 8px; }
    .provider-card { display: flex; justify-content: space-between; align-items: center; padding: 12px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; }
    .provider-name { font-weight: 600; font-size: 0.95rem; text-transform: capitalize; }
    .status-badge { padding: 2px 10px; border-radius: 9999px; font-size: 0.8rem; font-weight: 600; }
    .status-badge.enabled { background: #d1fae5; color: #065f46; }
    .status-badge.disabled { background: #fee2e2; color: #991b1b; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _providers: SocialProvider[] = [];
  @state() private _loading = false;

  private readonly _api = "/umbraco/api/SocialLoginApi";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadProviders();
  }

  private async _loadProviders(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetProviders`);
      this._providers = await r.json();
    } catch { this._providers = []; }
    this._loading = false;
  }

  override render() {
    if (this._loading) return html`<uui-loader-bar></uui-loader-bar>`;

    return html`
      <h1>Social Media Login</h1>
      <p class="description">View configured social login providers and their status.</p>

      ${this._providers.length === 0
        ? html`<div class="empty">No social login providers configured.</div>`
        : html`
            <div class="provider-list">
              ${this._providers.map((p) => html`
                <div class="provider-card">
                  <span class="provider-name">${p.name}</span>
                  <span class="status-badge ${p.enabled ? "enabled" : "disabled"}">
                    ${p.enabled ? "Enabled" : "Disabled"}
                  </span>
                </div>
              `)}
            </div>
          `}
    `;
  }
}

export default SocialMediaLoginDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "socialmedia-login-dashboard": SocialMediaLoginDashboardElement;
  }
}
