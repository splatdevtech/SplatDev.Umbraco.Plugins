import { html as f, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as w } from "@umbraco-cms/backoffice/modal";
import { UMB_ACTION_EVENT_CONTEXT as m } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as y } from "@umbraco-cms/backoffice/entity-action";
import { n as E, p as C } from "./index.js";
var D = Object.defineProperty, b = Object.getOwnPropertyDescriptor, q = (t, e, a, r) => {
  for (var o = r > 1 ? void 0 : r ? b(e, a) : e, s = t.length - 1, l; s >= 0; s--)
    (l = t[s]) && (o = (r ? l(e, a, o) : l(o)) || o);
  return r && o && D(e, a, o), o;
}, p = (t, e, a) => {
  if (!e.has(t))
    throw TypeError("Cannot " + a);
}, n = (t, e, a) => (p(t, e, "read from private field"), a ? a.call(t) : e.get(t)), c = (t, e, a) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, a);
}, T = (t, e, a, r) => (p(t, e, "write to private field"), e.set(t, a), a), M = (t, e, a) => (p(t, e, "access private method"), a), u, i, d, _;
let h = class extends w {
  constructor() {
    super(...arguments), c(this, d), c(this, u, void 0), c(this, i, new E(this));
  }
  async connectedCallback() {
    var e;
    if (super.connectedCallback(), !((e = this.data) != null && e.unique))
      return;
    const t = await n(this, i).requestByUnique(this.data.unique);
    T(this, u, t.data), this.requestUpdate();
  }
  render() {
    var t;
    return f`
			<umb-body-layout headline=${this.localize.term("formDataSources_deleteDatasource")}>
				<uui-box>
					<p>Are you sure you wish to delete the datasource <b>${(t = n(this, u)) == null ? void 0 : t.name}</b>?</p>
				</uui-box>
				<uui-button slot="actions" id="cancel" label=${this.localize.term("general_cancel")} @click="${this._rejectModal}">${this.localize.term("general_cancel")}</uui-button>
				<uui-button slot="actions" type="button" label=${this.localize.term("general_delete")} look="primary" color="danger" @click=${M(this, d, _)}></uui-button>
			</umb-body-layout>
		`;
  }
};
u = /* @__PURE__ */ new WeakMap();
i = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakSet();
_ = async function(t) {
  var e, a;
  if (t.stopPropagation(), (e = this.data) != null && e.unique) {
    await n(this, i).delete(this.data.unique);
    const r = await this.getContext(m), o = {
      entityType: C,
      unique: null
    }, s = new y(o);
    r.dispatchEvent(s);
  }
  if (this._submitModal(), t.stopPropagation(), (a = this.data) != null && a.unique) {
    const { data: r } = await n(this, i).requestByUnique(this.data.unique);
    if (!r)
      throw new Error("Item not found.");
    await n(this, i).delete(this.data.unique), (await this.getContext(m)).dispatchEvent(t);
  }
  this._submitModal();
};
h = q([
  v("forms-datasource-delete-confirm-modal")
], h);
const P = h;
export {
  h as FormsDatasourceDeleteConfirmModalElement,
  P as default
};
//# sourceMappingURL=datasource-delete-confirm-modal.element.js.map
