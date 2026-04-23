import { LitElement as _, html as f, css as h, state as w, customElement as x } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as E } from "@umbraco-cms/backoffice/element-api";
import { c as y } from "./index.js";
var C = Object.defineProperty, O = Object.getOwnPropertyDescriptor, l = (e, t, r, s) => {
  for (var a = s > 1 ? void 0 : s ? O(t, r) : t, n = e.length - 1, v; n >= 0; n--)
    (v = e[n]) && (a = (s ? v(t, r, a) : v(a)) || a);
  return s && a && C(t, r, a), a;
}, p = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, d = (e, t, r) => (p(e, t, "read from private field"), t.get(e)), u = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, F = (e, t, r, s) => (p(e, t, "write to private field"), t.set(e, r), r), W = (e, t, r) => (p(e, t, "access private method"), r), o, c, m;
const b = "workspace-view-form-advanced";
let i = class extends E(_) {
  constructor() {
    super(), u(this, c), u(this, o, void 0), this.consumeContext(y, (e) => {
      F(this, o, e), W(this, c, m).call(this);
    });
  }
  render() {
    return f`<uui-box>
      <forms-advanced-validation-rules></forms-advanced-validation-rules>
    </uui-box>`;
  }
};
o = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
m = function() {
  d(this, o) && this.observe(d(this, o).data, async (e) => {
    e && (this._form = e);
  });
};
i.styles = [
  h`
      :host {
        display: block;
        padding: var(--uui-size-layout-1);
      }

      uui-box + uui-box {
        margin-top: var(--uui-size-layout-1);
      }
    `
];
l([
  w()
], i.prototype, "_form", 2);
i = l([
  x(b)
], i);
const g = i;
export {
  i as UmbWorkspaceViewFormAdvancedElement,
  g as default
};
//# sourceMappingURL=workspace-view-form-advanced.element.js.map
