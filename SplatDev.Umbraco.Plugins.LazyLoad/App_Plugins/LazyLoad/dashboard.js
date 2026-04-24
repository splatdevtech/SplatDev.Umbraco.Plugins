import { LitElement, html, css } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

class LazyLoadDashboard extends UmbElementMixin(LitElement) {
    static properties = {
        _settings: { state: true },
        _loading: { state: true },
        _saved: { state: true },
    };

    static styles = css`
        :host { display: block; padding: 1rem; }
        .form-row { display: flex; align-items: center; gap: 1rem; margin-bottom: 1rem; }
        label { min-width: 160px; font-weight: 600; }
        input[type="text"] { flex: 1; padding: 0.4rem; border: 1px solid var(--uui-color-border); border-radius: 4px; }
    `;

    constructor() {
        super();
        this._settings = { enabled: true, placeholder: '', lazyLoadIframes: true };
        this._loading = true;
        this._saved = false;
    }

    connectedCallback() {
        super.connectedCallback();
        this._loadSettings();
    }

    async _loadSettings() {
        try {
            const response = await fetch('/umbraco/api/lazyload/GetSettings');
            this._settings = await response.json();
        } finally {
            this._loading = false;
        }
    }

    async _saveSettings() {
        await fetch('/umbraco/api/lazyload/SaveSettings', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(this._settings),
        });
        this._saved = true;
        setTimeout(() => { this._saved = false; }, 3000);
    }

    _toggle(field) {
        this._settings = { ...this._settings, [field]: !this._settings[field] };
    }

    render() {
        if (this._loading) return html`<uui-loader></uui-loader>`;
        return html`
            <uui-box headline="Lazy Load Settings">
                <div class="form-row">
                    <label>Enabled</label>
                    <uui-toggle ?checked=${this._settings.enabled} @change=${() => this._toggle('enabled')}></uui-toggle>
                </div>
                <div class="form-row">
                    <label>Lazy Load Iframes</label>
                    <uui-toggle ?checked=${this._settings.lazyLoadIframes} @change=${() => this._toggle('lazyLoadIframes')}></uui-toggle>
                </div>
                <div class="form-row">
                    <label>Placeholder</label>
                    <input type="text" .value=${this._settings.placeholder}
                        @input=${(e) => this._settings = { ...this._settings, placeholder: e.target.value }} />
                </div>
                <uui-button look="primary" @click=${this._saveSettings}>Save Settings</uui-button>
                ${this._saved ? html`<uui-tag color="positive" look="secondary">Saved!</uui-tag>` : ''}
            </uui-box>
        `;
    }
}

customElements.define('lazyload-dashboard', LazyLoadDashboard);
export default LazyLoadDashboard;
