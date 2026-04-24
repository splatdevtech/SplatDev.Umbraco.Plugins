import { html as x, css as z, state as K, property as L, customElement as X } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as B } from "@umbraco-cms/backoffice/lit-element";
import { c as N } from "./index.js";
var q = Object.defineProperty, G = Object.getOwnPropertyDescriptor, T = (t, e, s, u) => {
  for (var r = u > 1 ? void 0 : u ? G(e, s) : e, M = t.length - 1, k; M >= 0; M--)
    (k = t[M]) && (r = (u ? k(e, s, r) : k(r)) || r);
  return u && r && q(e, s, r), r;
}, $ = (t, e, s) => {
  if (!e.has(t))
    throw TypeError("Cannot " + s);
}, l = (t, e, s) => ($(t, e, "read from private field"), s ? s.call(t) : e.get(t)), a = (t, e, s) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, s);
}, h = (t, e, s, u) => ($(t, e, "write to private field"), e.set(t, s), s), i = (t, e, s) => ($(t, e, "access private method"), s), p, S, n, E, I, g, F, W, D, y, U, w, O, P, V, f, m, o, _, R, b, d, v;
const H = "text-with-field-picker-property-editor", C = "FREE_TEXT", A = "FIELD_PICKER";
let c = class extends B {
  constructor() {
    super(), a(this, E), a(this, g), a(this, W), a(this, y), a(this, w), a(this, P), a(this, f), a(this, o), a(this, R), a(this, d), a(this, p, void 0), a(this, S, []), this.inputMode = "", a(this, n, ""), this.consumeContext(N, (t) => {
      h(this, p, t), i(this, E, I).call(this);
    });
  }
  get value() {
    return this.inputMode = i(this, o, _).call(this, l(this, n)) ? A : C, l(this, n);
  }
  set value(t) {
    const e = l(this, n);
    h(this, n, t), this.requestUpdate("value", e);
  }
  render() {
    return x`
      <uui-button-group>
        <uui-button label="Free text" look=${i(this, d, v).call(this) ? "primary" : "secondary"} color="default" @click=${() => i(this, g, F).call(this, C)}></uui-button>
        <uui-button label="Select field" look=${i(this, d, v).call(this) ? "secondary" : "primary"} color="default" @click=${() => i(this, g, F).call(this, A)}></uui-button>
      </uui-button-group>
      ${i(this, d, v).call(this) ? x`
          <div class="text-with-field-picker-margin-div">
            <uui-input
              type="text"
              .value=${l(this, n)}
              @change=${(t) => i(this, W, D).call(this, t)}>
            </uui-input>
          </div>
        ` : x`
          <div class="text-with-field-picker-margin-div">
            <uui-select
              .options=${l(this, S).map((t) => ({
      name: t.caption,
      value: t.alias,
      selected: t.alias === i(this, f, m).call(this, l(this, n))
    })) ?? []}
              placeholder="Select a field"
              @change=${(t) => i(this, y, U).call(this, t)}>
            </uui-select>
          </div>
        `}
    `;
  }
};
p = /* @__PURE__ */ new WeakMap();
S = /* @__PURE__ */ new WeakMap();
n = /* @__PURE__ */ new WeakMap();
E = /* @__PURE__ */ new WeakSet();
I = function() {
  h(this, S, l(this, p).getAllFields());
};
g = /* @__PURE__ */ new WeakSet();
F = function(t) {
  this.inputMode = t;
};
W = /* @__PURE__ */ new WeakSet();
D = function(t) {
  const e = t.target.value.toString();
  i(this, R, b).call(this, e) ? h(this, n, i(this, f, m).call(this, e)) : h(this, n, e), i(this, w, O).call(this);
};
y = /* @__PURE__ */ new WeakSet();
U = function(t) {
  h(this, n, i(this, P, V).call(this, t.target.value.toString())), i(this, w, O).call(this);
};
w = /* @__PURE__ */ new WeakSet();
O = function() {
  this.dispatchEvent(new CustomEvent("property-value-change"));
};
P = /* @__PURE__ */ new WeakSet();
V = function(t) {
  return t ? i(this, o, _).call(this, t) ? t : `{${t}}` : "";
};
f = /* @__PURE__ */ new WeakSet();
m = function(t) {
  return i(this, o, _).call(this, t) ? t.substring(1, l(this, n).length - 1) : t;
};
o = /* @__PURE__ */ new WeakSet();
_ = function(t) {
  const e = new RegExp(/^{\w+}$/gi);
  return !!t && e.test(t);
};
R = /* @__PURE__ */ new WeakSet();
b = function(t) {
  return i(this, o, _).call(this, t) ? l(this, p).getAllFields().map((s) => s.alias).includes(i(this, f, m).call(this, t)) : !1;
};
d = /* @__PURE__ */ new WeakSet();
v = function() {
  return this.inputMode === C;
};
c.styles = [
  z`
      .text-with-field-picker-margin-div{
        margin-top: var(--uui-size-3);
      }
    `
];
T([
  K()
], c.prototype, "inputMode", 2);
T([
  L()
], c.prototype, "value", 1);
c = T([
  X(H)
], c);
const Z = c;
export {
  c as FormsTextWithFieldPickerPropertyUiElement,
  Z as default
};
//# sourceMappingURL=text-with-field-picker-property-editor.element.js.map
