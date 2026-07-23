import { E as a, r as t, s as o, t as l } from "./index.js";
import { UmbServerFilePathUniqueSerializer as s } from "@umbraco-cms/backoffice/server-file-system";
import { UmbTreeServerDataSourceBase as T, UmbTreeRepositoryBase as E } from "@umbraco-cms/backoffice/tree";
import { FORMS_EMAIL_TEMPLATE_TREE_STORE_CONTEXT as p } from "./email-template-tree.store.js";
class u extends T {
  constructor(r) {
    super(r, {
      getRootItems: n,
      getChildrenOf: c,
      getAncestorsOf: m,
      mapper: _
    });
  }
}
const n = (e) => (
  // eslint-disable-next-line local-rules/no-direct-api-import
  a.getTreeEmailTemplateRoot()
), c = (e) => {
  const r = new s().toServerPath(e.parent.unique);
  return r === null ? n() : a.getTreeEmailTemplateChildrenByParentPath({
    parentPath: r
  });
}, m = (e) => {
  throw new Error("Ancestors is not available.");
}, _ = (e) => {
  const r = new s();
  return {
    unique: r.toUnique(e.path),
    parent: {
      unique: e.parent ? r.toUnique(e.parent.path) : null,
      entityType: e.parent ? t : o
    },
    entityType: e.isFolder ? t : l,
    name: e.name,
    isFolder: e.isFolder,
    hasChildren: e.hasChildren,
    icon: e.isFolder ? void 0 : "icon-notepad"
  };
};
class M extends E {
  constructor(r) {
    super(r, u, p);
  }
  async requestTreeRoot() {
    const { data: r } = await this._treeSource.getRootItems({ skip: 0, take: 1 }), i = r ? r.total > 0 : !1;
    return { data: {
      unique: null,
      entityType: o,
      name: "Email Templates",
      hasChildren: i,
      isFolder: !0
    } };
  }
}
export {
  M as FormsEmailTemplateTreeRepository,
  M as default
};
//# sourceMappingURL=email-template-tree.repository.js.map
