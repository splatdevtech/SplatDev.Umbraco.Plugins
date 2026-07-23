import { state as s } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as p } from "@umbraco-cms/backoffice/lit-element";
var F = Object.defineProperty, o = (r, e, d, w) => {
  for (var l = void 0, t = r.length - 1, m; t >= 0; t--)
    (m = r[t]) && (l = m(e, d, l) || l);
  return l && F(e, d, l), l;
};
class a extends p {
  constructor() {
    super(...arguments), this.allowMultiple = !1, this.allowedFolders = [], this.allowedForms = [];
  }
  set config(e) {
    this.allowedFolders = (e == null ? void 0 : e.getValueByAlias("allowedFolders")) || [], this.allowedForms = (e == null ? void 0 : e.getValueByAlias("allowedForms")) || [];
  }
}
o([
  s()
], a.prototype, "allowMultiple");
o([
  s()
], a.prototype, "allowedFolders");
o([
  s()
], a.prototype, "allowedForms");
export {
  a as F
};
//# sourceMappingURL=form-picker-base.element.js.map
