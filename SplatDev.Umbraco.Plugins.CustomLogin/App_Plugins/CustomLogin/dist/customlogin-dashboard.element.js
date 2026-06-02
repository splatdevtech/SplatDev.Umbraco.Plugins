import { LitElement as h, nothing as g, html as a, css as p, state as l, customElement as m } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as b } from "@umbraco-cms/backoffice/element-api";
var v = Object.defineProperty, x = Object.getOwnPropertyDescriptor, n = (e, i, t, s) => {
  for (var o = s > 1 ? void 0 : s ? x(i, t) : i, d = e.length - 1, u; d >= 0; d--)
    (u = e[d]) && (o = (s ? u(i, t, o) : u(o)) || o);
  return s && o && v(i, t, o), o;
};
const f = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], _ = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];
function c() {
  return {
    brandName: "",
    logoUrl: "",
    logoAlternativeUrl: "",
    backgroundImageUrl: "",
    supportEmail: "",
    loginPageTitle: "",
    faviconUrl: "",
    footerText: "",
    backgroundColor: "",
    primaryColor: "",
    textColor: "",
    curvesColor: "",
    showCurves: !0,
    showImagePanel: !0,
    imageBorderRadius: "",
    contentBackground: "",
    contentWidth: "",
    contentHeight: "",
    contentBorderRadius: "",
    alignItems: "",
    headerFontSize: "",
    headerFontSizeLarge: "",
    headerSecondaryFontSize: "",
    buttonBorderRadius: "",
    allowPasswordReset: !0,
    enableSso: !1,
    greetings: Array(7).fill(""),
    greetingsEs: Array(7).fill(""),
    timeoutBackgroundImageUrl: "",
    timeoutInstructionText: "",
    timeoutInstructionTextEs: "",
    customCss: ""
  };
}
let r = class extends b(h) {
  constructor() {
    super(...arguments), this._s = c(), this._loading = !1, this._saving = !1, this._message = null, this._api = "/umbraco/api/customlogin";
  }
  connectedCallback() {
    super.connectedCallback(), this._load();
  }
  async _load() {
    this._loading = !0;
    try {
      const e = await fetch(`${this._api}/GetSettings`);
      if (e.ok) {
        const i = await e.json();
        this._s = { ...c(), ...i }, (!Array.isArray(this._s.greetings) || this._s.greetings.length < 7) && (this._s.greetings = Array(7).fill("")), (!Array.isArray(this._s.greetingsEs) || this._s.greetingsEs.length < 7) && (this._s.greetingsEs = Array(7).fill(""));
      }
    } catch {
    } finally {
      this._loading = !1;
    }
  }
  async _save() {
    this._saving = !0, this._message = null;
    try {
      const e = await fetch(`${this._api}/SaveSettings`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(this._s)
      });
      this._message = e.ok ? { type: "success", text: "Settings saved. CSS and localization files regenerated." } : { type: "error", text: "Failed to save settings." };
    } catch {
      this._message = { type: "error", text: "Network error." };
    } finally {
      this._saving = !1;
    }
  }
  _set(e, i) {
    this._s = { ...this._s, [e]: i };
  }
  _setGreeting(e, i) {
    const t = [...this._s.greetings];
    t[e] = i, this._set("greetings", t);
  }
  _setGreetingEs(e, i) {
    const t = [...this._s.greetingsEs];
    t[e] = i, this._set("greetingsEs", t);
  }
  _textField(e, i, t) {
    return a`
      <div class="field">
        <label>${e}</label>
        <input type="${(t == null ? void 0 : t.type) ?? "text"}"
          .value=${this._s[i] ?? ""}
          @input=${(s) => this._set(i, s.target.value)}
          placeholder="${(t == null ? void 0 : t.placeholder) ?? ""}" />
        ${t != null && t.hint ? a`<div class="hint">${t.hint}</div>` : g}
      </div>`;
  }
  _colorField(e, i, t) {
    const s = this._s[i] ?? "";
    return a`
      <div class="field">
        <label>${e}</label>
        <div class="color-row">
          <input type="color" .value=${s || "#000000"}
            @input=${(o) => this._set(i, o.target.value)} />
          <input type="text" .value=${s}
            @input=${(o) => this._set(i, o.target.value)}
            placeholder="#000000" />
        </div>
        ${t ? a`<div class="hint">${t}</div>` : g}
      </div>`;
  }
  render() {
    return this._loading ? a`<uui-loader></uui-loader>` : a`
      <h1>Custom Login Settings</h1>
      <p class="desc">Configure every aspect of the Umbraco backoffice login page — branding, colors, layout, greetings, and more.</p>

      ${this._message ? a`<div class="msg ${this._message.type}">${this._message.text}</div>` : g}

      <!-- BRANDING -->
      <uui-box headline="Branding">
        <div class="section-note">Logo and background image paths are relative to <code>/wwwroot/umbraco/</code> or absolute URLs.
          These also configure <code>Umbraco:CMS:Content:LoginLogoImage</code> and related appsettings (requires restart for appsettings changes).</div>
        ${this._textField("Brand Name", "brandName", { placeholder: "My Company" })}
        ${this._textField("Logo URL", "logoUrl", { type: "url", placeholder: "../myImages/logo.svg", hint: "Maps to LoginLogoImage appsetting. Shown on the login screen." })}
        ${this._textField("Logo Alternative URL", "logoAlternativeUrl", { type: "url", placeholder: "../myImages/logo-alt.svg", hint: "Shown on small screens. Maps to LoginLogoImageAlternative." })}
        ${this._textField("Background Image URL", "backgroundImageUrl", { type: "url", placeholder: "../myImages/login-bg.jpg", hint: "Left panel background. Also configurable via CSS --umb-login-image." })}
        ${this._textField("Support Email", "supportEmail", { type: "email", placeholder: "support@example.com" })}
        ${this._textField("Login Page Title", "loginPageTitle", { placeholder: "My Company — Login", hint: "Overrides the browser tab title on the login page." })}
        ${this._textField("Favicon URL", "faviconUrl", { type: "url", placeholder: "/favicon.ico", hint: "Custom favicon shown on the login page." })}
        ${this._textField("Footer Text", "footerText", { placeholder: "© 2026 My Company", hint: "Displayed at the bottom of the login page." })}
      </uui-box>

      <!-- COLORS -->
      <uui-box headline="Colors">
        <div class="section-note">These map to Umbraco CSS custom properties on the login page. Leave blank to use Umbraco defaults.</div>
        <div class="two-col">
          ${this._colorField("Background", "backgroundColor", "--umb-login-background (default: #f4f4f4)")}
          ${this._colorField("Primary Color", "primaryColor", "--umb-login-primary-color (default: #283a97)")}
          ${this._colorField("Text Color", "textColor", "--umb-login-text-color (default: #000)")}
          ${this._colorField("Curves Color", "curvesColor", "--umb-login-curves-color (default: #f5c1bc)")}
        </div>
      </uui-box>

      <!-- LAYOUT -->
      <uui-box headline="Layout">
        <div class="section-note">Control the structural layout of the login page panels.</div>
        <div class="field">
          <uui-toggle .checked=${this._s.showImagePanel}
            @change=${(e) => this._set("showImagePanel", e.target.checked)}>
            Show image panel (left side)
          </uui-toggle>
          <div class="hint">--umb-login-image-display. When hidden, only the login form is shown.</div>
        </div>
        <div class="field">
          <uui-toggle .checked=${this._s.showCurves}
            @change=${(e) => this._set("showCurves", e.target.checked)}>
            Show decorative curves
          </uui-toggle>
          <div class="hint">--umb-login-curves-display. The SVG wave overlay on the image panel.</div>
        </div>
        <div class="two-col">
          ${this._textField("Image Border Radius", "imageBorderRadius", { placeholder: "38px", hint: "--umb-login-image-border-radius" })}
          ${this._textField("Content Background", "contentBackground", { placeholder: "none", hint: "--umb-login-content-background" })}
          ${this._textField("Content Width", "contentWidth", { placeholder: "100%", hint: "--umb-login-content-width" })}
          ${this._textField("Content Height", "contentHeight", { placeholder: "100%", hint: "--umb-login-content-height" })}
          ${this._textField("Content Border Radius", "contentBorderRadius", { placeholder: "0", hint: "--umb-login-content-border-radius" })}
          ${this._textField("Align Items", "alignItems", { placeholder: "unset", hint: "--umb-login-align-items (e.g., center, flex-start)" })}
        </div>
      </uui-box>

      <!-- TYPOGRAPHY -->
      <uui-box headline="Typography">
        <div class="two-col">
          ${this._textField("Header Font Size", "headerFontSize", { placeholder: "3rem", hint: "--umb-login-header-font-size" })}
          ${this._textField("Header Font Size (Large)", "headerFontSizeLarge", { placeholder: "4rem", hint: "--umb-login-header-font-size-large (screens > 900px)" })}
          ${this._textField("Secondary Header Font Size", "headerSecondaryFontSize", { placeholder: "2.4rem", hint: "--umb-login-header-secondary-font-size" })}
        </div>
      </uui-box>

      <!-- BUTTONS -->
      <uui-box headline="Buttons">
        ${this._textField("Button Border Radius", "buttonBorderRadius", { placeholder: "45px", hint: "--umb-login-button-border-radius" })}
      </uui-box>

      <!-- GREETINGS -->
      <uui-box headline="Greetings (English)">
        <div class="section-note">Custom greeting text shown on the login page by day of week. Leave blank for Umbraco defaults.</div>
        <div class="greeting-grid">
          ${f.map((e, i) => a`
            <label>${e}</label>
            <input type="text" .value=${this._s.greetings[i] ?? ""}
              @input=${(t) => this._setGreeting(i, t.target.value)}
              placeholder="Happy ${e}" />
          `)}
        </div>
      </uui-box>

      <uui-box headline="Greetings (Spanish)">
        <div class="greeting-grid">
          ${_.map((e, i) => a`
            <label>${e}</label>
            <input type="text" .value=${this._s.greetingsEs[i] ?? ""}
              @input=${(t) => this._setGreetingEs(i, t.target.value)}
              placeholder="Feliz ${e.toLowerCase()}" />
          `)}
        </div>
      </uui-box>

      <!-- TIMEOUT -->
      <uui-box headline="Session Timeout Screen">
        <div class="section-note">Customize the screen shown when the user's session expires.</div>
        ${this._textField("Timeout Background Image", "timeoutBackgroundImageUrl", { type: "url", placeholder: "../myImages/timeout-bg.jpg", hint: "Overrides --umb-login-image for the timeout screen only." })}
        ${this._textField("Timeout Instruction (EN)", "timeoutInstructionText", { placeholder: "Log in again to continue", hint: "Overrides auth.instruction localization key." })}
        ${this._textField("Timeout Instruction (ES)", "timeoutInstructionTextEs", { placeholder: "Inicie sesión nuevamente para continuar" })}
      </uui-box>

      <!-- SECURITY -->
      <uui-box headline="Security">
        <div class="field">
          <uui-toggle .checked=${this._s.allowPasswordReset}
            @change=${(e) => this._set("allowPasswordReset", e.target.checked)}>
            Allow password reset
          </uui-toggle>
          <div class="hint">Shows the "Forgot password?" link. Requires SMTP configured. Maps to Umbraco:CMS:Security:AllowPasswordReset (restart required).</div>
        </div>
        <div class="field">
          <uui-toggle .checked=${this._s.enableSso}
            @change=${(e) => this._set("enableSso", e.target.checked)}>
            Enable SSO hook
          </uui-toggle>
          <div class="hint">Shows "Sign in with SSO" button on member login pages.</div>
        </div>
      </uui-box>

      <!-- CUSTOM CSS -->
      <uui-box headline="Custom CSS">
        <div class="section-note">Raw CSS appended after the generated styles. Applies to the entire backoffice, not just the login page.</div>
        <div class="field">
          <textarea .value=${this._s.customCss}
            @input=${(e) => this._set("customCss", e.target.value)}
            placeholder=":root { /* your overrides */ }"></textarea>
        </div>
      </uui-box>

      <div class="actions">
        <uui-button look="primary" ?disabled=${this._saving} @click=${this._save}>
          ${this._saving ? "Saving..." : "Save Settings"}
        </uui-button>
        <uui-button look="secondary" @click=${this._load} ?disabled=${this._loading}>
          Reset
        </uui-button>
      </div>
    `;
  }
};
r.styles = p`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
      max-width: 900px;
    }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0 0 4px; }
    p.desc { color: var(--uui-color-text-alt, #6b7280); margin: 0 0 20px; font-size: 0.9rem; }
    uui-box { margin-bottom: 16px; }
    .field { margin-bottom: 14px; }
    .field label {
      display: block;
      font-size: 0.8rem;
      font-weight: 600;
      margin-bottom: 3px;
      color: var(--uui-color-text, #333);
    }
    .field .hint {
      font-size: 0.75rem;
      color: var(--uui-color-text-alt, #6b7280);
      margin-top: 2px;
    }
    .field input[type="text"],
    .field input[type="url"],
    .field input[type="email"] {
      width: 100%;
      max-width: 520px;
      padding: 7px 10px;
      border: 1px solid var(--uui-color-border, #d1d5db);
      border-radius: 4px;
      font-size: 0.9rem;
    }
    .field textarea {
      width: 100%;
      max-width: 520px;
      min-height: 100px;
      padding: 7px 10px;
      border: 1px solid var(--uui-color-border, #d1d5db);
      border-radius: 4px;
      font-size: 0.85rem;
      font-family: monospace;
      resize: vertical;
    }
    .color-row { display: flex; align-items: center; gap: 8px; }
    .color-row input[type="color"] { width: 36px; height: 32px; padding: 0; border: 1px solid #ccc; cursor: pointer; }
    .color-row input[type="text"] { width: 120px; }
    .two-col { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
    .msg { padding: 10px 14px; border-radius: 4px; margin-bottom: 16px; font-size: 0.9rem; }
    .msg.success { background: #d1fae5; color: #065f46; }
    .msg.error { background: #fee2e2; color: #991b1b; }
    .greeting-grid { display: grid; grid-template-columns: 100px 1fr; gap: 6px 10px; align-items: center; }
    .greeting-grid label { font-size: 0.8rem; font-weight: 500; }
    .greeting-grid input { padding: 5px 8px; border: 1px solid var(--uui-color-border, #d1d5db); border-radius: 4px; font-size: 0.85rem; }
    .section-note {
      background: var(--uui-color-surface-alt, #f9fafb);
      border: 1px solid var(--uui-color-border, #e5e7eb);
      border-radius: 6px;
      padding: 10px 14px;
      font-size: 0.8rem;
      color: var(--uui-color-text-alt, #6b7280);
      margin-bottom: 14px;
    }
    .actions { margin-top: 20px; display: flex; gap: 10px; }
  `;
n([
  l()
], r.prototype, "_s", 2);
n([
  l()
], r.prototype, "_loading", 2);
n([
  l()
], r.prototype, "_saving", 2);
n([
  l()
], r.prototype, "_message", 2);
r = n([
  m("customlogin-dashboard")
], r);
const $ = r;
export {
  r as CustomLoginDashboardElement,
  $ as default
};
