import { html as _, property as f, customElement as m } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as w } from "@umbraco-cms/backoffice/lit-element";
var g = Object.defineProperty, y = Object.getOwnPropertyDescriptor, d = (t, e, r, s) => {
  for (var a = s > 1 ? void 0 : s ? y(e, r) : e, i = t.length - 1, p; i >= 0; i--)
    (p = t[i]) && (a = (s ? p(e, r, a) : p(a)) || a);
  return s && a && g(e, r, a), a;
}, u = (t, e, r) => {
  if (!e.has(t))
    throw TypeError("Cannot " + r);
}, l = (t, e, r) => (u(t, e, "read from private field"), e.get(t)), h = (t, e, r) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, r);
}, E = (t, e, r, s) => (u(t, e, "write to private field"), e.set(t, r), r), P = (t, e, r) => (u(t, e, "access private method"), r), n, v, c;
const C = "forms-password-property-editor";
let o = class extends w {
  constructor() {
    super(...arguments), h(this, v), h(this, n, "");
  }
  get value() {
    return l(this, n);
  }
  set value(t) {
    let e = l(this, n);
    E(this, n, t), this.requestUpdate("value", e);
  }
  render() {
    return _`<uui-input-password value="${this.value}" @change=${P(this, v, c)}></uui-input-password>`;
  }
};
n = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakSet();
c = function(t) {
  const e = t.target.value.toString();
  this.value = e, this.dispatchEvent(new CustomEvent("property-value-change"));
};
d([
  f({ type: String })
], o.prototype, "value", 1);
o = d([
  m(C)
], o);
const U = o;
export {
  o as FormsPasswordPropertyUiElement,
  U as default
};
//# sourceMappingURL=password-property-editor.element.js.map
