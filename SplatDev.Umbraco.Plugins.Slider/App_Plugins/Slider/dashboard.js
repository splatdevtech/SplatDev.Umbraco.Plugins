import { LitElement, html, css } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

class SliderDashboard extends UmbElementMixin(LitElement) {
    static properties = {
        _sliders: { state: true },
        _loading: { state: true },
    };

    static styles = css`
        :host { display: block; padding: 1rem; }
        .slide-list { list-style: none; padding: 0; margin: 0; }
        .slide-list li { padding: 0.25rem 0; border-bottom: 1px solid var(--uui-color-border); }
    `;

    constructor() {
        super();
        this._sliders = [];
        this._loading = true;
    }

    connectedCallback() {
        super.connectedCallback();
        this._loadSliders();
    }

    async _loadSliders() {
        try {
            const response = await fetch('/umbraco/api/slider/GetSliders');
            this._sliders = await response.json();
        } finally {
            this._loading = false;
        }
    }

    render() {
        if (this._loading) return html`<uui-loader></uui-loader>`;
        return html`
            <uui-box headline="Slider Manager">
                ${this._sliders.map(slider => html`
                    <uui-box headline=${slider.name}>
                        <p>Effect: ${slider.effect} | Autoplay: ${slider.autoplay ? 'Yes' : 'No'} (${slider.autoplayDelay}ms)</p>
                        <ul class="slide-list">
                            ${(slider.slides ?? [])
                                .sort((a, b) => a.sortOrder - b.sortOrder)
                                .map(slide => html`<li><strong>${slide.title}</strong>${slide.subtitle ? ` - ${slide.subtitle}` : ''}</li>`)}
                        </ul>
                    </uui-box>
                `)}
            </uui-box>
        `;
    }
}

customElements.define('slider-dashboard', SliderDashboard);
export default SliderDashboard;
