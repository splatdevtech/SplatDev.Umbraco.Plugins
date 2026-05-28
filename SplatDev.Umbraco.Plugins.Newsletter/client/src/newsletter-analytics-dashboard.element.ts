import { LitElement, html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface CampaignDto {
  id: number;
  name: string;
  subject: string;
  status: string;
  sentAt: string | null;
}

interface CampaignStatsDto {
  id: number;
  campaignId: number;
  opens: number;
  clicks: number;
  delivered: number;
  bounced: number;
  fetchedAt: string;
}

const API_BASE = "/umbraco/management/api/v1/newsletter";

@customElement("newsletter-analytics-dashboard")
export class NewsletterAnalyticsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    .desc { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 12px; margin-bottom: 16px; align-items: center; }
    .toolbar select { padding: 6px 12px; border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 6px; font-size: 0.875rem; }
    .stats-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); gap: 16px; margin-bottom: 16px; }
    .stat-card { background: var(--uui-color-surface-alt, #f9fafb); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 10px; padding: 20px; text-align: center; }
    .stat-card .value { font-size: 2.25rem; font-weight: 700; }
    .stat-card .label { font-size: 0.8rem; color: var(--uui-color-text-alt, #6b7280); margin-top: 4px; }
    .stat-card .rate { font-size: 0.75rem; color: var(--uui-color-text-alt, #9ca3af); margin-top: 2px; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _campaigns: CampaignDto[] = [];
  @state() private _selectedCampaignId: number | null = null;
  @state() private _stats: CampaignStatsDto | null = null;
  @state() private _loading = false;

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadCampaigns();
  }

  private async _loadCampaigns(): Promise<void> {
    this._loading = true;
    try {
      const res = await fetch(`${API_BASE}/campaigns`);
      if (res.ok) {
        this._campaigns = ((await res.json()) as CampaignDto[]).filter(
          (c) => c.status === "Sent"
        );
      }
    } finally {
      this._loading = false;
    }
  }

  private async _onCampaignChange(e: Event): Promise<void> {
    const val = (e.target as HTMLSelectElement).value;
    this._selectedCampaignId = val ? Number(val) : null;
    if (this._selectedCampaignId === null) {
      this._stats = null;
      return;
    }
    await this._loadStats();
  }

  private async _loadStats(): Promise<void> {
    if (this._selectedCampaignId === null) return;
    this._loading = true;
    try {
      const res = await fetch(`${API_BASE}/campaigns/${this._selectedCampaignId}/stats`);
      if (res.ok) this._stats = (await res.json()) as CampaignStatsDto;
      else this._stats = null;
    } finally {
      this._loading = false;
    }
  }

  private async _fetchFromMailgun(): Promise<void> {
    if (this._selectedCampaignId === null) return;
    const res = await fetch(`${API_BASE}/campaigns/${this._selectedCampaignId}/stats/fetch`, { method: "POST" });
    if (res.ok) this._stats = (await res.json()) as CampaignStatsDto;
  }

  private _rate(part: number, total: number): string {
    if (total === 0) return "\u2014";
    return `${((part / total) * 100).toFixed(1)}%`;
  }

  override render() {
    const d = this._stats?.delivered ?? 0;

    return html`
      <h1>Newsletter Analytics</h1>
      <p class="desc">View delivery, open, click, and bounce statistics for sent campaigns.</p>

      <div class="toolbar">
        <select @change=${this._onCampaignChange}>
          <option value="">-- Select sent campaign --</option>
          ${this._campaigns.map(
            (c) => html`<option value=${c.id}>${c.name} (${c.subject})</option>`
          )}
        </select>
        ${this._selectedCampaignId
          ? html`<uui-button look="primary" compact label="Fetch from Mailgun" @click=${this._fetchFromMailgun}>Refresh from Mailgun</uui-button>`
          : null}
      </div>

      ${this._loading
        ? html`<uui-loader></uui-loader>`
        : this._stats
        ? html`
            <uui-box headline="Campaign Performance">
              <div class="stats-grid">
                <div class="stat-card">
                  <div class="value">${d.toLocaleString()}</div>
                  <div class="label">Delivered</div>
                </div>
                <div class="stat-card">
                  <div class="value">${this._stats.opens.toLocaleString()}</div>
                  <div class="label">Opens</div>
                  <div class="rate">${this._rate(this._stats.opens, d)} open rate</div>
                </div>
                <div class="stat-card">
                  <div class="value">${this._stats.clicks.toLocaleString()}</div>
                  <div class="label">Clicks</div>
                  <div class="rate">${this._rate(this._stats.clicks, d)} click rate</div>
                </div>
                <div class="stat-card">
                  <div class="value">${this._stats.bounced.toLocaleString()}</div>
                  <div class="label">Bounced</div>
                  <div class="rate">${this._rate(this._stats.bounced, d)} bounce rate</div>
                </div>
              </div>
            </uui-box>
          `
        : html`<div class="empty">Select a sent campaign to view its analytics.</div>`}
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "newsletter-analytics-dashboard": NewsletterAnalyticsDashboardElement;
  }
}
