import { html as h, state as O, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as M, UMB_MODAL_MANAGER_CONTEXT as C } from "@umbraco-cms/backoffice/modal";
import { UMB_FOLDER_CREATE_MODAL as T } from "@umbraco-cms/backoffice/tree";
import { UMB_ACTION_EVENT_CONTEXT as F } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as R } from "@umbraco-cms/backoffice/entity-action";
import { v as w, a0 as y, a1 as $, a2 as b } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import "@umbraco-cms/backoffice/id";
var A = Object.defineProperty, g = Object.getOwnPropertyDescriptor, d = (e, t, a, i) => {
  for (var r = i > 1 ? void 0 : i ? g(t, a) : t, o = e.length - 1, n; o >= 0; o--)
    (n = e[o]) && (r = (i ? n(t, a, r) : n(r)) || r);
  return i && r && A(t, a, r), r;
}, f = (e, t, a) => {
  if (!t.has(e))
    throw TypeError("Cannot " + a);
}, D = (e, t, a) => (f(e, t, "read from private field"), a ? a.call(e) : t.get(e)), c = (e, t, a) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, a);
}, u = (e, t, a) => (f(e, t, "access private method"), a), m, p, E, s, _;
let l = class extends M {
  constructor() {
    super(...arguments), c(this, p), c(this, s), c(this, m, new w(this)), this._templates = [];
  }
  async connectedCallback() {
    super.connectedCallback();
    const { data: e } = await D(this, m).requestTemplates();
    e && (this._templates = e);
  }
  render() {
    return h`
			<umb-body-layout headline=${this.localize.term("formCreate_title")}>
				<uui-box>
					<uui-menu-item href=${u(this, s, _).call(this)} label=${this.localize.term("formCreate_newFormMenu")} @click=${this._submitModal}>
						<uui-icon slot="icon" name="icon-autofill"></uui-icon>
					</uui-menu-item>
					${this._templates.map(
      (e) => h`<uui-menu-item href=${u(this, s, _).call(this, e)} label="New ${e.name}..." @click=${this._submitModal}>
							<uui-icon slot="icon" name="icon-autofill"></uui-icon>
						</uui-menu-item>`
    )}
					<uui-menu-item @click=${u(this, p, E)} label=${this.localize.term("formCreate_newFolderMenu")}>
						<uui-icon slot="icon" name="icon-folder"></uui-icon>
					</uui-menu-item>
				</uui-box>
				<uui-button slot="actions" id="cancel" label=${this.localize.term("general_cancel")} @click="${this._rejectModal}">${this.localize.term("general_cancel")}</uui-button>
			</umb-body-layout>
		`;
  }
};
m = /* @__PURE__ */ new WeakMap();
p = /* @__PURE__ */ new WeakSet();
E = async function(e) {
  var r;
  if (e.stopPropagation(), !((r = this.data) != null && r.parent))
    throw new Error("A parent is required to create a form");
  const a = (await this.getContext(C)).open(this, T, {
    data: {
      folderRepositoryAlias: y,
      parent: this.data.parent
    }
  }), i = this.data.parent;
  a == null || a.onSubmit().then(async () => {
    this._submitModal();
    const o = new R({
      entityType: i.unique ? $ : b,
      unique: i.unique
    });
    (await this.getContext(F)).dispatchEvent(o);
  }).catch(() => {
  });
};
s = /* @__PURE__ */ new WeakSet();
_ = function(e) {
  var a, i;
  let t = `section/forms/workspace/form/create/parent/${(a = this.data) == null ? void 0 : a.parent.entityType}/${((i = this.data) == null ? void 0 : i.parent.unique) || "null"}`;
  return e && (t = t.replace("/create/", "/createFromTemplate/") + "/" + e.alias), t;
};
d([
  O()
], l.prototype, "_templates", 2);
l = d([
  v("forms-form-create-options-modal")
], l);
const U = l;
export {
  l as FormsFormCreateOptionsModalElement,
  U as default
};
//# sourceMappingURL=form-create-options-modal.element.js.map
