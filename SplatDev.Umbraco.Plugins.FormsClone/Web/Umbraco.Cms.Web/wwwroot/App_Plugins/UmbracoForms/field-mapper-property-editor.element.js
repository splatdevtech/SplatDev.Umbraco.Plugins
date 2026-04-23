import { html as _, when as N, css as T, property as U, customElement as R } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as J } from "@umbraco-cms/backoffice/lit-element";
import { c as q } from "./index.js";
var G = Object.defineProperty, K = Object.getOwnPropertyDescriptor, O = (e, t, i, u) => {
  for (var r = u > 1 ? void 0 : u ? K(t, i) : t, v = e.length - 1, g; v >= 0; v--)
    (g = e[v]) && (r = (u ? g(t, i, r) : g(r)) || r);
  return u && r && G(t, i, r), r;
}, x = (e, t, i) => {
  if (!t.has(e))
    throw TypeError("Cannot " + i);
}, n = (e, t, i) => (x(e, t, "read from private field"), t.get(e)), a = (e, t, i) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, i);
}, h = (e, t, i, u) => (x(e, t, "write to private field"), t.set(e, i), i), l = (e, t, i) => (x(e, t, "access private method"), i), d, c, s, f, S, M, b, $, w, A, k, E, C, V, y, P, W, z, F, D, o, p;
const L = "forms-field-mapper-property-editor";
let m = class extends J {
  constructor() {
    super(), a(this, S), a(this, b), a(this, w), a(this, k), a(this, C), a(this, y), a(this, W), a(this, F), a(this, o), a(this, d, void 0), a(this, c, ""), a(this, s, []), a(this, f, []), this.consumeContext(q, (e) => {
      h(this, d, e), l(this, b, $).call(this);
    });
  }
  get value() {
    return n(this, c);
  }
  set value(e) {
    const t = n(this, c);
    h(this, c, e), this.requestUpdate("value", t), l(this, S, M).call(this, e);
  }
  async connectedCallback() {
    super.connectedCallback();
  }
  render() {
    return _`

      ${N(
      n(this, s).length > 0,
      () => _`
          <div class="umb-forms-mappings">

            <div class="umb-forms-mapping-header">
                <div class="umb-forms-mapping-field -no-margin-left">Alias</div>
                <div class="umb-forms-mapping-field">Form value</div>
                <div class="umb-forms-mapping-field">Static value</div>
                <div class="umb-forms-mapping-remove -no-margin-right"></div>
            </div>

            ${n(this, s).map((e, t) => _`
              <div class="umb-forms-mapping">

                <div class="umb-forms-mapping-field -no-margin-left">
                  <uui-input
                    type="text"
                    label="Alias"
                    placeholder="Alias"
                    .value=${e.alias}
                    @change=${(i) => l(this, k, E).call(this, i, t)}
                  ></uui-input>
                </div>

                <div class="umb-forms-mapping-field">
                  <uui-select
                    .options=${l(this, w, A).call(this, e.value)}
                    placeholder="Map a field"
                    @change=${(i) => l(this, C, V).call(this, i, t)}>
                  </uui-select>
                </div>

                <div class="umb-forms-mapping-field">
                  <uui-input
                    type="text"
                    label="Static value"
                    placeholder="Static value"
                    .value=${e.staticValue}
                    @change=${(i) => l(this, y, P).call(this, i, t)}
                  ></uui-input>
                </div>

                <div class="umb-forms-mapping-remove -no-margin-right">
                  <uui-button
                    label=${this.localize.term("general_delete")}
                    look="secondary"
                    color="default"
                    @click=${() => l(this, W, z).call(this, t)}
                  >
                    <uui-icon name="icon-delete"></uui-icon>
                  </uui-button>
                </div>

              </div>
            `)}
          </div>`
    )}

          <uui-button
            label="add"
            look="secondary"
            color="default"
            @click=${l(this, F, D)}
          >
            <uui-icon name="icon-add"></uui-icon>
          </uui-button>
        `;
  }
};
d = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakMap();
f = /* @__PURE__ */ new WeakMap();
S = /* @__PURE__ */ new WeakSet();
M = function(e) {
  e && e.length > 0 && h(this, s, JSON.parse(e));
};
b = /* @__PURE__ */ new WeakSet();
$ = function() {
  const t = n(this, d).getAllFields().map((i) => ({ name: i.caption, value: i.id }));
  t.unshift({ name: "Map a field", value: "" }), h(this, f, t);
};
w = /* @__PURE__ */ new WeakSet();
A = function(e) {
  return n(this, f).map((t) => ({
    ...t,
    selected: t.value === e ? !0 : void 0
  }));
};
k = /* @__PURE__ */ new WeakSet();
E = function(e, t) {
  const i = e.target.value.toString();
  n(this, s)[t].alias = i, l(this, o, p).call(this);
};
C = /* @__PURE__ */ new WeakSet();
V = function(e, t) {
  const i = e.target.value.toString();
  n(this, s)[t].value = i, l(this, o, p).call(this);
};
y = /* @__PURE__ */ new WeakSet();
P = function(e, t) {
  const i = e.target.value.toString();
  n(this, s)[t].staticValue = i, l(this, o, p).call(this);
};
W = /* @__PURE__ */ new WeakSet();
z = function(e) {
  n(this, s).splice(e, 1), l(this, o, p).call(this);
};
F = /* @__PURE__ */ new WeakSet();
D = function() {
  n(this, s).push({ alias: "", value: "", staticValue: "" }), l(this, o, p).call(this);
};
o = /* @__PURE__ */ new WeakSet();
p = function() {
  this.value = JSON.stringify(n(this, s)), this.dispatchEvent(new CustomEvent("property-value-change"));
};
m.styles = [
  T`
    .umb-forms-mappings {
      display: flex;
      flex-direction: column;
    }

    .umb-forms-mapping-header {
        display: flex;
        flex-direction: row;
        font-weight: bold;
    }

    .umb-forms-mapping {
        display: flex;
        flex-direction: row;
        margin-bottom: 5px;
        align-items: center;
    }

    .umb-forms-mapping-field {
        flex: 1 1 33%;
        margin: 5px;
    }

    .umb-forms-mapping-field input,
    .umb-forms-mapping-field select {
        margin-bottom: 0;
    }

    .umb-forms-mapping-field.-no-margin-left {
        margin-left: 0;
    }

    .umb-forms-mapping-remove {
        flex: 1 1 20px;
        margin: 5px;
    }


    .umb-forms-mapping-remove.-no-margin-right {
        margin-right: 0;
    }
		`
];
O([
  U({ type: String })
], m.prototype, "value", 1);
m = O([
  R(L)
], m);
const I = m;
export {
  m as FormsFieldMapperPropertyUiElement,
  I as default
};
//# sourceMappingURL=field-mapper-property-editor.element.js.map
