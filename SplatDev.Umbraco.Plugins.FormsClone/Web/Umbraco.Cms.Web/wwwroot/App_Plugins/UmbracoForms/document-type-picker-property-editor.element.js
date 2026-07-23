import { html as d, state as _, property as y, customElement as f } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as w } from "@umbraco-cms/backoffice/lit-element";
import { i as P } from "./index.js";
var C = Object.defineProperty, g = Object.getOwnPropertyDescriptor, l = (e, t, r, o) => {
  for (var a = o > 1 ? void 0 : o ? g(t, r) : t, s = e.length - 1, i; s >= 0; s--)
    (i = e[s]) && (a = (o ? i(t, r, a) : i(a)) || a);
  return o && a && C(t, r, a), a;
}, D = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, u = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, h = (e, t, r) => (D(e, t, "access private method"), r), c, m, p, v;
const E = "forms-document-type-picker-property-editor";
let n = class extends w {
  constructor() {
    super(...arguments), u(this, c), u(this, p), this._options = [], this.value = "";
  }
  async connectedCallback() {
    super.connectedCallback(), await h(this, c, m).call(this);
  }
  render() {
    return d`<uui-select
      label="Document type picker"
      @change=${h(this, p, v)}
      .options=${this._options.map((e) => ({
      name: e.value,
      value: e.id,
      selected: e.id === this.value
    }))}
    >
    </uui-select>`;
  }
};
c = /* @__PURE__ */ new WeakSet();
m = async function() {
  const e = await P.getPickerDocumentType();
  this._options = e;
};
p = /* @__PURE__ */ new WeakSet();
v = function(e) {
  const t = e.target.value.toString();
  t !== this.value && (this.value = t, this.dispatchEvent(new CustomEvent("property-value-change")));
};
l([
  _()
], n.prototype, "_options", 2);
l([
  y()
], n.prototype, "value", 2);
n = l([
  f(E)
], n);
const O = n;
export {
  n as FormsDocumentTypePickerPropertyUiElement,
  O as default
};
//# sourceMappingURL=document-type-picker-property-editor.element.js.map
