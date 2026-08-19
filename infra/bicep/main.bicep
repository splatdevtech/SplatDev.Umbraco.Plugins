param location string = 'East US'
@description('Deployment environment. Used to isolate resource names and security settings.')
@allowed([
  'dev'
  'staging'
  'prod'
])
param environment string = 'dev'

var rgName = 'rg-splatdev-${environment}'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}

// Storage is private by default. Use a private endpoint or an explicitly approved
// network rule when an application needs to access it.
resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: 'stg${environment}splatdev'
  location: location
  sku: { name: 'Standard_GRS' }
  kind: 'StorageV2'
  properties: {
    publicNetworkAccess: 'Disabled'
    allowBlobPublicAccess: false
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Deny'
    }
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-10-01' = {
  name: 'kv-splatdev-${environment}'
  location: location
  properties: {
    enableSoftDelete: true
    publicNetworkAccess: 'Disabled'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Deny'
    }
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: subscription().tenantId
    accessPolicies: []
  }
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-06-01' = {
  name: 'sb-splatdev-${environment}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {}
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: 'asp-splatdev-${environment}'
  location: location
  sku: { name: 'P1v2', tier: 'PremiumV2' }
  properties: {
    perSiteScaling: false
    zoneRedundant: true
  }
}

resource webApp 'Microsoft.Web/sites@2021-02-01' = {
  name: 'web-splatdev-${environment}'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    publicNetworkAccess: 'Disabled'
    siteConfig: {
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      clientCertEnabled: true
      clientCertMode: 'Required'
      minimumElasticInstanceCount: 2
      healthCheckPath: '/health'
    }
  }
}

output storageAccountName string = storage.name
output keyVaultName string = keyVault.name
output serviceBusNamespace string = serviceBus.name
output webAppName string = webApp.name
