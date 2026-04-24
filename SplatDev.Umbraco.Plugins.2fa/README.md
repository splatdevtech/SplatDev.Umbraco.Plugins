# UmbracoCms.Plugins.2fa

Two-factor authentication plugin for Umbraco CMS. Implements TOTP (Time-based OTP) with EF Core-backed backup code storage.

## Targets

- **Umbraco 13** (net8.0)
- **Umbraco 17** (net10.0)

## Features

- TOTP secret generation (Base64-encoded HMAC-SHA1 key)
- TOTP verification with ±1 time-step window (30-second windows)
- Backup code generation (8 × 8-char hyphenated codes)
- Single-use backup code redemption
- EF Core schema: `twofactor.TwoFactorSetups`, `twofactor.BackupCodes`
- Backoffice dashboard (AngularJS for U13, Lit 3 Web Component for U17)

## Database Migrations

Run EF Core migrations to create the schema:

```bash
dotnet ef migrations add InitialCreate --context TwoFactorDbContext
dotnet ef database update --context TwoFactorDbContext
```

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/umbraco/api/twofactor/SetupTotp?memberId=N` | Generate / reset TOTP secret |
| POST | `/umbraco/api/twofactor/VerifyTotp?memberId=N&code=XXXXXX` | Verify code and enable 2FA |
| POST | `/umbraco/api/twofactor/GenerateBackupCodes?memberId=N&count=8` | Generate backup codes |
| POST | `/umbraco/api/twofactor/UseBackupCode?memberId=N&code=XXXX-XXXX` | Redeem a backup code |
| GET | `/umbraco/api/twofactor/IsEnabled?memberId=N` | Check if 2FA is enabled |
| POST | `/umbraco/api/twofactor/Disable?memberId=N` | Disable 2FA |

## Note on TOTP

The built-in TOTP implementation is a simplified HMAC-SHA1 approach for demonstration. For production, replace with a dedicated library such as [OtpNet](https://github.com/kspearrin/Otp.NET).

## Client Build

```bash
cd client
npm install
npm run build
```
