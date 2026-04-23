import { LitElement, html, css } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

class PhotoGalleryDashboard extends UmbElementMixin(LitElement) {
    static properties = {
        _albums: { state: true },
        _loading: { state: true },
    };

    static styles = css`
        :host { display: block; padding: 1rem; }
        .photo-grid { display: flex; flex-wrap: wrap; gap: 0.5rem; }
        .photo-thumb img { width: 120px; height: 90px; object-fit: cover; border-radius: 4px; }
    `;

    constructor() {
        super();
        this._albums = [];
        this._loading = true;
    }

    connectedCallback() {
        super.connectedCallback();
        this._loadAlbums();
    }

    async _loadAlbums() {
        try {
            const response = await fetch('/umbraco/api/photogallery/GetAlbums');
            this._albums = await response.json();
        } finally {
            this._loading = false;
        }
    }

    render() {
        if (this._loading) return html`<uui-loader></uui-loader>`;
        return html`
            <uui-box headline="Photo Gallery">
                ${this._albums.map(album => html`
                    <uui-box headline=${album.title}>
                        <p>${album.description}</p>
                        <div class="photo-grid">
                            ${(album.photos ?? []).map(photo => html`
                                <div class="photo-thumb">
                                    <img src=${photo.thumbnailUrl ?? photo.imageUrl} alt=${photo.title} />
                                </div>
                            `)}
                        </div>
                    </uui-box>
                `)}
            </uui-box>
        `;
    }
}

customElements.define('photogallery-dashboard', PhotoGalleryDashboard);
export default PhotoGalleryDashboard;
