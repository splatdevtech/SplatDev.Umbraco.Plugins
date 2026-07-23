var d = (e, i, t) => {
  if (!i.has(e))
    throw TypeError("Cannot " + t);
};
var n = (e, i, t) => (d(e, i, "read from private field"), t ? t.call(e) : i.get(e)), m = (e, i, t) => {
  if (i.has(e))
    throw TypeError("Cannot add the same private member more than once");
  i instanceof WeakSet ? i.add(e) : i.set(e, t);
}, p = (e, i, t, s) => (d(e, i, "write to private field"), s ? s.call(e, t) : i.set(e, t), t);
import { UMB_ACTION_EVENT_CONTEXT as l } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadStructureForEntityEvent as v } from "@umbraco-cms/backoffice/entity-action";
import { UmbObjectState as b } from "@umbraco-cms/backoffice/observable-api";
import { UmbSubmittableWorkspaceContextBase as q } from "@umbraco-cms/backoffice/workspace";
import "@umbraco-cms/backoffice/class-api";
import "@umbraco-cms/backoffice/resources";
import { d as g } from "./index.js";
import "@umbraco-cms/backoffice/current-user";
var o, h, c;
class F extends q {
  constructor(t, s, a, u, y) {
    super(t, s);
    m(this, o, void 0);
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    m(this, h, void 0);
    m(this, c, void 0);
    this._data = new b(void 0), this.data = this._data.asObservable(), this.unique = this._data.asObservablePart((r) => r == null ? void 0 : r.unique), this.name = this._data.asObservablePart((r) => r == null ? void 0 : r.name), this.id = this._data.asObservablePart((r) => r == null ? void 0 : r.unique), p(this, o, new u(this)), p(this, c, a), this.routes.setRoutes([
      {
        path: "edit/:unique",
        component: y,
        setup: (r, _) => {
          const f = _.match.params.unique;
          this.load(f);
        }
      }
    ]);
  }
  getData() {
    return this._data.getValue();
  }
  getUnique() {
    var t;
    return ((t = this.getData()) == null ? void 0 : t.unique) || "";
  }
  getEntityType() {
    return n(this, c);
  }
  setProperty(t, s) {
    this._data.value && this._data.update({ [t]: s });
  }
  async load(t) {
    var a;
    p(this, h, (a = n(this, o)) == null ? void 0 : a.requestByUnique(t));
    const { data: s } = await n(this, h);
    s && (this.setIsNew(!1), this._data.update(s));
  }
  async submit() {
    var u;
    if (!this._data.value || !this._data.value.unique)
      return;
    const t = await this.getContext(l);
    await ((u = n(this, o)) == null ? void 0 : u.save(this._data.value)), await (await this.getContext(g)).getUserSecurity();
    const a = new v({
      unique: this.getUnique(),
      entityType: this.getEntityType()
    });
    t.dispatchEvent(a);
  }
  toggleFormSecurityAccess(t) {
    if (!this._data.value)
      return;
    const s = this._data.value.formsSecurity.map((a) => ({
      ...a,
      ...a.form === t && {
        hasAccess: !a.hasAccess
      }
    }));
    this._data.update({ formsSecurity: s });
  }
  setFormSecurityAccess(t, s) {
    if (!this._data.value)
      return;
    const a = this._data.value.formsSecurity.map((u) => ({
      ...u,
      ...u.form === t && {
        hasAccess: s
      }
    }));
    this._data.update({ formsSecurity: a });
  }
  destroy() {
    this._data.destroy();
  }
}
o = new WeakMap(), h = new WeakMap(), c = new WeakMap();
export {
  F
};
//# sourceMappingURL=security-workspace-base.context.js.map
