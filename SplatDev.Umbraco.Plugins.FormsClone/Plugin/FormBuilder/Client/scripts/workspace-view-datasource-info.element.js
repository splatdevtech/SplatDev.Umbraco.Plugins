import { UmbTextStyles as b } from "@umbraco-cms/backoffice/style";
import { LitElement as f, html as p, when as d, css as S, state as $, customElement as w } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as D } from "@umbraco-cms/backoffice/element-api";
import { j as x, D as h } from "./index.js";
var z = Object.defineProperty, E = Object.getOwnPropertyDescriptor, y = (t, e, a, i) => {
  for (var r = i > 1 ? void 0 : i ? E(e, a) : e, s = t.length - 1, o; s >= 0; s--)
    (o = t[s]) && (r = (i ? o(e, a, r) : o(r)) || r);
  return i && r && z(e, a, r), r;
}, m = (t, e, a) => {
  if (!e.has(t))
    throw TypeError("Cannot " + a);
}, n = (t, e, a) => (m(t, e, "read from private field"), a ? a.call(t) : e.get(t)), _ = (t, e, a) => {
  if (e.has(t))
    throw TypeError("Cannot add the same private member more than once");
  e instanceof WeakSet ? e.add(t) : e.set(t, a);
}, C = (t, e, a, i) => (m(t, e, "write to private field"), e.set(t, a), a), O = (t, e, a) => (m(t, e, "access private method"), a), l, c, v;
const T = "workspace-view-datasource-info";
let u = class extends D(f) {
  constructor() {
    super(), _(this, c), _(this, l, void 0), this.consumeContext(x, (t) => {
      C(this, l, t), O(this, c, v).call(this);
    });
  }
  render() {
    var t, e;
    return p`
      <uui-box headline=${this.localize.term("general_general")}>
        <umb-property-layout label="Id">
          <div slot="editor">${(t = this._dataSource) == null ? void 0 : t.unique}</div>
        </umb-property-layout>
        ${d(
      !((e = n(this, l)) != null && e.getIsNew()),
      () => {
        var a, i, r, s;
        return p`
            <umb-property-layout .label=${this.localize.term("content_createDate")}>
              <umb-localize-date
                slot="editor"
                .date=${(a = this._dataSource) == null ? void 0 : a.created}
                .options=${h}
              ></umb-localize-date>
            </umb-property-layout>
            ${d(
          (i = this._dataSource) == null ? void 0 : i.createdBy,
          () => {
            var o;
            return p`
                <umb-property-layout .label=${this.localize.term("content_createBy")}>
                  <div slot="editor">${(o = this._dataSource) == null ? void 0 : o.createdByName}</div>
                </umb-property-layout>`;
          }
        )}
            <umb-property-layout .label=${this.localize.term("content_updateDate")}>
              <umb-localize-date
                slot="editor"
                .date=${(r = this._dataSource) == null ? void 0 : r.updated}
                .options=${h}
              ></umb-localize-date>
            </umb-property-layout>
            ${d(
          (s = this._dataSource) == null ? void 0 : s.updatedBy,
          () => {
            var o;
            return p`
                <umb-property-layout .label=${this.localize.term("content_updatedBy")}>
                  <div slot="editor">${(o = this._dataSource) == null ? void 0 : o.updatedByName}</div>
                </umb-property-layout>`;
          }
        )}
          `;
      }
    )}
      </uui-box>`;
  }
};
l = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
v = function() {
  n(this, l) && this.observe(n(this, l).data, (t) => {
    t && (this._dataSource = t);
  });
};
u.styles = [
  b,
  S`
			:host {
				display: block;
				padding: var(--uui-size-layout-1);
			}

      uui-box { margin-bottom: 20px }
		`
];
y([
  $()
], u.prototype, "_dataSource", 2);
u = y([
  w(T)
], u);
const P = u;
export {
  u as UmbWorkspaceViewDataSourceInfoElement,
  P as default
};
//# sourceMappingURL=workspace-view-datasource-info.element.js.map
