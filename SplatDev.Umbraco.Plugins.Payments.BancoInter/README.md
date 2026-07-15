# SplatDev.Umbraco.Plugins.Payments.BancoInter

Umbraco plugin for Banco Inter payment gateway integration. Supports Pix (immediate and due charges), Boleto com Pix (issuance, PDF export, cancellation), and Banking operations (balance, statement, outbound payments) through Banco Inter's REST API. Includes an EF Core transaction log and a Lit-based backoffice dashboard.

## Install

```bash
dotnet add package SplatDev.Umbraco.Plugins.Payments.BancoInter
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17). Published to nuget.org.

## What's implemented

### Services (4 interfaces + implementations)

| Service | Scope | Covers |
|---------|-------|--------|
| `IBancoInterAuthService` | OAuth 2.0 client_credentials | Token acquisition with `IMemoryCache` caching |
| `IBancoInterPixService` | Pix APIs | Immediate and due charges, devolution, webhooks |
| `IBancoInterBoletoService` | Boleto APIs | Issuance, PDF export, cancellation |
| `IBancoInterBankingService` | Banking APIs | Balance, statement, outbound Pix/boleto payments |

All services use `IHttpClientFactory` for HTTP and `IMemoryCache` for token caching (cached for `expires_in - 30s`).

### API Controller (`BancoInterApiController`)

Route prefix: `/umbraco/api/bancointersandbox/`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `CreatePixCharge` | Create an immediate Pix charge |
| `GET` | `GetPixCharge?txid=` | Get Pix charge status |
| `POST` | `IssueBoleto` | Issue a boleto com Pix |
| `GET` | `GetBoletoPdf?nossoNumero=` | Download boleto PDF |
| `GET` | `GetBalance` | Account balance |
| `GET` | `GetStatement?startDate=&endDate=` | Bank statement |
| `POST` | `WebhookPix` | Pix payment confirmation webhook |
| `POST` | `RegisterPixWebhook` | Register webhook for a Pix key |

### Transaction persistence

`BancoInterDbContext` stores transactions in `[bancointer].[BancoInterTransactions]` using the same SQL Server connection as Umbraco. Entity fields include `Type`, `Status`, `Amount`, `Txid`, `NossoNumero`, `PixCopiaECola`, `BoletoLinhaDigitavel`, `EndToEndId`, and timestamps.

### Backoffice Dashboard (Lit)

Appears under **Content** section at path `banco-inter`. Displays plugin status (Active badge), a toggle control, and Save/Documentation buttons. Currently a read-only status display; UI is wired for future backend interaction.

## Configuration

Add to `appsettings.json`:

```json
{
  "BancoInter": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Sandbox": true,
    "CertificatePath": "/path/to/cert.pem",
    "CertificateKeyPath": "/path/to/key.pem"
  }
}
```

| Key | Required | Default | Description |
|-----|----------|---------|-------------|
| `ClientId` | Yes | — | OAuth client ID |
| `ClientSecret` | Yes | — | OAuth client secret |
| `Sandbox` | No | `true` | `true` for sandbox, `false` for production |
| `CertificatePath` | Prod only | `null` | mTLS certificate (PEM) |
| `CertificateKeyPath` | Prod only | `null` | mTLS private key (PEM) |

**Sandbox base URL:** `https://cdpj-sandbox.partners.uatinter.co`  
**Production base URL:** `https://cdpj.partners.uatinter.co`

Transaction data is stored in the Umbraco SQL Server database (same `ConnectionStrings:umbracoDbDSN`).

## Usage

### Creating a Pix charge

```csharp
var pix = serviceProvider.GetRequiredService<IBancoInterPixService>();
var charge = await pix.CreateImmediateChargeAsync(new InterPixChargeRequest
{
    Calendario = new InterCalendario { Expiracao = 3600 },
    Devedor = new InterDevedor { Cpf = "12345678900", Nome = "Customer" },
    Valor = new InterValor { Original = "99.90" },
    Chave = "customer-pix-key",
    SolicitacaoPagador = "Order #1042"
});
```

### Issuing a boleto

```csharp
var boleto = serviceProvider.GetRequiredService<IBancoInterBoletoService>();
var result = await boleto.IssueBoletoAsync(new InterBoletoRequest
{
    SeuNumero = "1042",
    ValorNominal = 99.90m,
    DataVencimento = "2026-08-01",
    Pagador = new InterPagador { CpfCnpj = "12345678900", Nome = "Customer" }
});
```

## DI Registration

Automatic via `BancoInterComposer : IComposer`. No `.AddBancoInter()` call needed. The following are registered:

| Service | Lifetime |
|---------|----------|
| `IMemoryCache` | Singleton |
| `HttpClient` ("BancoInter") | Singleton (via factory) |
| `IBancoInterAuthService` | Scoped |
| `IBancoInterPixService` | Scoped |
| `IBancoInterBoletoService` | Scoped |
| `IBancoInterBankingService` | Scoped |
| `BancoInterDbContext` | Scoped |

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` | Composers, DI, controllers |
| `Umbraco.Cms.Web.Common` | Web hosting |
| `Microsoft.EntityFrameworkCore.SqlServer` | Transaction persistence |
| `Microsoft.Extensions.Caching.Memory` | OAuth token caching |
| `SplatDev.Payments.BancoInter` | Shared models, settings, DTOs |

---

**SplatDev.Umbraco.Plugins.Payments.BancoInter** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
