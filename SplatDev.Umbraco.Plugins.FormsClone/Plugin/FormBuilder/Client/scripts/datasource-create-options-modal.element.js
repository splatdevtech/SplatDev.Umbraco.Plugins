import { html as p, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as w } from "@umbraco-cms/backoffice/modal";
import { FormsDataSourceTypeCollectionRepository as y } from "./datasource-type-collection.repository.js";
var C = Object.defineProperty, $ = Object.getOwnPropertyDescriptor, b = (e, t, r, i) => {
  for (var a = i > 1 ? void 0 : i ? $(t, r) : t, s = e.length - 1, n; s >= 0; s--)
    (n = e[s]) && (a = (i ? n(t, r, a) : n(a)) || a);
  return i && a && C(t, r, a), a;
}, m = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, D = (e, t, r) => (m(e, t, "read from private field"), r ? r.call(e) : t.get(e)), c = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, M = (e, t, r, i) => (m(e, t, "write to private field"), t.set(e, r), r), f = (e, t, r) => (m(e, t, "access private method"), r), o, l, d, u, _;
const S = "forms-datasource-create-options-modal";
let h = class extends w {
  constructor() {
    super(), c(this, l), c(this, u), c(this, o, []), f(this, l, d).call(this);
  }
  render() {
    return p`<umb-body-layout headline=${this.localize.term("formDataSources_chooseDatasourceType")}>
        <uui-box>
          <uui-ref-list>
            ${D(this, o).map(
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
o = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakSet();
d = async function() {
  const e = new y(this), { data: t } = await e.requestCollection();
  M(this, o, (t == null ? void 0 : t.items) || []), this.requestUpdate();
};
u = /* @__PURE__ */ new WeakSet();
_ = function(e) {
  const t = `section/forms/workspace/datasource/create/${e}`;
  window.history.pushState({}, "", t), this._submitModal();
};
h = b([
  v(S)
], h);
const x = h;
export {
  h as FormsDataSourceCreateOptionsModalElement,
  x as default
};
//# sourceMappingURL=datasource-create-options-modal.element.js.map
