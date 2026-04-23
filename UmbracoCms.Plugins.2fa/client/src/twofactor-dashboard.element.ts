import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

@customElement("twofactor-dashboard")
export class TwoFactorDashboardElement extends UmbElementMixin(LitElement) {
  static override styles = css`
    :host { display: block; padding: var(--uui-size-layout-1, 24px); }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 8px; }
    p.description { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 24px; }
    .input-row { display: flex; gap: 10px; align-items: center; margin-bottom: 20px; }
    .status-badge { display: inline-block; padding: 4px 14px; border-radius: 9999px; font-weight: 600; font-size: 0.875rem; }
    .status-badge.enabled { background: #d1fae5; color: #065f46; }
    .status-badge.disabled { background: #fef3c7; color: #92400e; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 16px; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .secret-box { background: #eff6ff; padding: 1rem; border-radius: 6px; margin-top: 12px; }
    .secret-box code { font-size: 0.875rem; word-break: break-all; }
    .totp-input { font-size: 1.2rem; letter-spacing: 0.3em; text-align: center; width: 130px; padding: 8px; border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; }
    .backup-list { list-style: none; padding: 0; margin: 8px 0 0; font-family: monospace; }
    .backup-list li { padding: 2px 0; }
    .action-row { display: flex; gap: 10px; margin-top: 16px; flex-wrap: wrap; }
  `;

  @state() private _memberId = "";
  @state() private _status: boolean | null = null;
  @state() private _loading = false;
  @state() private _secret: string | null = null;
  @state() private _totpCode = "";
  @state() private _backupCodes: string[] = [];
  @state() private _message: { type: "success" | "error"; text: string } | null = null;

  private readonly _api = "/umbraco/api/twofactor";

  private async _checkStatus(): Promise<void> {
    if (!this._memberId) return;
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/IsEnabled?memberId=${this._memberId}`);
      const d = await r.json();
      this._status = d.enabled;
    } finally {
      this._loading = false;
    }
  }

  private async _setup(): Promise<void> {
    this._loading = true;
    this._message = null;
    try {
      const r = await fetch(`${this._api}/SetupTotp?memberId=${this._memberId}`, { method: "POST" });
      const d = await r.json();
      this._secret = d.secretKey;
    } finally {
      this._loading = false;
    }
  }

  private async _verify(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/VerifyTotp?memberId=${this._memberId}&code=${this._totpCode}`, { method: "POST" });
      const d = await r.json();
      if (d.valid) {
        this._status = true;
        this._secret = null;
        this._message = { type: "success", text: "2FA enabled successfully!" };
      } else {
        this._message = { type: "error", text: "Invalid code. Please try again." };
      }
    } finally {
      this._loading = false;
    }
  }

  private async _generateBackupCodes(): Promise<void> {
    this._loading = true;
    try {
      const r = await fetch(`${this._api}/GenerateBackupCodes?memberId=${this._memberId}&count=8`, { method: "POST" });
      const d = await r.json();
      this._backupCodes = d.codes ?? [];
      this._message = { type: "success", text: "Backup codes generated. Save them securely!" };
    } finally {
      this._loading = false;
    }
  }

  private async _disable(): Promise<void> {
    if (!confirm("Disable 2FA for this member?")) return;
    this._loading = true;
    try {
      await fetch(`${this._api}/Disable?memberId=${this._memberId}`, { method: "POST" });
      this._status = false;
      this._secret = null;
      this._backupCodes = [];
      this._message = { type: "success", text: "2FA disabled." };
    } finally {
      this._loading = false;
    }
  }

  override render() {
    return html`
      <h1>Two-Factor Authentication</h1>
      <p class="description">Manage TOTP 2FA and backup codes for Umbraco members.</p>

      <uui-box headline="Member Lookup">
        <div class="input-row">
          <input type="number" .value=${this._memberId}
            @input=${(e: InputEvent) => (this._memberId = (e.target as HTMLInputElement).value)}
            placeholder="Member ID" style="width:140px;padding:8px;border:1px solid #d1d5db;border-radius:4px;" />
          <uui-button look="secondary" ?disabled=${!this._memberId || this._loading}
            @click=${this._checkStatus}>Check Status</uui-button>
        </div>
      </uui-box>

      ${this._message ? html`<div class="msg ${this._message.type}" style="margin-top:12px;">${this._message.text}</div>` : ""}

      ${this._status !== null ? html`
        <uui-box headline="2FA Status" style="margin-top:16px;">
          <span class="status-badge ${this._status ? "enabled" : "disabled"}">
            ${this._status ? "Enabled" : "Disabled"}
          </span>

          ${this._status ? html`
            <div class="action-row">
              <uui-button look="secondary" @click=${this._generateBackupCodes} ?disabled=${this._loading}>
                Generate Backup Codes
              </uui-button>
              <uui-button look="danger" @click=${this._disable} ?disabled=${this._loading}>
                Disable 2FA
              </uui-button>
            </div>
            ${this._backupCodes.length > 0 ? html`
              <div style="margin-top:16px;background:#f9fafb;padding:1rem;border-radius:6px;">
                <strong>Backup Codes (save these now):</strong>
                <ul class="backup-list">
                  ${this._backupCodes.map((c) => html`<li>${c}</li>`)}
                </ul>
              </div>
            ` : ""}
          ` : html`
            <div class="action-row">
              <uui-button look="primary" @click=${this._setup} ?disabled=${this._loading}>
                Set Up TOTP
              </uui-button>
            </div>
            ${this._secret ? html`
              <div class="secret-box">
                <p><strong>Secret Key:</strong> <code>${this._secret}</code></p>
                <p style="margin-top:8px;">Enter this key in your authenticator app, then verify:</p>
                <div class="input-row" style="margin-top:8px;">
                  <input class="totp-input" type="text" maxlength="6" placeholder="000000"
                    .value=${this._totpCode}
                    @input=${(e: InputEvent) => (this._totpCode = (e.target as HTMLInputElement).value)} />
                  <uui-button look="positive" @click=${this._verify} ?disabled=${this._loading}>
                    Verify &amp; Enable
                  </uui-button>
                </div>
              </div>
            ` : ""}
          `}
        </uui-box>
      ` : ""}
    `;
  }
}

export default TwoFactorDashboardElement;

declare global {
  interface HTMLElementTagNameMap {
    "twofactor-dashboard": TwoFactorDashboardElement;
  }
}
