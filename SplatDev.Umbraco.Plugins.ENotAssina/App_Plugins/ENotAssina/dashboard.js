import { LitElement, html, css }
    from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';

const API_BASE = '/umbraco/api/enotassina';

class ENotAssinaDashboard extends UmbElementMixin(LitElement) {
    #notificationContext;
    #authContext;

    static properties = {
        _loading: { state: true },
        _error: { state: true },
        _documents: { state: true },
        _search: { state: true },
        _statusFilter: { state: true },
        _confirmDialog: { state: true },
    };

    constructor() {
        super();
        this._loading = false;
        this._error = null;
        this._documents = [];
        this._search = '';
        this._statusFilter = '';
        this._confirmDialog = null;

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (ctx) => {
            this.#notificationContext = ctx;
        });
        this.consumeContext(UMB_AUTH_CONTEXT, (ctx) => {
            this.#authContext = ctx;
        });
    }

    async #fetch(url, options = {}) {
        const token = await this.#authContext?.getLatestToken();
        const headers = {
            'Content-Type': 'application/json',
            ...(options.headers ?? {}),
        };
        if (token) headers['Authorization'] = `Bearer ${token}`;
        return fetch(url, { credentials: 'include', ...options, headers });
    }

    connectedCallback() {
        super.connectedCallback();
        this.#loadDocuments();
    }

    async #loadDocuments() {
        this._loading = true;
        this._error = null;
        try {
            const res = await this.#fetch(`${API_BASE}/documents`);
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            const data = await res.json();
            this._documents = data.documents ?? [];
        } catch (e) {
            this._error = e.message ?? 'Erro desconhecido';
            this.#notificationContext?.peek('danger', { data: { headline: 'Erro ao carregar documentos', message: this._error } });
        } finally {
            this._loading = false;
        }
    }

    async #checkStatus(docId) {
        this.#notificationContext?.peek('positive', { data: { headline: 'Verificando status...', message: 'Consultando e-Not Assina.' } });
        try {
            const res = await this.#fetch(`${API_BASE}/checkstatus`, {
                method: 'POST',
                body: JSON.stringify({ docId }),
            });
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            const data = await res.json();
            if (data.updated) {
                this.#notificationContext?.peek('positive', { data: { headline: 'Status atualizado', message: `Novo status: ${this.#statusLabel(data.status)}` } });
                await this.#loadDocuments();
            } else {
                this.#notificationContext?.peek('default', { data: { headline: 'Status atual', message: this.#statusLabel(data.status) } });
            }
        } catch (e) {
            this.#notificationContext?.peek('danger', { data: { headline: 'Erro ao verificar status', message: e.message } });
        }
    }

    #confirmCancel(docId, razaoSocial) {
        this._confirmDialog = { type: 'cancel', docId, razaoSocial };
    }

    async #executeCancel() {
        const { docId } = this._confirmDialog;
        this._confirmDialog = null;
        this.#notificationContext?.peek('default', { data: { headline: 'Cancelando...', message: 'Aguarde.' } });
        try {
            const res = await this.#fetch(`${API_BASE}/cancel`, {
                method: 'POST',
                body: JSON.stringify({ docId }),
            });
            if (!res.ok) {
                const err = await res.json().catch(() => ({}));
                throw new Error(err.message ?? `HTTP ${res.status}`);
            }
            this.#notificationContext?.peek('positive', { data: { headline: 'Cancelado', message: 'O fluxo de assinatura foi cancelado.' } });
            await this.#loadDocuments();
        } catch (e) {
            this.#notificationContext?.peek('danger', { data: { headline: 'Erro ao cancelar', message: e.message } });
        }
    }

    #statusLabel(status) {
        const map = { aguardando_assinatura: 'Aguardando Assinatura', assinado: 'Assinado', cancelado: 'Cancelado' };
        return map[status] ?? status;
    }

    get #filteredDocs() {
        const q = this._search.toLowerCase();
        return this._documents.filter(d => {
            const matchSearch = !q
                || (d.cartorio_doc_id ?? '').toLowerCase().includes(q)
                || (d.razao_social ?? '').toLowerCase().includes(q)
                || (d.cnpj ?? '').toLowerCase().includes(q)
                || (d.nome_assinante ?? '').toLowerCase().includes(q)
                || (d.cpf_assinante ?? '').toLowerCase().includes(q)
                || (d.email_assinante ?? '').toLowerCase().includes(q);
            const matchStatus = !this._statusFilter || d.status === this._statusFilter;
            return matchSearch && matchStatus;
        });
    }

    get #stats() {
        return {
            total: this._documents.length,
            aguardando: this._documents.filter(d => d.status === 'aguardando_assinatura').length,
            assinados: this._documents.filter(d => d.status === 'assinado').length,
            cancelados: this._documents.filter(d => d.status === 'cancelado').length,
        };
    }

    #fmtDate(val) {
        if (!val) return '—';
        return new Date(val).toLocaleString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
    }

    #renderConfirm() {
        if (!this._confirmDialog) return html``;
        const { razaoSocial } = this._confirmDialog;
        return html`
        <div class="confirm-overlay">
            <div class="confirm-dialog">
                <h3>Cancelar Fluxo de Assinatura</h3>
                <p>Tem certeza que deseja cancelar o fluxo de assinatura de <strong>${razaoSocial}</strong>?</p>
                <p class="confirm-warning">Esta ação não pode ser desfeita. O signatário não poderá mais assinar este documento.</p>
                <div class="confirm-actions">
                    <uui-button look="secondary" @click=${() => this._confirmDialog = null}>Não</uui-button>
                    <uui-button look="primary" color="danger" @click=${this.#executeCancel.bind(this)}>Sim, cancelar</uui-button>
                </div>
            </div>
        </div>`;
    }

    #renderStatusBadge(doc) {
        const colorMap = { aguardando_assinatura: 'warning', assinado: 'positive', cancelado: 'danger' };
        const color = colorMap[doc.status] ?? 'default';
        return html`
            <uui-badge color=${color}>${this.#statusLabel(doc.status)}</uui-badge>
            ${doc.certificado_ativo === true ? html`<br><small style="color:var(--uui-color-positive-standalone)">✓ Certificado Ativo</small>` : ''}
            ${doc.certificado_ativo === false ? html`<br><small style="color:var(--uui-color-warning-standalone)">⚠ Certificado Inativo</small>` : ''}
        `;
    }

    render() {
        const stats = this.#stats;
        const docs = this.#filteredDocs;

        return html`
        ${this.#renderConfirm()}
        <uui-box headline="e-Not Assina – Gerenciador de Assinaturas Eletrônicas Notarizadas">
            <!-- Stats -->
            <div class="stats-row">
                <div class="stat-card"><div class="stat-label">Total</div><div class="stat-value">${stats.total}</div></div>
                <div class="stat-card warning"><div class="stat-label">Aguardando</div><div class="stat-value">${stats.aguardando}</div></div>
                <div class="stat-card positive"><div class="stat-label">Assinados</div><div class="stat-value">${stats.assinados}</div></div>
                <div class="stat-card danger"><div class="stat-label">Cancelados</div><div class="stat-value">${stats.cancelados}</div></div>
            </div>

            <!-- Filters -->
            <div class="filters-row">
                <uui-input
                    placeholder="Buscar por ID, razão social, CPF ou e-mail..."
                    .value=${this._search}
                    @input=${e => this._search = e.target.value}
                    style="flex:1;">
                </uui-input>
                <select class="umb-select" @change=${e => this._statusFilter = e.target.value}>
                    <option value="">Todos os status</option>
                    <option value="aguardando_assinatura">Aguardando Assinatura</option>
                    <option value="assinado">Assinado</option>
                    <option value="cancelado">Cancelado</option>
                </select>
                <uui-button look="secondary" @click=${this.#loadDocuments.bind(this)} ?disabled=${this._loading}>
                    <uui-icon name="icon-refresh"></uui-icon> Atualizar
                </uui-button>
            </div>

            ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : ''}
            ${this._error ? html`<uui-tag color="danger">Erro: ${this._error}</uui-tag>` : ''}

            <!-- Table -->
            <uui-table>
                <uui-table-head>
                    <uui-table-head-cell>Loc. ID</uui-table-head-cell>
                    <uui-table-head-cell>Doc ID</uui-table-head-cell>
                    <uui-table-head-cell>Razão Social / CNPJ</uui-table-head-cell>
                    <uui-table-head-cell>Signatário</uui-table-head-cell>
                    <uui-table-head-cell>Status</uui-table-head-cell>
                    <uui-table-head-cell>Cartório</uui-table-head-cell>
                    <uui-table-head-cell>Criado Em</uui-table-head-cell>
                    <uui-table-head-cell>Assinado Em</uui-table-head-cell>
                    <uui-table-head-cell>Ações</uui-table-head-cell>
                </uui-table-head>
                ${docs.length === 0 && !this._loading ? html`
                    <uui-table-row>
                        <uui-table-cell colspan="9" style="text-align:center;padding:40px;color:var(--uui-color-text-alt)">
                            Nenhum documento e-Not Assina encontrado.
                        </uui-table-cell>
                    </uui-table-row>` : ''}
                ${docs.map(doc => html`
                    <uui-table-row>
                        <uui-table-cell>${doc.locacao_id}</uui-table-cell>
                        <uui-table-cell><code style="font-size:11px">${(doc.cartorio_doc_id ?? '').substring(0, 14)}…</code></uui-table-cell>
                        <uui-table-cell>
                            <strong>${doc.razao_social}</strong><br>
                            <small style="color:var(--uui-color-text-alt)">${doc.cnpj}</small>
                        </uui-table-cell>
                        <uui-table-cell>
                            <div>${doc.nome_assinante}</div>
                            <small style="color:var(--uui-color-text-alt)">CPF: ${doc.cpf_assinante}<br>${doc.email_assinante}</small>
                        </uui-table-cell>
                        <uui-table-cell>${this.#renderStatusBadge(doc)}</uui-table-cell>
                        <uui-table-cell><small>${doc.cartorio_emissor || '—'}</small></uui-table-cell>
                        <uui-table-cell>${this.#fmtDate(doc.criado_em)}</uui-table-cell>
                        <uui-table-cell>${this.#fmtDate(doc.assinado_em)}</uui-table-cell>
                        <uui-table-cell>
                            <div class="actions-cell">
                                ${doc.cartorio_link ? html`
                                <uui-button
                                    look="secondary"
                                    href="${doc.cartorio_link}"
                                    target="_blank"
                                    label="Abrir no e-Not Assina"
                                    title="Abrir no e-Not Assina">
                                    <uui-icon name="icon-link"></uui-icon>
                                </uui-button>` : ''}
                                ${doc.pdf_blob_url ? html`
                                <uui-button
                                    look="secondary"
                                    color="positive"
                                    @click=${() => window.open(doc.pdf_blob_url, '_blank')}
                                    label="Baixar PDF"
                                    title="Baixar PDF assinado">
                                    <uui-icon name="icon-download"></uui-icon>
                                </uui-button>` : ''}
                                ${doc.status === 'aguardando_assinatura' ? html`
                                <uui-button
                                    look="secondary"
                                    @click=${() => this.#checkStatus(doc.cartorio_doc_id)}
                                    label="Verificar status"
                                    title="Verificar status">
                                    <uui-icon name="icon-refresh"></uui-icon>
                                </uui-button>
                                <uui-button
                                    look="secondary"
                                    color="danger"
                                    @click=${() => this.#confirmCancel(doc.cartorio_doc_id, doc.razao_social)}
                                    label="Cancelar fluxo"
                                    title="Cancelar fluxo de assinatura">
                                    <uui-icon name="icon-delete"></uui-icon>
                                </uui-button>` : ''}
                            </div>
                        </uui-table-cell>
                    </uui-table-row>`)}
            </uui-table>
            <div style="margin-top:8px;color:var(--uui-color-text-alt);font-size:12px">
                Mostrando ${docs.length} de ${this._documents.length} documento(s)
            </div>
        </uui-box>`;
    }

    static styles = css`
        :host { display: block; padding: var(--uui-size-layout-1); }

        .stats-row {
            display: flex; gap: 16px; margin-bottom: 20px; flex-wrap: wrap;
        }
        .stat-card {
            flex: 1; min-width: 120px; padding: 16px; border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius); text-align: center; background: var(--uui-color-surface);
        }
        .stat-label { font-size: 12px; color: var(--uui-color-text-alt); margin-bottom: 4px; }
        .stat-value { font-size: 28px; font-weight: bold; color: var(--uui-color-text); }
        .stat-card.warning .stat-value { color: var(--uui-color-warning-standalone); }
        .stat-card.positive .stat-value { color: var(--uui-color-positive-standalone); }
        .stat-card.danger .stat-value { color: var(--uui-color-danger-standalone); }

        .filters-row {
            display: flex; gap: 8px; margin-bottom: 16px; align-items: center; flex-wrap: wrap;
        }
        .umb-select {
            padding: var(--uui-size-2) var(--uui-size-3);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface);
            color: var(--uui-color-text);
            font-size: 14px;
        }

        .actions-cell { display: flex; gap: 4px; flex-wrap: wrap; align-items: center; }

        .confirm-overlay {
            position: fixed; inset: 0; background: rgba(0,0,0,0.5);
            display: flex; align-items: center; justify-content: center; z-index: 9999;
        }
        .confirm-dialog {
            background: var(--uui-color-surface); padding: 32px; border-radius: var(--uui-border-radius);
            max-width: 480px; width: 90%; box-shadow: var(--uui-shadow-depth-3);
        }
        .confirm-dialog h3 { margin: 0 0 12px; }
        .confirm-warning { color: var(--uui-color-danger-standalone); font-size: 13px; }
        .confirm-actions { display: flex; gap: 8px; justify-content: flex-end; margin-top: 20px; }
    `;
}

customElements.define('enotassina-dashboard', ENotAssinaDashboard);
export default ENotAssinaDashboard;

