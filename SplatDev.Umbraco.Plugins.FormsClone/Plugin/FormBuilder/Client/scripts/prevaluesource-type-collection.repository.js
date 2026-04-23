var u = (t, e, r) => {
  if (!e.has(t))
    throw TypeError("Cannot " + r);
};
var a = (t, e, r) => (u(t, e, "read from private field"), r ? r.call(t) : e.get(t)), s = (t, e, r) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, r);
}, l = (t, e, r, i) => (u(t, e, "write to private field"), i ? i.call(t, r) : e.set(t, r), r);
import { tryExecuteAndNotify as n } from "@umbraco-cms/backoffice/resources";
import { $ as y } from "./index.js";
var o;
class m {
  constructor(e) {
    s(this, o, void 0);
    l(this, o, e);
  }
  async getCollection() {
    const { data: e, error: r } = await n(
      a(this, o),
      y.getPrevalueSourceType()
    );
    return r ? { error: r } : e ? { data: { items: e, total: e.length } } : { data: { items: [], total: 0 } };
  }
}
o = new WeakMap();
var c;
class f {
  constructor(e) {
    s(this, c, void 0);
    l(this, c, new m(e));
  }
  async requestCollection() {
    return a(this, c).getCollection();
  }
  destroy() {
  }
}
c = new WeakMap();
export {
  f as FormsPrevalueSourceTypeCollectionRepository,
  f as default
};
//# sourceMappingURL=prevaluesource-type-collection.repository.js.map
