import { html as _, when as y, css as F, property as w, customElement as P } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as C } from "@umbraco-cms/backoffice/lit-element";
import "@umbraco-cms/backoffice/server-file-system";
import "@umbraco-cms/backoffice/tree";
import "./email-template-tree.store.js";
import { l as $ } from "./index.js";
var A = Object.defineProperty, M = Object.getOwnPropertyDescriptor, g = (e, t, r, i) => {
  for (var a = i > 1 ? void 0 : i ? M(t, r) : t, o = e.length - 1, p; o >= 0; o--)
    (p = e[o]) && (a = (i ? p(t, r, a) : p(a)) || a);
  return i && a && A(t, r, a), a;
}, m = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, d = (e, t, r) => (m(e, t, "read from private field"), t.get(e)), n = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, O = (e, t, r, i) => (m(e, t, "write to private field"), t.set(e, r), r), c = (e, t, r) => (m(e, t, "access private method"), r), l, h, E, u, S, v, T;
const W = "forms-email-template-picker-property-editor", f = "Forms/Emails";
let s = class extends C {
  constructor() {
    super(...arguments), n(this, h), n(this, u), n(this, v), n(this, l, "");
  }
  get value() {
    return d(this, l) ?? "";
  }
  set value(e) {
    const t = d(this, l);
    O(this, l, e), this.requestUpdate("value", t);
  }
  render() {
    return _`
        ${y(
      this.value.length > 0,
      () => _`<div>Selected template: <strong>${c(this, v, T).call(this)}</strong></div>`
    )}
        <umb-tree
          id="EmailTemplate"
          alias="${$}"
          .props=${{
      hideTreeRoot: !1,
      selectionConfiguration: {
        multiple: !1,
        selection: c(this, u, S).call(this)
      },
      selectableFilter: (e) => !e.isFolder
    }}
          @selection-change=${c(this, h, E)}
        ></umb-tree>`;
  }
};
l = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakSet();
E = function(e) {
  const r = e.target.getSelection();
  r.length > 0 ? this.value = f + r[0].replace("%25dot%25", ".").replace("%2F", "/") : this.value = "", this.dispatchEvent(new CustomEvent("property-value-change"));
};
u = /* @__PURE__ */ new WeakSet();
S = function() {
  return this.value.length === 0 ? [] : [this.value.replace(f, "").replace(".", "%25dot%25").replace("/", "%2F")];
};
v = /* @__PURE__ */ new WeakSet();
T = function() {
  return this.value.replace(f + "/", "");
};
s.styles = [
  F`

		`
];
g([
  w({ type: String })
], s.prototype, "value", 1);
s = g([
  P(W)
], s);
const V = s;
export {
  s as FormsEmailTemplatePickerPropertyUiElement,
  V as default
};
//# sourceMappingURL=email-template-picker-property-editor.element.js.map
