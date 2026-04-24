import { LitElement, html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

@customElement("exif-dashboard")
export class ExifDashboard extends UmbElementMixin(LitElement) {
    static styles = css`
        :host { display: block; padding: 20px; }
        .lookup-row { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
        table { width: 100%; border-collapse: collapse; margin-top: 16px; }
        th, td { border: 1px solid var(--uui-color-border); padding: 8px 12px; }
        th { background: var(--uui-color-surface-emphasis); width: 160px; font-weight: 600; }
    `;

    @state() _mediaKey = "";
    @state() _filePath = "";
    @state() _data = null;
    @state() _error = "";

    async _lookupByKey() {
        this._data = null; this._error = "";
        try {
            const r = await fetch(`/umbraco/api/exif/GetByMediaKey?mediaKey=${encodeURIComponent(this._mediaKey)}`);
            if (!r.ok) throw new Error(await r.text());
            this._data = await r.json();
        } catch (e) { this._error = e.message; }
    }

    async _lookupByPath() {
        this._data = null; this._error = "";
        try {
            const r = await fetch(`/umbraco/api/exif/GetByFilePath?filePath=${encodeURIComponent(this._filePath)}`);
            if (!r.ok) throw new Error(await r.text());
            this._data = await r.json();
        } catch (e) { this._error = e.message; }
    }

    _row(label, value) {
        if (!value) return "";
        return html`<tr><th>${label}</th><td>${value}</td></tr>`;
    }

    render() {
        return html`
            <uui-box headline="EXIF Metadata Viewer">
                <div class="lookup-row">
                    <div>
                        <h4>Look up by Media Key</h4>
                        <uui-form-layout-item>
                            <uui-label slot="label">Media Key (GUID)</uui-label>
                            <uui-input .value=${this._mediaKey} @input=${e => this._mediaKey = e.target.value} placeholder="xxxxxxxx-xxxx-..."></uui-input>
                        </uui-form-layout-item>
                        <uui-button look="primary" label="Get EXIF" @click=${this._lookupByKey}>Get EXIF</uui-button>
                    </div>
                    <div>
                        <h4>Look up by File Path</h4>
                        <uui-form-layout-item>
                            <uui-label slot="label">Physical File Path</uui-label>
                            <uui-input .value=${this._filePath} @input=${e => this._filePath = e.target.value} placeholder="/var/www/media/..."></uui-input>
                        </uui-form-layout-item>
                        <uui-button look="primary" label="Get EXIF" @click=${this._lookupByPath}>Get EXIF</uui-button>
                    </div>
                </div>

                ${this._error ? html`<uui-badge color="danger" style="margin-top:12px;">${this._error}</uui-badge>` : ""}

                ${this._data ? html`
                    <h4 style="margin-top:16px;">EXIF Data</h4>
                    <table>
                        <tbody>
                            ${this._row("Camera", this._data.camera)}
                            ${this._row("Lens", this._data.lens)}
                            ${this._row("Date Taken", this._data.dateTaken)}
                            ${this._row("Exposure Time", this._data.exposureTime)}
                            ${this._row("F-Number", this._data.fNumber)}
                            ${this._row("ISO", this._data.iso)}
                            ${this._data.width ? html`<tr><th>Width</th><td>${this._data.width} px</td></tr>` : ""}
                            ${this._data.height ? html`<tr><th>Height</th><td>${this._data.height} px</td></tr>` : ""}
                            ${this._row("GPS Latitude", this._data.gpsLatitude)}
                            ${this._row("GPS Longitude", this._data.gpsLongitude)}
                        </tbody>
                    </table>` : ""}
            </uui-box>`;
    }
}

export default ExifDashboard;
