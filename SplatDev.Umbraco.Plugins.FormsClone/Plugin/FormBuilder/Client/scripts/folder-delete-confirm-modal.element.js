import { html as s, customElement as E } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as y } from "@umbraco-cms/backoffice/modal";
import { UMB_ACTION_EVENT_CONTEXT as M } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as T } from "@umbraco-cms/backoffice/entity-action";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import { a4 as w, a1 as C, a2 as D } from "./index.js";
import "@umbraco-cms/backoffice/id";
var F = Object.defineProperty, O = Object.getOwnPropertyDescriptor, q = (e, t, a, l) => {
  for (var r = l > 1 ? void 0 : l ? O(t, a) : t, c = e.length - 1, p; c >= 0; c--)
    (p = e[c]) && (r = (l ? p(t, a, r) : p(r)) || r);
  return l && r && F(t, a, r), r;
}, _ = (e, t, a) => {
  if (!t.has(e))
    throw TypeError("Cannot " + a);
}, o = (e, t, a) => (_(e, t, "read from private field"), a ? a.call(e) : t.get(e)), u = (e, t, a) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, a);
}, f = (e, t, a, l) => (_(e, t, "write to private field"), t.set(e, a), a), R = (e, t, a) => (_(e, t, "access private method"), a), i, n, d, h, v;
let m = class extends y {
  constructor() {
    super(...arguments), u(this, h), u(this, i, void 0), u(this, n, void 0), u(this, d, new w(this));
  }
  async connectedCallback() {
    var a;
    if (super.connectedCallback(), !((a = this.data) != null && a.unique))
      return;
    const e = await o(this, d).requestByUnique(this.data.unique);
    f(this, i, e.data);
    const t = await o(this, d).isEmpty(this.data.unique);
    f(this, n, t.data), this.requestUpdate();
  }
  render() {
    var e;
    return s`
			<umb-body-layout headline=${this.localize.term("formDelete_folderDeleteModalHeadline")}>
				<uui-box>
					<p>${o(this, n) ? s`${this.localize.term("formDelete_folderDeleteModalHeadline", (e = o(this, i)) == null ? void 0 : e.name)}` : s`${this.localize.term("formDelete_folderNotEmptyMessage")}`}</p>
				</uui-box>
				<uui-button slot="actions" id="cancel" label=${this.localize.term("general_cancel")} @click="${this._rejectModal}">${this.localize.term("general_cancel")}</uui-button>
				${o(this, n) ? s`<uui-button slot="actions" type="button" label=${this.localize.term("general_delete")} look="primary" color="danger" @click=${R(this, h, v)}></uui-button>` : s``}
			</umb-body-layout>
		`;
  }
};
i = /* @__PURE__ */ new WeakMap();
n = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakSet();
v = async function(e) {
  var t;
  if (e.stopPropagation(), (t = this.data) != null && t.unique && o(this, i)) {
    await o(this, d).delete(this.data.unique);
    const a = new T({
      entityType: o(this, i).parentUnique ? C : D,
      unique: o(this, i).parentUnique
    });
    (await this.getContext(M)).dispatchEvent(a);
  }
  this._submitModal();
};
m = q([
  E("forms-folder-delete-confirm-modal")
], m);
const N = m;
export {
  m as FormsFolderDeleteConfirmModalElement,
  N as default
};
//# sourceMappingURL=folder-delete-confirm-modal.element.js.map
