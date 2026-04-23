var S = (o, r, e) => {
  if (!r.has(o))
    throw TypeError("Cannot " + e);
};
var n = (o, r, e) => (S(o, r, "read from private field"), e ? e.call(o) : r.get(o)), f = (o, r, e) => {
  if (r.has(o))
    throw TypeError("Cannot add the same private member more than once");
  r instanceof WeakSet ? r.add(o) : r.set(o, e);
}, g = (o, r, e, t) => (S(o, r, "write to private field"), t ? t.call(o, e) : r.set(o, e), e);
var _ = (o, r, e) => (S(o, r, "access private method"), e);
import { UmbSubmittableWorkspaceContextBase as A } from "@umbraco-cms/backoffice/workspace";
import { UmbObjectState as U } from "@umbraco-cms/backoffice/observable-api";
import { c as x, u as D, v as M, w as z, x as V, y as B, G as L, z as G } from "./index.js";
import { UMB_ACTION_EVENT_CONTEXT as $ } from "@umbraco-cms/backoffice/action";
import { UmbRequestReloadChildrenOfEntityEvent as X, UmbRequestReloadStructureForEntityEvent as Y } from "@umbraco-cms/backoffice/entity-action";
import { tryExecuteAndNotify as H } from "@umbraco-cms/backoffice/resources";
import { UmbPropertyEditorConfigCollection as K } from "@umbraco-cms/backoffice/property-editor";
import "@umbraco-cms/backoffice/repository";
import "@umbraco-cms/backoffice/id";
import { FormsPrevalueSourceCollectionRepository as J } from "./prevaluesource-collection.repository.js";
import { UmbElementMixin as Q } from "@umbraco-cms/backoffice/element-api";
import { LitElement as Z, html as j, css as ee, state as te, property as se, customElement as ie } from "@umbraco-cms/backoffice/external/lit";
import { UMB_NOTIFICATION_CONTEXT as oe } from "@umbraco-cms/backoffice/notification";
import { UmbLocalizationController as re } from "@umbraco-cms/backoffice/localization-api";
var ae = Object.defineProperty, ne = Object.getOwnPropertyDescriptor, R = (o, r, e, t) => {
  for (var s = t > 1 ? void 0 : t ? ne(r, e) : r, i = o.length - 1, a; i >= 0; i--)
    (a = o[i]) && (s = (t ? a(r, e, s) : a(s)) || s);
  return t && s && ae(r, e, s), s;
}, k = (o, r, e) => {
  if (!r.has(o))
    throw TypeError("Cannot " + e);
}, T = (o, r, e) => (k(o, r, "read from private field"), r.get(o)), E = (o, r, e) => {
  if (r.has(o))
    throw TypeError("Cannot add the same private member more than once");
  r instanceof WeakSet ? r.add(o) : r.set(o, e);
}, le = (o, r, e, t) => (k(o, r, "write to private field"), r.set(o, e), e), O = (o, r, e) => (k(o, r, "access private method"), e), v, q, I, W, b;
const ue = "forms-form-workspace-editor";
let y = class extends Q(Z) {
  constructor() {
    super(), E(this, q), E(this, W), this._formName = "", E(this, v, void 0), this.consumeContext(x, (o) => {
      le(this, v, o), O(this, q, I).call(this);
    });
  }
  render() {
    return j` <umb-workspace-editor alias="Forms.Workspace.Form">
      <uui-input
        slot="header"
        id="nameInput"
        label=${this.localize.term("placeholders_entername")}
        placeholder=${this.localize.term("placeholders_entername")}
        required
        .value=${this._formName}
        @input="${O(this, W, b)}"
      ></uui-input>
    </umb-workspace-editor>`;
  }
};
v = /* @__PURE__ */ new WeakMap();
q = /* @__PURE__ */ new WeakSet();
I = function() {
  T(this, v) && this.observe(T(this, v).data, (o) => this._formName = (o == null ? void 0 : o.name) ?? "");
};
W = /* @__PURE__ */ new WeakSet();
b = function(o) {
  var r;
  (r = T(this, v)) == null || r.setName(o.target.value.toString());
};
y.styles = [
  ee`
        :host {
            display: block;
            width: 100%;
            height: 100%;
        }

        #nameInput {
            flex: 1 1 auto;
        }
    `
];
R([
  te()
], y.prototype, "_formName", 2);
R([
  se({ type: String, attribute: !1 })
], y.prototype, "workspaceAlias", 2);
y = R([
  ie(ue)
], y);
const P = y;
var m, l, w, d, C, N;
class pe extends A {
  constructor(e) {
    super(e, D);
    f(this, C);
    f(this, m, void 0);
    f(this, l, void 0);
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    f(this, w, void 0);
    f(this, d, void 0);
    this.formRepository = new M(this), this.fieldTypeRepository = new z(this), this.workflowTypeRepository = new V(this), this.prevalueSourceCollectionRepository = new J(this), g(this, l, new U(void 0)), this.data = n(this, l).asObservable(), this.unique = n(this, l).asObservablePart((t) => t == null ? void 0 : t.unique), this.name = n(this, l).asObservablePart((t) => t == null ? void 0 : t.name), this.id = n(this, l).asObservablePart((t) => t == null ? void 0 : t.unique), g(this, d, async (t, s) => {
      const i = s.match.params.entityType, a = s.match.params.parentUnique === "null" ? null : s.match.params.parentUnique, u = s.match.params.template;
      await this.create({ entityType: i, unique: a }, u);
    }), this.getConditionActionTypes = [{
      name: "Show",
      value: "Show"
    }, {
      name: "Hide",
      value: "Hide"
    }], this.getConditionLogicTypes = [{
      name: "all",
      value: "All"
    }, {
      name: "any",
      value: "Any"
    }], this.getConditionOperators = [
      {
        name: "is",
        value: "Is"
      },
      {
        name: "is not",
        value: "IsNot"
      },
      {
        name: "is greater than",
        value: "GreaterThen"
      },
      {
        name: "is less than",
        value: "LessThen"
      },
      {
        name: "contains (case-sensitive)",
        value: "Contains"
      },
      {
        name: "contains (case-insensitive)",
        value: "ContainsIgnoreCase"
      },
      {
        name: "starts with (case-sensitive)",
        value: "StartsWith"
      },
      {
        name: "starts with (case-insensitive)",
        value: "StartsWithIgnoreCase"
      },
      {
        name: "ends with",
        value: "EndsWith"
      },
      {
        name: "ends with (case-insensitive)",
        value: "EndsWithIgnoreCase"
      },
      {
        name: "does not contain (case-sensitive)",
        value: "NotContains"
      },
      {
        name: "does not contain (case-insensitive)",
        value: "NotContainsIgnoreCase"
      },
      {
        name: "does not start with (case-sensitive)",
        value: "NotStartsWith"
      },
      {
        name: "does not start with (case-insensitive)",
        value: "NotStartsWithIgnoreCase"
      },
      {
        name: "does not end with (case-sensitive)",
        value: "NotEndsWith"
      },
      {
        name: "does not end with (case-insensitive)",
        value: "NotEndsWithIgnoreCase"
      }
    ], this.routes.setRoutes([
      {
        path: "create/parent/:entityType/:parentUnique",
        component: P,
        setup: n(this, d)
      },
      {
        path: "createFromTemplate/parent/:entityType/:parentUnique/:template",
        component: P,
        setup: n(this, d)
      },
      {
        path: "edit/:unique",
        component: P,
        setup: (t, s) => {
          const i = s.match.params.unique;
          this.load(i);
        }
      }
    ]);
  }
  async load(e) {
    var s;
    g(this, w, (s = this.formRepository) == null ? void 0 : s.requestByUnique(e));
    const { data: t } = await n(this, w);
    t && (this.setIsNew(!1), n(this, l).update(t));
  }
  async create(e, t) {
    g(this, m, e);
    let i = (await this.formRepository.requestFormScaffold(t || "")).data.data;
    if (!i)
      throw new Error("Form design data is missing");
    return i.folderId = e.unique, this.modalContext && (i = { ...i, ...this.modalContext.data.preset }), this.setIsNew(!0), n(this, l).setValue(i), { data: i };
  }
  async submit() {
    var t, s;
    if (!n(this, l).value || !n(this, l).value.unique)
      return;
    if (n(this, l).value.name.trim().length === 0) {
      const i = await this.getContext(
        oe
      ), a = new re(this);
      i.peek("danger", {
        data: { message: a.term("formEdit_noNameForForm") }
      });
      return;
    }
    const e = await this.getContext($);
    if (this.getIsNew()) {
      if (!n(this, m))
        throw new Error("Parent is not set");
      await ((t = this.formRepository) == null ? void 0 : t.create(n(this, l).value, n(this, m).unique));
      const i = new X({
        entityType: n(this, m).entityType,
        unique: n(this, m).unique
      });
      e.dispatchEvent(i), this.setIsNew(!1);
    } else {
      await ((s = this.formRepository) == null ? void 0 : s.save(n(this, l).value));
      const i = new Y({
        unique: this.getUnique(),
        entityType: this.getEntityType()
      });
      e.dispatchEvent(i);
    }
  }
  async loadFieldType(e) {
    const { data: t } = await this.fieldTypeRepository.requestByUnique(e);
    return t;
  }
  async loadWorkflowType(e) {
    const { data: t } = await this.workflowTypeRepository.requestByUnique(e);
    return t;
  }
  async loadValidationPatterns() {
    const { data: e } = await this.fieldTypeRepository.requestValidationPatterns();
    return e;
  }
  async loadPrevalueSources() {
    const { data: e } = await this.prevalueSourceCollectionRepository.requestCollection({});
    return e == null ? void 0 : e.items;
  }
  getData() {
    return n(this, l).getValue();
  }
  getUnique() {
    var e;
    return ((e = this.getData()) == null ? void 0 : e.unique) || "";
  }
  getEntityType() {
    return B;
  }
  getName() {
    var e;
    return (e = n(this, l).getValue()) == null ? void 0 : e.name;
  }
  setName(e) {
    n(this, l).update({ name: e });
  }
  setFormProperty(e, t) {
    n(this, l).update({ [e]: t });
  }
  getFormProperty(e) {
    var t;
    return (t = this.getData()) == null ? void 0 : t[e];
  }
  setPageProperty(e, t, s) {
    var a;
    const i = (a = n(this, l).value) == null ? void 0 : a.pages.map((u, p) => ({
      ...u,
      ...p === e && {
        [t]: s
      }
    }));
    this.setFormProperty("pages", i);
  }
  setFieldsetProperty(e, t, s, i) {
    var u;
    const a = (u = n(this, l).value) == null ? void 0 : u.pages[e].fieldSets.map((p, c) => ({
      ...p,
      ...c === t && {
        [s]: i
      }
    }));
    this.setPageProperty(e, "fieldSets", a);
  }
  setContainerProperty(e, t, s, i, a) {
    var p;
    const u = (p = n(this, l).value) == null ? void 0 : p.pages[e].fieldSets[t].containers.map((c, h) => ({
      ...c,
      ...h === s && {
        [i]: a
      }
    }));
    this.setFieldsetProperty(e, t, "containers", u);
  }
  setFieldProperty(e, t, s, i, a, u) {
    var c;
    const p = (c = n(this, l).value) == null ? void 0 : c.pages[e].fieldSets[t].containers[s].fields.map((h, F) => ({
      ...h,
      ...F === i && {
        [a]: u
      }
    }));
    this.setContainerProperty(e, t, s, "fields", p);
  }
  setWorkflowProperty(e, t, s, i) {
    var u;
    const a = structuredClone((u = n(this, l).value) == null ? void 0 : u.formWorkflows);
    a && (a[e][t][s] = i, this.setFormProperty("formWorkflows", a));
  }
  moveFieldSet(e, t, s) {
    var a;
    if (s === e)
      return;
    const i = structuredClone((a = n(this, l).value) == null ? void 0 : a.pages);
    if (i) {
      const p = i[e].fieldSets.splice(t, 1)[0];
      i[s].fieldSets.push(p), this.setFormProperty("pages", i);
    }
  }
  moveField(e, t, s) {
    var a;
    if (t === s)
      return;
    const i = structuredClone((a = n(this, l).value) == null ? void 0 : a.pages);
    if (i) {
      const u = _(this, C, N).call(this, i, t), p = _(this, C, N).call(this, i, s), c = u.fields.findIndex((F) => F.id == e), h = u.fields.splice(c, 1)[0];
      p.fields.push(h), this.setFormProperty("pages", i);
    }
  }
  moveWorkflow(e, t, s) {
    if (!n(this, l).value)
      return;
    const i = structuredClone(n(this, l).value.formWorkflows), u = i[t].splice(e, 1)[0];
    i[s].push(u), this.setFormProperty("formWorkflows", i);
  }
  getAllPages() {
    const e = this.getData();
    if (!e)
      return [];
    const t = [];
    return e.pages.forEach((s) => {
      t.push(s);
    }), t;
  }
  getAllContainers() {
    const e = this.getData();
    if (!e)
      return [];
    const t = [];
    return e.pages.forEach((s) => {
      s.fieldSets.forEach((i) => {
        i.containers.forEach((a) => {
          t.push(a);
        });
      });
    }), t;
  }
  getAllFields() {
    const e = this.getAllContainers(), t = [];
    return e.forEach((s) => {
      s.fields.forEach((i) => {
        t.push(i);
      });
    }), t;
  }
  getAllFieldAliases() {
    return this.getAllFields().map((e) => e.alias);
  }
  getFieldsWithPrevalues() {
    return this.getAllFields().filter((s) => s.preValues.length > 0 || s.prevalueSourceId !== L).map((s) => ({
      id: s.id,
      prevalues: s.preValues,
      prevalueSourceId: s.prevalueSourceId
    }));
  }
  getContainerIndexPathForField(e) {
    const t = this.getData();
    if (!t)
      return "";
    for (let s = 0; s < t.pages.length; s++) {
      const i = t.pages[s];
      for (let a = 0; a < i.fieldSets.length; a++) {
        const u = i.fieldSets[a];
        for (let p = 0; p < u.containers.length; p++) {
          const c = u.containers[p];
          for (let h = 0; h < c.fields.length; h++)
            if (c.fields[h].id === e)
              return s + "_" + a + "_" + p;
        }
      }
    }
    return "";
  }
  async getRichTextConfiguration() {
    const { data: e } = await H(
      this,
      G.getFieldTypeRichtextDatatype()
    ), t = new K([]);
    return t.push({
      alias: "maxImageSize",
      value: e.configurationData.maxImageSize
    }), t.push({
      alias: "overlaySize",
      value: e.configurationData.overlaySize
    }), t.push({
      alias: "toolbar",
      value: e.configurationData.toolbar
    }), t.push({
      alias: "extensions",
      value: e.configurationData.extensions
    }), t;
  }
  destroy() {
    n(this, l).destroy(), super.destroy();
  }
}
m = new WeakMap(), l = new WeakMap(), w = new WeakMap(), d = new WeakMap(), C = new WeakSet(), N = function(e, t) {
  const s = t.split("_").map((i) => parseInt(i));
  return e[s[0]].fieldSets[s[1]].containers[s[2]];
};
const Te = pe;
export {
  pe as FormsFormWorkspaceContext,
  Te as api
};
//# sourceMappingURL=form-workspace.context.js.map
