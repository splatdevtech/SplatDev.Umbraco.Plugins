var i = (r, e, o) => {
  if (!e.has(r))
    throw TypeError("Cannot " + o);
};
var l = (r, e, o) => (i(r, e, "read from private field"), o ? o.call(r) : e.get(r)), u = (r, e, o) => {
  if (e.has(r))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(r) : e.set(r, o);
}, a = (r, e, o, s) => (i(r, e, "write to private field"), s ? s.call(r, o) : e.set(r, o), o);
import { tryExecuteAndNotify as n } from "@umbraco-cms/backoffice/resources";
import { A as S } from "./index.js";
var t;
class m {
  constructor(e) {
    u(this, t, void 0);
    a(this, t, e);
  }
  async getCollection(e) {
    const { data: o, error: s } = await n(l(this, t), S.getPrevalueSource(e));
    return o ? { data: o } : { error: s };
  }
}
t = new WeakMap();
var c;
class h {
  constructor(e) {
    u(this, c, void 0);
    a(this, c, new m(e));
  }
  async requestCollection(e) {
    return l(this, c).getCollection(e);
  }
  destroy() {
  }
}
c = new WeakMap();
export {
  h as FormsPrevalueSourceCollectionRepository,
  h as default
};
//# sourceMappingURL=prevaluesource-collection.repository.js.map
