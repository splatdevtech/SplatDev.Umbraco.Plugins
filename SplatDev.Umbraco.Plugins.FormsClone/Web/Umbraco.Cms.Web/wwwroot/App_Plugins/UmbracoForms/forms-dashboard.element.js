import { UmbElementMixin as g } from "@umbraco-cms/backoffice/element-api";
import { LitElement as S, html as a, when as h, css as $, state as n, customElement as w } from "@umbraco-cms/backoffice/external/lit";
import { L as D, U as O, S as L } from "./index.js";
var k = Object.defineProperty, x = Object.getOwnPropertyDescriptor, o = (s, e, t, l) => {
  for (var r = l > 1 ? void 0 : l ? x(e, t) : e, c = s.length - 1, u; c >= 0; c--)
    (u = s[c]) && (r = (l ? u(e, t, r) : u(r)) || r);
  return l && r && k(e, t, r), r;
}, z = (s, e, t) => {
  if (!e.has(s))
    throw TypeError("Cannot " + t);
}, m = (s, e, t) => {
  if (e.has(s))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(s) : e.set(s, t);
}, d = (s, e, t) => (z(s, e, "access private method"), t), _, f, p, y, v, b;
const C = "forms-dashboard";
let i = class extends g(S) {
  constructor() {
    super(...arguments), m(this, _), m(this, p), m(this, v), this._showOverlay = !1;
  }
  async connectedCallback() {
    super.connectedCallback();
    let s;
    [this._status, this._currentVersion, s] = await Promise.all([
      D.getLicensingStatus(),
      O.getUpdatesVersion(),
      L.getSecurityUserCurrentFormSecurity({
        includeFormFieldDetails: !1
      })
    ]), this._userSecurity = s.userSecurity;
  }
  render() {
    var s, e, t;
    return a`
      ${h(
      this._showOverlay && ((s = this._userSecurity) == null ? void 0 : s.manageForms),
      () => a` <div class="umb-forms-install-overlay">
          <uui-button
            @click=${() => this._showOverlay = !1}
            .label=${this.localize.term("general_close")}
          ></uui-button>

          <div>
            <div class="succes"></div>

            <h2>
              <strong
                >${this.localize.term(
        "formsDashboard_installOverlayTitle"
      )}</strong
              >
            </h2>

            <p>
              ${this.localize.term("formsDashboard_installOverlayDescription")}
            </p>

            <div class="divider"></div>

            <uui-button
              color="positive"
              look="default"
              @click=${d(this, _, f)}
              label=${this.localize.term("formsDashboard_createAForm")}
            >
            </uui-button>
          </div>
        </div>`
    )}

      <forms-licensing .status=${this._status}></forms-licensing>

      ${h(
      this._currentVersion,
      () => a`<small
          >${this.localize.term(
        "formsDashboard_currentVersion",
        this._currentVersion
      )}</small
        >`
    )}

      <form-grid .config=${this._userSecurity}></form-grid>

      ${h(
      !((e = this._status) != null && e.isTrial) && !((t = this._status) != null && t.isInvalid) && !d(this, p, y).call(this),
      () => a`<small
            >${this.localize.term("formsDashboard_licensedDomains")}:</small
          >
          <small>${d(this, v, b).call(this)}</small>`
    )}
    `;
  }
};
_ = /* @__PURE__ */ new WeakSet();
f = function() {
};
p = /* @__PURE__ */ new WeakSet();
y = function() {
  var s;
  return ((s = this._status) == null ? void 0 : s.licenseLimitations.includes(
    "*not* associated with any ips or domains"
  )) ?? !1;
};
v = /* @__PURE__ */ new WeakSet();
b = function() {
  var s;
  return (s = this._status) != null && s.validDomains ? a`<ul>
      ${this._status.validDomains.split("|").map((e) => a`<li>${e}</li>`)}
    </ul>` : null;
};
i.styles = [
  $`
      :host {
        display: block;
        padding: var(--uui-size-layout-1);
      }

      @media (min-width: 1024px) {
        uui-box {
          display: block;
          max-width: calc(100% - 300px);
        }
      }

      form-grid {
        display: block;
        margin-bottom: 20px;
      }
    `
];
o([
  n()
], i.prototype, "_showOverlay", 2);
o([
  n()
], i.prototype, "_userSecurity", 2);
o([
  n()
], i.prototype, "_currentVersion", 2);
o([
  n()
], i.prototype, "_status", 2);
i = o([
  w(C)
], i);
const V = i;
export {
  i as FormsDashboardElement,
  V as default
};
//# sourceMappingURL=forms-dashboard.element.js.map
