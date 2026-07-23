import { html as v, customElement as _ } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as f } from "@umbraco-cms/backoffice/modal";
import { UMB_ACTION_EVENT_CONTEXT as E } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as y } from "@umbraco-cms/backoffice/entity-action";
import "@umbraco-cms/backoffice/resources";
import { M as C, O as P } from "./index.js";
var S = Object.defineProperty, w = Object.getOwnPropertyDescriptor, T = (t, e, r, o) => {
  for (var a = o > 1 ? void 0 : o ? w(e, r) : e, n = t.length - 1, s; n >= 0; n--)
    (s = t[n]) && (a = (o ? s(e, r, a) : s(a)) || a);
  return o && a && S(e, r, a), a;
}, m = (t, e, r) => {
  if (!e.has(t))
    throw TypeError("Cannot " + r);
}, c = (t, e, r) => (m(t, e, "read from private field"), r ? r.call(t) : e.get(t)), u = (t, e, r) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, r);
}, O = (t, e, r, o) => (m(t, e, "write to private field"), e.set(t, r), r), b = (t, e, r) => (m(t, e, "access private method"), r), i, l, d, h;
let p = class extends f {
  constructor() {
    super(...arguments), u(this, d), u(this, i, void 0), u(this, l, new C(this));
  }
  async connectedCallback() {
    var e;
    if (super.connectedCallback(), !((e = this.data) != null && e.unique))
      return;
    const t = await c(this, l).requestByUnique(this.data.unique);
    O(this, i, t.data), this.requestUpdate();
  }
  render() {
    var t;
    return v`
			<umb-body-layout headline=${this.localize.term("formPrevalueSources_deletePrevalueSource")}>
				<uui-box>
					<p>${this.localize.term("formPrevalueSources_deleteConfirm", (t = c(this, i)) == null ? void 0 : t.name)}</p>
				</uui-box>
				<uui-button slot="actions" id="cancel" label=${this.localize.term("general_cancel")} @click="${this._rejectModal}">${this.localize.term("general_cancel")}</uui-button>
				<uui-button slot="actions" type="button" label=${this.localize.term("general_delete")} look="primary" color="danger" @click=${b(this, d, h)}></uui-button>
			</umb-body-layout>
		`;
  }
};
i = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakSet();
h = async function(t) {
  var e;
  if (t.stopPropagation(), (e = this.data) != null && e.unique) {
    await c(this, l).delete(this.data.unique);
    const r = await this.getContext(E), o = {
      entityType: P,
      unique: null
    }, a = new y(o);
    r.dispatchEvent(a);
  }
  this._submitModal();
};
p = T([
  _("forms-prevaluesource-delete-confirm-modal")
], p);
const U = p;
export {
  p as FormsPrevalueSourceDeleteConfirmModalElement,
  U as default
};
//# sourceMappingURL=prevaluesource-delete-confirm-modal.element.js.map
