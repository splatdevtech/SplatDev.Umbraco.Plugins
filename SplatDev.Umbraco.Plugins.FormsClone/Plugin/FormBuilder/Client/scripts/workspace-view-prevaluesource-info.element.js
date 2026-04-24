import { UmbTextStyles as b } from "@umbraco-cms/backoffice/style";
import { LitElement as f, html as p, when as n, css as S, state as $, customElement as w } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as E } from "@umbraco-cms/backoffice/element-api";
import { F as x } from "./prevaluesource-workspace.context-token.js";
import { D as v } from "./index.js";
var z = Object.defineProperty, P = Object.getOwnPropertyDescriptor, _ = (e, t, r, i) => {
  for (var o = i > 1 ? void 0 : i ? P(t, r) : t, l = e.length - 1, a; l >= 0; l--)
    (a = e[l]) && (o = (i ? a(t, r, o) : a(o)) || o);
  return i && o && z(t, r, o), o;
}, m = (e, t, r) => {
  if (!t.has(e))
    throw TypeError("Cannot " + r);
}, c = (e, t, r) => (m(e, t, "read from private field"), r ? r.call(e) : t.get(e)), h = (e, t, r) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, r);
}, C = (e, t, r, i) => (m(e, t, "write to private field"), t.set(e, r), r), O = (e, t, r) => (m(e, t, "access private method"), r), u, d, y;
const B = "workspace-view-prevaluesource-info";
let s = class extends E(f) {
  constructor() {
    super(), h(this, d), h(this, u, void 0), this.consumeContext(x, (e) => {
      C(this, u, e), O(this, d, y).call(this);
    });
  }
  render() {
    var e, t;
    return p`
      <uui-box headline=${this.localize.term("general_general")}>
        <umb-property-layout label="Id">
          <div slot="editor">${(e = this._prevalueSource) == null ? void 0 : e.unique}</div>
        </umb-property-layout>
        ${n(
      !((t = c(this, u)) != null && t.getIsNew()),
      () => {
        var r, i, o, l;
        return p`
            <umb-property-layout .label=${this.localize.term("content_createDate")}>
              <umb-localize-date
                slot="editor"
                .date=${(r = this._prevalueSource) == null ? void 0 : r.created}
                .options=${v}
              ></umb-localize-date>
            </umb-property-layout>
            ${n(
          (i = this._prevalueSource) == null ? void 0 : i.createdBy,
          () => {
            var a;
            return p`
                <umb-property-layout .label=${this.localize.term("content_createBy")}>
                  <div slot="editor">${(a = this._prevalueSource) == null ? void 0 : a.createdByName}</div>
                </umb-property-layout>`;
          }
        )}
            <umb-property-layout .label=${this.localize.term("content_updateDate")}>
              <umb-localize-date
                slot="editor"
                .date=${(o = this._prevalueSource) == null ? void 0 : o.updated}
                .options=${v}
              ></umb-localize-date>
            </umb-property-layout>
            ${n(
          (l = this._prevalueSource) == null ? void 0 : l.updatedBy,
          () => {
            var a;
            return p`
                <umb-property-layout .label=${this.localize.term("content_updatedBy")}>
                  <div slot="editor">${(a = this._prevalueSource) == null ? void 0 : a.updatedByName}</div>
                </umb-property-layout>`;
          }
        )}
          `;
      }
    )}
      </uui-box>`;
  }
};
u = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakSet();
y = function() {
  c(this, u) && this.observe(c(this, u).data, (e) => {
    e && (this._prevalueSource = e);
  });
};
s.styles = [
  b,
  S`
			:host {
				display: block;
				padding: var(--uui-size-layout-1);
			}

      uui-box { margin-bottom: 20px }
		`
];
_([
  $()
], s.prototype, "_prevalueSource", 2);
s = _([
  w(B)
], s);
const N = s;
export {
  s as UmbWorkspaceViewPrevalueSourceInfoElement,
  N as default
};
//# sourceMappingURL=workspace-view-prevaluesource-info.element.js.map
