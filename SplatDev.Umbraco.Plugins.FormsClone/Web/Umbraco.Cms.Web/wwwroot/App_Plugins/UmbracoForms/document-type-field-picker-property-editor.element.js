import { html as P, property as E, state as d, customElement as C } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as T } from "@umbraco-cms/backoffice/lit-element";
import { c as S, j as A, i as O } from "./index.js";
import { F } from "./prevaluesource-workspace.context-token.js";
import { tryExecuteAndNotify as w } from "@umbraco-cms/backoffice/resources";
var D = Object.defineProperty, R = Object.getOwnPropertyDescriptor, c = (e, t, i, a) => {
  for (var s = a > 1 ? void 0 : a ? R(t, i) : t, h = e.length - 1, u; h >= 0; h--)
    (u = e[h]) && (s = (a ? u(t, i, s) : u(s)) || s);
  return a && s && D(t, i, s), s;
}, W = (e, t, i) => {
  if (!t.has(e))
    throw TypeError("Cannot " + i);
}, l = (e, t, i) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, i);
}, r = (e, t, i) => (W(e, t, "access private method"), i), m, y, n, p, _, f, v, g;
const N = "forms-document-type-field-picker-property-editor";
let o = class extends T {
  constructor() {
    super(), l(this, m), l(this, n), l(this, _), l(this, v), this.value = "", this._settingProvidingDocTypeAlias = "", this._properties = [], r(this, m, y).call(this);
  }
  set config(e) {
    this._settingProvidingDocTypeAlias = (e == null ? void 0 : e.getValueByAlias("settingProvidingDocTypeAlias")) || "";
  }
  render() {
    return P`
      <uui-select
        id="field"
        @change=${r(this, v, g)}
        .options=${[
      ...["Id", "Key", "Name"].map((e) => ({
        name: e,
        value: e,
        group: this.localize.term("formPropertyEditors_standardFields"),
        selected: this.value === e || !this.value && e === "Name"
      })),
      ...this._properties.map((e) => ({
        name: e.value,
        value: e.id,
        group: this.localize.term("formPropertyEditors_customFields"),
        selected: this.value === e.id
      }))
    ]}
      >
      </uui-select>
    `;
  }
};
m = /* @__PURE__ */ new WeakSet();
y = function() {
  this.consumeContext(S, (e) => {
    e && r(this, n, p).call(this, e);
  }), this.consumeContext(A, (e) => {
    e && r(this, n, p).call(this, e);
  }), this.consumeContext(F, (e) => {
    e && r(this, n, p).call(this, e);
  });
};
n = /* @__PURE__ */ new WeakSet();
p = function(e) {
  this.observe(e.data, async (t) => {
    if (this._settingProvidingDocTypeAlias.length === 0)
      return;
    const i = t == null ? void 0 : t.settings[this._settingProvidingDocTypeAlias];
    i && await r(this, _, f).call(this, i);
  });
};
_ = /* @__PURE__ */ new WeakSet();
f = async function(e) {
  const { data: t } = await w(
    this,
    O.getPickerDocumentTypeByAliasProperties({ alias: e })
  );
  t && (this._properties = t);
};
v = /* @__PURE__ */ new WeakSet();
g = function(e) {
  const t = e.target.value.toString();
  t !== this.value && (this.value = t, this.dispatchEvent(new CustomEvent("property-value-change")));
};
c([
  E()
], o.prototype, "value", 2);
c([
  d()
], o.prototype, "_settingProvidingDocTypeAlias", 2);
c([
  d()
], o.prototype, "_properties", 2);
o = c([
  C(N)
], o);
const K = o;
export {
  o as FormsDocumentTypeFieldPickerPropertyUiElement,
  K as default
};
//# sourceMappingURL=document-type-field-picker-property-editor.element.js.map
