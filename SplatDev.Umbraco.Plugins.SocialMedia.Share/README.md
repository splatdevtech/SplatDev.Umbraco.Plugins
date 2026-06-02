# SocialMedia.Share

Umbraco social sharing buttons plugin — configurable platforms and share links. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.SocialMedia.Share.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.SocialMedia.Share)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.SocialMedia.Share
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddSocialMediaShare()   // <-- add this
    .Build();
```


## Dashboard

![Social Share dashboard](../docs/screenshots/social-share.png)
## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
