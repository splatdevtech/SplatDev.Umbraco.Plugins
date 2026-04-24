import { LitElement, html, css } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

class CharLimitDashboard extends UmbElementMixin(LitElement) {
    static properties = {
        _config: { state: true },
        _loading: { state: true },
    };

    static styles = css`
        :host { display: block; padding: 1rem; }
        .info-row { display: flex; gap: 1rem; margin-bottom: 0.5rem; }
        .info-label { font-weight: 600; min-width: 160px; }
    `;

    constructor() {
        super();
        this._config = null;
        this._loading = true;
    }

    connectedCallback() {
        super.connectedCallback();
        this._loadConfig();
    }

    async _loadConfig() {
        try {
            const response = await fetch('/umbraco/api/charlimit/GetConfig');
            this._config = await response.json();
        } finally {
            this._loading = false;
        }
    }

    render() {
        if (this._loading) return html`<uui-loader></uui-loader>`;
        return html`
            <uui-box headline="Character Limit Property Editor">
                <p>This property editor enforces a maximum character count with an optional countdown display.</p>
                ${this._config ? html`
                    <div class="info-row">
                        <span class="info-label">Default Max Characters:</span>
                        <span>${this._config.maxChars}</span>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Show Countdown:</span>
                        <span>${this._config.showCountdown ? 'Yes' : 'No'}</span>
                    </div>
                ` : ''}
                <p>Configure max characters and countdown display per property in the Data Types section.</p>
            </uui-box>
        `;
    }
}

customElements.define('charlimit-dashboard', CharLimitDashboard);
export default CharLimitDashboard;
