import { html as c, property as v, customElement as l } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as u } from "@umbraco-cms/backoffice/lit-element";
var f = Object.defineProperty, _ = Object.getOwnPropertyDescriptor, i = (t, e, r, o) => {
  for (var a = o > 1 ? void 0 : o ? _(e, r) : e, s = t.length - 1, p; s >= 0; s--)
    (p = t[s]) && (a = (o ? p(e, r, a) : p(a)) || a);
  return o && a && f(e, r, a), a;
}, d = (t, e, r) => {
  if (!e.has(t))
    throw TypeError("Cannot " + r);
}, y = (t, e, r) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, r);
}, E = (t, e, r) => (d(t, e, "access private method"), r), h, m;
const P = "forms-theme-picker-property-editor";
let n = class extends u {
  constructor() {
    super(...arguments), y(this, h), this.value = "";
  }
  render() {
    return c`<forms-input-theme
      @change=${E(this, h, m)}
      .value=${this.value}
      slot="editor"></forms-input-theme>`;
  }
};
h = /* @__PURE__ */ new WeakSet();
m = function(t) {
  const e = t.target;
  this.value = e.value, this.dispatchEvent(new CustomEvent("property-value-change"));
};
i([
  v()
], n.prototype, "value", 2);
n = i([
  l(P)
], n);
const w = n;
export {
  n as FormsThemePickerPropertyUiElement,
  w as default
};
//# sourceMappingURL=theme-picker-property-editor.element.js.map
