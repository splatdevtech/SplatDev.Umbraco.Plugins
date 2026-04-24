import { LitElement as R, html as v, when as b, repeat as H, css as q, state as h, customElement as G } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as K } from "@umbraco-cms/backoffice/element-api";
import { F as X } from "./prevaluesource-workspace.context-token.js";
import { P as J } from "./index.js";
var Q = Object.defineProperty, Y = Object.getOwnPropertyDescriptor, c = (t, e, i, n) => {
  for (var p = n > 1 ? void 0 : n ? Y(e, i) : e, S = t.length - 1, P; S >= 0; S--)
    (P = t[S]) && (p = (n ? P(e, i, p) : P(p)) || p);
  return n && p && Q(e, i, p), p;
}, k = (t, e, i) => {
  if (!e.has(t))
    throw TypeError("Cannot " + i);
}, a = (t, e, i) => (k(t, e, "read from private field"), i ? i.call(t) : e.get(t)), s = (t, e, i) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, i);
}, m = (t, e, i, n) => (k(t, e, "write to private field"), e.set(t, i), i), l = (t, e, i) => (k(t, e, "access private method"), i), d, r, o, g, C, x, T, B, w, I, O, L, E, D, U, M, _, f, W, F, $, A, V, N, y, z;
const Z = "workspace-view-prevaluesource-design";
let u = class extends K(R) {
  constructor() {
    super(), s(this, C), s(this, T), s(this, w), s(this, O), s(this, E), s(this, U), s(this, _), s(this, W), s(this, $), s(this, V), s(this, y), s(this, d, []), this._settingValues = [], this._settingsConfig = [], this._settingsConfigLoaded = !1, this._cachePrevaluesOption = "", this._cachePrevaluesByTimeValue = 0, this._cachePrevaluesByTimeUnit = "seconds", s(this, r, void 0), s(this, o, void 0), s(this, g, void 0), m(this, g, Promise.all([
      this.consumeContext(
        X,
        (t) => {
          m(this, r, t), l(this, C, x).call(this);
        }
      ).asPromise()
    ]));
  }
  async connectedCallback() {
    var e, i;
    if (super.connectedCallback(), await a(this, g), !a(this, r))
      return;
    this._prevalueSource = a(this, r).getData();
    const t = (e = this._prevalueSource) == null ? void 0 : e.fieldPreValueSourceTypeId;
    t && (this._prevalueSourceType = await ((i = a(this, r)) == null ? void 0 : i.loadPrevalueSourceType(
      t
    )), this._prevalueSourceType && (m(this, o, new J(
      this,
      this._prevalueSourceType,
      "formProviderPrevalueSources"
    )), await a(this, o).loadSettingValueConverterApis(), await l(this, T, B).call(this)));
  }
  render() {
    var t;
    return v`
      <uui-box
        headline=${this.localize.term(
      "formPrevalueSources_designModalHeadline"
    )}
      >
        <umb-property-layout
          alias="type"
          .label=${this.localize.term("general_type")}
        >
          <div slot="editor">${(t = this._prevalueSourceType) == null ? void 0 : t.name}</div>
        </umb-property-layout>

        ${b(
      a(this, o) && this._prevalueSourceType && this._settingsConfigLoaded,
      () => v`<umb-property-dataset
            .value=${this._settingValues}
            @change=${l(this, w, I)}
          >
            ${this._prevalueSourceType.settings.map(
        (e) => {
          var i, n;
          return v`
                <umb-property
                  ?inert=${e.isReadOnly}
                  .label=${(i = a(this, o)) == null ? void 0 : i.getLocalizedSettingDetail(
            e,
            "Label",
            e.name
          )}
                  .description=${(n = a(this, o)) == null ? void 0 : n.getLocalizedSettingDetail(
            e,
            "Description",
            e.description
          )}
                  alias=${e.alias}
                  .config=${a(this, o).getPropertyConfigForSetting(
            this._settingsConfig,
            e
          )}
                  .appearance=${a(this, o).getPropertyAppearanceForSetting(
            e.view
          )}
                  property-editor-ui-alias=${e.view}
                >
                </umb-property>
              `;
        }
      )}
          </umb-property-dataset> `
    )}

        <umb-property-layout
          alias="_cachePrevalues"
          .label=${this.localize.term("formPrevalueSources_cacheOptionsLabel")}
          .description=${this.localize.term(
      "formPrevalueSources_cacheOptionsDescription"
    )}
        >
          <div slot="editor">
            <uui-radio-group
              .value=${this._cachePrevaluesOption}
              @change=${l(this, O, L)}
            >
              ${H(
      a(this, d),
      (e) => e,
      (e) => v`<uui-radio
                    value=${e.value}
                    label=${e.label}
                  ></uui-radio>`
    )}
            </uui-radio-group>
            ${b(
      this._cachePrevaluesOption === "time",
      () => v`
                <div>
                  <label
                    >${this.localize.term(
        "formPrevalueSources_cacheOptionsEnterTime"
      )}</label
                  >
                  <uui-input
                    type="number"
                    value=${this._cachePrevaluesByTimeValue}
                    min="1"
                    max="59"
                    label="Time value"
                    @change=${l(this, $, A)}
                  ></uui-input>
                  <uui-select
                    label="Time unit"
                    @change=${l(this, V, N)}
                    .options=${["Seconds", "Minutes", "Hours"].map((e) => ({
        name: this.localize.term(
          `formPrevalueSources_cacheOptions${e}`
        ),
        value: e.toLowerCase(),
        selected: this._cachePrevaluesByTimeUnit === e.toLowerCase()
      }))}
                  ></uui-select>
                </div>
              `
    )}
          </div>
        </umb-property-layout>
      </uui-box>

      ${b(
      a(this, r) && a(this, r).getPrevalues().length > 0,
      () => {
        var e;
        return v`
          <uui-box
            headline=${this.localize.term("formPrevalueSources_listHeadline")}
          >
            <uui-table>
              <uui-table-head>
                <uui-table-head-cell
                  >${this.localize.term("general_value")}</uui-table-head-cell
                >
                <uui-table-head-cell
                  >${this.localize.term(
          "formPrevalues_caption"
        )}</uui-table-head-cell
                >
              </uui-table-head>
              ${(e = a(this, r)) == null ? void 0 : e.getPrevalues().map(
          (i) => v` <uui-table-row>
                  <uui-table-cell>${i.value}</uui-table-cell>
                  <uui-table-cell>${i.caption}</uui-table-cell>
                </uui-table-row>`
        )}
            </uui-table>
          </uui-box>`;
      }
    )}
    `;
  }
};
d = /* @__PURE__ */ new WeakMap();
r = /* @__PURE__ */ new WeakMap();
o = /* @__PURE__ */ new WeakMap();
g = /* @__PURE__ */ new WeakMap();
C = /* @__PURE__ */ new WeakSet();
x = function() {
  a(this, r) && this.observe(a(this, r).prevalues, () => {
    this.requestUpdate();
  });
};
T = /* @__PURE__ */ new WeakSet();
B = async function() {
  var t;
  if (!(!a(this, r) || !this._prevalueSource)) {
    if (l(this, E, D).call(this), this._settingValues = await a(this, o).getSettingValues(
      this._prevalueSource.settings
    ), this._settingsConfig = await a(this, o).getSettingsConfig(
      this._settingValues,
      this._prevalueSourceType.settings
    ), this._settingsConfigLoaded = !0, (t = a(this, r)) != null && t.getIsNew() && this._prevalueSourceType) {
      const e = structuredClone(this._prevalueSourceType.settings);
      for (let i = 0; i < this._prevalueSourceType.settings.length; i++) {
        const n = this._prevalueSourceType.settings[i];
        n.defaultValue && (e[n.alias] = n.defaultValue);
      }
      this._settingValues = e;
    }
    this._cachePrevaluesOption === "" && l(this, U, M).call(this);
  }
};
w = /* @__PURE__ */ new WeakSet();
I = async function(t) {
  const e = t.target.value;
  this._settingValues = e;
  const i = await a(this, o).getUpdatedSettingsForPersistence(
    e
  );
  l(this, y, z).call(this, "settings", i);
};
O = /* @__PURE__ */ new WeakSet();
L = function(t) {
  t.stopPropagation(), t.target.tagName === "UUI-RADIO" && (this._cachePrevaluesOption = t.target.value, l(this, _, f).call(this));
};
E = /* @__PURE__ */ new WeakSet();
D = function() {
  m(this, d, [
    {
      label: this.localize.term("formPrevalueSources_cacheOptionsNone"),
      value: "none"
    },
    {
      label: this.localize.term("formPrevalueSources_cacheOptionsTime"),
      value: "time"
    },
    {
      label: this.localize.term("formPrevalueSources_cacheOptionsNoExpiry"),
      value: "noExpiry"
    }
  ]);
};
U = /* @__PURE__ */ new WeakSet();
M = function() {
  var e;
  const t = (e = this._prevalueSource) == null ? void 0 : e.cachePrevaluesFor.toString();
  if (t)
    if (t[0] === "-")
      this._cachePrevaluesOption = "noExpiry";
    else if (t === "00:00:00")
      this._cachePrevaluesOption = "none";
    else {
      this._cachePrevaluesOption = "time";
      const i = t.split(":");
      parseInt(i[0]) > 0 ? (this._cachePrevaluesByTimeValue = parseInt(i[0]), this._cachePrevaluesByTimeUnit = "hours") : parseInt(i[1]) > 0 ? (this._cachePrevaluesByTimeValue = parseInt(i[1]), this._cachePrevaluesByTimeUnit = "minutes") : (this._cachePrevaluesByTimeValue = parseInt(i[2]), this._cachePrevaluesByTimeUnit = "seconds");
    }
  else
    this._cachePrevaluesOption = "noExpiry";
};
_ = /* @__PURE__ */ new WeakSet();
f = function() {
  let t = "";
  switch (this._cachePrevaluesOption) {
    case "none":
      t = "00:00:00.0000000";
      break;
    case "time":
      t = l(this, W, F).call(this);
      break;
    case "noExpiry":
      t = "-00:00:00.0010000";
      break;
  }
  l(this, y, z).call(this, "cachePrevaluesFor", t);
};
W = /* @__PURE__ */ new WeakSet();
F = function() {
  const t = this._cachePrevaluesByTimeValue, e = this._cachePrevaluesByTimeUnit;
  return new Date(
    t * 1e3 * (e === "minutes" || e === "hours" ? 60 : 1) * (e === "hours" ? 60 : 1)
  ).toISOString().slice(11, 19);
};
$ = /* @__PURE__ */ new WeakSet();
A = function(t) {
  const e = t.target.value;
  this._cachePrevaluesByTimeValue = parseInt(e), l(this, _, f).call(this);
};
V = /* @__PURE__ */ new WeakSet();
N = function(t) {
  const e = t.target.value.toString();
  this._cachePrevaluesByTimeUnit = e, l(this, _, f).call(this);
};
y = /* @__PURE__ */ new WeakSet();
z = function(t, e) {
  var i;
  (i = a(this, r)) == null || i.setPrevalueSourceProperty(t, e), this.dispatchEvent(new CustomEvent("valueChange"));
};
u.styles = [
  q`
      :host {
        display: block;
        padding: var(--uui-size-layout-1);
      }

      uui-box + uui-box {
        margin-top: var(--uui-size-layout-1);
      }

      [inert] {
        opacity: 0.5;
      }
    `
];
c([
  h()
], u.prototype, "_prevalueSource", 2);
c([
  h()
], u.prototype, "_prevalueSourceType", 2);
c([
  h()
], u.prototype, "_settingValues", 2);
c([
  h()
], u.prototype, "_settingsConfig", 2);
c([
  h()
], u.prototype, "_settingsConfigLoaded", 2);
c([
  h()
], u.prototype, "_cachePrevaluesOption", 2);
c([
  h()
], u.prototype, "_cachePrevaluesByTimeValue", 2);
c([
  h()
], u.prototype, "_cachePrevaluesByTimeUnit", 2);
u = c([
  G(Z)
], u);
const ae = u;
export {
  u as UmbWorkspaceViewPrevalueSourceDesignElement,
  ae as default
};
//# sourceMappingURL=workspace-view-prevaluesource-design.element.js.map
