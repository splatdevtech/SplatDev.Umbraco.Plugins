import { UmbConditionBase as s } from "@umbraco-cms/backoffice/extension-registry";
import { j as r } from "./index.js";
class n extends s {
  constructor(t, e) {
    super(t, e), this.consumeContext(r, (o) => {
      this.permitted = !o.getIsNew();
    });
  }
}
export {
  n as FormsDataSourceCreatedCondition,
  n as default
};
//# sourceMappingURL=datasource-created.condition.js.map
