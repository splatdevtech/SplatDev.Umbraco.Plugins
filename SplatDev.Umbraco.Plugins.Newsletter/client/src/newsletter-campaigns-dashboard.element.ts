import { LitElement, html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface SubscriberListDto {
  id: number;
  name: string;
  createdAt: string;
}

interface CampaignDto {
  id: number;
  name: string;
  templateId: number | null;
  listId: number;
  subject: string;
  status: string;
  scheduledAt: string | null;
  sentAt: string | null;
  createdAt: string;
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

const STATUS_COLORS: Record<string, string> = {
  Draft: "badge-draft",
  Scheduled: "badge-scheduled",
  Sending: "badge-sending",
  Sent: "badge-sent",
};

@customElement("newsletter-campaigns-dashboard")
export class NewsletterCampaignsDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    .desc { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 12px; margin-bottom: 16px; }
    .table-wrap { overflow-x: auto; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 8px 12px; border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
    .badge { border-radius: 9999px; padding: 2px 10px; font-size: 0.7rem; font-weight: 600; }
    .badge-draft { background: #f3f4f6; color: #374151; }
    .badge-scheduled { background: #dbeafe; color: #1e40af; }
    .badge-sending { background: #fef9c3; color: #854d0e; }
    .badge-sent { background: #dcfce7; color: #166534; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
    .form-overlay { background: var(--uui-color-surface-alt, #f9fafb); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 16px 20px; margin-bottom: 16px; }
    .form-row { display: flex; gap: 12px; margin-bottom: 12px; align-items: center; flex-wrap: wrap; }
    .form-row label { font-size: 0.8rem; font-weight: 600; display: block; margin-bottom: 4px; }
    .form-row input, .form-row select { padding: 6px 12px; border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 6px; font-size: 0.875rem; }
    .form-row input { min-width: 200px; }
    .form-actions { display: flex; gap: 8px; margin-top: 12px; }
    .stats-cards { display: flex; gap: 16px; margin-bottom: 16px; flex-wrap: wrap; }
    .stat-card { background: var(--uui-color-surface-alt, #f9fafb); border: 1px solid var(--uui-color-border, #e5e7eb); border-radius: 8px; padding: 12px 20px; min-width: 110px; }
    .stat-card .value { font-size: 1.5rem; font-weight: 700; }
    .stat-card .label { font-size: 0.75rem; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _campaigns: CampaignDto[] = [];
  @state() private _lists: SubscriberListDto[] = [];
  @state() private _loading = false;
  @state() private _showForm = false;
  @state() private _selectedStats: CampaignStatsDto | null = null;
  @state() private _selectedStatsId: number | null = null;
  @state() private _form: Partial<CampaignDto> = {};

  override connectedCallback(): void {
    super.connectedCallback();
    this._load();
  }

  private async _load(): Promise<void> {
    this._loading = true;
    try {
      const [campRes, listRes] = await Promise.all([
        fetch(`${API_BASE}/campaigns`),
        fetch(`${API_BASE}/lists`),
      ]);
      if (campRes.ok) this._campaigns = (await campRes.json()) as CampaignDto[];
      if (listRes.ok) this._lists = (await listRes.json()) as SubscriberListDto[];
    } finally {
      this._loading = false;
    }
  }

  private async _create(): Promise<void> {
    await fetch(`${API_BASE}/campaigns`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(this._form),
    });
    this._showForm = false;
    this._form = {};
    await this._load();
  }

  private async _deleteCampaign(id: number): Promise<void> {
    if (!confirm("Delete this campaign?")) return;
    await fetch(`${API_BASE}/campaigns/${id}`, { method: "DELETE" });
    await this._load();
  }

  private async _sendCampaign(c: CampaignDto): Promise<void> {
    if (!confirm(`Send campaign "${c.subject}" to all subscribers?`)) return;
    const res = await fetch(`${API_BASE}/campaigns/${c.id}/send`, { method: "POST" });
    if (res.ok) {
      const body = (await res.json()) as { sent: number };
      alert(`Sent to ${body.sent} subscribers.`);
    } else {
      const err = await res.text();
      alert(`Send failed: ${err}`);
    }
    await this._load();
  }

  private async _viewStats(campaignId: number): Promise<void> {
    const res = await fetch(`${API_BASE}/campaigns/${campaignId}/stats`);
    if (res.ok) {
      this._selectedStats = (await res.json()) as CampaignStatsDto;
      this._selectedStatsId = campaignId;
    } else {
      this._selectedStats = null;
      this._selectedStatsId = null;
    }
  }

  private async _fetchStats(): Promise<void> {
    if (this._selectedStatsId === null) return;
    const res = await fetch(`${API_BASE}/campaigns/${this._selectedStatsId}/stats/fetch`, { method: "POST" });
    if (res.ok) {
      this._selectedStats = (await res.json()) as CampaignStatsDto;
    }
  }

  private _statusBadge(status: string) {
    const cls = STATUS_COLORS[status] ?? "badge-draft";
    return html`<span class="badge ${cls}">${status}</span>`;
  }

  override render() {
    return html`
      <h1>Newsletter Campaigns</h1>
      <p class="desc">Create, manage, and send email campaigns through Mailgun.</p>

      <div class="toolbar">
        <uui-button look="primary" label="New Campaign" @click=${() => (this._showForm = !this._showForm)}>
          ${this._showForm ? "Cancel" : "New Campaign"}
        </uui-button>
        <uui-button look="default" label="Refresh" @click=${this._load}>Refresh</uui-button>
      </div>

      ${this._showForm
        ? html`
            <div class="form-overlay">
              <div class="form-row">
                <div>
                  <label>Campaign Name</label>
                  <input type="text" placeholder="My Campaign" @input=${(e: Event) => (this._form.name = (e.target as HTMLInputElement).value)} />
                </div>
                <div>
                  <label>Subject</label>
                  <input type="text" placeholder="Subject line" @input=${(e: Event) => (this._form.subject = (e.target as HTMLInputElement).value)} />
                </div>
              </div>
              <div class="form-row">
                <div>
                  <label>List</label>
                  <select @change=${(e: Event) => (this._form.listId = Number((e.target as HTMLSelectElement).value))}>
                    <option value="">-- Select list --</option>
                    ${this._lists.map((l) => html`<option value=${l.id}>${l.name}</option>`)}
                  </select>
                </div>
              </div>
              <div class="form-actions">
                <uui-button look="primary" label="Save" @click=${this._create}>Save</uui-button>
                <uui-button look="default" label="Cancel" @click=${() => (this._showForm = false)}>Cancel</uui-button>
              </div>
            </div>
          `
        : null}

      <uui-box headline="Campaigns">
        ${this._loading
          ? html`<uui-loader></uui-loader>`
          : this._campaigns.length === 0
          ? html`<div class="empty">No campaigns yet. Create one to get started.</div>`
          : html`
              <div class="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Subject</th>
                      <th>Status</th>
                      <th>List ID</th>
                      <th>Created</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    ${this._campaigns.map(
                      (c) => html`
                        <tr>
                          <td>${c.name}</td>
                          <td>${c.subject}</td>
                          <td>${this._statusBadge(c.status)}</td>
                          <td>${c.listId}</td>
                          <td>${new Date(c.createdAt).toLocaleDateString()}</td>
                          <td>
                            ${c.status === "Sent"
                              ? html`<uui-button compact look="default" label="Stats" @click=${() => this._viewStats(c.id)}>Stats</uui-button>`
                              : html`
                                  <uui-button compact look="primary" label="Send" @click=${() => this._sendCampaign(c)}>Send</uui-button>
                                  <uui-button compact look="danger" label="Delete" @click=${() => this._deleteCampaign(c.id)}>Delete</uui-button>
                                `}
                          </td>
                        </tr>
                      `
                    )}
                  </tbody>
                </table>
              </div>
            `}
      </uui-box>

      ${this._selectedStats
        ? html`
            <uui-box headline="Campaign Stats">
              <div class="stats-cards">
                <div class="stat-card"><div class="value">${this._selectedStats.delivered}</div><div class="label">Delivered</div></div>
                <div class="stat-card"><div class="value">${this._selectedStats.opens}</div><div class="label">Opens</div></div>
                <div class="stat-card"><div class="value">${this._selectedStats.clicks}</div><div class="label">Clicks</div></div>
                <div class="stat-card"><div class="value">${this._selectedStats.bounced}</div><div class="label">Bounced</div></div>
              </div>
              <uui-button look="primary" compact label="Fetch from Mailgun" @click=${this._fetchStats}>Refresh from Mailgun</uui-button>
            </uui-box>
          `
        : null}
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "newsletter-campaigns-dashboard": NewsletterCampaignsDashboardElement;
  }
}
