import { css as f, customElement as m, html as a } from "@umbraco-cms/backoffice/external/lit";
import { UmbCollectionDefaultElement as p } from "@umbraco-cms/backoffice/collection";
import { UmbLitElement as u } from "@umbraco-cms/backoffice/lit-element";
import "./form-entry-table-collection-view.element.js";
var d = Object.defineProperty, v = Object.getOwnPropertyDescriptor, y = (i, t, n, r) => {
  for (var e = r > 1 ? void 0 : r ? v(t, n) : t, l = i.length - 1, o; l >= 0; l--)
    (o = i[l]) && (e = (r ? o(t, n, e) : o(e)) || e);
  return r && e && d(t, n, e), e;
};
const h = "form-entry-collection-header";
let s = class extends u {
  render() {
    return a`
      <umb-collection-action-bundle></umb-collection-action-bundle>
      <div>
        <form-entry-filter-text></form-entry-filter-text>
        <form-entry-filter-date-range-selector></form-entry-filter-date-range-selector>
      </div>
    `;
  }
};
s.styles = [
  f`
      :host {
        width: 100%;
        height: 100%;
        display: flex;
        justify-content: space-between;
        gap: var(--uui-size-space-5);
        white-space: nowrap;
        align-items: center;
      }

      div {
        width: 100%;
        display: flex;
        justify-content: space-between;
        gap: var(--uui-size-space-5);
        align-items: center;
        margin-right:var(--uui-size-5);
      }

      forms-entry-filter-text {
        width: 100%;
      }
    `
];
s = y([
  m(h)
], s);
var b = Object.defineProperty, _ = Object.getOwnPropertyDescriptor, w = (i, t, n, r) => {
  for (var e = r > 1 ? void 0 : r ? _(t, n) : t, l = i.length - 1, o; l >= 0; l--)
    (o = i[l]) && (e = (r ? o(t, n, e) : o(e)) || e);
  return r && e && b(t, n, e), e;
};
const g = "form-entry-collection";
let c = class extends p {
  renderToolbar() {
    return a`<form-entry-collection-header
      slot="header"
    ></form-entry-collection-header> `;
  }
};
c = w([
  m(g)
], c);
const j = c;
export {
  c as FormEntryCollectionElement,
  j as default
};
//# sourceMappingURL=form-entry-collection.element.js.map
