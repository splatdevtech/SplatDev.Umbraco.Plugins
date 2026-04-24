import { LitElement, html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

@customElement("examine-extensions-dashboard")
export class ExamineExtensionsDashboard extends UmbElementMixin(LitElement) {
    static styles = css`
        :host { display: block; padding: 20px; }
        .search-row { display: flex; gap: 12px; align-items: flex-end; margin-bottom: 16px; }
        .results-table { width: 100%; border-collapse: collapse; margin-top: 16px; }
        .results-table th, .results-table td { border: 1px solid var(--uui-color-border); padding: 8px 12px; text-align: left; }
        .results-table th { background: var(--uui-color-surface-emphasis); }
    `;

    @state() _indexes = [];
    @state() _selectedIndex = "";
    @state() _query = "";
    @state() _pageSize = 20;
    @state() _results = null;
    @state() _rebuildIndex = "";
    @state() _rebuildMsg = "";
    @state() _loading = true;

    connectedCallback() {
        super.connectedCallback();
        this._loadIndexes();
    }

    async _loadIndexes() {
        const resp = await fetch("/umbraco/api/examineextensions/GetIndexes");
        this._indexes = await resp.json();
        if (this._indexes.length) {
            this._selectedIndex = this._indexes[0];
            this._rebuildIndex = this._indexes[0];
        }
        this._loading = false;
    }

    async _search() {
        const resp = await fetch("/umbraco/api/examineextensions/Search", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                query: this._query,
                indexName: this._selectedIndex,
                page: 1,
                pageSize: this._pageSize
            })
        });
        this._results = await resp.json();
    }

    async _rebuild() {
        const resp = await fetch("/umbraco/api/examineextensions/RebuildIndex", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(this._rebuildIndex)
        });
        const data = await resp.json();
        this._rebuildMsg = data.message ?? "Done";
    }

    render() {
        if (this._loading) return html`<uui-loader></uui-loader>`;
        return html`
            <uui-box headline="Examine Extensions">
                <div style="display:grid;grid-template-columns:1fr 1fr;gap:24px;">
                    <div>
                        <h3>Search</h3>
                        <uui-form-layout-item>
                            <uui-label slot="label">Query</uui-label>
                            <uui-input .value=${this._query} @input=${e => this._query = e.target.value} placeholder="Search query..."></uui-input>
                        </uui-form-layout-item>
                        <uui-form-layout-item>
                            <uui-label slot="label">Index</uui-label>
                            <uui-select
                                .options=${this._indexes.map(i => ({ name: i, value: i }))}
                                .value=${this._selectedIndex}
                                @change=${e => this._selectedIndex = e.target.value}>
                            </uui-select>
                        </uui-form-layout-item>
                        <uui-form-layout-item>
                            <uui-label slot="label">Page Size</uui-label>
                            <uui-input type="number" .value=${this._pageSize} @input=${e => this._pageSize = parseInt(e.target.value)}></uui-input>
                        </uui-form-layout-item>
                        <uui-button look="primary" label="Search" @click=${this._search}>Search</uui-button>
                    </div>
                    <div>
                        <h3>Index Management</h3>
                        <uui-form-layout-item>
                            <uui-label slot="label">Index</uui-label>
                            <uui-select
                                .options=${this._indexes.map(i => ({ name: i, value: i }))}
                                .value=${this._rebuildIndex}
                                @change=${e => this._rebuildIndex = e.target.value}>
                            </uui-select>
                        </uui-form-layout-item>
                        <uui-button look="warning" label="Rebuild" @click=${this._rebuild}>Rebuild Index</uui-button>
                        ${this._rebuildMsg ? html`<uui-badge color="positive">${this._rebuildMsg}</uui-badge>` : ""}
                    </div>
                </div>
                ${this._results ? html`
                    <hr />
                    <h3>Results <small>(${this._results.totalItems} total)</small></h3>
                    <table class="results-table">
                        <thead><tr><th>ID</th><th>Score</th><th>Fields</th></tr></thead>
                        <tbody>
                            ${this._results.items.length === 0
                                ? html`<tr><td colspan="3">No results found.</td></tr>`
                                : this._results.items.map(item => html`
                                    <tr>
                                        <td>${item.id}</td>
                                        <td>${item.score.toFixed(4)}</td>
                                        <td>${Object.entries(item.fields).map(([k, v]) => html`<strong>${k}</strong>: ${v}<br />`)}</td>
                                    </tr>`)}
                        </tbody>
                    </table>` : ""}
            </uui-box>`;
    }
}

export default ExamineExtensionsDashboard;
