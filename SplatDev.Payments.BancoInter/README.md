# SplatDev.Payments.BancoInter

Request/response DTOs and OAuth settings for integrating with the [Banco Inter](https://developers.inter.co/) payment APIs. This is a **contracts-only package** — it provides the type surface for Pix charges, Boleto issuance, webhook payloads, and banking statements. Consumers supply their own HTTP client logic.

## Install

```sh
dotnet add package SplatDev.Payments.BancoInter
```

## What's implemented

### Pix
- `InterPixChargeRequest` / `InterPixChargeResponse` — create a Pix charge with calendar, debtor, value, and additional info.
- `InterPixPaymentRequest` / `InterPixPaymentResponse` — send an outbound Pix payment.
- `InterPixWebhookPayload` — event payload for incoming Pix callbacks.
- Supporting types: `InterCalendario`, `InterDevedor`, `InterValor`, `InterLocation`, `InterPixReceived`, `InterDevolucao`.

### Boleto (Brazilian bank slip)
- `InterBoletoRequest` / `InterBoletoResponse` — issue a boleto with payer info, discount, interest, fine.
- `InterBoletoPaymentRequest` / `InterBoletoPaymentResponse` — pay a boleto/convenio/tribute.
- Supporting types: `InterPagador`, `InterBoletoDesconto`, `InterBoletoMulta`, `InterBoletoMora`, `InterMensagem`.

### OAuth & Banking
- `BancoInterSettings` — client credentials, certificate paths, sandbox toggle, base/token URLs.
- `InterTokenResponse` — OAuth token + expiration.
- `InterBankingBalance` / `InterBankingStatement` — balance and statement DTOs.
- `InterBankingWebhookPayload` — banking event callback payload.

### System.Text.Json attributes

All DTOs carry `[JsonPropertyName]` attributes mapping to Banco Inter's snake_case JSON. Ready for `HttpClient` + `System.Text.Json` deserialization out of the box.

## Configuration

Bind `BancoInterSettings` from `appsettings.json`:

```json
{
  "SplatDev": {
    "Payments": {
      "BancoInter": {
        "ClientId": "",
        "ClientSecret": "",
        "Sandbox": true,
        "CertificatePath": "certs/inter.pfx",
        "CertificateKeyPath": "certs/inter.key",
        "BaseUrl": "https://cdpj.partners.bancointer.com.br",
        "TokenUrl": "https://cdpj.partners.bancointer.com.br/oauth/v2/token"
      }
    }
  }
}
```

```csharp
services.Configure<BancoInterSettings>(
    configuration.GetSection("SplatDev:Payments:BancoInter"));
```

## DI registration

No built-in DI extensions. This is a contracts-only package. Register settings and build your own HTTP client:

```csharp
services.Configure<BancoInterSettings>(configuration.GetSection("SplatDev:Payments:BancoInter"));
services.AddHttpClient<IBancoInterClient, YourBancoInterClient>();
```

## Usage

```csharp
// Create a Pix charge request
var charge = new InterPixChargeRequest
{
    Calendario = new InterCalendario { Expiracao = 3600 },
    Valor = new InterValor { Original = "99.90", ModalidadeAlteracao = 0 },
    Chave = "customer-pix-key@example.com",
    Devedor = new InterDevedor
    {
        Nome = "John Doe",
        Cpf = "12345678900"
    }
};

// POST to Inter API (consumer builds the HTTP pipeline)
// var response = await httpClient.PostAsJsonAsync(
//     $"{baseUrl}/cob/v2", charge);
```

---

**SplatDev.Payments.BancoInter** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
