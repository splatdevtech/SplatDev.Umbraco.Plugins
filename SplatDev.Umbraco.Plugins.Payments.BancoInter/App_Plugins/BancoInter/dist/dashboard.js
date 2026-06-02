import { LitElement as u, html as r, css as h, state as i, customElement as b } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as f } from "@umbraco-cms/backoffice/element-api";
var x = Object.defineProperty, m = Object.getOwnPropertyDescriptor, o = (t, a, l, n) => {
  for (var s = n > 1 ? void 0 : n ? m(a, l) : a, d = t.length - 1, c; d >= 0; d--)
    (c = t[d]) && (s = (n ? c(a, l, s) : c(s)) || s);
  return n && s && x(a, l, s), s;
};
const p = "/umbraco/api/bancointersandbox";
let e = class extends f(u) {
  constructor() {
    super(...arguments), this._tab = "overview", this._connStatus = "unknown", this._balance = null, this._balanceLoading = !1, this._balanceError = "", this._stmtStart = this._isoDate(-30), this._stmtEnd = this._isoDate(0), this._statement = [], this._stmtLoading = !1, this._stmtError = "", this._pixAmount = "10.00", this._pixKey = "", this._pixDesc = "Test charge", this._pixLoading = !1, this._pixResult = "", this._whPixKey = "", this._whUrl = "", this._whLoading = !1, this._whResult = "";
  }
  connectedCallback() {
    super.connectedCallback(), this._checkConnection();
  }
  _isoDate(t) {
    const a = /* @__PURE__ */ new Date();
    return a.setDate(a.getDate() + t), a.toISOString().slice(0, 10);
  }
  async _checkConnection() {
    this._connStatus = "checking", this._balanceLoading = !0, this._balanceError = "";
    try {
      const t = await fetch(`${p}/GetBalance`);
      t.ok ? (this._balance = await t.json(), this._connStatus = "connected") : (this._connStatus = "error", this._balanceError = `HTTP ${t.status}: ${t.statusText}`);
    } catch (t) {
      this._connStatus = "error", this._balanceError = t instanceof Error ? t.message : String(t);
    } finally {
      this._balanceLoading = !1;
    }
  }
  async _loadStatement() {
    this._stmtLoading = !0, this._stmtError = "";
    try {
      const t = await fetch(
        `${p}/GetStatement?startDate=${this._stmtStart}&endDate=${this._stmtEnd}`
      );
      if (t.ok) {
        const a = await t.json();
        this._statement = Array.isArray(a) ? a : a.transacoes ?? [];
      } else
        this._stmtError = `HTTP ${t.status}: ${t.statusText}`;
    } catch (t) {
      this._stmtError = t instanceof Error ? t.message : String(t);
    } finally {
      this._stmtLoading = !1;
    }
  }
  async _createPixCharge() {
    this._pixLoading = !0, this._pixResult = "";
    try {
      const a = await (await fetch(`${p}/CreatePixCharge`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          amount: parseFloat(this._pixAmount),
          pixKey: this._pixKey,
          description: this._pixDesc
        })
      })).json();
      this._pixResult = JSON.stringify(a, null, 2);
    } catch (t) {
      this._pixResult = t instanceof Error ? t.message : String(t);
    } finally {
      this._pixLoading = !1;
    }
  }
  async _registerWebhook() {
    this._whLoading = !0, this._whResult = "";
    try {
      const a = await (await fetch(`${p}/RegisterPixWebhook`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ pixKey: this._whPixKey, webhookUrl: this._whUrl })
      })).json();
      this._whResult = JSON.stringify(a, null, 2);
    } catch (t) {
      this._whResult = t instanceof Error ? t.message : String(t);
    } finally {
      this._whLoading = !1;
    }
  }
  _fmtBRL(t) {
    return t == null ? "—" : new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL"
    }).format(t);
  }
  _connLabel() {
    switch (this._connStatus) {
      case "checking":
        return "Verificando…";
      case "connected":
        return "Conectado";
      case "error":
        return "Falha de conexão";
      default:
        return "Desconhecido";
    }
  }
  _renderOverview() {
    var l, n, s, d, c;
    const t = ((n = (l = this._balance) == null ? void 0 : l.saldo) == null ? void 0 : n.disponivel) ?? ((s = this._balance) == null ? void 0 : s.disponivel), a = (c = (d = this._balance) == null ? void 0 : d.saldo) == null ? void 0 : c.bloqueado;
    return r`
      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-card__label">Saldo disponível</div>
          <div class="stat-card__value stat-card__value--green">
            ${this._balanceLoading ? r`<span style="opacity:.5">Carregando…</span>` : this._fmtBRL(t)}
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-card__label">Saldo bloqueado</div>
          <div class="stat-card__value">
            ${this._balanceLoading ? r`<span style="opacity:.5">Carregando…</span>` : this._fmtBRL(a)}
          </div>
        </div>
      </div>

      ${this._balanceError ? r`<div class="notice notice--warn" style="margin-bottom:16px">
            <strong>Aviso:</strong> ${this._balanceError}. Verifique as
            credenciais em <code>appsettings.json</code> (seção
            <code>BancoInter</code>).
          </div>` : ""}

      <uui-box headline="Funcionalidades ativas">
        <div class="feature-grid">
          ${this._renderFeature("API Pix", !0)}
          ${this._renderFeature("Boleto c/ Pix", !0)}
          ${this._renderFeature("Pix Automático", !1)}
          ${this._renderFeature("Banking (Extrato)", !0)}
        </div>
        <p style="font-size:.8125rem;color:var(--uui-color-text-alt);margin:12px 0 0">
          Funcionalidades são habilitadas via <code>appsettings.json</code>.
        </p>
      </uui-box>

      <uui-box headline="Configuração">
        <div class="notice notice--info" style="margin:0">
          As credenciais (ClientId, ClientSecret, CertificatePath) são
          gerenciadas em <code>appsettings.json</code> na seção
          <code>BancoInter</code>. A chave <code>Sandbox: true</code> ativa o
          ambiente de testes. Configure mTLS com <code>CertificatePath</code> e
          <code>CertificateKeyPath</code> para produção.
        </div>
      </uui-box>

      <uui-box headline="Ação rápida">
        <uui-button
          look="secondary"
          label="Recarregar saldo"
          @click=${this._checkConnection}
          ?disabled=${this._balanceLoading}
        >
          ${this._balanceLoading ? "Carregando…" : "↻ Recarregar saldo"}
        </uui-button>
      </uui-box>
    `;
  }
  _renderFeature(t, a) {
    return r`
      <div class="feature-chip">
        <span class="feature-chip__dot feature-chip__dot--${a ? "on" : "off"}"></span>
        ${t}
        <span style="margin-left:auto;font-size:.75rem;color:${a ? "#15803d" : "#9ca3af"}">
          ${a ? "Ativo" : "Inativo"}
        </span>
      </div>
    `;
  }
  _renderStatement() {
    return r`
      <uui-box headline="Extrato bancário">
        <div class="statement-controls">
          <div class="field-group">
            <span class="field-label">Data início</span>
            <input
              class="native-input"
              type="date"
              .value=${this._stmtStart}
              @change=${(t) => this._stmtStart = t.target.value}
            />
          </div>
          <div class="field-group">
            <span class="field-label">Data fim</span>
            <input
              class="native-input"
              type="date"
              .value=${this._stmtEnd}
              @change=${(t) => this._stmtEnd = t.target.value}
            />
          </div>
          <uui-button
            look="primary"
            label="Buscar extrato"
            ?disabled=${this._stmtLoading}
            @click=${this._loadStatement}
          >
            ${this._stmtLoading ? "Buscando…" : "Buscar"}
          </uui-button>
        </div>

        ${this._stmtError ? r`<div class="notice notice--error">${this._stmtError}</div>` : ""}

        ${this._statement.length === 0 && !this._stmtLoading ? r`<div class="empty">Nenhuma transação. Selecione um período e clique em Buscar.</div>` : r`
            <table>
              <thead>
                <tr>
                  <th>Data/Hora</th>
                  <th>Tipo</th>
                  <th>Histórico</th>
                  <th style="text-align:right">Valor (BRL)</th>
                </tr>
              </thead>
              <tbody>
                ${this._statement.map(
      (t) => r`
                    <tr>
                      <td>${t.dataHora ? new Date(t.dataHora).toLocaleString("pt-BR") : "—"}</td>
                      <td><code style="font-size:.8125rem">${t.tipoOperacao ?? "—"}</code></td>
                      <td style="max-width:260px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap">
                        ${t.historico ?? "—"}
                      </td>
                      <td style="text-align:right">
                        <span class="${(t.valor ?? 0) >= 0 ? "amount--credit" : "amount--debit"}">
                          ${this._fmtBRL(t.valor)}
                        </span>
                      </td>
                    </tr>
                  `
    )}
              </tbody>
            </table>
          `}
      </uui-box>
    `;
  }
  _renderTools() {
    return r`
      <uui-box headline="Criar cobrança Pix (teste)">
        <div class="notice notice--warn">
          Esta ação cria uma cobrança real no ambiente configurado (sandbox ou
          produção). Use apenas para testes.
        </div>
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Valor (BRL) *</span>
            <input
              class="native-input"
              type="number"
              step="0.01"
              min="0.01"
              .value=${this._pixAmount}
              @input=${(t) => this._pixAmount = t.target.value}
            />
          </div>
          <div class="form-col">
            <span class="field-label">Chave Pix *</span>
            <input
              class="native-input"
              type="text"
              placeholder="CPF, CNPJ, e-mail ou telefone"
              .value=${this._pixKey}
              @input=${(t) => this._pixKey = t.target.value}
            />
          </div>
          <div class="form-col">
            <span class="field-label">Descrição</span>
            <input
              class="native-input"
              type="text"
              .value=${this._pixDesc}
              @input=${(t) => this._pixDesc = t.target.value}
            />
          </div>
        </div>
        <uui-button
          look="primary"
          label="Criar cobrança"
          ?disabled=${this._pixLoading || !this._pixKey}
          @click=${this._createPixCharge}
        >
          ${this._pixLoading ? "Criando…" : "Criar cobrança Pix"}
        </uui-button>
        ${this._pixResult ? r`<div class="response-box">${this._pixResult}</div>` : ""}
      </uui-box>

      <uui-box headline="Registrar webhook Pix">
        <div class="form-row">
          <div class="form-col">
            <span class="field-label">Chave Pix *</span>
            <input
              class="native-input"
              type="text"
              placeholder="Chave cadastrada no Inter"
              .value=${this._whPixKey}
              @input=${(t) => this._whPixKey = t.target.value}
            />
          </div>
          <div class="form-col">
            <span class="field-label">URL do webhook *</span>
            <input
              class="native-input"
              type="url"
              placeholder="https://seusite.com/umbraco/api/bancointersandbox/WebhookPix"
              .value=${this._whUrl}
              @input=${(t) => this._whUrl = t.target.value}
            />
          </div>
        </div>
        <uui-button
          look="secondary"
          label="Registrar webhook"
          ?disabled=${this._whLoading || !this._whPixKey || !this._whUrl}
          @click=${this._registerWebhook}
        >
          ${this._whLoading ? "Registrando…" : "Registrar webhook"}
        </uui-button>
        ${this._whResult ? r`<div class="response-box">${this._whResult}</div>` : ""}
      </uui-box>
    `;
  }
  render() {
    return r`
      <div class="dashboard-header">
        <div class="brand-logo"><span>BI</span></div>
        <div>
          <h1>Banco Inter Payments</h1>
          <p>Pix · Boleto · Banking — plugin para Umbraco 13/17</p>
        </div>
        <span class="status-pill status-pill--${this._connStatus}">
          <span class="status-dot"></span>
          ${this._connLabel()}
        </span>
      </div>

      <div class="tab-nav">
        ${["overview", "statement", "tools"].map(
      (t) => r`
            <button
              class="tab-btn ${this._tab === t ? "active" : ""}"
              @click=${() => this._tab = t}
            >
              ${{ overview: "Visão geral", statement: "Extrato", tools: "Ferramentas" }[t]}
            </button>
          `
    )}
      </div>

      ${this._tab === "overview" ? this._renderOverview() : ""}
      ${this._tab === "statement" ? this._renderStatement() : ""}
      ${this._tab === "tools" ? this._renderTools() : ""}
    `;
  }
};
e.styles = h`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
      font-family: var(--uui-font-family, sans-serif);
      color: var(--uui-color-text, #1a1a1a);
    }

    /* ── Header ── */
    .dashboard-header {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 24px;
    }
    .brand-logo {
      width: 40px;
      height: 40px;
      border-radius: 8px;
      background: #FF7A00;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    .brand-logo span {
      color: #fff;
      font-weight: 900;
      font-size: 18px;
      line-height: 1;
    }
    .dashboard-header h1 {
      margin: 0 0 4px;
      font-size: 1.5rem;
      font-weight: 700;
    }
    .dashboard-header p {
      margin: 0;
      color: var(--uui-color-text-alt, #6b7280);
      font-size: 0.875rem;
    }

    /* ── Status badge ── */
    .status-pill {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 4px 12px;
      border-radius: 9999px;
      font-size: 0.8125rem;
      font-weight: 600;
      margin-left: auto;
    }
    .status-pill--unknown { background: #f3f4f6; color: #374151; }
    .status-pill--checking { background: #eff6ff; color: #1d4ed8; }
    .status-pill--connected { background: #dcfce7; color: #15803d; }
    .status-pill--error { background: #fee2e2; color: #dc2626; }
    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: currentColor;
    }

    /* ── Tab nav ── */
    .tab-nav {
      display: flex;
      gap: 4px;
      border-bottom: 2px solid var(--uui-color-border, #e5e7eb);
      margin-bottom: 24px;
    }
    .tab-btn {
      background: none;
      border: none;
      padding: 8px 16px;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 500;
      color: var(--uui-color-text-alt, #6b7280);
      border-bottom: 2px solid transparent;
      margin-bottom: -2px;
      transition: color 0.15s, border-color 0.15s;
    }
    .tab-btn:hover { color: var(--uui-color-text, #1a1a1a); }
    .tab-btn.active {
      color: #FF7A00;
      border-bottom-color: #FF7A00;
    }

    /* ── Grid ── */
    .stat-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 16px;
      margin-bottom: 24px;
    }
    .stat-card {
      background: var(--uui-color-surface, #fff);
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 8px;
      padding: 16px;
    }
    .stat-card__label {
      font-size: 0.75rem;
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--uui-color-text-alt, #6b7280);
      margin-bottom: 6px;
    }
    .stat-card__value {
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--uui-color-text, #1a1a1a);
    }
    .stat-card__value--green { color: #15803d; }

    /* ── Feature chips ── */
    .feature-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
      gap: 12px;
      margin-top: 12px;
    }
    .feature-chip {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 10px 14px;
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 8px;
      font-size: 0.8125rem;
      font-weight: 500;
    }
    .feature-chip__dot {
      width: 10px;
      height: 10px;
      border-radius: 50%;
      flex-shrink: 0;
    }
    .feature-chip__dot--on { background: #22c55e; }
    .feature-chip__dot--off { background: #d1d5db; }

    /* ── Statement table ── */
    .statement-controls {
      display: flex;
      gap: 12px;
      align-items: flex-end;
      flex-wrap: wrap;
      margin-bottom: 16px;
    }
    .field-group {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .field-label {
      font-size: 0.75rem;
      font-weight: 500;
      color: var(--uui-color-text-alt, #6b7280);
    }
    .native-input {
      border: 1px solid var(--uui-color-border, #d1d5db);
      border-radius: 4px;
      padding: 7px 10px;
      font-size: 0.875rem;
      background: var(--uui-color-surface, #fff);
    }
    table {
      width: 100%;
      border-collapse: collapse;
      font-size: 0.875rem;
    }
    thead th {
      text-align: left;
      padding: 8px 12px;
      background: var(--uui-color-surface-alt, #f9fafb);
      border-bottom: 1px solid var(--uui-color-border, #e5e7eb);
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      color: var(--uui-color-text-alt, #6b7280);
    }
    tbody tr { border-bottom: 1px solid var(--uui-color-border, #e5e7eb); }
    tbody tr:hover { background: var(--uui-color-surface-alt, #f9fafb); }
    tbody td { padding: 10px 12px; }
    .amount--credit { color: #15803d; font-weight: 600; }
    .amount--debit { color: #dc2626; font-weight: 600; }

    /* ── Tools form ── */
    .form-row { display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 12px; }
    .form-col { display: flex; flex-direction: column; gap: 4px; flex: 1; min-width: 180px; }
    .notice {
      padding: 12px 16px;
      border-radius: 6px;
      font-size: 0.875rem;
      margin-bottom: 16px;
    }
    .notice--info { background: #eff6ff; color: #1e40af; border-left: 3px solid #3b82f6; }
    .notice--warn { background: #fffbeb; color: #92400e; border-left: 3px solid #f59e0b; }
    .notice--success { background: #f0fdf4; color: #15803d; border-left: 3px solid #22c55e; }
    .notice--error { background: #fef2f2; color: #991b1b; border-left: 3px solid #ef4444; }
    .response-box {
      background: #1e293b;
      color: #e2e8f0;
      padding: 12px 16px;
      border-radius: 6px;
      font-family: monospace;
      font-size: 0.8125rem;
      white-space: pre-wrap;
      word-break: break-all;
      max-height: 240px;
      overflow-y: auto;
      margin-top: 12px;
    }

    /* ── Misc ── */
    .section { margin-bottom: 24px; }
    .empty { padding: 32px; text-align: center; color: var(--uui-color-text-alt, #9ca3af); }
    uui-box { margin-bottom: 16px; }
  `;
o([
  i()
], e.prototype, "_tab", 2);
o([
  i()
], e.prototype, "_connStatus", 2);
o([
  i()
], e.prototype, "_balance", 2);
o([
  i()
], e.prototype, "_balanceLoading", 2);
o([
  i()
], e.prototype, "_balanceError", 2);
o([
  i()
], e.prototype, "_stmtStart", 2);
o([
  i()
], e.prototype, "_stmtEnd", 2);
o([
  i()
], e.prototype, "_statement", 2);
o([
  i()
], e.prototype, "_stmtLoading", 2);
o([
  i()
], e.prototype, "_stmtError", 2);
o([
  i()
], e.prototype, "_pixAmount", 2);
o([
  i()
], e.prototype, "_pixKey", 2);
o([
  i()
], e.prototype, "_pixDesc", 2);
o([
  i()
], e.prototype, "_pixLoading", 2);
o([
  i()
], e.prototype, "_pixResult", 2);
o([
  i()
], e.prototype, "_whPixKey", 2);
o([
  i()
], e.prototype, "_whUrl", 2);
o([
  i()
], e.prototype, "_whLoading", 2);
o([
  i()
], e.prototype, "_whResult", 2);
e = o([
  b("bancointer-dashboard")
], e);
const v = e;
export {
  e as BancoInterDashboardElement,
  v as default
};
