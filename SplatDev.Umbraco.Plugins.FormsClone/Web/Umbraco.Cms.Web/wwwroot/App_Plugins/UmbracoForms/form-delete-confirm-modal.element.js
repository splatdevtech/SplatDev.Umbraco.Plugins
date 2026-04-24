import { html as c, customElement as R } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as w } from "@umbraco-cms/backoffice/modal";
import { UMB_ACTION_EVENT_CONTEXT as E } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as M } from "@umbraco-cms/backoffice/entity-action";
import { v as q, a1 as y, a2 as T } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import "@umbraco-cms/backoffice/id";
var $ = Object.defineProperty, O = Object.getOwnPropertyDescriptor, C = (t, e, a, o) => {
  for (var r = o > 1 ? void 0 : o ? O(e, a) : e, l = t.length - 1, h; l >= 0; l--)
    (h = t[l]) && (r = (o ? h(e, a, r) : h(r)) || r);
  return o && r && $(e, a, r), r;
}, v = (t, e, a) => {
  if (!e.has(t))
    throw TypeError("Cannot " + a);
}, i = (t, e, a) => (v(t, e, "read from private field"), a ? a.call(t) : e.get(t)), m = (t, e, a) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, a);
}, p = (t, e, a, o) => (v(t, e, "write to private field"), e.set(t, a), a), F = (t, e, a) => (v(t, e, "access private method"), a), u, n, d, s, _, D;
let f = class extends w {
  constructor() {
    super(...arguments), m(this, _), m(this, u, void 0), m(this, n, void 0), m(this, d, void 0), m(this, s, new q(this));
  }
  async connectedCallback() {
    var o;
    if (super.connectedCallback(), !((o = this.data) != null && o.unique))
      return;
    const t = await i(this, s).requestByUnique(this.data.unique);
    p(this, u, t.data);
    const e = await i(this, s).requestRecordsMetaData(this.data.unique);
    p(this, n, e.data);
    const a = await i(this, s).requestHasRelations(this.data.unique);
    p(this, d, a.data), this.requestUpdate();
  }
  render() {
    var t, e, a, o;
    return c`
			<umb-body-layout headline=${this.localize.term("formDelete_modalHeadline")}>
				<uui-box>
					<p>${this.localize.term("formDelete_title", (t = i(this, u)) == null ? void 0 : t.name)}</p>
					<p><i>${this.localize.term("formDelete_timingNote")}</i></p>
					<p>${this.localize.term("formDelete_recordDeleteNote")}</p>
					<p>${((e = i(this, n)) == null ? void 0 : e.count) == 0 ? c`${this.localize.term("formDelete_noRecordDetail")}` : c`${this.localize.term("formDelete_recordDetail", (a = i(this, n)) == null ? void 0 : a.count, (o = i(this, n)) == null ? void 0 : o.lastSubmittedDate)}`}</p>
					<p>${i(this, d) ? c`${this.localize.term("formDelete_deleteWarning")}` : c`${this.localize.term("formDelete_noReferencedMessage")}`}</p>
				</uui-box>
				<uui-button slot="actions" id="cancel" label=${this.localize.term("general_cancel")} @click="${this._rejectModal}">${this.localize.term("general_cancel")}</uui-button>
				<uui-button slot="actions" type="button" label=${this.localize.term("general_delete")} look="primary" color="danger" @click=${F(this, _, D)}></uui-button>
			</umb-body-layout>
		`;
  }
};
u = /* @__PURE__ */ new WeakMap();
n = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakSet();
D = async function(t) {
  var e;
  if (t.stopPropagation(), (e = this.data) != null && e.unique) {
    const { data: a } = await i(this, s).requestByUnique(this.data.unique);
    if (!a)
      throw new Error("Item not found.");
    await i(this, s).delete(this.data.unique);
    const o = await this.getContext(E), r = {
      entityType: a.parentUnique ? y : T,
      unique: a.parentUnique || null
    }, l = new M(r);
    o.dispatchEvent(l);
  }
  this._submitModal();
};
f = C([
  R("forms-form-delete-confirm-modal")
], f);
const x = f;
export {
  f as FormsFormDeleteConfirmModalElement,
  x as default
};
//# sourceMappingURL=form-delete-confirm-modal.element.js.map
