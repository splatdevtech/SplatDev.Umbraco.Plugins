import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";

export interface BackupInfo {
  name: string;
  createdAt: string;
  sizeBytes: number;
  extension: string;
  isCompressed: boolean;
  isEncrypted: boolean;
}

export interface CloudProviderInfo {
  id: string;
  providerType: string;
  enabled: boolean;
  settings: Record<string, string>;
}

export interface CreateBackupRequest {
  name: string;
  includeMedia: boolean;
  scope: number;
  compress: boolean;
  encrypt: boolean;
  encryptionKey: string;
  cloudProviderIds: string[];
}

export const BACKUPS_CONTEXT_TOKEN =
  new UmbContextToken<BackupsContext>("SplatDev.Backups.Context");

export class BackupsContext extends UmbContextBase<BackupsContext> {
  #baseUrl = "/umbraco/api/backups";

  constructor(host: UmbControllerHost) {
    super(host, BACKUPS_CONTEXT_TOKEN);
  }

  async getBackups(): Promise<BackupInfo[]> {
    const resp = await fetch(`${this.#baseUrl}/GetAll`);
    if (!resp.ok) throw new Error("Failed to load backups");
    return resp.json();
  }

  async getProviders(): Promise<CloudProviderInfo[]> {
    const resp = await fetch(`${this.#baseUrl}/GetCloudProviders`);
    if (!resp.ok) throw new Error("Failed to load providers");
    return resp.json();
  }

  async createBackup(request: CreateBackupRequest): Promise<void> {
    const resp = await fetch(`${this.#baseUrl}/Create`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });
    if (!resp.ok) throw new Error("Failed to create backup");
  }

  async deleteBackup(name: string): Promise<void> {
    const resp = await fetch(
      `${this.#baseUrl}/Delete?name=${encodeURIComponent(name)}`,
      { method: "DELETE" },
    );
    if (!resp.ok) throw new Error("Failed to delete backup");
  }

  formatSize(bytes: number): string {
    if (bytes === 0) return "0 B";
    const units = ["B", "KB", "MB", "GB", "TB"];
    const i = Math.floor(Math.log(bytes) / Math.log(1024));
    return `${(bytes / Math.pow(1024, i)).toFixed(1)} ${units[i]}`;
  }
}
