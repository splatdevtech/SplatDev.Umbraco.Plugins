import { UmbTextStyles as V } from "@umbraco-cms/backoffice/style";
import { LitElement as B, html as p, when as b, css as j, state as W, queryAsync as G, customElement as X } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as K } from "@umbraco-cms/backoffice/element-api";
import "@umbraco-cms/backoffice/class-api";
import "@umbraco-cms/backoffice/resources";
import "@umbraco-cms/backoffice/observable-api";
import { c as L, d as Q, B as Y, H as Z, I as ee } from "./index.js";
import "@umbraco-cms/backoffice/current-user";
var te = Object.defineProperty, ie = Object.getOwnPropertyDescriptor, k = (e, t, i, o) => {
  for (var a = o > 1 ? void 0 : o ? ie(t, i) : t, u = e.length - 1, n; u >= 0; u--)
    (n = e[u]) && (a = (o ? n(t, i, a) : n(a)) || a);
  return o && a && te(t, i, a), a;
}, E = (e, t, i) => {
  if (!t.has(e))
    throw TypeError("Cannot " + i);
}, r = (e, t, i) => (E(e, t, "read from private field"), i ? i.call(e) : t.get(e)), s = (e, t, i) => {
  if (t.has(e))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(e) : t.set(e, i);
}, c = (e, t, i, o) => (E(e, t, "write to private field"), t.set(e, i), i), l = (e, t, i) => (E(e, t, "access private method"), i), m, y, _, P, f, T, M, I, d, S, x, U, v, w, g, C, D, F, q, A, H, O, N, $, z, R, J;
const oe = "workspace-view-form-design";
let h = class extends K(B) {
  constructor() {
    super(), s(this, M), s(this, d), s(this, x), s(this, C), s(this, F), s(this, A), s(this, O), s(this, $), s(this, R), this._sortModeActive = !1, s(this, m, void 0), s(this, y, void 0), s(this, _, void 0), s(this, P, void 0), s(this, f, []), s(this, T, []), s(this, v, -1), s(this, w, !1), s(this, g, -1), c(this, P, Promise.all([
      this.consumeContext(L, (e) => {
        c(this, m, e);
      }).asPromise(),
      this.consumeContext(Q, (e) => {
        c(this, y, e);
      }).asPromise()
    ])), l(this, M, I).call(this);
  }
  render() {
    var e, t;
    return p`
      <div id="formHeader">
        <div class="actions">
          ${b(
      !this._sortModeActive,
      () => p`
              <uui-button-group>
                <uui-button
                  look="placeholder"
                  label=${this.localize.term("formEdit_addToTop")}
                  @click=${() => l(this, d, S).call(this, !0)}
                >
                  ${this.localize.term("formEdit_addToTop")}
                </uui-button>
                <uui-button
                  look="placeholder"
                  label=${this.localize.term("formEdit_addToBottom")}
                  @click=${() => l(this, d, S).call(this, !1)}
                >
                  ${this.localize.term("formEdit_addToBottom")}
                </uui-button>
              </uui-button-group>`
    )}

          <uui-button
            @click=${l(this, x, U)}
            compact
            look="outline"
            label=${this.localize.term(
      this._sortModeActive ? "general_reorderDone" : "general_reorder"
    )}
          >
            <uui-icon name="icon-navigation"></uui-icon>
            ${this.localize.term(
      this._sortModeActive ? "general_reorderDone" : "general_reorder"
    )}
          </uui-button>

          ${b(
      this._form && this._form.pages.length > 1,
      () => p`
              <uui-select
                label=${this.localize.term("formEdit_jumpToPage")}
                name="jumpToPage"
                @change=${l(this, O, N)}
                .options=${l(this, A, H).call(this)}
              >
              </uui-select>
            `
    )}
        </div>
      </div>

      <div id="formPages">
        ${(e = this._form) == null ? void 0 : e.pages.map(
      (i, o) => p`<forms-form-page
              class="sort-mode-${this._sortModeActive ? "active" : "inactive"}"
              data-page-index="${o}"
              .page=${i}
              .index=${o}
              .allFields=${r(this, f)}
              .allFieldTypes=${r(this, T)}
              ?sort-mode-active=${this._sortModeActive}
            >
            </forms-form-page>`
    )}
      </div>

      <div id="formFooter">
        ${b(
      !this._sortModeActive,
      () => p` <uui-button
            @click=${() => l(this, d, S).call(this, !1)}
            look="outline"
            label=${this.localize.term("formEdit_addPage")}
          ></uui-button>`
    )}
      </div>

      ${b(
      !this._sortModeActive && ((t = this._userSecurity) == null ? void 0 : t.userSecurity.manageWorkflows),
      // TODO: <-- ths doesn't work when we have group security enabled.
      () => {
        var i, o, a, u, n;
        return p` <forms-form-workflow-summary
          .workflows=${(i = this._form) == null ? void 0 : i.formWorkflows}
          .submitMessageDetail=${{
          messageOnSubmit: (o = this._form) == null ? void 0 : o.messageOnSubmit,
          messageOnSubmitIsHtml: ((a = this._form) == null ? void 0 : a.messageOnSubmitIsHtml) ?? !1,
          goToPageOnSubmit: (u = this._form) == null ? void 0 : u.goToPageOnSubmit
        }}
          ?manualApproval=${(n = this._form) == null ? void 0 : n.manualApproval}
          .allFields=${r(this, f)}
        ></forms-form-workflow-summary>`;
      }
    )}
    `;
  }
};
m = /* @__PURE__ */ new WeakMap();
y = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakMap();
P = /* @__PURE__ */ new WeakMap();
f = /* @__PURE__ */ new WeakMap();
T = /* @__PURE__ */ new WeakMap();
M = /* @__PURE__ */ new WeakSet();
I = async function() {
  if (await r(this, P), !r(this, m))
    return;
  c(this, _, new Y(
    this,
    r(this, m)
  )), this.provideContext(Z, r(this, _));
  const e = new ee(this), { data: t } = await e.requestCollection();
  c(this, T, (t == null ? void 0 : t.items) || []), this.observe(r(this, m).data, (i) => {
    var o;
    i && (this._form = i, r(this, m) && c(this, f, (o = r(this, m)) == null ? void 0 : o.getAllFields()), l(this, C, D).call(this));
  }), this.observe(r(this, y).userSecurity, (i) => {
    this._userSecurity = i;
  });
};
d = /* @__PURE__ */ new WeakSet();
S = function(e) {
  var t;
  (t = r(this, _)) == null || t.addFormPage(e);
};
x = /* @__PURE__ */ new WeakSet();
U = function() {
  this._sortModeActive = !this._sortModeActive, l(this, F, q).call(this);
};
v = /* @__PURE__ */ new WeakMap();
w = /* @__PURE__ */ new WeakMap();
g = /* @__PURE__ */ new WeakMap();
C = /* @__PURE__ */ new WeakSet();
D = async function() {
  if (!this._form || (await this._pagesPromise, r(this, g) > 0 && this._form.pages.length === r(this, g)))
    return;
  const e = {
    root: null,
    threshold: 0.1
  }, t = (a) => {
    r(this, w) || a.filter((n) => n.isIntersecting).map((n) => n.target.getAttribute("data-page-index")).forEach((n) => {
      n && c(this, v, n);
    });
  }, i = new IntersectionObserver(t, e), o = this.renderRoot.querySelectorAll("forms-form-page");
  c(this, g, o.length), o.forEach((a) => {
    i.observe(a);
  });
};
F = /* @__PURE__ */ new WeakSet();
q = function() {
  if (r(this, v) >= 0) {
    const e = this.shadowRoot.querySelector(
      "forms-form-page[data-page-index='" + r(this, v) + "']"
    );
    e && l(this, $, z).call(this, e);
  }
};
A = /* @__PURE__ */ new WeakSet();
H = function() {
  var t;
  const e = ((t = this._form) == null ? void 0 : t.pages.map((i, o) => ({
    name: (o + 1).toString(),
    value: o.toString(),
    selected: !1
  }))) ?? [];
  return e.unshift({
    name: this.localize.term("formEdit_jumpToPage"),
    value: "",
    selected: !0
  }), e;
};
O = /* @__PURE__ */ new WeakSet();
N = function(e) {
  const t = e.target, i = t.value.toString();
  if (i !== "") {
    const o = this.shadowRoot.querySelector(
      "forms-form-page[data-page-index='" + i + "']"
    );
    o && l(this, $, z).call(this, o);
  }
  l(this, R, J).call(this, t);
};
$ = /* @__PURE__ */ new WeakSet();
z = function(e) {
  c(this, w, !0), e.scrollIntoView(), c(this, w, !1);
};
R = /* @__PURE__ */ new WeakSet();
J = function(e) {
  for (let t = 0; t < e.options.length; t++) {
    const i = e.options[t];
    i.selected = !1;
  }
  e.options[0].selected = !0, this.requestUpdate();
};
h.styles = [
  V,
  j`
      :host {
        display: block;
        padding: var(--uui-size-layout-1);
      }

      #formHeader {
        position: sticky;
        top: 0;
        border: solid 1px #ccc;
        padding: 10px;
        background-color: #fff;
        z-index: 9;
      }

      #formHeader,
      actions {
        margin-bottom: var(--uui-size-5);
        display: flex;
        justify-content: flex-end;
        column-gap: var(--uui-size-3);
      }

      forms-form-page.sort-mode-inactive {
        scroll-margin-top: 80px;
      }

      forms-form-page.sort-mode-active {
        scroll-margin-top: 30px;
      }

      #formFooter {
        display: flex;
        align-items: center;
        justify-content: center;
        margin-bottom: 30px;
      }

      uui-button uui-icon {
        margin-right: var(--uui-size-2);
      }
    `
];
k([
  W()
], h.prototype, "_form", 2);
k([
  W()
], h.prototype, "_userSecurity", 2);
k([
  W()
], h.prototype, "_sortModeActive", 2);
k([
  G("forms-form-page")
], h.prototype, "_pagesPromise", 2);
h = k([
  X(oe)
], h);
const ue = h;
export {
  h as UmbWorkspaceViewFormDesignElement,
  ue as default
};
//# sourceMappingURL=workspace-view-form-design.element.js.map
