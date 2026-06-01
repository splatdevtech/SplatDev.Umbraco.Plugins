# ExamineExtensions

Examine search extensions for Umbraco — additional query helpers, result transformers, and index utilities.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.ExamineExtensions.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.ExamineExtensions)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.ExamineExtensions
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddExamineExtensions()   // <-- add this
    .Build();
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
