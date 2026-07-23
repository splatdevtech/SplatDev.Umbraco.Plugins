import { LitElement as v, html as _, when as h, customElement as w } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as E } from "@umbraco-cms/backoffice/element-api";
import { c as O, J as d } from "./index.js";
var u = Object.defineProperty, C = Object.getOwnPropertyDescriptor, M = (e, t, r, s) => {
  for (var a = s > 1 ? void 0 : s ? C(t, r) : t, i = e.length - 1, o; i >= 0; i--)
    (o = e[i]) && (a = (s ? o(t, r, a) : o(a)) || a);
  return s && a && u(t, r, a), a;
}, f = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, l = (e, t, r) => (f(e, t, "read from private field"), r ? r.call(e) : t.get(e)), m = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, S = (e, t, r, s) => (f(e, t, "write to private field"), t.set(e, r), r), n, c;
const F = "workspace-view-form-entries";
let p = class extends E(v) {
  constructor() {
    super(), m(this, n, void 0), m(this, c, {
      pageSize: 10
    }), this.consumeContext(O, (e) => {
      S(this, n, e);
    });
  }
  render() {
    var e;
    return _`
      ${h(
      !((e = l(this, n)) != null && e.getIsNew()),
      () => _`<umb-collection
        alias=${d}
        .config=${l(this, c)}
      ></umb-collection>`
    )}`;
  }
};
n = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakMap();
p = M([
  w(F)
], p);
const x = p;
export {
  p as UmbWorkspaceViewFormEntriesElement,
  x as default
};
//# sourceMappingURL=workspace-view-form-entries.element.js.map
