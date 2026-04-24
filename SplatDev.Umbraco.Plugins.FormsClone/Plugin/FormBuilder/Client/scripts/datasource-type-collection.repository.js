var n = (e, t, o) => {
  if (!t.has(e))
    throw TypeError("Cannot " + o);
};
var c = (e, t, o) => (n(e, t, "read from private field"), o ? o.call(e) : t.get(e)), s = (e, t, o) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, o);
}, i = (e, t, o, l) => (n(e, t, "write to private field"), l ? l.call(e, o) : t.set(e, o), o);
import { tryExecuteAndNotify as u } from "@umbraco-cms/backoffice/resources";
import { _ as y } from "./index.js";
var r;
class m {
  constructor(t) {
    s(this, r, void 0);
    i(this, r, t);
  }
  async getCollection() {
    const { data: t, error: o } = await u(c(this, r), y.getDataSourceType());
    return o ? { error: o } : t ? { data: { items: t, total: t.length } } : { data: { items: [], total: 0 } };
  }
}
r = new WeakMap();
var a;
class f {
  constructor(t) {
    s(this, a, void 0);
    i(this, a, new m(t));
  }
  async requestCollection() {
    return c(this, a).getCollection();
  }
  destroy() {
  }
}
a = new WeakMap();
export {
  f as FormsDataSourceTypeCollectionRepository,
  f as default
};
//# sourceMappingURL=datasource-type-collection.repository.js.map
