import { LitElement, html, css } from "@umbraco-cms/backoffice/external/lit";
import { customElement, property } from "@umbraco-cms/backoffice/external/lit";

const PROVIDER_MAP: Record<string, { initials: string; color: string; label: string }> = {
  local:               { initials: "FS",  color: "#64748b", label: "Local" },
  LocalFileSystem:     { initials: "FS",  color: "#64748b", label: "Local" },
  AzureBlobStorage:    { initials: "AZ",  color: "#0078d4", label: "Azure Blob" },
  azure:               { initials: "AZ",  color: "#0078d4", label: "Azure Blob" },
  GoogleDrive:         { initials: "GD",  color: "#4285f4", label: "Google Drive" },
  googledrive:         { initials: "GD",  color: "#4285f4", label: "Google Drive" },
  Dropbox:             { initials: "DB",  color: "#0061ff", label: "Dropbox" },
  dropbox:             { initials: "DB",  color: "#0061ff", label: "Dropbox" },
  BoxNet:              { initials: "BX",  color: "#0061d5", label: "Box.net" },
  box:                 { initials: "BX",  color: "#0061d5", label: "Box.net" },
  OneDrive:            { initials: "OD",  color: "#0f3cc9", label: "OneDrive" },
  onedrive:            { initials: "OD",  color: "#0f3cc9", label: "OneDrive" },
  Mega:                { initials: "MG",  color: "#d9272e", label: "Mega" },
  mega:                { initials: "MG",  color: "#d9272e", label: "Mega" },
  Seafile:             { initials: "SF",  color: "#0d8f75", label: "Seafile" },
  seafile:             { initials: "SF",  color: "#0d8f75", label: "Seafile" },
  AwsS3:               { initials: "S3",  color: "#ff9900", label: "Amazon S3" },
  s3:                  { initials: "S3",  color: "#ff9900", label: "Amazon S3" },
  Sftp:                { initials: "FTP", color: "#4a4a4a", label: "SFTP" },
  sftp:                { initials: "FTP", color: "#4a4a4a", label: "SFTP" },
};

@customElement("backups-provider-badge")
export class BackupsProviderBadgeElement extends LitElement {
  static override styles = css`
    :host {
      display: inline-flex;
      align-items: center;
      gap: 6px;
    }
    .badge {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 28px;
      height: 28px;
      border-radius: 4px;
      color: #fff;
      font-size: 0.6rem;
      font-weight: 700;
      letter-spacing: 0.5px;
    }
    .label {
      font-size: 0.825rem;
    }
  `;

  @property({ type: String }) provider = "local";
  @property({ type: Boolean, attribute: "show-label" }) showLabel = false;

  override render() {
    const info = PROVIDER_MAP[this.provider] ?? { initials: "?", color: "#999", label: this.provider };
    return html`
      <span class="badge" style="background:${info.color}">${info.initials}</span>
      ${this.showLabel ? html`<span class="label">${info.label}</span>` : ""}
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "backups-provider-badge": BackupsProviderBadgeElement;
  }
}
