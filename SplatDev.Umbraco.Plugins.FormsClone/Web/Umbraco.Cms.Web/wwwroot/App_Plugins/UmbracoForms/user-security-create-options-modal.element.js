import { html as u, when as p, customElement as b } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as g } from "@umbraco-cms/backoffice/modal";
import { UMB_ACTION_EVENT_CONTEXT as O } from "@umbraco-cms/backoffice/action";
import { W as k, a7 as M } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/id";
import "@umbraco-cms/backoffice/resources";
import { UmbRequestReloadChildrenOfEntityEvent as R } from "@umbraco-cms/backoffice/entity-action";
var W = Object.defineProperty, x = Object.getOwnPropertyDescriptor, z = (e, t, r, n) => {
  for (var s = n > 1 ? void 0 : n ? x(t, r) : t, h = e.length - 1, m; h >= 0; h--)
    (m = e[h]) && (s = (n ? m(t, r, s) : m(s)) || s);
  return n && s && W(t, r, s), s;
}, v = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, o = (e, t, r) => (v(e, t, "read from private field"), r ? r.call(e) : t.get(e)), i = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, I = (e, t, r, n) => (v(e, t, "write to private field"), t.set(e, r), r), l = (e, t, r) => (v(e, t, "access private method"), r), a, c, d, S, _, U, y, C, w, T, E, $;
const N = "forms-user-security-create-options-modal";
let f = class extends g {
  constructor() {
    super(), i(this, d), i(this, _), i(this, y), i(this, w), i(this, E), i(this, a, []), i(this, c, new k(this)), l(this, d, S).call(this);
  }
  render() {
    return u`<umb-body-layout
      headline=${this.localize.term("formSecurity_createTitle")}
    >
      <uui-box>
        <p>${this.localize.term("formSecurity_createUserRecordNote")}</p>
        ${p(
      o(this, a).length === 0,
      () => u`<p>
              ${this.localize.term("formSecurity_allUsersHaveRecords")}
            </p>`
    )}
        ${p(
      o(this, a).length > 0,
      () => u` <uui-select
            id="users"
            label="User"
            .options=${o(this, a).map((e) => ({ name: e.name, value: e.id }))}
          >
          </uui-select>`
    )}
      </uui-box>
      <uui-button
        slot="actions"
        label=${this.localize.term("general_cancel")}
        @click=${this._rejectModal}
        ></uui-button
      >
      ${p(
      o(this, a).length > 0,
      () => u`<uui-button
            slot="actions"
            label=${this.localize.term("general_create")}
            look="primary"
            color="positive"
            @click=${l(this, _, U)}
            ></uui-button
          >`
    )}
    </umb-body-layout>`;
  }
};
a = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakSet();
S = async function() {
  I(this, a, await o(this, c).requestUsersToAssign()), this.requestUpdate();
};
_ = /* @__PURE__ */ new WeakSet();
U = async function() {
  const e = l(this, y, C).call(this);
  await l(this, w, T).call(this, e), l(this, E, $).call(this), this._submitModal(), history.pushState(
    {},
    "",
    `/umbraco/section/forms/workspace/forms-security-user/edit/${e}/view/permissions`
  );
};
y = /* @__PURE__ */ new WeakSet();
C = function() {
  var t;
  return ((t = this.shadowRoot) == null ? void 0 : t.getElementById(
    "users"
  )).value.toString();
};
w = /* @__PURE__ */ new WeakSet();
T = async function(e) {
  const { data: t } = await o(this, c).createScaffold({ unique: e });
  await o(this, c).create(t, null);
};
E = /* @__PURE__ */ new WeakSet();
$ = async function() {
  const e = new R({
    entityType: M,
    unique: "207c2294-970b-4e1f-82fd-ae8996ef171d"
    // Matches Umbraco.Forms.Web.Controllers.ManagementApi.Security.Tree.SecurityTreeControllerBase.UsersFolderId
  });
  (await this.getContext(O)).dispatchEvent(e);
};
f = z([
  b(N)
], f);
const H = f;
export {
  f as FormsUserSecurityCreateOptionsModalElement,
  H as default
};
//# sourceMappingURL=user-security-create-options-modal.element.js.map
