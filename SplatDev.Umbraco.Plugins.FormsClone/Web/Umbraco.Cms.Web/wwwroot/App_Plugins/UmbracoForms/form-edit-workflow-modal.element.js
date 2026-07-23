import { html as h, when as m, css as A, state as f, customElement as P } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as I } from "@umbraco-cms/backoffice/modal";
import { tryExecute as B } from "@umbraco-cms/backoffice/resources";
import { P as R, C as U, F as H, a as q, b as G } from "./index.js";
import { UMB_NOTIFICATION_CONTEXT as K } from "@umbraco-cms/backoffice/notification";
import { UmbId as X } from "@umbraco-cms/backoffice/id";
var J = Object.defineProperty, Q = Object.getOwnPropertyDescriptor, p = (e, t, i, l) => {
  for (var n = l > 1 ? void 0 : l ? Q(t, i) : t, u = e.length - 1, o; u >= 0; u--)
    (o = e[u]) && (n = (l ? o(t, i, n) : o(n)) || n);
  return l && n && J(t, i, n), n;
}, W = (e, t, i) => {
  if (!t.has(e))
    throw TypeError("Cannot " + i);
}, r = (e, t, i) => (W(e, t, "read from private field"), i ? i.call(e) : t.get(e)), s = (e, t, i) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, i);
}, Y = (e, t, i, l) => (W(e, t, "write to private field"), t.set(e, i), i), d = (e, t, i) => (W(e, t, "access private method"), i), a, g, w, E, _, V, v, x, y, z, b, D, k, L, C, F, T, O, $, N, S, M;
const Z = "form-edit-workflow-modal";
let c = class extends I {
  constructor() {
    super(...arguments), s(this, w), s(this, _), s(this, v), s(this, y), s(this, b), s(this, k), s(this, C), s(this, T), s(this, $), s(this, S), s(this, a, void 0), s(this, g, ["onSubmit", "onApprove", "onReject"]), this._settingValues = [], this._settingsConfig = [], this._settingsConfigLoaded = !1, this._conditionEnabled = !1;
  }
  async connectedCallback() {
    var e, t;
    super.connectedCallback(), Y(this, a, new R(
      this,
      this.data.workflowType,
      "formProviderWorkflows"
    )), await r(this, a).loadSettingValueConverterApis(), await d(this, w, E).call(this), this._settingsConfig = await r(this, a).getSettingsConfig(
      this._settingValues,
      this.data.workflowType.settings
    ), this._settingsConfigLoaded = !0, this._conditionEnabled = ((t = (e = this.value) == null ? void 0 : e.condition) == null ? void 0 : t.enabled) ?? !1;
  }
  render() {
    var e, t, i, l, n, u;
    return h`<umb-body-layout
      headline=${this.localize.term("formEdit_editWorkflow")}
    >
      <div id="main">
        <uui-box>
          <umb-property-layout
            alias="name"
            .label=${this.localize.term("formWorkflows_workflowNameLabel")}
            .description=${this.localize.term(
      "formWorkflows_workflowNameDescription"
    )}
          >
            <uui-input
              slot="editor"
              .value=${(e = this.value) == null ? void 0 : e.name}
              @change=${d(this, _, V)}
              label="name"
            ></uui-input>
          </umb-property-layout>

          <umb-property-layout
            alias="workflowType"
            .label=${this.localize.term("formWorkflows_workflowTypeLabel")}
            .description=${this.localize.term(
      "formWorkflows_workflowTypeDescription"
    )}
          >
            <div slot="editor" id="workflowType">
              <span class="icon"
                ><uui-icon name=${((t = this.data) == null ? void 0 : t.workflowType.icon) ?? ""}></uui-icon
              ></span>
              <div class="info">
                <div>${(i = this.data) == null ? void 0 : i.workflowType.name}</div>
                <small>${(l = this.data) == null ? void 0 : l.workflowType.description}</small>
              </div>
            </div>
          </umb-property-layout>

          <umb-property-layout
            alias="active"
            .label=${this.localize.term("formWorkflows_activeLabel")}
            .description=${this.localize.term(
      "formWorkflows_activeDescription"
    )}
          >
            <uui-toggle
              slot="editor"
              ?checked=${(n = this.value) == null ? void 0 : n.active}
              @change=${d(this, y, z)}
              label="${this.localize.term("formWorkflows_activeLabel")}"
            ></uui-toggle>
          </umb-property-layout>

          <umb-property-layout
            alias="sensitiveData"
            .label=${this.localize.term(
      "formWorkflows_includeSensitiveDataLabel"
    )}
            .description=${this.localize.term(
      "formWorkflows_includeSensitiveDataDescription"
    )}
          >
            <uui-toggle
              slot="editor"
              ?checked=${(u = this.value) == null ? void 0 : u.includeSensitiveData}
              @change=${d(this, v, x)}
              label="${this.localize.term(
      "formWorkflows_includeSensitiveDataLabel"
    )}"
            ></uui-toggle>
          </umb-property-layout>

          ${m(
      this.data && !this.data.isNew,
      () => h`<umb-property-layout alias="stage" label="Stage">
              <uui-select
                label="Stage"
                slot="editor"
                @change=${d(this, b, D)}
                .options=${r(this, g).map((o) => ({
        name: o,
        value: o,
        selected: this.value.collectionName === o
      }))}
              >
              </uui-select>
            </umb-property-layout>`
    )}

          ${m(
      r(this, a) && this.value && this._settingsConfigLoaded,
      () => h`
              <umb-property-dataset
                .value=${this._settingValues}
                @change=${d(this, k, L)}
              >
                ${this.data.workflowType.settings.map(
        (o) => h`
                    <umb-property
                      ?inert=${o.isReadOnly}
                      label=${r(this, a).getLocalizedSettingDetail(
          o,
          "Label",
          o.name
        )}
                      description=${r(this, a).getLocalizedSettingDetail(
          o,
          "Description",
          o.description
        )}
                      alias=${o.alias}
                      .config=${r(this, a).getPropertyConfigForSetting(this._settingsConfig, o)}
                      .appearance=${r(this, a).getPropertyAppearanceForSetting(o.view)}
                      property-editor-ui-alias=${o.view}
                    >
                    </umb-property>
                  `
      )}
              </umb-property-dataset>`
    )}

          <umb-property-layout
            alias="conditions"
            .label=${this.localize.term("formConditions_title")}
          >
            <div slot="editor">
              <uui-toggle
                ?checked=${this._conditionEnabled}
                .label=${this._conditionEnabled ? "On" : "Off"}
                @change=${d(this, C, F)}
              ></uui-toggle>
              ${m(
      this._conditionEnabled,
      () => {
        var o;
        return h`<form-edit-conditions
                    .value=${this.value.condition}
                    .fields=${((o = this.data) == null ? void 0 : o.fields) ?? []}
                    .appliedTo=${U.WORKFLOW}
                    @change=${d(this, $, N)}
                  ></form-edit-conditions>`;
      }
    )}
            </div>
          </umb-property-layout>
        </uui-box>
      </div>
      <div slot="actions">
        <uui-button
          label=${this.localize.term("general_close")}
          @click=${this._rejectModal}
        ></uui-button>
        <uui-button
          color="positive"
          look="primary"
          label=${this.localize.term("general_submit")}
          @click=${d(this, S, M)}
        ></uui-button>
      </div>
    </umb-body-layout>`;
  }
};
a = /* @__PURE__ */ new WeakMap();
g = /* @__PURE__ */ new WeakMap();
w = /* @__PURE__ */ new WeakSet();
E = async function() {
  var t;
  const e = { ...this.value.settings };
  (t = this.data) == null || t.workflowType.settings.forEach((i) => {
    e[i.alias] || (e[i.alias] = i.defaultValue);
  }), this._settingValues = await r(this, a).getSettingValues(e);
};
_ = /* @__PURE__ */ new WeakSet();
V = function(e) {
  var i;
  const t = e.target.value.toString();
  (i = this.modalContext) == null || i.updateValue({ name: t });
};
v = /* @__PURE__ */ new WeakSet();
x = function(e) {
  var i;
  const t = e.target.checked;
  (i = this.modalContext) == null || i.updateValue({ includeSensitiveData: t });
};
y = /* @__PURE__ */ new WeakSet();
z = function(e) {
  var i;
  const t = e.target.checked;
  (i = this.modalContext) == null || i.updateValue({ active: t });
};
b = /* @__PURE__ */ new WeakSet();
D = function(e) {
  var i;
  const t = e.target.value.toString();
  (i = this.modalContext) == null || i.updateValue({ collectionName: t });
};
k = /* @__PURE__ */ new WeakSet();
L = async function(e) {
  const t = e.target.value;
  this._settingValues = t;
};
C = /* @__PURE__ */ new WeakSet();
F = function(e) {
  var i;
  this._conditionEnabled = e.target.checked;
  const t = structuredClone(this.value.condition) || d(this, T, O).call(this);
  t.enabled = this._conditionEnabled, (i = this.modalContext) == null || i.updateValue({ condition: t });
};
T = /* @__PURE__ */ new WeakSet();
O = function() {
  return {
    id: X.new(),
    enabled: !1,
    actionType: H.SHOW,
    logicType: q.ALL,
    rules: []
  };
};
$ = /* @__PURE__ */ new WeakSet();
N = function(e) {
  var i;
  const t = e.target.value;
  (i = this.modalContext) == null || i.updateValue({ condition: t });
};
S = /* @__PURE__ */ new WeakSet();
M = async function() {
  var l;
  const e = await r(this, a).getUpdatedSettingsForPersistence(
    this._settingValues,
    this.data.workflowType.settings
  ), t = {
    id: this.data.workflowType.id,
    requestBody: {
      name: this.value.name,
      settings: e
    }
  }, { error: i } = await B(
    G.postFormWorkflowByIdValidateSettings(t)
  );
  if (i) {
    const n = r(this, a).createValidationErrorNotification(
      "formWorkflows_saveFailedTitle",
      i
    );
    (await this.getContext(
      K
    )).peek("danger", n);
  } else
    (l = this.modalContext) == null || l.updateValue({ settings: e }), this._submitModal();
};
c.styles = [
  A`
      [inert] {
        opacity: 0.5;
      }

			#workflowType {
				text-decoration: none;
				color: inherit;
				align-self: stretch;
				line-height: normal;

				display: flex;
				position: relative;
				align-items: center;
			}

      #workflowType .icon {
        margin-right: 10px;
      }

      #workflowType .icon uui-icon {
        width: 1.6em;
        height: 1.6em;
    `
];
p([
  f()
], c.prototype, "_settingValues", 2);
p([
  f()
], c.prototype, "_settingsConfig", 2);
p([
  f()
], c.prototype, "_settingsConfigLoaded", 2);
p([
  f()
], c.prototype, "_conditionEnabled", 2);
c = p([
  P(Z)
], c);
const nt = c;
export {
  c as FormsEditWorkflowModalElement,
  nt as default
};
//# sourceMappingURL=form-edit-workflow-modal.element.js.map
