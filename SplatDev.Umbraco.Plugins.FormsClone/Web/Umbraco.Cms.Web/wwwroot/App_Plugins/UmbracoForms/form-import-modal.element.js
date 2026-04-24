import { html as d, state as v, customElement as F } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as E } from "@umbraco-cms/backoffice/modal";
import { UmbTemporaryFileRepository as b } from "@umbraco-cms/backoffice/temporary-file";
import { b as w, y as C } from "./index.js";
import { UMB_ACTION_EVENT_CONTEXT as T } from "@umbraco-cms/backoffice/action";
import { tryExecuteAndNotify as I } from "@umbraco-cms/backoffice/resources";
import { UMB_NOTIFICATION_CONTEXT as g } from "@umbraco-cms/backoffice/notification";
import { UmbRequestReloadChildrenOfEntityEvent as $ } from "@umbraco-cms/backoffice/entity-action";
var x = Object.defineProperty, M = Object.getOwnPropertyDescriptor, h = (t, e, o, i) => {
  for (var r = i > 1 ? void 0 : i ? M(e, o) : e, a = t.length - 1, s; a >= 0; a--)
    (s = t[a]) && (r = (i ? s(e, o, r) : s(r)) || r);
  return i && r && x(e, o, r), r;
}, f = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, O = (t, e, o) => (f(t, e, "read from private field"), o ? o.call(t) : e.get(t)), l = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, u = (t, e, o) => (f(t, e, "access private method"), o), c, m, y, p, _;
const K = "forms-form-import-modal";
let n = class extends E {
  constructor() {
    super(...arguments), l(this, m), l(this, p), l(this, c, new b(this));
  }
  render() {
    return d`
      <umb-body-layout .headline=${this.localize.term("general_import")}>
        <uui-box
          ><p>${this.localize.term("formImport_importInstruction1")}</p>
          <umb-input-upload-field
            @change=${u(this, p, _)}
            ?multiple=${!1}
            .fileExtensions=${["json"]}
            .keys=${this._temporaryFileKey ? [this._temporaryFileKey] : []}
          ></umb-input-upload-field>
          <p>${this.localize.term("formImport_importInstruction2")}</p></uui-box
        >
        <uui-button
          slot="actions"
          label=${this.localize.term("general_cancel")}
          @click="${this._rejectModal}"
        ></uui-button>
        <uui-button
          slot="actions"
          label=${this.localize.term("general_import")}
          look="primary"
          color="positive"
          @click=${u(this, m, y)}
        ></uui-button>
      </umb-body-layout>
    `;
  }
};
c = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakSet();
y = async function() {
  var r;
  if (!this._temporaryFileKey)
    throw new Error("file key is missing");
  const { data: t, error: e } = await I(
    this,
    w.postFormImport({
      requestBody: {
        fileKey: this._temporaryFileKey,
        folderId: (r = this.data) == null ? void 0 : r.unique
      }
    })
  );
  (!t || e) && ((await this.getContext(
    g
  )).peek("danger", {
    data: { message: "Unable to import form." }
  }), this._rejectModal());
  const o = new $({
    entityType: C,
    unique: null
  });
  (await this.getContext(T)).dispatchEvent(o), history.pushState(
    {},
    "",
    `/umbraco/section/forms/workspace/form/edit/${t}`
  ), this._submitModal();
};
p = /* @__PURE__ */ new WeakSet();
_ = async function(t) {
  const o = t.target.value.temporaryFileId;
  !o && this._temporaryFileKey && await O(this, c).delete(this._temporaryFileKey), this._temporaryFileKey = o;
};
h([
  v()
], n.prototype, "_temporaryFileKey", 2);
n = h([
  F(K)
], n);
const A = n;
export {
  n as FormsFormImportModalElement,
  A as default
};
//# sourceMappingURL=form-import-modal.element.js.map
