import { html as c, unsafeHTML as m, customElement as p } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as h } from "@umbraco-cms/backoffice/modal";
import { b as d } from "./index.js";
import { blobDownload as f } from "@umbraco-cms/backoffice/utils";
var _ = Object.defineProperty, v = Object.getOwnPropertyDescriptor, b = (o, t, e, a) => {
  for (var r = a > 1 ? void 0 : a ? v(t, e) : t, i = o.length - 1, s; i >= 0; i--)
    (s = o[i]) && (r = (a ? s(t, e, r) : s(r)) || r);
  return a && r && _(t, e, r), r;
}, x = (o, t, e) => {
  if (!t.has(o))
    throw TypeError("Cannot " + e);
}, E = (o, t, e) => {
  if (t.has(o))
    throw TypeError("Cannot add the same private member more than once");
  t instanceof WeakSet ? t.add(o) : t.set(o, e);
}, w = (o, t, e) => (x(o, t, "access private method"), e), n, u;
const y = "forms-form-export-modal";
let l = class extends h {
  constructor() {
    super(...arguments), E(this, n);
  }
  render() {
    return c`
      <umb-body-layout .headline=${this.localize.term("actions_export")}>
        <uui-box
          >${m(
      this.localize.term("formExport_exportInstruction")
    )}</uui-box
        >
        <uui-button
          slot="actions"
          label=${this.localize.term("general_cancel")}
          @click="${this._rejectModal}"
        ></uui-button>
        <uui-button
          slot="actions"
          label=${this.localize.term("actions_export")}
          look="primary"
          color="positive"
          @click=${w(this, n, u)}
        ></uui-button>
      </umb-body-layout>
    `;
  }
};
n = /* @__PURE__ */ new WeakSet();
u = async function() {
  var t;
  if (!((t = this.data) != null && t.unique))
    throw new Error("unique is missing");
  const o = await d.getFormExport({ guid: this.data.unique });
  f(o, `form-${this.data.unique}.json`, "application/json"), this._submitModal();
};
l = b([
  p(y)
], l);
const q = l;
export {
  l as FormsFormExportModalElement,
  q as default
};
//# sourceMappingURL=form-export-modal.element.js.map
