import { html as u, property as d, customElement as h } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as m } from "@umbraco-cms/backoffice/lit-element";
var v = Object.defineProperty, f = Object.getOwnPropertyDescriptor, i = (t, e, r, n) => {
  for (var o = n > 1 ? void 0 : n ? f(e, r) : e, s = t.length - 1, l; s >= 0; s--)
    (l = t[s]) && (o = (n ? l(e, r, o) : l(o)) || o);
  return n && o && v(e, r, o), o;
}, _ = (t, e, r) => {
  if (!e.has(t))
    throw TypeError("Cannot " + r);
}, P = (t, e, r) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, r);
}, y = (t, e, r) => (_(t, e, "access private method"), r), c, p;
const C = "forms-folder-picker-multiple-property-editor";
let a = class extends m {
  constructor() {
    super(...arguments), P(this, c), this.value = [];
  }
  async connectedCallback() {
    super.connectedCallback();
  }
  render() {
    return u`<forms-input-folder
      .selection=${this.value}
      .multiple=${!0}
      @change=${y(this, c, p)}
      slot="editor"></forms-input-folder>`;
  }
};
c = /* @__PURE__ */ new WeakSet();
p = function(t) {
  const e = t.target;
  this.value = e.selection, this.dispatchEvent(new CustomEvent("property-value-change"));
};
i([
  d({ type: Array })
], a.prototype, "value", 2);
a = i([
  h(C)
], a);
const k = a;
export {
  a as FormsFolderPickerMultiplePropertyPickerElement,
  k as default
};
//# sourceMappingURL=folder-picker-multiple-property-editor.element.js.map
