import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

const API = "/umbraco/api/bancointersandbox";

interface Balance {
  saldo?: { disponivel?: number; bloqueado?: number };
  disponivel?: number;
}

interface StatementEntry {
  dataHora?: string;
  tipoOperacao?: string;
  valor?: number;
  historico?: string;
  endToEndId?: string;
}

type Tab = "overview" | "statement" | "tools";
type ConnStatus = "unknown" | "checking" | "connected" | "error";

@customElement("bancointer-dashboard")
export class BancoInterDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
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

  @state() private _tab: Tab = "overview";
  @state() private _connStatus: ConnStatus = "unknown";
  @state() private _balance: Balance | null = null;
  @state() private _balanceLoading = false;
  @state() private _balanceError = "";

  @state() private _stmtStart = this._isoDate(-30);
  @state() private _stmtEnd = this._isoDate(0);
  @state() private _statement: StatementEntry[] = [];
  @state() private _stmtLoading = false;
  @state() private _stmtError = "";

  // Tools — Pix charge test
  @state() private _pixAmount = "10.00";
  @state() private _pixKey = "";
  @state() private _pixDesc = "Test charge";
  @state() private _pixLoading = false;
  @state() private _pixResult = "";

  // Tools — Webhook registration
  @state() private _whPixKey = "";
  @state() private _whUrl = "";
  @state() private _whLoading = false;
  @state() private _whResult = "";

  override connectedCallback() {
    super.connectedCallback();
    this._checkConnection();
  }

  private _isoDate(offsetDays: number): string {
    const d = new Date();
    d.setDate(d.getDate() + offsetDays);
    return d.toISOString().slice(0, 10);
  }

  private async _checkConnection() {
    this._connStatus = "checking";
    this._balanceLoading = true;
    this._balanceError = "";
    try {
      const r = await fetch(`${API}/GetBalance`);
      if (r.ok) {
        this._balance = await r.json();
        this._connStatus = "connected";
      } else {
        this._connStatus = "error";
        this._balanceError = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._connStatus = "error";
      this._balanceError = e instanceof Error ? e.message : String(e);
    } finally {
      this._balanceLoading = false;
    }
  }

  private async _loadStatement() {
    this._stmtLoading = true;
    this._stmtError = "";
    try {
      const r = await fetch(
        `${API}/GetStatement?startDate=${this._stmtStart}&endDate=${this._stmtEnd}`
      );
      if (r.ok) {
        const data = await r.json();
        this._statement = Array.isArray(data) ? data : (data.transacoes ?? []);
      } else {
        this._stmtError = `HTTP ${r.status}: ${r.statusText}`;
      }
    } catch (e) {
      this._stmtError = e instanceof Error ? e.message : String(e);
    } finally {
      this._stmtLoading = false;
    }
  }

  private async _createPixCharge() {
    this._pixLoading = true;
    this._pixResult = "";
    try {
      const r = await fetch(`${API}/CreatePixCharge`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          amount: parseFloat(this._pixAmount),
          pixKey: this._pixKey,
          description: this._pixDesc,
        }),
      });
      const body = await r.json();
      this._pixResult = JSON.stringify(body, null, 2);
    } catch (e) {
      this._pixResult = e instanceof Error ? e.message : String(e);
    } finally {
      this._pixLoading = false;
    }
  }

  private async _registerWebhook() {
    this._whLoading = true;
    this._whResult = "";
    try {
      const r = await fetch(`${API}/RegisterPixWebhook`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ pixKey: this._whPixKey, webhookUrl: this._whUrl }),
      });
      const body = await r.json();
      this._whResult = JSON.stringify(body, null, 2);
    } catch (e) {
      this._whResult = e instanceof Error ? e.message : String(e);
    } finally {
      this._whLoading = false;
    }
  }

  private _fmtBRL(val: number | undefined): string {
    if (val == null) return "—";
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(val);
  }

  private _connLabel(): string {
    switch (this._connStatus) {
      case "checking": return "Verificando…";
      case "connected": return "Conectado";
      case "error": return "Falha de conexão";
      default: return "Desconhecido";
    }
  }

  private _renderOverview() {
    const avail =
      this._balance?.saldo?.disponivel ?? this._balance?.disponivel;
    const bloq = this._balance?.saldo?.bloqueado;

    return html`
      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-card__label">Saldo disponível</div>
          <div class="stat-card__value stat-card__value--green">
            ${this._balanceLoading
              ? html`<span style="opacity:.5">Carregando…</span>`
              : this._fmtBRL(avail)}
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-card__label">Saldo bloqueado</div>
          <div class="stat-card__value">
            ${this._balanceLoading
              ? html`<span style="opacity:.5">Carregando…</span>`
              : this._fmtBRL(bloq)}
          </div>
        </div>
      </div>

      ${this._balanceError
        ? html`<div class="notice notice--warn" style="margin-bottom:16px">
            <strong>Aviso:</strong> ${this._balanceError}. Verifique as
            credenciais em <code>appsettings.json</code> (seção
            <code>BancoInter</code>).
          </div>`
        : ""}

      <uui-box headline="Funcionalidades ativas">
        <div class="feature-grid">
          ${this._renderFeature("API Pix", true)}
          ${this._renderFeature("Boleto c/ Pix", true)}
          ${this._renderFeature("Pix Automático", false)}
          ${this._renderFeature("Banking (Extrato)", true)}
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

  private _renderFeature(name: string, on: boolean) {
    return html`
      <div class="feature-chip">
        <span class="feature-chip__dot feature-chip__dot--${on ? "on" : "off"}"></span>
        ${name}
        <span style="margin-left:auto;font-size:.75rem;color:${on ? "#15803d" : "#9ca3af"}">
          ${on ? "Ativo" : "Inativo"}
        </span>
      </div>
    `;
  }

  private _renderStatement() {
    return html`
      <uui-box headline="Extrato bancário">
        <div class="statement-controls">
          <div class="field-group">
            <span class="field-label">Data início</span>
            <input
              class="native-input"
              type="date"
              .value=${this._stmtStart}
              @change=${(e: Event) => (this._stmtStart = (e.target as HTMLInputElement).value)}
            />
          </div>
          <div class="field-group">
            <span class="field-label">Data fim</span>
            <input
              class="native-input"
              type="date"
              .value=${this._stmtEnd}
              @change=${(e: Event) => (this._stmtEnd = (e.target as HTMLInputElement).value)}
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

        ${this._stmtError
          ? html`<div class="notice notice--error">${this._stmtError}</div>`
          : ""}

        ${this._statement.length === 0 && !this._stmtLoading
          ? html`<div class="empty">Nenhuma transação. Selecione um período e clique em Buscar.</div>`
          : html`
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
                  (t) => html`
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

  private _renderTools() {
    return html`
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
              @input=${(e: Event) => (this._pixAmount = (e.target as HTMLInputElement).value)}
            />
          </div>
          <div class="form-col">
            <span class="field-label">Chave Pix *</span>
            <input
              class="native-input"
              type="text"
              placeholder="CPF, CNPJ, e-mail ou telefone"
              .value=${this._pixKey}
              @input=${(e: Event) => (this._pixKey = (e.target as HTMLInputElement).value)}
            />
          </div>
          <div class="form-col">
            <span class="field-label">Descrição</span>
            <input
              class="native-input"
              type="text"
              .value=${this._pixDesc}
              @input=${(e: Event) => (this._pixDesc = (e.target as HTMLInputElement).value)}
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
        ${this._pixResult
          ? html`<div class="response-box">${this._pixResult}</div>`
          : ""}
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
              @input=${(e: Event) => (this._whPixKey = (e.target as HTMLInputElement).value)}
            />
          </div>
          <div class="form-col">
            <span class="field-label">URL do webhook *</span>
            <input
              class="native-input"
              type="url"
              placeholder="https://seusite.com/umbraco/api/bancointersandbox/WebhookPix"
              .value=${this._whUrl}
              @input=${(e: Event) => (this._whUrl = (e.target as HTMLInputElement).value)}
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
        ${this._whResult
          ? html`<div class="response-box">${this._whResult}</div>`
          : ""}
      </uui-box>
    `;
  }

  override render() {
    return html`
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
        ${(["overview", "statement", "tools"] as Tab[]).map(
          (t) => html`
            <button
              class="tab-btn ${this._tab === t ? "active" : ""}"
              @click=${() => (this._tab = t)}
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
}

export default BancoInterDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "bancointer-dashboard": BancoInterDashboardElement;
  }
}
