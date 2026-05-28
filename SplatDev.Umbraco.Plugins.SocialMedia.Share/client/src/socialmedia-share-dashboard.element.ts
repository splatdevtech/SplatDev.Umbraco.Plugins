import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface ShareLink {
  platform: string;
  url: string;
}

@customElement("socialmedia-share-dashboard")
export class SocialMediaShareDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .input-row { display: flex; gap: 10px; align-items: flex-end; margin-bottom: 20px; flex-wrap: wrap; }
    .field { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 200px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .links-list { display: flex; flex-direction: column; gap: 8px; }
    .link-card { display: flex; justify-content: space-between; align-items: center; padding: 12px; border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 6px; }
    .link-platform { font-weight: 600; text-transform: capitalize; min-width: 120px; }
    .link-url { font-size: 0.8rem; color: var(--uui-color-text-alt, #6b7280); word-break: break-all; flex: 1; margin: 0 12px; }
  `;

  @state() private _pageUrl = "";
  @state() private _pageTitle = "";
  @state() private _links: ShareLink[] = [];
  @state() private _loading = false;
  @state() private _error = "";

  private readonly _api = "/umbraco/api/ShareApi";

  private async _generateLinks(): Promise<void> {
    if (!this._pageUrl || !this._pageTitle) return;
    this._loading = true;
    this._error = "";
    try {
      const params = new URLSearchParams({ pageUrl: this._pageUrl, pageTitle: this._pageTitle });
      const r = await fetch(`${this._api}/GetShareLinks?${params}`);
      if (!r.ok) throw new Error();
      this._links = await r.json();
    } catch {
      this._error = "Failed to generate share links.";
      this._links = [];
    }
    this._loading = false;
  }

  override render() {
    return html`
      <h1>Social Media Share</h1>
      <p class="description">Generate social sharing links for any page.</p>

      <uui-box headline="Generate Share Links">
        <div class="input-row">
          <div class="field">
            <label>Page URL</label>
            <uui-input placeholder="https://example.com/page" .value=${this._pageUrl}
              @input=${(e: InputEvent) => (this._pageUrl = (e.target as HTMLInputElement).value)}></uui-input>
          </div>
          <div class="field">
            <label>Page Title</label>
            <uui-input placeholder="My Page Title" .value=${this._pageTitle}
              @input=${(e: InputEvent) => (this._pageTitle = (e.target as HTMLInputElement).value)}></uui-input>
          </div>
          <uui-button look="primary" ?disabled=${!this._pageUrl || !this._pageTitle || this._loading}
            @click=${this._generateLinks}>Generate</uui-button>
        </div>
      </uui-box>

      ${this._error ? html`<div class="msg error">${this._error}</div>` : nothing}
      ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}

      ${this._links.length > 0 ? html`
        <uui-box headline="Share Links" style="margin-top:16px;">
          <div class="links-list">
            ${this._links.map((l) => html`
              <div class="link-card">
                <span class="link-platform">${l.platform}</span>
                <span class="link-url">${l.url}</span>
                <uui-button look="secondary" compact @click=${() => navigator.clipboard.writeText(l.url)}>Copy</uui-button>
              </div>
            `)}
          </div>
        </uui-box>
      ` : nothing}
    `;
  }
}

export default SocialMediaShareDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "socialmedia-share-dashboard": SocialMediaShareDashboardElement;
  }
}
