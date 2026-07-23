import { UmbTextStyles as w } from "@umbraco-cms/backoffice/style";
import { LitElement as y, html as p, when as u, css as F, state as S, customElement as C } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as E } from "@umbraco-cms/backoffice/element-api";
import { c as O, d as x } from "./index.js";
import "@umbraco-cms/backoffice/class-api";
import "@umbraco-cms/backoffice/resources";
import "@umbraco-cms/backoffice/observable-api";
import "@umbraco-cms/backoffice/current-user";
var M = Object.defineProperty, P = Object.getOwnPropertyDescriptor, g = (t, e, s, o) => {
  for (var r = o > 1 ? void 0 : o ? P(e, s) : e, l = t.length - 1, f; l >= 0; l--)
    (f = t[l]) && (r = (o ? f(e, s, r) : f(r)) || r);
  return o && r && M(e, s, r), r;
}, h = (t, e, s) => {
  if (!e.has(t))
    throw TypeError("Cannot " + s);
}, m = (t, e, s) => (h(t, e, "read from private field"), s ? s.call(t) : e.get(t)), d = (t, e, s) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, s);
}, v = (t, e, s, o) => (h(t, e, "write to private field"), e.set(t, s), s), T = (t, e, s) => (h(t, e, "access private method"), s), i, n, c, _;
const W = "workspace-view-form-settings";
let a = class extends E(y) {
  constructor() {
    super(), d(this, c), d(this, i, void 0), d(this, n, !1), this.consumeContext(O, (t) => {
      v(this, i, t), T(this, c, _).call(this);
    }), this.consumeContext(x, (t) => {
      t && this.observe(t.config, (e) => {
        e && v(this, n, e == null ? void 0 : e.enableMultiPageFormSettings);
      });
    });
  }
  render() {
    var t, e;
    return p`<uui-box>
      <forms-settings-store-records></forms-settings-store-records>
      <forms-settings-captions></forms-settings-captions>
      <forms-settings-styling></forms-settings-styling>
      <forms-settings-validation></forms-settings-validation>
      <forms-settings-autocomplete></forms-settings-autocomplete>
      ${u(
      m(this, n),
      () => p`<forms-settings-multi-page></forms-settings-multi-page>`
    )}
      <forms-settings-moderation></forms-settings-moderation>
      <forms-settings-fields-display
        .formFields=${((t = m(this, i)) == null ? void 0 : t.getAllFields()) ?? []}
      ></forms-settings-fields-display>
      ${u(
      (e = this._form) == null ? void 0 : e.storeRecordsLocally,
      () => p`<forms-settings-data-retention></forms-settings-data-retention>`
    )}
    </uui-box>`;
  }
};
i = /* @__PURE__ */ new WeakMap();
n = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
_ = function() {
  m(this, i) && this.observe(m(this, i).data, (t) => {
    t && (this._form = t);
  });
};
a.styles = [
  w,
  F`
      :host {
        display: block;
        padding: var(--uui-size-layout-1);
      }
    `
];
g([
  S()
], a.prototype, "_form", 2);
a = g([
  C(W)
], a);
const L = a;
export {
  a as UmbWorkspaceViewFormSettingsElement,
  L as default
};
//# sourceMappingURL=workspace-view-form-settings.element.js.map
