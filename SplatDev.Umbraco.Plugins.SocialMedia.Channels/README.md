# SocialMedia.Channels

Umbraco social media channel management plugin — connect accounts and schedule posts. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.SocialMedia.Channels.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.SocialMedia.Channels)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.SocialMedia.Channels
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddSocialMediaChannels()   // <-- add this
    .Build();
```


## Dashboard

![Social Channels dashboard](../docs/screenshots/social-channels.png)
## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
