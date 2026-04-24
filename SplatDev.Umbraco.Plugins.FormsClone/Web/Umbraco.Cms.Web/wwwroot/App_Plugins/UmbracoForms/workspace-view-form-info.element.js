import { LitElement as g, html as s, when as d, css as N, state as v, customElement as x } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as E } from "@umbraco-cms/backoffice/element-api";
import { c as T, D as f, b as O } from "./index.js";
import { tryExecuteAndNotify as C } from "@umbraco-cms/backoffice/resources";
var F = Object.defineProperty, S = Object.getOwnPropertyDescriptor, b = (e, t, a, l) => {
  for (var i = l > 1 ? void 0 : l ? S(t, a) : t, r = e.length - 1, u; r >= 0; r--)
    (u = e[r]) && (i = (l ? u(t, a, i) : u(i)) || i);
  return l && i && F(t, a, i), i;
}, _ = (e, t, a) => {
  if (!t.has(e))
    throw TypeError("Cannot " + a);
}, p = (e, t, a) => (_(e, t, "read from private field"), a ? a.call(e) : t.get(e)), h = (e, t, a) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, a);
}, B = (e, t, a, l) => (_(e, t, "write to private field"), t.set(e, a), a), $ = (e, t, a) => (_(e, t, "access private method"), a), o, m, w, y, z;
const W = "workspace-view-form-info";
let n = class extends E(g) {
  constructor() {
    super(), h(this, m), h(this, y), this._relations = [], h(this, o, void 0), this.consumeContext(T, (e) => {
      B(this, o, e), $(this, m, w).call(this);
    });
  }
  render() {
    var e, t, a;
    return s` <uui-box .headline=${this.localize.term("general_general")}>
        <umb-property-layout .label=${this.localize.term("general_id")}>
          <span slot="editor">${(e = this._form) == null ? void 0 : e.unique}</span>
        </umb-property-layout>
        ${d(
      !((t = p(this, o)) != null && t.getIsNew()),
      () => {
        var l, i, r, u;
        return s`
            <umb-property-layout .label=${this.localize.term("content_createDate")}>
              <umb-localize-date
                slot="editor"
                .date=${(l = this._form) == null ? void 0 : l.created}
                .options=${f}
              ></umb-localize-date>
            </umb-property-layout>
            ${d(
          (i = this._form) == null ? void 0 : i.createdBy,
          () => {
            var c;
            return s`
                <umb-property-layout .label=${this.localize.term("content_createBy")}>
                  <div slot="editor">${(c = this._form) == null ? void 0 : c.createdByName}</div>
                </umb-property-layout>`;
          }
        )}
            <umb-property-layout .label=${this.localize.term("content_updateDate")}>
              <umb-localize-date
                slot="editor"
                .date=${(r = this._form) == null ? void 0 : r.updated}
                .options=${f}
              ></umb-localize-date>
            </umb-property-layout>
            ${d(
          (u = this._form) == null ? void 0 : u.updatedBy,
          () => {
            var c;
            return s`
                <umb-property-layout .label=${this.localize.term("content_updatedBy")}>
                  <div slot="editor">${(c = this._form) == null ? void 0 : c.updatedByName}</div>
                </umb-property-layout>`;
          }
        )}
          `;
      }
    )}
      </uui-box>
      ${d(
      !((a = p(this, o)) != null && a.getIsNew()),
      () => s`
          <uui-box headline=${this.localize.term("formSettings_referencesTitle")}>
            <uui-table>
              <uui-table-head>
                <uui-table-head-cell
                  >${this.localize.term(
        "general_nodeName"
      )}</uui-table-head-cell
                >
                <uui-table-head-cell
                  >${this.localize.term(
        "general_status"
      )}</uui-table-head-cell
                >
                <uui-table-head-cell
                  >${this.localize.term(
        "general_typeName"
      )}</uui-table-head-cell
                >
                <uui-table-head-cell
                  >${this.localize.term(
        "general_type"
      )}</uui-table-head-cell
                >
                <uui-table-head-cell
                  >${this.localize.term(
        "relationType_relation"
      )}</uui-table-head-cell
                >
              </uui-table-head>
              ${this._relations.map(
        (l) => s`<uui-table-row>
                    <uui-table-cell>${l.nodeName}</uui-table-cell>
                    <uui-table-cell>${this.localize.term(
          "content_" + (l.nodePublished ? "published" : "unpublished")
        )}</uui-table-cell>
                    <uui-table-cell>${l.contentTypeName}</uui-table-cell>
                    <uui-table-cell>${l.nodeType}</uui-table-cell>
                    <uui-table-cell>${l.relationTypeName}</uui-table-cell>
                  </uui-table-row>`
      )}
            </uui-table>
          </uui-box>`
    )}
      `;
  }
};
o = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakSet();
w = function() {
  p(this, o) && this.observe(p(this, o).data, async (e) => {
    var t;
    e && (this._form = e, (t = p(this, o)) != null && t.getIsNew() || await $(this, y, z).call(this, e.id));
  });
};
y = /* @__PURE__ */ new WeakSet();
z = async function(e) {
  const { data: t } = await C(
    this,
    O.getFormByIdRelations({
      id: e
    })
  );
  t && (this._relations = t.items);
};
n.styles = [
  N`
      :host {
        display: block;
        padding: var(--uui-size-layout-1);
      }

      uui-box + uui-box {
        margin-top: var(--uui-size-layout-1);
      }
    `
];
b([
  v()
], n.prototype, "_form", 2);
b([
  v()
], n.prototype, "_relations", 2);
n = b([
  x(W)
], n);
const k = n;
export {
  n as UmbWorkspaceViewFormInfoElement,
  k as default
};
//# sourceMappingURL=workspace-view-form-info.element.js.map
