import { html as y, when as Z, css as j, property as ee, customElement as te } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as ie } from "@umbraco-cms/backoffice/lit-element";
import { c as ae, i as $ } from "./index.js";
import { tryExecuteAndNotify as T } from "@umbraco-cms/backoffice/resources";
var se = Object.defineProperty, le = Object.getOwnPropertyDescriptor, A = (e, t, i, h) => {
  for (var c = h > 1 ? void 0 : h ? le(t, i) : t, g = e.length - 1, S; g >= 0; g--)
    (S = e[g]) && (c = (h ? S(t, i, c) : S(c)) || c);
  return h && c && se(t, i, c), c;
}, D = (e, t, i) => {
  if (!t.has(e))
    throw TypeError("Cannot " + i);
}, a = (e, t, i) => (D(e, t, "read from private field"), t.get(e)), n = (e, t, i) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, i);
}, d = (e, t, i, h) => (D(e, t, "write to private field"), t.set(e, i), i), s = (e, t, i) => (D(e, t, "access private method"), i), v, p, l, m, _, w, U, W, R, E, q, k, B, N, J, C, K, x, G, P, L, z, X, f, F, b, H, M, I, O, Q, V, Y, r, o;
const ne = "forms-document-mapper-property-editor";
let u = class extends ie {
  constructor() {
    super(), n(this, w), n(this, W), n(this, E), n(this, k), n(this, N), n(this, C), n(this, x), n(this, P), n(this, z), n(this, f), n(this, b), n(this, M), n(this, O), n(this, V), n(this, r), n(this, v, void 0), n(this, p, ""), n(this, l, s(this, w, U).call(this)), n(this, m, []), n(this, _, []), this.consumeContext(ae, (e) => {
      d(this, v, e), s(this, k, B).call(this), s(this, C, K).call(this);
    });
  }
  get value() {
    return a(this, p);
  }
  set value(e) {
    let t = a(this, p);
    d(this, p, e), this.requestUpdate("value", t), s(this, W, R).call(this, e);
  }
  async connectedCallback() {
    super.connectedCallback();
  }
  render() {
    return y`
        <div>

          <uui-select
            .options=${s(this, x, G).call(this)}
            placeholder="Choose type"
            @change=${s(this, P, L)}>
          </uui-select>

          ${Z(
      a(this, l).doctype.length > 0,
      () => y`
            <div class="umb-forms__save-as-node">
              <div class="umb-forms__save-as-node__head">
                  <div class="umb-forms__save-as-node-field">
                      <strong><small>Property</small></strong>
                  </div>
                  <div class="umb-forms__save-as-node-field">
                      <strong><small>Field value</small></strong>
                  </div>
                  <div class="umb-forms__save-as-node-field">
                      <strong><small>Static value</small></strong>
                  </div>
              </div>

              <div class="umb-forms__save-as-node-group">
                  <div class="umb-forms__save-as-node-field">
                      <small>Node Name</small>
                  </div>

                  <div class="umb-forms__save-as-node-field">
                    <uui-select
                      .options=${s(this, f, F).call(this, a(this, l).nameField)}
                      placeholder="Select form field"
                      @change=${s(this, b, H)}>
                    </uui-select>
                  </div>

                  <div class="umb-forms__save-as-node-field">
                    <uui-input
                      type="text"
                      label="Static value"
                      placeholder="Static value"
                      .value=${a(this, l).nameStaticValue || ""}
                      @change=${s(this, M, I)}
                    ></uui-input>
                  </div>
              </div>

              ${a(this, l).properties.map((e, t) => y`
                  <div class="umb-forms__save-as-node-group">
                    <div class="umb-forms__save-as-node-field">
                        <small>${e.value}</small>
                    </div>

                    <div class="umb-forms__save-as-node-field">
                      <uui-select
                        .options=${s(this, f, F).call(this, e.field)}
                        placeholder="Map a field"
                        @change=${(i) => s(this, O, Q).call(this, i, t)}>
                      </uui-select>
                    </div>

                    <div class="umb-forms__save-as-node-field">
                      <uui-input
                        type="text"
                        label="Static value"
                        placeholder="Static value"
                        .value=${e.staticValue}
                        @change=${(i) => s(this, V, Y).call(this, i, t)}>
                      ></uui-input>
                    </div>
                  </div>`)}

            </div>
          `
    )}

        </div>`;
  }
};
v = /* @__PURE__ */ new WeakMap();
p = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakMap();
w = /* @__PURE__ */ new WeakSet();
U = function() {
  return { doctype: "", nameField: "", nameStaticValue: "", properties: [] };
};
W = /* @__PURE__ */ new WeakSet();
R = function(e) {
  e && e.length > 0 && (d(this, l, JSON.parse(e)), s(this, E, q).call(this));
};
E = /* @__PURE__ */ new WeakSet();
q = function() {
  for (let e = 0; e < a(this, l).properties.length; e++) {
    const t = a(this, l).properties[e];
    delete t.$$hashKey;
  }
};
k = /* @__PURE__ */ new WeakSet();
B = async function() {
  const { data: e } = await T(
    this,
    $.getPickerDocumentType()
  );
  e && (d(this, m, e), await s(this, N, J).call(this), this.requestUpdate());
};
N = /* @__PURE__ */ new WeakSet();
J = async function() {
  if (a(this, l).doctype.length === 0)
    return;
  const e = {
    doctypeAlias: a(this, l).doctype,
    currentProperties: a(this, l).properties
  }, { data: t } = await T(
    this,
    $.postPickerDocumentTypeMappingsRefresh({ requestBody: e })
  );
  t && (a(this, l).properties = t, s(this, r, o).call(this));
};
C = /* @__PURE__ */ new WeakSet();
K = function() {
  const t = a(this, v).getAllFields().map((i) => ({ name: i.caption, value: i.id }));
  t.unshift({ name: "Select form field", value: "" }), d(this, _, t);
};
x = /* @__PURE__ */ new WeakSet();
G = function() {
  return a(this, m).map((e) => ({
    name: e.value,
    value: e.id,
    selected: e.id === a(this, l).doctype ? !0 : void 0
  }));
};
P = /* @__PURE__ */ new WeakSet();
L = async function(e) {
  const t = e.target.value.toString();
  a(this, l).doctype = t, await s(this, z, X).call(this, t), s(this, r, o).call(this);
};
z = /* @__PURE__ */ new WeakSet();
X = async function(e) {
  if (a(this, l).properties = [], e.length === 0)
    return;
  const { data: t } = await T(
    this,
    $.getPickerDocumentTypeByAliasProperties({ alias: e })
  );
  t && (a(this, l).properties = t.map((i) => ({ id: i.id, value: i.value, staticValue: "", field: "" })), s(this, r, o).call(this));
};
f = /* @__PURE__ */ new WeakSet();
F = function(e) {
  return a(this, _).map((t) => ({
    ...t,
    selected: e && t.value === e ? !0 : void 0
  }));
};
b = /* @__PURE__ */ new WeakSet();
H = function(e) {
  const t = e.target.value.toString();
  a(this, l).nameField = t.length > 0 ? t : void 0, s(this, r, o).call(this);
};
M = /* @__PURE__ */ new WeakSet();
I = function(e) {
  const t = e.target.value.toString();
  a(this, l).nameStaticValue = t.length > 0 ? t : void 0, s(this, r, o).call(this);
};
O = /* @__PURE__ */ new WeakSet();
Q = function(e, t) {
  const i = e.target.value.toString();
  a(this, l).properties[t].field = i, s(this, r, o).call(this);
};
V = /* @__PURE__ */ new WeakSet();
Y = function(e, t) {
  const i = e.target.value.toString();
  a(this, l).properties[t].staticValue = i, s(this, r, o).call(this);
};
r = /* @__PURE__ */ new WeakSet();
o = function() {
  this.value = JSON.stringify(a(this, l)), this.dispatchEvent(new CustomEvent("property-value-change"));
};
u.styles = [
  j`
    .umb-forms__save-as-node {
        flex-wrap: wrap;
        margin-top: 20px;
    }

    .umb-forms__save-as-node-group {
        display: flex;
        flex-wrap: wrap;
        flex-direction: row;
        padding: 5px 0;
    }

    .umb-forms__save-as-node__head {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
        width: 100%;
    }

    .umb-forms__save-as-node-field {
        display: flex;
        align-items: center;
        flex: 0 0 37%;
        margin: 3px;
    }

    .umb-forms__save-as-node-field:first-of-type {
        flex: 0 0 20%;
    }

    .umb-forms__save-as-node .-full-width {
        width: 100%;
    }
		`
];
A([
  ee({ type: String })
], u.prototype, "value", 1);
u = A([
  te(ne)
], u);
const pe = u;
export {
  u as FormsDocumentMapperPropertyUiElement,
  pe as default
};
//# sourceMappingURL=document-mapper-property-editor.element.js.map
