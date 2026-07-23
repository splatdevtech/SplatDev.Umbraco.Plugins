import { html as y, unsafeHTML as w, state as F, customElement as S } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as O } from "@umbraco-cms/backoffice/modal";
import { UmbRequestReloadChildrenOfEntityEvent as R } from "@umbraco-cms/backoffice/entity-action";
import { UMB_ACTION_EVENT_CONTEXT as C } from "@umbraco-cms/backoffice/action";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import { a4 as g, a3 as $, a1 as q, a2 as k } from "./index.js";
import "@umbraco-cms/backoffice/id";
var x = Object.defineProperty, D = Object.getOwnPropertyDescriptor, p = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? D(e, o) : e, a = t.length - 1, l; a >= 0; a--)
    (l = t[a]) && (i = (r ? l(e, o, i) : l(i)) || i);
  return r && i && x(e, o, i), i;
}, T = (t, e, o) => {
  if (!e.has(t))
    throw TypeError("Cannot " + o);
}, h = (t, e, o) => (T(t, e, "read from private field"), o ? o.call(t) : e.get(t)), s = (t, e, o) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, o);
}, n = (t, e, o) => (T(t, e, "access private method"), o), u, f, b, m, M, v, E, c, _;
let d = class extends O {
  constructor() {
    super(...arguments), s(this, f), s(this, m), s(this, v), s(this, c), s(this, u, new g(this)), this._canSubmit = !1;
  }
  async connectedCallback() {
    var e;
    if (super.connectedCallback(), !((e = this.data) != null && e.unique))
      return;
    const t = await h(this, u).requestByUnique(
      this.data.unique
    );
    this._folderToMove = t.data;
  }
  render() {
    var t;
    return y`
      <umb-body-layout headline=${this.localize.term("formMove_folderMoveModalHeadline")}>
        <uui-box>
          <p>
            ${w(this.localize.term("formMove_title", (t = this._folderToMove) == null ? void 0 : t.name))}
          </p>
          <uui-form>
            <form id="MoveFolder" @submit=${n(this, v, E)}>
              <uui-form-layout-item>
                <umb-tree
                  id="MoveToFolder"
                  alias=${$}
                  .props=${{
      hideTreeRoot: !1,
      selectionConfiguration: {
        multiple: !1
      },
      selectableFilter: (e) => e.isFolder
    }}
                  @selection-change=${n(this, f, b)}
                ></umb-tree>
              </uui-form-layout-item>
            </form>
          </uui-form>
        </uui-box>
        <uui-button
          slot="actions"
          label=${this.localize.term("general_cancel")}
          @click=${this._rejectModal}
          >${this.localize.term("general_cancel")}</uui-button
        >
        <uui-button
          form="MoveFolder"
          ?disabled=${!this._canSubmit}
          slot="actions"
          type="submit"
          label="Move"
          look="primary"
          color="positive"
        ></uui-button>
      </umb-body-layout>
    `;
  }
};
u = /* @__PURE__ */ new WeakMap();
f = /* @__PURE__ */ new WeakSet();
b = function() {
  const t = n(this, m, M).call(this);
  this._canSubmit = t !== void 0;
};
m = /* @__PURE__ */ new WeakSet();
M = function() {
  var e;
  return ((e = this.shadowRoot) == null ? void 0 : e.getElementById(
    "MoveToFolder"
  )).getSelection()[0];
};
v = /* @__PURE__ */ new WeakSet();
E = async function(t) {
  var a;
  if (t.preventDefault(), !h(this, u))
    throw new Error("Repository is not available");
  if (!((a = this.data) != null && a.unique))
    throw new Error("Unique identifier is not available");
  const e = t.target;
  if (!e || !e.checkValidity())
    return;
  const r = n(this, m, M).call(this), { error: i } = await h(this, u).moveFolder(
    this.data.unique,
    r
  );
  if (i)
    this._rejectModal();
  else {
    const l = this._folderToMove.parentUnique;
    await n(this, c, _).call(this, l), await n(this, c, _).call(this, r), this._submitModal();
  }
};
c = /* @__PURE__ */ new WeakSet();
_ = async function(t) {
  const e = new R({
    entityType: t ? q : k,
    unique: t || null
  });
  (await this.getContext(C)).dispatchEvent(e);
};
p([
  F()
], d.prototype, "_folderToMove", 2);
p([
  F()
], d.prototype, "_canSubmit", 2);
d = p([
  S("forms-folder-move-options-modal")
], d);
const V = d;
export {
  d as FormsFolderMoveOptionsModalElement,
  V as default
};
//# sourceMappingURL=folder-move-options-modal.element.js.map
