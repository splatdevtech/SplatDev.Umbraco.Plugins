import { html as m, property as h, customElement as f } from "@umbraco-cms/backoffice/external/lit";
import { splitStringToArray as v } from "@umbraco-cms/backoffice/utils";
import { F as d } from "./form-picker-base.element.js";
var u = Object.defineProperty, _ = Object.getOwnPropertyDescriptor, p = (r, e, t, s) => {
  for (var o = s > 1 ? void 0 : s ? _(e, t) : e, l = r.length - 1, n; l >= 0; l--)
    (n = r[l]) && (o = (s ? n(e, t, o) : n(o)) || o);
  return s && o && u(e, t, o), o;
}, F = (r, e, t) => {
  if (!e.has(r))
    throw TypeError("Cannot " + t);
}, P = (r, e, t) => {
  if (e.has(r))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(r) : e.set(r, t);
}, g = (r, e, t) => (F(r, e, "access private method"), t), i, c;
const w = "forms-form-picker-single-property-editor";
let a = class extends d {
  constructor() {
    super(), P(this, i), this.value = "", this.allowMultiple = !1;
  }
  render() {
    return m`<forms-input-form
      .selection=${v(this.value)}
      .multiple=${this.allowMultiple}
      .allowedFolders=${this.allowedFolders}
      .allowedForms=${this.allowedForms}
      @change=${g(this, i, c)}
      slot="editor"></forms-input-form>`;
  }
};
i = /* @__PURE__ */ new WeakSet();
c = function(r) {
  const e = r.target;
  this.value = e.selection.length > 0 ? e.selection[0] : "", this.dispatchEvent(new CustomEvent("property-value-change"));
};
p([
  h({ type: String })
], a.prototype, "value", 2);
a = p([
  f(w)
], a);
const k = a;
export {
  a as FormsFormPickerSinglePropertyPickerElement,
  k as default
};
//# sourceMappingURL=form-picker-single-property-editor.element.js.map
