import { UmbUfmElementBase as c, UMB_UFM_RENDER_CONTEXT as u, UmbUfmComponentBase as l } from "@umbraco-cms/backoffice/ufm";
import { property as h, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import { g as _ } from "./index.js";
import "@umbraco-cms/backoffice/id";
var d = Object.defineProperty, F = Object.getOwnPropertyDescriptor, f = (r, e, t, s) => {
  for (var o = s > 1 ? void 0 : s ? F(e, t) : e, a = r.length - 1, m; a >= 0; a--)
    (m = r[a]) && (o = (s ? m(e, t, o) : m(o)) || o);
  return s && o && d(e, t, o), o;
}, b = (r, e, t) => {
  if (!e.has(r))
    throw TypeError("Cannot " + t);
}, y = (r, e, t) => {
  if (e.has(r))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(r) : e.set(r, t);
}, C = (r, e, t) => (b(r, e, "access private method"), t), n, p;
const E = "ufm-form-name";
let i = class extends c {
  constructor() {
    super(), y(this, n), this.consumeContext(u, (r) => {
      this.observe(
        r.value,
        async (e) => {
          const t = C(this, n, p).call(this, e);
          if (t) {
            const s = new _(this), { data: o } = await s.requestItems([t]);
            o && o.length > 0 && (this.value = o[0].name);
          }
        },
        "observeValue"
      );
    });
  }
};
n = /* @__PURE__ */ new WeakSet();
p = function(r) {
  if (this.alias && typeof r == "object") {
    const t = r[this.alias];
    return t ? typeof t == "object" ? t.formId.toString() : t.toString() : void 0;
  }
};
f([
  h()
], i.prototype, "alias", 2);
i = f([
  v(E)
], i);
class N extends l {
  render(e) {
    return e.text ? `<ufm-form-name ${super.getAttributes(e.text)}></ufm-form-name>` : void 0;
  }
}
export {
  N as FormsUfmFormNameComponent,
  N as api
};
//# sourceMappingURL=form-name.component.js.map
