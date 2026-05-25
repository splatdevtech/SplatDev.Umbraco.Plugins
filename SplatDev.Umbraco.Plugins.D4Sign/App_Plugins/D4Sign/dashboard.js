// D4Sign Dashboard — Lit 3 element for Umbraco 14–17
// Registered as a custom element; referenced from umbraco-package.json.
// Uses UMB_AUTH_CONTEXT for authenticated API calls.

import { LitElement, html, css, nothing } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

const API_BASE = '/umbraco/api/d4sign';

class D4SignDashboard extends UmbElementMixin(LitElement) {

    static properties = {
        _loading:       { state: true },
        _checkingUuid:  { state: true },
        _documents:     { state: true },
        _search:        { state: true },
        _statusFilter:  { state: true },
        _stats:         { state: true },
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

        .toolbar {
            display: flex;
            gap: var(--uui-size-3, 8px);
            flex-wrap: wrap;
            align-items: center;
            margin-bottom: var(--uui-size-layout-2, 32px);
        }

        .toolbar uui-input {
            min-width: 220px;
            flex: 1;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
            gap: var(--uui-size-3, 8px);
            margin-bottom: var(--uui-size-layout-2, 32px);
        }

        .stat-card {
            background: var(--uui-color-surface, #fff);
            border: 1px solid var(--uui-color-border, #e3e3e3);
            border-radius: var(--uui-border-radius, 4px);
            padding: var(--uui-size-5, 16px);
            text-align: center;
            cursor: pointer;
            transition: border-color 0.15s;
        }

        .stat-card:hover,
        .stat-card.active {
            border-color: var(--uui-color-interactive, #1b264f);
        }

        .stat-label {
            font-size: 11px;
            color: var(--uui-color-text-alt, #888);
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: var(--uui-size-2, 6px);
        }

        .stat-value {
            font-size: 28px;
            font-weight: 600;
            color: var(--uui-color-interactive, #1b264f);
            line-height: 1;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            background: var(--uui-color-surface, #fff);
            border: 1px solid var(--uui-color-border, #e3e3e3);
            border-radius: var(--uui-border-radius, 4px);
            overflow: hidden;
        }

        th {
            background: var(--uui-color-surface-alt, #f5f5f5);
            text-align: left;
            padding: var(--uui-size-3, 10px) var(--uui-size-5, 16px);
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            color: var(--uui-color-text-alt, #888);
            border-bottom: 1px solid var(--uui-color-border, #e3e3e3);
        }

        td {
            padding: var(--uui-size-3, 10px) var(--uui-size-5, 16px);
            font-size: 13px;
            color: var(--uui-color-text, #333);
            border-bottom: 1px solid var(--uui-color-border, #e3e3e3);
            vertical-align: middle;
        }

        tr:last-child td { border-bottom: none; }

        tr:hover td { background: var(--uui-color-surface-alt, #f9f9f9); }

        .badge {
            display: inline-block;
            padding: 2px 10px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 600;
            letter-spacing: 0.03em;
        }

        .badge-aguardando {
            background: #fff3cd;
            color: #856404;
        }

        .badge-assinado {
            background: #d1e7dd;
            color: #0a5e35;
        }

        .badge-cancelado {
            background: #f8d7da;
            color: #721c24;
        }

        .badge-unknown {
            background: #e2e3e5;
            color: #383d41;
        }

        .row-actions {
            display: flex;
            gap: var(--uui-size-2, 6px);
            flex-wrap: wrap;
        }

        .empty-state {
            text-align: center;
            padding: var(--uui-size-layout-2, 48px);
            color: var(--uui-color-text-alt, #888);
        }

        .error-box {
            background: #f8d7da;
            border: 1px solid #f5c2c7;
            border-radius: var(--uui-border-radius, 4px);
            padding: var(--uui-size-5, 16px);
            color: #721c24;
            margin-bottom: var(--uui-size-layout-2, 32px);
        }
    `;

    constructor() {
        super();
        this._loading       = false;
        this._checkingUuid  = null;
        this._documents     = [];
        this._search        = '';
        this._statusFilter  = '';
        this._stats         = { total: 0, aguardando: 0, assinados: 0, cancelados: 0 };
        this._authContext   = null;
        this._notifContext  = null;
        this._error         = null;
    }

    connectedCallback() {
        super.connectedCallback();

        this.consumeContext(UMB_AUTH_CONTEXT, (ctx) => {
            this._authContext = ctx;
            this._loadDocuments();
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (ctx) => {
            this._notifContext = ctx;
        });
    }

    // ── Auth helper ───────────────────────────────────────────────────────────

    async _fetch(path, options = {}) {
        const headers = { 'Content-Type': 'application/json', ...(options.headers ?? {}) };

        if (this._authContext) {
            const token = await this._authContext.getLatestToken();
            if (token) headers['Authorization'] = `Bearer ${token}`;
        }

        return fetch(`${API_BASE}${path}`, { ...options, headers });
    }

    // ── Data loading ──────────────────────────────────────────────────────────

    async _loadDocuments() {
        this._loading = true;
        this._error   = null;

        try {
            const res = await this._fetch('/Documents');

            if (!res.ok) {
                const err = await res.json().catch(() => ({ message: res.statusText }));
                throw new Error(err.message ?? res.statusText);
            }

            const data = await res.json();
            this._documents = data.documents ?? [];
            this._recalcStats();
        } catch (e) {
            this._error = e.message ?? 'Erro desconhecido ao carregar documentos.';
            this._notify('danger', 'Erro', this._error);
        } finally {
            this._loading = false;
        }
    }

    _recalcStats() {
        const docs = this._documents;
        this._stats = {
            total:      docs.length,
            aguardando: docs.filter(d => d.status === 'aguardando_assinatura').length,
            assinados:  docs.filter(d => d.status === 'assinado').length,
            cancelados: docs.filter(d => d.status === 'cancelado').length,
        };
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    async _checkStatus(docUuid, locacaoId) {
        if (this._checkingUuid) return;
        this._checkingUuid = docUuid;
        this._notify('default', 'Verificando', 'Consultando status no D4Sign…');

        try {
            const res = await this._fetch('/CheckStatus', {
                method: 'POST',
                body: JSON.stringify({ docUuid, locacaoId }),
            });

            if (!res.ok) {
                const err = await res.json().catch(() => ({ message: res.statusText }));
                throw new Error(err.message ?? res.statusText);
            }

            const data = await res.json();
            if (data.updated) {
                this._notify('positive', 'Atualizado',
                    `Status atualizado para: ${this._statusLabel(data.status)}`);
                await this._loadDocuments();
            } else {
                this._notify('default', 'Status',
                    `Status atual: ${this._statusLabel(data.status)}`);
            }
        } catch (e) {
            this._notify('danger', 'Erro', `Não foi possível verificar o status: ${e.message ?? ''}`);
        } finally {
            this._checkingUuid = null;
        }
    }

    _downloadPdf(blobUrl) {
        if (!blobUrl) {
            this._notify('warning', 'Indisponível', 'O PDF ainda não está disponível para download.');
            return;
        }
        window.open(blobUrl, '_blank', 'noopener,noreferrer');
        this._notify('positive', 'Download', 'Abrindo PDF assinado…');
    }

    _setStatusFilter(filter) {
        this._statusFilter = this._statusFilter === filter ? '' : filter;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    _statusLabel(status) {
        const map = {
            aguardando_assinatura: 'Aguardando Assinatura',
            assinado: 'Assinado',
            cancelado: 'Cancelado',
        };
        return map[status] ?? status;
    }

    _statusBadgeClass(status) {
        const map = {
            aguardando_assinatura: 'badge badge-aguardando',
            assinado: 'badge badge-assinado',
            cancelado: 'badge badge-cancelado',
        };
        return map[status] ?? 'badge badge-unknown';
    }

    _notify(color, headline, message) {
        this._notifContext?.peek(color, { data: { headline, message } });
    }

    get _filteredDocuments() {
        const search = this._search.toLowerCase();
        return this._documents.filter(d => {
            if (this._statusFilter && d.status !== this._statusFilter) return false;
            if (!search) return true;
            return (
                (d.razaoSocial ?? '').toLowerCase().includes(search) ||
                (d.cnpj        ?? '').toLowerCase().includes(search) ||
                (d.docUuid     ?? '').toLowerCase().includes(search)
            );
        });
    }

    // ── Render ────────────────────────────────────────────────────────────────

    _renderStats() {
        const s = this._stats;
        return html`
            <div class="stats-grid">
                ${this._statCard('Total', s.total, '')}
                ${this._statCard('Aguardando', s.aguardando, 'aguardando_assinatura')}
                ${this._statCard('Assinados', s.assinados,   'assinado')}
                ${this._statCard('Cancelados', s.cancelados, 'cancelado')}
            </div>`;
    }

    _statCard(label, value, filterKey) {
        const active = this._statusFilter === filterKey && filterKey !== '';
        return html`
            <div class="stat-card ${active ? 'active' : ''}"
                 @click=${() => filterKey ? this._setStatusFilter(filterKey) : null}>
                <div class="stat-label">${label}</div>
                <div class="stat-value">${value}</div>
            </div>`;
    }

    _renderTable() {
        const rows = this._filteredDocuments;

        if (!rows.length) {
            return html`
                <div class="empty-state">
                    ${this._search || this._statusFilter
                        ? 'Nenhum documento encontrado com os filtros aplicados.'
                        : 'Nenhum documento D4Sign encontrado.'}
                </div>`;
        }

        return html`
            <table>
                <thead>
                    <tr>
                        <th>Empresa</th>
                        <th>CNPJ</th>
                        <th>Regional / UF</th>
                        <th>Status</th>
                        <th>Criado em</th>
                        <th>Assinado em</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    ${rows.map(d => this._renderRow(d))}
                </tbody>
            </table>`;
    }

    _renderRow(d) {
        const isChecking = this._checkingUuid === d.docUuid;
        return html`
            <tr>
                <td>${d.razaoSocial ?? '—'}</td>
                <td>${d.cnpj ?? '—'}</td>
                <td>${d.regionalCodigo ?? ''}${d.uf ? ` / ${d.uf}` : ''}</td>
                <td>
                    <span class=${this._statusBadgeClass(d.status)}>
                        ${this._statusLabel(d.status)}
                    </span>
                </td>
                <td>${d.criadoEm ? new Date(d.criadoEm).toLocaleDateString('pt-BR') : '—'}</td>
                <td>${d.assinadoEm ? new Date(d.assinadoEm).toLocaleDateString('pt-BR') : '—'}</td>
                <td>
                    <div class="row-actions">
                        <uui-button
                            look="secondary"
                            compact
                            label=${isChecking ? 'Verificando…' : 'Verificar'}
                            ?disabled=${!!this._checkingUuid}
                            @click=${() => this._checkStatus(d.docUuid, d.locacaoId)}>
                            ${isChecking ? html`<uui-loader-circle></uui-loader-circle>` : nothing}
                            ${isChecking ? 'Verificando…' : 'Verificar'}
                        </uui-button>

                        ${d.status === 'assinado' ? html`
                            <uui-button
                                look="secondary"
                                compact
                                label="Baixar PDF"
                                @click=${() => this._downloadPdf(d.pdfBlobUrl)}>
                                Baixar PDF
                            </uui-button>` : nothing}
                    </div>
                </td>
            </tr>`;
    }

    render() {
        return html`
            <div class="header">
                <h1>D4Sign — Assinaturas Digitais</h1>
                <p>Gerencie documentos enviados para assinatura digital via D4Sign.</p>
            </div>

            ${this._error
                ? html`<div class="error-box">${this._error}</div>`
                : nothing}

            ${this._renderStats()}

            <div class="toolbar">
                <uui-input
                    placeholder="Buscar por empresa, CNPJ ou UUID…"
                    .value=${this._search}
                    @input=${e => { this._search = e.target.value; }}>
                </uui-input>

                <uui-button
                    look="secondary"
                    label="Atualizar"
                    ?disabled=${this._loading}
                    @click=${this._loadDocuments}>
                    ${this._loading ? html`<uui-loader-circle></uui-loader-circle>` : nothing}
                    Atualizar
                </uui-button>
            </div>

            ${this._loading && !this._documents.length
                ? html`<uui-loader-bar></uui-loader-bar>`
                : this._renderTable()}
        `;
    }
}

customElements.define('d4sign-dashboard', D4SignDashboard);
export default D4SignDashboard;
