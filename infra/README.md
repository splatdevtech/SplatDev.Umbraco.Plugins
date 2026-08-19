# Infrastructure as Code — Azure Bicep Templates

Azure Bicep templates for provisioning Umbraco hosting infrastructure on Microsoft Azure. Covers resource groups, storage accounts, and Key Vault across dev/staging/prod environments.

## What's Included

- **Resource Group** — scoped per environment (`dev`, `staging`, `prod`)
- **Storage Account** — Standard_LRS, v2, for Umbraco media and log storage
- **Key Vault** — secrets/keys management with soft-delete enabled
- **Environment parameterisation** — single `environment` param drives naming and SKU selection

## Usage

```bash
az deployment sub create \
  --location eastus \
  --template-file infra/bicep/main.bicep \
  --parameters environment=staging
```

## Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| `location` | `East US` | Azure region for all resources |
| `environment` | `dev` | Deployment target: `dev`, `staging`, or `prod` |

## Compatibility

Works with Azure CLI 2.60+ and Bicep CLI 0.28+. The templates target the Azure Resource Manager API versions current as of 2024.

## Security Defaults

The template applies secure defaults in every environment: GRS storage replication, storage and Key Vault firewall deny rules with public access disabled, zone-redundant App Service Plans, and App Service health checks, HTTP/2, TLS 1.2, required client certificates, two minimum instances, and public access disabled. Private endpoints and the `/health` application endpoint must be provisioned by the consuming deployment.

## Known Limitations

- SQL Server/database, Application Insights, private endpoints, and network integration are outside this minimal template.
- Resource names must remain within Azure naming limits; the environment allow-list prevents accidental cross-environment names.
- Key Vault access policies are intentionally empty; grant access through the deployment's managed identity configuration.

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
