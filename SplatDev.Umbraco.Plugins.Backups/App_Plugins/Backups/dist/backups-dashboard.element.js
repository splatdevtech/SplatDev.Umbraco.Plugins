var U = (e) => {
  throw TypeError(e);
};
var se = (e, a, t) => a.has(e) || U("Cannot " + t);
var g = (e, a, t) => (se(e, a, "read from private field"), t ? t.call(e) : a.get(e)), I = (e, a, t) => a.has(e) ? U("Cannot add the same private member more than once") : a instanceof WeakSet ? a.add(e) : a.set(e, t);
import { LitElement as P, html as l, css as D, state as n, customElement as E, property as q, nothing as oe } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as A } from "@umbraco-cms/backoffice/element-api";
import { UmbContextBase as re } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as le } from "@umbraco-cms/backoffice/context-api";
const M = new le("SplatDev.Backups.Context");
var f;
class ne extends re {
  constructor(t) {
    super(t, M);
    I(this, f, "/umbraco/api/backups");
  }
  async getBackups() {
    const t = await fetch(`${g(this, f)}/GetAll`);
    if (!t.ok) throw new Error("Failed to load backups");
    return t.json();
  }
  async getProviders() {
    const t = await fetch(`${g(this, f)}/GetCloudProviders`);
    if (!t.ok) throw new Error("Failed to load providers");
    return t.json();
  }
  async createBackup(t) {
    if (!(await fetch(`${g(this, f)}/Create`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(t)
    })).ok) throw new Error("Failed to create backup");
  }
  async deleteBackup(t) {
    if (!(await fetch(
      `${g(this, f)}/Delete?name=${encodeURIComponent(t)}`,
      { method: "DELETE" }
    )).ok) throw new Error("Failed to delete backup");
  }
  formatSize(t) {
    if (t === 0) return "0 B";
    const s = ["B", "KB", "MB", "GB", "TB"], i = Math.floor(Math.log(t) / Math.log(1024));
    return `${(t / Math.pow(1024, i)).toFixed(1)} ${s[i]}`;
  }
}
f = new WeakMap();
var ce = Object.defineProperty, ue = Object.getOwnPropertyDescriptor, Z = (e) => {
  throw TypeError(e);
}, p = (e, a, t, s) => {
  for (var i = s > 1 ? void 0 : s ? ue(a, t) : a, o = e.length - 1, r; o >= 0; o--)
    (r = e[o]) && (i = (s ? r(a, t, i) : r(i)) || i);
  return s && i && ce(a, t, i), i;
}, F = (e, a, t) => a.has(e) || Z("Cannot " + t), B = (e, a, t) => (F(e, a, "read from private field"), a.get(e)), H = (e, a, t) => a.has(e) ? Z("Cannot add the same private member more than once") : a instanceof WeakSet ? a.add(e) : a.set(e, t), pe = (e, a, t, s) => (F(e, a, "write to private field"), a.set(e, t), t), x = (e, a, t) => (F(e, a, "access private method"), t), v, _, J, O, V;
const de = [
  { value: 1, label: "Content" },
  { value: 2, label: "Media" },
  { value: 4, label: "Database" },
  { value: 3, label: "Content + Media" },
  { value: 7, label: "Full (Content + Media + Database)" }
];
let c = class extends A(P) {
  constructor() {
    super(...arguments), H(this, _), this._open = !1, this._creating = !1, this._providers = [], this._name = "", this._includeMedia = !0, this._scope = 7, this._compress = !0, this._selectedProviders = [], H(this, v);
  }
  connectedCallback() {
    super.connectedCallback(), this.consumeContext(M, (e) => {
      pe(this, v, e), x(this, _, J).call(this);
    });
  }
  render() {
    const e = this._providers.filter((t) => t.enabled), a = de.map((t) => ({
      name: t.label,
      value: String(t.value),
      selected: t.value === this._scope
    }));
    return l`
      <uui-button
        look="primary"
        label=${this._creating ? "Creating…" : "Create Backup"}
        ?disabled=${this._creating}
        @click=${x(this, _, O)}
      >
        ${this._creating ? "Creating…" : "Create Backup"}
      </uui-button>

      <div class="form-container">
        <uui-box headline="New Backup">
          <div class="form-row">
            <label for="backup-name">Backup Name</label>
            <uui-input
              id="backup-name"
              placeholder="e.g. pre-deploy-2026-05-28"
              .value=${this._name}
              @input=${(t) => {
      this._name = t.target.value;
    }}
            ></uui-input>
          </div>

          <div class="form-row">
            <label>Scope</label>
            <uui-select
              .options=${a}
              @change=${(t) => {
      this._scope = Number(t.target.value);
    }}
            ></uui-select>
          </div>

          <div class="toggle-row">
            <uui-toggle
              label="Include Media"
              ?checked=${this._includeMedia}
              @change=${(t) => {
      this._includeMedia = t.target.checked;
    }}
            ></uui-toggle>
          </div>

          <div class="toggle-row">
            <uui-toggle
              label="Compress"
              ?checked=${this._compress}
              @change=${(t) => {
      this._compress = t.target.checked;
    }}
            ></uui-toggle>
          </div>

          ${e.length > 0 ? l`
                <div class="form-row">
                  <label>Cloud Storage Destinations</label>
                  ${e.map(
      (t) => l`
                      <div class="toggle-row">
                        <uui-checkbox
                          label=${t.providerType}
                          ?checked=${this._selectedProviders.includes(t.id)}
                          @change=${(s) => {
        s.target.checked ? this._selectedProviders = [...this._selectedProviders, t.id] : this._selectedProviders = this._selectedProviders.filter(
          (o) => o !== t.id
        );
      }}
                        ></uui-checkbox>
                      </div>
                    `
    )}
                </div>
              ` : ""}

          <div class="button-row">
            <uui-button
              look="primary"
              label="Run Backup Now"
              ?disabled=${this._creating || !this._name.trim()}
              @click=${x(this, _, V)}
            >
              ${this._creating ? "Running…" : "Run Backup Now"}
            </uui-button>
            <uui-button
              look="secondary"
              label="Cancel"
              @click=${x(this, _, O)}
            >
              Cancel
            </uui-button>
          </div>
        </uui-box>
      </div>
    `;
  }
};
v = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakSet();
J = async function() {
  if (B(this, v))
    try {
      this._providers = await B(this, v).getProviders();
    } catch {
      this._providers = [];
    }
};
O = function() {
  this._open = !this._open, this._open ? this.setAttribute("open", "") : this.removeAttribute("open");
};
V = async function() {
  if (B(this, v) && this._name.trim()) {
    this._creating = !0;
    try {
      await B(this, v).createBackup({
        name: this._name.trim(),
        includeMedia: this._includeMedia,
        scope: this._scope,
        compress: this._compress,
        encrypt: !1,
        encryptionKey: "",
        cloudProviderIds: this._selectedProviders
      }), this._name = "", this._open = !1, this.removeAttribute("open"), this.dispatchEvent(
        new CustomEvent("backup-created", { bubbles: !0, composed: !0 })
      );
    } catch {
      this.dispatchEvent(
        new CustomEvent("backup-error", {
          detail: { message: "Failed to create backup" },
          bubbles: !0,
          composed: !0
        })
      );
    }
    this._creating = !1;
  }
};
c.styles = D`
    :host {
      display: block;
    }
    .form-container {
      display: none;
      margin-top: var(--uui-size-space-4, 12px);
    }
    :host([open]) .form-container {
      display: block;
    }
    .form-row {
      display: flex;
      flex-direction: column;
      gap: 4px;
      margin-bottom: var(--uui-size-space-4, 12px);
      max-width: 480px;
    }
    .form-row label {
      font-weight: 500;
      font-size: 0.875rem;
    }
    .toggle-row {
      display: flex;
      align-items: center;
      gap: var(--uui-size-space-3, 8px);
      margin-bottom: var(--uui-size-space-3, 8px);
    }
    .button-row {
      display: flex;
      gap: var(--uui-size-space-3, 8px);
      margin-top: var(--uui-size-space-5, 16px);
    }
  `;
p([
  n()
], c.prototype, "_open", 2);
p([
  n()
], c.prototype, "_creating", 2);
p([
  n()
], c.prototype, "_providers", 2);
p([
  n()
], c.prototype, "_name", 2);
p([
  n()
], c.prototype, "_includeMedia", 2);
p([
  n()
], c.prototype, "_scope", 2);
p([
  n()
], c.prototype, "_compress", 2);
p([
  n()
], c.prototype, "_selectedProviders", 2);
c = p([
  E("backups-create-modal")
], c);
var he = Object.defineProperty, be = Object.getOwnPropertyDescriptor, G = (e, a, t, s) => {
  for (var i = s > 1 ? void 0 : s ? be(a, t) : a, o = e.length - 1, r; o >= 0; o--)
    (r = e[o]) && (i = (s ? r(a, t, i) : r(i)) || i);
  return s && i && he(a, t, i), i;
};
const fe = {
  local: { initials: "FS", color: "#64748b", label: "Local" },
  LocalFileSystem: { initials: "FS", color: "#64748b", label: "Local" },
  AzureBlobStorage: { initials: "AZ", color: "#0078d4", label: "Azure Blob" },
  azure: { initials: "AZ", color: "#0078d4", label: "Azure Blob" },
  GoogleDrive: { initials: "GD", color: "#4285f4", label: "Google Drive" },
  googledrive: { initials: "GD", color: "#4285f4", label: "Google Drive" },
  Dropbox: { initials: "DB", color: "#0061ff", label: "Dropbox" },
  dropbox: { initials: "DB", color: "#0061ff", label: "Dropbox" },
  BoxNet: { initials: "BX", color: "#0061d5", label: "Box.net" },
  box: { initials: "BX", color: "#0061d5", label: "Box.net" },
  OneDrive: { initials: "OD", color: "#0f3cc9", label: "OneDrive" },
  onedrive: { initials: "OD", color: "#0f3cc9", label: "OneDrive" },
  Mega: { initials: "MG", color: "#d9272e", label: "Mega" },
  mega: { initials: "MG", color: "#d9272e", label: "Mega" },
  Seafile: { initials: "SF", color: "#0d8f75", label: "Seafile" },
  seafile: { initials: "SF", color: "#0d8f75", label: "Seafile" },
  AwsS3: { initials: "S3", color: "#ff9900", label: "Amazon S3" },
  s3: { initials: "S3", color: "#ff9900", label: "Amazon S3" },
  Sftp: { initials: "FTP", color: "#4a4a4a", label: "SFTP" },
  sftp: { initials: "FTP", color: "#4a4a4a", label: "SFTP" }
};
let k = class extends P {
  constructor() {
    super(...arguments), this.provider = "local", this.showLabel = !1;
  }
  render() {
    const e = fe[this.provider] ?? { initials: "?", color: "#999", label: this.provider };
    return l`
      <span class="badge" style="background:${e.color}">${e.initials}</span>
      ${this.showLabel ? l`<span class="label">${e.label}</span>` : ""}
    `;
  }
};
k.styles = D`
    :host {
      display: inline-flex;
      align-items: center;
      gap: 6px;
    }
    .badge {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 28px;
      height: 28px;
      border-radius: 4px;
      color: #fff;
      font-size: 0.6rem;
      font-weight: 700;
      letter-spacing: 0.5px;
    }
    .label {
      font-size: 0.825rem;
    }
  `;
G([
  q({ type: String })
], k.prototype, "provider", 2);
G([
  q({ type: Boolean, attribute: "show-label" })
], k.prototype, "showLabel", 2);
k = G([
  E("backups-provider-badge")
], k);
var ve = Object.defineProperty, _e = Object.getOwnPropertyDescriptor, Q = (e) => {
  throw TypeError(e);
}, N = (e, a, t, s) => {
  for (var i = s > 1 ? void 0 : s ? _e(a, t) : a, o = e.length - 1, r; o >= 0; o--)
    (r = e[o]) && (i = (s ? r(a, t, i) : r(i)) || i);
  return s && i && ve(a, t, i), i;
}, L = (e, a, t) => a.has(e) || Q("Cannot " + t), y = (e, a, t) => (L(e, a, "read from private field"), t ? t.call(e) : a.get(e)), K = (e, a, t) => a.has(e) ? Q("Cannot add the same private member more than once") : a instanceof WeakSet ? a.add(e) : a.set(e, t), ge = (e, a, t, s) => (L(e, a, "write to private field"), a.set(e, t), t), m = (e, a, t) => (L(e, a, "access private method"), t), h, b, S, Y, j;
let w = class extends A(P) {
  constructor() {
    super(...arguments), K(this, b), this._backups = [], this._loading = !1, K(this, h);
  }
  connectedCallback() {
    super.connectedCallback(), this.consumeContext(M, (e) => {
      ge(this, h, e), m(this, b, S).call(this);
    });
  }
  async refresh() {
    await m(this, b, S).call(this);
  }
  render() {
    return this._loading ? l`<uui-loader-bar></uui-loader-bar>` : this._backups.length === 0 ? l`
        <div class="empty-state">
          <p>No backups found. Create your first backup above.</p>
        </div>
      ` : l`
      <uui-table>
        <uui-table-head>
          <uui-table-head-cell>Name</uui-table-head-cell>
          <uui-table-head-cell>Date</uui-table-head-cell>
          <uui-table-head-cell>Size</uui-table-head-cell>
          <uui-table-head-cell>Type</uui-table-head-cell>
          <uui-table-head-cell>Actions</uui-table-head-cell>
        </uui-table-head>
        ${this._backups.map(
      (e) => {
        var a;
        return l`
            <uui-table-row>
              <uui-table-cell>${e.name}</uui-table-cell>
              <uui-table-cell>${m(this, b, j).call(this, e.createdAt)}</uui-table-cell>
              <uui-table-cell>${((a = y(this, h)) == null ? void 0 : a.formatSize(e.sizeBytes)) ?? e.sizeBytes}</uui-table-cell>
              <uui-table-cell>
                <span class="extension-badge">${e.extension || "zip"}</span>
                ${e.isCompressed ? l`<uui-tag look="secondary" size="small">Compressed</uui-tag>` : oe}
              </uui-table-cell>
              <uui-table-cell>
                <div class="actions">
                  <uui-button
                    look="secondary"
                    compact
                    label="Delete"
                    @click=${() => m(this, b, Y).call(this, e.name)}
                  >
                    <uui-icon name="icon-delete"></uui-icon>
                  </uui-button>
                </div>
              </uui-table-cell>
            </uui-table-row>
          `;
      }
    )}
      </uui-table>
    `;
  }
};
h = /* @__PURE__ */ new WeakMap();
b = /* @__PURE__ */ new WeakSet();
S = async function() {
  if (y(this, h)) {
    this._loading = !0;
    try {
      this._backups = await y(this, h).getBackups();
    } catch {
      this._backups = [];
    }
    this._loading = !1;
  }
};
Y = async function(e) {
  if (y(this, h) && confirm(`Delete backup '${e}'? This cannot be undone.`))
    try {
      await y(this, h).deleteBackup(e), this.dispatchEvent(
        new CustomEvent("backup-deleted", { detail: { name: e }, bubbles: !0, composed: !0 })
      ), await m(this, b, S).call(this);
    } catch {
      this.dispatchEvent(
        new CustomEvent("backup-error", {
          detail: { message: "Failed to delete backup" },
          bubbles: !0,
          composed: !0
        })
      );
    }
};
j = function(e) {
  try {
    return new Date(e).toLocaleString();
  } catch {
    return e;
  }
};
w.styles = D`
    :host {
      display: block;
    }
    .empty-state {
      text-align: center;
      padding: var(--uui-size-space-6, 24px);
      color: var(--uui-color-text-alt, #6b7280);
    }
    .actions {
      display: flex;
      gap: 4px;
    }
    .extension-badge {
      display: inline-block;
      padding: 2px 6px;
      border-radius: 3px;
      font-size: 0.7rem;
      font-weight: 600;
      background: var(--uui-color-surface-alt, #f3f4f6);
      color: var(--uui-color-text-alt, #6b7280);
    }
    uui-table {
      width: 100%;
    }
  `;
N([
  n()
], w.prototype, "_backups", 2);
N([
  n()
], w.prototype, "_loading", 2);
w = N([
  E("backups-history-table")
], w);
var me = Object.defineProperty, ke = Object.getOwnPropertyDescriptor, ee = (e) => {
  throw TypeError(e);
}, W = (e, a, t, s) => {
  for (var i = s > 1 ? void 0 : s ? ke(a, t) : a, o = e.length - 1, r; o >= 0; o--)
    (r = e[o]) && (i = (s ? r(a, t, i) : r(i)) || i);
  return s && i && me(a, t, i), i;
}, R = (e, a, t) => a.has(e) || ee("Cannot " + t), ye = (e, a, t) => (R(e, a, "read from private field"), t ? t.call(e) : a.get(e)), X = (e, a, t) => a.has(e) ? ee("Cannot add the same private member more than once") : a instanceof WeakSet ? a.add(e) : a.set(e, t), we = (e, a, t, s) => (R(e, a, "write to private field"), a.set(e, t), t), d = (e, a, t) => (R(e, a, "access private method"), t), C, u, te, ae, ie, T, z;
let $ = class extends A(P) {
  constructor() {
    super(...arguments), X(this, u), this._notificationMessage = "", this._notificationType = "", X(this, C);
  }
  connectedCallback() {
    super.connectedCallback(), we(this, C, new ne(this)), this.provideContext(M, ye(this, C));
  }
  render() {
    return l`
      <h2>Backups</h2>
      <p class="description">
        Create and manage quick backups from the backoffice.
      </p>

      ${this._notificationMessage ? l`
            <uui-toast-notification
              color=${this._notificationType}
              auto-close="4000"
            >
              <uui-toast-notification-layout headline="Backups">
                ${this._notificationMessage}
              </uui-toast-notification-layout>
            </uui-toast-notification>
          ` : ""}

      <backups-create-modal
        @backup-created=${d(this, u, ae)}
        @backup-error=${d(this, u, T)}
      ></backups-create-modal>

      <div style="margin-top: var(--uui-size-space-6, 24px);">
        <div class="section-header">
          <h3 class="section-title">Backup History</h3>
        </div>
        <backups-history-table
          @backup-deleted=${d(this, u, ie)}
          @backup-error=${d(this, u, T)}
        ></backups-history-table>
      </div>
    `;
  }
};
C = /* @__PURE__ */ new WeakMap();
u = /* @__PURE__ */ new WeakSet();
te = function() {
  return this.renderRoot.querySelector("backups-history-table");
};
ae = function() {
  var e;
  d(this, u, z).call(this, "Backup started successfully.", "positive"), (e = d(this, u, te).call(this)) == null || e.refresh();
};
ie = function() {
  d(this, u, z).call(this, "Backup deleted.", "positive");
};
T = function(e) {
  d(this, u, z).call(this, e.detail.message, "danger");
};
z = function(e, a) {
  this._notificationMessage = e, this._notificationType = a, setTimeout(() => {
    this._notificationMessage = "", this._notificationType = "";
  }, 4e3);
};
$.styles = D`
    :host {
      display: block;
      padding: var(--uui-size-layout-1, 24px);
    }
    h2 {
      font-size: 1.5rem;
      font-weight: 600;
      margin: 0 0 var(--uui-size-space-2, 4px);
    }
    .description {
      color: var(--uui-color-text-alt, #6b7280);
      margin: 0 0 var(--uui-size-space-6, 24px);
    }
    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: var(--uui-size-space-5, 16px);
    }
    .section-title {
      font-size: 1rem;
      font-weight: 600;
      margin: 0;
    }
  `;
W([
  n()
], $.prototype, "_notificationMessage", 2);
W([
  n()
], $.prototype, "_notificationType", 2);
$ = W([
  E("backups-dashboard")
], $);
export {
  M as BACKUPS_CONTEXT_TOKEN,
  ne as BackupsContext,
  c as BackupsCreateModalElement,
  $ as BackupsDashboardElement,
  w as BackupsHistoryTableElement,
  k as BackupsProviderBadgeElement
};
