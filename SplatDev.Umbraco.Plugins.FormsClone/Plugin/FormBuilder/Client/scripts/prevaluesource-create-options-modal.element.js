import { html as p, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as w } from "@umbraco-cms/backoffice/modal";
import { FormsPrevalueSourceTypeCollectionRepository as y } from "./prevaluesource-type-collection.repository.js";
var C = Object.defineProperty, $ = Object.getOwnPropertyDescriptor, P = (e, t, r, o) => {
  for (var i = o > 1 ? void 0 : o ? $(t, r) : t, s = e.length - 1, n; s >= 0; s--)
    (n = e[s]) && (i = (o ? n(t, r, i) : n(i)) || i);
  return o && i && C(t, r, i), i;
}, m = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, S = (e, t, r) => (m(e, t, "read from private field"), r ? r.call(e) : t.get(e)), l = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, b = (e, t, r, o) => (m(e, t, "write to private field"), t.set(e, r), r), f = (e, t, r) => (m(e, t, "access private method"), r), a, c, d, u, _;
const M = "forms-prevaluesource-create-options-modal";
let h = class extends w {
  constructor() {
    super(), l(this, c), l(this, u), l(this, a, []), f(this, c, d).call(this);
  }
  render() {
    return p`<umb-body-layout headline=${this.localize.term("formPrevalueSources_choosePrevalueSourceType")}>
      <uui-box>
        <uui-ref-list>
          ${S(this, a).map(
      (e) => p`<umb-ref-item
                selectable
                .name=${e.name}
                .detail=${e.description}
                .icon=${e.icon}
                @click=${() => f(this, u, _).call(this, e.id)}
              >
              </umb-ref-item>`
    )}
        </uui-ref-list>
      </uui-box>
      <uui-button
        slot="actions"
        label=${this.localize.term("general_cancel")}
        @click=${this._rejectModal}
      ></uui-button>
    </umb-body-layout>`;
  }
};
a = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
d = async function() {
  const e = new y(this), { data: t } = await e.requestCollection();
  b(this, a, (t == null ? void 0 : t.items) || []), this.requestUpdate();
};
u = /* @__PURE__ */ new WeakSet();
_ = function(e) {
  const t = `section/forms/workspace/prevaluesource/create/${e}`;
  window.history.pushState({}, "", t), this._submitModal();
};
h = P([
  v(M)
], h);
const x = h;
export {
  h as FormsPrevalueSourceCreateOptionsModalElement,
  x as default
};
//# sourceMappingURL=prevaluesource-create-options-modal.element.js.map
