# SplatDev.Umbraco.Membership

Membership extensions for Umbraco.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Membership
```

## Dependencies

- `Umbraco.Cms.Core`
- `Umbraco.Cms.Web.Common`

## Features

- `MemberHelper` — register, login, profile management
- `MemberGroupHelper` — group assignment and role checks
- Password reset and email verification flows
- Custom member properties and profile enrichment
- Login tracking and lockout support
- Integration with Umbraco `IMemberService` and `IMemberManager`

## Usage

### Register a member

```csharp
using SplatDev.Umbraco.Membership;

var result = await MemberHelper.RegisterAsync(new RegisterModel
{
    Email = "user@example.com",
    Password = "securePassword123",
    Name = "John Doe",
    MemberTypeAlias = "Member"
});
```

### Login

```csharp
var result = await MemberHelper.LoginAsync(email, password);
if (result.Success)
    await HttpContext.SignInMemberAsync(result.Member);
```

### Check access

```csharp
bool canAccess = MemberGroupHelper.IsInRole(memberId, "Premium");
```

## License

MIT
