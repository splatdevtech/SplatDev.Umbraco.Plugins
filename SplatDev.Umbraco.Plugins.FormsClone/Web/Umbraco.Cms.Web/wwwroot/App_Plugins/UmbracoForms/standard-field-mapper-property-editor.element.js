import { html as x, css as D, property as U, customElement as I } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as K } from "@umbraco-cms/backoffice/lit-element";
var O = Object.defineProperty, T = Object.getOwnPropertyDescriptor, F = (e, i, t, p) => {
  for (var l = p > 1 ? void 0 : p ? T(i, t) : i, u = e.length - 1, c; u >= 0; u--)
    (c = e[u]) && (l = (p ? c(i, t, l) : c(l)) || l);
  return p && l && O(i, t, l), l;
}, b = (e, i, t) => {
  if (!i.has(e))
    throw TypeError("Cannot " + t);
}, s = (e, i, t) => (b(e, i, "read from private field"), i.get(e)), r = (e, i, t) => {
  if (i.has(e))
    throw TypeError("Cannot add the same private member more than once");
  i instanceof WeakSet ? i.add(e) : i.set(e, t);
}, M = (e, i, t, p) => (b(e, i, "write to private field"), i.set(e, t), t), n = (e, i, t) => (b(e, i, "access private method"), t), o, a, g, N, w, W, m, d, S, $, v, C, _, P, y, E, h, k;
const V = "forms-standard-field-mapper-property-editor";
let f = class extends K {
  constructor() {
    super(...arguments), r(this, g), r(this, w), r(this, m), r(this, S), r(this, v), r(this, _), r(this, y), r(this, h), r(this, o, ""), r(this, a, []);
  }
  get value() {
    return s(this, o);
  }
  set value(e) {
    const i = s(this, o);
    M(this, o, e), this.requestUpdate("value", i), n(this, g, N).call(this, e);
  }
  async connectedCallback() {
    super.connectedCallback();
  }
  render() {
    return x`<div class="umb-forms-mappings">

        <div class="umb-forms-mapping-header">
          <div class="umb-forms-mapping-field -no-margin-left">Field</div>
          <div class="umb-forms-mapping-field">Include?</div>
          <div class="umb-forms-mapping-field -no-margin-right">Key</div>
        </div>

        ${s(this, a).map((e, i) => x`
          <div class="umb-forms-mapping">
            <div class="umb-forms-mapping-field -no-margin-left">
              <span>${n(this, v, C).call(this, e.field)}</span>
            </div>
            <div class="umb-forms-mapping-field" data-umb-standard-field-mapping-include="${e.field}">
              <uui-toggle
                ?checked=${e.include}
                @change=${() => n(this, _, P).call(this, i)}
              ></uui-toggle>
            </div>
            <div class="umb-forms-mapping-field -no-margin-right" data-umb-standard-field-mapping-key-name="${e.field}">
              <uui-input
                type="text"
                label="Key"
                placeholder="Key name"
                .value=${e.keyName || ""}
                @change=${(t) => n(this, y, E).call(this, t, i)}
              ></uui-input>
            </div>
          </div>`)}
      </div>`;
  }
};
o = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakMap();
g = /* @__PURE__ */ new WeakSet();
N = function(e) {
  e && e.length > 0 && (M(this, a, JSON.parse(e)), n(this, w, W).call(this)), n(this, m, d).call(this, "FormId"), n(this, m, d).call(this, "FormName"), n(this, m, d).call(this, "PageUrl"), n(this, m, d).call(this, "SubmissionDate");
};
w = /* @__PURE__ */ new WeakSet();
W = function() {
  for (let e = 0; e < s(this, a).length; e++) {
    const i = s(this, a)[e];
    delete i.$$hashKey;
  }
};
m = /* @__PURE__ */ new WeakSet();
d = function(e) {
  n(this, S, $).call(this, e) || s(this, a).push({
    field: e,
    include: !1
  });
};
S = /* @__PURE__ */ new WeakSet();
$ = function(e) {
  return s(this, a).find((i) => i.field == e) !== void 0;
};
v = /* @__PURE__ */ new WeakSet();
C = function(e) {
  switch (e) {
    case "FormId":
      return "Form ID";
    case "FormName":
      return "Form name";
    case "PageUrl":
      return "Page URL";
    case "SubmissionDate":
      return "Submission date/time";
    default:
      return e;
  }
};
_ = /* @__PURE__ */ new WeakSet();
P = function(e) {
  s(this, a)[e].include = !s(this, a)[e].include, n(this, h, k).call(this);
};
y = /* @__PURE__ */ new WeakSet();
E = function(e, i) {
  const t = e.target.value.toString();
  t.length > 0 ? s(this, a)[i].keyName = t : s(this, a)[i].keyName = void 0, n(this, h, k).call(this);
};
h = /* @__PURE__ */ new WeakSet();
k = function() {
  this.value = JSON.stringify(s(this, a)), this.dispatchEvent(new CustomEvent("property-value-change"));
};
f.styles = [
  D`
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
F([
  U({ type: String })
], f.prototype, "value", 1);
f = F([
  I(V)
], f);
const L = f;
export {
  f as FormsStandardFieldMapperPropertyUiElement,
  L as default
};
//# sourceMappingURL=standard-field-mapper-property-editor.element.js.map
