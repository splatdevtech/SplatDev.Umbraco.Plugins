import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface AnalyticsSettings {
  measurementId: string;
  trackingEnabled: boolean;
}

@customElement("analytics-dashboard")
export class AnalyticsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .fields { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; margin-bottom: 16px; }
    .field { display: flex; flex-direction: column; gap: 4px; }
    .field label { font-size: 0.8rem; font-weight: 500; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .views-list { margin-top: 24px; }
    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
    th { text-align: left; padding: 8px 12px; background: var(--uui-color-surface-alt, #f9fafb); border-bottom: 2px solid var(--uui-color-border, #e5e7eb); }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
  `;

  @state() private _settings: AnalyticsSettings = { measurementId: "", trackingEnabled: false };
  @state() private _pageViews: Array<{ page: string; views: number }> = [];
  @state() private _loading = false;
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/analytics";

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadSettings();
  }

  private async _loadSettings(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GetSettings`);
      this._settings = await r.json();
      if (this._settings.measurementId) await this._loadPageViews();
    } catch {
      this._message = { type: "error", text: "Failed to load settings." };
    }
    this._loading = false;
  }

  private async _saveSettings(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/SaveSettings`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._settings),
      });
      const d = await r.json();
      this._message = { type: r.ok ? "success" : "error", text: d.message ?? "Saved." };
    } catch {
      this._message = { type: "error", text: "Save failed." };
    }
    this._loading = false;
  }

  private async _loadPageViews(): Promise<void> {
    try {
      const r = await fetch(`${this._api}/GetPageViews`);
      if (r.ok) this._pageViews = await r.json();
    } catch { /* */ }
  }

  override render() {
    if (this._loading) return html`<uui-loader-bar></uui-loader-bar>`;

    return html`
      <h1>Analytics</h1>
      <p class="description">Configure Google Analytics tracking and view page statistics.</p>

      ${this._message ? html`<div class="msg ${this._message.type}">${this._message.text}</div>` : nothing}

      <uui-box headline="Settings">
        <div class="fields">
          <div class="field">
            <label>Measurement ID</label>
            <uui-input .value=${this._settings.measurementId}
              @input=${(e: InputEvent) => { this._settings = { ...this._settings, measurementId: (e.target as HTMLInputElement).value }; }}></uui-input>
          </div>
          <div class="field">
            <label>Tracking</label>
            <uui-toggle label="Enable tracking" ?checked=${this._settings.trackingEnabled}
              @change=${(e: Event) => { this._settings = { ...this._settings, trackingEnabled: (e.target as HTMLInputElement).checked }; }}></uui-toggle>
          </div>
        </div>
        <uui-button look="primary" @click=${this._saveSettings}>Save Settings</uui-button>
      </uui-box>

      ${this._pageViews.length > 0 ? html`
        <uui-box headline="Page Views" class="views-list">
          <table>
            <thead><tr><th>Page</th><th>Views</th></tr></thead>
            <tbody>
              ${this._pageViews.map((pv) => html`<tr><td>${pv.page}</td><td>${pv.views}</td></tr>`)}
            </tbody>
          </table>
        </uui-box>
      ` : nothing}
    `;
  }
}

export default AnalyticsDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "analytics-dashboard": AnalyticsDashboardElement;
  }
}
