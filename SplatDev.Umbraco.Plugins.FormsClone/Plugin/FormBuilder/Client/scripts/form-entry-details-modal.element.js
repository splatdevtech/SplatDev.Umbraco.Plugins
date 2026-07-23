import { html as u, repeat as xe, when as _, css as ze, state as Ve, customElement as Pe, unsafeHTML as Re } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as Oe, umbConfirmModal as qe } from "@umbraco-cms/backoffice/modal";
import { c as Le, d as Te, R as Be, e as he, f as Ne } from "./index.js";
import { tryExecute as je } from "@umbraco-cms/backoffice/resources";
import { UMB_NOTIFICATION_CONTEXT as He } from "@umbraco-cms/backoffice/notification";
import "@umbraco-cms/backoffice/class-api";
import "@umbraco-cms/backoffice/router";
import "@umbraco-cms/backoffice/external/rxjs";
import { E as De } from "./entry-render-helper.class.js";
var Ge = Object.defineProperty, Xe = Object.getOwnPropertyDescriptor, me = (e, t, i, l) => {
  for (var o = l > 1 ? void 0 : l ? Xe(t, i) : t, r = e.length - 1, n; r >= 0; r--)
    (n = e[r]) && (o = (l ? n(t, i, o) : n(o)) || o);
  return l && o && Ge(t, i, o), o;
}, Z = (e, t, i) => {
  if (!t.has(e))
    throw TypeError("Cannot " + i);
}, c = (e, t, i) => (Z(e, t, "read from private field"), i ? i.call(e) : t.get(e)), a = (e, t, i) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, i);
}, h = (e, t, i, l) => (Z(e, t, "write to private field"), t.set(e, i), i), s = (e, t, i) => (Z(e, t, "access private method"), i), f, k, S, v, p, U, $, P, q, fe, ee, pe, te, be, ie, _e, w, E, g, F, le, we, L, ve, T, ge, B, ye, A, x, b, y, R, se, z, N, j, Ee, H, ke, oe, Se, V, D, re, $e, ae, Fe, G, Ae, O, ne, X, We, K, Me, J, Ce, Q, Ie, Y, Ue;
const Ke = "form-entry-details-modal";
let W = class extends Oe {
  constructor() {
    super(), a(this, $), a(this, q), a(this, ee), a(this, te), a(this, ie), a(this, w), a(this, g), a(this, le), a(this, L), a(this, T), a(this, B), a(this, A), a(this, b), a(this, R), a(this, z), a(this, j), a(this, H), a(this, oe), a(this, V), a(this, re), a(this, ae), a(this, G), a(this, O), a(this, X), a(this, K), a(this, J), a(this, Q), a(this, Y), a(this, f, void 0), a(this, k, []), a(this, S, []), this._editModeActive = !1, a(this, v, []), a(this, p, {}), a(this, U, void 0), this.consumeContext(Le, (e) => {
      h(this, f, e);
    }), this.consumeContext(Te, (e) => {
      e && this.observe(e.userSecurity, (t) => {
        h(this, U, t);
      });
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await s(this, G, Ae).call(this), await s(this, X, We).call(this);
  }
  render() {
    return u`<umb-body-layout
      headline=${this.localize.term("formEntries_entryDetails")}
    >
      <uui-box
        headline=${this.localize.term("formEntries_formSubmissionHeadline")}
      >
        <umb-property-dataset
          id="formSubmissionFields"
          .value=${s(this, q, fe).call(this)}
        >
          ${xe(
      s(this, $, P).call(this),
      (e) => e.fieldId,
      (e) => u`<umb-property
                .label=${s(this, L, ve).call(this, e) ?? ""}
                alias=${e.fieldId}
                .config=${s(this, B, ye).call(this, e)}
                property-editor-ui-alias="${this._editModeActive ? s(this, T, ge).call(this, e) ?? "Umb.PropertyEditorUi.Label" : "Umb.PropertyEditorUi.Label"}"
              >
              </umb-property>
              ${_(
        s(this, j, Ee).call(this, e.fieldId),
        () => u`<div class="validation-error">
                  ${s(this, H, ke).call(this, e.fieldId).map(
          (t) => u`<div>${t}</div>`
        )}
                </div>`
      )} `
    )}
        </umb-property-dataset>
        ${s(this, Y, Ue).call(this)}
      </uui-box>

      <uui-box headline=${this.localize.term("general_general")}>
        <umb-property-layout
          label=${this.localize.term("formEntries_submittedOn")}
        >
          <div slot="editor">${this.value.created.toLocaleString()}</div>
        </umb-property-layout>
        ${_(
      this.value.updated !== this.value.created,
      () => u`<umb-property-layout
            label=${this.localize.term("formEntries_updatedOn")}
          >
            <div slot="editor">${this.value.updated.toLocaleString()}</div>
          </umb-property-layout>`
    )}
        ${_(
      this.value.pageName && this.value.pageName.length > 0,
      () => u`<umb-property-layout
            label=${this.localize.term("formEntries_fromPage")}
          >
            <div slot="editor" id="document">
              <span>${this.value.pageName}</span>
              <uui-button
                look="primary"
                color="default"
                @click=${s(this, J, Ce)}
                label=${this.localize.term("formEntries_goToDocument")}
              ></uui-button>
            </div>
          </umb-property-layout>`
    )}
        ${s(this, Q, Ie).call(this)}

        <umb-property-layout label=${this.localize.term("formEntries_state")}>
          <div slot="editor">${this.value.state}</div>
        </umb-property-layout>
        <umb-property-layout label=${this.localize.term("general_id")}>
          <div slot="editor">${this.value.id}</div>
        </umb-property-layout>
        <umb-property-layout
          label=${this.localize.term("formEntries_uniqueId")}
        >
          <div slot="editor">${this.value.unique}</div>
        </umb-property-layout>
      </uui-box>

      ${_(
      c(this, k).length > 0,
      () => u`<uui-box headline=${this.localize.term("formEntries_workflowAudit")}>
          <uui-table>
            <uui-table-head>
              <uui-table-head-cell>
                ${this.localize.term("formEntries_workflowAuditName")}
              </uui-table-head-cell>
              <uui-table-head-cell>
                ${this.localize.term("formEntries_workflowAuditType")}
              </uui-table-head-cell>
              <uui-table-head-cell>
                ${this.localize.term("formEntries_workflowAuditExecutedOn")}
              </uui-table-head-cell>
              <uui-table-head-cell>
                ${this.localize.term("formEntries_workflowAuditExecutionStage")}
              </uui-table-head-cell>
              <uui-table-head-cell>
                ${this.localize.term("formEntries_workflowAuditResult")}
              </uui-table-head-cell>
              <uui-table-head-cell></uui-table-head-cell>
            </uui-table-head>
            ${c(this, k).map(
        (e) => u`<uui-table-row>
                  <uui-table-cell>${e.name}</uui-table-cell>
                  <uui-table-cell>${e.typeName}</uui-table-cell>
                  <uui-table-cell
                    >${e.executedOn.toLocaleString()}</uui-table-cell
                  >
                  <uui-table-cell>
                    ${this.localize.term(
          "formEntries_workflowExecutionStage" + e.executionStage
        )}
                  </uui-table-cell>
                  <uui-table-cell>${e.result}</uui-table-cell>
                  <uui-table-cell>
                    <uui-button
                      label=${this.localize.term("formEntries_runWorkflow")}
                      @click="${() => s(this, K, Me).call(this, e.workflowKey)}"
                    >
                      ${this.localize.term(
          e.result === "Completed" ? "formEntries_workflowAuditRunAgain" : e.result === "SkippedDueToConditions" ? "formEntries_workflowAuditRunAnyway" : "formEntries_workflowAuditRetry"
        )}
                    </uui-button>
                  </uui-table-cell>
                </uui-table-row>`
      )}
          </uui-table>
        </uui-box>`
    )}
      ${_(
      c(this, S).length > 0,
      () => u`<uui-box headline=${this.localize.term("formEntries_recordAudit")}>
          <uui-table>
            <uui-table-head>
              <uui-table-head-cell
                >${this.localize.term(
        "formEntries_updatedOn"
      )}</uui-table-head-cell
              >
              <uui-table-head-cell
                >${this.localize.term(
        "formEntries_updatedBy"
      )}</uui-table-head-cell
              >
            </uui-table-head>
            ${c(this, S).map(
        (e) => u`<uui-table-row>
                  <uui-table-cell
                    >${e.updatedOn.toLocaleString()}</uui-table-cell
                  >
                  <uui-table-cell>${e.updatedBy}</uui-table-cell>
                </uui-table-row>`
      )}
          </uui-table>
        </uui-box>`
    )}
      <!-- submitting is intentional to ensure result is returned to the callee, indicating whether
      the collection should be updated (ie modal had saved changes) -->
      <uui-button
        slot="actions"
        label=${this.localize.term("general_close")}
        @click=${this._submitModal}
      ></uui-button>
    </umb-body-layout>`;
  }
};
f = /* @__PURE__ */ new WeakMap();
k = /* @__PURE__ */ new WeakMap();
S = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakMap();
p = /* @__PURE__ */ new WeakMap();
U = /* @__PURE__ */ new WeakMap();
$ = /* @__PURE__ */ new WeakSet();
P = function() {
  return this.value.fields.filter((e) => !s(this, le, we).call(this, e));
};
q = /* @__PURE__ */ new WeakSet();
fe = function() {
  return s(this, $, P).call(this).map((t) => ({
    alias: t.fieldId,
    value: this._editModeActive ? s(this, ie, _e).call(this, t) : s(this, ee, pe).call(this, t)
  }));
};
ee = /* @__PURE__ */ new WeakSet();
pe = function(e) {
  var l;
  const t = s(this, b, y).call(this, e);
  if (!t)
    return u`${e.value}`;
  const i = new De(this, ((l = this.data) == null ? void 0 : l.fieldPrevalues) || []);
  if (t.view === "file")
    return Re(s(this, te, be).call(this, e));
  {
    const o = i.getRenderValue(e, t.view);
    return u`${o}`;
  }
};
te = /* @__PURE__ */ new WeakSet();
be = function(e) {
  const t = ["jpg", "jpeg", "png", "gif", "bmp"];
  if (!e.value)
    return "";
  const i = e.value.toString().split(", ");
  let l = "";
  for (let o = 0; o < i.length; o++) {
    const r = i[o], n = r.substring(r.lastIndexOf(".") + 1).toLowerCase();
    if (l += "<div>", t.indexOf(n) >= 0)
      l += `<a target="_blank" href="${r}"><img src="${r}" width="200" /></a>`;
    else {
      const d = r.substring(r.lastIndexOf("/") + 1);
      l += `<a target="_blank" href="${r}">${d}</a>`;
    }
    l += "</div>";
  }
  return l;
};
ie = /* @__PURE__ */ new WeakSet();
_e = function(e) {
  var l, o;
  const t = s(this, b, y).call(this, e);
  if (!t)
    return null;
  const i = ((l = e.value) == null ? void 0 : l.toString()) || "";
  switch (t.editView) {
    case "Umb.PropertyEditorUi.Toggle":
      return i.toLowerCase() === "true";
    case "Umb.PropertyEditorUi.Dropdown": {
      const r = s(this, A, x).call(this, t.alias);
      if (((o = r == null ? void 0 : r.settings.AllowMultipleSelections) == null ? void 0 : o.toLowerCase()) === "true") {
        const n = s(this, w, E).call(this, e.fieldId, i.split(",").map((d) => d.trim()));
        return this._editModeActive ? n : n.join(", ");
      } else
        return s(this, w, E).call(this, e.fieldId, [i]);
    }
    case "Umb.PropertyEditorUi.RadioButtonList":
      return s(this, w, E).call(this, e.fieldId, [i])[0];
    case "Umb.PropertyEditorUi.CheckBoxList": {
      const r = s(this, w, E).call(this, e.fieldId, i.split(",").map((n) => n.trim()));
      return this._editModeActive ? r : r.join(", ");
    }
    case "Umb.PropertyEditorUi.DatePicker":
      return this._editModeActive ? i : new Date(i).toLocaleDateString();
    default:
      return i;
  }
};
w = /* @__PURE__ */ new WeakSet();
E = function(e, t) {
  const i = [];
  for (let l = 0; l < t.length; l++) {
    const o = t[l];
    if (o.length === 0)
      continue;
    const n = s(this, g, F).call(this, e).find((M) => M.value === o), d = n != null && n.caption && n.caption.length > 0 ? n == null ? void 0 : n.caption : o;
    i.push(d);
  }
  return i;
};
g = /* @__PURE__ */ new WeakSet();
F = function(e) {
  var t, i;
  return ((i = (t = this.data) == null ? void 0 : t.fieldPrevalues.find((l) => l.id === e)) == null ? void 0 : i.prevalues) || [];
};
le = /* @__PURE__ */ new WeakSet();
we = function(e) {
  const t = (l) => "0123456789abcdef".includes(l.toLowerCase());
  return !((l) => (l = l.replace(/-/g, ""), l.length === 32 && [...l].every(t)))(e.fieldId);
};
L = /* @__PURE__ */ new WeakSet();
ve = function(e) {
  const t = s(this, b, y).call(this, e);
  return t == null ? void 0 : t.name;
};
T = /* @__PURE__ */ new WeakSet();
ge = function(e) {
  const t = s(this, b, y).call(this, e);
  return t == null ? void 0 : t.editView;
};
B = /* @__PURE__ */ new WeakSet();
ye = function(e) {
  var l;
  const t = [];
  if (!this._editModeActive)
    return t;
  const i = s(this, b, y).call(this, e);
  if (!i)
    return t;
  switch (i.editView) {
    case "Umb.PropertyEditorUi.Dropdown": {
      const o = s(this, A, x).call(this, i.alias);
      o && (t.push({
        alias: "multiple",
        value: ((l = o.settings.AllowMultipleSelections) == null ? void 0 : l.toLowerCase()) === "true"
      }), t.push({
        alias: "items",
        value: s(this, g, F).call(this, o.id).map(
          (r) => r.caption && r.caption.length > 0 ? r.caption : r.value
        )
      }));
      break;
    }
    case "Umb.PropertyEditorUi.RadioButtonList":
    case "Umb.PropertyEditorUi.CheckBoxList": {
      const o = s(this, A, x).call(this, i.alias);
      o && t.push({
        alias: "items",
        value: s(this, g, F).call(this, o.id).map(
          (r) => r.caption && r.caption.length > 0 ? r.caption : r.value
        )
      });
      break;
    }
  }
  return t;
};
A = /* @__PURE__ */ new WeakSet();
x = function(e) {
  var i;
  return (((i = c(this, f)) == null ? void 0 : i.getAllFields()) || []).find((l) => l.alias === e);
};
b = /* @__PURE__ */ new WeakSet();
y = function(e) {
  return s(this, R, se).call(this, e.fieldId);
};
R = /* @__PURE__ */ new WeakSet();
se = function(e) {
  var t;
  return (t = this.data) == null ? void 0 : t.schema.find((i) => i.id === e);
};
z = /* @__PURE__ */ new WeakSet();
N = function() {
  this._editModeActive = !this._editModeActive, this._editModeActive ? h(this, v, s(this, $, P).call(this)) : h(this, v, []), h(this, p, {});
};
j = /* @__PURE__ */ new WeakSet();
Ee = function(e) {
  var t;
  return ((t = c(this, p)[e]) == null ? void 0 : t.length) > 0;
};
H = /* @__PURE__ */ new WeakSet();
ke = function(e) {
  return c(this, p)[e];
};
oe = /* @__PURE__ */ new WeakSet();
Se = async function() {
  var M, ue, ce, de;
  const e = [], t = c(this, v), i = s(this, re, $e).call(this);
  for (let m = 0; m < t.length; m++) {
    const C = t[m], I = i[m];
    C.value !== I.values.join(",") && e.push(I);
  }
  if (e.length === 0) {
    s(this, V, D).call(this);
    return;
  }
  const l = {
    recordId: ((M = this.value) == null ? void 0 : M.unique) || "",
    formId: ((ue = c(this, f)) == null ? void 0 : ue.getUnique()) || "",
    requestBody: e
  }, { error: o } = await je(
    Be.putFormByFormIdRecordByRecordId(l)
  );
  if (o) {
    h(this, p, o.body.errors), this.requestUpdate();
    return;
  }
  s(this, V, D).call(this);
  const r = {
    data: {
      message: this.localize.term("formEntries_updateRecordSuccess")
    }
  }, n = await this.getContext(He);
  n == null || n.peek("positive", r);
  const d = structuredClone((ce = this.value) == null ? void 0 : ce.fields);
  for (let m = 0; m < e.length; m++) {
    const C = e[m];
    d.find((I) => I.fieldId === C.fieldId).value = C.values;
  }
  (de = this.modalContext) == null || de.updateValue({ fields: d, updateCollection: !0 });
};
V = /* @__PURE__ */ new WeakSet();
D = function() {
  this._editModeActive = !1, h(this, p, {}), h(this, v, []);
};
re = /* @__PURE__ */ new WeakSet();
$e = function() {
  var i;
  const e = (i = this.shadowRoot) == null ? void 0 : i.getElementById(
    "formSubmissionFields"
  );
  if (!e)
    throw new Error("Could not find form submission fields element.");
  const t = [];
  for (let l = 0; l < e.value.length; l++) {
    const o = e.value[l];
    t.push({
      fieldId: o.alias,
      values: s(this, ae, Fe).call(this, o.alias, o)
    });
  }
  return t;
};
ae = /* @__PURE__ */ new WeakSet();
Fe = function(e, t) {
  const i = s(this, R, se).call(this, t.alias);
  if (!i)
    return [];
  switch (i.editView) {
    case "Umb.PropertyEditorUi.Dropdown":
    case "Umb.PropertyEditorUi.RadioButtonList":
    case "Umb.PropertyEditorUi.CheckBoxList": {
      const l = Array.isArray(t.value) ? t.value : [t.value];
      if (l.length === 0)
        return [];
      const o = s(this, g, F).call(this, e);
      return l.map((r) => {
        const n = o.find(
          (d) => d.caption === r
        );
        return n ? n.value : r;
      });
    }
    default:
      return [t.value];
  }
};
G = /* @__PURE__ */ new WeakSet();
Ae = async function() {
  var l;
  const e = (l = c(this, f)) == null ? void 0 : l.getUnique();
  if (!e)
    return [];
  const t = this.value.unique, i = new he(this);
  await s(this, O, ne).call(this, i, e, t);
};
O = /* @__PURE__ */ new WeakSet();
ne = async function(e, t, i) {
  var o;
  const l = await e.requestCollection({ formId: t, recordId: i });
  h(this, k, ((o = l == null ? void 0 : l.data) == null ? void 0 : o.items) || []), this.requestUpdate();
};
X = /* @__PURE__ */ new WeakSet();
We = async function() {
  var o, r;
  const e = (o = c(this, f)) == null ? void 0 : o.getUnique();
  if (!e)
    return [];
  const t = this.value.unique, l = await new Ne(this).requestCollection({ formId: e, recordId: t });
  h(this, S, ((r = l == null ? void 0 : l.data) == null ? void 0 : r.items) || []), this.requestUpdate();
};
K = /* @__PURE__ */ new WeakSet();
Me = async function(e) {
  var o;
  const t = (o = c(this, f)) == null ? void 0 : o.getUnique();
  if (!t)
    return;
  const i = this.value.unique;
  await qe(this, {
    headline: this.localize.term(
      "formEntries_workflowAuditRetryConfirmTitle"
    ),
    content: this.localize.term(
      "formEntries_workflowAuditRetryConfirmMessage"
    ),
    confirmLabel: this.localize.term("general_yes"),
    color: "positive"
  });
  const l = new he(this);
  await l.executeWorkflow(t, i, e), await s(this, O, ne).call(this, l, t, i);
};
J = /* @__PURE__ */ new WeakSet();
Ce = function() {
  history.pushState(
    {},
    "",
    `/umbraco/section/content/workspace/document/edit/${this.value.documentUnique}`
  ), this._rejectModal();
};
Q = /* @__PURE__ */ new WeakSet();
Ie = function() {
  var t;
  if (!((t = this.value.member) != null && t.unique))
    return null;
  const e = () => {
    history.pushState(
      {},
      "",
      `/umbraco/section/member-management/workspace/member/edit/${this.value.member.unique}`
    ), this._rejectModal();
  };
  return u` <umb-property-layout
      label=${this.localize.term("formEntries_member")}
    >
      <uui-ref-node-member
        slot="editor"
        name=${this.value.member.name}
        group-name=${this.value.member.email}
      >
        <uui-action-bar slot="actions">
          <uui-button
            label=${this.localize.term("general_edit")}
            @click=${e}
          ></uui-button>
        </uui-action-bar>
      </uui-ref-node-member>
    </umb-property-layout>`;
};
Y = /* @__PURE__ */ new WeakSet();
Ue = function() {
  var e;
  return (e = c(this, U)) != null && e.userSecurity.editEntries ? u`<div id="edit">
      ${_(
    this._editModeActive,
    () => u` <uui-button
            @click=${s(this, z, N)}
            look="default"
            label=${this.localize.term("general_cancel")}
          ></uui-button>
          <uui-button
            @click=${s(this, oe, Se)}
            look="primary"
            color="positive"
            label=${this.localize.term("buttons_save")}
          ></uui-button>`,
    () => u`<uui-button
          @click=${s(this, z, N)}
          look="primary"
          label=${this.localize.term("general_edit")}
        ></uui-button>`
  )}
    </div>` : null;
};
W.styles = [
  ze`
      uui-box {
        display: block;
      }
      uui-box + uui-box {
        margin-top: var(--uui-size-layout-1);
      }

      uui-table {
        width: 100%;
      }

      .validation-error {
        color: red;
      }

      #document {
        display: flex;
        justify-content: space-between;
      }

      #document uui-button {
        margin-left: var(--uui-size-6);
      }

      #edit {
        display: flex;
        justify-content: end;
        column-gap: var(--uui-size-3);
      }
    `
];
me([
  Ve()
], W.prototype, "_editModeActive", 2);
W = me([
  Pe(Ke)
], W);
const ot = W;
export {
  W as FormsEntryDetailsModalElement,
  ot as default
};
//# sourceMappingURL=form-entry-details-modal.element.js.map
