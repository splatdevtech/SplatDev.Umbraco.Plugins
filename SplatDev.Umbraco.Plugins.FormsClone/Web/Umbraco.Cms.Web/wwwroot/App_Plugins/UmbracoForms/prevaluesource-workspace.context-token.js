import { UmbContextToken as R } from "@umbraco-cms/backoffice/context-api";
import { k as T } from "./index.js";
const C = new R(
  "UmbWorkspaceContext",
  void 0,
  (E) => {
    var o;
    return ((o = E.getEntityType) == null ? void 0 : o.call(E)) === T;
  }
);
export {
  C as F
};
//# sourceMappingURL=prevaluesource-workspace.context-token.js.map
