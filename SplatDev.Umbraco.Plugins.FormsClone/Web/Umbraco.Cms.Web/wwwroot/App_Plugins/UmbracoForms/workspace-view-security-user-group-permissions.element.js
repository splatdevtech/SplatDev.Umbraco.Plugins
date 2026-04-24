import { html as a, state as p, customElement as m } from "@umbraco-cms/backoffice/external/lit";
import { F as l } from "./security-user-group-workspace.context-token.js";
import { U as h } from "./workspace-view-security-permissions-base.element.js";
var y = Object.defineProperty, _ = Object.getOwnPropertyDescriptor, u = (e, t, s, i) => {
  for (var r = i > 1 ? void 0 : i ? _(t, s) : t, o = e.length - 1, n; o >= 0; o--)
    (n = e[o]) && (r = (i ? n(t, s, r) : n(r)) || r);
  return i && r && y(t, s, r), r;
};
const f = "workspace-view-security-user-group-permissions";
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
    (i = this.workspaceContext) == null || i.setUserGroupSecurityProperty(t, s), t === "viewEntries" && !s && ((r = this.workspaceContext) == null || r.setUserGroupSecurityProperty("editEntries", !1), (o = this.workspaceContext) == null || o.setUserGroupSecurityProperty("deleteEntries", !1)), this.dispatchEvent(new CustomEvent("valueChange")), this.requestUpdate();
  }
  render() {
    var e, t, s;
    return a` ${this.renderToggles((e = this._security) == null ? void 0 : e.userGroupSecurity)}
    ${this.renderStartFolders(((t = this._security) == null ? void 0 : t.startFolderIds) ?? [])}
    ${this.renderSecurityTable(((s = this._security) == null ? void 0 : s.formsSecurity) ?? [])}`;
  }
};
u([
  p()
], c.prototype, "_security", 2);
c = u([
  m(f)
], c);
const P = c;
export {
  c as UmbWorkspaceViewSecurityUserGroupPermissionsElement,
  P as default
};
//# sourceMappingURL=workspace-view-security-user-group-permissions.element.js.map
