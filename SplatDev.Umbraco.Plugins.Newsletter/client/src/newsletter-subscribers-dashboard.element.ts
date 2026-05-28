import { LitElement, html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

interface SubscriberListDto {
  id: number;
  name: string;
  createdAt: string;
}

interface SubscriberDto {
  id: number;
  listId: number;
  email: string;
  name: string | null;
  active: boolean;
  subcribedAt: string;
  unsubscribedAt: string | null;
}

const API_BASE = "/umbraco/management/api/v1/newsletter";

@customElement("newsletter-subscribers-dashboard")
export class NewsletterSubscribersDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    .desc { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .toolbar { display: flex; gap: 12px; margin-bottom: 16px; align-items: center; }
    .toolbar select { padding: 6px 12px; border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 6px; font-size: 0.875rem; }
    .toolbar input { padding: 6px 12px; border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 6px; font-size: 0.875rem; flex: 1; max-width: 260px; }
    .table-wrap { overflow-x: auto; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th { text-align: left; padding: 8px 12px; border-bottom: 2px solid var(--uui-color-border, #e5e7eb); font-weight: 600; }
    td { padding: 8px 12px; border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
    .badge { border-radius: 9999px; padding: 2px 10px; font-size: 0.7rem; font-weight: 600; }
    .badge-active { background: #dcfce7; color: #166534; }
    .badge-inactive { background: #f3f4f6; color: #374151; }
    .empty { text-align: center; padding: 32px; color: var(--uui-color-text-alt, #6b7280); }
  `;

  @state() private _lists: SubscriberListDto[] = [];
  @state() private _subscribers: SubscriberDto[] = [];
  @state() private _selectedListId: number | null = null;
  @state() private _loading = false;

  override connectedCallback(): void {
    super.connectedCallback();
    this._loadLists();
  }

  private async _loadLists(): Promise<void> {
    this._loading = true;
    try {
      const res = await fetch(`${API_BASE}/lists`);
      if (res.ok) {
        this._lists = (await res.json()) as SubscriberListDto[];
        if (this._lists.length > 0 && this._selectedListId === null) {
          this._selectedListId = this._lists[0].id;
          await this._loadSubscribers();
        }
      }
    } finally {
      this._loading = false;
    }
  }

  private async _loadSubscribers(): Promise<void> {
    if (this._selectedListId === null) return;
    const res = await fetch(`${API_BASE}/lists/${this._selectedListId}/subscribers`);
    if (res.ok) this._subscribers = (await res.json()) as SubscriberDto[];
  }

  private async _createList(): Promise<void> {
    const name = prompt("List name:");
    if (!name?.trim()) return;
    await fetch(`${API_BASE}/lists`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name: name.trim() }),
    });
    await this._loadLists();
  }

  private async _deleteList(id: number): Promise<void> {
    if (!confirm("Delete this list and all its subscribers?")) return;
    await fetch(`${API_BASE}/lists/${id}`, { method: "DELETE" });
    if (this._selectedListId === id) this._selectedListId = null;
    await this._loadLists();
    if (this._selectedListId === null && this._lists.length > 0) {
      this._selectedListId = this._lists[0].id;
      await this._loadSubscribers();
    }
  }

  private async _subscribe(): Promise<void> {
    if (this._selectedListId === null) return;
    const email = prompt("Email:");
    if (!email?.trim()) return;
    const name = prompt("Name (optional):")?.trim() || undefined;
    await fetch(`${API_BASE}/lists/${this._selectedListId}/subscribers`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: email.trim(), name }),
    });
    await this._loadSubscribers();
  }

  private async _unsubscribe(email: string): Promise<void> {
    if (this._selectedListId === null) return;
    if (!confirm(`Unsubscribe ${email}?`)) return;
    await fetch(`${API_BASE}/lists/${this._selectedListId}/subscribers/${encodeURIComponent(email)}`, { method: "DELETE" });
    await this._loadSubscribers();
  }

  private async _deleteSubscriber(id: number): Promise<void> {
    if (!confirm("Permanently delete this subscriber?")) return;
    await fetch(`${API_BASE}/subscribers/${id}`, { method: "DELETE" });
    await this._loadSubscribers();
  }

  private async _onListChange(e: Event): Promise<void> {
    const val = (e.target as HTMLSelectElement).value;
    this._selectedListId = val ? Number(val) : null;
    await this._loadSubscribers();
  }

  override render() {
    return html`
      <h1>Newsletter Subscribers</h1>
      <p class="desc">Manage subscriber lists and contacts.</p>

      <div class="toolbar">
        <select @change=${this._onListChange}>
          ${this._lists.map(
            (l) => html`<option value=${l.id} ?selected=${l.id === this._selectedListId}>${l.name}</option>`
          )}
        </select>
        <uui-button look="primary" compact label="Add List" @click=${this._createList}>
          Add List
        </uui-button>
        ${this._selectedListId
          ? html`
              <uui-button look="default" compact label="Delete List" @click=${() => this._deleteList(this._selectedListId!)}>
                Delete List
              </uui-button>
              <uui-button look="primary" compact label="Add Subscriber" @click=${this._subscribe}>
                Add Subscriber
              </uui-button>
            `
          : null}
      </div>

      <uui-box headline="Subscribers">
        ${this._loading
          ? html`<uui-loader></uui-loader>`
          : this._subscribers.length === 0
          ? html`<div class="empty">No subscribers in this list.</div>`
          : html`
              <div class="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>Email</th>
                      <th>Name</th>
                      <th>Status</th>
                      <th>Subscribed</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    ${this._subscribers.map(
                      (s) => html`
                        <tr>
                          <td>${s.email}</td>
                          <td>${s.name ?? "\u2014"}</td>
                          <td><span class="badge ${s.active ? "badge-active" : "badge-inactive"}">${s.active ? "Active" : "Inactive"}</span></td>
                          <td>${new Date(s.subcribedAt).toLocaleDateString()}</td>
                          <td>
                            ${s.active
                              ? html`<uui-button compact look="default" label="Unsubscribe" @click=${() => this._unsubscribe(s.email)}>Unsub</uui-button>`
                              : html`<uui-button compact look="danger" label="Delete" @click=${() => this._deleteSubscriber(s.id)}>Delete</uui-button>`}
                          </td>
                        </tr>
                      `
                    )}
                  </tbody>
                </table>
              </div>
            `}
      </uui-box>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "newsletter-subscribers-dashboard": NewsletterSubscribersDashboardElement;
  }
}
