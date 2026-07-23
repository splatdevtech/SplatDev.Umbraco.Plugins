import { html as m, property as h, customElement as u } from "@umbraco-cms/backoffice/external/lit";
import { F as v } from "./form-picker-base.element.js";
var d = Object.defineProperty, f = Object.getOwnPropertyDescriptor, p = (r, e, t, s) => {
  for (var o = s > 1 ? void 0 : s ? f(e, t) : e, l = r.length - 1, i; l >= 0; l--)
    (i = r[l]) && (o = (s ? i(e, t, o) : i(o)) || o);
  return s && o && d(e, t, o), o;
}, _ = (r, e, t) => {
  if (!e.has(r))
    throw TypeError("Cannot " + t);
}, F = (r, e, t) => {
  if (e.has(r))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(r) : e.set(r, t);
}, P = (r, e, t) => (_(r, e, "access private method"), t), n, c;
const w = "forms-form-picker-multiple-property-editor";
let a = class extends v {
  constructor() {
    super(), F(this, n), this.value = [], this.allowMultiple = !0;
  }
  render() {
    return m`<forms-input-form
      .selection=${this.value}
      .multiple=${this.allowMultiple}
      .allowedFolders=${this.allowedFolders}
      .allowedForms=${this.allowedForms}
      @change=${P(this, n, c)}
      slot="editor"></forms-input-form>`;
  }
};
n = /* @__PURE__ */ new WeakSet();
c = function(r) {
  const e = r.target;
  this.value = e.selection, this.dispatchEvent(new CustomEvent("property-value-change"));
};
p([
  h({ type: Array })
], a.prototype, "value", 2);
a = p([
  u(w)
], a);
const C = a;
export {
  a as FormsFormPickerMultiplePropertyPickerElement,
  C as default
};
//# sourceMappingURL=form-picker-multiple-property-editor.element.js.map
