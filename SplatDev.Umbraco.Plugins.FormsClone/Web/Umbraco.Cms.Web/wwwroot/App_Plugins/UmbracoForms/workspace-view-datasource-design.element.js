import { UmbTextStyles as D } from "@umbraco-cms/backoffice/style";
import { LitElement as V, html as g, when as $, css as E, state as c, customElement as b } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as x } from "@umbraco-cms/backoffice/element-api";
import { j as W, P as k } from "./index.js";
var O = Object.defineProperty, L = Object.getOwnPropertyDescriptor, u = (e, t, i, r) => {
  for (var p = r > 1 ? void 0 : r ? L(t, i) : t, d = e.length - 1, _; d >= 0; d--)
    (_ = e[d]) && (p = (r ? _(t, i, p) : _(p)) || p);
  return r && p && O(t, i, p), p;
}, m = (e, t, i) => {
  if (!t.has(e))
    throw TypeError("Cannot " + i);
}, a = (e, t, i) => (m(e, t, "read from private field"), i ? i.call(e) : t.get(e)), l = (e, t, i) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, i);
}, y = (e, t, i, r) => (m(e, t, "write to private field"), t.set(e, i), i), f = (e, t, i) => (m(e, t, "access private method"), i), o, s, h, S, C, v, T, w, P;
const z = "workspace-view-datasource-design";
let n = class extends x(V) {
  constructor() {
    super(), l(this, S), l(this, v), l(this, w), this._settingValues = [], this._settingsConfig = [], this._settingsConfigLoaded = !1, l(this, o, void 0), l(this, s, void 0), l(this, h, void 0), y(this, h, Promise.all([
      this.consumeContext(W, (e) => {
        y(this, o, e);
      }).asPromise()
    ]));
  }
  async connectedCallback() {
    var t, i;
    if (super.connectedCallback(), await a(this, h), !a(this, o))
      return;
    this._dataSource = a(this, o).getData();
    const e = (t = this._dataSource) == null ? void 0 : t.formDataSourceTypeId;
    e && (this._dataSourceType = await ((i = a(this, o)) == null ? void 0 : i.loadDataSourceType(
      e
    )), this._dataSourceType && (y(this, s, new k(
      this,
      this._dataSourceType,
      "formProviderDataSources"
    )), await a(this, s).loadSettingValueConverterApis(), await f(this, S, C).call(this)));
  }
  render() {
    var e;
    return g` <uui-box>
      <umb-property-layout
        alias="type"
        .label=${this.localize.term("general_type")}
      >
        <div slot="editor">${(e = this._dataSourceType) == null ? void 0 : e.name}</div>
      </umb-property-layout>

      ${$(
      a(this, s) && this._dataSourceType && this._settingsConfigLoaded,
      () => g`<umb-property-dataset
          .value=${this._settingValues}
          @change=${f(this, v, T)}
        >
          ${this._dataSourceType.settings.map(
        (t) => g`
              <umb-property
                ?inert=${t.isReadOnly}
                .label=${a(this, s).getLocalizedSettingDetail(
          t,
          "Label",
          t.name
        )}
                .description=${a(this, s).getLocalizedSettingDetail(
          t,
          "Description",
          t.description
        )}
                alias=${t.alias}
                .config=${a(this, s).getPropertyConfigForSetting(
          this._settingsConfig,
          t
        )}
                .appearance=${a(this, s).getPropertyAppearanceForSetting(
          t.view
        )}
                property-editor-ui-alias=${t.view}
              >
              </umb-property>
            `
      )}
        </umb-property-dataset>`
    )}
    </uui-box>`;
  }
};
o = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakMap();
S = /* @__PURE__ */ new WeakSet();
C = async function() {
  var e;
  if (!(!a(this, o) || !this._dataSource) && (this._settingValues = await a(this, s).getSettingValues(
    this._dataSource.settings
  ), this._settingsConfig = await a(this, s).getSettingsConfig(
    this._settingValues,
    this._dataSourceType.settings
  ), this._settingsConfigLoaded = !0, (e = a(this, o)) != null && e.getIsNew() && this._dataSourceType)) {
    const t = structuredClone(this._dataSourceType.settings);
    for (let i = 0; i < this._dataSourceType.settings.length; i++) {
      const r = this._dataSourceType.settings[i];
      r.defaultValue && (t[r.alias] = r.defaultValue);
    }
    this._settingValues = t;
  }
};
v = /* @__PURE__ */ new WeakSet();
T = async function(e) {
  const t = e.target.value;
  this._settingValues = t;
  const i = await a(this, s).getUpdatedSettingsForPersistence(
    t
  );
  f(this, w, P).call(this, "settings", i);
};
w = /* @__PURE__ */ new WeakSet();
P = function(e, t) {
  var i;
  (i = a(this, o)) == null || i.setDataSourceProperty(e, t), this.dispatchEvent(new CustomEvent("valueChange"));
};
n.styles = [
  D,
  E`
      :host {
        display: flex;
        flex-direction: column;
        margin: var(--uui-size-layout-1);
      }

      [inert] {
        opacity: 0.5;
      }
    `
];
u([
  c()
], n.prototype, "_dataSource", 2);
u([
  c()
], n.prototype, "_dataSourceType", 2);
u([
  c()
], n.prototype, "_settingValues", 2);
u([
  c()
], n.prototype, "_settingsConfig", 2);
u([
  c()
], n.prototype, "_settingsConfigLoaded", 2);
n = u([
  b(z)
], n);
const R = n;
export {
  n as UmbWorkspaceViewDataSourceDesignElement,
  R as default
};
//# sourceMappingURL=workspace-view-datasource-design.element.js.map
