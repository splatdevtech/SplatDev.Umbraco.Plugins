import { html as y, unsafeHTML as w, state as T, customElement as S } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as O } from "@umbraco-cms/backoffice/modal";
import { UmbRequestReloadChildrenOfEntityEvent as R } from "@umbraco-cms/backoffice/entity-action";
import { UMB_ACTION_EVENT_CONTEXT as C } from "@umbraco-cms/backoffice/action";
import { v as g, a3 as $, a1 as q, a2 as k } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import "@umbraco-cms/backoffice/id";
var x = Object.defineProperty, D = Object.getOwnPropertyDescriptor, p = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? D(e, o) : e, a = t.length - 1, n; a >= 0; a--)
    (n = t[a]) && (i = (r ? n(e, o, i) : n(i)) || i);
  return r && i && x(e, o, i), i;
}, F = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, d = (t, e, o) => (F(t, e, "read from private field"), o ? o.call(t) : e.get(t)), l = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, s = (t, e, o) => (F(t, e, "access private method"), o), m, f, b, h, M, _, E, c, v;
let u = class extends O {
  constructor() {
    super(...arguments), l(this, f), l(this, h), l(this, _), l(this, c), l(this, m, new g(this)), this._canSubmit = !1;
  }
  async connectedCallback() {
    var e;
    if (super.connectedCallback(), !((e = this.data) != null && e.unique))
      return;
    const { data: t } = await d(this, m).requestByUnique(
      this.data.unique
    );
    this._formToMove = t;
  }
  render() {
    var t;
    return y`
      <umb-body-layout headline=${this.localize.term("formMove_formMoveModalHeadline")}>
        <uui-box>
          <p>
            ${w(this.localize.term("formMove_title", (t = this._formToMove) == null ? void 0 : t.name))}
          </p>
          <uui-form>
            <form id="MoveForm" @submit="${s(this, _, E)}">
              <uui-form-layout-item>
                <umb-tree
                  id="MoveToFolder"
                  alias="${$}"
                  .props=${{
      hideTreeRoot: !1,
      selectionConfiguration: {
        multiple: !1
      },
      selectableFilter: (e) => e.isFolder
    }}
                  @selection-change=${s(this, f, b)}
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
          form="MoveForm"
          ?disabled=${!this._canSubmit}
          slot="actions"
          type="submit"
          label=${this.localize.term("formMove_moveButtonLabel")}
          look="primary"
          color="positive"
        ></uui-button>
      </umb-body-layout>
    `;
  }
};
m = /* @__PURE__ */ new WeakMap();
f = /* @__PURE__ */ new WeakSet();
b = function() {
  const t = s(this, h, M).call(this);
  this._canSubmit = t !== void 0;
};
h = /* @__PURE__ */ new WeakSet();
M = function() {
  var e;
  return ((e = this.shadowRoot) == null ? void 0 : e.getElementById(
    "MoveToFolder"
  )).getSelection()[0];
};
_ = /* @__PURE__ */ new WeakSet();
E = async function(t) {
  var a, n;
  if (t.preventDefault(), !d(this, m))
    throw new Error("Repository is not available");
  if (!((a = this.data) != null && a.unique))
    throw new Error("Unique identifier is not available");
  const e = t.target;
  if (!e || !e.checkValidity())
    return;
  const r = s(this, h, M).call(this), { error: i } = await d(this, m).moveForm(
    this.data.unique,
    r
  );
  i ? this._rejectModal() : (await s(this, c, v).call(this, (n = this._formToMove) == null ? void 0 : n.parentUnique), await s(this, c, v).call(this, r), this._submitModal());
};
c = /* @__PURE__ */ new WeakSet();
v = async function(t) {
  const e = new R({
    entityType: t ? q : k,
    unique: t
  });
  (await this.getContext(C)).dispatchEvent(e);
};
p([
  T()
], u.prototype, "_formToMove", 2);
p([
  T()
], u.prototype, "_canSubmit", 2);
u = p([
  S("forms-form-move-options-modal")
], u);
const L = u;
export {
  u as FormsFormMoveOptionsModalElement,
  L as default
};
//# sourceMappingURL=form-move-options-modal.element.js.map
