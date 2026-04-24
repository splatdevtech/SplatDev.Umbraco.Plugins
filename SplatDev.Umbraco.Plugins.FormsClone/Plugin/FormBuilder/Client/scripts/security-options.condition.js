import { UmbConditionBase as e } from "@umbraco-cms/backoffice/extension-registry";
import { Q as r } from "./index.js";
import { tryExecuteAndNotify as n } from "@umbraco-cms/backoffice/resources";
class c extends e {
  constructor(i, o) {
    super(i, o), n(this, r.getConfig()).then((t) => {
      this.permitted = t.data !== void 0 && this.config.match(t.data);
    });
  }
}
export {
  c as SecurityOptionsCondition,
  c as default
};
//# sourceMappingURL=security-options.condition.js.map
