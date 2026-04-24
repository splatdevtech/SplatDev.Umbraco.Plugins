import { X as d, Y as v, Z as S } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/id";
import "@umbraco-cms/backoffice/resources";
import { UmbElementMixin as f } from "@umbraco-cms/backoffice/element-api";
import { LitElement as y, html as E, css as U, state as C, property as R, customElement as w } from "@umbraco-cms/backoffice/external/lit";
import { F as O } from "./security-user-group-workspace.context-token.js";
import { F as G } from "./security-workspace-base.context.js";
var F = Object.defineProperty, P = Object.getOwnPropertyDescriptor, c = (e, r, t, s) => {
  for (var o = s > 1 ? void 0 : s ? P(r, t) : r, p = e.length - 1, n; p >= 0; p--)
    (n = e[p]) && (o = (s ? n(r, t, o) : n(o)) || o);
  return s && o && F(r, t, o), o;
}, _ = (e, r, t) => {
  if (!r.has(e))
    throw TypeError("Cannot " + t);
}, m = (e, r, t) => (_(e, r, "read from private field"), r.get(e)), l = (e, r, t) => {
  if (r.has(e))
    throw TypeError("Cannot add the same private member more than once");
  r instanceof WeakSet ? r.add(e) : r.set(e, t);
}, T = (e, r, t, s) => (_(e, r, "write to private field"), r.set(e, t), t), k = (e, r, t) => (_(e, r, "access private method"), t), i, u, h;
const x = "forms-security-user-group-workspace-editor";
let a = class extends f(y) {
  constructor() {
    super(), l(this, u), this._name = "", l(this, i, void 0), this.consumeContext(O, (e) => {
      T(this, i, e), k(this, u, h).call(this);
    });
  }
  render() {
    return E` <umb-workspace-editor alias="Forms.Workspace.Security.UserGroup">
      <uui-input
        id="name"
        slot="header"
        readonly
        label="User Group"
        .value=${this.localize.term("formSecurity_manageIndividualFormsLabel") + " " + this._name}
      ></uui-input>
    </umb-workspace-editor>`;
  }
};
i = /* @__PURE__ */ new WeakMap();
u = /* @__PURE__ */ new WeakSet();
h = function() {
  m(this, i) && this.observe(m(this, i).data, (e) => this._name = (e == null ? void 0 : e.name) ?? "");
};
a.styles = [
  U`
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
c([
  C()
], a.prototype, "_name", 2);
c([
  R({ type: String, attribute: !1 })
], a.prototype, "workspaceAlias", 2);
a = c([
  w(x)
], a);
class W extends G {
  constructor(r) {
    super(
      r,
      d,
      v,
      S,
      a
    );
  }
  setUserGroupSecurityProperty(r, t) {
    if (!this._data.value)
      return;
    const s = structuredClone(
      this._data.value.userGroupSecurity
    );
    s[r] = t, this._data.update({ userGroupSecurity: s });
  }
}
const N = W;
export {
  W as FormsSecurityUserGroupWorkspaceContext,
  N as api
};
//# sourceMappingURL=security-user-group-workspace.context.js.map
