import { UmbTextStyles as u } from "@umbraco-cms/backoffice/style";
import { LitElement as n, html as i, css as c } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as m } from "@umbraco-cms/backoffice/element-api";
const a = class a extends m(n) {
  constructor(t) {
    super(), this.consumeContext(t, (e) => {
      this.workspaceContext = e, this.observeSecurityItem();
    });
  }
  renderSecurityTable(t) {
    if (this.workspaceContext)
      return i`<uui-box
      .headline=${this.localize.term("formSecurity_manageIndividualFormsLabel")}
    >
      <umb-property-layout
        alias="forms"
        .label=${this.localize.term("sections_forms")}
        .description=${this.localize.term(
        "formSecurity_manageIndividualFormsDescription"
      )}
      >
        <form-security-table
          slot="editor"
          .records=${t ?? []}
          .set=${(e, r) => {
        var o;
        return (o = this.workspaceContext) == null ? void 0 : o.setFormSecurityAccess(e, r);
      }}
          .toggle=${(e) => {
        var r;
        return (r = this.workspaceContext) == null ? void 0 : r.toggleFormSecurityAccess(e);
      }}
        ></form-security-table>
      </umb-property-layout>
    </uui-box>`;
  }
  renderStartFolders(t) {
    if (!this.workspaceContext)
      return;
    const e = (r) => {
      var s;
      const o = r.target;
      (s = this.workspaceContext) == null || s.setProperty("startFolderIds", o.selection), this.dispatchEvent(new CustomEvent("valueChange"));
    };
    return i`<uui-box
      .headline=${this.localize.term("formSecurity_startFolders")}
    >
      <umb-property-layout
        alias="startFolders"
        .label=${this.localize.term("formSecurity_selectNewStartFolders")}
        .description=${this.localize.term(
      "formSecurity_startFoldersDescription"
    )}
      >
        <forms-input-folder
          .selection=${t}
          @change=${e}
          slot="editor"
        ></forms-input-folder>
      </umb-property-layout>
    </uui-box>`;
  }
  renderToggles(t) {
    if (t)
      return i`<uui-box headline=${this.localize.term("formSecurity_packagePermissions")}>
      <umb-property-layout
        alias="manageForms"
        .label=${this.localize.term("formSecurity_manageFormsLabel")}
        .description=${this.localize.term(
        "formSecurity_manageFormsDescription"
      )}
      >
        <uui-toggle
          slot="editor"
          ?checked=${t.manageForms}
          @change=${(e) => this.onPackagePermissionsChange(e, "manageForms")}
        ></uui-toggle>
      </umb-property-layout>
      <umb-property-layout
        alias="viewEntries"
        .label=${this.localize.term("formSecurity_viewEntriesLabel")}
        .description=${this.localize.term(
        "formSecurity_viewEntriesDescription"
      )}
      >
        <uui-toggle
          slot="editor"
          ?checked=${t.viewEntries}
          @change=${(e) => this.onPackagePermissionsChange(e, "viewEntries")}
        ></uui-toggle>
      </umb-property-layout>
      <umb-property-layout
        alias="editEntries"
        .label=${this.localize.term("formSecurity_editEntriesLabel")}
        .description=${this.localize.term(
        "formSecurity_editEntriesDescription"
      )}
      >
        <uui-toggle
          slot="editor"
          ?checked=${t.editEntries}
          ?disabled=${!t.viewEntries}
          @change=${(e) => this.onPackagePermissionsChange(e, "editEntries")}
        ></uui-toggle>
      </umb-property-layout>
      <umb-property-layout
        alias="editEntries"
        .label=${this.localize.term("formSecurity_deleteEntriesLabel")}
        .description=${this.localize.term(
        "formSecurity_deleteEntriesDescription"
      )}
      >
        <uui-toggle
          slot="editor"
          ?checked=${t.deleteEntries}
          ?disabled=${!t.viewEntries}
          @change=${(e) => this.onPackagePermissionsChange(e, "deleteEntries")}
        ></uui-toggle>
      </umb-property-layout>
      <umb-property-layout
        alias="manageWorkflows"
        .label=${this.localize.term("formSecurity_manageWorkflowsLabel")}
        .description=${this.localize.term(
        "formSecurity_manageWorkflowsDescription"
      )}
      >
        <uui-toggle
          slot="editor"
          ?checked=${t.manageWorkflows}
          @change=${(e) => this.onPackagePermissionsChange(e, "manageWorkflows")}
        ></uui-toggle>
      </umb-property-layout>
      <umb-property-layout
        alias="manageDatasources"
        .label=${this.localize.term("formSecurity_manageDatasourcesLabel")}
        .description=${this.localize.term(
        "formSecurity_manageDatasourcesDescription"
      )}
      >
        <uui-toggle
          slot="editor"
          ?checked=${t.manageDataSources}
          @change=${(e) => this.onPackagePermissionsChange(e, "manageDataSources")}
        ></uui-toggle>
      </umb-property-layout>
      <umb-property-layout
        alias="managePrevalueSources"
        .label=${this.localize.term("formSecurity_managePrevalueSourcesLabel")}
        .description=${this.localize.term(
        "formSecurity_managePrevalueSourcesDescription"
      )}
      >
        <uui-toggle
          slot="editor"
          ?checked=${t.managePreValueSources}
          @change=${(e) => this.onPackagePermissionsChange(e, "managePreValueSources")}
        ></uui-toggle>
      </umb-property-layout>
    </uui-box>`;
  }
};
a.styles = [
  u,
  c`
      :host {
        display: block;
        padding: var(--uui-size-layout-1);
      }

      uui-box {
        margin-bottom: var(--uui-size-6);
      }
    `
];
let l = a;
export {
  l as U
};
//# sourceMappingURL=workspace-view-security-permissions-base.element.js.map
