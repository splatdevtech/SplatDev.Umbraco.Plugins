# SplatDev.Umbraco.2FA

Two-factor authentication plugin for Umbraco backoffice.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.2FA
```

## Features

- TOTP-based 2FA (Google Authenticator, Authy, etc.)
- QR code setup wizard in the Umbraco backoffice
- Per-user enable/disable
- Fallback recovery codes
- Audit logging for 2FA events

## Usage

After installation, users will see a 2FA setup prompt on next login.
Administrators can enforce 2FA for specific user groups.

## License

MIT
