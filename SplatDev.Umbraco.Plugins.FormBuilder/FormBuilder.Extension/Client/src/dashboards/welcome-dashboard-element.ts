import {
    html,
    customElement,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

@customElement('welcome-dashboard')
export default class FormBuilderDashboardtElement extends UmbLitElement {

    constructor() {
        super();
    }

    render() {
        return html`
             <h1>Form Builder Dashboard</h1>
              <div>
                <p>
                  Add a new form
                </p>
              </div>
        `;
    }
}

declare global {
    interface HTMLElementTagNameMap {
        'welcome-dashboard': FormBuilderDashboardtElement;
    }
}
