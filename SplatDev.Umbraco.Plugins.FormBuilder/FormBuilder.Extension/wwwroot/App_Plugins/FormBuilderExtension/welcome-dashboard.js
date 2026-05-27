import { customElement as d, html as n } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as p } from "@umbraco-cms/backoffice/lit-element";
var u = Object.getOwnPropertyDescriptor, c = (t, o, l, m) => {
  for (var r = m > 1 ? void 0 : m ? u(o, l) : o, e = t.length - 1, s; e >= 0; e--)
    (s = t[e]) && (r = s(r) || r);
  return r;
};
let a = class extends p {
  constructor() {
    super();
  }
  render() {
    return n`
             <h1>Form Builder Dashboard</h1>
              <div>
                <p>
                  Add a new form
                </p>
              </div>
        `;
  }
};
a = c([
  d("welcome-dashboard")
], a);
export {
  a as default
};
//# sourceMappingURL=welcome-dashboard.js.map
