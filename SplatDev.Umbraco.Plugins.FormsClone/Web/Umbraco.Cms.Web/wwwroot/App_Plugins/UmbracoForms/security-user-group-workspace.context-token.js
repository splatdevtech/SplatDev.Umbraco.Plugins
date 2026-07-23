import { UmbContextToken as R } from "@umbraco-cms/backoffice/context-api";
import { Y as T } from "./index.js";
const S = new R(
  "UmbWorkspaceContext",
  void 0,
  (_) => {
    var E;
    return ((E = _.getEntityType) == null ? void 0 : E.call(_)) === T;
  }
);
export {
  S as F
};
//# sourceMappingURL=security-user-group-workspace.context-token.js.map
