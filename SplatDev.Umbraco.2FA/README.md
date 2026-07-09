# SplatDev.Umbraco.2FA

Cryptographically secure password and PIN generators for Umbraco applications. Uses `System.Security.Cryptography.RandomNumberGenerator` — suitable for 2FA codes, temporary passwords, recovery tokens, and member registration.

## Install

```bash
dotnet add package SplatDev.Umbraco.2FA
```

Targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17).

## What's implemented

### `GeneratePassword` — random password generator

```csharp
using SplatDev.Umbraco._2FA.Extensions;

string password = TwoFactorAuthorizationExtensions.GeneratePassword(
    length: 12,
    nonAlphanumeric: true,
    digit: true,
    lowercase: true,
    uppercase: true);
```

| Parameter | Default | Description |
|-----------|---------|-------------|
| `length` | `12` | Desired password length |
| `nonAlphanumeric` | `true` | Require at least one symbol (`!`, `#`, `$`, etc.) |
| `digit` | `true` | Require at least one numeric digit (`0`–`9`) |
| `lowercase` | `true` | Require at least one lowercase letter (`a`–`z`) |
| `uppercase` | `true` | Require at least one uppercase letter (`A`–`Z`) |

Characters are drawn from printable ASCII (decimal 32–126). If the required character classes are not satisfied after filling to `length`, additional characters from the missing classes are appended. The returned string may therefore be slightly longer than `length` in edge cases.

### `GeneratePin` — numeric PIN generator

```csharp
string pin = TwoFactorAuthorizationExtensions.GeneratePin();
// => "482917"
```

Always returns a 6-digit numeric string. Leading zeros are possible. Backed by `RandomNumberGenerator` — not `System.Random`.

## Usage

### Member registration

```csharp
public class MemberRegistrationService
{
    public string CreateTemporaryPassword()
        => TwoFactorAuthorizationExtensions.GeneratePassword(length: 16);

    public string GenerateTwoFactorCode()
        => TwoFactorAuthorizationExtensions.GeneratePin();
}
```

### Password reset flow

```csharp
var tempPassword = TwoFactorAuthorizationExtensions.GeneratePassword(
    length: 14, nonAlphanumeric: true, digit: true);
// Send to member via email/SMS with force-change flag
```

## Dependencies

No external NuGet dependencies. Uses only `System.Security.Cryptography`.

## Notes

- Despite the package name, there is no TOTP, HOTP, QR code generation, or authenticator-app integration. The library provides only cryptographic random generation primitives — the rest is up to the consumer application or a dedicated identity provider.
- All generator classes are `partial` — customizable via additional partial definitions if needed.

---

Copyright SplatDev
