# UmbracoCms.Plugins.MemberLogin

Custom member login plugin for Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Features

- Login form with username/email and password
- Remember me support
- Forgot password with token-based email reset
- Account lockout detection and user feedback
- Approval workflow support

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/umbraco/api/memberlogin/Login` | Authenticate a member |
| POST | `/umbraco/api/memberlogin/Logout` | Sign out the current member |
| POST | `/umbraco/api/memberlogin/ForgotPassword` | Request a password reset link |
| POST | `/umbraco/api/memberlogin/ResetPassword` | Reset password with token |

## View Component

```cshtml
@await Component.InvokeAsync("MemberLogin", new { returnUrl = "/members" })
```

## Installation

Register the composer automatically via Umbraco's composition system. No manual configuration required.
