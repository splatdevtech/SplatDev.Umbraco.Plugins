var T = (e, r, t) => {
  if (!r.has(e))
    throw TypeError("Cannot " + t);
};
var i = (e, r, t) => (T(e, r, "read from private field"), t ? t.call(e) : r.get(e)), m = (e, r, t) => {
  if (r.has(e))
    throw TypeError("Cannot add the same private member more than once");
  r instanceof WeakSet ? r.add(e) : r.set(e, t);
}, l = (e, r, t, a) => (T(e, r, "write to private field"), a ? a.call(e, t) : r.set(e, t), t);
import { UmbSubmittableWorkspaceContextBase as O } from "@umbraco-cms/backoffice/workspace";
import { UmbObjectState as D } from "@umbraco-cms/backoffice/observable-api";
import { UMB_ACTION_EVENT_CONTEXT as R } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as N, UmbRequestReloadStructureForEntityEvent as q } from "@umbraco-cms/backoffice/entity-action";
import { j as b, m as I, n as U, o as A, p as x, q as P } from "./index.js";
import { UmbElementMixin as F } from "@umbraco-cms/backoffice/element-api";
import { LitElement as W, html as k, css as M, state as z, property as B, customElement as V } from "@umbraco-cms/backoffice/external/lit";
import { UMB_NOTIFICATION_CONTEXT as Y } from "@umbraco-cms/backoffice/notification";
import { UmbLocalizationController as $ } from "@umbraco-cms/backoffice/localization-api";
var L = Object.defineProperty, X = Object.getOwnPropertyDescriptor, _ = (e, r, t, a) => {
  for (var o = a > 1 ? void 0 : a ? X(r, t) : r, n = e.length - 1, h; n >= 0; n--)
    (h = e[n]) && (o = (a ? h(r, t, o) : h(o)) || o);
  return a && o && L(r, t, o), o;
}, v = (e, r, t) => {
  if (!r.has(e))
    throw TypeError("Cannot " + t);
}, f = (e, r, t) => (v(e, r, "read from private field"), r.get(e)), d = (e, r, t) => {
  if (r.has(e))
    throw TypeError("Cannot add the same private member more than once");
  r instanceof WeakSet ? r.add(e) : r.set(e, t);
}, K = (e, r, t, a) => (v(e, r, "write to private field"), r.set(e, t), t), w = (e, r, t) => (v(e, r, "access private method"), t), u, y, g, S, C;
const G = "forms-datasource-workspace-editor";
let c = class extends F(W) {
  constructor() {
    super(), d(this, y), d(this, S), this._dataSourceName = "", d(this, u, void 0), this.consumeContext(b, (e) => {
      K(this, u, e), w(this, y, g).call(this);
    });
  }
  render() {
    return k` <umb-workspace-editor alias="Forms.Workspace.DataSources">
      <uui-input
        slot="header"
        id="nameInput"
        label=${this.localize.term("placeholders_entername")}
        placeholder=${this.localize.term("placeholders_entername")}
        required
        .value=${this._dataSourceName}
        @input="${w(this, S, C)}"
      ></uui-input>
    </umb-workspace-editor>`;
  }
};
u = /* @__PURE__ */ new WeakMap();
y = /* @__PURE__ */ new WeakSet();
g = function() {
  f(this, u) && this.observe(f(this, u).data, (e) => this._dataSourceName = (e == null ? void 0 : e.name) ?? "");
};
S = /* @__PURE__ */ new WeakSet();
C = function(e) {
  var r;
  (r = f(this, u)) == null || r.setName(e.target.value.toString());
};
c.styles = [
  M`
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
_([
  z()
], c.prototype, "_dataSourceName", 2);
_([
  B({ type: String, attribute: !1 })
], c.prototype, "workspaceAlias", 2);
c = _([
  V(G)
], c);
const E = c;
var s, p;
class j extends O {
  constructor(t) {
    super(t, I);
    m(this, s, void 0);
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    m(this, p, void 0);
    this.dataSourceRepository = new U(this), this.dataSourceTypeRepository = new A(this), l(this, s, new D(void 0)), this.data = i(this, s).asObservable(), this.unique = i(this, s).asObservablePart((a) => a == null ? void 0 : a.unique), this.name = i(this, s).asObservablePart((a) => a == null ? void 0 : a.name), this.id = i(this, s).asObservablePart((a) => a == null ? void 0 : a.unique), this.routes.setRoutes([
      {
        path: "create/:type",
        component: E,
        setup: async (a, o) => {
          const n = o.match.params.type;
          await this.create(n);
        }
      },
      {
        path: "edit/:unique",
        component: E,
        setup: (a, o) => {
          const n = o.match.params.unique;
          this.load(n);
        }
      }
    ]);
  }
  async load(t) {
    l(this, p, this.dataSourceRepository.requestByUnique(t));
    const { data: a } = await i(this, p);
    a && (this.setIsNew(!1), i(this, s).update(a));
  }
  async create(t) {
    let o = (await this.dataSourceRepository.requestDataSourceScaffold()).data;
    return o.formDataSourceTypeId = t, this.modalContext && (o = { ...o, ...this.modalContext.data.preset }), this.setIsNew(!0), i(this, s).setValue(o), { data: o };
  }
  async submit() {
    if (!i(this, s).value || !i(this, s).value.unique)
      return;
    if (i(this, s).value.name.trim().length === 0) {
      const a = await this.getContext(
        Y
      ), o = new $(this);
      a.peek("danger", {
        data: { message: o.term("formEdit_noNameForForm") }
      });
      return;
    }
    const t = await this.getContext(R);
    if (this.getIsNew()) {
      await this.dataSourceRepository.create(i(this, s).value, null);
      const a = new N({
        entityType: x,
        unique: null
      });
      t.dispatchEvent(a), this.setIsNew(!1);
    } else {
      await this.dataSourceRepository.save(i(this, s).value);
      const a = new q({
        unique: this.getUnique(),
        entityType: this.getEntityType()
      });
      t.dispatchEvent(a);
    }
  }
  async loadDataSourceType(t) {
    const { data: a } = await this.dataSourceTypeRepository.requestByUnique(t);
    return a;
  }
  getData() {
    return i(this, s).getValue();
  }
  getUnique() {
    var t;
    return ((t = this.getData()) == null ? void 0 : t.unique) || "";
  }
  getDataSourceTypeId() {
    var t;
    return ((t = this.getData()) == null ? void 0 : t.formDataSourceTypeId) || "";
  }
  getEntityType() {
    return P;
  }
  getName() {
    var t;
    return (t = i(this, s).getValue()) == null ? void 0 : t.name;
  }
  setName(t) {
    i(this, s).update({ name: t });
  }
  setDataSourceProperty(t, a) {
    i(this, s).update({ [t]: a });
  }
  getDataSourceProperty(t) {
    var a;
    return (a = this.getData()) == null ? void 0 : a[t];
  }
  async getWizardScaffold() {
    const { data: t } = await this.dataSourceRepository.requestDataSourceWizardScaffold(this.getUnique());
    return t;
  }
  destroy() {
    i(this, s).destroy();
  }
}
s = new WeakMap(), p = new WeakMap();
const it = j;
export {
  j as FormsDataSourceWorkspaceContext,
  it as api
};
//# sourceMappingURL=datasource-workspace.context.js.map
