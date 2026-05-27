// Yaml2Schema Dashboard — Lit element for Umbraco 14–17
// Informational dashboard: shows plugin status, config path, and supported entity types.

import { LitElement, html, css, nothing } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';

const API_BASE = '/umbraco/api/Yaml2Schema';

const ENTITY_TYPES = [
    'Languages',
    'Data Types',
    'Document Types',
    'Media Types',
    'Templates',
    'Content',
    'Media',
    'Dictionary Items',
    'Members',
    'Member Types',
    'Member Groups',
    'Users',
    'NuGet Packages',
    'Property Editors',
    'Static Assets',
];

class Yaml2SchemaDashboard extends UmbElementMixin(LitElement) {

    static properties = {
        _status:      { state: true },
        _loadingStatus: { state: true },
    };

    static styles = css`
        :host {
            display: block;
            padding: var(--uui-size-layout-1, 24px);
        }

        .header {
            margin-bottom: var(--uui-size-layout-2, 32px);
        }

        .header h1 {
            font-size: var(--uui-type-h3-size, 1.5rem);
            font-weight: 600;
            margin: 0 0 var(--uui-size-3, 8px) 0;
            color: var(--uui-color-text, #1b264f);
        }

        .header p {
            margin: 0;
            color: var(--uui-color-text-alt, #666);
        }

        .section {
            margin-bottom: var(--uui-size-layout-2, 32px);
        }

        .section h2 {
            font-size: var(--uui-type-h5-size, 1rem);
            font-weight: 600;
            margin: 0 0 var(--uui-size-3, 8px) 0;
            color: var(--uui-color-text, #1b264f);
            border-bottom: 1px solid var(--uui-color-border, #e3e3e3);
            padding-bottom: var(--uui-size-2, 6px);
        }

        .info-card {
            background: var(--uui-color-surface, #fff);
            border: 1px solid var(--uui-color-border, #e3e3e3);
            border-radius: var(--uui-border-radius, 4px);
            padding: var(--uui-size-5, 16px);
            margin-bottom: var(--uui-size-3, 8px);
        }

        .info-row {
            display: flex;
            align-items: baseline;
            gap: var(--uui-size-3, 8px);
            padding: 6px 0;
            border-bottom: 1px solid var(--uui-color-border, #f0f0f0);
            font-size: 14px;
        }

        .info-row:last-child {
            border-bottom: none;
        }

        .info-label {
            font-weight: 500;
            color: var(--uui-color-text, #1b264f);
            min-width: 160px;
        }

        .info-value {
            color: var(--uui-color-text-alt, #555);
            font-family: 'Courier New', Consolas, monospace;
            font-size: 13px;
        }

        .entity-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
            gap: var(--uui-size-3, 8px);
        }

        .entity-chip {
            display: flex;
            align-items: center;
            gap: var(--uui-size-2, 6px);
            background: var(--uui-color-surface, #fff);
            border: 1px solid var(--uui-color-border, #e3e3e3);
            border-radius: var(--uui-border-radius, 4px);
            padding: 10px var(--uui-size-4, 12px);
            font-size: 13px;
            color: var(--uui-color-text, #1b264f);
        }

        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 500;
        }

        .status-badge.ok {
            background: #e8f5e9;
            color: #2e7d32;
        }

        .status-badge.warn {
            background: #fff8e1;
            color: #f57f17;
        }

        .status-badge.idle {
            background: var(--uui-color-surface-alt, #f5f5f5);
            color: var(--uui-color-text-alt, #666);
        }

        .howto {
            background: var(--uui-color-surface-alt, #f5f5f5);
            border: 1px solid var(--uui-color-border, #e3e3e3);
            border-radius: var(--uui-border-radius, 4px);
            padding: var(--uui-size-5, 16px);
            font-size: 13px;
            line-height: 1.7;
            color: var(--uui-color-text-alt, #555);
        }

        .howto code {
            background: rgba(0,0,0,.06);
            border-radius: 3px;
            padding: 1px 5px;
            font-family: 'Courier New', Consolas, monospace;
        }
    `;

    constructor() {
        super();
        this._status = null;
        this._loadingStatus = false;
        this._authContext = null;
    }

    connectedCallback() {
        super.connectedCallback();
        this.consumeContext(UMB_AUTH_CONTEXT, (ctx) => {
            this._authContext = ctx;
            this._loadStatus();
        });
    }

    async _getToken() {
        if (!this._authContext) return null;
        // getLatestToken() is deprecated in Umbraco 17 but functional until v19.
        // getOpenApiConfiguration() configures the OpenAPI SDK client and does not
        // expose a raw Bearer token for use in custom fetch() calls.
        return this._authContext.getLatestToken?.() ?? null;
    }

    async _fetchAuthenticated(path, options = {}) {
        const headers = { 'Content-Type': 'application/json', ...(options.headers ?? {}) };
        const token = await this._getToken();
        if (token) headers['Authorization'] = `Bearer ${token}`;
        return fetch(`${API_BASE}${path}`, { ...options, headers });
    }

    async _loadStatus() {
        this._loadingStatus = true;
        try {
            const res = await this._fetchAuthenticated('/Status');
            if (res.ok) this._status = await res.json();
        } catch {
            // Status endpoint may not exist yet — that's fine
        } finally {
            this._loadingStatus = false;
        }
    }

    _renderStatus() {
        if (this._loadingStatus) {
            return html`<uui-loader-circle></uui-loader-circle>`;
        }

        if (!this._status) {
            return html`
                <div class="info-card">
                    <div class="info-row">
                        <span class="info-label">Config file</span>
                        <span class="info-value">config/umbraco.yaml</span>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Import runs</span>
                        <span class="info-value">On every application startup when the YAML file is present</span>
                    </div>
                    <div class="info-row">
                        <span class="info-label">File processed</span>
                        <span class="info-value">Renamed to <code>*.done</code> after a successful import</span>
                    </div>
                </div>`;
        }

        const badge = this._status.lastImportSucceeded
            ? html`<span class="status-badge ok">✓ Last import succeeded</span>`
            : html`<span class="status-badge warn">⚠ Last import had errors</span>`;

        return html`
            <div class="info-card">
                <div class="info-row">
                    <span class="info-label">Status</span>
                    <span>${badge}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Config file</span>
                    <span class="info-value">${this._status.configPath ?? 'config/umbraco.yaml'}</span>
                </div>
                ${this._status.lastImportDate ? html`
                    <div class="info-row">
                        <span class="info-label">Last import</span>
                        <span class="info-value">${new Date(this._status.lastImportDate).toLocaleString()}</span>
                    </div>` : nothing}
                ${this._status.processedFile ? html`
                    <div class="info-row">
                        <span class="info-label">Processed file</span>
                        <span class="info-value">${this._status.processedFile}</span>
                    </div>` : nothing}
            </div>`;
    }

    render() {
        return html`
            <div class="header">
                <h1>YAML Import</h1>
                <p>Declarative Infrastructure-as-Code bootstrapping for Umbraco — define your entire site structure in a YAML file.</p>
            </div>

            <div class="section">
                <h2>Status</h2>
                ${this._renderStatus()}
            </div>

            <div class="section">
                <h2>Supported Entity Types</h2>
                <div class="entity-grid">
                    ${ENTITY_TYPES.map(label => html`
                        <div class="entity-chip">${label}</div>`)}
                </div>
            </div>

            <div class="section">
                <h2>How it works</h2>
                <div class="howto">
                    <p>
                        Place a <code>umbraco.yaml</code> file at the path configured in
                        <code>appsettings.json</code> under <code>UmbracoYaml:ConfigPath</code>
                        (default: <code>config/umbraco.yaml</code>).
                    </p>
                    <p>
                        On the next application startup, Yaml2Schema reads the file and creates or updates
                        all declared entities. After a successful import the file is renamed to
                        <code>umbraco.yaml.done</code> so it is not re-processed.
                    </p>
                    <p>
                        Use <code>[UPDATE]</code> prefixes on entity names to force updates on existing items,
                        and <code>[REMOVE]</code> to delete them.
                    </p>
                </div>
            </div>
        `;
    }
}

customElements.define('yaml2schema-dashboard', Yaml2SchemaDashboard);

export default Yaml2SchemaDashboard;
