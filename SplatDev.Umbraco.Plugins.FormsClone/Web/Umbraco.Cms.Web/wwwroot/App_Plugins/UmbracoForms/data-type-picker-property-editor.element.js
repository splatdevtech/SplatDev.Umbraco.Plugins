import { html as d, state as _, property as y, customElement as f } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as w } from "@umbraco-cms/backoffice/lit-element";
import { i as P } from "./index.js";
var C = Object.defineProperty, g = Object.getOwnPropertyDescriptor, l = (e, t, a, n) => {
  for (var r = n > 1 ? void 0 : n ? g(t, a) : t, o = e.length - 1, i; o >= 0; o--)
    (i = e[o]) && (r = (n ? i(t, a, r) : i(r)) || r);
  return n && r && C(t, a, r), r;
}, D = (e, t, a) => {
  if (!t.has(e))
    throw TypeError("Cannot " + a);
}, h = (e, t, a) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, a);
}, u = (e, t, a) => (D(e, t, "access private method"), a), p, v, c, m;
const E = "forms-data-type-picker-property-editor";
let s = class extends w {
  constructor() {
    super(...arguments), h(this, p), h(this, c), this._options = [], this.value = "";
  }
  async connectedCallback() {
    super.connectedCallback(), await u(this, p, v).call(this);
  }
  render() {
    return d`<uui-select
      label="Data type picker"
      @change=${u(this, c, m)}
      .options=${this._options.map((e) => ({
      name: e.value,
      value: e.id,
      selected: e.id === this.value
    }))}
    >
    </uui-select>`;
  }
};
p = /* @__PURE__ */ new WeakSet();
v = async function() {
  const e = await P.getPickerDataType();
  this._options = e;
};
c = /* @__PURE__ */ new WeakSet();
m = function(e) {
  const t = e.target.value.toString();
  t !== this.value && (this.value = t, this.dispatchEvent(new CustomEvent("property-value-change")));
};
l([
  _()
], s.prototype, "_options", 2);
l([
  y()
], s.prototype, "value", 2);
s = l([
  f(E)
], s);
const O = s;
export {
  s as FormsDataTypePickerPropertyUiElement,
  O as default
};
//# sourceMappingURL=data-type-picker-property-editor.element.js.map
