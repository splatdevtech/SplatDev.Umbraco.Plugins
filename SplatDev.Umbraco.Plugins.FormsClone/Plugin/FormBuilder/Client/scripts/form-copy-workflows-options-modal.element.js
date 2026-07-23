import { html as n, when as f, unsafeHTML as x, css as D, state as M, customElement as U } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as N } from "@umbraco-cms/backoffice/modal";
import { v as B, a3 as I } from "./index.js";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/resources";
import "@umbraco-cms/backoffice/id";
import { UMB_NOTIFICATION_CONTEXT as P } from "@umbraco-cms/backoffice/notification";
var H = Object.defineProperty, j = Object.getOwnPropertyDescriptor, v = (e, t, o, c) => {
  for (var a = c > 1 ? void 0 : c ? j(t, o) : t, k = e.length - 1, W; k >= 0; k--)
    (W = e[k]) && (a = (c ? W(t, o, a) : W(a)) || a);
  return c && a && H(t, o, a), a;
}, C = (e, t, o) => {
  if (!t.has(e))
    throw TypeError("Cannot " + o);
}, i = (e, t, o) => (C(e, t, "read from private field"), o ? o.call(e) : t.get(e)), r = (e, t, o) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, o);
}, E = (e, t, o, c) => (C(e, t, "write to private field"), t.set(e, o), o), s = (e, t, o) => (C(e, t, "access private method"), o), l, h, d, w, g, p, b, S, T, z, q, y, A, $, O, F, R, m, _;
let u = class extends N {
  constructor() {
    super(), r(this, w), r(this, p), r(this, S), r(this, z), r(this, y), r(this, $), r(this, F), r(this, m), r(this, l, void 0), this._selectedWorkflows = [], r(this, h, new B(this)), r(this, d, void 0), this.consumeContext(P, (e) => {
      E(this, d, e);
    });
  }
  async connectedCallback() {
    var t;
    if (super.connectedCallback(), !((t = this.data) != null && t.unique))
      return;
    const { data: e } = await i(this, h).requestByUnique(
      this.data.unique
    );
    e && (E(this, l, e), this._selectedWorkflows = s(this, w, g).call(this, e).map((o) => o.id), this.requestUpdate());
  }
  render() {
    return n`
      ${f(
      i(this, l),
      () => n`
          <umb-body-layout headline=${this.localize.term("formCopyWorkflows_title")}>
            <uui-box>
              <p><b>${this.localize.term("formCopyWorkflows_subtitle", i(this, l).name)}</b></p>
              ${f(
        !s(this, p, b).call(this, i(this, l)),
        () => n`<p>${this.localize.term("formCopyWorkflows_noWorkflows")}</p>`
      )}

              ${f(
        s(this, p, b).call(this, i(this, l)),
        () => n`
                  <p>${this.localize.term("formCopyWorkflows_selectedWorkflows")}</p>

                  <uui-table>
                    <uui-table-head>
                      <uui-table-head-cell>${this.localize.term("formCopyWorkflows_stage")}</uui-table-head-cell>
                      <uui-table-head-cell>${this.localize.term("formEntries_workflowAuditName")}</uui-table-head-cell>
                      <uui-table-head-cell>${this.localize.term("formEntries_workflowAuditType")}</uui-table-head-cell>
                      <uui-table-head-cell>${this.localize.term("formCopyWorkflows_selected")}?</uui-table-head-cell>
                    </uui-table-head>

                    ${s(this, m, _).call(this, i(this, l).formWorkflows.onSubmit, "onSubmit")}
                    ${s(this, m, _).call(this, i(this, l).formWorkflows.onApprove, "onApprove")}
                    ${s(this, m, _).call(this, i(this, l).formWorkflows.onReject, "onReject")}

                  </uui-table>

                  ${f(
          this._destinationForm,
          () => n`
                      <p>
                        ${x(this.localize.term("formCopyWorkflows_selectedDestinationForm", this._destinationForm.name))}
                        <small>(<a @click=${s(this, $, O)}>${this.localize.term("formCopyWorkflows_selectAnotherForm")}</a>)</small>
                      </p>`
        )}
                  ${f(
          !this._destinationForm,
          () => n`
                      <p>${this.localize.term("formCopyWorkflows_destinationForm")}</p>
                      <umb-tree
                        id="CopyToFolder"
                        alias="${I}"
                        .props=${{
            hideTreeRoot: !0,
            selectionConfiguration: {
              multiple: !1
            },
            selectableFilter: (e) => !e.isFolder && e.unique !== i(this, l).unique
          }}
                        @selection-change=${s(this, y, A)}
                      ></umb-tree>`
        )}
                `
      )}
            </uui-box>
            <uui-button
              slot="actions"
              id="cancel"
              label=${this.localize.term("general_cancel")}
              @click=${this._rejectModal}
              >${this.localize.term("general_cancel")}</uui-button
            >
            <uui-button
              form="CopyForm"
              slot="actions"
              type="submit"
              label=${this.localize.term("general_copy")}
              look="primary"
              color="positive"
              @click=${s(this, F, R)}
              ?disabled=${!i(this, l) || !this._destinationForm || this._selectedWorkflows.length === 0}
            ></uui-button>
          </umb-body-layout>
      `
    )}`;
  }
};
l = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakMap();
w = /* @__PURE__ */ new WeakSet();
g = function(e) {
  return e.formWorkflows.onSubmit.concat(e.formWorkflows.onApprove).concat(e.formWorkflows.onReject);
};
p = /* @__PURE__ */ new WeakSet();
b = function(e) {
  return s(this, w, g).call(this, e).length > 0;
};
S = /* @__PURE__ */ new WeakSet();
T = function(e) {
  return this._selectedWorkflows.includes(e);
};
z = /* @__PURE__ */ new WeakSet();
q = function(e) {
  const t = this._selectedWorkflows.indexOf(e);
  t >= 0 ? this._selectedWorkflows.splice(t, 1) : this._selectedWorkflows.push(e), this.requestUpdate();
};
y = /* @__PURE__ */ new WeakSet();
A = async function(e) {
  const o = e.target.getSelection();
  if (o.length > 0 && o[0] != i(this, l).id) {
    const c = o[0], { data: a } = await i(this, h).requestByUnique(
      c
    );
    a ? this._destinationForm = a : this._destinationForm = null;
  } else
    this._destinationForm = null;
  this.dispatchEvent(new CustomEvent("property-value-change"));
};
$ = /* @__PURE__ */ new WeakSet();
O = function() {
  this._destinationForm = null;
};
F = /* @__PURE__ */ new WeakSet();
R = async function() {
  var t, o;
  if (!i(this, h))
    throw new Error("Repository is not available");
  if (!i(this, l))
    throw new Error("Source form is not available");
  if (!this._destinationForm)
    throw new Error("Destination form is not available");
  const { error: e } = await i(this, h).copyFormWorkflows(
    i(this, l).unique,
    this._destinationForm.unique,
    this._selectedWorkflows
  );
  e ? ((o = i(this, d)) == null || o.peek("danger", {
    data: { message: this.localize.term("formCopyWorkflows_copiedWorkflowsError") }
  }), this._rejectModal()) : ((t = i(this, d)) == null || t.peek("positive", {
    data: { message: this.localize.term("formCopyWorkflows_copiedWorkflowsSuccess") }
  }), this._submitModal());
};
m = /* @__PURE__ */ new WeakSet();
_ = function(e, t) {
  return n`
      ${e.map(
    (o) => n`<uui-table-row>
            <uui-table-cell>${this.localize.term("formWorkflows_" + t)}</uui-table-cell>
            <uui-table-cell>${o.name}</uui-table-cell>
            <uui-table-cell>${o.workflowTypeName}</uui-table-cell>
            <uui-table-cell>
              <uui-toggle
                ?checked=${s(this, S, T).call(this, o.id)}
                @change=${() => s(this, z, q).call(this, o.id)}
              ></uui-toggle>
            </uui-table-cell>
          </uui-table-row>`
  )}`;
};
u.styles = [
  D`
      small a {
        text-decoration: underline;
        cursor: pointer;
      }
    `
];
v([
  M()
], u.prototype, "_selectedWorkflows", 2);
v([
  M()
], u.prototype, "_destinationForm", 2);
u = v([
  U("forms-form-copy-workflows-options-modal")
], u);
const Y = u;
export {
  u as FormsFormCopyWorkflowsOptionsModalElement,
  Y as default
};
//# sourceMappingURL=form-copy-workflows-options-modal.element.js.map
