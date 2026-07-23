# SplatDev.Payments.Getnet

Getnet (Santander's card acquirer) payment SDK for .NET — OAuth2 `client_credentials` + HTTP Basic auth,
**no client certificate** (distinct from `SplatDev.Payments.Santander`, which is the mTLS Open Banking SDK).

Targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17). Transport-only; the Umbraco webhook/backoffice
controllers and dashboard live in `SplatDev.Umbraco.Plugins.Getnet`, and RISIN-specific orchestration stays
in the consuming app.

| Type | Role |
|------|------|
| `GetnetApiOptions` | BaseUrl (sandbox/prod), token path, SellerId/ClientId/ClientSecret, payment paths, dev-mock flag |
| `GetnetApiClient` | OAuth2 token (Basic auth) + `seller_id` header; PIX/boleto/payment-link/status/list; in-memory dev mock |
