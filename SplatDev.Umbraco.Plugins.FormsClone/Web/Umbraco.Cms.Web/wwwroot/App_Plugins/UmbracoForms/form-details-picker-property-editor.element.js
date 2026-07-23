import { html as d, when as y, property as T, state as h, customElement as g } from "@umbraco-cms/backoffice/external/lit";
import { splitStringToArray as $ } from "@umbraco-cms/backoffice/utils";
import { UmbLitElement as C } from "@umbraco-cms/backoffice/lit-element";
var E = Object.defineProperty, I = Object.getOwnPropertyDescriptor, o = (e, t, r, i) => {
  for (var l = i > 1 ? void 0 : i ? I(t, r) : t, u = e.length - 1, m; u >= 0; u--)
    (m = e[u]) && (l = (i ? m(t, r, l) : m(l)) || l);
  return i && l && E(t, r, l), l;
}, P = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, f = (e, t, r) => (P(e, t, "read from private field"), t.get(e)), n = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, b = (e, t, r, i) => (P(e, t, "write to private field"), t.set(e, r), r), p = (e, t, r) => (P(e, t, "access private method"), r), s, c, w, _, k, v, F;
const R = "forms-form-details-property-editor";
let a = class extends C {
  constructor() {
    super(...arguments), n(this, c), n(this, _), n(this, v), n(this, s, {
      formId: null,
      theme: null,
      redirectToPageId: null
    }), this._includeThemePicker = !1, this._includeRedirectPicker = !1, this._allowedForms = [], this._allowedFolders = [];
  }
  set value(e) {
    b(this, s, e ? structuredClone(e) : f(this, s));
  }
  get value() {
    return f(this, s);
  }
  set config(e) {
    this._includeThemePicker = (e == null ? void 0 : e.getValueByAlias("includeThemePicker")) || !1, this._includeRedirectPicker = (e == null ? void 0 : e.getValueByAlias("includeRedirectPicker")) || !1, this._allowedFolders = (e == null ? void 0 : e.getValueByAlias("allowedFolders")) || [], this._allowedForms = (e == null ? void 0 : e.getValueByAlias("allowedForms")) || [];
  }
  render() {
    var e;
    return d`
      <umb-property-layout
        alias="form"
        .label=${this.localize.term("formPicker_form")}
      >
        <forms-input-form
          id="form"
          .selection=${(e = this.value) != null && e.formId ? $(this.value.formId) : []}
          .multiple=${!1}
          .allowedFolders=${this._allowedFolders}
          .allowedForms=${this._allowedForms}
          @change=${p(this, c, w)}
          slot="editor"></forms-input-form>
      </umb-property-layout>

      ${y(
      this._includeThemePicker,
      () => {
        var t;
        return d`
          <umb-property-layout
            alias="theme"
            .label=${this.localize.term("formPicker_theme")}
          >
            <forms-input-theme
                  @change=${p(this, _, k)}
                  .value=${((t = this.value) == null ? void 0 : t.theme) || ""}
                  slot="editor"></forms-input-theme>
          </umb-property-layout>`;
      }
    )}

      ${y(
      this._includeRedirectPicker,
      () => {
        var t;
        return d`
          <umb-property-layout
            alias="theme"
            .label=${this.localize.term("formPicker_redirectToPage")}
            .description=${this.localize.term("formPicker_redirectToPageDescription")}
          >
          <umb-input-document
              slot="editor"
              .value=${((t = this.value) == null ? void 0 : t.redirectToPageId) || ""}
              max="1"
              @change=${p(this, v, F)}
            ></umb-input-document>
          </umb-property-layout>        `;
      }
    )}
      `;
  }
};
s = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
w = function(e) {
  const t = e.target;
  this.value.formId = t.selection.length > 0 ? t.selection[0] : null, this.dispatchEvent(new CustomEvent("property-value-change"));
};
_ = /* @__PURE__ */ new WeakSet();
k = function(e) {
  const t = e.target;
  this.value.theme = t.value, this.dispatchEvent(new CustomEvent("property-value-change"));
};
v = /* @__PURE__ */ new WeakSet();
F = function(e) {
  const t = e.target.value;
  this.value.redirectToPageId = t !== "" ? t : null, this.dispatchEvent(new CustomEvent("property-value-change"));
};
o([
  T({ type: Object })
], a.prototype, "value", 1);
o([
  h()
], a.prototype, "_includeThemePicker", 2);
o([
  h()
], a.prototype, "_includeRedirectPicker", 2);
o([
  h()
], a.prototype, "_allowedForms", 2);
o([
  h()
], a.prototype, "_allowedFolders", 2);
a = o([
  g(R)
], a);
const O = a;
export {
  a as FormsFormDetailsPickerPropertyPickerElement,
  O as default
};
//# sourceMappingURL=form-details-picker-property-editor.element.js.map
