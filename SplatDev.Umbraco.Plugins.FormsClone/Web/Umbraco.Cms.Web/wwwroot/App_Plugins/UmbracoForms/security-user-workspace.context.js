import { T as d, V as v, W as S } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/id";
import "@umbraco-cms/backoffice/resources";
import { UmbElementMixin as f } from "@umbraco-cms/backoffice/element-api";
import { LitElement as y, html as E, css as C, state as w, property as R, customElement as U } from "@umbraco-cms/backoffice/external/lit";
import { F } from "./security-user-workspace.context-token.js";
import { F as T } from "./security-workspace-base.context.js";
var O = Object.defineProperty, W = Object.getOwnPropertyDescriptor, u = (e, t, r, s) => {
  for (var a = s > 1 ? void 0 : s ? W(t, r) : t, n = e.length - 1, p; n >= 0; n--)
    (p = e[n]) && (a = (s ? p(t, r, a) : p(a)) || a);
  return s && a && O(t, r, a), a;
}, m = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, _ = (e, t, r) => (m(e, t, "read from private field"), t.get(e)), l = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, k = (e, t, r, s) => (m(e, t, "write to private field"), t.set(e, r), r), x = (e, t, r) => (m(e, t, "access private method"), r), i, c, h;
const I = "forms-security-user-workspace-editor";
let o = class extends f(y) {
  constructor() {
    super(), l(this, c), this._name = "", l(this, i, void 0), this.consumeContext(F, (e) => {
      k(this, i, e), x(this, c, h).call(this);
    });
  }
  render() {
    return E` <umb-workspace-editor alias="Forms.Workspace.Security.User">
      <uui-input
        id="name"
        slot="header"
        readonly
        label=${this.localize.term("general_name")}
        .value=${this.localize.term("formSecurity_manageIndividualFormsLabel") + " " + this._name}
      ></uui-input>
    </umb-workspace-editor>`;
  }
};
i = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
h = function() {
  _(this, i) && this.observe(_(this, i).data, (e) => this._name = (e == null ? void 0 : e.name) ?? "");
};
o.styles = [
  C`
        :host {
            display: block;
            width: 100%;
            height: 100%;
        }

        #name {
            flex: 1 1 auto;
        }
    `
];
u([
  w()
], o.prototype, "_name", 2);
u([
  R({ type: String, attribute: !1 })
], o.prototype, "workspaceAlias", 2);
o = u([
  U(I)
], o);
class P extends T {
  constructor(t) {
    super(
      t,
      d,
      v,
      S,
      o
    );
  }
  setUserSecurityProperty(t, r) {
    if (!this._data.value)
      return;
    const s = structuredClone(this._data.value.userSecurity);
    s[t] = r, this._data.update({ userSecurity: s });
  }
}
const z = P;
export {
  P as FormsSecurityUserWorkspaceContext,
  z as api
};
//# sourceMappingURL=security-user-workspace.context.js.map
