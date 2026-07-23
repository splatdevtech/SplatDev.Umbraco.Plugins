import { html as _, customElement as f } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as y } from "@umbraco-cms/backoffice/modal";
import { UMB_ACTION_EVENT_CONTEXT as v } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as E } from "@umbraco-cms/backoffice/entity-action";
import { W as C, a7 as S } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/id";
import "@umbraco-cms/backoffice/resources";
var w = Object.defineProperty, T = Object.getOwnPropertyDescriptor, U = (t, e, r, i) => {
  for (var a = i > 1 ? void 0 : i ? T(e, r) : e, n = t.length - 1, l; n >= 0; n--)
    (l = t[n]) && (a = (i ? l(e, r, a) : l(a)) || a);
  return i && a && w(e, r, a), a;
}, m = (t, e, r) => {
  if (!e.has(t))
    throw TypeError("Cannot " + r);
}, c = (t, e, r) => (m(t, e, "read from private field"), r ? r.call(t) : e.get(t)), u = (t, e, r) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, r);
}, b = (t, e, r, i) => (m(t, e, "write to private field"), e.set(t, r), r), M = (t, e, r) => (m(t, e, "access private method"), r), o, s, d, h;
let p = class extends y {
  constructor() {
    super(...arguments), u(this, d), u(this, o, void 0), u(this, s, new C(this));
  }
  async connectedCallback() {
    var e;
    if (super.connectedCallback(), !((e = this.data) != null && e.unique))
      return;
    const { data: t } = await c(this, s).requestByUnique(this.data.unique);
    b(this, o, t), this.requestUpdate();
  }
  render() {
    var t;
    return _`
			<umb-body-layout headline="${this.localize.term("formSecurity_deleteTitle", (t = c(this, o)) == null ? void 0 : t.name)}">
				<uui-box>
					<p>${this.localize.term("formSecurity_deleteUserRecordNote")}</p>
				</uui-box>
				<uui-button slot="actions" id="cancel" label=${this.localize.term("general_cancel")} @click="${this._rejectModal}">${this.localize.term("general_cancel")}</uui-button>
				<uui-button slot="actions" type="button" label=${this.localize.term("general_delete")} look="primary" color="danger" @click=${M(this, d, h)}></uui-button>
			</umb-body-layout>
		`;
  }
};
o = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakSet();
h = async function(t) {
  var e;
  if (t.stopPropagation(), (e = this.data) != null && e.unique) {
    await c(this, s).delete(this.data.unique);
    const r = new E({
      entityType: S,
      unique: "207c2294-970b-4e1f-82fd-ae8996ef171d"
      // Matches Umbraco.Forms.Web.Controllers.ManagementApi.Security.Tree.SecurityTreeControllerBase.UsersFolderId
    });
    (await this.getContext(v)).dispatchEvent(r);
  }
  this._submitModal();
};
p = U([
  f("forms-user-security-delete-confirm-modal")
], p);
const k = p;
export {
  p as FormsUserSecurityDeleteConfirmModalElement,
  k as default
};
//# sourceMappingURL=user-security-delete-confirm-modal.element.js.map
