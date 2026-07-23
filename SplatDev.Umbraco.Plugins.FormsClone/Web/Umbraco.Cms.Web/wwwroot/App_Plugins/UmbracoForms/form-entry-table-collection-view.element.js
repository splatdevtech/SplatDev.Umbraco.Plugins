import { UmbTextStyles as X } from "@umbraco-cms/backoffice/style";
import { LitElement as z, html as u, when as K, css as B, property as J, state as w, customElement as Q } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as Z } from "@umbraco-cms/backoffice/element-api";
import { UmbModalRouteRegistrationController as j } from "@umbraco-cms/backoffice/router";
import { c as ee, a5 as te, G as se, M as ie, a6 as ne } from "./index.js";
import "@umbraco-cms/backoffice/resources";
import { E as ae } from "./entry-render-helper.class.js";
var oe = Object.defineProperty, re = Object.getOwnPropertyDescriptor, p = (e, t, s, o) => {
  for (var a = o > 1 ? void 0 : o ? re(t, s) : t, m = e.length - 1, S; m >= 0; m--)
    (S = e[m]) && (a = (o ? S(t, s, a) : S(a)) || a);
  return o && a && oe(t, s, a), a;
}, W = (e, t, s) => {
  if (!t.has(e))
    throw TypeError("Cannot " + s);
}, n = (e, t, s) => (W(e, t, "read from private field"), s ? s.call(e) : t.get(e)), i = (e, t, s) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, s);
}, d = (e, t, s, o) => (W(e, t, "write to private field"), t.set(e, s), s), r = (e, t, s) => (W(e, t, "access private method"), s), f, l, _, v, h, b, y, A, g, F, I, D, M, q, T, L, P, V, O, N, R, U, $, G, k, Y, E, H, C, x;
const le = "form-entry-table-collection-view";
let c = class extends Z(
  z
) {
  constructor() {
    super(...arguments), i(this, y), i(this, g), i(this, I), i(this, M), i(this, T), i(this, P), i(this, O), i(this, R), i(this, $), i(this, k), i(this, E), i(this, C), i(this, f, void 0), i(this, l, void 0), i(this, _, []), this._loading = !1, this._tableConfig = {
      allowSelection: !0
    }, this._tableColumns = [], this._tableItems = [], i(this, v, []), i(this, h, []), i(this, b, void 0);
  }
  get selectedEntryIds() {
    var e;
    return (e = n(this, l)) == null ? void 0 : e.selection.getSelection();
  }
  async connectedCallback() {
    super.connectedCallback(), this.consumeContext(ee, (e) => {
      d(this, f, e), this.observe(n(this, f).data, async (t) => {
        t && (d(this, h, n(this, f).getFieldsWithPrevalues()), await r(this, y, A).call(this, t.id), this.consumeContext(te, (s) => {
          d(this, l, s), r(this, g, F).call(this);
        }));
      });
    });
  }
  render() {
    return u`
      ${K(
      this._loading,
      () => u`<uui-loader-bar></uui-loader-bar>`
    )}
      <umb-table
        .config=${this._tableConfig}
        .columns=${this._tableColumns}
        .items=${this._tableItems}
        @selected="${r(this, k, Y)}"
        @deselected="${r(this, E, H)}"
      ></umb-table>`;
  }
};
f = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakMap();
b = /* @__PURE__ */ new WeakMap();
y = /* @__PURE__ */ new WeakSet();
A = async function(e) {
  for (let t = 0; t < n(this, h).length; t++) {
    const s = n(this, h)[t];
    if (s.prevalueSourceId !== se) {
      const o = new ie(this), { data: a } = await o.requestPrevalues(
        s.prevalueSourceId,
        e,
        s.id
      );
      a && (s.prevalues = a.map((m) => ({
        value: m.value,
        caption: m.caption
      })));
    }
  }
};
g = /* @__PURE__ */ new WeakSet();
F = function() {
  n(this, l) && (this.observe(n(this, l).loading, (e) => this._loading = e, "_observeLoading"), this.observe(
    n(this, l).items,
    (e) => {
      r(this, M, q).call(this, e), e.length && !n(this, b) && r(this, I, D).call(this);
    },
    "formsFormEntriesCollectionItemsObserver"
  ));
};
I = /* @__PURE__ */ new WeakSet();
D = function() {
  d(this, b, new j(
    this,
    ne
  ).addAdditionalPath(":unique").onSetup(async (e) => {
    const t = n(this, _).find((s) => s.unique == e.unique);
    if (!t)
      throw new Error("Could not find entry");
    return {
      data: {
        schema: n(this, v),
        fieldPrevalues: n(this, h)
      },
      value: t
    };
  }).onSubmit((e) => {
    var t;
    e.updateCollection && ((t = n(this, l)) == null || t.requestCollection());
  }));
};
M = /* @__PURE__ */ new WeakSet();
q = function(e) {
  if (e.length === 0)
    return;
  const t = e[0].schema;
  if (!t)
    throw new Error("Schema not provided.");
  d(this, v, t), d(this, _, e.slice(1)), r(this, T, L).call(this), r(this, P, V).call(this, t);
};
T = /* @__PURE__ */ new WeakSet();
L = function() {
  const e = n(this, v).filter((t) => t.showOnListingScreen).map((t) => ({
    name: t.name,
    alias: t.id
  }));
  e.push({
    name: "",
    alias: "buttonActions"
  }), this._tableColumns = e;
};
P = /* @__PURE__ */ new WeakSet();
V = function(e) {
  this._tableItems = n(this, _).map((t) => {
    const s = t.fields.map((o) => ({
      columnAlias: o.fieldId,
      value: r(this, O, N).call(this, o, e)
    }));
    return s.push({
      columnAlias: "buttonActions",
      value: u`<uui-button
          .label=${this.localize.term("formEntries_entryDetails")}
          look="secondary"
          color="default"
          @click=${(o) => r(this, $, G).call(this, o, t.unique)}
        ></uui-button>`
    }), {
      id: t.unique,
      icon: "icon-collection",
      data: s
    };
  });
};
O = /* @__PURE__ */ new WeakSet();
N = function(e, t) {
  const s = t.find((a) => a.id === e.fieldId);
  if (!s)
    return u`${e.value}`;
  const o = new ae(this, n(this, h));
  switch (s.view) {
    case "member": {
      if (!e.value)
        return null;
      const a = e.value;
      return u`${a.name}`;
    }
    case "workflowSummary":
      return u`<span
          style="color: ${r(this, R, U).call(this, e.value)}"
          >${e.value}</span
        >`;
    default: {
      const a = o.getRenderValue(e, s.view);
      return u`${a}`;
    }
  }
};
R = /* @__PURE__ */ new WeakSet();
U = function(e) {
  return e.split("/")[0] === e.split("/")[1] ? "green" : "red";
};
$ = /* @__PURE__ */ new WeakSet();
G = function(e, t) {
  e.stopImmediatePropagation(), history.pushState(
    {},
    "",
    `${location.href}/modal/forms-entrydetails-modal/${t}`
  );
};
k = /* @__PURE__ */ new WeakSet();
Y = function(e) {
  e.stopPropagation(), r(this, C, x).call(this, e.target);
};
E = /* @__PURE__ */ new WeakSet();
H = function(e) {
  e.stopPropagation(), r(this, C, x).call(this, e.target);
};
C = /* @__PURE__ */ new WeakSet();
x = function(e) {
  var s;
  const t = e.selection;
  (s = n(this, l)) == null || s.selection.setSelection(t);
};
c.styles = [
  X,
  B`
      :host {
        display: flex;
        flex-direction: column;
      }
      umb-table {
        padding: 0;
      }
      .workflow-summary-success {
        color: green;
      }
      .workflow-summary-failure {
        color: red;
      }
    `
];
p([
  J({ type: Array })
], c.prototype, "selectedEntryIds", 1);
p([
  w()
], c.prototype, "_loading", 2);
p([
  w()
], c.prototype, "_tableConfig", 2);
p([
  w()
], c.prototype, "_tableColumns", 2);
p([
  w()
], c.prototype, "_tableItems", 2);
c = p([
  Q(le)
], c);
const _e = c;
export {
  c as FormsFormEntryTableCollectionViewElement,
  _e as default
};
//# sourceMappingURL=form-entry-table-collection-view.element.js.map
