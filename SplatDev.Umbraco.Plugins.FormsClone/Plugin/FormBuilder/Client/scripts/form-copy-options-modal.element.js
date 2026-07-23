import { html as N, customElement as O } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as M } from "@umbraco-cms/backoffice/modal";
import { v as k, a3 as R, a1 as $, a2 as g } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import "@umbraco-cms/backoffice/id";
import { UmbRequestReloadChildrenOfEntityEvent as x } from "@umbraco-cms/backoffice/entity-action";
import { UMB_ACTION_EVENT_CONTEXT as S } from "@umbraco-cms/backoffice/action";
import { UMB_NOTIFICATION_CONTEXT as z } from "@umbraco-cms/backoffice/notification";
var W = Object.defineProperty, q = Object.getOwnPropertyDescriptor, D = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? q(e, o) : e, l = t.length - 1, u; l >= 0; l--)
    (u = t[l]) && (i = (r ? u(e, o, i) : u(i)) || i);
  return r && i && W(e, o, i), i;
}, y = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, a = (t, e, o) => (y(t, e, "read from private field"), o ? o.call(t) : e.get(t)), c = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, b = (t, e, o, r) => (y(t, e, "write to private field"), e.set(t, o), o), I = (t, e, o) => (y(t, e, "access private method"), o), p, n, m, f, E;
let h = class extends M {
  constructor() {
    super(), c(this, f), c(this, p, void 0), c(this, n, new k(this)), c(this, m, void 0), this.consumeContext(z, (t) => {
      b(this, m, t);
    });
  }
  async connectedCallback() {
    var e;
    if (super.connectedCallback(), !((e = this.data) != null && e.unique))
      return;
    const { data: t } = await a(this, n).requestByUnique(
      this.data.unique
    );
    b(this, p, t), this.requestUpdate();
  }
  render() {
    var t;
    return N`
      <umb-body-layout headline=${this.localize.term("formCopy_modalHeadline")}>
        <uui-box>
          <p>Copying form <b>${(t = a(this, p)) == null ? void 0 : t.name}</b></p>
          <uui-form>
            <form id="CopyForm" @submit="${I(this, f, E)}">
              <uui-form-layout-item>
                <uui-label id="NewNameLabel" for="NewName" slot="label"
                  >${this.localize.term("formCopy_enterName")}</uui-label
                >
                <uui-input
                  type="text"
                  id="NewName"
                  name="NewName"
                  placeholder=${this.localize.term("formCopy_enterNewName")}
                ></uui-input>
              </uui-form-layout-item>
              <uui-form-layout-item>
                <uui-checkbox name="CopyWorkflows" label=${this.localize.term("formCopy_alsoCopyWorkflows")}
                  >${this.localize.term("formCopy_alsoCopyWorkflows")}</uui-checkbox
                >
                <div><small>${this.localize.term("formCopy_mandatoryWorkflowsCopied")}</small></div>
              </uui-form-layout-item>
              <uui-form-layout-item>
                <umb-tree
                  id="CopyToFolder"
                  alias="${R}"
                  .props=${{
      hideTreeRoot: !1,
      selectionConfiguration: {
        multiple: !1
      },
      selectableFilter: (e) => e.isFolder
    }}
                ></umb-tree>
              </uui-form-layout-item>
            </form>
          </uui-form>
        </uui-box>
        <uui-button
          slot="actions"
          id="cancel"
          label=${this.localize.term("general_cancel")}
          @click="${this._rejectModal}"
          >${this.localize.term("general_cancel")}</uui-button
        >
        <uui-button
          form="CopyForm"
          slot="actions"
          type="submit"
          label=${this.localize.term("general_copy")}
          look="primary"
          color="positive"
        ></uui-button>
      </umb-body-layout>
    `;
  }
};
p = /* @__PURE__ */ new WeakMap();
n = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakMap();
f = /* @__PURE__ */ new WeakSet();
E = async function(t) {
  var _, C, v, w;
  if (t.preventDefault(), !a(this, n))
    throw new Error("Repository is not available");
  if (!((_ = this.data) != null && _.unique))
    throw new Error("Unique identifier is not available");
  const e = t.target;
  if (!e || !e.checkValidity())
    return;
  const r = new FormData(e), i = r.get("NewName"), l = r.get("CopyWorkflows") === "on", d = ((C = this.shadowRoot) == null ? void 0 : C.getElementById(
    "CopyToFolder"
  )).getSelection()[0], s = d === void 0 ? null : d === null ? "-1" : d, { error: T } = await a(this, n).copyForm(
    this.data.unique,
    l,
    i,
    s
  );
  if (T)
    (w = a(this, m)) == null || w.peek("danger", {
      data: { message: this.localize.term("formCopy_copyError") }
    }), this._rejectModal();
  else {
    const F = new x({
      entityType: s !== "-1" && s ? $ : g,
      unique: s === "-1" ? null : s || null
    });
    (await this.getContext(S)).dispatchEvent(F), (v = a(this, m)) == null || v.peek("positive", {
      data: { message: this.localize.term("formCopy_copySuccess") }
    }), this._submitModal();
  }
};
h = D([
  O("forms-form-copy-options-modal")
], h);
const J = h;
export {
  h as FormsFormCopyOptionsModalElement,
  J as default
};
//# sourceMappingURL=form-copy-options-modal.element.js.map
