var d = (t, s, e) => {
  if (!s.has(t))
    throw TypeError("Cannot " + e);
};
var i = (t, s, e) => (d(t, s, "read from private field"), e ? e.call(t) : s.get(t)), h = (t, s, e) => {
  if (s.has(t))
    throw TypeError("Cannot add the same private member more than once");
  s instanceof WeakSet ? s.add(t) : s.set(t, e);
}, f = (t, s, e, r) => (d(t, s, "write to private field"), r ? r.call(t, e) : s.set(t, e), e);
var y = (t, s, e) => (d(t, s, "access private method"), e);
import { UmbSubmittableWorkspaceContextBase as q } from "@umbraco-cms/backoffice/workspace";
import { UmbObjectState as O } from "@umbraco-cms/backoffice/observable-api";
import { UMB_ACTION_EVENT_CONTEXT as U } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as I, UmbRequestReloadStructureForEntityEvent as x } from "@umbraco-cms/backoffice/entity-action";
import { K as k, M as F, N as A, O as V, k as W } from "./index.js";
import "@umbraco-cms/backoffice/resources";
import { UmbElementMixin as D } from "@umbraco-cms/backoffice/element-api";
import { LitElement as M, html as L, css as B, state as z, property as Y, customElement as $ } from "@umbraco-cms/backoffice/external/lit";
import { F as K } from "./prevaluesource-workspace.context-token.js";
import { UMB_NOTIFICATION_CONTEXT as X } from "@umbraco-cms/backoffice/notification";
import { UmbLocalizationController as G } from "@umbraco-cms/backoffice/localization-api";
var j = Object.defineProperty, H = Object.getOwnPropertyDescriptor, T = (t, s, e, r) => {
  for (var a = r > 1 ? void 0 : r ? H(s, e) : s, u = t.length - 1, n; u >= 0; u--)
    (n = t[u]) && (a = (r ? n(s, e, a) : n(a)) || a);
  return r && a && j(s, e, a), a;
}, g = (t, s, e) => {
  if (!s.has(t))
    throw TypeError("Cannot " + e);
}, S = (t, s, e) => (g(t, s, "read from private field"), s.get(t)), _ = (t, s, e) => {
  if (s.has(t))
    throw TypeError("Cannot add the same private member more than once");
  s instanceof WeakSet ? s.add(t) : s.set(t, e);
}, J = (t, s, e, r) => (g(t, s, "write to private field"), s.set(t, e), e), C = (t, s, e) => (g(t, s, "access private method"), e), l, E, N, w, b;
const Q = "forms-prevaluesource-workspace-editor";
let c = class extends D(M) {
  constructor() {
    super(), _(this, E), _(this, w), this._prevalueSourceName = "", _(this, l, void 0), this.consumeContext(K, (t) => {
      J(this, l, t), C(this, E, N).call(this);
    });
  }
  render() {
    return L`
      <umb-workspace-editor alias="Forms.Workspace.PrevalueSources">
        <uui-input
          slot="header"
          id="nameInput"
          label=${this.localize.term("placeholders_entername")}
          placeholder=${this.localize.term("placeholders_entername")}
          required
          .value=${this._prevalueSourceName}
          @input="${C(this, w, b)}"
        ></uui-input>
      </umb-workspace-editor>`;
  }
};
l = /* @__PURE__ */ new WeakMap();
E = /* @__PURE__ */ new WeakSet();
N = function() {
  S(this, l) && this.observe(S(this, l).data, (t) => this._prevalueSourceName = (t == null ? void 0 : t.name) ?? "");
};
w = /* @__PURE__ */ new WeakSet();
b = function(t) {
  var s;
  (s = S(this, l)) == null || s.setName(t.target.value.toString());
};
c.styles = [
  B`
        :host {
            display: block;
            width: 100%;
            height: 100%;
        }

        #nameInput {
            flex: 1 1 auto;
        }
    `
];
T([
  z()
], c.prototype, "_prevalueSourceName", 2);
T([
  Y({ type: String, attribute: !1 })
], c.prototype, "workspaceAlias", 2);
c = T([
  $(Q)
], c);
const R = c;
var o, m, p, v, P;
class Z extends q {
  constructor(e) {
    super(e, k);
    h(this, v);
    h(this, o, void 0);
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    h(this, m, void 0);
    h(this, p, void 0);
    this.prevalueSourceRepository = new F(this), this.prevalueSourceTypeRepository = new A(this), f(this, o, new O(void 0)), this.data = i(this, o).asObservable(), this.unique = i(this, o).asObservablePart((r) => r == null ? void 0 : r.unique), this.name = i(this, o).asObservablePart((r) => r == null ? void 0 : r.name), this.id = i(this, o).asObservablePart((r) => r == null ? void 0 : r.unique), f(this, p, new O([])), this.prevalues = i(this, p).asObservable(), this.routes.setRoutes([
      {
        path: "create/:type",
        component: R,
        setup: async (r, a) => {
          const u = a.match.params.type;
          await this.create(u);
        }
      },
      {
        path: "edit/:unique",
        component: R,
        setup: (r, a) => {
          const u = a.match.params.unique;
          this.load(u);
        }
      }
    ]);
  }
  async load(e) {
    var a;
    f(this, m, (a = this.prevalueSourceRepository) == null ? void 0 : a.requestByUnique(e));
    const { data: r } = await i(this, m);
    r && (this.setIsNew(!1), i(this, o).update(r), await y(this, v, P).call(this, r.unique));
  }
  async create(e) {
    var u;
    let a = (await ((u = this.prevalueSourceRepository) == null ? void 0 : u.requestPrevalueSourceScaffold())).data;
    return a.fieldPreValueSourceTypeId = e, this.modalContext && (a = { ...a, ...this.modalContext.data.preset }), this.setIsNew(!0), i(this, o).setValue(a), { data: a };
  }
  async submit() {
    if (!i(this, o).value || !i(this, o).value.unique)
      return;
    if (i(this, o).value.name.trim().length === 0) {
      const u = await this.getContext(
        X
      ), n = new G(this);
      u.peek("danger", {
        data: { message: n.term("formEdit_noNameForForm") }
      });
      return;
    }
    const e = await this.getContext(U);
    if (this.getIsNew()) {
      const { error: u } = await this.prevalueSourceRepository.create(
        i(this, o).value,
        null
      );
      if (u)
        return;
      const n = new I({
        entityType: V,
        unique: null
      });
      e.dispatchEvent(n), this.setIsNew(!1);
      return;
    }
    const { error: r } = await this.prevalueSourceRepository.save(
      i(this, o).value
    );
    if (r)
      return;
    await y(this, v, P).call(this, i(this, o).value.unique);
    const a = new x({
      unique: this.getUnique(),
      entityType: this.getEntityType()
    });
    e.dispatchEvent(a);
  }
  async loadPrevalueSourceType(e) {
    const { data: r } = await this.prevalueSourceTypeRepository.requestByUnique(
      e
    );
    return r;
  }
  getData() {
    return i(this, o).getValue();
  }
  getPrevalues() {
    const e = i(this, p).getValue();
    return Object.keys(e).map((r) => e[r]);
  }
  getUnique() {
    var e;
    return ((e = this.getData()) == null ? void 0 : e.unique) || "";
  }
  getPrevalueSourceTypeId() {
    var e;
    return ((e = this.getData()) == null ? void 0 : e.fieldPreValueSourceTypeId) || "";
  }
  getEntityType() {
    return W;
  }
  getName() {
    var e;
    return (e = i(this, o).getValue()) == null ? void 0 : e.name;
  }
  setName(e) {
    i(this, o).update({ name: e });
  }
  setPrevalueSourceProperty(e, r) {
    i(this, o).update({ [e]: r });
  }
  getPrevalueSourceProperty(e) {
    var r;
    return (r = this.getData()) == null ? void 0 : r[e];
  }
  destroy() {
    i(this, o).destroy();
  }
}
o = new WeakMap(), m = new WeakMap(), p = new WeakMap(), v = new WeakSet(), P = async function(e) {
  const { data: r } = await this.prevalueSourceRepository.requestPrevalues(e);
  r && i(this, p).update(r);
};
const he = Z;
export {
  Z as FormsPrevalueSourceWorkspaceContext,
  he as api
};
//# sourceMappingURL=prevaluesource-workspace.context.js.map
