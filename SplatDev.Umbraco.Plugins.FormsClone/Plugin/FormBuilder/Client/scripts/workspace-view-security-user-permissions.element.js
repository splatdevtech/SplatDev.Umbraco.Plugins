import { html as u, state as p, customElement as m } from "@umbraco-cms/backoffice/external/lit";
import { F as l } from "./security-user-workspace.context-token.js";
import { U as h } from "./workspace-view-security-permissions-base.element.js";
var y = Object.defineProperty, f = Object.getOwnPropertyDescriptor, a = (e, t, s, i) => {
  for (var r = i > 1 ? void 0 : i ? f(t, s) : t, o = e.length - 1, n; o >= 0; o--)
    (n = e[o]) && (r = (i ? n(t, s, r) : n(r)) || r);
  return i && r && y(t, s, r), r;
};
const S = "workspace-view-security-user-permissions";
let c = class extends h {
  constructor() {
    super(l.contextAlias);
  }
  observeSecurityItem() {
    this.workspaceContext && this.observe(this.workspaceContext.data, (e) => {
      e && (this._security = e);
    });
  }
  onPackagePermissionsChange(e, t) {
    var i, r, o;
    const s = e.target.checked;
    (i = this.workspaceContext) == null || i.setUserSecurityProperty(t, s), t === "viewEntries" && !s && ((r = this.workspaceContext) == null || r.setUserSecurityProperty("editEntries", !1), (o = this.workspaceContext) == null || o.setUserSecurityProperty("deleteEntries", !1)), this.dispatchEvent(new CustomEvent("valueChange")), this.requestUpdate();
  }
  render() {
    var e, t, s;
    return u`
      ${this.renderToggles((e = this._security) == null ? void 0 : e.userSecurity)}
      ${this.renderStartFolders(((t = this._security) == null ? void 0 : t.startFolderIds) ?? [])}
      ${this.renderSecurityTable(((s = this._security) == null ? void 0 : s.formsSecurity) ?? [])}
    `;
  }
};
a([
  p()
], c.prototype, "_security", 2);
c = a([
  m(S)
], c);
const d = c;
export {
  c as UmbWorkspaceViewSecurityUserPermissionsElement,
  d as default
};
//# sourceMappingURL=workspace-view-security-user-permissions.element.js.map
