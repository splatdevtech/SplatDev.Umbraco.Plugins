# Security triage — 2026-08-19

Scope: the 14 findings listed in SPL-3788. Artifact findings and the unlisted findings referenced by the original scanner count are outside this report.

## Disposition

| Rule | Disposition | Evidence / treatment |
|---|---|---|
| CKV2_GHA_1 | Remediated | `SplatDev.Umbraco.Plugins.NewFeature/.github/workflows/release.yml` now declares top-level `permissions: contents: read`; no write permission is required by the jobs. |
| CKV_AZURE_222 | Remediated | App Service `publicNetworkAccess` is disabled. |
| CKV_AZURE_67 | Remediated | App Service enables HTTP/2 (`http20Enabled: true`). |
| CKV_AZURE_18 | Remediated | App Service enables HTTP/2 (`http20Enabled: true`). |
| CKV_AZURE_17 | Remediated | Incoming client certificates are enabled and required. |
| CKV_SECRET_4 | False positive / no credential present | Repository-root `package.json` contains only package metadata and the `http-server` dependency; no credential value exists at the reported line. No rotation is indicated by repository evidence. |
| CKV_AZURE_212 | Remediated | App Service minimum elastic instance count is 2. |
| CKV_AZURE_225 | Remediated | App Service Plan is zone redundant. |
| CKV_AZURE_109 | Remediated | Key Vault network ACL default is deny. |
| CKV_AZURE_189 | Remediated | Key Vault public network access is disabled. |
| CKV_AZURE_206 | Remediated | Storage uses Standard_GRS replication. |
| CKV_AZURE_43 | Remediated | Storage naming is environment-scoped and conforms to Azure storage naming constraints. |
| CKV_AZURE_213 | Remediated | App Service health check path is `/health`. |
| CKV_AZURE_35 | Remediated | Storage network ACL default is deny. |

## Verification

- `git diff --check` passes for all triage changes.
- Azure CLI/Bicep CLI are not installed in this runner, so template compilation and Checkov execution must run in CI or a deployment toolchain.
- The `/health` endpoint is an application responsibility and must be supplied by deployed sites.
