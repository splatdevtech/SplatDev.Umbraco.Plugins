# SplatDev.Umbraco.Tools.PackageActions

Umbraco package-action helpers for creating content types, data types, templates, content nodes, and user-group permissions during package installation. Provides abstract base classes so package authors can define setup steps declaratively and run them via a single `PackageActionRunner`.

Supports both **Umbraco 13** (net8.0) and **Umbraco 17** (net10.0) with per-framework conditional compilation for the Bellissima API surface.

## Install

```sh
dotnet add package SplatDev.Umbraco.Tools.PackageActions
```

## What's implemented

### Abstract action bases

| Action | Override | Creates |
|---|---|---|
| `DocumentTypeAction` | `Alias`, `DisplayName`, `Icon` | Umbraco document type (container sets its own allowed children) |
| `ContentNodeAction` | `NodeName`, `ContentTypeAlias`, `ParentId` | Content node published at root or under a parent, with U13/U17 publishing |
| `DataTypeAction` | `DataTypeName`, `EditorAlias` | Umbraco data type (uses `VoidEditor`, compatible with both U13 and U17) |
| `TemplateAction` | `TemplateAlias`, `TemplateName`, `Content` | Razor template with content, created via `ITemplateService` (U17) or `IFileService` (U13) |
| `PermissionsAction` | `GroupName`, `PermissionChars`, `ContentId` | User-group content permissions, via `IUserGroupService` (U17) or `IUserService` (U13) |

All actions implement `IPackageAction` with a `Name` and `ExecuteAsync(CancellationToken)`.

### PackageActionRunner

Scans an assembly for all `IPackageAction` implementations registered in the DI container and runs them in order.

```csharp
var runner = new PackageActionRunner(serviceProvider, logger);
await runner.RunAllAsync(Assembly.GetExecutingAssembly());
```

## Configuration

No `appsettings.json` configuration. All behavior is configured via abstract member overrides in your derived classes.

## DI registration

Register your action classes and the runner in a Composer:

```csharp
public class SetupComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<IPackageAction, SeoDocumentTypeAction>();
        builder.Services.AddTransient<IPackageAction, BlogDataTypeAction>();
        builder.Services.AddTransient<IPackageAction, HomePageTemplateAction>();
        builder.Services.AddTransient<PackageActionRunner>();
    }
}
```

## Usage

### Creating a document type action

```csharp
public class SeoDocumentTypeAction : DocumentTypeAction
{
    public SeoDocumentTypeAction(IContentTypeService contentTypeService)
        : base(contentTypeService) { }

    public override string Name => "SEO Document Type Setup";
    protected override string Alias => "seoPage";
    protected override string DisplayName => "SEO Page";
    protected override string Icon => "icon-globe";
}
```

### Creating a content node action

```csharp
public class HomePageNodeAction : ContentNodeAction
{
    public HomePageNodeAction(
        IContentService contentService,
        IContentPublishingService publishingService,
        IContentTypeService contentTypeService,
        ILogger<ContentNodeAction> logger)
        : base(
            contentService,
            publishingService,
            contentTypeService,
            logger) { }

    public override string Name => "Home Page Setup";
    protected override string NodeName => "Home";
    protected override string ContentTypeAlias => "homePage";
    protected override int ParentId => -1; // root
}
```

### Running all actions on startup

```csharp
public class InstallComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<PackageActionRunner>();

        builder.Components().Append<InstallComponent>();
    }
}

public class InstallComponent : IComponent
{
    private readonly PackageActionRunner _runner;

    public InstallComponent(PackageActionRunner runner) => _runner = runner;

    public void Initialize() =>
        _runner.RunAllAsync(Assembly.GetExecutingAssembly()).GetAwaiter().GetResult();

    public void Terminate() { }
}
```

---

**SplatDev.Umbraco.Tools.PackageActions** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
